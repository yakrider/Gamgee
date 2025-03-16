using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Gamgee;


public partial class Window : Form
{
    // Constants for hotkey registration
    private const int HOTKEY_ID = 9000;
    private const int MOD_ALT = 0x0001;
    private const int MOD_CONTROL = 0x0002;
    private const int MOD_SHIFT = 0x0004;
    private const int WM_HOTKEY = 0x0312;

    // Default hotkey configuration
    private const Keys DEFAULT_HOTKEY_KEY = Keys.G;
    private const int DEFAULT_HOTKEY_MODIFIERS = MOD_ALT | MOD_SHIFT;

    // P/Invoke declarations for hotkey functionality
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    public enum GammaSrcColor
    {
        Combined = 0,
        Red,
        Green,
        Blue
    }
    public enum GBC
    {
        Gamma = 0,
        Bright,
        Contrast
    }


    System.Globalization.CultureInfo customCulture;
    IniFile iniFile;
    List<DisplayInfo> displays = [];
    DisplayInfo curDisp;
    int numDisplay;

    private bool disableChangeFunc = false;
    private GammaSrcColor gammaSrc = GammaSrcColor.Combined;

    public static bool gaammaErrState = false;
    // ^^ ideally these should be monitor specific ..
    // .. but for now we'll make it static here and query them directly from within trackbars


    private static readonly Color BackgroundColor = Color.FromArgb(30,30,30);
    private static readonly Color ForegroundColor = Color.White;
    private static readonly Color ButtonColor = Color.FromArgb(60,60,60);
    private static readonly Color ButtonBorderColor = Color.FromArgb(120, 120, 120);
    private static readonly Color ButtonTextColor = Color.White;
    private static readonly Color BorderColor = Color.BurlyWood;



    private void initPresets()
    {
        comboBoxPresets.Items.Clear();
        comboBoxPresets.Text = string.Empty;
        foreach (var preset in iniFile.GetSections())
        {
            if (iniFile.Read("monitor", preset).Equals(curDisp.displayName))
            {
                // Extract just the preset name part (before the __monitor_name if it exists)
                string displayName = preset.Contains("__") ? preset.Substring(0, preset.IndexOf("__")) : preset;
                comboBoxPresets.Items.Add(displayName);
            }
        }
    }





    private void RenderCurInfo_ColorBtns()
    {
        void setFontStyle(Button btn, GammaSrcColor boldSrc)
        {
            var style = gammaSrc == boldSrc ? FontStyle.Bold : FontStyle.Regular;
            btn.Font = new Font(btn.Font.Name, btn.Font.Size, style);
            // .. could try to color font here, but the Red/Green/Blue picks dont look good in dark, esp for the blue
        }
        setFontStyle (buttonRed, GammaSrcColor.Red);
        setFontStyle (buttonGreen, GammaSrcColor.Green);
        setFontStyle (buttonBlue, GammaSrcColor.Blue);
        setFontStyle (buttonAllColors, GammaSrcColor.Combined);
    }


    private void ResetInfo_GBC()
    {
        gammaSrc = GammaSrcColor.Combined;
        curDisp.ramp_gbc = RampGbc.GetDefaultRampGbc();
        Gamma.ResetGammaRamp(curDisp.displayLink);
        RenderCurInfo_ColorBtns();
        RenderCurInfo_GBC();
    }
    private void RenderCurInfo_GBC()
    {
        disableChangeFunc = true;

        var (gamma, bright, contrast) = (
            curDisp.ramp_gbc.Gamma.get(gammaSrc),
            curDisp.ramp_gbc.Bright.get(gammaSrc),
            curDisp.ramp_gbc.Contrast.get(gammaSrc)
        );
        trackBarGamma.Value = (int)(100f * gamma);
        textBoxGamma.Text = gamma.ToString("0.00");

        trackBarBright.Value = (int)(100f * bright);
        textBoxBright.Text = bright.ToString("0.00");

        trackBarContrast.Value = (int)(100f * contrast);
        textBoxContrast.Text = contrast.ToString("0.00");

        disableChangeFunc = false;
    }


    private void ResetInfo_ColorTemp()
    {
        curDisp.colorTempEnabled = false;
        curDisp.colorTemp = 6500;
        RenderCurInfo_ColorTemp();
        RenderTrackBars_FollowerInfo();
    }
    private void RenderCurInfo_ColorTemp()
    {
        if (displays.Count <= 0) return;
        disableChangeFunc = true;
        checkBoxColorTemp.Checked = curDisp.colorTempEnabled;
        checkBoxColorTempBlend.Checked = curDisp.colorTempBlendEnabled;
        trackBarColorTemp.Value = (int)(curDisp.colorTemp / 100f);
        textBoxColorTemp.Text = (curDisp.colorTemp / 1000f).ToString("0.00");
        disableChangeFunc = false;
    }


