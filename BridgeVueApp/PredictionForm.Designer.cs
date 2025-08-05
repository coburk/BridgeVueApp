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
            btnPredictSatic = new Button();
            rtbRandomPredictionOutput = new RichTextBox();
            tabManual = new TabPage();
            lblManualPredictionResult = new Label();
            btnWhatIfPredict = new Button();
            gbxBehavior = new GroupBox();
            nudRedZonePercent = new NumericUpDown();
            lblRedZone = new Label();
            lblAvgEngagement = new Label();
            lblAvgPhysicalAggression = new Label();
            nudAvgEngagement = new NumericUpDown();
            nudAvgPhysicalAggression = new NumericUpDown();
            lblAvgVerbalAggression = new Label();
            nudAvgVerbalAggression = new NumericUpDown();
            gbxIntake = new GroupBox();
            cmbSocialSkillsLevel = new ComboBox();
            cmbAcademicLevel = new ComboBox();
            lblExpulsions = new Label();
            nudExpulsions = new NumericUpDown();
            lblSuspensions = new Label();
            nudSuspensions = new NumericUpDown();
            lblOfficeReferrals = new Label();
            nudOfficeReferrals = new NumericUpDown();
            label1 = new Label();
            nudPriorIncidents = new NumericUpDown();
            cmbEntryReason = new ComboBox();
            gbxStudentProfile = new GroupBox();
            checkBox1 = new CheckBox();
            ckbSpecialEd = new CheckBox();
            cmbEthnicity = new ComboBox();
            lblAge = new Label();
            nudAge = new NumericUpDown();
            cmbGender = new ComboBox();
            cmbGrade = new ComboBox();
            tabBatch = new TabPage();
            dgvBatchPrediction = new DataGridView();
            gbxBatchSummary = new GroupBox();
            lblBatchSummary = new Label();
            btnBatchPredict = new Button();
            tabTrain = new TabPage();
            btnTrain = new Button();
            tabControl1.SuspendLayout();
            tabRandom.SuspendLayout();
            tabManual.SuspendLayout();
            gbxBehavior.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudRedZonePercent).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudAvgEngagement).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudAvgPhysicalAggression).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudAvgVerbalAggression).BeginInit();
            gbxIntake.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudExpulsions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudSuspensions).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudOfficeReferrals).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudPriorIncidents).BeginInit();
            gbxStudentProfile.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)nudAge).BeginInit();
            tabBatch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvBatchPrediction).BeginInit();
            gbxBatchSummary.SuspendLayout();
            tabTrain.SuspendLayout();
            SuspendLayout();
            // 
            // btnRandomStudentPredict
            // 
            btnRandomStudentPredict.Location = new Point(216, 21);
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
            tabControl1.Controls.Add(tabTrain);
            tabControl1.Location = new Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(711, 503);
            tabControl1.TabIndex = 7;
            // 
            // tabRandom
            // 
            tabRandom.Controls.Add(btnPredictSatic);
            tabRandom.Controls.Add(rtbRandomPredictionOutput);
            tabRandom.Controls.Add(btnRandomStudentPredict);
            tabRandom.Location = new Point(4, 24);
            tabRandom.Name = "tabRandom";
            tabRandom.Padding = new Padding(3);
            tabRandom.Size = new Size(703, 475);
            tabRandom.TabIndex = 0;
            tabRandom.Text = "Random Prediction";
            tabRandom.UseVisualStyleBackColor = true;
            // 
            // btnPredictSatic
            // 
            btnPredictSatic.Location = new Point(249, 410);
            btnPredictSatic.Name = "btnPredictSatic";
            btnPredictSatic.Size = new Size(175, 23);
            btnPredictSatic.TabIndex = 7;
            btnPredictSatic.Text = "Predict Static Student Data";
            btnPredictSatic.TextAlign = ContentAlignment.TopCenter;
            btnPredictSatic.UseVisualStyleBackColor = true;
            btnPredictSatic.Click += btnPredictSatic_Click_1;
            // 
            // rtbRandomPredictionOutput
            // 
            rtbRandomPredictionOutput.Font = new Font("Consolas", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            rtbRandomPredictionOutput.Location = new Point(83, 61);
            rtbRandomPredictionOutput.Name = "rtbRandomPredictionOutput";
            rtbRandomPredictionOutput.ReadOnly = true;
            rtbRandomPredictionOutput.ScrollBars = RichTextBoxScrollBars.Vertical;
            rtbRandomPredictionOutput.Size = new Size(509, 332);
            rtbRandomPredictionOutput.TabIndex = 6;
            rtbRandomPredictionOutput.Text = "";
            // 
            // tabManual
            // 
            tabManual.BackColor = Color.AliceBlue;
            tabManual.BackgroundImageLayout = ImageLayout.Zoom;
            tabManual.Controls.Add(lblManualPredictionResult);
            tabManual.Controls.Add(btnWhatIfPredict);
            tabManual.Controls.Add(gbxBehavior);
            tabManual.Controls.Add(gbxIntake);
            tabManual.Controls.Add(gbxStudentProfile);
            tabManual.Location = new Point(4, 24);
            tabManual.Name = "tabManual";
            tabManual.Padding = new Padding(3);
            tabManual.RightToLeft = RightToLeft.Yes;
            tabManual.Size = new Size(703, 475);
            tabManual.TabIndex = 1;
            tabManual.Text = "Manual What-if";
            // 
            // lblManualPredictionResult
            // 
            lblManualPredictionResult.AutoSize = true;
            lblManualPredictionResult.Location = new Point(25, 318);
            lblManualPredictionResult.Name = "lblManualPredictionResult";
            lblManualPredictionResult.Size = new Size(133, 15);
            lblManualPredictionResult.TabIndex = 4;
            lblManualPredictionResult.Text = "What-if Predition Result";
            // 
            // btnWhatIfPredict
            // 
            btnWhatIfPredict.Location = new Point(506, 265);
            btnWhatIfPredict.Name = "btnWhatIfPredict";
            btnWhatIfPredict.Size = new Size(125, 44);
            btnWhatIfPredict.TabIndex = 3;
            btnWhatIfPredict.Text = "What-If Prediction";
            btnWhatIfPredict.UseVisualStyleBackColor = true;
            // 
            // gbxBehavior
            // 
            gbxBehavior.BackColor = Color.LightBlue;
            gbxBehavior.Controls.Add(nudRedZonePercent);
            gbxBehavior.Controls.Add(lblRedZone);
            gbxBehavior.Controls.Add(lblAvgEngagement);
            gbxBehavior.Controls.Add(lblAvgPhysicalAggression);
            gbxBehavior.Controls.Add(nudAvgEngagement);
            gbxBehavior.Controls.Add(nudAvgPhysicalAggression);
            gbxBehavior.Controls.Add(lblAvgVerbalAggression);
            gbxBehavior.Controls.Add(nudAvgVerbalAggression);
            gbxBehavior.Location = new Point(469, 22);
            gbxBehavior.Name = "gbxBehavior";
            gbxBehavior.RightToLeft = RightToLeft.No;
            gbxBehavior.Size = new Size(196, 239);
            gbxBehavior.TabIndex = 2;
            gbxBehavior.TabStop = false;
            gbxBehavior.Text = "Behavior Summary";
            // 
            // nudRedZonePercent
            // 
            nudRedZonePercent.DecimalPlaces = 2;
            nudRedZonePercent.Increment = new decimal(new int[] { 1, 0, 0, 131072 });
            nudRedZonePercent.Location = new Point(65, 207);
            nudRedZonePercent.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            nudRedZonePercent.Name = "nudRedZonePercent";
            nudRedZonePercent.Size = new Size(57, 23);
            nudRedZonePercent.TabIndex = 7;
            nudRedZonePercent.TextAlign = HorizontalAlignment.Center;
            // 
            // lblRedZone
            // 
            lblRedZone.AutoSize = true;
            lblRedZone.Location = new Point(56, 186);
            lblRedZone.Name = "lblRedZone";
            lblRedZone.Size = new Size(70, 15);
            lblRedZone.TabIndex = 6;
            lblRedZone.Text = "Red Zone %";
            // 
            // lblAvgEngagement
            // 
            lblAvgEngagement.AutoSize = true;
            lblAvgEngagement.Location = new Point(44, 129);
            lblAvgEngagement.Name = "lblAvgEngagement";
            lblAvgEngagement.Size = new Size(98, 15);
            lblAvgEngagement.TabIndex = 5;
            lblAvgEngagement.Text = "Avg Engagement";
            // 
            // lblAvgPhysicalAggression
            // 
            lblAvgPhysicalAggression.AutoSize = true;
            lblAvgPhysicalAggression.Location = new Point(26, 74);
            lblAvgPhysicalAggression.Name = "lblAvgPhysicalAggression";
            lblAvgPhysicalAggression.Size = new Size(136, 15);
            lblAvgPhysicalAggression.TabIndex = 4;
            lblAvgPhysicalAggression.Text = "Avg Physical Aggression";
            // 
            // nudAvgEngagement
            // 
            nudAvgEngagement.DecimalPlaces = 1;
            nudAvgEngagement.Increment = new decimal(new int[] { 1, 0, 0, 65536 });
            nudAvgEngagement.Location = new Point(65, 149);
            nudAvgEngagement.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            nudAvgEngagement.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudAvgEngagement.Name = "nudAvgEngagement";
            nudAvgEngagement.Size = new Size(57, 23);
            nudAvgEngagement.TabIndex = 3;
            nudAvgEngagement.TextAlign = HorizontalAlignment.Center;
            nudAvgEngagement.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // nudAvgPhysicalAggression
            // 
            nudAvgPhysicalAggression.DecimalPlaces = 1;
            nudAvgPhysicalAggression.Location = new Point(65, 93);
            nudAvgPhysicalAggression.Name = "nudAvgPhysicalAggression";
            nudAvgPhysicalAggression.Size = new Size(57, 23);
            nudAvgPhysicalAggression.TabIndex = 2;
            nudAvgPhysicalAggression.TextAlign = HorizontalAlignment.Center;
            // 
            // lblAvgVerbalAggression
            // 
            lblAvgVerbalAggression.AutoSize = true;
            lblAvgVerbalAggression.Location = new Point(37, 19);
            lblAvgVerbalAggression.Name = "lblAvgVerbalAggression";
            lblAvgVerbalAggression.Size = new Size(125, 15);
            lblAvgVerbalAggression.TabIndex = 1;
            lblAvgVerbalAggression.Text = "Avg Verbal Aggression";
            // 
            // nudAvgVerbalAggression
            // 
            nudAvgVerbalAggression.DecimalPlaces = 1;
            nudAvgVerbalAggression.Location = new Point(65, 38);
            nudAvgVerbalAggression.Name = "nudAvgVerbalAggression";
            nudAvgVerbalAggression.Size = new Size(57, 23);
            nudAvgVerbalAggression.TabIndex = 0;
            nudAvgVerbalAggression.TextAlign = HorizontalAlignment.Center;
            // 
            // gbxIntake
            // 
            gbxIntake.BackColor = Color.LightBlue;
            gbxIntake.Controls.Add(cmbSocialSkillsLevel);
            gbxIntake.Controls.Add(cmbAcademicLevel);
            gbxIntake.Controls.Add(lblExpulsions);
            gbxIntake.Controls.Add(nudExpulsions);
            gbxIntake.Controls.Add(lblSuspensions);
            gbxIntake.Controls.Add(nudSuspensions);
            gbxIntake.Controls.Add(lblOfficeReferrals);
            gbxIntake.Controls.Add(nudOfficeReferrals);
            gbxIntake.Controls.Add(label1);
            gbxIntake.Controls.Add(nudPriorIncidents);
            gbxIntake.Controls.Add(cmbEntryReason);
            gbxIntake.Location = new Point(227, 18);
            gbxIntake.Name = "gbxIntake";
            gbxIntake.RightToLeft = RightToLeft.No;
            gbxIntake.Size = new Size(204, 284);
            gbxIntake.TabIndex = 1;
            gbxIntake.TabStop = false;
            gbxIntake.Text = "Entry Characteristics";
            // 
            // cmbSocialSkillsLevel
            // 
            cmbSocialSkillsLevel.FormattingEnabled = true;
            cmbSocialSkillsLevel.Items.AddRange(new object[] { "High", "Medium", "Low", "Unknown" });
            cmbSocialSkillsLevel.Location = new Point(32, 241);
            cmbSocialSkillsLevel.Name = "cmbSocialSkillsLevel";
            cmbSocialSkillsLevel.Size = new Size(138, 23);
            cmbSocialSkillsLevel.TabIndex = 10;
            cmbSocialSkillsLevel.Text = "Social Skills Level";
            // 
            // cmbAcademicLevel
            // 
            cmbAcademicLevel.FormattingEnabled = true;
            cmbAcademicLevel.Items.AddRange(new object[] { "Above Grade", "At Grade", "Below Grade", "Unknown" });
            cmbAcademicLevel.Location = new Point(32, 204);
            cmbAcademicLevel.Name = "cmbAcademicLevel";
            cmbAcademicLevel.Size = new Size(138, 23);
            cmbAcademicLevel.TabIndex = 9;
            cmbAcademicLevel.Text = "Academic Level";
            // 
            // lblExpulsions
            // 
            lblExpulsions.AutoSize = true;
            lblExpulsions.Location = new Point(32, 170);
            lblExpulsions.Name = "lblExpulsions";
            lblExpulsions.Size = new Size(63, 15);
            lblExpulsions.TabIndex = 8;
            lblExpulsions.Text = "Expulsions";
            // 
            // nudExpulsions
            // 
            nudExpulsions.Location = new Point(124, 168);
            nudExpulsions.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            nudExpulsions.Name = "nudExpulsions";
            nudExpulsions.Size = new Size(46, 23);
            nudExpulsions.TabIndex = 7;
            nudExpulsions.TextAlign = HorizontalAlignment.Center;
            // 
            // lblSuspensions
            // 
            lblSuspensions.AutoSize = true;
            lblSuspensions.Location = new Point(32, 132);
            lblSuspensions.Name = "lblSuspensions";
            lblSuspensions.Size = new Size(72, 15);
            lblSuspensions.TabIndex = 6;
            lblSuspensions.Text = "Suspensions";
            // 
            // nudSuspensions
            // 
            nudSuspensions.Location = new Point(124, 130);
            nudSuspensions.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            nudSuspensions.Name = "nudSuspensions";
            nudSuspensions.Size = new Size(46, 23);
            nudSuspensions.TabIndex = 5;
            nudSuspensions.TextAlign = HorizontalAlignment.Center;
            // 
            // lblOfficeReferrals
            // 
            lblOfficeReferrals.AutoSize = true;
            lblOfficeReferrals.Location = new Point(32, 96);
            lblOfficeReferrals.Name = "lblOfficeReferrals";
            lblOfficeReferrals.Size = new Size(87, 15);
            lblOfficeReferrals.TabIndex = 4;
            lblOfficeReferrals.Text = "Office Referrals";
            // 
            // nudOfficeReferrals
            // 
            nudOfficeReferrals.Location = new Point(124, 93);
            nudOfficeReferrals.Name = "nudOfficeReferrals";
            nudOfficeReferrals.Size = new Size(46, 23);
            nudOfficeReferrals.TabIndex = 3;
            nudOfficeReferrals.TextAlign = HorizontalAlignment.Center;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(32, 60);
            label1.Name = "label1";
            label1.Size = new Size(83, 15);
            label1.TabIndex = 2;
            label1.Text = "Prior Incidents";
            // 
            // nudPriorIncidents
            // 
            nudPriorIncidents.Location = new Point(124, 58);
            nudPriorIncidents.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            nudPriorIncidents.Name = "nudPriorIncidents";
            nudPriorIncidents.Size = new Size(46, 23);
            nudPriorIncidents.TabIndex = 1;
            nudPriorIncidents.TextAlign = HorizontalAlignment.Center;
            // 
            // cmbEntryReason
            // 
            cmbEntryReason.FormattingEnabled = true;
            cmbEntryReason.Items.AddRange(new object[] { "Aggression", "Anxiety", "Trauma", "Withdrawn", "Disruptive", "Other" });
            cmbEntryReason.Location = new Point(32, 25);
            cmbEntryReason.Name = "cmbEntryReason";
            cmbEntryReason.Size = new Size(138, 23);
            cmbEntryReason.TabIndex = 0;
            cmbEntryReason.Text = "Entry Reason";
            // 
            // gbxStudentProfile
            // 
            gbxStudentProfile.BackColor = Color.LightBlue;
            gbxStudentProfile.Controls.Add(checkBox1);
            gbxStudentProfile.Controls.Add(ckbSpecialEd);
            gbxStudentProfile.Controls.Add(cmbEthnicity);
            gbxStudentProfile.Controls.Add(lblAge);
            gbxStudentProfile.Controls.Add(nudAge);
            gbxStudentProfile.Controls.Add(cmbGender);
            gbxStudentProfile.Controls.Add(cmbGrade);
            gbxStudentProfile.Location = new Point(25, 18);
            gbxStudentProfile.Name = "gbxStudentProfile";
            gbxStudentProfile.RightToLeft = RightToLeft.No;
            gbxStudentProfile.Size = new Size(171, 243);
            gbxStudentProfile.TabIndex = 0;
            gbxStudentProfile.TabStop = false;
            gbxStudentProfile.Text = "Student Profile";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(79, 204);
            checkBox1.Name = "checkBox1";
            checkBox1.RightToLeft = RightToLeft.Yes;
            checkBox1.Size = new Size(42, 19);
            checkBox1.TabIndex = 6;
            checkBox1.Text = "IEP";
            checkBox1.TextAlign = ContentAlignment.MiddleCenter;
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // ckbSpecialEd
            // 
            ckbSpecialEd.AutoSize = true;
            ckbSpecialEd.Location = new Point(42, 169);
            ckbSpecialEd.Name = "ckbSpecialEd";
            ckbSpecialEd.RightToLeft = RightToLeft.Yes;
            ckbSpecialEd.Size = new Size(79, 19);
            ckbSpecialEd.TabIndex = 5;
            ckbSpecialEd.Text = "Special Ed";
            ckbSpecialEd.TextAlign = ContentAlignment.MiddleCenter;
            ckbSpecialEd.UseVisualStyleBackColor = true;
            // 
            // cmbEthnicity
            // 
            cmbEthnicity.FormattingEnabled = true;
            cmbEthnicity.Items.AddRange(new object[] { "White", "Black", "Hispanic", "Asian", "Other" });
            cmbEthnicity.Location = new Point(42, 129);
            cmbEthnicity.Name = "cmbEthnicity";
            cmbEthnicity.RightToLeft = RightToLeft.No;
            cmbEthnicity.Size = new Size(79, 23);
            cmbEthnicity.TabIndex = 4;
            cmbEthnicity.Text = "Ethnicity";
            // 
            // lblAge
            // 
            lblAge.AutoSize = true;
            lblAge.Location = new Point(42, 60);
            lblAge.Name = "lblAge";
            lblAge.Size = new Size(28, 15);
            lblAge.TabIndex = 3;
            lblAge.Text = "Age";
            // 
            // nudAge
            // 
            nudAge.Location = new Point(77, 58);
            nudAge.Maximum = new decimal(new int[] { 12, 0, 0, 0 });
            nudAge.Minimum = new decimal(new int[] { 4, 0, 0, 0 });
            nudAge.Name = "nudAge";
            nudAge.Size = new Size(44, 23);
            nudAge.TabIndex = 2;
            nudAge.TextAlign = HorizontalAlignment.Center;
            nudAge.Value = new decimal(new int[] { 4, 0, 0, 0 });
            // 
            // cmbGender
            // 
            cmbGender.FormattingEnabled = true;
            cmbGender.Items.AddRange(new object[] { "Male", "Female", "Unspecified" });
            cmbGender.Location = new Point(42, 93);
            cmbGender.Name = "cmbGender";
            cmbGender.RightToLeft = RightToLeft.No;
            cmbGender.Size = new Size(79, 23);
            cmbGender.TabIndex = 1;
            cmbGender.Text = "Gender";
            // 
            // cmbGrade
            // 
            cmbGrade.FormattingEnabled = true;
            cmbGrade.Items.AddRange(new object[] { "0", "1", "2", "3", "4", "5" });
            cmbGrade.Location = new Point(42, 25);
            cmbGrade.Name = "cmbGrade";
            cmbGrade.RightToLeft = RightToLeft.No;
            cmbGrade.Size = new Size(79, 23);
            cmbGrade.TabIndex = 0;
            cmbGrade.Text = "Grade";
            // 
            // tabBatch
            // 
            tabBatch.Controls.Add(dgvBatchPrediction);
            tabBatch.Controls.Add(gbxBatchSummary);
            tabBatch.Controls.Add(btnBatchPredict);
            tabBatch.Location = new Point(4, 24);
            tabBatch.Name = "tabBatch";
            tabBatch.Padding = new Padding(3);
            tabBatch.Size = new Size(703, 475);
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
            dgvBatchPrediction.Location = new Point(27, 206);
            dgvBatchPrediction.Name = "dgvBatchPrediction";
            dgvBatchPrediction.ReadOnly = true;
            dgvBatchPrediction.RowHeadersVisible = false;
            dgvBatchPrediction.RowHeadersWidth = 62;
            dgvBatchPrediction.Size = new Size(646, 256);
            dgvBatchPrediction.TabIndex = 2;
            // 
            // gbxBatchSummary
            // 
            gbxBatchSummary.Controls.Add(lblBatchSummary);
            gbxBatchSummary.Location = new Point(27, 17);
            gbxBatchSummary.Name = "gbxBatchSummary";
            gbxBatchSummary.Size = new Size(646, 154);
            gbxBatchSummary.TabIndex = 1;
            gbxBatchSummary.TabStop = false;
            gbxBatchSummary.Text = "Batch Prediction Overview";
            // 
            // lblBatchSummary
            // 
            lblBatchSummary.AutoSize = true;
            lblBatchSummary.Location = new Point(11, 22);
            lblBatchSummary.Name = "lblBatchSummary";
            lblBatchSummary.Size = new Size(91, 15);
            lblBatchSummary.TabIndex = 0;
            lblBatchSummary.Text = "Batch Summary";
            // 
            // btnBatchPredict
            // 
            btnBatchPredict.Location = new Point(218, 177);
            btnBatchPredict.Name = "btnBatchPredict";
            btnBatchPredict.Size = new Size(256, 23);
            btnBatchPredict.TabIndex = 0;
            btnBatchPredict.Text = "Run Batch Prediction on Current Students";
            btnBatchPredict.UseVisualStyleBackColor = true;
            btnBatchPredict.Click += btnBatchPredict_Click;
            // 
            // tabTrain
            // 
            tabTrain.Controls.Add(btnTrain);
            tabTrain.Location = new Point(4, 24);
            tabTrain.Name = "tabTrain";
            tabTrain.Padding = new Padding(3);
            tabTrain.Size = new Size(703, 475);
            tabTrain.TabIndex = 3;
            tabTrain.Text = "Train Model";
            tabTrain.UseVisualStyleBackColor = true;
            // 
            // btnTrain
            // 
            btnTrain.Location = new Point(40, 19);
            btnTrain.Name = "btnTrain";
            btnTrain.Size = new Size(112, 23);
            btnTrain.TabIndex = 0;
            btnTrain.Text = "Train Model";
            btnTrain.UseVisualStyleBackColor = true;
            btnTrain.Click += btnTrain_Click;
            // 
            // PredictionForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(753, 527);
            Controls.Add(tabControl1);
            Name = "PredictionForm";
            Text = "Student Predictions";
            Load += PredictionForm_Load;
            tabControl1.ResumeLayout(false);
            tabRandom.ResumeLayout(false);
            tabManual.ResumeLayout(false);
            tabManual.PerformLayout();
            gbxBehavior.ResumeLayout(false);
            gbxBehavior.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudRedZonePercent).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudAvgEngagement).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudAvgPhysicalAggression).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudAvgVerbalAggression).EndInit();
            gbxIntake.ResumeLayout(false);
            gbxIntake.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudExpulsions).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudSuspensions).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudOfficeReferrals).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudPriorIncidents).EndInit();
            gbxStudentProfile.ResumeLayout(false);
            gbxStudentProfile.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)nudAge).EndInit();
            tabBatch.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvBatchPrediction).EndInit();
            gbxBatchSummary.ResumeLayout(false);
            gbxBatchSummary.PerformLayout();
            tabTrain.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Button btnRandomStudentPredict;
        private TabControl tabControl1;
        private TabPage tabRandom;
        private TabPage tabManual;
        private TabPage tabBatch;
        private GroupBox gbxStudentProfile;
        private Label lblManualPredictionResult;
        private Button btnWhatIfPredict;
        private GroupBox gbxBehavior;
        private GroupBox gbxIntake;
        private Button btnBatchPredict;
        private RichTextBox rtbRandomPredictionOutput;
        private GroupBox gbxBatchSummary;
        private Label lblBatchSummary;
        private DataGridView dgvBatchPrediction;
        private Button btnPredictSatic;
        private ComboBox cmbGrade;
        private ComboBox cmbEthnicity;
        private Label lblAge;
        private NumericUpDown nudAge;
        private ComboBox cmbGender;
        private CheckBox ckbSpecialEd;
        private CheckBox checkBox1;
        private ComboBox cmbEntryReason;
        private NumericUpDown nudPriorIncidents;
        private Label label1;
        private NumericUpDown nudExpulsions;
        private Label lblSuspensions;
        private NumericUpDown nudSuspensions;
        private Label lblOfficeReferrals;
        private NumericUpDown nudOfficeReferrals;
        private ComboBox cmbAcademicLevel;
        private Label lblExpulsions;
        private ComboBox cmbSocialSkillsLevel;
        private NumericUpDown nudAvgPhysicalAggression;
        private Label lblAvgVerbalAggression;
        private NumericUpDown nudAvgVerbalAggression;
        private Label lblAvgPhysicalAggression;
        private NumericUpDown nudAvgEngagement;
        private Label lblAvgEngagement;
        private NumericUpDown nudRedZonePercent;
        private Label lblRedZone;
        private TabPage tabTrain;
        private Button btnTrain;
    }
}