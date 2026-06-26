using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyberSecurityChatbot
{
    public partial class rtbLog : Form
    {
        Chatbot bot = new Chatbot();
        private string connectionString =
 "Data Source=localhost;Initial Catalog=CyberBotDB;Integrated Security=True;";
        public rtbLog()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bot.SetUserName("User");
            rtbChat.AppendText("========================================\n");
            rtbChat.AppendText("     CYBER SECURITY CHATBOT 🔐\n");
            rtbChat.AppendText("========================================\n\n");

            rtbChat.AppendText("Welcome! I am your Cybersecurity Assistant.\n");
            rtbChat.AppendText("Ask me about passwords, scams, privacy or phishing.\n\n");


            try
            {
                string path = Path.Combine(Application.StartupPath, "greeting.wav");

                SoundPlayer player = new SoundPlayer(path);
                player.Play();
            }
            catch
            {
                rtbChat.AppendText("Voice greeting could not be loaded.\n\n");
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string input = txtInput.Text.Trim();

            if (input == "")
            {
                rtbChat.AppendText("Bot: Please type something.\n\n");
                return;
            }

            rtbChat.AppendText("You: " + input + "\n");

            string response = bot.GetResponse(input);

            rtbChat.AppendText("Bot: " + response + "\n\n");

            txtInput.Clear();
        }

        private void txtInput_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();

            var log = bot.GetActivityLog();

            foreach (var item in log)
            {
                richTextBox1.AppendText(item + "\n");
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void LoadTasksToGrid(List<string[]> tasks)
        {
            dgvTasks.Rows.Clear();
            dgvTasks.Columns.Clear();

            dgvTasks.Columns.Add("Title", "Title");
            dgvTasks.Columns.Add("Description", "Description");
            

            foreach (var task in tasks)
            {
                dgvTasks.Rows.Add(task[0], task[1], task[2]);
            }
        }

        private void AddTask(string title, string description)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "INSERT INTO Tasks (Title, Description, Reminder) VALUES (@t, @d, @r)";
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@t", title);
                cmd.Parameters.AddWithValue("@d", description);
                

                cmd.ExecuteNonQuery();
            }
        }
        private void btnDeleteTask_Click(object sender, EventArgs e)
        {
            if (dgvTasks.SelectedRows.Count == 0) return;

            string title = dgvTasks.SelectedRows[0].Cells[0].Value.ToString();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "DELETE FROM Tasks WHERE Title=@t";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@t", title);
                cmd.ExecuteNonQuery();
            }

            btnLoadTasks_Click(null, null);
        }
        private void btnAddTask_Click(object sender, EventArgs e)
        {
            AddTask("Test Task", "Test Description");

            MessageBox.Show("Task added successfully!");

            btnLoadTasks_Click(null, null);
        }
        private void btnLoadTasks_Click(object sender, EventArgs e)
        {
            dgvTasks.Rows.Clear();
            dgvTasks.Columns.Clear();

            dgvTasks.Columns.Add("Title", "Title");
            dgvTasks.Columns.Add("Description", "Description");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT Title, Description FROM Tasks";
                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    dgvTasks.Rows.Add(
                        reader["Title"].ToString(),
                        reader["Description"].ToString()
                    );
                }
            }
        }

        private void rtbChat_TextChanged(object sender, EventArgs e)
        {

        }
    }
}