    private void RenderTrackBars_FollowerInfo()
    {
        trackBarColorTemp.IsFollower = !curDisp.colorTempEnabled;
        trackBarGamma.IsFollower = trackBarBright.IsFollower = trackBarContrast.IsFollower =
            (curDisp.colorTempEnabled && !curDisp.colorTempBlendEnabled);
        InvalidateTrackBars();
    }
    private void InvalidateTrackBars()
    {
        trackBarGamma.Invalidate();
        trackBarBright.Invalidate();
        trackBarContrast.Invalidate();
        trackBarColorTemp.Invalidate();
    }


    private void ResetInfo_Overlay()
    {
        curDisp.ovTransparency = 1.0f;
        curDisp.overlayEnforced = false;
        curDisp.overlay.Update();
        RenderCurInfo_Overlay();;
    }
    private void RenderCurInfo_Overlay()
    {
        if (displays.Count <= 0) return;
        disableChangeFunc = true;
        
        checkBoxOverlay.Checked = curDisp.overlayEnabled;
        checkBoxOverlayEnforced.Checked = curDisp.overlayEnforced;
        checkBoxOverlayEnforced.Enabled = curDisp.overlayEnabled;
        
        trackBarOverlay.Value = (int)(100f * curDisp.ovTransparency);
        trackBarOverlay.IsFollower = !curDisp.overlayEnabled;
        trackBarOverlay.Invalidate();
        
        textBoxOverlay.Text = curDisp.ovTransparency.ToString("0.00");
        
        disableChangeFunc = false;
    }


    private void ResetInfo_Monitors()
    {
        // here we could directly update/reset monitor values
        // (but typically we should seldom want to do so)
        trackBarMonitorBright.Value = 100;
        if (curDisp.isExternal) {
            trackBarMonitorContrast.Value = 50;
        }
    }
    private void RenderCurInfo_Monitors()
    {
        disableChangeFunc = true;

        trackBarMonitorBright.Value = curDisp.isExternal ?
            Display.External.GetBrightness(curDisp.PhysicalHandle) : Display.Internal.GetBrightness();
        textBoxMonitorBright.Text  = trackBarMonitorBright.Value.ToString();

        var numDispStr = (1 + curDisp.numDisplay).ToString("0");
        labelMonitorBright.Text   = numDispStr + ": Bright";
        labelMonitorContrast.Text = numDispStr + ": Contrast";

        if (curDisp.isExternal) {
            trackBarMonitorContrast.Enabled = textBoxMonitorContrast.Enabled = true;
            trackBarMonitorContrast.Value = Display.External.GetContrast(curDisp.PhysicalHandle);
        } else {
            trackBarMonitorContrast.Enabled = textBoxMonitorContrast.Enabled = false;
            trackBarMonitorContrast.Value = 50;
        }
        textBoxMonitorContrast.Text = trackBarMonitorContrast.Value.ToString();
        disableChangeFunc = false;
    }
    private void RegisterHotkeyFromIni()
    {
        try
        {
            // Try to read hotkey configuration from INI file
            string hotkeySection = "Hotkey";
            string keyStr = iniFile.Read("Key", hotkeySection);
            string modifiersStr = iniFile.Read("Modifiers", hotkeySection);

            // If either key or modifiers are not specified, use defaults and save them to INI
            if (string.IsNullOrEmpty(keyStr) || string.IsNullOrEmpty(modifiersStr))
            {
                RegisterHotKey(Handle, HOTKEY_ID, DEFAULT_HOTKEY_MODIFIERS, (int)DEFAULT_HOTKEY_KEY);
                // Save the default configuration to the INI file for future reference
                SaveHotkeyToIni(DEFAULT_HOTKEY_KEY, DEFAULT_HOTKEY_MODIFIERS);
                return;
            }

            // Parse the key
            if (!Enum.TryParse(keyStr, true, out Keys key))
            {
                RegisterHotKey(Handle, HOTKEY_ID, DEFAULT_HOTKEY_MODIFIERS, (int)DEFAULT_HOTKEY_KEY);
                // Save the default configuration to the INI file for future reference
                SaveHotkeyToIni(DEFAULT_HOTKEY_KEY, DEFAULT_HOTKEY_MODIFIERS);
                return;
            }

            // Parse the modifiers
            int modifiers = 0;
            string[] modifierArray = modifiersStr.Split(',');
            foreach (string modifier in modifierArray)
            {
                string trimmedModifier = modifier.Trim().ToLower();
                if (trimmedModifier == "alt")
                    modifiers |= MOD_ALT;
                else if (trimmedModifier == "control" || trimmedModifier == "ctrl")
                    modifiers |= MOD_CONTROL;
                else if (trimmedModifier == "shift")
                    modifiers |= MOD_SHIFT;
            }

            // Register the hotkey
            RegisterHotKey(Handle, HOTKEY_ID, modifiers, (int)key);
        }
        catch (Exception)
        {
            // If anything goes wrong, fall back to the default hotkey
            RegisterHotKey(Handle, HOTKEY_ID, DEFAULT_HOTKEY_MODIFIERS, (int)DEFAULT_HOTKEY_KEY);
            // Save the default configuration to the INI file for future reference
            SaveHotkeyToIni(DEFAULT_HOTKEY_KEY, DEFAULT_HOTKEY_MODIFIERS);
        }
    }

