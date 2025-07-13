
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
            btnLoadAdditionalData = new Button();
            lblStatus = new Label();
            btnViewDatabaseInfo = new Button();
            SuspendLayout();
            // 
            // lblSetup
            // 
            lblSetup.AutoSize = true;
            lblSetup.Location = new Point(34, 25);
            lblSetup.Name = "lblSetup";
            lblSetup.Size = new Size(128, 15);
            lblSetup.TabIndex = 0;
            lblSetup.Text = "BridgeVue Setup Utility";
            // 
            // btnCreateDatabaseAndTables
            // 
            btnCreateDatabaseAndTables.Location = new Point(34, 65);
            btnCreateDatabaseAndTables.Name = "btnCreateDatabaseAndTables";
            btnCreateDatabaseAndTables.Size = new Size(228, 23);
            btnCreateDatabaseAndTables.TabIndex = 1;
            btnCreateDatabaseAndTables.Text = "Create Database and Tables";
            btnCreateDatabaseAndTables.UseVisualStyleBackColor = true;
            btnCreateDatabaseAndTables.Click += btnCreateDatabaseAndTables_Click;
            // 
            // btnLoadStudentProfile
            // 
            btnLoadStudentProfile.Location = new Point(34, 152);
            btnLoadStudentProfile.Name = "btnLoadStudentProfile";
            btnLoadStudentProfile.Size = new Size(228, 23);
            btnLoadStudentProfile.TabIndex = 2;
            btnLoadStudentProfile.Text = "Load Student Profile CSV";
            btnLoadStudentProfile.UseVisualStyleBackColor = true;
            // 
            // btnLoadAdditionalData
            // 
            btnLoadAdditionalData.Location = new Point(34, 185);
            btnLoadAdditionalData.Name = "btnLoadAdditionalData";
            btnLoadAdditionalData.Size = new Size(228, 23);
            btnLoadAdditionalData.TabIndex = 3;
            btnLoadAdditionalData.Text = "Load Intake and Behavior Data";
            btnLoadAdditionalData.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(34, 226);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(112, 15);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "Loaded successfully";
            // 
            // btnViewDatabaseInfo
            // 
            btnViewDatabaseInfo.Location = new Point(34, 96);
            btnViewDatabaseInfo.Name = "btnViewDatabaseInfo";
            btnViewDatabaseInfo.Size = new Size(228, 23);
            btnViewDatabaseInfo.TabIndex = 5;
            btnViewDatabaseInfo.Text = "View Database Info";
            btnViewDatabaseInfo.UseVisualStyleBackColor = true;
            btnViewDatabaseInfo.Click += btnViewDatabaseInfo_Click;
            // 
            // SetupForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(299, 256);
            Controls.Add(btnViewDatabaseInfo);
            Controls.Add(lblStatus);
            Controls.Add(btnLoadAdditionalData);
            Controls.Add(btnLoadStudentProfile);
            Controls.Add(btnCreateDatabaseAndTables);
            Controls.Add(lblSetup);
            Name = "SetupForm";
            Text = "SetupForm";
            Load += SetupForm_Load;
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
        private Button btnLoadAdditionalData;
        private Label lblStatus;
        private Button btnViewDatabaseInfo;
    }
}