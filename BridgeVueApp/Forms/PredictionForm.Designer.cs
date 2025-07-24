namespace BridgeVueApp
{
    partial class PredictionForm
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
            btnRandomStudentPredict = new Button();
            tabControl1 = new TabControl();
            tabRandom = new TabPage();
            lblStaticPrediction = new Label();
            btnPredictStatic = new Button();
            rtbRandomPredictionOutput = new RichTextBox();
            tabManual = new TabPage();
            rtbManualPredictionOuput = new RichTextBox();
            btnManualPredict = new Button();
            gbxBehavior = new GroupBox();
            nudRedZonePct = new NumericUpDown();
            lblRedZone = new Label();
            nudAvgEngagement = new NumericUpDown();
            lblAvgEngagement = new Label();
            nudAvgPhysical = new NumericUpDown();
            lblAvgPhysical = new Label();
            nudAvgVerbal = new NumericUpDown();
            lblAvgVerbal = new Label();
            gbxIntake = new GroupBox();
            cmbSocialSkills = new ComboBox();
            cmbAcademicLevel = new ComboBox();
            nudExpulsions = new NumericUpDown();
            lblSocialSkills = new Label();
            lblAcademicLevel = new Label();
            lblExpulsions = new Label();
            nudSuspensions = new NumericUpDown();
            lblSuspensions = new Label();
            nudOfficeReferrals = new NumericUpDown();
            lblOfficeReferrals = new Label();
            nudPriorIncidents = new NumericUpDown();
            lblPriorIncidents = new Label();
            cmbEntryReason = new ComboBox();
            lblEntryReason = new Label();
            gbxDemographics = new GroupBox();
            chkIEP = new CheckBox();
            chkSpecialEd = new CheckBox();
            cmbEthnicity = new ComboBox();
            lblEthnicity = new Label();
            cmbGender = new ComboBox();
            lblGender = new Label();
            nudAge = new NumericUpDown();
            lblAge = new Label();
            lblGrade = new Label();
            cmbGrade = new ComboBox();
            tabBatch = new TabPage();
            dgvBatchPrediction = new DataGridView();
            gbxBatchSummary = new GroupBox();
            lblBatchSummary = new Label();
            btnBatchPredict = new Button();
            tabControl1.SuspendLayout();
            tabRandom.SuspendLayout();
            tabManual.SuspendLayout();
            gbxBehavior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudRedZonePct).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudAvgEngagement).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudAvgPhysical).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudAvgVerbal).BeginInit();
            gbxIntake.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudExpulsions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudSuspensions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudOfficeReferrals).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudPriorIncidents).BeginInit();
            gbxDemographics.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudAge).BeginInit();
            tabBatch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvBatchPrediction).BeginInit();
            gbxBatchSummary.SuspendLayout();
            SuspendLayout();
            // 
            // btnRandomStudentPredict
            // 
            btnRandomStudentPredict.Location = new Point(216, 11);
            btnRandomStudentPredict.Name = "btnRandomStudentPredict";
            btnRandomStudentPredict.Size = new Size(246, 23);
            btnRandomStudentPredict.TabIndex = 5;
            btnRandomStudentPredict.Text = "Predict Outcome for a Random Student";
            btnRandomStudentPredict.UseVisualStyleBackColor = true;
            btnRandomStudentPredict.Click += btnRandomStudentPredict_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabRandom);
            tabControl1.Controls.Add(tabManual);
            tabControl1.Controls.Add(tabBatch);
            tabControl1.Location = new Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(711, 407);
            tabControl1.TabIndex = 7;
            // 
            // tabRandom
            // 
            tabRandom.Controls.Add(lblStaticPrediction);
            tabRandom.Controls.Add(btnPredictStatic);
            tabRandom.Controls.Add(rtbRandomPredictionOutput);
            tabRandom.Controls.Add(btnRandomStudentPredict);
            tabRandom.Location = new Point(4, 24);
            tabRandom.Name = "tabRandom";
            tabRandom.Padding = new Padding(3);
            tabRandom.Size = new Size(703, 379);
            tabRandom.TabIndex = 0;
            tabRandom.Text = "Random Prediction";
            tabRandom.UseVisualStyleBackColor = true;
            // 
            // lblStaticPrediction
            // 
            lblStaticPrediction.AutoSize = true;
            lblStaticPrediction.Location = new Point(366, 347);
            lblStaticPrediction.Name = "lblStaticPrediction";
            lblStaticPrediction.Size = new Size(119, 15);
            lblStaticPrediction.TabIndex = 8;
            lblStaticPrediction.Text = "Test Prediction Result";
            // 
            // btnPredictStatic
            // 
            btnPredictStatic.Location = new Point(157, 344);
            btnPredictStatic.Name = "btnPredictStatic";
            btnPredictStatic.Size = new Size(193, 23);
            btnPredictStatic.TabIndex = 7;
            btnPredictStatic.Text = "Test Prediction with Static Data";
            btnPredictStatic.UseVisualStyleBackColor = true;
            // 
            // rtbRandomPredictionOutput
            // 
            rtbRandomPredictionOutput.Font = new Font("Consolas", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            rtbRandomPredictionOutput.Location = new Point(97, 41);
            rtbRandomPredictionOutput.Name = "rtbRandomPredictionOutput";
            rtbRandomPredictionOutput.ReadOnly = true;
            rtbRandomPredictionOutput.ScrollBars = RichTextBoxScrollBars.Vertical;
            rtbRandomPredictionOutput.Size = new Size(473, 292);
            rtbRandomPredictionOutput.TabIndex = 6;
            rtbRandomPredictionOutput.Text = "";
            // 
            // tabManual
            // 
            tabManual.Controls.Add(rtbManualPredictionOuput);
            tabManual.Controls.Add(btnManualPredict);
            tabManual.Controls.Add(gbxBehavior);
            tabManual.Controls.Add(gbxIntake);
            tabManual.Controls.Add(gbxDemographics);
            tabManual.Location = new Point(4, 24);
            tabManual.Name = "tabManual";
            tabManual.Padding = new Padding(3);
            tabManual.RightToLeft = RightToLeft.No;
            tabManual.Size = new Size(703, 379);
            tabManual.TabIndex = 1;
            tabManual.Text = "Manual What-if";
            tabManual.UseVisualStyleBackColor = true;
            // 
            // rtbManualPredictionOuput
            // 
            rtbManualPredictionOuput.Location = new Point(27, 306);
            rtbManualPredictionOuput.Name = "rtbManualPredictionOuput";
            rtbManualPredictionOuput.Size = new Size(654, 69);
            rtbManualPredictionOuput.TabIndex = 4;
            rtbManualPredictionOuput.Text = "What-if Prediction Output";
            // 
            // btnManualPredict
            // 
            btnManualPredict.Location = new Point(75, 253);
            btnManualPredict.Name = "btnManualPredict";
            btnManualPredict.Size = new Size(75, 23);
            btnManualPredict.TabIndex = 3;
            btnManualPredict.Text = "Predict";
            btnManualPredict.UseVisualStyleBackColor = true;
            btnManualPredict.Click += btnManualPredict_Click;
            // 
            // gbxBehavior
            // 
            gbxBehavior.BackColor = Color.SkyBlue;
            gbxBehavior.Controls.Add(nudRedZonePct);
            gbxBehavior.Controls.Add(lblRedZone);
            gbxBehavior.Controls.Add(nudAvgEngagement);
            gbxBehavior.Controls.Add(lblAvgEngagement);
            gbxBehavior.Controls.Add(nudAvgPhysical);
            gbxBehavior.Controls.Add(lblAvgPhysical);
            gbxBehavior.Controls.Add(nudAvgVerbal);
            gbxBehavior.Controls.Add(lblAvgVerbal);
            gbxBehavior.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            gbxBehavior.Location = new Point(481, 18);
            gbxBehavior.Name = "gbxBehavior";
            gbxBehavior.Size = new Size(200, 284);
            gbxBehavior.TabIndex = 2;
            gbxBehavior.TabStop = false;
            gbxBehavior.Text = "Behavior Aggregates";
            // 
            // nudRedZonePct
            // 
            nudRedZonePct.DecimalPlaces = 2;
            nudRedZonePct.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            nudRedZonePct.Location = new Point(34, 237);
            nudRedZonePct.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            nudRedZonePct.Name = "nudRedZonePct";
            nudRedZonePct.Size = new Size(120, 23);
            nudRedZonePct.TabIndex = 7;
            // 
            // lblRedZone
            // 
            lblRedZone.AutoSize = true;
            lblRedZone.Location = new Point(51, 220);
            lblRedZone.Name = "lblRedZone";
            lblRedZone.Size = new Size(70, 15);
            lblRedZone.TabIndex = 6;
            lblRedZone.Text = "Red Zone %";
            // 
            // nudAvgEngagement
            // 
            nudAvgEngagement.DecimalPlaces = 1;
            nudAvgEngagement.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudAvgEngagement.Location = new Point(34, 179);
            nudAvgEngagement.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            nudAvgEngagement.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudAvgEngagement.Name = "nudAvgEngagement";
            nudAvgEngagement.Size = new Size(120, 23);
            nudAvgEngagement.TabIndex = 5;
            nudAvgEngagement.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblAvgEngagement
            // 
            lblAvgEngagement.AutoSize = true;
            lblAvgEngagement.Location = new Point(51, 159);
            lblAvgEngagement.Name = "lblAvgEngagement";
            lblAvgEngagement.Size = new Size(101, 15);
            lblAvgEngagement.TabIndex = 4;
            lblAvgEngagement.Text = "Avg. Engagement";
            // 
            // nudAvgPhysical
            // 
            nudAvgPhysical.DecimalPlaces = 1;
            nudAvgPhysical.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudAvgPhysical.Location = new Point(34, 116);
            nudAvgPhysical.Name = "nudAvgPhysical";
            nudAvgPhysical.Size = new Size(120, 23);
            nudAvgPhysical.TabIndex = 3;
            // 
            // lblAvgPhysical
            // 
            lblAvgPhysical.AutoSize = true;
            lblAvgPhysical.Location = new Point(24, 98);
            lblAvgPhysical.Name = "lblAvgPhysical";
            lblAvgPhysical.Size = new Size(139, 15);
            lblAvgPhysical.TabIndex = 2;
            lblAvgPhysical.Text = "Avg. Physical Aggression";
            // 
            // nudAvgVerbal
            // 
            nudAvgVerbal.DecimalPlaces = 1;
            nudAvgVerbal.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudAvgVerbal.Location = new Point(34, 54);
            nudAvgVerbal.Name = "nudAvgVerbal";
            nudAvgVerbal.Size = new Size(120, 23);
            nudAvgVerbal.TabIndex = 1;
            // 
            // lblAvgVerbal
            // 
            lblAvgVerbal.AutoSize = true;
            lblAvgVerbal.Location = new Point(29, 34);
            lblAvgVerbal.Name = "lblAvgVerbal";
            lblAvgVerbal.Size = new Size(129, 15);
            lblAvgVerbal.TabIndex = 0;
            lblAvgVerbal.Text = "Avg. Verbal Aggression";
            // 
            // gbxIntake
            // 
            gbxIntake.BackColor = Color.SkyBlue;
            gbxIntake.Controls.Add(cmbSocialSkills);
            gbxIntake.Controls.Add(cmbAcademicLevel);
            gbxIntake.Controls.Add(nudExpulsions);
            gbxIntake.Controls.Add(lblSocialSkills);
            gbxIntake.Controls.Add(lblAcademicLevel);
            gbxIntake.Controls.Add(lblExpulsions);
            gbxIntake.Controls.Add(nudSuspensions);
            gbxIntake.Controls.Add(lblSuspensions);
            gbxIntake.Controls.Add(nudOfficeReferrals);
            gbxIntake.Controls.Add(lblOfficeReferrals);
            gbxIntake.Controls.Add(nudPriorIncidents);
            gbxIntake.Controls.Add(lblPriorIncidents);
            gbxIntake.Controls.Add(cmbEntryReason);
            gbxIntake.Controls.Add(lblEntryReason);
            gbxIntake.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            gbxIntake.Location = new Point(231, 18);
            gbxIntake.Name = "gbxIntake";
            gbxIntake.Size = new Size(244, 284);
            gbxIntake.TabIndex = 1;
            gbxIntake.TabStop = false;
            gbxIntake.Text = "Intake Info";
            // 
            // cmbSocialSkills
            // 
            cmbSocialSkills.FormattingEnabled = true;
            cmbSocialSkills.Items.AddRange(new object[] { "High", "Moderate", "Low" });
            cmbSocialSkills.Location = new Point(117, 252);
            cmbSocialSkills.Name = "cmbSocialSkills";
            cmbSocialSkills.Size = new Size(121, 23);
            cmbSocialSkills.TabIndex = 13;
            // 
            // cmbAcademicLevel
            // 
            cmbAcademicLevel.FormattingEnabled = true;
            cmbAcademicLevel.Items.AddRange(new object[] { "Above Grade", "At Grade", "Below Grade", "Far Below Grade" });
            cmbAcademicLevel.Location = new Point(116, 212);
            cmbAcademicLevel.Name = "cmbAcademicLevel";
            cmbAcademicLevel.Size = new Size(121, 23);
            cmbAcademicLevel.TabIndex = 12;
            // 
            // nudExpulsions
            // 
            nudExpulsions.Location = new Point(117, 179);
            nudExpulsions.Name = "nudExpulsions";
            nudExpulsions.Size = new Size(120, 23);
            nudExpulsions.TabIndex = 11;
            // 
            // lblSocialSkills
            // 
            lblSocialSkills.AutoSize = true;
            lblSocialSkills.Location = new Point(11, 255);
            lblSocialSkills.Name = "lblSocialSkills";
            lblSocialSkills.Size = new Size(99, 15);
            lblSocialSkills.TabIndex = 10;
            lblSocialSkills.Text = "Social Skills Level";
            // 
            // lblAcademicLevel
            // 
            lblAcademicLevel.AutoSize = true;
            lblAcademicLevel.Location = new Point(11, 215);
            lblAcademicLevel.Name = "lblAcademicLevel";
            lblAcademicLevel.Size = new Size(90, 15);
            lblAcademicLevel.TabIndex = 9;
            lblAcademicLevel.Text = "Academic Level";
            // 
            // lblExpulsions
            // 
            lblExpulsions.AutoSize = true;
            lblExpulsions.Location = new Point(11, 181);
            lblExpulsions.Name = "lblExpulsions";
            lblExpulsions.Size = new Size(63, 15);
            lblExpulsions.TabIndex = 8;
            lblExpulsions.Text = "Expulsions";
            // 
            // nudSuspensions
            // 
            nudSuspensions.Location = new Point(117, 140);
            nudSuspensions.Name = "nudSuspensions";
            nudSuspensions.Size = new Size(120, 23);
            nudSuspensions.TabIndex = 7;
            // 
            // lblSuspensions
            // 
            lblSuspensions.AutoSize = true;
            lblSuspensions.Location = new Point(11, 142);
            lblSuspensions.Name = "lblSuspensions";
            lblSuspensions.Size = new Size(73, 15);
            lblSuspensions.TabIndex = 6;
            lblSuspensions.Text = "Suspensions";
            // 
            // nudOfficeReferrals
            // 
            nudOfficeReferrals.Location = new Point(117, 106);
            nudOfficeReferrals.Name = "nudOfficeReferrals";
            nudOfficeReferrals.Size = new Size(120, 23);
            nudOfficeReferrals.TabIndex = 5;
            // 
            // lblOfficeReferrals
            // 
            lblOfficeReferrals.AutoSize = true;
            lblOfficeReferrals.Location = new Point(11, 108);
            lblOfficeReferrals.Name = "lblOfficeReferrals";
            lblOfficeReferrals.Size = new Size(87, 15);
            lblOfficeReferrals.TabIndex = 4;
            lblOfficeReferrals.Text = "Office Referrals";
            // 
            // nudPriorIncidents
            // 
            nudPriorIncidents.Location = new Point(117, 71);
            nudPriorIncidents.Name = "nudPriorIncidents";
            nudPriorIncidents.Size = new Size(120, 23);
            nudPriorIncidents.TabIndex = 3;
            // 
            // lblPriorIncidents
            // 
            lblPriorIncidents.AutoSize = true;
            lblPriorIncidents.Location = new Point(11, 73);
            lblPriorIncidents.Name = "lblPriorIncidents";
            lblPriorIncidents.Size = new Size(84, 15);
            lblPriorIncidents.TabIndex = 2;
            lblPriorIncidents.Text = "Prior Incidents";
            // 
            // cmbEntryReason
            // 
            cmbEntryReason.FormattingEnabled = true;
            cmbEntryReason.Items.AddRange(new object[] { "Aggression", "Anxiety", "Disruptive", "Trauma", "Withdrawn", "Other" });
            cmbEntryReason.Location = new Point(117, 31);
            cmbEntryReason.Name = "cmbEntryReason";
            cmbEntryReason.Size = new Size(121, 23);
            cmbEntryReason.TabIndex = 1;
            // 
            // lblEntryReason
            // 
            lblEntryReason.AutoSize = true;
            lblEntryReason.Location = new Point(11, 34);
            lblEntryReason.Name = "lblEntryReason";
            lblEntryReason.Size = new Size(75, 15);
            lblEntryReason.TabIndex = 0;
            lblEntryReason.Text = "Entry Reason";
            // 
            // gbxDemographics
            // 
            gbxDemographics.BackColor = Color.SkyBlue;
            gbxDemographics.Controls.Add(chkIEP);
            gbxDemographics.Controls.Add(chkSpecialEd);
            gbxDemographics.Controls.Add(cmbEthnicity);
            gbxDemographics.Controls.Add(lblEthnicity);
            gbxDemographics.Controls.Add(cmbGender);
            gbxDemographics.Controls.Add(lblGender);
            gbxDemographics.Controls.Add(nudAge);
            gbxDemographics.Controls.Add(lblAge);
            gbxDemographics.Controls.Add(lblGrade);
            gbxDemographics.Controls.Add(cmbGrade);
            gbxDemographics.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            gbxDemographics.Location = new Point(25, 18);
            gbxDemographics.Name = "gbxDemographics";
            gbxDemographics.Size = new Size(200, 186);
            gbxDemographics.TabIndex = 0;
            gbxDemographics.TabStop = false;
            gbxDemographics.Text = "Demographics";
            // 
            // chkIEP
            // 
            chkIEP.AutoSize = true;
            chkIEP.Location = new Point(116, 153);
            chkIEP.Name = "chkIEP";
            chkIEP.RightToLeft = RightToLeft.Yes;
            chkIEP.Size = new Size(43, 19);
            chkIEP.TabIndex = 11;
            chkIEP.Text = "IEP";
            chkIEP.TextAlign = ContentAlignment.MiddleRight;
            chkIEP.UseVisualStyleBackColor = true;
            // 
            // chkSpecialEd
            // 
            chkSpecialEd.AutoSize = true;
            chkSpecialEd.Location = new Point(9, 153);
            chkSpecialEd.Name = "chkSpecialEd";
            chkSpecialEd.RightToLeft = RightToLeft.Yes;
            chkSpecialEd.Size = new Size(80, 19);
            chkSpecialEd.TabIndex = 10;
            chkSpecialEd.Text = "Special Ed";
            chkSpecialEd.TextAlign = ContentAlignment.MiddleRight;
            chkSpecialEd.UseVisualStyleBackColor = true;
            // 
            // cmbEthnicity
            // 
            cmbEthnicity.FormattingEnabled = true;
            cmbEthnicity.Items.AddRange(new object[] { "Asian", "White", "Black", "Hispanic", "Native American", "Pacific Islander", "Other" });
            cmbEthnicity.Location = new Point(65, 120);
            cmbEthnicity.Name = "cmbEthnicity";
            cmbEthnicity.Size = new Size(121, 23);
            cmbEthnicity.TabIndex = 8;
            // 
            // lblEthnicity
            // 
            lblEthnicity.AutoSize = true;
            lblEthnicity.Location = new Point(9, 123);
            lblEthnicity.Name = "lblEthnicity";
            lblEthnicity.Size = new Size(53, 15);
            lblEthnicity.TabIndex = 7;
            lblEthnicity.Text = "Ethnicity";
            // 
            // cmbGender
            // 
            cmbGender.FormattingEnabled = true;
            cmbGender.Items.AddRange(new object[] { "Male", "Female", "Other" });
            cmbGender.Location = new Point(66, 90);
            cmbGender.Name = "cmbGender";
            cmbGender.Size = new Size(121, 23);
            cmbGender.TabIndex = 6;
            // 
            // lblGender
            // 
            lblGender.AutoSize = true;
            lblGender.Location = new Point(9, 93);
            lblGender.Name = "lblGender";
            lblGender.Size = new Size(45, 15);
            lblGender.TabIndex = 5;
            lblGender.Text = "Gender";
            // 
            // nudAge
            // 
            nudAge.Location = new Point(66, 60);
            nudAge.Maximum = new decimal(new int[] { 12, 0, 0, 0 });
            nudAge.Minimum = new decimal(new int[] { 4, 0, 0, 0 });
            nudAge.Name = "nudAge";
            nudAge.Size = new Size(120, 23);
            nudAge.TabIndex = 4;
            nudAge.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // lblAge
            // 
            lblAge.AutoSize = true;
            lblAge.Location = new Point(9, 62);
            lblAge.Name = "lblAge";
            lblAge.Size = new Size(28, 15);
            lblAge.TabIndex = 3;
            lblAge.Text = "Age";
            // 
            // lblGrade
            // 
            lblGrade.AutoSize = true;
            lblGrade.Location = new Point(9, 34);
            lblGrade.Name = "lblGrade";
            lblGrade.Size = new Size(38, 15);
            lblGrade.TabIndex = 2;
            lblGrade.Text = "Grade";
            // 
            // cmbGrade
            // 
            cmbGrade.FormattingEnabled = true;
            cmbGrade.Items.AddRange(new object[] { "Kindergarten", "First", "Second", "Third", "Fourth", "Fifth" });
            cmbGrade.Location = new Point(65, 31);
            cmbGrade.Name = "cmbGrade";
            cmbGrade.Size = new Size(121, 23);
            cmbGrade.TabIndex = 1;
            // 
            // tabBatch
            // 
            tabBatch.Controls.Add(dgvBatchPrediction);
            tabBatch.Controls.Add(gbxBatchSummary);
            tabBatch.Controls.Add(btnBatchPredict);
            tabBatch.Location = new Point(4, 24);
            tabBatch.Name = "tabBatch";
            tabBatch.Padding = new Padding(3);
            tabBatch.Size = new Size(703, 379);
            tabBatch.TabIndex = 2;
            tabBatch.Text = "Batch Prediction";
            tabBatch.UseVisualStyleBackColor = true;
            tabBatch.Click += tabBatch_Click;
            // 
            // dgvBatchPrediction
            // 
            dgvBatchPrediction.AllowUserToAddRows = false;
            dgvBatchPrediction.AllowUserToDeleteRows = false;
            dgvBatchPrediction.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvBatchPrediction.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvBatchPrediction.Location = new Point(27, 146);
            dgvBatchPrediction.Name = "dgvBatchPrediction";
            dgvBatchPrediction.ReadOnly = true;
            dgvBatchPrediction.RowHeadersVisible = false;
            dgvBatchPrediction.Size = new Size(646, 227);
            dgvBatchPrediction.TabIndex = 2;
            // 
            // gbxBatchSummary
            // 
            gbxBatchSummary.Controls.Add(lblBatchSummary);
            gbxBatchSummary.Location = new Point(27, 6);
            gbxBatchSummary.Name = "gbxBatchSummary";
            gbxBatchSummary.Size = new Size(646, 108);
            gbxBatchSummary.TabIndex = 1;
            gbxBatchSummary.TabStop = false;
            gbxBatchSummary.Text = "Batch Prediction Overview";
            // 
            // lblBatchSummary
            // 
            lblBatchSummary.AutoSize = true;
            lblBatchSummary.Location = new Point(11, 22);
            lblBatchSummary.Name = "lblBatchSummary";
            lblBatchSummary.Size = new Size(38, 15);
            lblBatchSummary.TabIndex = 0;
            lblBatchSummary.Text = "label1";
            // 
            // btnBatchPredict
            // 
            btnBatchPredict.Location = new Point(218, 120);
            btnBatchPredict.Name = "btnBatchPredict";
            btnBatchPredict.Size = new Size(256, 23);
            btnBatchPredict.TabIndex = 0;
            btnBatchPredict.Text = "Run Batch Prediction on Current Students";
            btnBatchPredict.UseVisualStyleBackColor = true;
            btnBatchPredict.Click += btnBatchPredict_Click;
            // 
            // PredictionForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(737, 430);
            Controls.Add(tabControl1);
            Name = "PredictionForm";
            Text = "Student Predictions";
            Load += PredictionForm_Load;
            tabControl1.ResumeLayout(false);
            tabRandom.ResumeLayout(false);
            tabRandom.PerformLayout();
            tabManual.ResumeLayout(false);
            gbxBehavior.ResumeLayout(false);
            gbxBehavior.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudRedZonePct).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudAvgEngagement).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudAvgPhysical).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudAvgVerbal).EndInit();
            gbxIntake.ResumeLayout(false);
            gbxIntake.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudExpulsions).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudSuspensions).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudOfficeReferrals).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudPriorIncidents).EndInit();
            gbxDemographics.ResumeLayout(false);
            gbxDemographics.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudAge).EndInit();
            tabBatch.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvBatchPrediction).EndInit();
            gbxBatchSummary.ResumeLayout(false);
            gbxBatchSummary.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Button btnRandomStudentPredict;
        private TabControl tabControl1;
        private TabPage tabRandom;
        private TabPage tabManual;
        private TabPage tabBatch;
        private GroupBox gbxDemographics;
        private Button btnManualPredict;
        private GroupBox gbxBehavior;
        private GroupBox gbxIntake;
        private Button btnBatchPredict;
        private RichTextBox rtbRandomPredictionOutput;
        private Button btnPredictStatic;
        private Label lblStaticPrediction;
        private GroupBox gbxBatchSummary;
        private Label lblBatchSummary;
        private DataGridView dgvBatchPrediction;
        private ComboBox cmbGrade;
        private Label lblGrade;
        private ComboBox cmbGender;
        private Label lblGender;
        private NumericUpDown nudAge;
        private Label lblAge;
        private CheckBox chkSpecialEd;
        private ComboBox cmbEthnicity;
        private Label lblEthnicity;
        private CheckBox chkIEP;
        private ComboBox cmbEntryReason;
        private Label lblEntryReason;
        private Label lblSuspensions;
        private NumericUpDown nudOfficeReferrals;
        private Label lblOfficeReferrals;
        private NumericUpDown nudPriorIncidents;
        private Label lblPriorIncidents;
        private ComboBox cmbAcademicLevel;
        private NumericUpDown nudExpulsions;
        private Label lblSocialSkills;
        private Label lblAcademicLevel;
        private Label lblExpulsions;
        private NumericUpDown nudSuspensions;
        private ComboBox cmbSocialSkills;
        private Label lblAvgVerbal;
        private NumericUpDown nudAvgPhysical;
        private Label lblAvgPhysical;
        private NumericUpDown nudAvgVerbal;
        private NumericUpDown nudRedZonePct;
        private Label lblRedZone;
        private NumericUpDown nudAvgEngagement;
        private Label lblAvgEngagement;
        private RichTextBox rtbManualPredictionOuput;
    }
}