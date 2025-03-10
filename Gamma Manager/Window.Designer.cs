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
        this.trackBarGamma = new CustomTrackBar{};
        this.buttonRed = new Button();
        this.buttonGreen = new Button();
        this.buttonBlue = new Button();
        this.buttonAllColors = new Button();
        this.buttonResync = new Button();

        this.comboBoxPresets = new Krypton.Toolkit.KryptonComboBox();
        this.comboBoxMonitors = new Krypton.Toolkit.KryptonComboBox();

        this.buttonReset = new Button();
        this.buttonSave = new Button();
        this.trackBarContrast = new CustomTrackBar { };
        this.trackBarBright = new CustomTrackBar { };
        this.textBoxGamma = new TextBox();
        this.textBoxContrast = new TextBox();
        this.textBoxBrightness = new TextBox();
        this.labelGamma = new Label();
        this.labelContrast = new Label();
        this.labelBrightness = new Label();
        this.buttonDelete = new Button();
        this.labelMonitorBrightnessUp = new Label();
        this.textBoxMonitorBrightness = new TextBox();
        this.trackBarMonitorBright = new CustomTrackBar { };
        this.labelMonitorBrightnessDown = new Label();
        this.buttonHide = new Button();
        this.buttonExit = new Button();
        this.labelMonitorContrastUp = new Label();
        this.labelMonitorContrastDown = new Label();
        this.trackBarMonitorContrast = new CustomTrackBar { };
        this.textBoxMonitorContrast = new TextBox();
        this.buttonForward = new Button();
        this.checkBoxExContrast = new CheckBox();
        this.notifyIcon = new NotifyIcon(this.components);
        this.pictureBox1 = new PictureBox();
        this.contextMenu = new ContextMenuStrip(this.components);

        ((System.ComponentModel.ISupportInitialize)(this.trackBarGamma)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.trackBarContrast)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.trackBarBright)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.trackBarMonitorBright)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.trackBarMonitorContrast)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();

        this.SuspendLayout();

        //
        // labelGamma
        //
        this.labelGamma.AutoSize = true;
        //this.labelGamma.ForeColor = Color.DarkGray;
        this.labelGamma.Location = new Point(6, 24);
        this.labelGamma.Margin = new Padding(4, 0, 4, 0);
        this.labelGamma.Name = "labelGamma";
        this.labelGamma.Size = new Size(86, 25);
        this.labelGamma.TabIndex = 14;
        this.labelGamma.Text = "Gamma";
        //
        // labelBrightness
        //
        this.labelBrightness.AutoSize = true;
        this.labelBrightness.Location = new Point(6, 78);
        this.labelBrightness.Margin = new Padding(4, 0, 4, 0);
        this.labelBrightness.Name = "labelBrightness";
        this.labelBrightness.Size = new Size(114, 25);
        this.labelBrightness.TabIndex = 16;
        this.labelBrightness.Text = "Bright";
        //
        // labelContrast
        //
        this.labelContrast.AutoSize = true;
        this.labelContrast.Location = new Point(6, 132);
        this.labelContrast.Margin = new Padding(4, 0, 4, 0);
        this.labelContrast.Name = "labelContrast";
        this.labelContrast.Size = new Size(93, 25);
        this.labelContrast.TabIndex = 15;
        this.labelContrast.Text = "Contrast";

        //
        // trackBarGamma
        //
        this.trackBarGamma.LargeChange = 1;
        this.trackBarGamma.Location = new Point(122, 0);
        this.trackBarGamma.Maximum = 440;
        this.trackBarGamma.Minimum = 30;
        this.trackBarGamma.Name = "trackBarGamma";
        this.trackBarGamma.SmallChange = 5;
        this.trackBarGamma.TabIndex = 0;
        this.trackBarGamma.Value = 100;
        this.trackBarGamma.ValueChanged += new EventHandler(this.trackBarGamma_ValueChanged);
        //
        // trackBarBright
        //
        this.trackBarBright.LargeChange = 1;
        this.trackBarBright.Location = new Point(122, 54);
        this.trackBarBright.Maximum = 100;
        this.trackBarBright.Minimum = -100;
        this.trackBarBright.Name = "trackBarBright";
        this.trackBarBright.SmallChange = 5;
        this.trackBarBright.TabIndex = 10;
        this.trackBarBright.ValueChanged += new EventHandler(this.trackBarBright_ValueChanged);
        //
        // trackBarContrast
        //
        this.trackBarContrast.LargeChange = 1;
        this.trackBarContrast.Location = new Point(122, 108);
        this.trackBarContrast.Maximum = 300;
        this.trackBarContrast.Minimum = 10;
        this.trackBarContrast.Name = "trackBarContrast";
        this.trackBarContrast.TabIndex = 9;
        this.trackBarContrast.Value = 10;
        this.trackBarContrast.ValueChanged += new EventHandler(this.trackBarContrast_ValueChanged);

        //
        // textBoxGamma
        //
        this.textBoxGamma.Location = new Point(507, 24);
        this.textBoxGamma.Margin = new Padding(4, 5, 4, 5);
        this.textBoxGamma.Name = "textBoxGamma";
        this.textBoxGamma.ReadOnly = true;
        this.textBoxGamma.Size = new Size(62, 31);
        this.textBoxGamma.TabIndex = 11;
        this.textBoxGamma.TextAlign = HorizontalAlignment.Center;
        //
        // textBoxBrightness
        //
        this.textBoxBrightness.Location = new Point(507, 78);
        this.textBoxBrightness.Margin = new Padding(4, 5, 4, 5);
        this.textBoxBrightness.Name = "textBoxBrightness";
        this.textBoxBrightness.ReadOnly = true;
        this.textBoxBrightness.Size = new Size(62, 31);
        this.textBoxBrightness.TabIndex = 13;
        this.textBoxBrightness.TextAlign = HorizontalAlignment.Center;
        //
        // textBoxContrast
        //
        this.textBoxContrast.Location = new Point(507, 132);
        this.textBoxContrast.Margin = new Padding(4, 5, 4, 5);
        this.textBoxContrast.Name = "textBoxContrast";
        this.textBoxContrast.ReadOnly = true;
        this.textBoxContrast.Size = new Size(62, 31);
        this.textBoxContrast.TabIndex = 12;
        this.textBoxContrast.TextAlign = HorizontalAlignment.Center;
        //
        // checkBoxExContrast
        //
        this.checkBoxExContrast.AutoSize = true;
        this.checkBoxExContrast.Location = new Point(588, 134);
        this.checkBoxExContrast.Margin = new Padding(4, 5, 4, 5);
        this.checkBoxExContrast.Name = "checkBoxExContrast";
        this.checkBoxExContrast.Size = new Size(80, 29);
        this.checkBoxExContrast.TabIndex = 30;
        this.checkBoxExContrast.Text = "+++";
        this.checkBoxExContrast.UseVisualStyleBackColor = true;
        this.checkBoxExContrast.CheckedChanged += new EventHandler(this.checkBoxExContrast_CheckedChanged);

        //
        // buttonAllColors
        //
        this.buttonAllColors.Location = new Point(592, 4);
        this.buttonAllColors.Name = "buttonAllColors";
        this.buttonAllColors.Size = new Size(104, 118);
        this.buttonAllColors.TabIndex = 4;
        this.buttonAllColors.Text = "All Colors";
        this.buttonAllColors.UseVisualStyleBackColor = true;
        this.buttonAllColors.Click += new EventHandler(this.buttonAllColors_Click);
        //
        // buttonRed
        //
        this.buttonRed.Location = new Point(694, 4);
        this.buttonRed.Name = "buttonRed";
        this.buttonRed.Size = new Size(104, 40);
        this.buttonRed.TabIndex = 1;
        this.buttonRed.Text = "Red";
        this.buttonRed.UseVisualStyleBackColor = true;
        this.buttonRed.Click += new EventHandler(this.buttonRed_Click);
        //
        // buttonGreen
        //
        this.buttonGreen.Location = new Point(694, 42);
        this.buttonGreen.Name = "buttonGreen";
        this.buttonGreen.Size = new Size(104, 40);
        this.buttonGreen.TabIndex = 2;
        this.buttonGreen.Text = "Green";
        this.buttonGreen.UseVisualStyleBackColor = true;
        this.buttonGreen.Click += new EventHandler(this.buttonGreen_Click);
        //
        // buttonBlue
        //
        this.buttonBlue.Location = new Point(694, 80);
        this.buttonBlue.Name = "buttonBlue";
        this.buttonBlue.Size = new Size(104, 40);
        this.buttonBlue.TabIndex = 3;
        this.buttonBlue.Text = "Blue";
        this.buttonBlue.UseVisualStyleBackColor = true;
        this.buttonBlue.Click += new EventHandler(this.buttonBlue_Click);
        //
        // buttonResync
        //
        this.buttonResync.Location = new Point(694, 134);
        this.buttonResync.Name = "buttonBlue";
        this.buttonResync.Size = new Size(104, 40);
        //this.buttonResync.TabIndex = 3;
        this.buttonResync.Text = "Sync";
        this.buttonResync.UseVisualStyleBackColor = true;
        this.buttonResync.Click += new EventHandler(this.buttonResync_Click);

        //
        // comboBoxMonitors
        //
        this.comboBoxMonitors.DropDownStyle = ComboBoxStyle.DropDownList;
        this.comboBoxMonitors.PaletteMode = Krypton.Toolkit.PaletteMode.Office2010BlackDarkMode;
        this.comboBoxMonitors.FormattingEnabled = true;
        this.comboBoxMonitors.Location = new Point(9, 188);
        this.comboBoxMonitors.Margin = new Padding(4, 5, 4, 5);
        this.comboBoxMonitors.Name = "comboBoxMonitors";
        this.comboBoxMonitors.Size = new Size(193, 33);
        this.comboBoxMonitors.TabIndex = 8;
        this.comboBoxMonitors.SelectedIndexChanged += new EventHandler(this.comboBoxMonitors_SelectedIndexChanged);
        //
        // buttonForward
        //
        this.buttonForward.Location = new Point(213, 188);
        this.buttonForward.Margin = new Padding(4, 5, 4, 5);
        this.buttonForward.Name = "buttonForward";
        this.buttonForward.Size = new Size(44, 44);
        this.buttonForward.TabIndex = 29;
        this.buttonForward.Text = ">";
        this.buttonForward.UseVisualStyleBackColor = true;
        this.buttonForward.Click += new EventHandler(this.buttonForward_Click);
        //
        // comboBoxPresets
        //
        this.comboBoxPresets.DropDownStyle = ComboBoxStyle.DropDown;
        this.comboBoxPresets.PaletteMode = Krypton.Toolkit.PaletteMode.Office2010BlackDarkMode;
        this.comboBoxPresets.FormattingEnabled = true;
        this.comboBoxPresets.Location = new Point(266, 188);
        this.comboBoxPresets.Margin = new Padding(4, 5, 4, 5);
        this.comboBoxPresets.Name = "comboBoxPresets";
        this.comboBoxPresets.Size = new Size(294, 33);
        this.comboBoxPresets.TabIndex = 5;
        this.comboBoxPresets.SelectedIndexChanged += new EventHandler(this.comboBoxPresets_SelectedIndexChanged);
        //
        // buttonSave
        //
        this.buttonSave.Location = new Point(592, 188);
        this.buttonSave.Margin = new Padding(4, 5, 4, 5);
        this.buttonSave.Name = "buttonSave";
        this.buttonSave.Size = new Size(104, 44);
        this.buttonSave.TabIndex = 7;
        this.buttonSave.Text = "Save";
        this.buttonSave.UseVisualStyleBackColor = true;
        this.buttonSave.Click += new EventHandler(this.buttonSave_Click);
        //
        // buttonDelete
        //
        this.buttonDelete.Location = new Point(694, 188);
        this.buttonDelete.Margin = new Padding(4, 5, 4, 5);
        this.buttonDelete.Name = "buttonDelete";
        this.buttonDelete.Size = new Size(104, 44);
        this.buttonDelete.TabIndex = 17;
        this.buttonDelete.Text = "Delete";
        this.buttonDelete.UseVisualStyleBackColor = true;
        this.buttonDelete.Click += new EventHandler(this.buttonDelete_Click);

        //
        // labelMonitorBrightnessUp
        //
        this.labelMonitorBrightnessUp.AutoSize = true;
        this.labelMonitorBrightnessUp.Location = new Point(4, 244);
        this.labelMonitorBrightnessUp.Margin = new Padding(4, 0, 4, 0);
        this.labelMonitorBrightnessUp.Name = "labelMonitorBrightnessUp";
        this.labelMonitorBrightnessUp.Size = new Size(84, 25);
        this.labelMonitorBrightnessUp.TabIndex = 20;
        this.labelMonitorBrightnessUp.Text = "Monitor";
        //
        // labelMonitorBrightnessDown
        //
        this.labelMonitorBrightnessDown.AutoSize = true;
        this.labelMonitorBrightnessDown.Location = new Point(4, 272);
        this.labelMonitorBrightnessDown.Margin = new Padding(4, 0, 4, 0);
        this.labelMonitorBrightnessDown.Name = "labelMonitorBrightnessDown";
        this.labelMonitorBrightnessDown.Size = new Size(114, 25);
        this.labelMonitorBrightnessDown.TabIndex = 21;
        this.labelMonitorBrightnessDown.Text = "Bright";
        //
        // trackBarMonitorBright
        //
        this.trackBarMonitorBright.LargeChange = 1;
        this.trackBarMonitorBright.Location = new Point(120, 224);
        this.trackBarMonitorBright.Maximum = 100;
        this.trackBarMonitorBright.Name = "trackBarMonitorBright";
        this.trackBarMonitorBright.TabIndex = 18;
        this.trackBarMonitorBright.Value = 100;
        this.trackBarMonitorBright.ValueChanged += new EventHandler(this.trackBarMonitorBright_ValueChanged);
        //
        // textBoxMonitorBrightness
        //
        this.textBoxMonitorBrightness.Location = new Point(507, 248);
        this.textBoxMonitorBrightness.Margin = new Padding(4, 5, 4, 5);
        this.textBoxMonitorBrightness.Name = "textBoxMonitorBrightness";
        this.textBoxMonitorBrightness.ReadOnly = true;
        this.textBoxMonitorBrightness.Size = new Size(62, 31);
        this.textBoxMonitorBrightness.TabIndex = 19;
        this.textBoxMonitorBrightness.TextAlign = HorizontalAlignment.Center;
        //
        // buttonReset
        //
        this.buttonReset.Location = new Point(592, 248);
        this.buttonReset.Margin = new Padding(4, 5, 4, 5);
        this.buttonReset.Name = "buttonReset";
        this.buttonReset.Size = new Size(206, 40);
        this.buttonReset.TabIndex = 6;
        this.buttonReset.Text = "Reset";
        this.buttonReset.UseVisualStyleBackColor = true;
        this.buttonReset.Click += new EventHandler(this.buttonReset_Click);

        //
        // buttonHide
        //
        this.buttonHide.Location = new Point(592, 286);
        this.buttonHide.Margin = new Padding(4, 5, 4, 5);
        this.buttonHide.Name = "buttonHide";
        this.buttonHide.Size = new Size(104, 40);
        this.buttonHide.TabIndex = 22;
        this.buttonHide.Text = "Hide";
        this.buttonHide.UseVisualStyleBackColor = true;
        this.buttonHide.Click += new EventHandler(this.buttonHide_Click);
        //
        // buttonExit
        //
        this.buttonExit.Location = new Point(694, 286);
        this.buttonExit.Margin = new Padding(4, 5, 4, 5);
        this.buttonExit.Name = "buttonExit";
        this.buttonExit.Size = new Size(104, 40);
        this.buttonExit.TabIndex = 23;
        this.buttonExit.Text = "Exit";
        this.buttonExit.UseVisualStyleBackColor = true;
        this.buttonExit.Click += new EventHandler(this.buttonExit_Click);

        //
        // labelMonitorContrastUp
        //
        this.labelMonitorContrastUp.AutoSize = true;
        this.labelMonitorContrastUp.Location = new Point(6, 266);
        this.labelMonitorContrastUp.Margin = new Padding(4, 0, 4, 0);
        this.labelMonitorContrastUp.Name = "labelMonitorContrastUp";
        this.labelMonitorContrastUp.Size = new Size(84, 25);
        this.labelMonitorContrastUp.TabIndex = 24;
        this.labelMonitorContrastUp.Text = "Monitor";
        //
        // labelMonitorContrastDown
        //
        this.labelMonitorContrastDown.AutoSize = true;
        this.labelMonitorContrastDown.Location = new Point(6, 289);
        this.labelMonitorContrastDown.Margin = new Padding(4, 0, 4, 0);
        this.labelMonitorContrastDown.Name = "labelMonitorContrastDown";
        this.labelMonitorContrastDown.Size = new Size(93, 25);
        this.labelMonitorContrastDown.TabIndex = 25;
        this.labelMonitorContrastDown.Text = "Contrast";
        //
        // trackBarMonitorContrast
        //
        this.trackBarMonitorContrast.LargeChange = 1;
        this.trackBarMonitorContrast.Location = new Point(122, 267);
        this.trackBarMonitorContrast.Maximum = 100;
        this.trackBarMonitorContrast.Name = "trackBarMonitorContrast";
        this.trackBarMonitorContrast.TabIndex = 26;
        this.trackBarMonitorContrast.Value = 100;
        this.trackBarMonitorContrast.ValueChanged += new EventHandler(this.trackBarMonitorContrast_ValueChanged);
        //
        // textBoxMonitorContrast
        //
        this.textBoxMonitorContrast.Location = new Point(507, 272);
        this.textBoxMonitorContrast.Margin = new Padding(4, 5, 4, 5);
        this.textBoxMonitorContrast.Name = "textBoxMonitorContrast";
        this.textBoxMonitorContrast.ReadOnly = true;
        this.textBoxMonitorContrast.Size = new Size(62, 31);
        this.textBoxMonitorContrast.TabIndex = 27;
        this.textBoxMonitorContrast.TextAlign = HorizontalAlignment.Center;

        //
        // notifyIcon
        //
        this.notifyIcon.Icon = ((Icon)(resources.GetObject("notifyIcon.Icon")));
        this.notifyIcon.Text = "Gamma Manager";
        this.notifyIcon.Visible = true;
        this.notifyIcon.MouseClick       += new MouseEventHandler (this.notifyIcon_Click);
        this.notifyIcon.MouseDoubleClick += new MouseEventHandler (this.notifyIcon_DoubleClick);
        //
        // pictureBox1
        //
        this.pictureBox1.BackColor = SystemColors.Control;
        this.pictureBox1.BackgroundImage = global::Gamma_Manager.Properties.Resources.TestMonitor;
        this.pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
        this.pictureBox1.ErrorImage = null;
        this.pictureBox1.InitialImage = null;
        this.pictureBox1.Location = new Point(810, 2);
        this.pictureBox1.Margin = new Padding(4);
        this.pictureBox1.Size = new Size(380, 330);
        this.pictureBox1.Name = "pictureBox1";
        this.pictureBox1.TabIndex = 28;
        this.pictureBox1.TabStop = false;
        this.pictureBox1.MouseClick += new MouseEventHandler(this.pictureBox_Click);

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
        this.ClientSize = new Size(1196, 338);
        // ^^ but since we anchor to dimensions, we'll set the full window dims first
        //
        contentPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        contentPanel.Location = new Point(2, 2);
        contentPanel.Size = new Size(this.ClientSize.Width - 4, this.ClientSize.Height - 4);
        contentPanel.BackColor = BackgroundColor;
        // now we'll add all the contents to the panel
        contentPanel.Controls.Add(this.checkBoxExContrast);
        contentPanel.Controls.Add(this.buttonForward);
        contentPanel.Controls.Add(this.pictureBox1);
        contentPanel.Controls.Add(this.textBoxMonitorContrast);
        contentPanel.Controls.Add(this.trackBarMonitorContrast);
        contentPanel.Controls.Add(this.labelMonitorContrastDown);
        contentPanel.Controls.Add(this.labelMonitorContrastUp);
        contentPanel.Controls.Add(this.buttonHide);
        contentPanel.Controls.Add(this.buttonExit);
        contentPanel.Controls.Add(this.labelMonitorBrightnessDown);
        contentPanel.Controls.Add(this.comboBoxMonitors);
        contentPanel.Controls.Add(this.buttonDelete);
        contentPanel.Controls.Add(this.buttonSave);
        contentPanel.Controls.Add(this.comboBoxPresets);
        contentPanel.Controls.Add(this.labelMonitorBrightnessUp);
        contentPanel.Controls.Add(this.textBoxMonitorBrightness);
        contentPanel.Controls.Add(this.trackBarMonitorBright);
        contentPanel.Controls.Add(this.labelBrightness);
        contentPanel.Controls.Add(this.labelContrast);
        contentPanel.Controls.Add(this.labelGamma);
        contentPanel.Controls.Add(this.textBoxBrightness);
        contentPanel.Controls.Add(this.textBoxContrast);
        contentPanel.Controls.Add(this.textBoxGamma);
        contentPanel.Controls.Add(this.trackBarContrast);
        contentPanel.Controls.Add(this.buttonReset);
        contentPanel.Controls.Add(this.buttonAllColors);
        contentPanel.Controls.Add(this.buttonRed);
        contentPanel.Controls.Add(this.buttonGreen);
        contentPanel.Controls.Add(this.buttonBlue);
        contentPanel.Controls.Add(this.buttonResync);
        contentPanel.Controls.Add(this.trackBarBright);
        contentPanel.Controls.Add(this.trackBarGamma);
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
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();

        this.ResumeLayout(false);
        this.PerformLayout();
    }



    private Panel contentPanel;

    private Label labelGamma;
    private Label labelBrightness;
    private Label labelContrast;

    private TextBox textBoxGamma;
    private TextBox textBoxBrightness;
    private TextBox textBoxContrast;

    private CustomTrackBar trackBarGamma;
    private CustomTrackBar trackBarContrast;
    private CustomTrackBar trackBarBright;

    private CustomTrackBar trackBarMonitorBright;
    private CustomTrackBar trackBarMonitorContrast;

    private Button buttonRed;
    private Button buttonGreen;
    private Button buttonBlue;
    private Button buttonAllColors;
    private Button buttonResync;

    private Krypton.Toolkit.KryptonComboBox comboBoxPresets;
    private Krypton.Toolkit.KryptonComboBox comboBoxMonitors;

    private Button buttonForward;
    private CheckBox checkBoxExContrast;

    private Button buttonSave;
    private Button buttonDelete;


    private Label labelMonitorBrightnessUp;
    private Label labelMonitorBrightnessDown;
    private TextBox textBoxMonitorBrightness;

    private Button buttonReset;
    private Button buttonHide;
    private Button buttonExit;

    private Label labelMonitorContrastUp;
    private Label labelMonitorContrastDown;
    private TextBox textBoxMonitorContrast;

    private PictureBox pictureBox1;
    private NotifyIcon notifyIcon;
    private ContextMenuStrip contextMenu;
}
