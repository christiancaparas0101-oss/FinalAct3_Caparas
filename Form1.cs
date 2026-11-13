using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace CaparasAct_3
{
    public partial class Form1 : Form
    {
        private readonly string dbServerConnection = "server=localhost;database=InventoryDB;uid=root;pwd=;";

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ConfigurePasswordMasking();
        }

        private void ConfigurePasswordMasking()
        {
            textBox2.PasswordChar = '•';
            textBox4.PasswordChar = '•';
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Modularized validation pipeline
            if (!AreInputsValid() || !DoPasswordsMatch())
                return;

            try
            {
                using (var connection = new MySqlConnection(dbServerConnection))
                {
                    connection.Open();

                    // Verify if username already exists in database
                    if (CheckIfUsernameExists(connection, textBox3.Text.Trim()))
                    {
                        MessageBox.Show("Username is already taken. Please choose another one.", "Registration Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Execute insertion and clean up
                    ExecuteUserRegistration(connection);
                    MessageBox.Show("Account successfully created!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    WipeAllFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool AreInputsValid()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || 
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrEmpty(textBox2.Text) || 
                string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("Please fill out all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private bool DoPasswordsMatch()
        {
            if (textBox2.Text != textBox4.Text)
            {
                MessageBox.Show("Passwords do not match!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox2.Clear();
                textBox4.Clear();
                textBox2.Focus();
                return false;
            }
            return true;
        }

        private bool CheckIfUsernameExists(MySqlConnection conn, string targetUser)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Username = @usr";
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@usr", targetUser);
                int recordCount = Convert.ToInt32(cmd.ExecuteScalar());
                return recordCount > 0;
            }
        }

        private void ExecuteUserRegistration(MySqlConnection conn)
        {
            string query = "INSERT INTO Users (FullName, Username, Password) VALUES (@fn, @un, @pw)";
            using (var cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@fn", textBox1.Text.Trim());
                cmd.Parameters.AddWithValue("@un", textBox3.Text.Trim());
                cmd.Parameters.AddWithValue("@pw", textBox2.Text);
                cmd.ExecuteNonQuery();
            }
        }

        private void WipeAllFields()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var loginWindow = new Form2();
            loginWindow.Show();
            this.Hide();
        }

        private void label1_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }
    }
}
