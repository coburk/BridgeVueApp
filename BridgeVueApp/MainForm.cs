using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Data.SqlClient;

namespace BridgeVueApp
{
    public partial class MainForm : Form
    {
        // --- Form-level fields ---
        private List<EmotionOption> emotionOptions;
        private PictureBox selectedPictureBox = null;
        private string selectedEmotionName = null;
        private int selectedEmotionValue = 0;


        public MainForm()
        {
            InitializeComponent();
            LoadStudents();
            LoadEmotionOptions();
            PopulateZoneGrid();
            PopulateEmotionGrid();
        }

        // --- Emotion class ---
        public class EmotionOption
        {
            public string Name { get; set; }
            public int Value { get; set; }
            public Image Icon { get; set; }
        }

        // --- Load emotions ---
        private void LoadEmotionOptions()
        {
            emotionOptions = new List<EmotionOption>
            {
                new EmotionOption { Name = "Happy", Value = 5, Icon = Properties.Resources.Happy },
                new EmotionOption { Name = "Sad", Value = 1, Icon = Properties.Resources.Sad },
                new EmotionOption { Name = "Angry", Value = 2, Icon = Properties.Resources.Angry },
                new EmotionOption { Name = "Calm", Value = 4, Icon = Properties.Resources.Calm },
                new EmotionOption { Name = "Anxious", Value = 3, Icon = Properties.Resources.Anxious }
            };
        }

        // --- Build grid ---
        private void PopulateEmotionGrid()
        {
            // Make sure this matches the name of your TableLayoutPanel in the designer
            tblEmotionIcons.Controls.Clear();

            int columns = 5;
            int rows = (int)Math.Ceiling(emotionOptions.Count / (double)columns);

            tblEmotionIcons.ColumnCount = columns;
            tblEmotionIcons.RowCount = rows;

            int index = 0;
            foreach (var emotion in emotionOptions)
            {
                PictureBox pic = new PictureBox
                {
                    Image = emotion.Icon,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Dock = DockStyle.Fill,
                    Tag = emotion,
                    BorderStyle = BorderStyle.FixedSingle,
                    Margin = new Padding(5),
                    Cursor = Cursors.Hand
                };

                pic.Click += EmotionPictogram_Click;

                int row = index / columns;
                int col = index % columns;

                tblEmotionIcons.Controls.Add(pic, col, row);
                index++;
            }
        }

        // --- Handle clicks ---
        private void EmotionPictogram_Click(object sender, EventArgs e)
        {
            if (selectedPictureBox != null)
                selectedPictureBox.BackColor = tblEmotionIcons.BackColor;

            selectedPictureBox = sender as PictureBox;
            selectedPictureBox.BackColor = Color.LightBlue;

            var selectedEmotion = (EmotionOption)selectedPictureBox.Tag;
            selectedEmotionName = selectedEmotion.Name;
            selectedEmotionValue = selectedEmotion.Value;

            // Make sure this label exists in your designer
            lblSelectedEmotion.Text = $"Selected: {selectedEmotionName}";
        }


        private void PopulateZoneGrid()
        {
            tblZoneColors.Controls.Clear();

            var zones = new List<(string Name, Color Color, int Value)>
    {
                ("Green", Color.Green, 3),
                ("Blue", Color.SkyBlue, 1),
                ("Yellow", Color.Gold, 2),
                ("Red", Color.Red, 0)
    };

            int index = 0;
            foreach (var (name, color, value) in zones)
            {
                var panel = new Panel
                {
                    BackColor = color,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(5),
                    Tag = (name, value),
                    Cursor = Cursors.Hand,
                    BorderStyle = BorderStyle.FixedSingle
                };

                panel.Click += ZonePanel_Click;

                var label = new Label
                {
                    Text = name,
                    Dock = DockStyle.Bottom,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.Black,
                    BackColor = Color.Transparent,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };

                panel.Controls.Add(label);

                int row = 0;
                int col = index;
                tblZoneColors.Controls.Add(panel, col, row);
                index++;
            }
        }

        private Panel selectedZonePanel = null;
        private string selectedZoneName = null;
        private int selectedZoneValue = -1;

        private void ZonePanel_Click(object sender, EventArgs e)
        {
            if (selectedZonePanel != null)
                selectedZonePanel.BorderStyle = BorderStyle.FixedSingle;

            selectedZonePanel = sender as Panel;
            selectedZonePanel.BorderStyle = BorderStyle.Fixed3D;

            var (zoneName, zoneValue) = ((string, int))selectedZonePanel.Tag;
            selectedZoneName = zoneName;
            selectedZoneValue = zoneValue;

            // Optional feedback
            lblSelectedZone.Text = $"Selected Zone: {zoneName}";
        }




        // --- Existing form event handlers ---
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
            // Nothing here yet
        }

        private void pbxLogo_Click(object sender, EventArgs e)
        {
            // Nothing here yet
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Optional additional logic
        }

        private void btnSaveDailyEntry_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedEmotionName == null || selectedZoneName == null)
                {
                    MessageBox.Show("Please select both an emotion and a zone before saving.", "Missing Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                // Replace with actual StudentID selection logic
                int studentId = 1; // Placeholder until student selection is implemented

                DateTime timestamp = DateTime.Now;

                // SQL connection string
                using (SqlConnection conn = new SqlConnection(DatabaseConfig.FullConnection))
                {
                    conn.Open();

                    string query = @"
                INSERT INTO DailyBehavior (
                    StudentID, Timestamp, ZoneOfRegulation, ZoneOfRegulationNumeric,
                    WeeklyEmotionPictogram, WeeklyEmotionPictogramNumeric, CreatedDate
                )
                VALUES (
                    @StudentID, @Timestamp, @Zone, @ZoneNum,
                    @Emotion, @EmotionNum, @CreatedDate
                )";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@StudentID", studentId);
                        cmd.Parameters.AddWithValue("@Timestamp", timestamp);
                        cmd.Parameters.AddWithValue("@Zone", selectedZoneName);
                        cmd.Parameters.AddWithValue("@ZoneNum", selectedZoneValue);
                        cmd.Parameters.AddWithValue("@Emotion", selectedEmotionName);
                        cmd.Parameters.AddWithValue("@EmotionNum", selectedEmotionValue);
                        cmd.Parameters.AddWithValue("@CreatedDate", timestamp);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Daily behavior entry recorded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving entry:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LoadStudents()
        {
            List<Student> students = new List<Student>();

            // SQL connection string
            using (SqlConnection conn = new SqlConnection(DatabaseConfig.FullConnection))
            {
                conn.Open();
                string query = "SELECT StudentID, FirstName, LastName FROM StudentProfile ORDER BY LastName, FirstName";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        students.Add(new Student
                        {
                            StudentID = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            //Grade = reader.GetInt32(3)
                        });
                    }
                }
            }

            cmbStudent.DataSource = students;
            cmbStudent.DisplayMember = "DisplayName";
            cmbStudent.ValueMember = "StudentID";
        }

        public class Student
        {
            public int StudentID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }

            public string DisplayName => $"{FirstName} {LastName}";
           // public int Grade { get; set; }


        }

        private void studentProfileBindingSource_CurrentChanged(object sender, EventArgs e)
        {

        }
    }
}
