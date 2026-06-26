using System;
using System.Data.SqlClient;

namespace CyberSecurityChatbot
{
    public class DatabaseHelper
    {
        private string connectionString =
            "Server=localhost;Database=CyberBotDB;Trusted_Connection=True;";

        public void AddTask(string title, string description)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "INSERT INTO Tasks (Title, Description) VALUES (@title, @desc)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@desc", description);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}