using System.Drawing;

namespace Gamma_Manager
{
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

        #region Код, автоматически созданный конструктором форм Windows

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Window));


            this.trackBarGamma = new CustomTrackBar{};
            this.buttonRed = new System.Windows.Forms.Button();
            this.buttonGreen = new System.Windows.Forms.Button();
            this.buttonBlue = new System.Windows.Forms.Button();
            this.buttonAllColors = new System.Windows.Forms.Button();

            this.comboBoxPresets = new Krypton.Toolkit.KryptonComboBox();
            this.comboBoxMonitors = new Krypton.Toolkit.KryptonComboBox();

            this.buttonReset = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.trackBarContrast = new CustomTrackBar { };
            this.trackBarBrightness = new CustomTrackBar { };
            this.textBoxGamma = new System.Windows.Forms.TextBox();
            this.textBoxContrast = new System.Windows.Forms.TextBox();
            this.textBoxBrightness = new System.Windows.Forms.TextBox();
            this.labelGamma = new System.Windows.Forms.Label();
            this.labelContrast = new System.Windows.Forms.Label();
            this.labelBrightness = new System.Windows.Forms.Label();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.labelMonitorBrightnessUp = new System.Windows.Forms.Label();
            this.textBoxMonitorBrightness = new System.Windows.Forms.TextBox();
            this.trackBarMonitorBrightness = new CustomTrackBar { };
            this.labelMonitorBrightnessDown = new System.Windows.Forms.Label();
            this.buttonHide = new System.Windows.Forms.Button();
            this.labelMonitorContrastUp = new System.Windows.Forms.Label();
            this.labelMonitorContrastDown = new System.Windows.Forms.Label();
            this.trackBarMonitorContrast = new CustomTrackBar { };
            this.textBoxMonitorContrast = new System.Windows.Forms.TextBox();
            this.buttonForward = new System.Windows.Forms.Button();
            this.checkBoxExContrast = new System.Windows.Forms.CheckBox();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);

            ((System.ComponentModel.ISupportInitialize)(this.trackBarGamma)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarContrast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBrightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMonitorBrightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMonitorContrast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();

            //
            // labelGamma
            //
            this.labelGamma.AutoSize = true;
            //this.labelGamma.ForeColor = System.Drawing.Color.DarkGray;
            this.labelGamma.Location = new System.Drawing.Point(6, 24);
            this.labelGamma.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelGamma.Name = "labelGamma";
            this.labelGamma.Size = new System.Drawing.Size(86, 25);
            this.labelGamma.TabIndex = 14;
            this.labelGamma.Text = "Gamma";
            //
            // labelBrightness
            //
            this.labelBrightness.AutoSize = true;
            this.labelBrightness.Location = new System.Drawing.Point(6, 78);
            this.labelBrightness.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelBrightness.Name = "labelBrightness";
            this.labelBrightness.Size = new System.Drawing.Size(114, 25);
            this.labelBrightness.TabIndex = 16;
            this.labelBrightness.Text = "Brightness";
            //
            // labelContrast
            //
            this.labelContrast.AutoSize = true;
            this.labelContrast.Location = new System.Drawing.Point(6, 132);
            this.labelContrast.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelContrast.Name = "labelContrast";
            this.labelContrast.Size = new System.Drawing.Size(93, 25);
            this.labelContrast.TabIndex = 15;
            this.labelContrast.Text = "Contrast";

            // 
            // trackBarGamma
            // 
            this.trackBarGamma.LargeChange = 1;
            this.trackBarGamma.Location = new System.Drawing.Point(122, 0);
            this.trackBarGamma.Maximum = 440;
            this.trackBarGamma.Minimum = 30;
            this.trackBarGamma.Name = "trackBarGamma";
            this.trackBarGamma.SmallChange = 5;
            this.trackBarGamma.TabIndex = 0;
            this.trackBarGamma.Value = 100;
            this.trackBarGamma.ValueChanged += new System.EventHandler(this.trackBarGamma_ValueChanged);
            // 
            // trackBarBrightness
            // 
            this.trackBarBrightness.LargeChange = 1;
            this.trackBarBrightness.Location = new System.Drawing.Point(122, 54);
            this.trackBarBrightness.Maximum = 100;
            this.trackBarBrightness.Minimum = -100;
            this.trackBarBrightness.Name = "trackBarBrightness";
            this.trackBarBrightness.SmallChange = 5;
            this.trackBarBrightness.TabIndex = 10;
            this.trackBarBrightness.ValueChanged += new System.EventHandler(this.trackBarBrightness_ValueChanged);
            // 
            // trackBarContrast
            // 
            this.trackBarContrast.LargeChange = 1;
            this.trackBarContrast.Location = new System.Drawing.Point(122, 108);
            this.trackBarContrast.Maximum = 300;
            this.trackBarContrast.Minimum = 10;
            this.trackBarContrast.Name = "trackBarContrast";
            this.trackBarContrast.TabIndex = 9;
            this.trackBarContrast.Value = 10;
            this.trackBarContrast.ValueChanged += new System.EventHandler(this.trackBarContrast_ValueChanged);

            //
            // textBoxGamma
            //
            this.textBoxGamma.Location = new System.Drawing.Point(507, 24);
            this.textBoxGamma.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxGamma.Name = "textBoxGamma";
            this.textBoxGamma.ReadOnly = true;
            this.textBoxGamma.Size = new System.Drawing.Size(62, 31);
            this.textBoxGamma.TabIndex = 11;
            this.textBoxGamma.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // textBoxBrightness
            //
            this.textBoxBrightness.Location = new System.Drawing.Point(507, 78);
            this.textBoxBrightness.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxBrightness.Name = "textBoxBrightness";
            this.textBoxBrightness.ReadOnly = true;
            this.textBoxBrightness.Size = new System.Drawing.Size(62, 31);
            this.textBoxBrightness.TabIndex = 13;
            this.textBoxBrightness.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // textBoxContrast
            //
            this.textBoxContrast.Location = new System.Drawing.Point(507, 132);
            this.textBoxContrast.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxContrast.Name = "textBoxContrast";
            this.textBoxContrast.ReadOnly = true;
            this.textBoxContrast.Size = new System.Drawing.Size(62, 31);
            this.textBoxContrast.TabIndex = 12;
            this.textBoxContrast.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // checkBoxExContrast
            //
            this.checkBoxExContrast.AutoSize = true;
            this.checkBoxExContrast.Location = new System.Drawing.Point(588, 134);
            this.checkBoxExContrast.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBoxExContrast.Name = "checkBoxExContrast";
            this.checkBoxExContrast.Size = new System.Drawing.Size(80, 29);
            this.checkBoxExContrast.TabIndex = 30;
            this.checkBoxExContrast.Text = "+++";
            this.checkBoxExContrast.UseVisualStyleBackColor = true;
            this.checkBoxExContrast.CheckedChanged += new System.EventHandler(this.checkBoxExContrast_CheckedChanged);

            //
            // buttonAllColors
            //
            this.buttonAllColors.Location = new System.Drawing.Point(592, 2);
            this.buttonAllColors.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.buttonAllColors.Name = "buttonAllColors";
            this.buttonAllColors.Size = new System.Drawing.Size(104, 124);
            this.buttonAllColors.TabIndex = 4;
            this.buttonAllColors.Text = "All Colors";
            this.buttonAllColors.UseVisualStyleBackColor = true;
            this.buttonAllColors.Click += new System.EventHandler(this.buttonAllColors_Click);
            // 
            // buttonRed
            // 
            this.buttonRed.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRed.Location = new System.Drawing.Point(694, 2);
            this.buttonRed.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.buttonRed.Name = "buttonRed";
            this.buttonRed.Size = new System.Drawing.Size(104, 40);
            this.buttonRed.TabIndex = 1;
            this.buttonRed.Text = "Red";
            this.buttonRed.UseVisualStyleBackColor = true;
            this.buttonRed.Click += new System.EventHandler(this.buttonRed_Click);
            // 
            // buttonGreen
            // 
            this.buttonGreen.Location = new System.Drawing.Point(694, 42);
            this.buttonGreen.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.buttonGreen.Name = "buttonGreen";
            this.buttonGreen.Size = new System.Drawing.Size(104, 40);
            this.buttonGreen.TabIndex = 2;
            this.buttonGreen.Text = "Green";
            this.buttonGreen.UseVisualStyleBackColor = true;
            this.buttonGreen.Click += new System.EventHandler(this.buttonGreen_Click);
            // 
            // buttonBlue
            // 
            this.buttonBlue.Location = new System.Drawing.Point(694, 84);
            this.buttonBlue.Margin = new System.Windows.Forms.Padding(4, 2, 4, 2);
            this.buttonBlue.Name = "buttonBlue";
            this.buttonBlue.Size = new System.Drawing.Size(104, 40);
            this.buttonBlue.TabIndex = 3;
            this.buttonBlue.Text = "Blue";
            this.buttonBlue.UseVisualStyleBackColor = true;
            this.buttonBlue.Click += new System.EventHandler(this.buttonBlue_Click);

            //
            // comboBoxMonitors
            //
            this.comboBoxMonitors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMonitors.PaletteMode = Krypton.Toolkit.PaletteMode.Office2010BlackDarkMode;
            this.comboBoxMonitors.FormattingEnabled = true;
            this.comboBoxMonitors.Location = new System.Drawing.Point(9, 188);
            this.comboBoxMonitors.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.comboBoxMonitors.Name = "comboBoxMonitors";
            this.comboBoxMonitors.Size = new System.Drawing.Size(193, 33);
            this.comboBoxMonitors.TabIndex = 8;
            this.comboBoxMonitors.SelectedIndexChanged += new System.EventHandler(this.comboBoxMonitors_SelectedIndexChanged);
            //
            // buttonForward
            //
            this.buttonForward.Location = new System.Drawing.Point(213, 188);
            this.buttonForward.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonForward.Name = "buttonForward";
            this.buttonForward.Size = new System.Drawing.Size(44, 44);
            this.buttonForward.TabIndex = 29;
            this.buttonForward.Text = ">";
            this.buttonForward.UseVisualStyleBackColor = true;
            this.buttonForward.Click += new System.EventHandler(this.buttonForward_Click);
            // 
            // comboBoxPresets
            // 
            this.comboBoxPresets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.comboBoxPresets.PaletteMode = Krypton.Toolkit.PaletteMode.Office2010BlackDarkMode;
            this.comboBoxPresets.FormattingEnabled = true;
            this.comboBoxPresets.Location = new System.Drawing.Point(266, 188);
            this.comboBoxPresets.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.comboBoxPresets.Name = "comboBoxPresets";
            this.comboBoxPresets.Size = new System.Drawing.Size(294, 33);
            this.comboBoxPresets.TabIndex = 5;
            this.comboBoxPresets.SelectedIndexChanged += new System.EventHandler(this.comboBoxPresets_SelectedIndexChanged);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(592, 188);
            this.buttonSave.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(104, 44);
            this.buttonSave.TabIndex = 7;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            //
            // buttonDelete
            //
            this.buttonDelete.Location = new System.Drawing.Point(694, 188);
            this.buttonDelete.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(104, 44);
            this.buttonDelete.TabIndex = 17;
            this.buttonDelete.Text = "Delete";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);

            // 
            // labelMonitorBrightnessUp
            // 
            this.labelMonitorBrightnessUp.AutoSize = true;
            this.labelMonitorBrightnessUp.Location = new System.Drawing.Point(4, 244);
            this.labelMonitorBrightnessUp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelMonitorBrightnessUp.Name = "labelMonitorBrightnessUp";
            this.labelMonitorBrightnessUp.Size = new System.Drawing.Size(84, 25);
            this.labelMonitorBrightnessUp.TabIndex = 20;
            this.labelMonitorBrightnessUp.Text = "Monitor";
            //
            // labelMonitorBrightnessDown
            //
            this.labelMonitorBrightnessDown.AutoSize = true;
            this.labelMonitorBrightnessDown.Location = new System.Drawing.Point(4, 272);
            this.labelMonitorBrightnessDown.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelMonitorBrightnessDown.Name = "labelMonitorBrightnessDown";
            this.labelMonitorBrightnessDown.Size = new System.Drawing.Size(114, 25);
            this.labelMonitorBrightnessDown.TabIndex = 21;
            this.labelMonitorBrightnessDown.Text = "Brightness";
            // 
            // trackBarMonitorBrightness
            // 
            this.trackBarMonitorBrightness.LargeChange = 1;
            this.trackBarMonitorBrightness.Location = new System.Drawing.Point(120, 224);
            this.trackBarMonitorBrightness.Maximum = 100;
            this.trackBarMonitorBrightness.Name = "trackBarMonitorBrightness";
            this.trackBarMonitorBrightness.TabIndex = 18;
            this.trackBarMonitorBrightness.Value = 100;
            this.trackBarMonitorBrightness.ValueChanged += new System.EventHandler(this.trackBarMonitorBrightness_ValueChanged);
            //
            // textBoxMonitorBrightness
            //
            this.textBoxMonitorBrightness.Location = new System.Drawing.Point(507, 248);
            this.textBoxMonitorBrightness.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxMonitorBrightness.Name = "textBoxMonitorBrightness";
            this.textBoxMonitorBrightness.ReadOnly = true;
            this.textBoxMonitorBrightness.Size = new System.Drawing.Size(62, 31);
            this.textBoxMonitorBrightness.TabIndex = 19;
            this.textBoxMonitorBrightness.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // buttonReset
            //
            this.buttonReset.Location = new System.Drawing.Point(592, 248);
            this.buttonReset.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(206, 44);
            this.buttonReset.TabIndex = 6;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.buttonReset_Click);

            // 
            // buttonHide
            // 
            this.buttonHide.Location = new System.Drawing.Point(592, 285);
            this.buttonHide.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonHide.Name = "buttonHide";
            this.buttonHide.Size = new System.Drawing.Size(206, 44);
            this.buttonHide.TabIndex = 22;
            this.buttonHide.Text = "Hide";
            this.buttonHide.UseVisualStyleBackColor = true;
            this.buttonHide.Click += new System.EventHandler(this.buttonHide_Click);

            // 
            // labelMonitorContrastUp
            // 
            this.labelMonitorContrastUp.AutoSize = true;
            this.labelMonitorContrastUp.Location = new System.Drawing.Point(6, 266);
            this.labelMonitorContrastUp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelMonitorContrastUp.Name = "labelMonitorContrastUp";
            this.labelMonitorContrastUp.Size = new System.Drawing.Size(84, 25);
            this.labelMonitorContrastUp.TabIndex = 24;
            this.labelMonitorContrastUp.Text = "Monitor";
            // 
            // labelMonitorContrastDown
            // 
            this.labelMonitorContrastDown.AutoSize = true;
            this.labelMonitorContrastDown.Location = new System.Drawing.Point(6, 289);
            this.labelMonitorContrastDown.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelMonitorContrastDown.Name = "labelMonitorContrastDown";
            this.labelMonitorContrastDown.Size = new System.Drawing.Size(93, 25);
            this.labelMonitorContrastDown.TabIndex = 25;
            this.labelMonitorContrastDown.Text = "Contrast";
            // 
            // trackBarMonitorContrast
            // 
            this.trackBarMonitorContrast.LargeChange = 1;
            this.trackBarMonitorContrast.Location = new System.Drawing.Point(122, 267);
            this.trackBarMonitorContrast.Maximum = 100;
            this.trackBarMonitorContrast.Name = "trackBarMonitorContrast";
            this.trackBarMonitorContrast.TabIndex = 26;
            this.trackBarMonitorContrast.Value = 100;
            this.trackBarMonitorContrast.ValueChanged += new System.EventHandler(this.trackBarMonitorContrast_ValueChanged);
            // 
            // textBoxMonitorContrast
            // 
            this.textBoxMonitorContrast.Location = new System.Drawing.Point(507, 272);
            this.textBoxMonitorContrast.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxMonitorContrast.Name = "textBoxMonitorContrast";
            this.textBoxMonitorContrast.ReadOnly = true;
            this.textBoxMonitorContrast.Size = new System.Drawing.Size(62, 31);
            this.textBoxMonitorContrast.TabIndex = 27;
            this.textBoxMonitorContrast.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

            // 
            // notifyIcon
            // 
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Gamma Manager";
            this.notifyIcon.Visible = true;
            this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox1.BackgroundImage = global::Gamma_Manager.Properties.Resources.TestMonitor;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.ErrorImage = null;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(810, 2);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(384, 326);
            this.pictureBox1.TabIndex = 28;
            this.pictureBox1.TabStop = false;
            // 
            // contextMenu
            // 
            this.contextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1193, 336);
            this.Controls.Add(this.buttonBlue);
            this.Controls.Add(this.checkBoxExContrast);
            this.Controls.Add(this.buttonForward);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.textBoxMonitorContrast);
            this.Controls.Add(this.trackBarMonitorContrast);
            this.Controls.Add(this.labelMonitorContrastDown);
            this.Controls.Add(this.labelMonitorContrastUp);
            this.Controls.Add(this.buttonHide);
            this.Controls.Add(this.labelMonitorBrightnessDown);
            this.Controls.Add(this.comboBoxMonitors);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.comboBoxPresets);
            this.Controls.Add(this.labelMonitorBrightnessUp);
            this.Controls.Add(this.textBoxMonitorBrightness);
            this.Controls.Add(this.trackBarMonitorBrightness);
            this.Controls.Add(this.labelBrightness);
            this.Controls.Add(this.labelContrast);
            this.Controls.Add(this.labelGamma);
            this.Controls.Add(this.textBoxBrightness);
            this.Controls.Add(this.textBoxContrast);
            this.Controls.Add(this.textBoxGamma);
            this.Controls.Add(this.trackBarContrast);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.buttonAllColors);
            this.Controls.Add(this.buttonGreen);
            this.Controls.Add(this.buttonRed);
            this.Controls.Add(this.trackBarBrightness);
            this.Controls.Add(this.trackBarGamma);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "Window";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Gamma Manager";
            this.TopMost = true;

            this.Load += new System.EventHandler(this.Window_Load);
            this.Resize += new System.EventHandler(this.Window_Resize);
            this.Activated += new System.EventHandler(this.Window_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Window_FormClosing);

            ((System.ComponentModel.ISupportInitialize)(this.trackBarGamma)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarContrast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBrightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMonitorBrightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMonitorContrast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CustomTrackBar trackBarGamma;
        private CustomTrackBar trackBarContrast;
        private CustomTrackBar trackBarBrightness;
        private CustomTrackBar trackBarMonitorBrightness;
        private CustomTrackBar trackBarMonitorContrast;

        private System.Windows.Forms.Button buttonRed;
        private System.Windows.Forms.Button buttonGreen;
        private System.Windows.Forms.Button buttonBlue;
        private System.Windows.Forms.Button buttonAllColors;

        private Krypton.Toolkit.KryptonComboBox comboBoxPresets;
        private Krypton.Toolkit.KryptonComboBox comboBoxMonitors;

        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.TextBox textBoxGamma;
        private System.Windows.Forms.TextBox textBoxContrast;
        private System.Windows.Forms.TextBox textBoxBrightness;
        private System.Windows.Forms.Label labelGamma;
        private System.Windows.Forms.Label labelContrast;
        private System.Windows.Forms.Label labelBrightness;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Label labelMonitorBrightnessUp;
        private System.Windows.Forms.TextBox textBoxMonitorBrightness;
        private System.Windows.Forms.Label labelMonitorBrightnessDown;
        private System.Windows.Forms.Button buttonHide;
        private System.Windows.Forms.Label labelMonitorContrastUp;
        private System.Windows.Forms.Label labelMonitorContrastDown;
        private System.Windows.Forms.TextBox textBoxMonitorContrast;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button buttonForward;
        private System.Windows.Forms.CheckBox checkBoxExContrast;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip contextMenu;
    }
}