    private void SaveHotkeyToIni(Keys key, int modifiers)
    {
        try
        {
            string hotkeySection = "Hotkey";
            
            // Save the key
            iniFile.Write("Key", key.ToString(), hotkeySection);
            
            // Build the modifiers string
            List<string> modifiersList = new List<string>();
            if ((modifiers & MOD_ALT) != 0)
                modifiersList.Add("Alt");
            if ((modifiers & MOD_CONTROL) != 0)
                modifiersList.Add("Control");
            if ((modifiers & MOD_SHIFT) != 0)
                modifiersList.Add("Shift");
            
            // Save the modifiers
            iniFile.Write("Modifiers", string.Join(", ", modifiersList), hotkeySection);
        }
        catch (Exception)
        {
            // Silently fail if we can't save the hotkey configuration
        }
    }

    private void ApplyCurMonitorBrightness()
    {
        // we'll just change the track-bar values, and its change listener will affect the change
        if (curDisp.isExternal) {
            trackBarMonitorBright.Value = curDisp.monitorBrightness;
            trackBarMonitorContrast.Value = curDisp.monitorContrast;
        } else {
            trackBarMonitorBright.Value = curDisp.monitorBrightness;
        }
    }




    private void ApplyCurGammaRamp()
    {
        //Gamma.SetGammaRamp (curDisp.displayLink, Gamma.CreateGammaRamp(curDisp.ramp_gbc));

        var (gbc, temp) = (curDisp.colorTempEnabled, curDisp.colorTempBlendEnabled) switch
        {
            (true,  true) => (curDisp.ramp_gbc, curDisp.colorTemp),
            (true, false) => (RampGbc.GetDefaultRampGbc(), curDisp.colorTemp),
            (false,    _) => (curDisp.ramp_gbc, 6500)
        };

        var success = Gamma.SetGammaRamp_ColorTemp_Blend (curDisp.displayLink, gbc, temp);

        if (gaammaErrState != !success) {
            gaammaErrState = !success;
            InvalidateTrackBars();
        }
        RenderCurInfo_Monitors();
    }

    private void ResyncGammaRampValues()
    {
        if (displays.Count <= 0) return;
        // we'll attempt to read gamma-ramp and reverse calc the approx values to set the track-bar
        buttonResync.Enabled = false;
        var gbc = Gamma.InverseGammaRamp (Gamma.GetGammaRamp(curDisp.displayLink));

        curDisp.ramp_gbc = new RampGbc (
            new RampGbcRgb (gbc[0], gbc[0], gbc[0]),
            new RampGbcRgb (gbc[1], gbc[1], gbc[1]),
            new RampGbcRgb (gbc[2], gbc[2], gbc[2])
        );
        RenderCurInfo_GBC();
        RenderCurInfo_ColorTemp();
        RenderCurInfo_Monitors();
        buttonResync.Enabled = true;
    }





    private void Window_Load(object sender, EventArgs e)
    {
        var wa = Screen.PrimaryScreen.WorkingArea;
        Location = new Point(wa.X + wa.Width - Width - 24, wa.Y + wa.Height - Height - 40);
    }

