using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace CaparasAct_3
{
    public partial class Form2 : Form
    {
        // Database connection string for the inventory system
        private readonly string _connectionUri = "server=localhost;database=inventorydb;uid=root;pwd=;";

        public Form2()
        {
            InitializeComponent();
        }

        // Initialize form settings when loaded
        private void Form2_Load(object sender, EventArgs e)
        {
            // Mask password characters for privacy
            Array.ForEach(new[] { textBoxPassword }, field => field.PasswordChar = '•');
        }

        // Handle login button click event
        private void button1_Click(object sender, EventArgs e)
        {
            string accountName = textBoxUsername.Text.Trim();
            string accountKey = textBoxPassword.Text;

            // Validate that input fields are not empty
            if (IsInputIncomplete(accountName, accountKey))
            {
                MessageBox.Show("Please enter both username and password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Verify user credentials in the database
                if (PerformCredentialValidation(accountName, accountKey))
                {
                    MessageBox.Show("Login successful! Welcome to the system.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Clear fields after successful login
                    textBoxUsername.Clear();
                    textBoxPassword.Clear();

                    // Open the main dashboard (Form3) and hide login
                    Form3 mainDashboard = new Form3();
                    mainDashboard.Show();
                    this.Hide();
                }
                else
                {
                    // Handle invalid credentials
                    MessageBox.Show("Invalid username or password. Please try again.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBoxPassword.Clear();
                    textBoxPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                // Handle database errors
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helper expression to check for empty inputs
        private bool IsInputIncomplete(string u, string p) => string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p);

        // Helper method to query the database and check credentials securely
        private bool PerformCredentialValidation(string user, string pass)
        {
            using (MySqlConnection connectionLink = new MySqlConnection(_connectionUri))
            {
                connectionLink.Open();
                string sqlCommandString = "SELECT COUNT(1) FROM Users WHERE Username = @usr AND Password = @pwd";
                
                using (MySqlCommand commandObject = new MySqlCommand(sqlCommandString, connectionLink))
                {
                    commandObject.Parameters.AddWithValue("@usr", user);
                    commandObject.Parameters.AddWithValue("@pwd", pass);

                    return Convert.ToInt32(commandObject.ExecuteScalar()) > 0;
                }
            }
        }

        private void label1_Click(object sender, EventArgs e) { }

        // Navigate back to the registration form (Form1)
        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form1 registrationScreen = new Form1();
            registrationScreen.Show();
            this.Hide();
        }
    }
}
