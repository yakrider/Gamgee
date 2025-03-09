using System;
using System.Runtime.InteropServices;
using MathNet.Numerics.Optimization;

namespace Gamma_Manager
{
    internal class Gamma
    {
        private static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static ushort[,] CreateGammaRamp(float rGamma, float gGamma, float bGamma, float rContrast, float gContrast, float bContrast, float rBright, float gBright, float bBright)
        {
            //Gamma check
            const float MaxGamma = 4.4f;
            const float MinGamma = 0.3f;
            rGamma = Clamp(rGamma, MinGamma, MaxGamma);
            gGamma = Clamp(gGamma, MinGamma, MaxGamma);
            bGamma = Clamp(bGamma, MinGamma, MaxGamma);

            //Contrast check 
            const float MaxContrast = 100.0f;
            const float MinContrast = 0.1f;
            rContrast = Clamp(rContrast, MinContrast, MaxContrast);
            gContrast = Clamp(gContrast, MinContrast, MaxContrast);
            bContrast = Clamp(bContrast, MinContrast, MaxContrast);

            //Brightness check
            const float MaxBright = 1.0f;
            const float MinBright = -1.0f;
            rBright = Clamp(rBright, MinBright, MaxBright);
            gBright = Clamp(gBright, MinBright, MaxBright);
            bBright = Clamp(bBright, MinBright, MaxBright);

            //Auxiliary parameters
            double rInvgamma = 1 / rGamma;
            double gInvgamma = 1 / gGamma;
            double bInvgamma = 1 / bGamma;
            double rNorm = Math.Pow(255.0, rInvgamma - 1);
            double gNorm = Math.Pow(255.0, gInvgamma - 1);
            double bNorm = Math.Pow(255.0, bInvgamma - 1);

            ushort[,] newGamma = new ushort[3, 256];

            for (int i = 0; i < 256; i++)
            {
                double rVal = i * rContrast - (rContrast - 1) * 127;
                double gVal = i * gContrast - (gContrast - 1) * 127;
                double bVal = i * bContrast - (bContrast - 1) * 127;

                if (rGamma != 1) rVal = Math.Pow(rVal, rInvgamma) / rNorm;
                if (gGamma != 1) gVal = Math.Pow(gVal, gInvgamma) / gNorm;
                if (bGamma != 1) bVal = Math.Pow(bVal, bInvgamma) / bNorm;

                rVal += rBright * 128;
                gVal += gBright * 128;
                bVal += bBright * 128;

                newGamma[0, i] = (ushort)Clamp((int)(rVal * 256), 0, 65535); // r
                newGamma[1, i] = (ushort)Clamp((int)(gVal * 256), 0, 65535); // g
                newGamma[2, i] = (ushort)Clamp((int)(bVal * 256), 0, 65535); // b
            }
            return newGamma;
        }

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, IntPtr lpInitData);
        [DllImport("gdi32.dll")]
        private static extern bool SetDeviceGammaRamp(IntPtr hdc, ushort[,] ramp);
        [DllImport("gdi32.dll")]
        private static extern bool GetDeviceGammaRamp(IntPtr hdc, ushort[,] lpRamp);



        public static void SetGammaRamp(string display_dc, ushort[,] newGammaArray)
        {
            IntPtr hDC = CreateDC(null, display_dc, null, IntPtr.Zero);
            SetDeviceGammaRamp(hDC, newGammaArray);
        }

        public static ushort[,] GetGammaRamp(string display_dc)
        {
            ushort[,] curGamma = new ushort[3, 256];
            IntPtr hDC = CreateDC(null, display_dc, null, IntPtr.Zero);
            GetDeviceGammaRamp(hDC, curGamma);
            return curGamma;
        }



        public static double[] InverseGammaRamp (ushort[,] obtainedGammaRamp)
        {
            //var roughGuess = new [] {1.0, 1.0, 0.0};
            var roughGuess = InverseGammaRamp_Grid(obtainedGammaRamp);
            var tunedGuess = InverseGammaRamp_NelderMead(obtainedGammaRamp, roughGuess);
            return tunedGuess;
        }


        private static double[] InverseGammaRamp_NelderMead (ushort[,] obtainedGammaRamp, double[] initialGuess)
        {
            Func<MathNet.Numerics.LinearAlgebra.Vector<double>, double> lossFunction = parameters => {
                var calculatedRamp = CreateGammaRamp (
                    (float)parameters[0], (float)parameters[0], (float)parameters[1],
                    (float)parameters[1], (float)parameters[1], (float)parameters[1],
                    (float)parameters[2], (float)parameters[2], (float)parameters[2]
                );
                double loss = 0.0;
                for (int i = 0; i < 256; i++)
                {
                    loss += Math.Pow(calculatedRamp[0, i] - obtainedGammaRamp[0, i], 2);
                    loss += Math.Pow(calculatedRamp[1, i] - obtainedGammaRamp[1, i], 2);
                    loss += Math.Pow(calculatedRamp[2, i] - obtainedGammaRamp[2, i], 2);
                }
                return loss;
            };
            // Use an NelderMeadSimplex optimization algorithm to minimize the loss function
            // (all the others require gradient, which we dont have in this wildly non-linear gamma ramp gen)
            var optimizer = new NelderMeadSimplex(0.005, 1000);
            var _initialGuess = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense (initialGuess);
            var result = optimizer.FindMinimum(ObjectiveFunction.Value(lossFunction), _initialGuess);
            return result.MinimizingPoint.ToArray();
        }


        private static double[] InverseGammaRamp_Grid (ushort[,] obtainedGammaRamp)
        {
            // Define the parameter ranges for the grid search
            var gammaRange = GenerateRange(0.4, 3.0, 0.075);
            var contrastRange = GenerateRange(0.4, 2.0, 0.075);
            var brightnessRange = GenerateRange(-1.0, 1.0, 0.075);

            var minLoss = double.MaxValue;
            var bestParams = new double[3];

            foreach (var gamma in gammaRange)
            {
                foreach (var contrast in contrastRange)
                {
                    foreach (var brightness in brightnessRange)
                    {
                        var calculatedRamp = CreateGammaRamp (
                            (float)gamma, (float)gamma, (float)gamma,
                            (float)contrast, (float)contrast, (float)contrast,
                            (float)brightness, (float)brightness, (float)brightness
                        );
                        double loss = 0.0;
                        for (int i = 0; i < 256; i++)
                        {
                            loss += Math.Pow(calculatedRamp[0, i] - obtainedGammaRamp[0, i], 2);
                            loss += Math.Pow(calculatedRamp[1, i] - obtainedGammaRamp[1, i], 2);
                            loss += Math.Pow(calculatedRamp[2, i] - obtainedGammaRamp[2, i], 2);
                        }
                        if (loss < minLoss)
                        {
                            minLoss = loss;
                            bestParams = new [] { gamma, contrast, brightness };
                        }
                    }
                }
            }
            return bestParams;
        }

        private static double[] GenerateRange (double start, double end, double step)
        {
            var count = (int)((end - start) / step) + 1;
            var range = new double[count];
            for (var i = 0; i < count; i++)
            {
                range[i] = start + i * step;
            }
            return range;
        }

    }
}
