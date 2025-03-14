using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Optimization;

namespace Gamma_Manager;



public struct RampGbcRgb (float red, float green, float blue)
{
    public float Red = red;
    public float Green = green;
    public float Blue = blue;

    private float mean() {
        return (Red + Green + Blue) / 3;
    }
    public float get (Window.GammaSrcColor src)
    {
        return src switch {
            Window.GammaSrcColor.Red => Red,
            Window.GammaSrcColor.Green => Green,
            Window.GammaSrcColor.Blue => Blue,
            _ => mean()
        };
    }
    public void set (Window.GammaSrcColor src, float v)
    {
        if      (src == Window.GammaSrcColor.Red)   { Red = v; }
        else if (src == Window.GammaSrcColor.Green) { Green = v; }
        else if (src == Window.GammaSrcColor.Blue)  { Blue = v; }
        else { Red = Green = Blue = v; }
    }

}



public struct RampGbc (RampGbcRgb gamma, RampGbcRgb bright, RampGbcRgb contrast)
{
    public RampGbcRgb Gamma = gamma;
    public RampGbcRgb Bright = bright;
    public RampGbcRgb Contrast = contrast;

    public void ApplyToDisplay (string displayDc)
    {
        var ramp = Gamma_Manager.Gamma.CreateGammaRamp(this);
        Gamma_Manager.Gamma.SetGammaRamp(displayDc, ramp);
    }

    public static RampGbc GetDefaultRampGbc()
    {
        return GetRampGbc_Gray(1, 0, 1);
    }
    public static RampGbc GetRampGbc_Gray (float gamma, float bright, float contrast)
    {
        var gbc_g = new RampGbcRgb(gamma, gamma, gamma);
        var gbc_b = new RampGbcRgb(bright, bright, bright);
        var gbc_c = new RampGbcRgb(contrast, contrast, contrast);
        return new RampGbc(gbc_g, gbc_b, gbc_c);
    }
}




internal class Gamma
{

    public static void SetGammaRamp(string displayDc, ushort[,] ramp)
    {
        var dc = CreateDC(null, displayDc, null, IntPtr.Zero);
        SetDeviceGammaRamp(dc, ramp);
        DeleteDC(dc);
    }
    public static ushort[,] GetGammaRamp(string displayDc)
    {
        var curGamma = new ushort[3, 256];
        var dc = CreateDC(null, displayDc, null, IntPtr.Zero);
        GetDeviceGammaRamp (dc, curGamma);
        DeleteDC(dc);
        return curGamma;
    }

    public static void ResetGammaRamp(string displayDc)
    {
        SetGammaRamp (displayDc, CreateGammaRamp (RampGbc.GetDefaultRampGbc()));
    }


