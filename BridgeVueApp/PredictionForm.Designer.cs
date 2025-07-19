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
            lblManualPredictionResult = new Label();
            btnManualPredict = new Button();
            gbxBehavior = new GroupBox();
            gbxIntake = new GroupBox();
            gbxDemographics = new GroupBox();
            tabBatch = new TabPage();
            dgvBatchPrediction = new DataGridView();
            gbxBatchSummary = new GroupBox();
            lblBatchSummary = new Label();
            btnBatchPredict = new Button();
            tabControl1.SuspendLayout();
            tabRandom.SuspendLayout();
            tabManual.SuspendLayout();
            tabBatch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvBatchPrediction).BeginInit();
            gbxBatchSummary.SuspendLayout();
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
            tabControl1.Location = new Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(711, 503);
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
            tabRandom.Size = new Size(703, 398);
            tabRandom.TabIndex = 0;
            tabRandom.Text = "Random Prediction";
            tabRandom.UseVisualStyleBackColor = true;
            // 
            // lblStaticPrediction
            // 
            lblStaticPrediction.AutoSize = true;
            lblStaticPrediction.Location = new Point(366, 369);
            lblStaticPrediction.Name = "lblStaticPrediction";
            lblStaticPrediction.Size = new Size(119, 15);
            lblStaticPrediction.TabIndex = 8;
            lblStaticPrediction.Text = "Test Prediction Result";
            // 
            // btnPredictStatic
            // 
            btnPredictStatic.Location = new Point(157, 365);
            btnPredictStatic.Name = "btnPredictStatic";
            btnPredictStatic.Size = new Size(193, 23);
            btnPredictStatic.TabIndex = 7;
            btnPredictStatic.Text = "Test Prediction with Static Data";
            btnPredictStatic.UseVisualStyleBackColor = true;
            // 
            // rtbRandomPredictionOutput
            // 
            rtbRandomPredictionOutput.Font = new Font("Consolas", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            rtbRandomPredictionOutput.Location = new Point(97, 61);
            rtbRandomPredictionOutput.Name = "rtbRandomPredictionOutput";
            rtbRandomPredictionOutput.ReadOnly = true;
            rtbRandomPredictionOutput.ScrollBars = RichTextBoxScrollBars.Vertical;
            rtbRandomPredictionOutput.Size = new Size(473, 292);
            rtbRandomPredictionOutput.TabIndex = 6;
            rtbRandomPredictionOutput.Text = "";
            // 
            // tabManual
            // 
            tabManual.Controls.Add(lblManualPredictionResult);
            tabManual.Controls.Add(btnManualPredict);
            tabManual.Controls.Add(gbxBehavior);
            tabManual.Controls.Add(gbxIntake);
            tabManual.Controls.Add(gbxDemographics);
            tabManual.Location = new Point(4, 24);
            tabManual.Name = "tabManual";
            tabManual.Padding = new Padding(3);
            tabManual.Size = new Size(703, 398);
            tabManual.TabIndex = 1;
            tabManual.Text = "Manual What-if";
            tabManual.UseVisualStyleBackColor = true;
            // 
            // lblManualPredictionResult
            // 
            lblManualPredictionResult.AutoSize = true;
            lblManualPredictionResult.Location = new Point(25, 334);
            lblManualPredictionResult.Name = "lblManualPredictionResult";
            lblManualPredictionResult.Size = new Size(133, 15);
            lblManualPredictionResult.TabIndex = 4;
            lblManualPredictionResult.Text = "What-if Predition Result";
            // 
            // btnManualPredict
            // 
            btnManualPredict.Location = new Point(538, 257);
            btnManualPredict.Name = "btnManualPredict";
            btnManualPredict.Size = new Size(75, 23);
            btnManualPredict.TabIndex = 3;
            btnManualPredict.Text = "Predict";
            btnManualPredict.UseVisualStyleBackColor = true;
            // 
            // gbxBehavior
            // 
            gbxBehavior.Location = new Point(481, 18);
            gbxBehavior.Name = "gbxBehavior";
            gbxBehavior.Size = new Size(200, 206);
            gbxBehavior.TabIndex = 2;
            gbxBehavior.TabStop = false;
            gbxBehavior.Text = "Behavior Aggregates";
            // 
            // gbxIntake
            // 
            gbxIntake.Location = new Point(262, 18);
            gbxIntake.Name = "gbxIntake";
            gbxIntake.Size = new Size(200, 284);
            gbxIntake.TabIndex = 1;
            gbxIntake.TabStop = false;
            gbxIntake.Text = "Intake Info";
            // 
            // gbxDemographics
            // 
            gbxDemographics.Location = new Point(25, 18);
            gbxDemographics.Name = "gbxDemographics";
            gbxDemographics.Size = new Size(216, 284);
            gbxDemographics.TabIndex = 0;
            gbxDemographics.TabStop = false;
            gbxDemographics.Text = "Demographics";
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
            lblBatchSummary.Size = new Size(38, 15);
            lblBatchSummary.TabIndex = 0;
            lblBatchSummary.Text = "label1";
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
            tabRandom.PerformLayout();
            tabManual.ResumeLayout(false);
            tabManual.PerformLayout();
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
        private Label lblManualPredictionResult;
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
    }
}