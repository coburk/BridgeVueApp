
namespace BridgeVueApp
{
    partial class SetupForm
    {
        
        /// Required designer variable.        
        private System.ComponentModel.IContainer components = null;

        
        /// Clean up any resources being used.        
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


        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupForm));
            lblSetup = new Label();
            btnCreateDatabaseAndTables = new Button();
            btnLoadStudentProfile = new Button();
            lblStatus = new Label();
            btnViewDatabaseInfo = new Button();
            btnLoadDailyBehavior = new Button();
            btnLoadIntakeData = new Button();
            btnGenerateSyntheticData = new Button();
            gbxDatbase = new GroupBox();
            btnExitOutcomeAvgs = new Button();
            btnExitOutcomeCount = new Button();
            gbxLoadData = new GroupBox();
            gbxGenerate = new GroupBox();
            btnSaveGeneratedCSV = new Button();
            btnLoadGeneratedData = new Button();
            progressBar = new ProgressBar();
            btnBackFillTraining = new Button();
            gbxDatbase.SuspendLayout();
            gbxLoadData.SuspendLayout();
            gbxGenerate.SuspendLayout();
            SuspendLayout();
            // 
            // lblSetup
            // 
            lblSetup.AutoSize = true;
            lblSetup.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSetup.Location = new Point(46, 13);
            lblSetup.Name = "lblSetup";
            lblSetup.Size = new Size(207, 25);
            lblSetup.TabIndex = 0;
            lblSetup.Text = "BridgeVue Setup Utility";
            // 
            // btnCreateDatabaseAndTables
            // 
            btnCreateDatabaseAndTables.BackColor = SystemColors.ControlLight;
            btnCreateDatabaseAndTables.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCreateDatabaseAndTables.ForeColor = Color.Black;
            btnCreateDatabaseAndTables.Location = new Point(15, 109);
            btnCreateDatabaseAndTables.Name = "btnCreateDatabaseAndTables";
            btnCreateDatabaseAndTables.Size = new Size(192, 49);
            btnCreateDatabaseAndTables.TabIndex = 1;
            btnCreateDatabaseAndTables.Text = "DELETE and Re-Create BridgeVue Database";
            btnCreateDatabaseAndTables.UseVisualStyleBackColor = false;
            btnCreateDatabaseAndTables.Click += btnCreateDatabaseAndTables_Click;
            // 
            // btnLoadStudentProfile
            // 
            btnLoadStudentProfile.Location = new Point(15, 31);
            btnLoadStudentProfile.Name = "btnLoadStudentProfile";
            btnLoadStudentProfile.Size = new Size(192, 23);
            btnLoadStudentProfile.TabIndex = 2;
            btnLoadStudentProfile.Text = "Load Student Profile Data (CSV)";
            btnLoadStudentProfile.UseVisualStyleBackColor = true;
            btnLoadStudentProfile.Click += btnLoadStudentProfile_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(34, 241);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(112, 15);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "Loaded successfully";
            // 
            // btnViewDatabaseInfo
            // 
            btnViewDatabaseInfo.Location = new Point(15, 22);
            btnViewDatabaseInfo.Name = "btnViewDatabaseInfo";
            btnViewDatabaseInfo.Size = new Size(192, 23);
            btnViewDatabaseInfo.TabIndex = 5;
            btnViewDatabaseInfo.Text = "View Database Info";
            btnViewDatabaseInfo.UseVisualStyleBackColor = true;
            btnViewDatabaseInfo.Click += btnViewDatabaseInfo_Click;
            // 
            // btnLoadDailyBehavior
            // 
            btnLoadDailyBehavior.Location = new Point(15, 90);
            btnLoadDailyBehavior.Name = "btnLoadDailyBehavior";
            btnLoadDailyBehavior.Size = new Size(192, 23);
            btnLoadDailyBehavior.TabIndex = 6;
            btnLoadDailyBehavior.Text = "Load Daily Behavior Data (CSV)";
            btnLoadDailyBehavior.UseVisualStyleBackColor = true;
            btnLoadDailyBehavior.Click += btnLoadDailyBehavior_Click;
            // 
            // btnLoadIntakeData
            // 
            btnLoadIntakeData.Location = new Point(15, 60);
            btnLoadIntakeData.Name = "btnLoadIntakeData";
            btnLoadIntakeData.Size = new Size(192, 23);
            btnLoadIntakeData.TabIndex = 7;
            btnLoadIntakeData.Text = "Load Intake Data (CSV)";
            btnLoadIntakeData.UseVisualStyleBackColor = true;
            btnLoadIntakeData.Click += btnLoadIntakeData_Click;
            // 
            // btnGenerateSyntheticData
            // 
            btnGenerateSyntheticData.Location = new Point(15, 31);
            btnGenerateSyntheticData.Name = "btnGenerateSyntheticData";
            btnGenerateSyntheticData.Size = new Size(192, 23);
            btnGenerateSyntheticData.TabIndex = 8;
            btnGenerateSyntheticData.Text = "Generate Synthethic Data";
            btnGenerateSyntheticData.UseVisualStyleBackColor = true;
            btnGenerateSyntheticData.Click += btnGenerateSyntheticData_Click;
            // 
            // gbxDatbase
            // 
            gbxDatbase.BackColor = SystemColors.Desktop;
            gbxDatbase.Controls.Add(btnExitOutcomeAvgs);
            gbxDatbase.Controls.Add(btnExitOutcomeCount);
            gbxDatbase.Controls.Add(btnCreateDatabaseAndTables);
            gbxDatbase.Controls.Add(btnViewDatabaseInfo);
            gbxDatbase.Location = new Point(34, 56);
            gbxDatbase.Name = "gbxDatbase";
            gbxDatbase.Size = new Size(228, 164);
            gbxDatbase.TabIndex = 9;
            gbxDatbase.TabStop = false;
            gbxDatbase.Text = "Database";
            // 
            // btnExitOutcomeAvgs
            // 
            btnExitOutcomeAvgs.Location = new Point(15, 80);
            btnExitOutcomeAvgs.Name = "btnExitOutcomeAvgs";
            btnExitOutcomeAvgs.Size = new Size(192, 23);
            btnExitOutcomeAvgs.TabIndex = 7;
            btnExitOutcomeAvgs.Text = "Behavior Avg. by Exit Outcome";
            btnExitOutcomeAvgs.UseVisualStyleBackColor = true;
            btnExitOutcomeAvgs.Click += btnExitOutcomeAvgs_Click;
            // 
            // btnExitOutcomeCount
            // 
            btnExitOutcomeCount.Location = new Point(15, 51);
            btnExitOutcomeCount.Name = "btnExitOutcomeCount";
            btnExitOutcomeCount.Size = new Size(192, 23);
            btnExitOutcomeCount.TabIndex = 6;
            btnExitOutcomeCount.Text = "Count of Each Exit Outcome";
            btnExitOutcomeCount.UseVisualStyleBackColor = true;
            btnExitOutcomeCount.Click += btnExitOutcomeCount_Click;
            // 
            // gbxLoadData
            // 
            gbxLoadData.BackColor = SystemColors.Desktop;
            gbxLoadData.Controls.Add(btnBackFillTraining);
            gbxLoadData.Controls.Add(btnLoadStudentProfile);
            gbxLoadData.Controls.Add(btnLoadIntakeData);
            gbxLoadData.Controls.Add(btnLoadDailyBehavior);
            gbxLoadData.Location = new Point(527, 56);
            gbxLoadData.Name = "gbxLoadData";
            gbxLoadData.Size = new Size(228, 164);
            gbxLoadData.TabIndex = 10;
            gbxLoadData.TabStop = false;
            gbxLoadData.Text = "Load Data";
            // 
            // gbxGenerate
            // 
            gbxGenerate.BackColor = SystemColors.Desktop;
            gbxGenerate.Controls.Add(btnSaveGeneratedCSV);
            gbxGenerate.Controls.Add(btnLoadGeneratedData);
            gbxGenerate.Controls.Add(btnGenerateSyntheticData);
            gbxGenerate.Location = new Point(280, 56);
            gbxGenerate.Name = "gbxGenerate";
            gbxGenerate.Size = new Size(228, 127);
            gbxGenerate.TabIndex = 11;
            gbxGenerate.TabStop = false;
            gbxGenerate.Text = "Generate Data";
            // 
            // btnSaveGeneratedCSV
            // 
            btnSaveGeneratedCSV.Location = new Point(15, 90);
            btnSaveGeneratedCSV.Name = "btnSaveGeneratedCSV";
            btnSaveGeneratedCSV.Size = new Size(192, 23);
            btnSaveGeneratedCSV.TabIndex = 10;
            btnSaveGeneratedCSV.Text = "Save Generated Data as CSV";
            btnSaveGeneratedCSV.UseVisualStyleBackColor = true;
            btnSaveGeneratedCSV.Click += btnSaveGeneratedCSV_Click;
            // 
            // btnLoadGeneratedData
            // 
            btnLoadGeneratedData.Location = new Point(15, 60);
            btnLoadGeneratedData.Name = "btnLoadGeneratedData";
            btnLoadGeneratedData.Size = new Size(192, 23);
            btnLoadGeneratedData.TabIndex = 9;
            btnLoadGeneratedData.Text = "Load Generated Data into DB";
            btnLoadGeneratedData.UseVisualStyleBackColor = true;
            btnLoadGeneratedData.Click += btnLoadGeneratedData_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(280, 197);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(228, 23);
            progressBar.TabIndex = 12;
            // 
            // btnBackFillTraining
            // 
            btnBackFillTraining.Location = new Point(15, 123);
            btnBackFillTraining.Name = "btnBackFillTraining";
            btnBackFillTraining.Size = new Size(192, 23);
            btnBackFillTraining.TabIndex = 8;
            btnBackFillTraining.Text = "Backfill Training Data";
            btnBackFillTraining.UseVisualStyleBackColor = true;
            btnBackFillTraining.Click += btnBackFillTraining_Click;
            // 
            // SetupForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(778, 567);
            Controls.Add(progressBar);
            Controls.Add(gbxGenerate);
            Controls.Add(gbxLoadData);
            Controls.Add(gbxDatbase);
            Controls.Add(lblStatus);
            Controls.Add(lblSetup);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "SetupForm";
            Text = "BridgeVue Setup";
            Load += SetupForm_Load;
            gbxDatbase.ResumeLayout(false);
            gbxLoadData.ResumeLayout(false);
            gbxGenerate.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private void SetupForm_Load(object sender, EventArgs e)
        {
            lblStatus.Text = "Ready to load data.";
        }

        #endregion

        private Label lblSetup;
        private Button btnCreateDatabaseAndTables;
        private Button btnLoadStudentProfile;
        private Label lblStatus;
        private Button btnViewDatabaseInfo;
        private Button btnLoadDailyBehavior;
        private Button btnLoadIntakeData;
        private Button btnGenerateSyntheticData;
        private GroupBox gbxDatbase;
        private GroupBox gbxLoadData;
        private GroupBox gbxGenerate;
        private Button btnSaveGeneratedCSV;
        private Button btnLoadGeneratedData;
        private Button btnExitOutcomeCount;
        private Button btnExitOutcomeAvgs;
        private ProgressBar progressBar;
        private Button btnBackFillTraining;
    }
}