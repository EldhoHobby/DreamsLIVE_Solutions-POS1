using DreamsLIVE_Solutions_POS1.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection.Emit;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace DreamsLIVE_Solutions_POS1
{
    public partial class LottoBalanceing : Form
    {
        private string _connectionString;
        private string internalText1 = "Form Open";  // Internal text point
        private string CommentTextHere = "Enter your comment here...";
        public LottoBalanceing(string action, string connectionString)
        {
            InitializeComponent();
            _connectionString = connectionString;
            LoadEntriesForPage();
            // Set the Bal_Close textbox text based on the action
            label1.Text = action;
            Bal_Close_Setup();
            // Attach event handler for cell formatting
            LottoBalanceingHistView.CellFormatting += LottoBalanceingHistView_CellFormatting;

            // Set the font size to be larger
            LottoBalanceingHistView.DefaultCellStyle.Font = new Font("Arial", 9); // Change "Arial" and "14" to desired font and size

            // Adjust the row height (set it to a desired value, e.g., 35)
            LottoBalanceingHistView.RowTemplate.Height = 35;  // Adjust this value to your preference
            ClearAll.Visible = false;

            // Set up the AutoComplete properties for empcode and empworked
            empcode.AutoCompleteMode = AutoCompleteMode.Append;
            empcode.AutoCompleteSource = AutoCompleteSource.CustomSource;
            empworked.AutoCompleteMode = AutoCompleteMode.Append;
            empworked.AutoCompleteSource = AutoCompleteSource.CustomSource;

            // Create an AutoComplete string collection
            AutoCompleteStringCollection autoCompleteData = new AutoCompleteStringCollection();

            // Define the file path
            string filePath = @"C:\Users\Public\Documents\DreamsLive\DreamsLive_POS\Config\Emp_Codes.txt";

            // Read the lines from the file and add them to the AutoComplete collection
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                autoCompleteData.AddRange(lines);
            }
            else
            {
                MessageBox.Show("The file 'Emp_Codes.txt' was not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Assign the AutoComplete data to both empcode and empworked TextBoxes
            empcode.AutoCompleteCustomSource = autoCompleteData;
            empworked.AutoCompleteCustomSource = autoCompleteData;

            // Handle the TextChanged event for both empcode and empworked
            empcode.TextChanged += (sender, e) =>
            {
                if (empcode.Text.Length > 0)
                {
                    empcode.Text = char.ToUpper(empcode.Text[0]) + empcode.Text.Substring(1);
                    empcode.SelectionStart = empcode.Text.Length;
                }
            };

            empworked.TextChanged += (sender, e) =>
            {
                if (empworked.Text.Length > 0)
                {
                    empworked.Text = char.ToUpper(empworked.Text[0]) + empworked.Text.Substring(1);
                    empworked.SelectionStart = empworked.Text.Length;
                }
            };

        }
        

        private SqlConnection BuildConnection()
        {
            return new SqlConnection(_connectionString);
        }

        private void LottoBalanceing_Load(object sender, EventArgs e)
        {
            SetCommentText();
            try
            {
                using (var connection = BuildConnection())
                {
                    connection.Open();
                    //MessageBox.Show("Connected to the database successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Disable sorting for all columns in the DataGridView
                    foreach (DataGridViewColumn column in LottoBalanceingHistView.Columns)
                    {
                        column.SortMode = DataGridViewColumnSortMode.NotSortable;
                    }
                }
               // LoadLastFiveEntries(1, 5); // Load last 5 entries into the DataGridView
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database connection failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Method to insert data into the LottoBalanceSheet table
        private void InsertDataToDatabase()
        {
            
            using (SqlConnection connection = BuildConnection())
            {
                try
                {
                    connection.Open();

                    string query = "INSERT INTO dbo.LottoBalanceSheet " +
                                   "(entry_date, entry_time, Bal_Close, yesdep1, yesdep2, salenow, cashdraw, slipdraw, cashshortdep, tickamount, empcode, empworked, " +
                                   "diff_amount, new_dep_amount, req_ticket_amount, req_amt_af_sd, cash_draw, req_cash, short_extra, T3Yesterday, T3Now) " +
                                   "VALUES (@entry_date, @entry_time, @Bal_Close, @yesdep1, @yesdep2, @salenow, @cashdraw, @slipdraw, @cashshortdep, @tickamount, @empcode, @empworked, " +
                                   "@diff_amount, @new_dep_amount, @req_ticket_amount, @req_amt_af_sd, @cash_draw, @req_cash, @short_extra, @T3Yesterday, @T3Now)";

                    // Convert text values to decimal
                    decimal yesdep1Value = Convert.ToDecimal(yesdep1.Text);
                    decimal yesdep2Value = Convert.ToDecimal(yesdep2.Text);
                    decimal salenowValue = Convert.ToDecimal(salenow.Text);
                    decimal cashdrawValue = Convert.ToDecimal(cashdraw.Text);
                    decimal slipdrawValue = Convert.ToDecimal(slipdraw.Text);
                    decimal cashshortdepValue = Convert.ToDecimal(cashshortdep.Text);
                    decimal tickamountValue = Convert.ToDecimal(tickamount.Text);
                    decimal t3yesterdayValue = Convert.ToDecimal(T3Yesterday.Text);
                    decimal t3nowValue = Convert.ToDecimal(T3Now.Text);

                    // Perform calculations
                    decimal diff_amount = yesdep2Value - yesdep1Value;
                    decimal new_dep_amount = salenowValue + diff_amount;
                    decimal T3_Sale = t3yesterdayValue + t3nowValue;
                    decimal req_ticket_amount = 10500m - tickamountValue;
                    decimal req_amt_af_sd = req_ticket_amount + cashshortdepValue;
                    decimal cash_draw = (slipdrawValue + T3_Sale) + cashdrawValue;
                    decimal req_cash = cash_draw - new_dep_amount;
                    decimal short_extra = req_cash - req_amt_af_sd;


                    // Format the values as strings with two decimal places
                    ListBox2.Items.Clear();  // Clear previous items
                    ListBox2.Items.Add(short_extra.ToString("F2"));  // Add the calculated value as an item
                    ListBox1.Items.Clear();
                    ListBox1.Items.Add("CURRENT");
                    DepAmt.Text = new_dep_amount.ToString("F2");  // Format with two decimal places
                    // Change ListBox2 text color based on short_extra value
                    if (short_extra == 0m)
                    {
                        ListBox2.ForeColor = Color.Green; // Show in green if 0
                        ListBox1.ForeColor = Color.Green; // Show in green if 0
                    }
                    else
                    {
                        ListBox2.ForeColor = Color.Red; // Show in red if not 0
                        ListBox1.ForeColor = Color.Red; // Show in red if not 0
                    }
                    // Add formatted values to the SQL command parameters
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@entry_date", DateTime.Now.AddDays(0).Date);
                        command.Parameters.AddWithValue("@entry_time", DateTime.Now.TimeOfDay);
                        command.Parameters.AddWithValue("@Bal_Close", internalText1);
                        command.Parameters.AddWithValue("@yesdep1", yesdep1Value.ToString("F2"));
                        command.Parameters.AddWithValue("@yesdep2", yesdep2Value.ToString("F2"));
                        command.Parameters.AddWithValue("@salenow", salenowValue.ToString("F2"));
                        command.Parameters.AddWithValue("@cashdraw", cashdrawValue.ToString("F2"));
                        command.Parameters.AddWithValue("@slipdraw", slipdrawValue.ToString("F2"));
                        command.Parameters.AddWithValue("@cashshortdep", cashshortdepValue.ToString("F2"));
                        command.Parameters.AddWithValue("@tickamount", tickamountValue.ToString("F2"));
                        command.Parameters.AddWithValue("@empcode", empcode.Text);
                        command.Parameters.AddWithValue("@empworked", empworked.Text);

                        // Add calculated values, formatted as needed
                        command.Parameters.AddWithValue("@diff_amount", diff_amount.ToString("F2"));
                        command.Parameters.AddWithValue("@new_dep_amount", new_dep_amount.ToString("F2"));
                        command.Parameters.AddWithValue("@req_ticket_amount", req_ticket_amount.ToString("F2"));
                        command.Parameters.AddWithValue("@req_amt_af_sd", req_amt_af_sd.ToString("F2"));
                        command.Parameters.AddWithValue("@cash_draw", cash_draw.ToString("F2"));
                        command.Parameters.AddWithValue("@req_cash", req_cash.ToString("F2"));
                        command.Parameters.AddWithValue("@short_extra", short_extra.ToString("F2"));

                        command.Parameters.AddWithValue("@T3Yesterday", t3yesterdayValue.ToString("F2"));
                        command.Parameters.AddWithValue("@T3Now", t3nowValue.ToString("F2"));

                        command.ExecuteNonQuery();
                    }

                    //MessageBox.Show("Data successfully saved to the database!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    empcode.Text = "";
                    // Set short_extra value in ListBox2
                    ListBox2.Items.Clear();  // Clear previous items
                    ListBox2.Items.Add(short_extra.ToString("F2"));  // Add the calculated value as an item
                    ListBox1.Items.Clear();
                    ListBox1.Items.Add("CURRENT");
                    LoadEntriesForPage(); // Reload the DataGridView with the updated data
                    CommentAdd.Text = "Enter your comment here...";
                    ClearAll.Visible = true;

                    SetAllTextFieldsToBlue();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ListBox2.Enabled = false;
                }
            }
        }


        // Method to validate user inputs and highlight invalid fields
        private bool ValidateInput()
        {
            bool isValid = true; // Flag to track if all inputs are valid

            // Helper function to validate decimal fields
            bool ValidateDecimalField(TextBox textBox)
            {
                decimal tempValue;
                if (!decimal.TryParse(textBox.Text, out tempValue))
                {
                    HighlightError(textBox);
                    if (isValid) textBox.Focus(); // Focus on the first invalid field
                    isValid = false;
                    return false;
                }
                ResetFieldColor(textBox);
                return true;
            }

            // Helper function to validate non-empty fields
            bool ValidateNonEmptyField(TextBox textBox)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    HighlightError(textBox);
                    if (isValid) textBox.Focus(); // Focus on the first invalid field
                    isValid = false;
                    return false;
                }
                ResetFieldColor(textBox);
                return true;
            }

            // Validate decimal fields
            ValidateDecimalField(yesdep1);
            ValidateDecimalField(yesdep2);
            ValidateDecimalField(salenow);
            ValidateDecimalField(cashdraw);
            ValidateDecimalField(slipdraw);
            ValidateDecimalField(cashshortdep);
            ValidateDecimalField(tickamount);
            ValidateDecimalField(T3Yesterday);
            ValidateDecimalField(T3Now);

            // Validate non-empty fields
            ValidateNonEmptyField(empcode);
            ValidateNonEmptyField(empworked);

            if (!isValid)
            {
                ListBox1.Items.Clear();
                ListBox2.Items.Clear();
                MessageBox.Show("Please correct the highlighted fields before submitting.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }

            return isValid; // Return whether all inputs are valid
        }

        // Method to highlight invalid textboxes (red border)
        private void HighlightError(TextBox textBox)
        {
            textBox.BackColor = System.Drawing.Color.LightCoral; // Set background to red
        }

        // Method to reset the textbox color (to default)
        private void ResetFieldColor(TextBox textBox)
        {
            textBox.BackColor = System.Drawing.Color.White; // Reset background to default
        }


        private void calculateshort_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                if (label1.Text == "LOTTERY CLOSING")
                {
                    salenow.Text = "0";
                    InsertDataToDatabase();
                }
               else
               {
                    InsertDataToDatabase();
               }
            }
        }

        // Method to fetch the last entry from the database and populate textboxes
        private void FetchLastEntry()
        {
            using (SqlConnection connection = BuildConnection())
            {
                try
                {
                    connection.Open();

                    string query = "SELECT TOP 1 * FROM dbo.LottoBalanceSheet ORDER BY SL_number DESC";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Populate textboxes with the last entry, formatting to 2 decimal places
                                yesdep1.Text = Convert.ToDecimal(reader["yesdep1"]).ToString("F2");
                                yesdep2.Text = Convert.ToDecimal(reader["yesdep2"]).ToString("F2");
                                salenow.Text = Convert.ToDecimal(reader["salenow"]).ToString("F2");
                                cashdraw.Text = Convert.ToDecimal(reader["cashdraw"]).ToString("F2");
                                slipdraw.Text = Convert.ToDecimal(reader["slipdraw"]).ToString("F2");
                                cashshortdep.Text = Convert.ToDecimal(reader["cashshortdep"]).ToString("F2");
                                tickamount.Text = Convert.ToDecimal(reader["tickamount"]).ToString("F2");
                                //empcode.Text = reader["empcode"].ToString();
                                empworked.Text = reader["empworked"].ToString();
                                T3Yesterday.Text = Convert.ToDecimal(reader["T3Yesterday"]).ToString("F2");
                                T3Now.Text = Convert.ToDecimal(reader["T3Now"]).ToString("F2");

                                // Populate ListBox2 with the value of short_extra
                                decimal short_extra = Convert.ToDecimal(reader["short_extra"]);
                                ListBox2.Items.Clear();  // Clear any previous entries
                                ListBox2.Items.Add(short_extra.ToString("F2"));  // Show short_extra value

                                // Set ListBox1 to show "Previous" and color it blue
                                ListBox1.Items.Clear();  // Clear previous items
                                ListBox1.Items.Add("Previous");  // Add "Previous" to ListBox1
                                ListBox1.ForeColor = Color.Blue;  // Set ListBox1 text color to blue

                                // Set ListBox2 color based on short_extra value
                                if (short_extra == 0m)
                                {
                                    ListBox2.ForeColor = Color.Green;  // Green for zero
                                }
                                else
                                {
                                    ListBox2.ForeColor = Color.Red;  // Red for non-zero
                                }

                               // MessageBox.Show("Last entry loaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("No previous entry found!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching last entry: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void PreviousEntryCall_Click(object sender, EventArgs e)
        {
            ResetAllFields();
            FetchLastEntry();
            SetAllTextFieldsToBlue();
            ClearAll.Visible = true;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            ResetAllFields();
            ClearAllFields();
        }
        // Method to clear all textboxes
        private void ClearAllFields()
        {
            yesdep1.Text = "";
            yesdep2.Text = "";
            salenow.Text = "";
            cashdraw.Text = "";
            slipdraw.Text = "";
            cashshortdep.Text = "";
            tickamount.Text = "";
            empcode.Text = "";
            empworked.Text = "";
            ListBox1.Items.Add("");
            ListBox2.Items.Add("");
            ListBox1.Text = "";
            ListBox2.Text = "";
            T3Yesterday.Text = "";
            T3Now.Text = "";
            ClearAll.Visible = false;

        }
        // Method to reset all textboxes to their default values (clear text and reset colors)
        private void ResetAllFields()
        {
            // Clear all textboxes
            yesdep1.Clear();
            yesdep2.Clear();
            salenow.Clear();
            cashdraw.Clear();
            slipdraw.Clear();
            cashshortdep.Clear();
            tickamount.Clear();
            empcode.Clear();
            empworked.Clear();
            T3Yesterday.Clear(); 
            T3Now.Clear();        

            // Reset all textbox background colors to default (white)
            ResetFieldColors();
        }
        // Method to reset field colors to normal (white)
        private void ResetFieldColors()
        {
            yesdep1.BackColor = Color.White;
            yesdep2.BackColor = Color.White;
            salenow.BackColor = Color.White;
            cashdraw.BackColor = Color.White;
            slipdraw.BackColor = Color.White;
            cashshortdep.BackColor = Color.White;
            tickamount.BackColor = Color.White;
            empcode.BackColor = Color.White;
            empworked.BackColor = Color.White;
            T3Yesterday.BackColor = Color.White; 
            T3Now.BackColor = Color.White;        
        }
        private void ListBox1_Enter(object sender, EventArgs e)
        {
            this.ActiveControl = null;
        }
        private void ListBox2_Enter(object sender, EventArgs e)
        {
            this.ActiveControl = null;
        }
        // Method to Set the Bal_Close textbox text based on the action
        private void Bal_Close_Setup()
        {
            if (label1.Text == "LOTTERY CLOSING")
            {
                label1.Text = "LOTTERY CLOSING";
                internalText1 = "CLOSING";
                label1.ForeColor = Color.DarkRed;
                label5.Visible = false;
                salenow.Visible = false;
                label6.Text = "Today's Sale Total :";
                label7.Text = "Today's Actual Sale Total :";
                label6.TextAlign = ContentAlignment.MiddleRight;
                label7.TextAlign = ContentAlignment.MiddleRight;
                calculatedep.Visible = false;
                label12.Visible = false;
                DepAmt.Visible = false;
                label14.Visible = false;
                label12.Visible = false;
                DepAmt.Visible = false;
                this.BackColor = Color.SandyBrown; // Change to any color
                salenow.Text = "0";
                SwitchButton.BackColor = Color.Turquoise;
                SwitchButton.Text = "Switch to Balancing";
            }
            else
            {
                label1.Text = "LOTTERY BALANCING";
                internalText1 = "BALANCING";
                label1.ForeColor = Color.Blue;
                label5.Visible = true;
                salenow.Visible = true;
                label6.Text = "Yesterday's Sale Total :";
                label7.Text = "Yesterday's Actual Sale Total :";
                label6.TextAlign = ContentAlignment.MiddleRight;
                label7.TextAlign = ContentAlignment.MiddleRight;
                calculatedep.Visible = true;
                label12.Visible = true;
                DepAmt.Visible = true;
                label14.Visible = true;
                label12.Visible = false;
                DepAmt.Visible = false;
                this.BackColor = Color.Turquoise; // Change to any color
                salenow.Text = "";
                SwitchButton.BackColor = Color.SandyBrown;
                SwitchButton.Text = "Switch to Closing";
            }
            
        }

        private void label4_Click(object sender, EventArgs e)
        {
            if (label1.Text == "LOTTERY CLOSEING")
            {
                MessageBox.Show("Take Todays Report From Draw. (Todays Deposit Slip) ", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Take Report From Draw. and Take Value from Bottom. ", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            
        }

        private void label13_Click(object sender, EventArgs e)
        {
            if (label1.Text == "LOTTERY CLOSEING")
            {
                MessageBox.Show("Print Again Today's Report from Lotto Meachine and Add last 2 Values. ", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Print Yesterday's Report from Lotto Meachine and Add last 2 Values. ", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void label14_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Print Today's Report from Lotto Meachine and Add last 2 Values. ", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void calculatedep_Click(object sender, EventArgs e)
            {
            // Validate and highlight textboxes if data is missing
            bool isValid = true;

            // Check if yesdep1 is empty
            if (string.IsNullOrWhiteSpace(yesdep1.Text))
            {
                yesdep1.BackColor = Color.LightCoral; // Highlight in red if empty
                isValid = false;
            }
            else
            {
                yesdep1.BackColor = Color.White; // Reset color if not empty
            }

            // Check if yesdep2 is empty
            if (string.IsNullOrWhiteSpace(yesdep2.Text))
            {
                yesdep2.BackColor = Color.LightCoral; // Highlight in red if empty
                isValid = false;
            }
            else
            {
                yesdep2.BackColor = Color.White; // Reset color if not empty
            }

            // Check if salenow is empty
            if (string.IsNullOrWhiteSpace(salenow.Text))
            {
                salenow.BackColor = Color.LightCoral; // Highlight in red if empty
                isValid = false;
            }
            else
            {
                salenow.BackColor = Color.White; // Reset color if not empty
            }

            // If any of the fields are invalid (missing data), show a message and stop the calculation
            if (!isValid)
            {
                MessageBox.Show("Please fill in all the required fields before calculating.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Stop further processing if fields are empty
            }

            // Convert text values to decimals
            decimal yesdep1Value = Convert.ToDecimal(yesdep1.Text);
            decimal yesdep2Value = Convert.ToDecimal(yesdep2.Text);
            decimal salenowValue = Convert.ToDecimal(salenow.Text);

            // Calculate DepAmt
            label12.Visible = true;
            DepAmt.Visible = true;
            ListBox1.Items.Clear();
            ListBox2.Items.Clear();
            decimal depAmt = (yesdep2Value - yesdep1Value) + salenowValue;
            DepAmt.Text = depAmt.ToString("F2");

            // Show the result (you can display it in a label or textbox)
            //MessageBox.Show($"DepAmt is: {depAmt}", "Calculation Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnViewHistory_Click(object sender, EventArgs e)
        {
            // Disable the button when clicked
            btnViewHistory.Enabled = false;

            // Create the history form and pass the connection string
            string connString = _connectionString; // Use your actual connection string
            LottoBalanceingHistory historyForm = new LottoBalanceingHistory(connString);

            // Set the owner to the current form
            historyForm.Owner = this;

            // Set the history form to always stay on top of the main form
            historyForm.TopMost = false;

            // Add a handler to re-enable the button when the form is closed
            historyForm.FormClosed += (s, args) =>
            {
                btnViewHistory.Enabled = true;  // Enable the button again after the form is closed
            };

            // Show the history form
            historyForm.Show();
        }

        private void LoadEntriesForPage()
        {
            try
            {
                string query = @"
                    SELECT TOP 100 
                           entry_date, 
                           RIGHT('0' + CAST((CASE WHEN DATEPART(HOUR, entry_time) % 12 = 0 THEN 12 ELSE DATEPART(HOUR, entry_time) % 12 END) AS VARCHAR), 2) 
                           + ':' + RIGHT('0' + CAST(DATEPART(MINUTE, entry_time) AS VARCHAR), 2) 
                           + ':' + RIGHT('0' + CAST(DATEPART(SECOND, entry_time) AS VARCHAR), 2) 
                           + ' ' + (CASE WHEN DATEPART(HOUR, entry_time) >= 12 THEN 'PM' ELSE 'AM' END) AS entry_time,
                           Bal_Close, yesdep1, yesdep2, salenow, 
                           cashdraw, slipdraw, cashshortdep, T3Yesterday, T3Now, tickamount, diff_amount, short_extra, empcode, empworked, comment
                    FROM LottoBalanceSheet
                    ORDER BY SL_number DESC"; // Get the most recent entry

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        LottoBalanceingHistView.DataSource = dt;
                    }
                }
                // Customize column headers
                LottoBalanceingHistView.Columns["entry_date"].HeaderText =      "Date";
                LottoBalanceingHistView.Columns["entry_time"].HeaderText =      "Time";
                LottoBalanceingHistView.Columns["Bal_Close"].HeaderText =       "Bal/Close";
                LottoBalanceingHistView.Columns["yesdep1"].HeaderText =         "Yes Dep";
                LottoBalanceingHistView.Columns["yesdep2"].HeaderText =         "YesAct Dep";
                LottoBalanceingHistView.Columns["salenow"].HeaderText =         "Sale Now";
                LottoBalanceingHistView.Columns["cashdraw"].HeaderText =        "Cash draw";
                LottoBalanceingHistView.Columns["slipdraw"].HeaderText =        "Slip Draw";
                LottoBalanceingHistView.Columns["cashshortdep"].HeaderText =    "Short Dep";
                LottoBalanceingHistView.Columns["T3Yesterday"].HeaderText =     "T3Yes Sale";
                LottoBalanceingHistView.Columns["T3Now"].HeaderText =           "T3Now Sale";
                LottoBalanceingHistView.Columns["tickamount"].HeaderText =      "TickAct Amt";
                LottoBalanceingHistView.Columns["diff_amount"].HeaderText =     "#Dep Diff";
                LottoBalanceingHistView.Columns["short_extra"].HeaderText =     "#Short/Extra";
                LottoBalanceingHistView.Columns["empcode"].HeaderText =         "Emp Bal";
                LottoBalanceingHistView.Columns["empworked"].HeaderText =       "Emp Worked";
                LottoBalanceingHistView.Columns["comment"].HeaderText =         "Comment";

                // Freeze the first 3 columns
                LottoBalanceingHistView.Columns["entry_date"].Frozen =true;
                LottoBalanceingHistView.Columns["entry_time"].Frozen = true;
                LottoBalanceingHistView.Columns["Bal_Close"].Frozen = true;

                // Adjust the width of all columns to fit their content
                LottoBalanceingHistView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                LottoBalanceingHistView.Refresh(); // Ensure the DataGridView is updated
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LottoBalanceingHistView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridViewHelper.ApplyCellFormatting(LottoBalanceingHistView, sender, e);
        }

        private void CommentAddButton_Click(object sender, EventArgs e)
        {
            // Check if the text is empty, or if it matches placeholder text
            if (string.IsNullOrWhiteSpace(CommentAdd.Text) ||
                CommentAdd.Text == "Enter your comment here..." ||
                CommentAdd.Text == "Comment Added, Modify Comment Here" ||
                CommentAdd.Text == "Add Comment Here")
            {
                // Display a warning if no valid comment is entered
                MessageBox.Show("Please add a comment before submitting.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // If valid comment, proceed with inserting data
                InsertCmdToDatabase();
            }
        }

        private void InsertCmdToDatabase()
        {
            using (SqlConnection connection = BuildConnection())
            {
                try
                {
                    connection.Open();

                    // Step 1: Get the last entry based on the most recent entry_date and entry_time
                    string getLastEntryQuery = @"
                SELECT TOP 1 SL_number FROM dbo.LottoBalanceSheet 
                ORDER BY entry_date DESC, entry_time DESC";

                    int lastEntryId = -1; // Default value if no entry is found

                    using (SqlCommand getLastEntryCommand = new SqlCommand(getLastEntryQuery, connection))
                    {
                        object result = getLastEntryCommand.ExecuteScalar();
                        if (result != null && int.TryParse(result.ToString(), out int id))
                        {
                            lastEntryId = id;
                        }
                    }

                    if (lastEntryId != -1)
                    {
                        // Step 2: Update the last entry with the comment
                        string updateQuery = @"
                    UPDATE dbo.LottoBalanceSheet 
                    SET comment = @comment 
                    WHERE SL_number = @lastEntryId";

                        using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                        {
                            string commentText = string.IsNullOrWhiteSpace(CommentAdd.Text) || CommentAdd.Text == "Enter your comment here..."
                                ? DBNull.Value.ToString()
                                : CommentAdd.Text;

                            updateCommand.Parameters.AddWithValue("@comment", commentText ?? (object)DBNull.Value);
                            updateCommand.Parameters.AddWithValue("@lastEntryId", lastEntryId);

                            int rowsAffected = updateCommand.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                LoadEntriesForPage();
                                   CommentAdd.Text = "Comment Added, Modify Comment Here";
                                   CommentAdd.ForeColor = Color.LightGray;
                            } 
                            else
                            {
                                MessageBox.Show("No entry found to update.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("No entries found in the database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SetAllTextFieldsToBlue()
        {
            yesdep1.ForeColor = Color.DarkBlue;
            yesdep2.ForeColor = Color.DarkBlue;
            salenow.ForeColor = Color.DarkBlue;
            cashdraw.ForeColor = Color.DarkBlue;
            slipdraw.ForeColor = Color.DarkBlue;
            cashshortdep.ForeColor = Color.DarkBlue;
            tickamount.ForeColor = Color.DarkBlue;
            T3Yesterday.ForeColor = Color.DarkBlue;
            T3Now.ForeColor = Color.DarkBlue;
            empcode.ForeColor = Color.DarkBlue;
            empworked.ForeColor = Color.DarkBlue;
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.ForeColor = Color.Black; // Change color to black when text changes
                textBox.BackColor = System.Drawing.Color.White; // Set background to red // Change Background color to WhiteSmoke when text changes
            }
        }

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow numbers, math operators, and backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && !"+-*/().".Contains(e.KeyChar))
            {
                e.Handled = true; // Block invalid input
            }

            // When "Enter" is pressed, evaluate the expression
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true; // Prevents the beep sound

                if (sender is TextBox textBox)
                {
                    try
                    {
                        string input = textBox.Text.Trim();

                        // Validate: Ensure input is not empty or ending in an operator
                        if (string.IsNullOrWhiteSpace(input) || "+-*/".Contains(input.Last()))
                        {
                            textBox.BackColor = Color.LightCoral; // Highlight invalid input
                            return;
                        }

                        // Solve using DataTable
                        DataTable dt = new DataTable();
                        var result = dt.Compute(input, "").ToString();

                        // Display result in the same TextBox
                        textBox.Text = result;
                        textBox.SelectionStart = textBox.Text.Length; // Keep cursor at end
                        textBox.BackColor = Color.LightGreen; // Highlight valid result
                    }
                    catch
                    {
                        textBox.BackColor = Color.LightCoral; // Error: invalid equation
                    }
                }
            }
        }


        // Function to set placeholder text
        private void SetCommentText()
        {
            CommentAdd.Text = CommentTextHere;
            CommentAdd.ForeColor = Color.LightGray;
        }


        private void CommentAdd_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CommentAdd.Text))
            {
                CommentAdd.Text = "Add Comment Here";
                CommentAdd.ForeColor = Color.LightGray;
            }
        }

        private void CommentAdd_Enter(object sender, EventArgs e)
        {
            if (CommentAdd.Text == "Enter your comment here..." ||
                CommentAdd.Text == "Comment Added, Modify Comment Here" ||
                CommentAdd.Text == "Add Comment Here")
            {
                CommentAdd.Text = "";
                CommentAdd.ForeColor = Color.Black; // Normal text color
            }
        }

        private void ClearAll_Click(object sender, EventArgs e)
        {
            ClearAllFields();
        }

        private void SwitchButton_Click(object sender, EventArgs e)
        {
            string currentText = label1.Text.Trim();

            if (currentText == "LOTTERY BALANCING")
            {
                label1.Text = "LOTTERY CLOSING";

            }
            else if (currentText == "LOTTERY CLOSING")
            {
                label1.Text = "LOTTERY BALANCING";

            }
            else
            {
                MessageBox.Show("Unexpected label value!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Bal_Close_Setup();
        }

    }
}
