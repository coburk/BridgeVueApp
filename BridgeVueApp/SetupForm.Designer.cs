
namespace BridgeVueApp
{
    partial class SetupForm
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
            lblSetup = new Label();
            btnCreateDatabaseAndTables = new Button();
            btnLoadStudentProfile = new Button();
            lblStatus = new Label();
            btnViewDatabaseInfo = new Button();
            btnLoadDailyBehavior = new Button();
            btnLoadIntakeData = new Button();
            btnGenerateData = new Button();
            gbxDatbase = new GroupBox();
            gbxLoadData = new GroupBox();
            gbxGenerate = new GroupBox();
            btnSaveGeneratedCSV = new Button();
            btnLoadGeneratedData = new Button();
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
            btnCreateDatabaseAndTables.Location = new Point(15, 27);
            btnCreateDatabaseAndTables.Name = "btnCreateDatabaseAndTables";
            btnCreateDatabaseAndTables.Size = new Size(192, 23);
            btnCreateDatabaseAndTables.TabIndex = 1;
            btnCreateDatabaseAndTables.Text = "Create Database and Tables";
            btnCreateDatabaseAndTables.UseVisualStyleBackColor = true;
            btnCreateDatabaseAndTables.Click += btnCreateDatabaseAndTables_Click;
            // 
            // btnLoadStudentProfile
            // 
            btnLoadStudentProfile.Location = new Point(15, 22);
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
            lblStatus.Location = new Point(34, 456);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(112, 15);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "Loaded successfully";
            // 
            // btnViewDatabaseInfo
            // 
            btnViewDatabaseInfo.Location = new Point(15, 62);
            btnViewDatabaseInfo.Name = "btnViewDatabaseInfo";
            btnViewDatabaseInfo.Size = new Size(192, 23);
            btnViewDatabaseInfo.TabIndex = 5;
            btnViewDatabaseInfo.Text = "View Database Info";
            btnViewDatabaseInfo.UseVisualStyleBackColor = true;
            btnViewDatabaseInfo.Click += btnViewDatabaseInfo_Click;
            // 
            // btnLoadDailyBehavior
            // 
            btnLoadDailyBehavior.Location = new Point(15, 80);
            btnLoadDailyBehavior.Name = "btnLoadDailyBehavior";
            btnLoadDailyBehavior.Size = new Size(192, 23);
            btnLoadDailyBehavior.TabIndex = 6;
            btnLoadDailyBehavior.Text = "Load Daily Behavior Data (CSV)";
            btnLoadDailyBehavior.UseVisualStyleBackColor = true;
            btnLoadDailyBehavior.Click += btnLoadDailyBehavior_Click;
            // 
            // btnLoadIntakeData
            // 
            btnLoadIntakeData.Location = new Point(15, 51);
            btnLoadIntakeData.Name = "btnLoadIntakeData";
            btnLoadIntakeData.Size = new Size(192, 23);
            btnLoadIntakeData.TabIndex = 7;
            btnLoadIntakeData.Text = "Load Intake Data (CSV)";
            btnLoadIntakeData.UseVisualStyleBackColor = true;
            btnLoadIntakeData.Click += btnLoadIntakeData_Click;
            // 
            // btnGenerateData
            // 
            btnGenerateData.Location = new Point(15, 31);
            btnGenerateData.Name = "btnGenerateData";
            btnGenerateData.Size = new Size(192, 23);
            btnGenerateData.TabIndex = 8;
            btnGenerateData.Text = "Generate Synthethic Data";
            btnGenerateData.UseVisualStyleBackColor = true;
            btnGenerateData.Click += btnGenerateData_Click;
            // 
            // gbxDatbase
            // 
            gbxDatbase.BackColor = SystemColors.Desktop;
            gbxDatbase.Controls.Add(btnCreateDatabaseAndTables);
            gbxDatbase.Controls.Add(btnViewDatabaseInfo);
            gbxDatbase.Location = new Point(34, 56);
            gbxDatbase.Name = "gbxDatbase";
            gbxDatbase.Size = new Size(228, 100);
            gbxDatbase.TabIndex = 9;
            gbxDatbase.TabStop = false;
            gbxDatbase.Text = "Database";
            // 
            // gbxLoadData
            // 
            gbxLoadData.BackColor = SystemColors.Desktop;
            gbxLoadData.Controls.Add(btnLoadStudentProfile);
            gbxLoadData.Controls.Add(btnLoadIntakeData);
            gbxLoadData.Controls.Add(btnLoadDailyBehavior);
            gbxLoadData.Location = new Point(34, 177);
            gbxLoadData.Name = "gbxLoadData";
            gbxLoadData.Size = new Size(228, 118);
            gbxLoadData.TabIndex = 10;
            gbxLoadData.TabStop = false;
            gbxLoadData.Text = "Load Data";
            // 
            // gbxGenerate
            // 
            gbxGenerate.BackColor = SystemColors.Desktop;
            gbxGenerate.Controls.Add(btnSaveGeneratedCSV);
            gbxGenerate.Controls.Add(btnLoadGeneratedData);
            gbxGenerate.Controls.Add(btnGenerateData);
            gbxGenerate.Location = new Point(34, 316);
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
            // SetupForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(311, 567);
            Controls.Add(gbxGenerate);
            Controls.Add(gbxLoadData);
            Controls.Add(gbxDatbase);
            Controls.Add(lblStatus);
            Controls.Add(lblSetup);
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
        private Button btnGenerateData;
        private GroupBox gbxDatbase;
        private GroupBox gbxLoadData;
        private GroupBox gbxGenerate;
        private Button btnSaveGeneratedCSV;
        private Button btnLoadGeneratedData;
    }
}