    public static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
    {
        if (val.CompareTo(min) < 0) return min;
        if (val.CompareTo(max) > 0) return max;
        return val;
    }
    public static ushort[,] CreateGammaRamp (RampGbc gbc) {

        //Gamma check
        const float maxGamma = 4.4f;
        const float minGamma = 0.3f;
        var rGamma = Clamp (gbc.Gamma.Red,   minGamma, maxGamma);
        var gGamma = Clamp (gbc.Gamma.Green, minGamma, maxGamma);
        var bGamma = Clamp (gbc.Gamma.Blue,  minGamma, maxGamma);

        //Bright check
        const float maxBright = 1.0f;
        const float minBright = -1.0f;
        var rBright = Clamp(gbc.Bright.Red,   minBright, maxBright);
        var gBright = Clamp(gbc.Bright.Green, minBright, maxBright);
        var bBright = Clamp(gbc.Bright.Blue,  minBright, maxBright);

        //Contrast check
        const float maxContrast = 100.0f;
        const float minContrast = 0.1f;
        var rContrast = Clamp(gbc.Contrast.Red,   minContrast, maxContrast);
        var gContrast = Clamp(gbc.Contrast.Green, minContrast, maxContrast);
        var bContrast = Clamp(gbc.Contrast.Blue,  minContrast, maxContrast);

        //Auxiliary parameters
        var rInvgamma = 1.0 / rGamma;
        var gInvgamma = 1.0 / gGamma;
        var bInvgamma = 1.0 / bGamma;
        var rNorm = Math.Pow(255.0, rInvgamma - 1);
        var gNorm = Math.Pow(255.0, gInvgamma - 1);
        var bNorm = Math.Pow(255.0, bInvgamma - 1);

        var ramp = new ushort[3, 256];

        const float TOLERANCE = 1e-6f;
        for (var i = 0; i < 256; i++)
        {
            double rVal = i * rContrast - (rContrast - 1) * 127;
            double gVal = i * gContrast - (gContrast - 1) * 127;
            double bVal = i * bContrast - (bContrast - 1) * 127;

            if (Math.Abs(rGamma - 1) > TOLERANCE) rVal = Math.Pow(rVal, rInvgamma) / rNorm;
            if (Math.Abs(gGamma - 1) > TOLERANCE) gVal = Math.Pow(gVal, gInvgamma) / gNorm;
            if (Math.Abs(bGamma - 1) > TOLERANCE) bVal = Math.Pow(bVal, bInvgamma) / bNorm;

            rVal += rBright * 128;
            gVal += gBright * 128;
            bVal += bBright * 128;

            ramp[0, i] = (ushort)Clamp((int)(rVal * 256), 0, 65535); // r
            ramp[1, i] = (ushort)Clamp((int)(gVal * 256), 0, 65535); // g
            ramp[2, i] = (ushort)Clamp((int)(bVal * 256), 0, 65535); // b
        }
        return ramp;
    }




    public static float[] InverseGammaRamp (ushort[,] obtainedGammaRamp)
    {
        //var gbcGuess = new [] {1.0, 0.0, 1.0};
        var gbcGuess = InverseGammaRamp_Grid(obtainedGammaRamp) .Select(d => (double)d).ToArray();
        var gbcTuned = InverseGammaRamp_NelderMead(obtainedGammaRamp, gbcGuess);
        return gbcTuned.Select(d => (float)d).ToArray();
    }

    private static double[] InverseGammaRamp_NelderMead (ushort[,] obtainedGammaRamp, double[] initialGuess)
    {
        // Use an NelderMeadSimplex optimization algorithm to minimize the loss function
        // (all the others require gradient, which we dont have in this wildly non-linear gamma ramp gen)
        var optimizer = new NelderMeadSimplex(0.005, 500);
        var _initialGuess = Vector<double>.Build.Dense (initialGuess);
        var result = optimizer.FindMinimum(ObjectiveFunction.Value(LossFunction), _initialGuess);
        return result.MinimizingPoint.ToArray();

        double LossFunction (Vector<double> gbc)
        {
            var calculatedRamp = CreateGammaRamp ( RampGbc.GetRampGbc_Gray (
                (float)gbc[0], (float)gbc[1], (float)gbc[2]
            ) );
            var loss = 0.0d;
            for (var i = 0; i < 256; i++)
            {
                loss += Math.Pow(calculatedRamp[0, i] - obtainedGammaRamp[0, i], 2);
                loss += Math.Pow(calculatedRamp[1, i] - obtainedGammaRamp[1, i], 2);
                loss += Math.Pow(calculatedRamp[2, i] - obtainedGammaRamp[2, i], 2);
            }
            return loss;
        }
    }

