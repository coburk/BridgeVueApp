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
            menuStrip1 = new MenuStrip();
            adminToolStripMenuItem = new ToolStripMenuItem();
            predictionsToolStripMenuItem = new ToolStripMenuItem();
            setupToolStripMenuItem = new ToolStripMenuItem();
            dataInsightsToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { adminToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // adminToolStripMenuItem
            // 
            adminToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { predictionsToolStripMenuItem, setupToolStripMenuItem, dataInsightsToolStripMenuItem });
            adminToolStripMenuItem.Name = "adminToolStripMenuItem";
            adminToolStripMenuItem.Size = new Size(55, 20);
            adminToolStripMenuItem.Text = "Admin";
            // 
            // predictionsToolStripMenuItem
            // 
            predictionsToolStripMenuItem.Name = "predictionsToolStripMenuItem";
            predictionsToolStripMenuItem.Size = new Size(180, 22);
            predictionsToolStripMenuItem.Text = "Predictions";
            predictionsToolStripMenuItem.Click += predictionsToolStripMenuItem_Click;
            // 
            // setupToolStripMenuItem
            // 
            setupToolStripMenuItem.Name = "setupToolStripMenuItem";
            setupToolStripMenuItem.Size = new Size(180, 22);
            setupToolStripMenuItem.Text = "Setup";
            setupToolStripMenuItem.Click += setupToolStripMenuItem_Click;
            // 
            // dataInsightsToolStripMenuItem
            // 
            dataInsightsToolStripMenuItem.Name = "dataInsightsToolStripMenuItem";
            dataInsightsToolStripMenuItem.Size = new Size(180, 22);
            dataInsightsToolStripMenuItem.Text = "Data Insights";
            dataInsightsToolStripMenuItem.Click += dataInsightsToolStripMenuItem_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            Text = "BridgeVue";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem adminToolStripMenuItem;
        private ToolStripMenuItem setupToolStripMenuItem;
        private ToolStripMenuItem predictionsToolStripMenuItem;
        private ToolStripMenuItem dataInsightsToolStripMenuItem;
    }
}