using BridgeVueApp.Models;

namespace BridgeVueApp
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            menuStrip1 = new MenuStrip();
            adminToolStripMenuItem = new ToolStripMenuItem();
            predictionsToolStripMenuItem = new ToolStripMenuItem();
            setupToolStripMenuItem = new ToolStripMenuItem();
            dailyBehaviorEntryToolStripMenuItem = new ToolStripMenuItem();
            pbxLogo = new PictureBox();
            lblDailyBehaviorEntry = new Label();
            cmbStudent = new ComboBox();
            studentProfileBindingSource = new BindingSource(components);
            dateTimePicker1 = new DateTimePicker();
            cmbLevel = new ComboBox();
            cmbStep = new ComboBox();
            ckbVerbalAggression = new CheckBox();
            ckbPhysicalAggression = new CheckBox();
            ckbElopement = new CheckBox();
            ckbOutOfSpot = new CheckBox();
            ckbWorkRefusal = new CheckBox();
            ckbProvokingPeers = new CheckBox();
            ckbInappropriateLanguage = new CheckBox();
            ckbOutOfLane = new CheckBox();
            rtbStaffComments = new RichTextBox();
            lblDailyComments = new Label();
            lblEmotionCheckin = new Label();
            tblEmotionIcons = new TableLayoutPanel();
            lblSelectedEmotion = new Label();
            lZoneOfRegulation = new Label();
            tblZoneColors = new TableLayoutPanel();
            lblSelectedZone = new Label();
            btnSaveDailyEntry = new Button();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbxLogo).BeginInit();
            ((System.ComponentModel.ISupportInitialize)studentProfileBindingSource).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { adminToolStripMenuItem, dailyBehaviorEntryToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(552, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // adminToolStripMenuItem
            // 
            adminToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { predictionsToolStripMenuItem, setupToolStripMenuItem });
            adminToolStripMenuItem.Name = "adminToolStripMenuItem";
            adminToolStripMenuItem.Size = new Size(55, 20);
            adminToolStripMenuItem.Text = "Admin";
            // 
            // predictionsToolStripMenuItem
            // 
            predictionsToolStripMenuItem.Name = "predictionsToolStripMenuItem";
            predictionsToolStripMenuItem.Size = new Size(133, 22);
            predictionsToolStripMenuItem.Text = "Predictions";
            predictionsToolStripMenuItem.Click += predictionsToolStripMenuItem_Click;
            // 
            // setupToolStripMenuItem
            // 
            setupToolStripMenuItem.Name = "setupToolStripMenuItem";
            setupToolStripMenuItem.Size = new Size(133, 22);
            setupToolStripMenuItem.Text = "Setup";
            setupToolStripMenuItem.Click += setupToolStripMenuItem_Click;
            // 
            // dailyBehaviorEntryToolStripMenuItem
            // 
            dailyBehaviorEntryToolStripMenuItem.Name = "dailyBehaviorEntryToolStripMenuItem";
            dailyBehaviorEntryToolStripMenuItem.Size = new Size(124, 20);
            dailyBehaviorEntryToolStripMenuItem.Text = "Daily Behavior Entry";
            // 
            // pbxLogo
            // 
            pbxLogo.Image = Properties.Resources.BridgeVue_Logo;
            pbxLogo.Location = new Point(364, 473);
            pbxLogo.MaximumSize = new Size(600, 400);
            pbxLogo.Name = "pbxLogo";
            pbxLogo.Size = new Size(176, 129);
            pbxLogo.SizeMode = PictureBoxSizeMode.Zoom;
            pbxLogo.TabIndex = 1;
            pbxLogo.TabStop = false;
            pbxLogo.Click += pbxLogo_Click;
            // 
            // lblDailyBehaviorEntry
            // 
            lblDailyBehaviorEntry.AutoSize = true;
            lblDailyBehaviorEntry.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblDailyBehaviorEntry.Location = new Point(193, 32);
            lblDailyBehaviorEntry.Name = "lblDailyBehaviorEntry";
            lblDailyBehaviorEntry.Size = new Size(161, 21);
            lblDailyBehaviorEntry.TabIndex = 2;
            lblDailyBehaviorEntry.Text = "Record daily behavior\r\n";
            lblDailyBehaviorEntry.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // cmbStudent
            // 
            cmbStudent.DataSource = studentProfileBindingSource;
            cmbStudent.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStudent.Font = new Font("Segoe UI", 12F);
            cmbStudent.FormattingEnabled = true;
            cmbStudent.Location = new Point(25, 80);
            cmbStudent.Name = "cmbStudent";
            cmbStudent.Size = new Size(345, 29);
            cmbStudent.TabIndex = 3;
            // 
            // studentProfileBindingSource
            // 
            studentProfileBindingSource.CurrentChanged += studentProfileBindingSource_CurrentChanged;
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dateTimePicker1.Format = DateTimePickerFormat.Short;
            dateTimePicker1.Location = new Point(25, 30);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(107, 29);
            dateTimePicker1.TabIndex = 4;
            // 
            // cmbLevel
            // 
            cmbLevel.Font = new Font("Segoe UI", 12F);
            cmbLevel.FormattingEnabled = true;
            cmbLevel.Items.AddRange(new object[] { "1", "2", "3", "4", "5" });
            cmbLevel.Location = new Point(382, 80);
            cmbLevel.Name = "cmbLevel";
            cmbLevel.Size = new Size(67, 29);
            cmbLevel.TabIndex = 5;
            cmbLevel.Text = "Level";
            // 
            // cmbStep
            // 
            cmbStep.Font = new Font("Segoe UI", 12F);
            cmbStep.FormattingEnabled = true;
            cmbStep.Items.AddRange(new object[] { "1", "2", "3", "4", "5" });
            cmbStep.Location = new Point(457, 80);
            cmbStep.Name = "cmbStep";
            cmbStep.Size = new Size(77, 29);
            cmbStep.TabIndex = 6;
            cmbStep.Text = "Step";
            // 
            // ckbVerbalAggression
            // 
            ckbVerbalAggression.AutoSize = true;
            ckbVerbalAggression.Font = new Font("Segoe UI", 12F);
            ckbVerbalAggression.Location = new Point(25, 126);
            ckbVerbalAggression.Name = "ckbVerbalAggression";
            ckbVerbalAggression.RightToLeft = RightToLeft.No;
            ckbVerbalAggression.Size = new Size(155, 25);
            ckbVerbalAggression.TabIndex = 8;
            ckbVerbalAggression.Text = "Verbal Aggression";
            ckbVerbalAggression.UseVisualStyleBackColor = true;
            // 
            // ckbPhysicalAggression
            // 
            ckbPhysicalAggression.AutoSize = true;
            ckbPhysicalAggression.Font = new Font("Segoe UI", 12F);
            ckbPhysicalAggression.Location = new Point(203, 126);
            ckbPhysicalAggression.Name = "ckbPhysicalAggression";
            ckbPhysicalAggression.RightToLeft = RightToLeft.No;
            ckbPhysicalAggression.Size = new Size(167, 25);
            ckbPhysicalAggression.TabIndex = 9;
            ckbPhysicalAggression.Text = "Physical Aggression";
            ckbPhysicalAggression.UseVisualStyleBackColor = true;
            // 
            // ckbElopement
            // 
            ckbElopement.AutoSize = true;
            ckbElopement.Font = new Font("Segoe UI", 12F);
            ckbElopement.Location = new Point(25, 156);
            ckbElopement.Name = "ckbElopement";
            ckbElopement.RightToLeft = RightToLeft.No;
            ckbElopement.Size = new Size(103, 25);
            ckbElopement.TabIndex = 10;
            ckbElopement.Text = "Elopement";
            ckbElopement.UseVisualStyleBackColor = true;
            // 
            // ckbOutOfSpot
            // 
            ckbOutOfSpot.AutoSize = true;
            ckbOutOfSpot.Font = new Font("Segoe UI", 12F);
            ckbOutOfSpot.Location = new Point(419, 126);
            ckbOutOfSpot.Name = "ckbOutOfSpot";
            ckbOutOfSpot.RightToLeft = RightToLeft.No;
            ckbOutOfSpot.Size = new Size(112, 25);
            ckbOutOfSpot.TabIndex = 11;
            ckbOutOfSpot.Text = "Out Of Spot";
            ckbOutOfSpot.UseVisualStyleBackColor = true;
            // 
            // ckbWorkRefusal
            // 
            ckbWorkRefusal.AutoSize = true;
            ckbWorkRefusal.Font = new Font("Segoe UI", 12F);
            ckbWorkRefusal.Location = new Point(203, 156);
            ckbWorkRefusal.Name = "ckbWorkRefusal";
            ckbWorkRefusal.RightToLeft = RightToLeft.No;
            ckbWorkRefusal.Size = new Size(121, 25);
            ckbWorkRefusal.TabIndex = 12;
            ckbWorkRefusal.Text = "Work Refusal";
            ckbWorkRefusal.UseVisualStyleBackColor = true;
            // 
            // ckbProvokingPeers
            // 
            ckbProvokingPeers.AutoSize = true;
            ckbProvokingPeers.Font = new Font("Segoe UI", 12F);
            ckbProvokingPeers.Location = new Point(25, 187);
            ckbProvokingPeers.Name = "ckbProvokingPeers";
            ckbProvokingPeers.RightToLeft = RightToLeft.No;
            ckbProvokingPeers.Size = new Size(141, 25);
            ckbProvokingPeers.TabIndex = 13;
            ckbProvokingPeers.Text = "Provoking Peers";
            ckbProvokingPeers.UseVisualStyleBackColor = true;
            // 
            // ckbInappropriateLanguage
            // 
            ckbInappropriateLanguage.AutoSize = true;
            ckbInappropriateLanguage.Font = new Font("Segoe UI", 12F);
            ckbInappropriateLanguage.Location = new Point(203, 187);
            ckbInappropriateLanguage.Name = "ckbInappropriateLanguage";
            ckbInappropriateLanguage.RightToLeft = RightToLeft.No;
            ckbInappropriateLanguage.Size = new Size(195, 25);
            ckbInappropriateLanguage.TabIndex = 14;
            ckbInappropriateLanguage.Text = "Inappropriate Language";
            ckbInappropriateLanguage.UseVisualStyleBackColor = true;
            // 
            // ckbOutOfLane
            // 
            ckbOutOfLane.AutoSize = true;
            ckbOutOfLane.Font = new Font("Segoe UI", 12F);
            ckbOutOfLane.Location = new Point(419, 156);
            ckbOutOfLane.Name = "ckbOutOfLane";
            ckbOutOfLane.RightToLeft = RightToLeft.No;
            ckbOutOfLane.Size = new Size(113, 25);
            ckbOutOfLane.TabIndex = 15;
            ckbOutOfLane.Text = "Out Of Lane";
            ckbOutOfLane.UseVisualStyleBackColor = true;
            // 
            // rtbStaffComments
            // 
            rtbStaffComments.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            rtbStaffComments.Location = new Point(28, 497);
            rtbStaffComments.Name = "rtbStaffComments";
            rtbStaffComments.Size = new Size(330, 107);
            rtbStaffComments.TabIndex = 17;
            rtbStaffComments.Text = "";
            // 
            // lblDailyComments
            // 
            lblDailyComments.AutoSize = true;
            lblDailyComments.Font = new Font("Segoe UI", 12F);
            lblDailyComments.Location = new Point(32, 473);
            lblDailyComments.Name = "lblDailyComments";
            lblDailyComments.Size = new Size(86, 21);
            lblDailyComments.TabIndex = 18;
            lblDailyComments.Text = "Comments";
            // 
            // lblEmotionCheckin
            // 
            lblEmotionCheckin.AutoSize = true;
            lblEmotionCheckin.Font = new Font("Segoe UI", 12F);
            lblEmotionCheckin.Location = new Point(29, 349);
            lblEmotionCheckin.Name = "lblEmotionCheckin";
            lblEmotionCheckin.Size = new Size(133, 21);
            lblEmotionCheckin.TabIndex = 19;
            lblEmotionCheckin.Text = "Emotion Check-in";
            // 
            // tblEmotionIcons
            // 
            tblEmotionIcons.ColumnCount = 5;
            tblEmotionIcons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tblEmotionIcons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tblEmotionIcons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tblEmotionIcons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tblEmotionIcons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tblEmotionIcons.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tblEmotionIcons.Location = new Point(26, 373);
            tblEmotionIcons.Name = "tblEmotionIcons";
            tblEmotionIcons.RowCount = 1;
            tblEmotionIcons.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tblEmotionIcons.Size = new Size(508, 90);
            tblEmotionIcons.TabIndex = 20;
            // 
            // lblSelectedEmotion
            // 
            lblSelectedEmotion.AutoSize = true;
            lblSelectedEmotion.Location = new Point(416, 355);
            lblSelectedEmotion.Name = "lblSelectedEmotion";
            lblSelectedEmotion.Size = new Size(99, 15);
            lblSelectedEmotion.TabIndex = 21;
            lblSelectedEmotion.Text = "Selected Emotion";
            // 
            // lZoneOfRegulation
            // 
            lZoneOfRegulation.AutoSize = true;
            lZoneOfRegulation.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lZoneOfRegulation.Location = new Point(28, 227);
            lZoneOfRegulation.Margin = new Padding(2, 0, 2, 0);
            lZoneOfRegulation.Name = "lZoneOfRegulation";
            lZoneOfRegulation.Size = new Size(142, 21);
            lZoneOfRegulation.TabIndex = 22;
            lZoneOfRegulation.Text = "Zone of Regulation";
            // 
            // tblZoneColors
            // 
            tblZoneColors.ColumnCount = 4;
            tblZoneColors.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tblZoneColors.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tblZoneColors.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tblZoneColors.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tblZoneColors.Location = new Point(26, 248);
            tblZoneColors.Margin = new Padding(2, 2, 2, 2);
            tblZoneColors.Name = "tblZoneColors";
            tblZoneColors.RowCount = 1;
            tblZoneColors.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tblZoneColors.Size = new Size(508, 90);
            tblZoneColors.TabIndex = 23;
            // 
            // lblSelectedZone
            // 
            lblSelectedZone.AutoSize = true;
            lblSelectedZone.Location = new Point(416, 227);
            lblSelectedZone.Margin = new Padding(2, 0, 2, 0);
            lblSelectedZone.Name = "lblSelectedZone";
            lblSelectedZone.Size = new Size(81, 15);
            lblSelectedZone.TabIndex = 24;
            lblSelectedZone.Text = "Selected Zone";
            // 
            // btnSaveDailyEntry
            // 
            btnSaveDailyEntry.BackColor = SystemColors.GradientActiveCaption;
            btnSaveDailyEntry.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSaveDailyEntry.Location = new Point(426, 31);
            btnSaveDailyEntry.Name = "btnSaveDailyEntry";
            btnSaveDailyEntry.Size = new Size(108, 38);
            btnSaveDailyEntry.TabIndex = 25;
            btnSaveDailyEntry.Text = "Save Entry";
            btnSaveDailyEntry.UseVisualStyleBackColor = false;
            btnSaveDailyEntry.Click += btnSaveDailyEntry_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(552, 614);
            Controls.Add(btnSaveDailyEntry);
            Controls.Add(lblSelectedZone);
            Controls.Add(tblZoneColors);
            Controls.Add(lZoneOfRegulation);
            Controls.Add(lblSelectedEmotion);
            Controls.Add(tblEmotionIcons);
            Controls.Add(lblEmotionCheckin);
            Controls.Add(lblDailyComments);
            Controls.Add(rtbStaffComments);
            Controls.Add(ckbOutOfLane);
            Controls.Add(ckbInappropriateLanguage);
            Controls.Add(ckbProvokingPeers);
            Controls.Add(ckbWorkRefusal);
            Controls.Add(ckbOutOfSpot);
            Controls.Add(ckbElopement);
            Controls.Add(ckbPhysicalAggression);
            Controls.Add(ckbVerbalAggression);
            Controls.Add(cmbStep);
            Controls.Add(cmbLevel);
            Controls.Add(dateTimePicker1);
            Controls.Add(cmbStudent);
            Controls.Add(lblDailyBehaviorEntry);
            Controls.Add(pbxLogo);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Text = "BridgeVue";
            Load += MainForm_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbxLogo).EndInit();
            ((System.ComponentModel.ISupportInitialize)studentProfileBindingSource).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem adminToolStripMenuItem;
        private ToolStripMenuItem setupToolStripMenuItem;
        private ToolStripMenuItem predictionsToolStripMenuItem;
        private PictureBox pbxLogo;
        private ToolStripMenuItem dailyBehaviorEntryToolStripMenuItem;
        private Label lblDailyBehaviorEntry;
        private ComboBox cmbStudent;
        private DateTimePicker dateTimePicker1;
        private ComboBox cmbLevel;
        private ComboBox cmbStep;
        private CheckBox ckbVerbalAggression;
        private CheckBox ckbPhysicalAggression;
        private CheckBox ckbElopement;
        private CheckBox ckbOutOfSpot;
        private CheckBox ckbWorkRefusal;
        private CheckBox ckbProvokingPeers;
        private CheckBox ckbInappropriateLanguage;
        private CheckBox ckbOutOfLane;
        private RichTextBox rtbStaffComments;
        private Label lblDailyComments;
        private BindingSource studentProfileBindingSource;
        private Label lblEmotionCheckin;
        private TableLayoutPanel tblEmotionIcons;
        private Label lblSelectedEmotion;
        private Label lZoneOfRegulation;
        private TableLayoutPanel tblZoneColors;
        private Label lblSelectedZone;
        private Button btnSaveDailyEntry;
    }
}