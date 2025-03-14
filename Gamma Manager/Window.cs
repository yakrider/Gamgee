using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Gamma_Manager;

public partial class Window : Form
{
    System.Globalization.CultureInfo customCulture;
    IniFile iniFile;

    List<Display.DisplayInfo> displays = new List<Display.DisplayInfo>();
    int numDisplay = 0;
    Display.DisplayInfo curDisplay;

    List<ToolStripComboBox> toolMonitors = new List<ToolStripComboBox>();
    ToolStripComboBox toolMonitor;

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

    bool disableChangeFunc = false;
    private GammaSrcColor gammaSrc = GammaSrcColor.Combined;


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
            if (iniFile.Read("monitor", preset).Equals(curDisplay.displayName))
            {
                comboBoxPresets.Items.Add(preset);
            }
        }
    }



    private void initTrayMenu()
    {
        contextMenu.Items.Clear();
        toolMonitors.Clear();

        var toolSetting = new ToolStripMenuItem("Settings", null, toolSettings_Click);
        contextMenu.Items.Add(toolSetting);

        var toolStripSeparator1 = new ToolStripSeparator();
        contextMenu.Items.Add(toolStripSeparator1);

        foreach (var display in displays)
        {
            toolMonitor = new ToolStripComboBox(display.displayName);
            toolMonitor.DropDownStyle = ComboBoxStyle.DropDownList;

            toolMonitor.Items.Add(display.displayName + ":");
            toolMonitor.Text = display.displayName + ":";

            toolMonitor.SelectedIndexChanged += comboBoxToolMonitor_IndexChanged;

            foreach (var preset in iniFile.GetSections())
            {
                if (iniFile.Read("monitor", preset).Equals(display.displayName))
                {
                    //preset.name = preset.Substring(preset.IndexOf(")") + 1);
                    toolMonitor.Items.Add(preset);
                }
            }
            toolMonitors.Add(toolMonitor);
            contextMenu.Items.Add(toolMonitor);
        }
        var toolStripSeparator2 = new ToolStripSeparator();
        contextMenu.Items.Add(toolStripSeparator2);
        var toolExit = new ToolStripMenuItem("Exit", null, toolExit_Click);
        contextMenu.Items.Add(toolExit);
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
        curDisplay.ramp_gbc = RampGbc.GetDefaultRampGbc();
        Gamma.ResetGammaRamp(curDisplay.displayLink);
        RenderCurInfo_ColorBtns();
        RenderCurInfo_GBC();
    }
    private void RenderCurInfo_GBC()
    {
        disableChangeFunc = true;

        var (gamma, bright, contrast) = (
            curDisplay.ramp_gbc.Gamma.get(gammaSrc),
            curDisplay.ramp_gbc.Bright.get(gammaSrc),
            curDisplay.ramp_gbc.Contrast.get(gammaSrc)
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
        curDisplay.colorTempEnabled = false;
        curDisplay.colorTemp = 6500;
        RenderCurInfo_ColorTemp();
        UpdateTrackBars_FollowerInfo();
    }
    private void RenderCurInfo_ColorTemp()
    {
        if (displays.Count <= 0) return;
        disableChangeFunc = true;
        trackBarColorTemp.Value = (int)(curDisplay.colorTemp / 100f);
        textBoxColorTemp.Text = (curDisplay.colorTemp / 1000f).ToString("0.00");
        disableChangeFunc = false;
    }

    private void UpdateTrackBars_FollowerInfo()
    {
        if (curDisplay.colorTempEnabled)
        {
            trackBarColorTemp.IsFollower = false;
            trackBarGamma.IsFollower = trackBarBright.IsFollower = trackBarContrast.IsFollower = true;
        } else {
            trackBarColorTemp.IsFollower = true;
            trackBarGamma.IsFollower = trackBarBright.IsFollower = trackBarContrast.IsFollower = false;
        }
        trackBarGamma.Invalidate(); trackBarBright.Invalidate(); trackBarContrast.Invalidate(); trackBarColorTemp.Invalidate();
    }

    private void ResetInfo_Overlay()
    {
        curDisplay.ovTransparency = 1.0f;
        curDisplay.overlayEnforced = false;
        curDisplay.overlay.Update();
        RenderCurInfo_Overlay();;
    }
    private void RenderCurInfo_Overlay()
    {
        if (displays.Count <= 0) return;
        disableChangeFunc = true;
        checkBoxOverlay.Checked = curDisplay.overlayEnabled;
        trackBarOverlay.Value = (int)(100f * curDisplay.ovTransparency);
        textBoxOverlay.Text = curDisplay.ovTransparency.ToString("0.00");
        disableChangeFunc = false;
    }

    private void ResetInfo_Monitors()
    {
        // here we could directly update/reset monitor values
        // (but typically we should seldom want to do so)
        trackBarMonitorBright.Value = 100;
        if (curDisplay.isExternal) {
            trackBarMonitorContrast.Value = 50;
        }
    }
    private void RenderCurInfo_Monitors()
    {
        disableChangeFunc = true;

        labelMonitorContrastUp.Visible = labelMonitorContrastDown.Visible =
            trackBarMonitorContrast.Visible = textBoxMonitorContrast.Visible = curDisplay.isExternal;

        trackBarMonitorBright.Value = curDisplay.isExternal ?
            ExternalMonitor.GetBrightness(curDisplay.PhysicalHandle) : InternalMonitor.GetBrightness();
        textBoxMonitorBright.Text  = trackBarMonitorBright.Value.ToString();

        if (curDisplay.isExternal) {
            trackBarMonitorContrast.Value = ExternalMonitor.GetContrast(curDisplay.PhysicalHandle);
            textBoxMonitorContrast.Text = trackBarMonitorContrast.Value.ToString();
        }
        disableChangeFunc = false;
    }




    private void ApplyCurGammaRamp()
    {
        var ramp = Gamma.CreateGammaRamp(curDisplay.ramp_gbc);
        Gamma.SetGammaRamp(curDisplay.displayLink, ramp);
    }
    private void ApplyCurMonitorBrightness()
    {
        // we'll just change the track-bar values, and its change listener will affect the change
        if (curDisplay.isExternal) {
            trackBarMonitorBright.Value = curDisplay.monitorBrightness;
            trackBarMonitorContrast.Value = curDisplay.monitorContrast;
        } else {
            trackBarMonitorBright.Value = curDisplay.monitorBrightness;
        }
    }

    private void ResyncGammaRampValues()
    {
        if (displays.Count <= 0) return;
        // we'll attempt to read gamma-ramp and reverse calc the approx values to set the track-bar
        buttonResync.Enabled = false;
        var gbc = Gamma.InverseGammaRamp (Gamma.GetGammaRamp(curDisplay.displayLink));

        curDisplay.ramp_gbc = new RampGbc (
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
        Location = new Point(wa.X + wa.Width - Width - 24, wa.Y + wa.Height - Height - 24);
    }

    public Window()
    {
        InitializeComponent();

        ApplyDarkMode(this);
        // ^^ we'll ourselves apply dark colors to the winforms default components we use

        EnableDarkMode(); // Enable dark mode
        // ^^ this is system wide dark-mode to mostly helps with titlebar etc
        // (but we no longer show titlebar at all, so not that important anymore)

        customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ",";

        iniFile = new IniFile("GammaManager.ini");

        buttonAllColors.Font = new Font(buttonAllColors.Font.Name, buttonAllColors.Font.Size, FontStyle.Bold);

        displays = Display.QueryDisplayDevices();
        displays.Reverse();
        for (var i = 0; i < displays.Count; i++)
        {
            displays[i].numDisplay = i;
            comboBoxMonitors.Items.Add(i + 1 + ") " + displays[i].displayName);
        }
        curDisplay = displays[numDisplay];
        comboBoxMonitors.SelectedIndex = numDisplay;

        // instead of reset, we'll try to resync to existant gamma ramp at startup
        ResetInfo_ColorTemp();

        // but the others should start cleared
        ResetInfo_Overlay();
        ResyncGammaRampValues();
        initPresets();
        initTrayMenu();
        notifyIcon.ContextMenuStrip = contextMenu;
    }




    private void HandleTrackBarValueChanged (GBC track)
    {
        comboBoxPresets.Text = string.Empty;
        if (disableChangeFunc) return;

        switch (track)
        {
            case GBC.Gamma:
                curDisplay.ramp_gbc.Gamma .set (gammaSrc, trackBarGamma.Value / 100f);
                textBoxGamma.Text = (trackBarGamma.Value / 100f).ToString("0.00");
                break;
            case GBC.Bright:
                curDisplay.ramp_gbc.Bright .set (gammaSrc, trackBarBright.Value / 100f);
                textBoxBright.Text = (trackBarBright.Value / 100f).ToString("0.00");
                break;
            case GBC.Contrast:
                curDisplay.ramp_gbc.Contrast .set (gammaSrc, trackBarContrast.Value / 100f);
                textBoxContrast.Text = (trackBarContrast.Value / 100f).ToString("0.00");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(track), track, null);
        }
        ApplyCurGammaRamp();
        ResetInfo_ColorTemp();
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

        curDisplay.monitorBrightness = trackBarMonitorBright.Value;
        textBoxMonitorBright.Text = trackBarMonitorBright.Value.ToString();

        if (curDisplay.isExternal) {
            ExternalMonitor.SetBrightness(curDisplay.PhysicalHandle, (uint)trackBarMonitorBright.Value);
        } else {
            InternalMonitor.SetBrightness((byte)trackBarMonitorBright.Value);
        }
    }
    private void trackBarMonitorContrast_ValueChanged(object sender, EventArgs e)
    {
        //comboBoxPresets.Text = string.Empty;
        if (disableChangeFunc) return;

        curDisplay.monitorContrast = trackBarMonitorContrast.Value;
        textBoxMonitorContrast.Text = trackBarMonitorContrast.Value.ToString();

        ExternalMonitor.SetContrast(curDisplay.PhysicalHandle, (uint)trackBarMonitorContrast.Value);
    }




    private void trackBarColorTemp_ValueChanged(object sender, EventArgs e)
    {
        comboBoxPresets.Text = string.Empty;
        if (disableChangeFunc) return;
        curDisplay.colorTempEnabled = true;
        UpdateTrackBars_FollowerInfo();
        curDisplay.colorTemp = trackBarColorTemp.Value * 100;
        textBoxColorTemp.Text = (trackBarColorTemp.Value / 10f).ToString("0.00");
        Gamma.SetGammaRamp_ColorTemp (curDisplay.displayLink, curDisplay.colorTemp);
    }




    private void trackBarOverlay_ValueChanged(object sender, EventArgs e)
    {
        if (disableChangeFunc) return;
        // ^^ e.g. from trackbar value update upon changing monitor selection
        curDisplay.ovTransparency = trackBarOverlay.Value / 100f;
        textBoxOverlay.Text = (trackBarOverlay.Value / 100f).ToString("0.00");
        curDisplay.overlay.Update();
    }
    private void checkBoxOverlay_CheckedChanged(object sender, EventArgs e)
    {
        if (disableChangeFunc) return;
        curDisplay.overlayEnabled = checkBoxOverlay.Checked;
        curDisplay.overlay.Update();
    }
    private void checkBoxOverlayEnforced_CheckedChanged(object sender, EventArgs e)
    {
        if (disableChangeFunc) return;
        curDisplay.overlayEnforced = checkBoxOverlayEnforced.Checked;
        curDisplay.overlay.Update();
    }



    private void buttonAllColors_Click(object sender, EventArgs e)
    {
        gammaSrc = GammaSrcColor.Combined;
        RenderCurInfo_ColorBtns();
        RenderCurInfo_GBC();
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
        initTrayMenu();
    }

    private void buttonSave_Click(object sender, EventArgs e)
    {
        var tmp = comboBoxPresets.Text;
        iniFile.Write ("monitor", curDisplay.displayName, curDisplay.displayName+": "+ comboBoxPresets.Text);
        writeInitSectionGbc();
        //iniFile.Write("monitorBrightness", curDisplay.monitorBrightness.ToString(customCulture), curDisplay.displayName + ": " + comboBoxPresets.Text);
        //iniFile.Write("monitorContrast", curDisplay.monitorContrast.ToString(customCulture), curDisplay.displayName + ": " + comboBoxPresets.Text);
        // ^^ we'll keep monitor brighness separate from gamma etc configs for now

        initPresets();
        comboBoxPresets.Text = curDisplay.displayName + ": " + tmp;

        initTrayMenu();
    }

    private void buttonDelete_Click(object sender, EventArgs e)
    {
        iniFile.DeleteSection(comboBoxPresets.Text);

        initPresets();
        initTrayMenu();
    }

    private void buttonHide_Click(object sender, EventArgs e)
    {
        Hide();
    }
    private void buttonExit_Click(object sender, EventArgs e)
    {
        FormClosing -= Window_FormClosing;
        Close();
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

        num = num.Substring(0, num.IndexOf(")"));
        numDisplay = int.Parse(num)-1;
        curDisplay = displays[numDisplay];

        // monitor changing shouldnt mean any active action, just to update UI with data for the selected monitor
        //RenderCurInfo_ColorBtns();
        RenderCurInfo_GBC();
        RenderCurInfo_Monitors();
        RenderCurInfo_Overlay();
        RenderCurInfo_ColorTemp();
        initPresets();
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

    private RampGbc readIniSectionGbc(string section)
    {
        var gbc = RampGbc.GetDefaultRampGbc();
        gbc.Gamma.Red      = float.Parse(iniFile.Read("rGamma",    comboBoxPresets.Text), customCulture);
        gbc.Gamma.Green    = float.Parse(iniFile.Read("gGamma",    comboBoxPresets.Text), customCulture);
        gbc.Gamma.Blue     = float.Parse(iniFile.Read("bGamma",    comboBoxPresets.Text), customCulture);
        gbc.Bright.Red     = float.Parse(iniFile.Read("rBright",   comboBoxPresets.Text), customCulture);
        gbc.Bright.Green   = float.Parse(iniFile.Read("gBright",   comboBoxPresets.Text), customCulture);
        gbc.Bright.Blue    = float.Parse(iniFile.Read("bBright",   comboBoxPresets.Text), customCulture);
        gbc.Contrast.Red   = float.Parse(iniFile.Read("rContrast", comboBoxPresets.Text), customCulture);
        gbc.Contrast.Green = float.Parse(iniFile.Read("gContrast", comboBoxPresets.Text), customCulture);
        gbc.Contrast.Blue  = float.Parse(iniFile.Read("bContrast", comboBoxPresets.Text), customCulture);
        return gbc;
    }
    private void writeInitSectionGbc()
    {
        iniFile.Write ("rGamma",    curDisplay.ramp_gbc.Gamma.Red      .ToString(customCulture), curDisplay.displayName + ": " + comboBoxPresets.Text);
        iniFile.Write ("gGamma",    curDisplay.ramp_gbc.Gamma.Green    .ToString(customCulture), curDisplay.displayName + ": " + comboBoxPresets.Text);
        iniFile.Write ("bGamma",    curDisplay.ramp_gbc.Gamma.Blue     .ToString(customCulture), curDisplay.displayName + ": " + comboBoxPresets.Text);
        iniFile.Write ("rContrast", curDisplay.ramp_gbc.Bright.Red     .ToString(customCulture), curDisplay.displayName + ": " + comboBoxPresets.Text);
        iniFile.Write ("gContrast", curDisplay.ramp_gbc.Bright.Green   .ToString(customCulture), curDisplay.displayName + ": " + comboBoxPresets.Text);
        iniFile.Write ("bContrast", curDisplay.ramp_gbc.Bright.Blue    .ToString(customCulture), curDisplay.displayName + ": " + comboBoxPresets.Text);
        iniFile.Write ("rBright",   curDisplay.ramp_gbc.Contrast.Red   .ToString(customCulture), curDisplay.displayName + ": " + comboBoxPresets.Text);
        iniFile.Write ("gBright",   curDisplay.ramp_gbc.Contrast.Green .ToString(customCulture), curDisplay.displayName + ": " + comboBoxPresets.Text);
        iniFile.Write ("bBright",   curDisplay.ramp_gbc.Contrast.Blue  .ToString(customCulture), curDisplay.displayName + ": " + comboBoxPresets.Text);
    }

    private void comboBoxPresets_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (disableChangeFunc) return;

        curDisplay.ramp_gbc = readIniSectionGbc(comboBoxPresets.Text);

        //curDisplay.monitorBrightness = int.Parse(iniFile.Read("monitorBrightness", comboBoxPresets.Text));
        //curDisplay.monitorContrast = int.Parse(iniFile.Read("monitorContrast", comboBoxPresets.Text));
        // ^^ we'll keep monitor brighness separate from gamma etc configs for now

        initTrayMenu();

        gammaSrc = GammaSrcColor.Combined;
        RenderCurInfo_ColorBtns();
        RenderCurInfo_GBC();

        // loading preset gamma ramp means color-temp based ramp becomes invalid
        ResetInfo_ColorTemp();

        // ResetInfo_Overlay();
        // ^^ we'll keep overlay orthogonal from presets for nwo

        //ApplyCurMonitorBrightness();
        RenderCurInfo_Monitors();
        // ^^ we'll keep monitor brightness separate from presets, but we'll sync up to external brighness changes

        // first lets set the gamma-ramp
        ApplyCurGammaRamp();

        // next set the monitor brightness too
        // .. but meh .. we're gonna disable this .. cleaner to avoid messing up monitor brightness just from picking gamma presets
        //ApplyCurMonitorBrightness();
    }

    //tray
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
        e.Cancel = true;
        Hide();
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
        Show();
        TopMost = true;
        WindowState = FormWindowState.Normal;
    }
    private void notifyIcon_DoubleClick(object sender, MouseEventArgs e)
    {
        Show();
        TopMost = true;
        WindowState = FormWindowState.Normal;
    }
    private void toolSettings_Click(object sender, EventArgs e)
    {
        Show();
        TopMost = true;
        WindowState = FormWindowState.Normal;
    }
    private void toolExit_Click(object sender, EventArgs e)
    {
        FormClosing -= Window_FormClosing;
        Close();
    }



    private void comboBoxToolMonitor_IndexChanged(object sender, EventArgs e)
    {
        if (disableChangeFunc) return;

        var monitor = sender.ToString().Substring(0, sender.ToString().IndexOf(":"));
        /*string comName = toolMonitor.Items[i].ToString().Substring
                    (0, toolMonitor.Items[i].ToString().IndexOf(":"));*/

        var tmp = 0;

        disableChangeFunc = true;

        for (var i = 0; i < displays.Count; i++)
        {
            if (monitor.Equals(displays[i].displayName)) {
                tmp = i;
            } else {
                toolMonitor = toolMonitors[i];
                toolMonitor.SelectedIndex = 0;
            }
        }
        disableChangeFunc = false;

        toolMonitor = toolMonitors[tmp];

        if (toolMonitor.SelectedIndex == 0) return;


        for (var i = 0; i < displays.Count; i++)
        {
            if (displays[i].displayName.Equals(toolMonitor.Items[0].ToString().Substring(0, toolMonitor.Items[0].ToString().IndexOf(":"))))
            {
                comboBoxMonitors.Text = i + 1 + ") " + displays[i].displayName;
                numDisplay = i;
                curDisplay.numDisplay = numDisplay;
                curDisplay.displayLink = displays[i].displayLink;
                curDisplay.isExternal = displays[i].isExternal;
                break;
            }
        }

        curDisplay.displayName = toolMonitor.Items[0].ToString().Substring(0, toolMonitor.Items[0].ToString().IndexOf(":"));

        curDisplay.ramp_gbc = readIniSectionGbc(toolMonitor.Text);

        //curDisplay.monitorBrightness = int.Parse(iniFile.Read("monitorBrightness", toolMonitor.Text));
        //curDisplay.monitorContrast = int.Parse(iniFile.Read("monitorContrast", toolMonitor.Text));

        initPresets();

        gammaSrc = GammaSrcColor.Combined;
        RenderCurInfo_ColorBtns();
        RenderCurInfo_GBC();
        ResetInfo_ColorTemp();
        //ApplyCurMonitorBrightness();
        RenderCurInfo_Monitors();
        //ResetInfo_Overlay();
        ApplyCurGammaRamp();
    }




    private static void ApplyDarkMode(Control control)
    {
        control.BackColor = BackgroundColor;
        control.ForeColor = ForegroundColor;

        foreach (Control childControl in control.Controls)
        {
            if (childControl is Button button)
            {
                button.BackColor = ButtonColor;
                button.ForeColor = ButtonTextColor;
                button.FlatStyle = FlatStyle.Flat;
                button.FlatAppearance.BorderColor = ButtonBorderColor;
                button.FlatAppearance.BorderSize = 1;
            }
            else if (childControl is TextBox textBox)
            {
                textBox.BackColor = BackgroundColor;
                textBox.ForeColor = ForegroundColor;
                textBox.BorderStyle = BorderStyle.None;
            }
            else if (childControl is ComboBox comboBox)
            {
                comboBox.FlatStyle = FlatStyle.Flat;
                comboBox.BackColor = BackgroundColor;
                comboBox.ForeColor = ForegroundColor;
            }
            else
            {
                ApplyDarkMode(childControl);
            }
        }
    }
    private void EnableDarkMode()
    {
        NativeMethods.SetPreferredAppMode(PreferredAppMode.ForceDark);

        var data = new WindowCompositionAttributeData
        {
            Attribute = WindowCompositionAttribute.WCA_USEDARKMODECOLORS,
            Data = Marshal.AllocHGlobal(sizeof(int)),
            SizeOfData = sizeof(int)
        };

        Marshal.WriteInt32(data.Data, 1);
        NativeMethods.SetWindowCompositionAttribute(Handle, ref data);
        Marshal.FreeHGlobal(data.Data);
    }

}




