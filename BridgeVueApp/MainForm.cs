﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BridgeVueApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void setupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetupForm setupForm = new SetupForm();
            setupForm.ShowDialog();
        }

        private void predictionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PredictionForm predictForm = new PredictionForm();
            predictForm.ShowDialog(); // Modal window
        }

        private void dataInsightsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void pbxLogo_Click(object sender, EventArgs e)
        {

        }
    }
}
