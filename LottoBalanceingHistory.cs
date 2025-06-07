using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using DreamsLIVE_Solutions_POS1.Helpers;

namespace DreamsLIVE_Solutions_POS1
{
    public partial class LottoBalanceingHistory : Form
    {
        private string connectionString;
        private int currentPage = 0;
        private int pageSize = 10;

        private DateTime currentDate;
        private DateTime minDate;
        private DateTime maxDate;
        public LottoBalanceingHistory(string connString)
        {
            InitializeComponent();
            connectionString = connString;
            currentDate = DateTime.Today;  // Start with today's date                                           
            LoadDateRange();            
            // Initialize DateTimePicker to today's date
            dateTimePickerHist.Value = currentDate;
            ViewByDate.Checked = Properties.Settings.Default.IsViewByDateChecked;

            // Initialize the checkbox state
            ViewByDate_CheckedChanged(this, EventArgs.Empty);  // Update UI based on the checkbox state

            ViewCheckchanged();

            // Attach event handler for cell formatting
            LottoBalanceingHistView.CellFormatting += LottoBalanceingHistView_CellFormatting;

            // Set the font size to be larger
            LottoBalanceingHistView.DefaultCellStyle.Font = new Font("Arial", 11); // Change "Arial" and "14" to desired font and size

            // Adjust the row height (set it to a desired value, e.g., 35)
            LottoBalanceingHistView.RowTemplate.Height = 35;  // Adjust this value to your preference
        }

        private void LoadEntriesForPage(int page)
        {
            try
            {
                string query = $@"
            WITH OrderedData AS 
            (
                SELECT ROW_NUMBER() OVER (ORDER BY entry_date DESC, entry_time DESC) AS RowNum, 
                       SL_number, entry_date, 
                       RIGHT('0' + CAST((CASE WHEN DATEPART(HOUR, entry_time) % 12 = 0 THEN 12 ELSE DATEPART(HOUR, entry_time) % 12 END) AS VARCHAR), 2) 
                       + ':' + RIGHT('0' + CAST(DATEPART(MINUTE, entry_time) AS VARCHAR), 2) 
                       + ':' + RIGHT('0' + CAST(DATEPART(SECOND, entry_time) AS VARCHAR), 2) 
                       + ' ' + (CASE WHEN DATEPART(HOUR, entry_time) >= 12 THEN 'PM' ELSE 'AM' END) AS entry_time, -- Proper time format
                       Bal_Close, yesdep1, yesdep2, salenow, cashdraw, slipdraw, cashshortdep, T3Yesterday, T3Now, tickamount, diff_amount, short_extra, 
                       empcode, empworked, comment
                FROM LottoBalanceSheet
            )
                SELECT SL_number, entry_date, entry_time, Bal_Close, yesdep1, yesdep2, salenow, 
                        cashdraw, slipdraw, cashshortdep, T3Yesterday, T3Now, 
                        tickamount, diff_amount, empcode, empworked, short_extra, comment
            FROM OrderedData 
            WHERE RowNum BETWEEN {(page * pageSize) + 1} AND {(page + 1) * pageSize}";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        LottoBalanceingHistView.DataSource = dt;
                    }
                }
                // Customize column headers
                CustomizeColumnHeaders();

                // Freeze the first 3 columns
                LottoBalanceingHistView.Columns["entry_date"].Frozen = true;
                LottoBalanceingHistView.Columns["entry_time"].Frozen = true;
                LottoBalanceingHistView.Columns["Bal_Close"].Frozen = true;

                // Adjust the width of all columns to fit their content
                LottoBalanceingHistView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                LottoBalanceingHistView.Refresh(); // Ensure the DataGridView is updated
                UpdateNavigationButtons();
                // Disable sorting for all columns in the DataGridView
                foreach (DataGridViewColumn column in LottoBalanceingHistView.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LoadDateRange()
        {
            try
            {
                // Get the minimum and maximum entry dates
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT MIN(entry_date), MAX(entry_date) FROM LottoBalanceSheet";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                minDate = reader.GetDateTime(0);
                                maxDate = reader.GetDateTime(1);
                            }
                        }
                    }
                }
                // Disable buttons if at the min/max date
                UpdateNavigationButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading date range: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadEntriesForDate(DateTime date)
        {
            try
            {
                string query = $@"
                WITH OrderedData AS 
                (
                    SELECT ROW_NUMBER() OVER (ORDER BY entry_date DESC, entry_time DESC) AS RowNum, 
                           SL_number, entry_date, 
                           RIGHT('0' + CAST((CASE WHEN DATEPART(HOUR, entry_time) % 12 = 0 THEN 12 ELSE DATEPART(HOUR, entry_time) % 12 END) AS VARCHAR), 2) 
                           + ':' + RIGHT('0' + CAST(DATEPART(MINUTE, entry_time) AS VARCHAR), 2) 
                           + ':' + RIGHT('0' + CAST(DATEPART(SECOND, entry_time) AS VARCHAR), 2) 
                           + ' ' + (CASE WHEN DATEPART(HOUR, entry_time) >= 12 THEN 'PM' ELSE 'AM' END) AS entry_time, -- Proper time format
                           Bal_Close, yesdep1, yesdep2, salenow, cashdraw, slipdraw, cashshortdep, T3Yesterday, T3Now, tickamount, diff_amount, short_extra, 
                           empcode, empworked, comment
                    FROM LottoBalanceSheet
                    WHERE entry_date = @entry_date
                )
                SELECT SL_number, entry_date, entry_time, Bal_Close, yesdep1, yesdep2, salenow, 
                        cashdraw, slipdraw, cashshortdep, T3Yesterday, T3Now, 
                        tickamount, diff_amount, empcode, empworked, short_extra, comment
                FROM OrderedData
                ORDER BY SL_number DESC";  // Ensure ordering is by date and time, descending

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(query, conn))
                    {
                        adapter.SelectCommand.Parameters.AddWithValue("@entry_date", date);
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        LottoBalanceingHistView.DataSource = dt;
                    }
                }

