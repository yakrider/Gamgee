using System;
using System.Drawing;
using System.Windows.Forms;

namespace Gamma_Manager;

partial class Window
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Window));

        this.contentPanel = new Panel();

        this.buttonAllColors = new Button();
        this.buttonRed = new Button();
        this.buttonGreen = new Button();
        this.buttonBlue = new Button();

        this.buttonResync = new Button();

        this.comboBoxPresets = new Krypton.Toolkit.KryptonComboBox();
        this.comboBoxMonitors = new Krypton.Toolkit.KryptonComboBox();

        this.buttonReset = new Button();
        this.buttonSave = new Button();
        this.buttonDelete = new Button();

        this.labelGamma = new Label();
        this.trackBarGamma = new CustomTrackBar{};
        this.textBoxGamma = new TextBox();

        this.labelBright = new Label();
        this.trackBarBright = new CustomTrackBar { };
        this.textBoxBright = new TextBox();

        this.labelContrast = new Label();
        this.trackBarContrast = new CustomTrackBar { };
        this.textBoxContrast = new TextBox();

        this.labelOverlay = new Label();
        this.trackBarOverlay = new CustomTrackBar { };
        this.textBoxOverlay = new TextBox();
        this.checkBoxOverlay = new CheckBox();
        this.checkBoxOverlayEnforced = new CheckBox();


        this.labelMonitorBrightnessUp = new Label();
        this.labelMonitorBrightnessDown = new Label();
        this.trackBarMonitorBright = new CustomTrackBar { };
        this.textBoxMonitorBrightness = new TextBox();

        this.buttonHide = new Button();
        this.buttonExit = new Button();

        this.labelMonitorContrastUp = new Label();
        this.labelMonitorContrastDown = new Label();
        this.trackBarMonitorContrast = new CustomTrackBar { };
        this.textBoxMonitorContrast = new TextBox();

        this.buttonForward = new Button();
        this.notifyIcon = new NotifyIcon(this.components);
        this.pictureBox = new PictureBox();
        this.contextMenu = new ContextMenuStrip(this.components);

        ((System.ComponentModel.ISupportInitialize)(this.trackBarGamma)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.trackBarContrast)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.trackBarBright)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.trackBarMonitorBright)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.trackBarMonitorContrast)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();

        this.SuspendLayout();

        var (topOffset, trackBarSpacing, trackBarOffset, textBoxOffset, checkBoxOffset) = (24, 52, -6, 0, 0);
        var (x_label, x_track, x_textbox, x_btns1, x_btns2, x_img) = (8, 104, 490, 582, 684, 800);

        //
        // labelGamma
        //
        this.labelGamma.Name = "labelGamma";
        this.labelGamma.Text = "Gamma";
        this.labelGamma.TabStop = false;
        this.labelGamma.Location = new Point(x_label, 28);
        this.labelGamma.Margin = new Padding(4, 0, 4, 0);
        //this.labelGamma.Size = new Size(86, 25);
        this.labelGamma.AutoSize = true;
        //
        // labelBright
        //
        this.labelBright.Name = "labelBright";
        this.labelBright.Text = "Bright";
        this.labelBright.TabStop = false;
        this.labelBright.Location = new Point(x_label, trackBarSpacing + labelGamma.Location.Y);
        this.labelBright.Margin = new Padding(4, 0, 4, 0);
        //this.labelBright.Size = new Size(114, 25);
        this.labelBright.AutoSize = true;
        //
        // labelContrast
        //
        this.labelContrast.Name = "labelContrast";
        this.labelContrast.Text = "Contrast";
        this.labelContrast.TabStop = false;
        this.labelContrast.Location = new Point(x_label, trackBarSpacing + labelBright.Location.Y);
        this.labelContrast.Margin = new Padding(4, 0, 4, 0);
        this.labelContrast.Size = new Size(93, 25);
        this.labelContrast.AutoSize = true;
        //
        // labelOverlay
        //
        this.labelOverlay.Name = "labelOverlay";
        this.labelOverlay.Text = "Overlay";
        this.labelOverlay.TabStop = false;
        this.labelOverlay.Location = new Point(x_label, trackBarSpacing + labelContrast.Location.Y);
        this.labelOverlay.Margin = new Padding(4, 0, 4, 0);
        this.labelOverlay.Size = new Size(93, 25);
        this.labelOverlay.AutoSize = true;

        //
        // trackBarGamma
        //
        this.trackBarGamma.Name = "trackBarGamma";
        this.trackBarGamma.Location = new Point(x_track, trackBarOffset + labelGamma.Location.Y);
        this.trackBarGamma.TabIndex = 0;
        this.trackBarGamma.SmallChange = 1;
        this.trackBarGamma.LargeChange = 5;
        this.trackBarGamma.Minimum = 30;
        this.trackBarGamma.Maximum = 440;
        this.trackBarGamma.Value = 100;
        this.trackBarGamma.ValueChanged += new EventHandler(this.trackBarGamma_ValueChanged);
        //
        // trackBarBright
        //
        this.trackBarBright.Name = "trackBarBright";
        this.trackBarBright.Location = new Point(x_track, trackBarOffset + labelBright.Location.Y);
        this.trackBarBright.TabIndex = 1 + trackBarGamma.TabIndex;
        this.trackBarBright.SmallChange = 1;
        this.trackBarBright.LargeChange = 5;
        this.trackBarBright.Minimum = -100;
        this.trackBarBright.Maximum = 100;
        this.trackBarBright.Value = 0;
        this.trackBarBright.ValueChanged += new EventHandler(this.trackBarBright_ValueChanged);
        //
        // trackBarContrast
        //
        this.trackBarContrast.Name = "trackBarContrast";
        this.trackBarContrast.Location = new Point(x_track, trackBarOffset + labelContrast.Location.Y);
        this.trackBarContrast.TabIndex = 1 + trackBarBright.TabIndex;
        this.trackBarContrast.SmallChange = 1;
        this.trackBarContrast.LargeChange = 5;
        this.trackBarContrast.Minimum = 10;
        this.trackBarContrast.Maximum = 300;
        this.trackBarContrast.Value = 10;
        this.trackBarContrast.ValueChanged += new EventHandler(this.trackBarContrast_ValueChanged);
        //
        // trackBarOverlay (dimming overlay transparency)
        //
        this.trackBarOverlay.Location = new Point(x_track, trackBarOffset + labelOverlay.Location.Y);
        this.trackBarOverlay.Name = "trackBarOverlay";
        this.trackBarOverlay.TabIndex = 1 + trackBarContrast.TabIndex;
        this.trackBarOverlay.SmallChange = 1;
        this.trackBarOverlay.LargeChange = 5;
        this.trackBarOverlay.Maximum = 100;
        this.trackBarOverlay.Minimum = 40;
        this.trackBarOverlay.Value = 100;
        this.trackBarOverlay.ValueChanged += new EventHandler(this.trackBarOverlay_ValueChanged);

        //
        // textBoxGamma
        //
        this.textBoxGamma.Name = "textBoxGamma";
        this.textBoxGamma.Location = new Point(x_textbox, textBoxOffset + labelGamma.Location.Y);
        this.textBoxGamma.TabStop = false;
        this.textBoxGamma.ReadOnly = true;
        this.textBoxGamma.Size = new Size(62, 31);
        this.textBoxGamma.Margin = new Padding(4, 5, 4, 5);
        this.textBoxGamma.TextAlign = HorizontalAlignment.Right;
        //
        // textBoxBright
        //
        this.textBoxBright.Name = "textBoxBright";
        this.textBoxBright.Location = new Point(x_textbox, textBoxOffset + labelBright.Location.Y);
        this.textBoxBright.TabStop = false;
        this.textBoxBright.ReadOnly = true;
        this.textBoxBright.Size = new Size(62, 31);
        this.textBoxBright.Margin = new Padding(4, 5, 4, 5);
        this.textBoxBright.TextAlign = HorizontalAlignment.Right;
        //
        // textBoxContrast
        //
        this.textBoxContrast.Name = "textBoxContrast";
        this.textBoxContrast.Location = new Point(x_textbox, textBoxOffset + labelContrast.Location.Y);
        this.textBoxContrast.TabStop = false;
        this.textBoxContrast.ReadOnly = true;
        this.textBoxContrast.Size = new Size(62, 31);
        this.textBoxContrast.Margin = new Padding(4, 5, 4, 5);
        this.textBoxContrast.TextAlign = HorizontalAlignment.Right;
        //
        // textBoxOverlay
        //
        this.textBoxOverlay.Name = "textBoxOverlay";
        this.textBoxOverlay.Location = new Point(x_textbox, textBoxOffset + labelOverlay.Location.Y);
        this.textBoxOverlay.TabStop = false;
        this.textBoxOverlay.ReadOnly = true;
        this.textBoxOverlay.Size = new Size(62, 31);
        this.textBoxOverlay.Margin = new Padding(4, 5, 4, 5);
        this.textBoxOverlay.TextAlign = HorizontalAlignment.Right;

        //
        // checkBoxOverlay
        //
        this.checkBoxOverlay.Name = "checkBoxExContrast";
        this.checkBoxOverlay.Text = "Allow";
        this.checkBoxOverlay.TabIndex = 1 + trackBarOverlay.TabIndex;
        this.checkBoxOverlay.Location = new Point(x_btns1, checkBoxOffset + labelOverlay.Location.Y);
        this.checkBoxOverlay.Margin = new Padding(4, 5, 4, 5);
        this.checkBoxOverlay.Size = new Size(80, 29);
        this.checkBoxOverlay.AutoSize = true;
        this.checkBoxOverlay.UseVisualStyleBackColor = true;
        this.checkBoxOverlay.CheckedChanged += new EventHandler(this.checkBoxOverlay_CheckedChanged);
        //
        // checkBoxOverlayEnforced
        //
        this.checkBoxOverlayEnforced.Name = "checkBoxOverlayEnforced";
        this.checkBoxOverlayEnforced.Text = "Force";
        this.checkBoxOverlayEnforced.TabIndex = 2 + trackBarOverlay.TabIndex;
        this.checkBoxOverlayEnforced.Location = new Point(x_btns2,  checkBoxOverlay.Location.Y);
        this.checkBoxOverlayEnforced.Margin = new Padding(4, 5, 4, 5);
        this.checkBoxOverlayEnforced.Size = new Size(80, 29);
        this.checkBoxOverlayEnforced.AutoSize = true;
        this.checkBoxOverlayEnforced.UseVisualStyleBackColor = true;
        this.checkBoxOverlayEnforced.CheckedChanged += new EventHandler(this.checkBoxOverlayEnforced_CheckedChanged);

        //
        // buttonAllColors
        //
        var (btns_y, btns_w, btns_h) = (8, 104, 42);
        this.buttonAllColors.Name = "buttonAllColors";
        this.buttonAllColors.Text = "All Colors";
        this.buttonAllColors.TabIndex = 4;
        this.buttonAllColors.Location = new Point(x_btns1, btns_y);
        this.buttonAllColors.TextAlign = ContentAlignment.MiddleCenter;
        this.buttonAllColors.Size = new Size(btns_w, 3*btns_h-2);
        this.buttonAllColors.UseVisualStyleBackColor = true;
        this.buttonAllColors.Click += new EventHandler(this.buttonAllColors_Click);
        //
        // buttonRed
        //
        this.buttonRed.Name = "buttonRed";
        this.buttonRed.Text = "Red";
        this.buttonRed.TabIndex = 1;
        this.buttonRed.Location = new Point(x_btns2, btns_y);
        this.buttonRed.TextAlign = ContentAlignment.TopCenter;
        this.buttonRed.Size = new Size(btns_w, btns_h);
        this.buttonRed.UseVisualStyleBackColor = true;
        this.buttonRed.Click += new EventHandler(this.buttonRed_Click);
        //
        // buttonGreen
        //
        this.buttonGreen.Name = "buttonGreen";
        this.buttonGreen.Text = "Green";
        this.buttonGreen.TabIndex = 2;
        this.buttonGreen.Location = new Point(x_btns2, btns_h-2 + buttonRed.Location.Y);
        this.buttonGreen.TextAlign = ContentAlignment.TopCenter;
        this.buttonGreen.Size = new Size(btns_w, btns_h);
        this.buttonGreen.UseVisualStyleBackColor = true;
        this.buttonGreen.Click += new EventHandler(this.buttonGreen_Click);
        //
        // buttonBlue
        //
        this.buttonBlue.Name = "buttonBlue";
        this.buttonBlue.Text = "Blue";
        this.buttonBlue.TabIndex = 3;
        this.buttonBlue.Location = new Point(x_btns2, btns_h-2 + buttonGreen.Location.Y);
        this.buttonBlue.TextAlign = ContentAlignment.TopCenter;
        this.buttonBlue.Size = new Size(btns_w, btns_h);
        this.buttonBlue.UseVisualStyleBackColor = true;
        this.buttonBlue.Click += new EventHandler(this.buttonBlue_Click);

        //
        // buttonResync
        //
        this.buttonResync.Name = "buttonResync";
        this.buttonResync.Text = "Sync";
        //this.buttonResync.TabIndex = 3;
        this.buttonResync.Location = new Point(x_btns1, btns_h-2 + buttonBlue.Location.Y);
        this.buttonResync.TextAlign = ContentAlignment.TopCenter;
        this.buttonResync.Size = new Size(btns_w, btns_h);
        this.buttonResync.UseVisualStyleBackColor = true;
        this.buttonResync.Click += new EventHandler(this.buttonResync_Click);
        //
        // buttonReset
        //
        this.buttonReset.Name = "buttonReset";
        this.buttonReset.Text = "Reset";
        this.buttonReset.TabIndex = 6;
        this.buttonReset.Location = new Point(x_btns2, btns_h-1 + buttonBlue.Location.Y);
        this.buttonReset.TextAlign = ContentAlignment.TopCenter;
        this.buttonReset.Size = new Size(btns_w, btns_h);
        this.buttonReset.UseVisualStyleBackColor = true;
        this.buttonReset.Click += new EventHandler(this.buttonReset_Click);

        //
        // comboBoxMonitors
        //
        this.comboBoxMonitors.Name = "comboBoxMonitors";
        this.comboBoxMonitors.TabIndex = 5;
        this.comboBoxMonitors.Location = new Point(4, trackBarSpacing + labelOverlay.Location.Y);
        this.comboBoxMonitors.Size = new Size(195, 0);
        this.comboBoxMonitors.FormattingEnabled = true;
        this.comboBoxMonitors.DropDownStyle = ComboBoxStyle.DropDownList;
        this.comboBoxMonitors.PaletteMode = Krypton.Toolkit.PaletteMode.Office2010BlackDarkMode;
        this.comboBoxMonitors.SelectedIndexChanged += new EventHandler(this.comboBoxMonitors_SelectedIndexChanged);
        //
        // buttonForward
        //
        this.buttonForward.Name = "buttonForward";
        this.buttonForward.Text = ">";
        this.buttonForward.TabIndex = 1 + comboBoxMonitors.TabIndex;
        this.buttonForward.Location = new Point(205, 0 + comboBoxMonitors.Location.Y);
        this.buttonForward.TextAlign = ContentAlignment.TopCenter;
        this.buttonForward.Size = new Size(42, 42);
        this.buttonForward.UseVisualStyleBackColor = true;
        this.buttonForward.Click += new EventHandler(this.buttonForward_Click);
        //
        // comboBoxPresets
        //
        this.comboBoxPresets.Name = "comboBoxPresets";
        this.comboBoxPresets.TabIndex = 2 + comboBoxMonitors.TabIndex;
        this.comboBoxPresets.Location = new Point(250, 0 + comboBoxMonitors.Location.Y);
        this.comboBoxPresets.Margin = new Padding(4, 5, 4, 5);
        this.comboBoxPresets.Size = new Size(305, 0);
        this.comboBoxPresets.FormattingEnabled = true;
        this.comboBoxPresets.DropDownStyle = ComboBoxStyle.DropDown;
        this.comboBoxPresets.PaletteMode = Krypton.Toolkit.PaletteMode.Office2010BlackDarkMode;
        this.comboBoxPresets.SelectedIndexChanged += new EventHandler(this.comboBoxPresets_SelectedIndexChanged);
        //
        // buttonSave
        //
        this.buttonSave.Name = "buttonSave";
        this.buttonSave.Text = "Save";
        this.buttonSave.TabIndex = 3 + comboBoxMonitors.TabIndex;
        this.buttonSave.Location = new Point(x_btns1, 0 + comboBoxMonitors.Location.Y);
        this.buttonSave.TextAlign = ContentAlignment.TopCenter;
        this.buttonSave.Size = new Size(btns_w, 42);
        this.buttonSave.UseVisualStyleBackColor = true;
        this.buttonSave.Click += new EventHandler(this.buttonSave_Click);
        //
        // buttonDelete
        //
        this.buttonDelete.Name = "buttonDelete";
        this.buttonDelete.Text = "Delete";
        this.buttonDelete.TabIndex = 4 + comboBoxMonitors.TabIndex;
        this.buttonDelete.Location = new Point(x_btns2, 0 + comboBoxMonitors.Location.Y);
        this.buttonDelete.TextAlign = ContentAlignment.TopCenter;
        this.buttonDelete.Size = new Size(btns_w, 42);
        this.buttonDelete.UseVisualStyleBackColor = true;
        this.buttonDelete.Click += new EventHandler(this.buttonDelete_Click);

        //
        // labelMonitorBrightnessUp
        //
        var monitorBrightnessY = 14 + trackBarSpacing + comboBoxMonitors.Location.Y;
        this.labelMonitorBrightnessUp.Name = "labelMonitorBrightnessUp";
        this.labelMonitorBrightnessUp.Text = "Monitor";
        this.labelMonitorBrightnessUp.TabStop = false;
        this.labelMonitorBrightnessUp.Location = new Point(x_label, -12 + monitorBrightnessY);
        this.labelMonitorBrightnessUp.Margin = new Padding(4, 0, 4, 0);
        this.labelMonitorBrightnessUp.Size = new Size(84, 25);
        this.labelMonitorBrightnessUp.AutoSize = true;
        //
        // labelMonitorBrightnessDown
        //
        this.labelMonitorBrightnessDown.Name = "labelMonitorBrightnessDown";
        this.labelMonitorBrightnessDown.Text = "Bright";
        this.labelMonitorBrightnessDown.TabStop = false;
        this.labelMonitorBrightnessDown.Location = new Point(x_label, 28 + labelMonitorBrightnessUp.Location.Y);
        this.labelMonitorBrightnessDown.Margin = new Padding(4, 0, 4, 0);
        this.labelMonitorBrightnessDown.Size = new Size(114, 25);
        this.labelMonitorBrightnessDown.AutoSize = true;
        //
        // trackBarMonitorBright
        //
        this.trackBarMonitorBright.Name = "trackBarMonitorBright";
        this.trackBarMonitorBright.TabIndex = 18;
        this.trackBarMonitorBright.Location = new Point(x_track, trackBarOffset + monitorBrightnessY);
        this.trackBarMonitorBright.Maximum = 100;
        this.trackBarMonitorBright.LargeChange = 1;
        this.trackBarMonitorBright.Value = 100;
        this.trackBarMonitorBright.ValueChanged += new EventHandler(this.trackBarMonitorBright_ValueChanged);
        //
        // textBoxMonitorBrightness
        //
        this.textBoxMonitorBrightness.Name = "textBoxMonitorBrightness";
        this.textBoxMonitorBrightness.ReadOnly = true;
        this.textBoxMonitorBrightness.TabStop = false;
        this.textBoxMonitorBrightness.Location = new Point(-8 + x_textbox, textBoxOffset + monitorBrightnessY);
        this.textBoxMonitorBrightness.Margin = new Padding(4, 5, 4, 5);
        this.textBoxMonitorBrightness.Size = new Size(62, 31);
        this.textBoxMonitorBrightness.TextAlign = HorizontalAlignment.Right;

        //
        // buttonHide
        //
        this.buttonHide.Name = "buttonHide";
        this.buttonHide.Text = "Hide";
        this.buttonHide.TabIndex = 22;
        this.buttonHide.Location = new Point(x_btns1, monitorBrightnessY);
        this.buttonHide.TextAlign = ContentAlignment.MiddleCenter;
        this.buttonHide.Size = new Size(btns_w, 40);
        this.buttonHide.UseVisualStyleBackColor = true;
        this.buttonHide.Click += new EventHandler(this.buttonHide_Click);
        //
        // buttonExit
        //
        this.buttonExit.Name = "buttonExit";
        this.buttonExit.Text = "Exit";
        this.buttonExit.TabIndex = 1 + buttonHide.TabIndex;
        this.buttonExit.Location = new Point(x_btns2, buttonHide.Location.Y);
        this.buttonExit.TextAlign = ContentAlignment.MiddleCenter;
        this.buttonExit.Size = new Size(btns_w, 40);
        this.buttonExit.UseVisualStyleBackColor = true;
        this.buttonExit.Click += new EventHandler(this.buttonExit_Click);

        //
        // labelMonitorContrastUp
        //
        var monitorContrastY = trackBarSpacing + monitorBrightnessY;
        this.labelMonitorContrastUp.Name = "labelMonitorContrastUp";
        this.labelMonitorContrastUp.Text = "Monitor";
        this.labelMonitorContrastUp.TabStop = false;
        this.labelMonitorContrastUp.Location = new Point(x_label, -6 + monitorContrastY);
        this.labelMonitorContrastUp.Margin = new Padding(4, 0, 4, 0);
        this.labelMonitorContrastUp.Size = new Size(84, 25);
        this.labelMonitorContrastUp.AutoSize = true;
        //
        // labelMonitorContrastDown
        //
        this.labelMonitorContrastDown.Name = "labelMonitorContrastDown";
        this.labelMonitorContrastDown.Text = "Contrast";
        this.labelMonitorContrastDown.TabStop = false;
        this.labelMonitorContrastDown.Location = new Point(x_label, 28 + labelMonitorContrastUp.Location.Y);
        this.labelMonitorContrastDown.Margin = new Padding(4, 0, 4, 0);
        this.labelMonitorContrastDown.Size = new Size(93, 25);
        this.labelMonitorContrastDown.AutoSize = true;
        //
        // trackBarMonitorContrast
        //
        this.trackBarMonitorContrast.Name = "trackBarMonitorContrast";
        this.trackBarMonitorContrast.TabIndex = 26;
        this.trackBarMonitorContrast.Location = new Point(x_track, trackBarOffset + monitorContrastY);
        this.trackBarMonitorContrast.LargeChange = 1;
        this.trackBarMonitorContrast.Maximum = 100;
        this.trackBarMonitorContrast.Value = 100;
        this.trackBarMonitorContrast.ValueChanged += new EventHandler(this.trackBarMonitorContrast_ValueChanged);
        //
        // textBoxMonitorContrast
        //
        this.textBoxMonitorContrast.Name = "textBoxMonitorContrast";
        this.textBoxMonitorContrast.ReadOnly = true;
        this.textBoxMonitorContrast.TabStop = false;
        this.textBoxMonitorContrast.Location = new Point(-8 + x_textbox, textBoxOffset + monitorContrastY);
        this.textBoxMonitorContrast.Margin = new Padding(4, 5, 4, 5);
        this.textBoxMonitorContrast.Size = new Size(62, 31);
        this.textBoxMonitorContrast.TextAlign = HorizontalAlignment.Right;

        //
        // pictureBox
        //
        this.pictureBox.BackColor = SystemColors.Control;
        this.pictureBox.BackgroundImage = global::Gamma_Manager.Properties.Resources.TestMonitor;
        this.pictureBox.BackgroundImageLayout = ImageLayout.Stretch;
        this.pictureBox.ErrorImage = null;
        this.pictureBox.InitialImage = null;
        this.pictureBox.Location = new Point(x_img, 2);
        this.pictureBox.Margin = new Padding(4);
        this.pictureBox.Size = new Size(380, 330);
        this.pictureBox.Name = "pictureBox";
        this.pictureBox.TabIndex = 28;
        this.pictureBox.TabStop = false;
        this.pictureBox.MouseClick += new MouseEventHandler(this.pictureBox_Click);

        //
        // notifyIcon
        //
        this.notifyIcon.Icon = ((Icon)(resources.GetObject("notifyIcon.Icon")));
        this.notifyIcon.Text = "Gamma Manager";
        this.notifyIcon.Visible = true;
        this.notifyIcon.MouseClick       += new MouseEventHandler (this.notifyIcon_Click);
        this.notifyIcon.MouseDoubleClick += new MouseEventHandler (this.notifyIcon_DoubleClick);
        //
        // contextMenu
        //
        this.contextMenu.ImageScalingSize = new Size(20, 20);
        this.contextMenu.Name = "contextMenu";
        this.contextMenu.Size = new Size(61, 4);

        //
        // Content Panel ..
        //
        // Apparently not straight-forward to customize full form's border itself..
        // so instead, we'll put all content in a panel, then paint a border on the panel inside-edge ourselves
        //
        //this.ClientSize = new Size(1196, 390);
        this.ClientSize = new Size(x_img, 36 + trackBarOffset + monitorBrightnessY + topOffset);
        // ^^ but since we anchor to dimensions, we'll set the full window dims first
        //
        contentPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        contentPanel.Location = new Point(2, 2);
        contentPanel.Size = new Size(this.ClientSize.Width - 4, this.ClientSize.Height - 4);
        contentPanel.BackColor = BackgroundColor;

        // now we'll add all the contents to the panel
        contentPanel.Controls.Add(this.labelGamma);
        contentPanel.Controls.Add(this.trackBarGamma);
        contentPanel.Controls.Add(this.textBoxGamma);

        contentPanel.Controls.Add(this.labelBright);
        contentPanel.Controls.Add(this.trackBarBright);
        contentPanel.Controls.Add(this.textBoxBright);

        contentPanel.Controls.Add(this.labelContrast);
        contentPanel.Controls.Add(this.trackBarContrast);
        contentPanel.Controls.Add(this.textBoxContrast);

        contentPanel.Controls.Add(this.labelOverlay);
        contentPanel.Controls.Add(this.trackBarOverlay);
        contentPanel.Controls.Add(this.textBoxOverlay);
        contentPanel.Controls.Add(this.checkBoxOverlay);
        contentPanel.Controls.Add(this.checkBoxOverlayEnforced);

        contentPanel.Controls.Add(this.buttonAllColors);
        contentPanel.Controls.Add(this.buttonRed);
        contentPanel.Controls.Add(this.buttonGreen);
        contentPanel.Controls.Add(this.buttonBlue);

        contentPanel.Controls.Add(this.buttonResync);
        contentPanel.Controls.Add(this.buttonReset);

        contentPanel.Controls.Add(this.comboBoxMonitors);
        contentPanel.Controls.Add(this.buttonForward);
        contentPanel.Controls.Add(this.comboBoxPresets);

        contentPanel.Controls.Add(this.buttonSave);
        contentPanel.Controls.Add(this.buttonDelete);

        contentPanel.Controls.Add(this.labelMonitorBrightnessUp);
        contentPanel.Controls.Add(this.labelMonitorBrightnessDown);
        contentPanel.Controls.Add(this.trackBarMonitorBright);
        contentPanel.Controls.Add(this.textBoxMonitorBrightness);

        contentPanel.Controls.Add(this.labelMonitorContrastDown);
        contentPanel.Controls.Add(this.labelMonitorContrastUp);
        contentPanel.Controls.Add(this.trackBarMonitorContrast);
        contentPanel.Controls.Add(this.textBoxMonitorContrast);

        contentPanel.Controls.Add(this.buttonHide);
        contentPanel.Controls.Add(this.buttonExit);

        //contentPanel.Controls.Add(this.pictureBox);

        // then add that to form
        this.Controls.Add(contentPanel);

        //
        // Window
        //
        this.Name = "Window";
        this.Text = "Gamma Manager";
        this.BackColor = BackgroundColor;
        this.AutoScaleMode = AutoScaleMode.Font;
        this.AutoScaleDimensions = new SizeF(12F, 25F);
        this.TopMost = true;
        this.ShowInTaskbar = false;
        this.StartPosition = FormStartPosition.Manual;
        this.FormBorderStyle = FormBorderStyle.None;
        // ^^ setting border-style None removes titlebar .. but also makes minimize/restore from taskbar icon no longer work
        // .. which is fine by us as we're disabling showing it in taskbar anyway
        //
        this.KeyPreview = true;
        this.KeyDown += new KeyEventHandler(this.Window_KeyDown);
        this.Load += new EventHandler(this.Window_Load);
        this.Resize += new EventHandler(this.Window_Resize);
        this.Activated += new EventHandler(this.Window_Activated);
        this.FormClosing += new FormClosingEventHandler(this.Window_FormClosing);
        this.Paint += new PaintEventHandler(this.Window_Paint);
        // ^^ we use the Paint handler to draw a custom border ourselves on the content-panel inside-edge

        ((System.ComponentModel.ISupportInitialize)(this.trackBarGamma)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.trackBarContrast)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.trackBarBright)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.trackBarMonitorBright)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.trackBarMonitorContrast)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();

        this.ResumeLayout(false);
        this.PerformLayout();
    }



    private Panel contentPanel;

    private Label labelGamma;
    private CustomTrackBar trackBarGamma;
    private TextBox textBoxGamma;

    private Label labelBright;
    private CustomTrackBar trackBarBright;
    private TextBox textBoxBright;

    private Label labelContrast;
    private CustomTrackBar trackBarContrast;
    private TextBox textBoxContrast;

    private Label labelOverlay;
    private CustomTrackBar trackBarOverlay;
    private TextBox textBoxOverlay;
    private CheckBox checkBoxOverlay;
    private CheckBox checkBoxOverlayEnforced;

    private Button buttonAllColors;
    private Button buttonRed;
    private Button buttonGreen;
    private Button buttonBlue;

    private Button buttonResync;
    private Button buttonReset;

    private Krypton.Toolkit.KryptonComboBox comboBoxPresets;
    private Button buttonForward;
    private Krypton.Toolkit.KryptonComboBox comboBoxMonitors;

    private Button buttonSave;
    private Button buttonDelete;

    private Label labelMonitorBrightnessUp;
    private Label labelMonitorBrightnessDown;
    private CustomTrackBar trackBarMonitorBright;
    private TextBox textBoxMonitorBrightness;

    private Label labelMonitorContrastUp;
    private Label labelMonitorContrastDown;
    private CustomTrackBar trackBarMonitorContrast;
    private TextBox textBoxMonitorContrast;

    private Button buttonHide;
    private Button buttonExit;

    private PictureBox pictureBox;
    private NotifyIcon notifyIcon;
    private ContextMenuStrip contextMenu;

}