    public Window()
    {
        InitializeComponent();

        EnableDarkMode();

        // no tray menu, we'll just invoke our lil app window upon any tray-icon interaction
        notifyIcon.ContextMenuStrip = null;

        customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ",";

        iniFile = new IniFile("Gamgee.ini");
        
        // Register the global hotkey from INI file (or use default if not specified)
        // To configure in INI file, add a [Hotkey] section with Key=G and Modifiers=Alt, Shift
        // Example:
        // [Hotkey]
        // Key=G
        // Modifiers=Alt, Shift
        RegisterHotkeyFromIni();

        buttonAllColors.Font = new Font(buttonAllColors.Font.Name, buttonAllColors.Font.Size, FontStyle.Bold);

        displays = Display.QueryDisplayDevices();
        displays.Reverse();
        for (var i = 0; i < displays.Count; i++)
        {
            displays[i].numDisplay = i;
            comboBoxMonitors.Items.Add(i + 1 + ": " + displays[i].displayName);
        }
        curDisp = displays[numDisplay];
        comboBoxMonitors.SelectedIndex = numDisplay;

        // instead of reset, we'll try to resync to existant gamma ramp at startup
        ResetInfo_ColorTemp();

        // but the others should start cleared
        ResetInfo_Overlay();
        ResyncGammaRampValues();
        initPresets();
        
        // Load active preset if it exists
        string activePreset = GetActivePreset();
        if (!string.IsNullOrEmpty(activePreset))
        {
            // Check if the preset exists in the combobox
            if (comboBoxPresets.Items.Contains(activePreset))
            {
                comboBoxPresets.Text = activePreset;
                // The SelectedIndexChanged event will handle loading the preset
            }
        }
    }




    private void HandleTrackBarValueChanged (GBC track)
    {
        comboBoxPresets.Text = string.Empty;
        if (disableChangeFunc) return;

        switch (track)
        {
            case GBC.Gamma:
                curDisp.ramp_gbc.Gamma .set (gammaSrc, trackBarGamma.Value / 100f);
                textBoxGamma.Text = (trackBarGamma.Value / 100f).ToString("0.00");
                break;
            case GBC.Bright:
                curDisp.ramp_gbc.Bright .set (gammaSrc, trackBarBright.Value / 100f);
                textBoxBright.Text = (trackBarBright.Value / 100f).ToString("0.00");
                break;
            case GBC.Contrast:
                curDisp.ramp_gbc.Contrast .set (gammaSrc, trackBarContrast.Value / 100f);
                textBoxContrast.Text = (trackBarContrast.Value / 100f).ToString("0.00");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(track), track, null);
        }

        if (!curDisp.colorTempBlendEnabled)
            ResetInfo_ColorTemp();