                // Customize column headers (same as before)
                CustomizeColumnHeaders();

                // Freeze the first 3 columns
                LottoBalanceingHistView.Columns["entry_date"].Frozen = true;
                LottoBalanceingHistView.Columns["entry_time"].Frozen = true;
                LottoBalanceingHistView.Columns["Bal_Close"].Frozen = true;
                dateTimePickerHist.Value = currentDate;
                // Adjust the width of all columns to fit their content
                LottoBalanceingHistView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                LottoBalanceingHistView.Refresh(); // Ensure the DataGridView is updated
                UpdateNavigationButtonsDay();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CustomizeColumnHeaders()
        {
            LottoBalanceingHistView.Columns["SL_number"].HeaderText = "SN";
            LottoBalanceingHistView.Columns["entry_date"].HeaderText = "Date";
            LottoBalanceingHistView.Columns["entry_time"].HeaderText = "Time";
            LottoBalanceingHistView.Columns["Bal_Close"].HeaderText = "Bal/Close";
            LottoBalanceingHistView.Columns["yesdep1"].HeaderText = "Yes Dep";
            LottoBalanceingHistView.Columns["yesdep2"].HeaderText = "YesAct Dep";
            LottoBalanceingHistView.Columns["salenow"].HeaderText = "Sale Now";
            LottoBalanceingHistView.Columns["cashdraw"].HeaderText = "Cash draw";
            LottoBalanceingHistView.Columns["slipdraw"].HeaderText = "Slip Draw";
            LottoBalanceingHistView.Columns["cashshortdep"].HeaderText = "Short Dep";
            LottoBalanceingHistView.Columns["T3Yesterday"].HeaderText = "T3Yes Sale";
            LottoBalanceingHistView.Columns["T3Now"].HeaderText = "T3Now Sale";
            LottoBalanceingHistView.Columns["tickamount"].HeaderText = "TickAct Amt";
            LottoBalanceingHistView.Columns["diff_amount"].HeaderText = "#Dep Diff";
            LottoBalanceingHistView.Columns["short_extra"].HeaderText = "#Short/Extra";
            LottoBalanceingHistView.Columns["empcode"].HeaderText = "Emp Bal";
            LottoBalanceingHistView.Columns["empworked"].HeaderText = "Emp Worked";
            LottoBalanceingHistView.Columns["comment"].HeaderText = "Comment";
        }

        private void UpdateNavigationButtonsDay()
        {
            btnPreviousDay.Enabled = currentDate > minDate;
            btnNextDay.Enabled = currentDate < maxDate;
        }
        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            if (currentPage > 0)
            {
                currentPage--;
                LoadEntriesForPage(currentPage);
            }
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            currentPage++;
            LoadEntriesForPage(currentPage);
        }

        private void UpdateNavigationButtons()
        {
            btnMoveUp.Enabled = currentPage > 0;

            int totalRows = GetTotalRowCount();
            btnMoveDown.Enabled = (currentPage + 1) * pageSize < totalRows;
        }

        private int GetTotalRowCount()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM LottoBalanceSheet", conn))
                    {
                        conn.Open();
                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch
            {
                return 0;
            }
        }

        private void LottoBalanceingHistView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridViewHelper.ApplyCellFormatting(LottoBalanceingHistView, sender, e);
        }

        private void btnPreviousDay_Click(object sender, EventArgs e)
        {
            if (currentDate > minDate)
            {
                currentDate = currentDate.AddDays(-1);
                LoadEntriesForDate(currentDate);
            }
        }

        private void btnNextDay_Click(object sender, EventArgs e)
        {
            if (currentDate < maxDate)
            {
                currentDate = currentDate.AddDays(1);
                LoadEntriesForDate(currentDate);
            }
        }
        // Event handler for DateTimePicker value change
        private void dateTimePickerHist_ValueChanged(object sender, EventArgs e)
        {
            currentDate = dateTimePickerHist.Value.Date; // Get the selected date
            LoadEntriesForDate(currentDate);

        }

        private void ViewByDate_CheckedChanged(object sender, EventArgs e)
        {
            // Save the checkbox state in settings
            Properties.Settings.Default.IsViewByDateChecked = ViewByDate.Checked;
            Properties.Settings.Default.Save();
            ViewCheckchanged();

        }

        private void ViewCheckchanged()
        {
            if (ViewByDate.Checked)
            {
                LoadEntriesForDate(currentDate);
                btnMoveUp.Enabled = false;
                btnMoveDown.Enabled = false;
                btnPreviousDay.Enabled = currentDate > minDate;
                btnNextDay.Enabled = currentDate < maxDate;
                UpdateNavigationButtonsDay();
            }
            else
            {
                LoadEntriesForPage(currentPage);
                btnPreviousDay.Enabled = false;
                btnNextDay.Enabled = false;
                btnMoveUp.Enabled = true;
                btnMoveDown.Enabled = true;
                UpdateNavigationButtons();
            }
        }
    }
}
