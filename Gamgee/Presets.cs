using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Gamgee;

public class Presets
{
    private readonly string iniPath;
    private readonly string exeName;

    public Presets(string iniPath = null)
    {
        exeName = Assembly.GetExecutingAssembly().GetName().Name;
        this.iniPath = new FileInfo(iniPath ?? exeName + ".ini").FullName;
    }

    private string Read(string key, string section = null)
    {
        var retVal = new StringBuilder(255);
        GetPrivateProfileString(section ?? exeName, key, "", retVal, 255, iniPath);
        return retVal.ToString();
    }

    private string[] GetSections()
    {
        uint maxBuffer = 32767;
        IntPtr pReturnedString = Marshal.AllocCoTaskMem((int)maxBuffer);
        var bytesReturned = GetPrivateProfileSectionNames(pReturnedString, maxBuffer, iniPath);
        if (bytesReturned == 0) return [];
            
        var local = Marshal.PtrToStringAnsi(pReturnedString, (int)bytesReturned);
        Marshal.FreeCoTaskMem(pReturnedString);
        return local.Substring(0, local.Length - 1).Split('\0');
    }

    private void Write(string key, string value, string section = null)
    {
        WritePrivateProfileString(section ?? exeName, key, value, iniPath);
    }

    private void DeleteSection(string section = null)
    {
        Write(null, null, section ?? exeName);
    }

    // Preset-specific functionality
    public List<string> GetPresetsForDisplay(string displayName)
    {
        var presets = new List<string>();
        foreach (var section in GetSections() ?? Array.Empty<string>())
        {
            if (Read("monitor", section).Equals(displayName))
            {
                // Extract just the preset name part (before the __monitor_name if it exists)
                var presetName = section.Contains("__") ?
                    section.Substring(0, section.IndexOf("__")) :
                    section;
                presets.Add(presetName);
            }
        }
        return presets;
    }

    public void SavePreset(string presetName, string displayName, RampGbc rampGbc, 
        bool colorTempEnabled, bool colorTempBlendEnabled, int colorTemp,
        bool overlayEnabled, bool overlayEnforced, float ovTransparency,
        System.Globalization.CultureInfo culture)
    {
        var sectionName = $"{presetName}__{displayName}";
        
        // Save monitor name for this preset
        Write("monitor", displayName, sectionName);
        
        // Save gamma, brightness, and contrast settings
        Write("rGamma", rampGbc.Gamma.Red.ToString("0.00", culture), sectionName);
        Write("gGamma", rampGbc.Gamma.Green.ToString("0.00", culture), sectionName);
        Write("bGamma", rampGbc.Gamma.Blue.ToString("0.00", culture), sectionName);
        Write("rBright", rampGbc.Bright.Red.ToString("0.00", culture), sectionName);
        Write("gBright", rampGbc.Bright.Green.ToString("0.00", culture), sectionName);
        Write("bBright", rampGbc.Bright.Blue.ToString("0.00", culture), sectionName);
        Write("rContrast", rampGbc.Contrast.Red.ToString("0.00", culture), sectionName);
        Write("gContrast", rampGbc.Contrast.Green.ToString("0.00", culture), sectionName);
        Write("bContrast", rampGbc.Contrast.Blue.ToString("0.00", culture), sectionName);
        
        // Save color temperature settings
        Write("colorTempEnabled", colorTempEnabled.ToString(), sectionName);
        Write("colorTempBlendEnabled", colorTempBlendEnabled.ToString(), sectionName);
        Write("colorTemp", colorTemp.ToString("0", culture), sectionName);
        
        // Save overlay settings
        Write("overlayEnabled", overlayEnabled.ToString(), sectionName);
        Write("overlayEnforced", overlayEnforced.ToString(), sectionName);
        Write("ovTransparency", ovTransparency.ToString("0.00", culture), sectionName);
    }