        RenderCurInfo_ColorTemp();
        ApplyCurGammaRamp();
    }

    private void trackBarGamma_ValueChanged(object sender, EventArgs e)
    {
        HandleTrackBarValueChanged (GBC.Gamma);
    }
    private void trackBarBright_ValueChanged(object sender, EventArgs e)
    {
        HandleTrackBarValueChanged(GBC.Bright);
    }
    private void trackBarContrast_ValueChanged(object sender, EventArgs e)
    {
        HandleTrackBarValueChanged (GBC.Contrast);
    }




    private void trackBarMonitorBright_ValueChanged(object sender, EventArgs e)
    {
        //comboBoxPresets.Text = string.Empty;
        if (disableChangeFunc) return;

        curDisp.monitorBrightness = trackBarMonitorBright.Value;
        textBoxMonitorBright.Text = trackBarMonitorBright.Value.ToString();

        if (curDisp.isExternal) {
            Display.External.SetBrightness(curDisp.PhysicalHandle, (uint)trackBarMonitorBright.Value);
        } else {
            Display.Internal.SetBrightness((byte)trackBarMonitorBright.Value);
        }
    }
    private void trackBarMonitorContrast_ValueChanged(object sender, EventArgs e)
    {
        //comboBoxPresets.Text = string.Empty;
        if (disableChangeFunc) return;

        curDisp.monitorContrast = trackBarMonitorContrast.Value;
        textBoxMonitorContrast.Text = trackBarMonitorContrast.Value.ToString();

        if (curDisp.isExternal) {
            Display.External.SetContrast(curDisp.PhysicalHandle, (uint)trackBarMonitorContrast.Value);
        }
    }




    private void trackBarColorTemp_ValueChanged(object sender, EventArgs e)
    {
        comboBoxPresets.Text = string.Empty;
        if (disableChangeFunc) return;

        // as soon as we change color-temp slider, its enabled (and we set gbc to follower mode)
        curDisp.colorTempEnabled = true;
        checkBoxColorTemp.Checked = true;
        curDisp.colorTemp = trackBarColorTemp.Value * 100;
        textBoxColorTemp.Text = (trackBarColorTemp.Value / 10f).ToString("0.00");
        RenderTrackBars_FollowerInfo();
        ApplyCurGammaRamp();
    }
    private void checkBoxColorTemp_CheckedChanged(object sender, EventArgs e)
    {
        if (disableChangeFunc) return;
        curDisp.colorTempEnabled = checkBoxColorTemp.Checked;
        RenderTrackBars_FollowerInfo();
        ApplyCurGammaRamp();
    }
    private void checkBoxColorTempBlend_CheckedChanged(object sender, EventArgs e)
    {
        if (disableChangeFunc) return;
        curDisp.colorTempBlendEnabled = checkBoxColorTempBlend.Checked;
        RenderTrackBars_FollowerInfo();
        if (curDisp.colorTempEnabled) {
            ApplyCurGammaRamp();
        }
    }




    private void trackBarOverlay_ValueChanged(object sender, EventArgs e)
    {
        if (disableChangeFunc) return;
        // ^^ e.g. from trackbar value update upon changing monitor selection
        curDisp.ovTransparency = trackBarOverlay.Value / 100f;
        textBoxOverlay.Text = (trackBarOverlay.Value / 100f).ToString("0.00");
        curDisp.overlay.Update();
        RenderCurInfo_Monitors();
    }
    private void checkBoxOverlay_CheckedChanged(object sender, EventArgs e)
    {
        if (disableChangeFunc) return;
        curDisp.overlayEnabled  = checkBoxOverlay.Checked;
        trackBarOverlay.IsFollower = !checkBoxOverlay.Checked;
        trackBarOverlay.Invalidate();
        checkBoxOverlayEnforced.Enabled = checkBoxOverlay.Checked;
        checkBoxOverlayEnforced.Invalidate();
        curDisp.overlay.Update();
        RenderCurInfo_Monitors();
    }
    private void checkBoxOverlayEnforced_CheckedChanged(object sender, EventArgs e)
    {
        if (disableChangeFunc) return;
        curDisp.overlayEnforced = checkBoxOverlayEnforced.Checked;
        curDisp.overlay.Update();
        RenderCurInfo_Monitors();
    }





    private void buttonAllColors_Click(object sender, MouseEventArgs e)
    {
        gammaSrc = GammaSrcColor.Combined;
        RenderCurInfo_ColorBtns();
        RenderCurInfo_GBC();
    }
    private void buttonAllColors_MouseDown(object sender, MouseEventArgs e)
    {
        // rbtn click on the big btn can be used to hide window
        if (e.Button == MouseButtons.Right)
            Hide();
    }
    private void buttonRed_Click(object sender, EventArgs e)
    {
        gammaSrc = GammaSrcColor.Red;
        RenderCurInfo_ColorBtns();
        RenderCurInfo_GBC();
    }
    private void buttonGreen_Click(object sender, EventArgs e)
    {
        gammaSrc = GammaSrcColor.Green;
        RenderCurInfo_ColorBtns();
        RenderCurInfo_GBC();
    }
    private void buttonBlue_Click(object sender, EventArgs e)
    {
        gammaSrc = GammaSrcColor.Blue;
        RenderCurInfo_ColorBtns();
        RenderCurInfo_GBC();
    }



    private void buttonResync_Click(object sender, EventArgs e)
    {
        gammaSrc = GammaSrcColor.Combined;
        RenderCurInfo_ColorBtns();
        RenderCurInfo_Overlay();
        RenderCurInfo_ColorTemp();
        ResyncGammaRampValues();
    }

    private void buttonReset_Click(object sender, EventArgs e)
    {
        comboBoxPresets.Text = string.Empty;

        ResetInfo_GBC();
        ResetInfo_ColorTemp();
        ResetInfo_Overlay();
        //ResetInfo_Monitors();
        // ^^ we'd rather have nothing touch physical monitor brightness .. not even the reset click !!
        initPresets();
        
        // Clear the active preset for this monitor
        SaveActivePreset("");
    }



    private void buttonSave_Click(object sender, EventArgs e)
    {
        var presetName = comboBoxPresets.Text;
        var sectionName = presetName + "__" + curDisp.displayName;
        
        // Save monitor name for this preset
        iniFile.Write("monitor", curDisp.displayName, sectionName);
        
        // Save gamma, brightness, and contrast settings
        writeInitSectionGbc();
        
        // Note: We intentionally don't save monitor brightness/contrast settings
        // as these are physical monitor settings that should be kept separate from gamma presets
        
        // Refresh the presets list
        initPresets();
        comboBoxPresets.Text = presetName;
        
        // Save this as the active preset for this monitor
        SaveActivePreset(presetName);
    }

    private void buttonDelete_Click(object sender, EventArgs e)
    {
        string presetName = comboBoxPresets.Text;
        string fullSectionName = presetName + "__" + curDisp.displayName;
        
        // Check if this is the active preset
        string activePreset = GetActivePreset();
        if (activePreset == presetName)
        {
            // Clear the active preset since we're deleting it
            SaveActivePreset("");
        }
        
        iniFile.DeleteSection(fullSectionName);
        initPresets();
        comboBoxPresets.Text = string.Empty;
    }

    private void buttonHide_Click(object sender, EventArgs e)
    {
        Hide();
    }
    private void buttonExit_Click(object sender, EventArgs e)
    {
        // Unregister the hotkey before exiting
        UnregisterHotKey(this.Handle, HOTKEY_ID);
        Application.Exit();
    }



    private void pictureBox_Click(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            Hide();
        }
    }



    private void comboBoxMonitors_SelectedIndexChanged(object sender, EventArgs e)
    {
        var num = comboBoxMonitors.SelectedItem.ToString();

        num = num.Substring(0, num.IndexOf(":"));
        numDisplay = int.Parse(num)-1;
        curDisp = displays[numDisplay];

        // monitor changing shouldnt mean any active action, just to update UI with data for the selected monitor
        //RenderCurInfo_ColorBtns();
        RenderCurInfo_GBC();
        RenderCurInfo_Monitors();
        RenderCurInfo_Overlay();
        RenderCurInfo_ColorTemp();
        initPresets();
        
        // Load active preset for this monitor if it exists
        disableChangeFunc = true;
        comboBoxPresets.Text = string.Empty;
        disableChangeFunc = false;
        
        string activePreset = GetActivePreset();
        if (!string.IsNullOrEmpty(activePreset))
        {
            // Check if the preset exists in the combobox
            if (comboBoxPresets.Items.Contains(activePreset))
            {
                comboBoxPresets.Text = activePreset;
                // The SelectedIndexChanged event will handle loading the preset
            }
        }
    }

    private void buttonForward_Click(object sender, EventArgs e)
    {
        if (numDisplay + 1 <= displays.Count-1)
        {
            comboBoxMonitors.SelectedIndex = numDisplay + 1;
        } else
        {
            comboBoxMonitors.SelectedIndex = 0;
        }
    }

    private void comboBoxPresets_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (disableChangeFunc) return;

        // Get the full section name with monitor suffix
        string presetName = comboBoxPresets.Text;
        string fullSectionName = presetName + "__" + curDisp.displayName;

        // Read and apply gamma, brightness, and contrast settings
        curDisp.ramp_gbc = readIniSectionGbc(fullSectionName);

        // Set color source to Combined
        gammaSrc = GammaSrcColor.Combined;
        RenderCurInfo_ColorBtns();
        RenderCurInfo_GBC();

        // Read and apply color temperature settings if they exist
        string colorTempEnabledStr = iniFile.Read("colorTempEnabled", fullSectionName);
        if (!string.IsNullOrEmpty(colorTempEnabledStr))
        {
            curDisp.colorTempEnabled = bool.Parse(colorTempEnabledStr);
            
            string colorTempBlendEnabledStr = iniFile.Read("colorTempBlendEnabled", fullSectionName);
            if (!string.IsNullOrEmpty(colorTempBlendEnabledStr))
            {
                curDisp.colorTempBlendEnabled = bool.Parse(colorTempBlendEnabledStr);
            }
            
            string colorTempStr = iniFile.Read("colorTemp", fullSectionName);
            if (!string.IsNullOrEmpty(colorTempStr))
            {
                curDisp.colorTemp = int.Parse(colorTempStr, customCulture);
            }
            
            RenderCurInfo_ColorTemp();
            RenderTrackBars_FollowerInfo();
        }
        else
        {
            // If color temperature settings don't exist, reset to default
            ResetInfo_ColorTemp();
        }

        // Read and apply overlay settings if they exist
        string overlayEnabledStr = iniFile.Read("overlayEnabled", fullSectionName);
        if (!string.IsNullOrEmpty(overlayEnabledStr))
        {
            curDisp.overlayEnabled = bool.Parse(overlayEnabledStr);
            
            string overlayEnforcedStr = iniFile.Read("overlayEnforced", fullSectionName);
            if (!string.IsNullOrEmpty(overlayEnforcedStr))
            {
                curDisp.overlayEnforced = bool.Parse(overlayEnforcedStr);
            }
            
            string ovTransparencyStr = iniFile.Read("ovTransparency", fullSectionName);
            if (!string.IsNullOrEmpty(ovTransparencyStr))
            {
                curDisp.ovTransparency = float.Parse(ovTransparencyStr, customCulture);
            }
            
            RenderCurInfo_Overlay();
            curDisp.overlay.Update();
        }

        // Update monitor information display
        RenderCurInfo_Monitors();

        // Apply the gamma ramp settings
        ApplyCurGammaRamp();
        
        // Save this as the active preset for this monitor
        if (!string.IsNullOrEmpty(presetName))
        {
            SaveActivePreset(presetName);
        }
    }


    private RampGbc readIniSectionGbc(string section)
    {
        var gbc = RampGbc.GetDefaultRampGbc();
        
        // Read gamma, brightness, and contrast settings if they exist
        string rGamma = iniFile.Read("rGamma", section);
        if (!string.IsNullOrEmpty(rGamma))
            gbc.Gamma.Red = float.Parse(rGamma, customCulture);
            
        string gGamma = iniFile.Read("gGamma", section);
        if (!string.IsNullOrEmpty(gGamma))
            gbc.Gamma.Green = float.Parse(gGamma, customCulture);
            
        string bGamma = iniFile.Read("bGamma", section);
        if (!string.IsNullOrEmpty(bGamma))
            gbc.Gamma.Blue = float.Parse(bGamma, customCulture);
            
        string rBright = iniFile.Read("rBright", section);
        if (!string.IsNullOrEmpty(rBright))
            gbc.Bright.Red = float.Parse(rBright, customCulture);
            
        string gBright = iniFile.Read("gBright", section);
        if (!string.IsNullOrEmpty(gBright))
            gbc.Bright.Green = float.Parse(gBright, customCulture);
            
        string bBright = iniFile.Read("bBright", section);
        if (!string.IsNullOrEmpty(bBright))
            gbc.Bright.Blue = float.Parse(bBright, customCulture);
            
        string rContrast = iniFile.Read("rContrast", section);
        if (!string.IsNullOrEmpty(rContrast))
            gbc.Contrast.Red = float.Parse(rContrast, customCulture);
            
        string gContrast = iniFile.Read("gContrast", section);
        if (!string.IsNullOrEmpty(gContrast))
            gbc.Contrast.Green = float.Parse(gContrast, customCulture);
            
        string bContrast = iniFile.Read("bContrast", section);
        if (!string.IsNullOrEmpty(bContrast))
            gbc.Contrast.Blue = float.Parse(bContrast, customCulture);
            
        return gbc;
    }
    private void writeInitSectionGbc()
    {
        var sectionName = comboBoxPresets.Text + "__" + curDisp.displayName;
        
        // Write gamma, brightness, and contrast settings
        iniFile.Write ("rGamma",    curDisp.ramp_gbc.Gamma.Red      .ToString("0.00", customCulture), sectionName);
        iniFile.Write ("gGamma",    curDisp.ramp_gbc.Gamma.Green    .ToString("0.00", customCulture), sectionName);
        iniFile.Write ("bGamma",    curDisp.ramp_gbc.Gamma.Blue     .ToString("0.00", customCulture), sectionName);
        iniFile.Write ("rBright",   curDisp.ramp_gbc.Bright.Red     .ToString("0.00", customCulture), sectionName);
        iniFile.Write ("gBright",   curDisp.ramp_gbc.Bright.Green   .ToString("0.00", customCulture), sectionName);
        iniFile.Write ("bBright",   curDisp.ramp_gbc.Bright.Blue    .ToString("0.00", customCulture), sectionName);
        iniFile.Write ("rContrast", curDisp.ramp_gbc.Contrast.Red   .ToString("0.00", customCulture), sectionName);
        iniFile.Write ("gContrast", curDisp.ramp_gbc.Contrast.Green .ToString("0.00", customCulture), sectionName);
        iniFile.Write ("bContrast", curDisp.ramp_gbc.Contrast.Blue  .ToString("0.00", customCulture), sectionName);
        
        // Write color temperature settings
        iniFile.Write ("colorTempEnabled", curDisp.colorTempEnabled.ToString(), sectionName);
        iniFile.Write ("colorTempBlendEnabled", curDisp.colorTempBlendEnabled.ToString(), sectionName);
        iniFile.Write ("colorTemp", curDisp.colorTemp.ToString("0", customCulture), sectionName);
        
        // Write overlay settings
        iniFile.Write ("overlayEnabled", curDisp.overlayEnabled.ToString(), sectionName);
        iniFile.Write ("overlayEnforced", curDisp.overlayEnforced.ToString(), sectionName);
        iniFile.Write ("ovTransparency", curDisp.ovTransparency.ToString("0.00", customCulture), sectionName);
    }




    private void Window_Resize(object sender, EventArgs e)
    {
        if (WindowState == FormWindowState.Minimized) {
            Hide();
        } else {
            TopMost = true;
        }
    }
    private void Window_Activated(object sender, EventArgs e)
    {
        TopMost = true;
        // we'll also do an eqv of resync
        gammaSrc = GammaSrcColor.Combined;
        RenderCurInfo_ColorBtns();
        RenderCurInfo_Overlay();
        RenderCurInfo_ColorTemp();
        ResyncGammaRampValues();
    }
    private void Window_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Unregister the hotkey if the form is actually closing (not just hiding)
        if (!e.Cancel)
        {
            UnregisterHotKey(this.Handle, HOTKEY_ID);
        }
        else
        {
            e.Cancel = true;
            Hide();
        }
    }
    private void Window_Paint(object sender, PaintEventArgs e)
    {
        // Draw border in the inside edge of our content panel (with its content prior anchored to accomodate 1px border)
        using var borderPen = new Pen(BorderColor, 1);
        e.Graphics.DrawRectangle (borderPen, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
    }
    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            Hide();
        }
    }

    private void notifyIcon_Click(object sender, MouseEventArgs e)
    {
        // Show the window on any mouse click (left or right)
        Show();
        TopMost = true;
        WindowState = FormWindowState.Normal;
    }




    private void EnableDarkMode()
    {
        // first we'll set system level dark-mode .. to things like titlebar etc
        // (though we no longer show titlebar at all, so its less important now)

        NativeMethods.SetPreferredAppMode (NativeMethods.PREFERRED_APP_MODE__FORCE_DARK_MODE);

        var data = new WindowCompositionAttributeData
        {
            Attribute = NativeMethods.WCA__USE_DARK_MODE_COLORS,
            Data = Marshal.AllocHGlobal(sizeof(int)),
            SizeOfData = sizeof(int)
        };
        Marshal.WriteInt32(data.Data, 1);
        NativeMethods.SetWindowCompositionAttribute(Handle, ref data);
        Marshal.FreeHGlobal(data.Data);

        // then we'll handle all the various controls in our ui
        ApplyDarkMode(this);
    }

    private static void ApplyDarkMode(Control control)
    {
        // we'll go through recursively apply dark-mode colors to our non-dark controls
        // (track-bar and check-box we ended up w custom impl .. so they are native dark mode)
        // (combo-box we pulled from krypton w dark-mode theme)

        control.BackColor = BackgroundColor;
        control.ForeColor = ForegroundColor;

        foreach (Control childControl in control.Controls)
        {
            switch (childControl)
            {
                case Button button:
                    button.BackColor = ButtonColor;
                    button.ForeColor = ButtonTextColor;
                    button.FlatStyle = FlatStyle.Flat;
                    button.FlatAppearance.BorderColor = ButtonBorderColor;
                    button.FlatAppearance.BorderSize = 1;
                    break;
                case TextBox textBox:
                    textBox.BackColor = BackgroundColor;
                    textBox.ForeColor = ForegroundColor;
                    textBox.BorderStyle = BorderStyle.None;
                    break;
                default:
                    ApplyDarkMode(childControl);
                    break;
            }
        }
    }


    // Override WndProc to handle the WM_HOTKEY message
    protected override void WndProc(ref Message m)
    {
        if (m.Msg == WM_HOTKEY && m.WParam.ToInt32() == HOTKEY_ID)
        {
            // Toggle between showing and hiding the application
            if (Visible)
            {
                Hide();
            }
            else
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    WindowState = FormWindowState.Normal;
                }
                Show();
                Activate();
                BringToFront();
            }
        }
        base.WndProc(ref m);
    }

    private void SaveActivePreset(string presetName)
    {
        // Save the active preset for the current monitor
        string monitorSection = "ActivePresets";
        iniFile.Write(curDisp.displayName, presetName, monitorSection);
    }

    private string GetActivePreset()
    {
        // Get the active preset for the current monitor
        string monitorSection = "ActivePresets";
        return iniFile.Read(curDisp.displayName, monitorSection);
    }
}




[StructLayout(LayoutKind.Sequential)]
internal struct WindowCompositionAttributeData
{
    public int Attribute;
    public IntPtr Data;
    public int SizeOfData;
}
internal static class NativeMethods
{
    public const int PREFERRED_APP_MODE__FORCE_DARK_MODE = 26;
    public const int WCA__USE_DARK_MODE_COLORS = 26;

    [DllImport("uxtheme.dll", EntryPoint = "#135", SetLastError = true)]
    public static extern int SetPreferredAppMode(int appMode);

    [DllImport("user32.dll")]
    public static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
}
