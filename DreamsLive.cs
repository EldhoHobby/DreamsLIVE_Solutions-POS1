using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;

namespace DreamsLIVE_Solutions_POS1
{
    public partial class Main : Form
    {
        private string _connectionString;
        private Timer timer;
        private Dictionary<string, string> _configData;

        public Main()
        {
            InitializeComponent();
            LoadConnectionString();  // Load the connection string from config
            Check_SQL_Status.Click += Check_SQL_Status_Click;

            // Create and configure the timer to update the time every second
            timer = new Timer { Interval = 1000 };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private async void Check_SQL_Status_Click(object sender, EventArgs e)
        {
            Check_SQL_Status.BackColor = Color.Black;

            string configFilePath = @"C:\Users\Public\Documents\DreamsLive\DreamsLive_POS\Config\POS_Config.txt";
            bool isConnected = await Task.Run(() => CheckConnection(configFilePath));

            if (isConnected)
            {
                SQL_Status.Text = "Connected";
                SQL_Status.ForeColor = Color.Green;
                Check_SQL_Status.BackColor = Color.Green;
            }
            else
            {
                SQL_Status.Text = "Failed";
                SQL_Status.ForeColor = Color.Red;
                Check_SQL_Status.BackColor = Color.Red;
            }
        }

        private void LoadConnectionString()
        {
            if (_configData == null)
            {
                string configFilePath = @"C:\Users\Public\Documents\DreamsLive\DreamsLive_POS\Config\POS_Config.txt";
                var configData = ReadConfigFile(configFilePath);

                if (string.IsNullOrEmpty(configData))
                {
                    MessageBox.Show("Failed to read database configuration.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _configData = ParseConfigData(configData);
            }

            _connectionString = $"Server={_configData["SQLServer"]};Database={_configData["Database"]};User Id={_configData["UserId"]};Password={_configData["Password"]};";
        }

        private bool CheckConnection(string configFilePath)
        {
            var configData = ReadConfigFile(configFilePath);
            if (string.IsNullOrEmpty(configData))
                return false;

            var config = ParseConfigData(configData);
            var connection = BuildConnection(config);

            try
            {
                connection.Open();
                return true;
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"SQL Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        private SqlConnection BuildConnection(Dictionary<string, string> config)
        {
            string connectionString = $"Server={config["SQLServer"]};Database={config["Database"]};User Id={config["UserId"]};Password={config["Password"]};";
            return new SqlConnection(connectionString);
        }

        private string ReadConfigFile(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath);  // Read the file content
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading config file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;  // Return empty if there is an error
            }
        }

        private Dictionary<string, string> ParseConfigData(string configData)
        {
            var configDictionary = new Dictionary<string, string>();
            var lines = configData.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var parts = line.Split('=');
                if (parts.Length == 2)
                {
                    configDictionary[parts[0].Trim()] = parts[1].Trim();
                }
            }

            return configDictionary;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DateTime currentTime = DateTime.Now;
            lblTime.Text = currentTime.ToString("hh:mm:ss tt");  // Update the time label every second
        }

        // This function opens the LottoBalanceing form with "Balancing" title
        private void LottoBalanceing_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                MessageBox.Show("Database connection string is not available. Please check configuration.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open LottoBalanceing form with "Balancing"
            LottoBalanceing lottoForm = new LottoBalanceing("LOTTERY BALANCING", _connectionString);
            lottoForm.Owner = this;
            lottoForm.Show();
            lottoForm.TopMost = false;  // Ensures the form stays on top

            // Disable LottoBalanceing and LottoCloseing buttons
            LottoBalanceing.Enabled = false;
            LottoCloseing.Enabled = false;

            // Register the FormClosed event to re-enable the buttons when the form is closed
            lottoForm.FormClosed += LottoForm_FormClosed;
        }

        // This function opens the LottoBalanceing form with "Closing" title
        private void LottoCloseing_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                MessageBox.Show("Database connection string is not available. Please check configuration.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open LottoBalanceing form with "Closing"
            LottoBalanceing lottoForm = new LottoBalanceing("LOTTERY CLOSING", _connectionString);
            lottoForm.Owner = this;
            lottoForm.Show();
            lottoForm.TopMost = false;  // Ensures the form stays on top

            // Disable LottoBalanceing and LottoCloseing buttons
            LottoBalanceing.Enabled = false;
            LottoCloseing.Enabled = false;

            // Register the FormClosed event to re-enable the buttons when the form is closed
            lottoForm.FormClosed += LottoForm_FormClosed;
        }
        // This function will be triggered when the LottoBalanceing form is closed
        private void LottoForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Re-enable the LottoBalanceing and LottoCloseing buttons when the form is closed
            LottoBalanceing.Enabled = true;
            LottoCloseing.Enabled = true;
        }

        private void LT_button_Click(object sender, EventArgs e)
        {
            // Check if the LT_button path exists in the config data
            if (_configData != null && _configData.ContainsKey("LT_button"))
            {
                string exePath = _configData["LT_button"];

                // Check if the file exists at the given path
                if (File.Exists(exePath))
                {
                    try
                    {
                        // Start the application (e.g., Word, Excel, etc.)
                        System.Diagnostics.Process.Start(exePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error opening application: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("The specified executable path does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("LT_button path is not configured in the config file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