    public bool LoadPreset(string presetName, string displayName, out RampGbc rampGbc,
        out bool colorTempEnabled, out bool colorTempBlendEnabled, out int colorTemp,
        out bool overlayEnabled, out bool overlayEnforced, out float ovTransparency,
        System.Globalization.CultureInfo culture)
    {
        var sectionName = $"{presetName}__{displayName}";
        
        // Initialize out parameters with defaults
        rampGbc = RampGbc.GetDefaultRampGbc();
        colorTempEnabled = false;
        colorTempBlendEnabled = false;
        colorTemp = 6500;
        overlayEnabled = false;
        overlayEnforced = false;
        ovTransparency = 1.0f;

        // Read gamma, brightness, and contrast settings
        var rGamma = Read("rGamma", sectionName);
        if (!string.IsNullOrEmpty(rGamma))
            rampGbc.Gamma.Red = float.Parse(rGamma, culture);
            
        var gGamma = Read("gGamma", sectionName);
        if (!string.IsNullOrEmpty(gGamma))
            rampGbc.Gamma.Green = float.Parse(gGamma, culture);
            
        var bGamma = Read("bGamma", sectionName);
        if (!string.IsNullOrEmpty(bGamma))
            rampGbc.Gamma.Blue = float.Parse(bGamma, culture);
            
        var rBright = Read("rBright", sectionName);
        if (!string.IsNullOrEmpty(rBright))
            rampGbc.Bright.Red = float.Parse(rBright, culture);
            
        var gBright = Read("gBright", sectionName);
        if (!string.IsNullOrEmpty(gBright))
            rampGbc.Bright.Green = float.Parse(gBright, culture);
            
        var bBright = Read("bBright", sectionName);
        if (!string.IsNullOrEmpty(bBright))
            rampGbc.Bright.Blue = float.Parse(bBright, culture);
            
        var rContrast = Read("rContrast", sectionName);
        if (!string.IsNullOrEmpty(rContrast))
            rampGbc.Contrast.Red = float.Parse(rContrast, culture);
            
        var gContrast = Read("gContrast", sectionName);
        if (!string.IsNullOrEmpty(gContrast))
            rampGbc.Contrast.Green = float.Parse(gContrast, culture);
            
        var bContrast = Read("bContrast", sectionName);
        if (!string.IsNullOrEmpty(bContrast))
            rampGbc.Contrast.Blue = float.Parse(bContrast, culture);

        // Read color temperature settings
        var colorTempEnabledStr = Read("colorTempEnabled", sectionName);
        if (!string.IsNullOrEmpty(colorTempEnabledStr))
        {
            colorTempEnabled = bool.Parse(colorTempEnabledStr);
            
            var colorTempBlendEnabledStr = Read("colorTempBlendEnabled", sectionName);
            if (!string.IsNullOrEmpty(colorTempBlendEnabledStr))
                colorTempBlendEnabled = bool.Parse(colorTempBlendEnabledStr);
            
            var colorTempStr = Read("colorTemp", sectionName);
            if (!string.IsNullOrEmpty(colorTempStr))
                colorTemp = int.Parse(colorTempStr, culture);
        }

        // Read overlay settings
        var overlayEnabledStr = Read("overlayEnabled", sectionName);
        if (!string.IsNullOrEmpty(overlayEnabledStr))
        {
            overlayEnabled = bool.Parse(overlayEnabledStr);
            
            var overlayEnforcedStr = Read("overlayEnforced", sectionName);
            if (!string.IsNullOrEmpty(overlayEnforcedStr))
                overlayEnforced = bool.Parse(overlayEnforcedStr);
            
            var ovTransparencyStr = Read("ovTransparency", sectionName);
            if (!string.IsNullOrEmpty(ovTransparencyStr))
                ovTransparency = float.Parse(ovTransparencyStr, culture);
        }

        return true;
    }

    public void DeletePreset(string presetName, string displayName)
    {
        var sectionName = $"{presetName}__{displayName}";
        DeleteSection(sectionName);
    }

    // Active preset management
    private const string ACTIVE_PRESETS_SECTION = "ActivePresets";

    public void SaveActivePreset(string displayName, string presetName)
    {
        Write(displayName, presetName, ACTIVE_PRESETS_SECTION);
    }

    public string GetActivePreset(string displayName)
    {
        return Read(displayName, ACTIVE_PRESETS_SECTION);
    }

    // Hotkey configuration
    private const string HOTKEY_SECTION = "Hotkey";

    public (string key, string modifiers) GetHotkeyConfig()
    {
        return (Read("Key", HOTKEY_SECTION), Read("Modifiers", HOTKEY_SECTION));
    }

    public void SaveHotkeyConfig(string key, string modifiers)
    {
        Write("Key", key, HOTKEY_SECTION);
        Write("Modifiers", modifiers, HOTKEY_SECTION);
    }






    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    private static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

    [DllImport("kernel32", CharSet = CharSet.Unicode)]
    private static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

    [DllImport("kernel32")]
    private static extern uint GetPrivateProfileSectionNames(IntPtr pszReturnBuffer, uint nSize, string lpFileName);



} 