internal enum PreferredAppMode
{
    Default,
    AllowDark,
    ForceDark,
    ForceLight,
    Max
}

internal enum WindowCompositionAttribute
{
    WCA_UNDEFINED = 0,
    WCA_NCRENDERING_ENABLED = 1,
    WCA_NCRENDERING_POLICY = 2,
    WCA_TRANSITIONS_FORCEDISABLED = 3,
    WCA_ALLOW_NCPAINT = 4,
    WCA_CAPTION_BUTTON_BOUNDS = 5,
    WCA_NONCLIENT_RTL_LAYOUT = 6,
    WCA_FORCE_ICONIC_REPRESENTATION = 7,
    WCA_EXTENDED_FRAME_BOUNDS = 8,
    WCA_HAS_ICONIC_BITMAP = 9,
    WCA_THEME_ATTRIBUTES = 10,
    WCA_NCRENDERING_EXILED = 11,
    WCA_NCADORNMENTINFO = 12,
    WCA_EXCLUDED_FROM_LIVEPREVIEW = 13,
    WCA_VIDEO_OVERLAY_ACTIVE = 14,
    WCA_FORCE_ACTIVEWINDOW_APPEARANCE = 15,
    WCA_DISALLOW_PEEK = 16,
    WCA_CLOAK = 17,
    WCA_CLOAKED = 18,
    WCA_ACCENT_POLICY = 19,
    WCA_FREEZE_REPRESENTATION = 20,
    WCA_EVER_UNCLOAKED = 21,
    WCA_VISUAL_OWNER = 22,
    WCA_HOLOGRAPHIC = 23,
    WCA_EXCLUDED_FROM_DDA = 24,
    WCA_PASSIVEUPDATEMODE = 25,
    WCA_USEDARKMODECOLORS = 26,
    WCA_LAST = 27
}

