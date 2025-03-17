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
    Presets presets;
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
        var displayPresets = presets.GetPresetsForDisplay(curDisp.displayName);
        comboBoxPresets.Items.AddRange(displayPresets.ToArray());
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
            var (keyStr, modifiersStr) = presets.GetHotkeyConfig();

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
            var modifiers = 0;
            var modifierArray = modifiersStr.Split(',');
            foreach (var modifier in modifierArray)
            {
                var trimmedModifier = modifier.Trim().ToLower();
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
            // Build the modifiers string
            var modifiersList = new List<string>();
            if ((modifiers & MOD_ALT) != 0)
                modifiersList.Add("Alt");
            if ((modifiers & MOD_CONTROL) != 0)
                modifiersList.Add("Control");
            if ((modifiers & MOD_SHIFT) != 0)
                modifiersList.Add("Shift");
            
            // Save the hotkey configuration
            presets.SaveHotkeyConfig(key.ToString(), string.Join(", ", modifiersList));
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

        presets = new Presets("Gamgee.ini");
        
        // Register the global hotkey from INI file (or use default if not specified)
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
        string activePreset = presets.GetActivePreset(curDisp.displayName);
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
        presets.SaveActivePreset(curDisp.displayName, "");
    }



    private void buttonSave_Click(object sender, EventArgs e)
    {
        var presetName = comboBoxPresets.Text;
        
        // Save the preset
        presets.SavePreset(presetName, curDisp.displayName, curDisp.ramp_gbc,
            curDisp.colorTempEnabled, curDisp.colorTempBlendEnabled, curDisp.colorTemp,
            curDisp.overlayEnabled, curDisp.overlayEnforced, curDisp.ovTransparency,
            customCulture);
        
        // Refresh the presets list
        initPresets();
        comboBoxPresets.Text = presetName;
        
        // Save this as the active preset for this monitor
        presets.SaveActivePreset(curDisp.displayName, presetName);
    }

    private void buttonDelete_Click(object sender, EventArgs e)
    {
        var presetName = comboBoxPresets.Text;
        
        // Check if this is the active preset
        var activePreset = presets.GetActivePreset(curDisp.displayName);
        if (activePreset == presetName)
        {
            // Clear the active preset since we're deleting it
            presets.SaveActivePreset(curDisp.displayName, "");
        }
        
        presets.DeletePreset(presetName, curDisp.displayName);
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
        
        var activePreset = presets.GetActivePreset(curDisp.displayName);
        if (string.IsNullOrEmpty(activePreset)) return;

        // Check if the preset exists in the combobox
        if (comboBoxPresets.Items.Contains(activePreset))
        {
            comboBoxPresets.Text = activePreset;
            // The SelectedIndexChanged event will handle loading the preset
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

        var presetName = comboBoxPresets.Text;
        
        // Load the preset
        if (presets.LoadPreset(presetName, curDisp.displayName, out var rampGbc,
            out var colorTempEnabled, out var colorTempBlendEnabled, out var colorTemp,
            out var overlayEnabled, out var overlayEnforced, out var ovTransparency,
            customCulture))
        {
            // Apply the loaded settings
            curDisp.ramp_gbc = rampGbc;
            curDisp.colorTempEnabled = colorTempEnabled;
            curDisp.colorTempBlendEnabled = colorTempBlendEnabled;
            curDisp.colorTemp = colorTemp;
            curDisp.overlayEnabled = overlayEnabled;
            curDisp.overlayEnforced = overlayEnforced;
            curDisp.ovTransparency = ovTransparency;

            // Set color source to Combined
            gammaSrc = GammaSrcColor.Combined;
            RenderCurInfo_ColorBtns();
            RenderCurInfo_GBC();
            RenderCurInfo_ColorTemp();
            RenderTrackBars_FollowerInfo();
            RenderCurInfo_Overlay();
            curDisp.overlay.Update();
            RenderCurInfo_Monitors();
            ApplyCurGammaRamp();
            
            // Save this as the active preset for this monitor
            if (!string.IsNullOrEmpty(presetName))
            {
                presets.SaveActivePreset(curDisp.displayName, presetName);
            }
        }
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