    private static float[] InverseGammaRamp_Grid(ushort[,] obtainedGammaRamp)
    {
        // Define the parameter ranges for the grid search
        var gammaRange = GenerateRange(0.4f, 3.0f, 0.1f);
        var brightRange = GenerateRange(-1.0f, 1.0f, 0.1f);
        var contrastRange = GenerateRange(0.4f, 2.0f, 0.1f);

        var minLoss = double.MaxValue;
        var bestParams = new float[3];
        var lockObject = new object();

        var parOpts = new ParallelOptions { MaxDegreeOfParallelism = 8 };
        Parallel.ForEach (gammaRange, parOpts, gamma => {
            Parallel.ForEach (brightRange, parOpts, bright => {
                Parallel.ForEach (contrastRange, parOpts, contrast => {
                    var calculatedRamp = CreateGammaRamp (RampGbc.GetRampGbc_Gray (gamma, bright, contrast));
                    var loss = 0.0d;
                    for (var i = 0; i < 256; i++)
                    {
                        loss += Math.Pow(calculatedRamp[0, i] - obtainedGammaRamp[0, i], 2);
                        loss += Math.Pow(calculatedRamp[1, i] - obtainedGammaRamp[1, i], 2);
                        loss += Math.Pow(calculatedRamp[2, i] - obtainedGammaRamp[2, i], 2);
                    }
                    lock (lockObject) {
                        if (loss < minLoss) {
                            minLoss = loss;
                            bestParams = [gamma, bright, contrast];
                        }
                    }
                } );
            } );
        } );
        return bestParams;
    }
    private static float[] GenerateRange (float start, float end, float step)
    {
        var count = (int)((end - start) / step) + 1;
        var range = new float[count];
        for (var i = 0; i < count; i++)
        {
            range[i] = start + i * step;
        }
        return range;
    }




    public static void SetGammaRamp_ColorTemp (string displayDc, int colorTemp)
    {
        var temp = Clamp(colorTemp, 2000, 9000);
        var (red, green, blue) = ColorTemperatureToRgb(temp);

        var ramp = new ushort[3, 256];
        for (var i = 0; i < 256; i++)
        {
            var brightness = i * 256.0f;
            ramp [0, i] = (ushort)Math.Max(0.0f, Math.Min(65535.0f, brightness * red));
            ramp [1, i] = (ushort)Math.Max(0.0f, Math.Min(65535.0f, brightness * green));
            ramp [2, i] = (ushort)Math.Max(0.0f, Math.Min(65535.0f, brightness * blue));
        }
        SetGammaRamp (displayDc, ramp);
    }

    private static (float red, float green, float blue) ColorTemperatureToRgb (int kelvin)
    {
        float red, green, blue;

        kelvin /= 100;

        if (kelvin <= 66) {
            red = 255;
        } else {
            red = kelvin - 60.0f;
            red = (float)(329.698727446 * Math.Pow(red, -0.1332047592));
            red = Math.Max(0.0f, Math.Min(255.0f, red));
        }

        if (kelvin <= 66) {
            green = kelvin;
            green = 99.4708025861f * (float)Math.Log(green) - 161.1195681661f;
            green = Math.Max(0.0f, Math.Min(255.0f, green));
        } else {
            green = kelvin - 60.0f;
            green = (float)(288.1221695283 * Math.Pow(green, -0.0755148492));
            green = Math.Max(0.0f, Math.Min(255.0f, green));
        }

        if (kelvin >= 66) {
            blue = 255.0f;
        } else {
            blue = kelvin - 10.0f;
            blue = 138.5177312231f * (float)Math.Log(blue) - 305.0447927307f;
            blue = Math.Max(0.0f, Math.Min(255.0f, blue));
        }

        red /= 255.0f;
        green /= 255.0f;
        blue /= 255.0f;

        return (red, green, blue);
    }


    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, IntPtr lpInitData);
    [DllImport("gdi32.dll")]
    private static extern bool DeleteDC(IntPtr hdc);
    [DllImport("gdi32.dll")]
    private static extern bool SetDeviceGammaRamp(IntPtr hdc, ushort[,] ramp);
    [DllImport("gdi32.dll")]
    private static extern bool GetDeviceGammaRamp(IntPtr hdc, ushort[,] lpRamp);


}