[StructLayout(LayoutKind.Sequential)]
internal struct WindowCompositionAttributeData
{
    public WindowCompositionAttribute Attribute;
    public IntPtr Data;
    public int SizeOfData;
}

internal static class NativeMethods
{
    [DllImport("uxtheme.dll", EntryPoint = "#135", SetLastError = true)]
    public static extern int SetPreferredAppMode(PreferredAppMode appMode);

    [DllImport("user32.dll")]
    public static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
}

public class CustomTrackBar : TrackBar
{
    private static readonly Color ThumbColor_Normal = Color.Aqua;
    private static readonly Color ThumbColor_Follower = Color.Peru;
    private static readonly Color TrackColor_Normal = Color.FromArgb(140,140,140);
    private static readonly Color TrackColor_Hovered = Color.FromArgb(200, 200, 200);

    private bool _isHovered;

    // our bar can be either driving data, or just following/reflecting external changes
    public bool IsFollower { get; set; }

    public CustomTrackBar()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        Size = new Size(200, 0);
        AutoSize = false;
    }
    protected override void OnValueChanged(EventArgs e)
    {
        base.OnValueChanged(e);
        Invalidate();
    }
    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        _isHovered = true;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _isHovered = false;
        Invalidate();
    }
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.Clear(BackColor);

        const int trackHeight = 1;  // Thickness of track
        const int thumbSize = 10;   // Size of the thumb

        var trackY = Height / 2 - trackHeight / 2;
        var thumbX = (int)((float)(Value - Minimum) / (Maximum - Minimum) * (Width - thumbSize));

        using Brush trackBrush = new SolidBrush(_isHovered ? TrackColor_Hovered : TrackColor_Normal);
        using Brush thumbBrush = new SolidBrush(IsFollower ? ThumbColor_Follower : ThumbColor_Normal);
        e.Graphics.FillRectangle (trackBrush, new Rectangle (0, trackY, Width, trackHeight));
        e.Graphics.FillEllipse   (thumbBrush, new Rectangle (thumbX, Height / 2 - thumbSize / 2, thumbSize, thumbSize));
    }
}
