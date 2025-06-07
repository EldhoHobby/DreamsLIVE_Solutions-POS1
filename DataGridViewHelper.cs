using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DreamsLIVE_Solutions_POS1.Helpers
{
    public static class DataGridViewHelper
    {
        public static void ApplyCellFormatting(DataGridView dataGridView, object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Ensure row and column indexes are valid (skip header rows)
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var columnName = dataGridView.Columns[e.ColumnIndex].Name;

            // Check and format entry_date column
            if (columnName == "entry_date")
            {
                var cellValue = dataGridView.Rows[e.RowIndex].Cells["entry_date"].Value;
                if (cellValue != null && cellValue != DBNull.Value)
                {
                    DateTime entryDate = Convert.ToDateTime(cellValue);
                    DataGridViewRow row = dataGridView.Rows[e.RowIndex];

                    // Define alternating colors
                    Color color1 = Color.White;
                    Color color2 = Color.LightGray;
                    Color currentColor = color1; // Start with the first color

                    DateTime? previousEntryDate = null; // Store the last processed date

                    foreach (DataGridViewRow gridRow in dataGridView.Rows) // Renamed 'row' to 'gridRow'
                    {
                        if (gridRow.Cells["entry_date"].Value != null && DateTime.TryParse(gridRow.Cells["entry_date"].Value.ToString(), out DateTime currentEntryDate)) // Renamed 'entryDate' to 'currentEntryDate'
                        {
                            // Apply color formatting for today's and yesterday's entries
                            if (currentEntryDate.Date == DateTime.Today)
                            {
                                gridRow.DefaultCellStyle.BackColor = Color.LightBlue;  // Today's entries
                            }
                            else if (currentEntryDate.Date == DateTime.Today.AddDays(-1))
                            {
                                gridRow.DefaultCellStyle.BackColor = Color.LightYellow; // Yesterday's entries
                            }
                            else
                            {
                                // Check if the date has changed compared to the previous row
                                if (previousEntryDate == null || currentEntryDate.Date != previousEntryDate.Value.Date)
                                {
                                    // Switch to the other color when the date changes
                                    currentColor = (currentColor == color1) ? color2 : color1;
                                }

                                gridRow.DefaultCellStyle.BackColor = currentColor; // Apply alternating color
                            }

                            previousEntryDate = currentEntryDate; // Update the last processed date
                        }
                    }


                }
                else
                {
                    // Handle missing date values
                    dataGridView.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGray;
                }
            }

            // Check and format short_extra column
            if (columnName == "short_extra")
            {
                var cellValue = dataGridView.Rows[e.RowIndex].Cells["short_extra"].Value;
                if (cellValue != null && cellValue != DBNull.Value)
                {
                    if (decimal.TryParse(cellValue.ToString(), out decimal shortExtraValue))
                    {
                        // Green if between -1 and +1 (inclusive), otherwise Red
                        if (shortExtraValue >= -1 && shortExtraValue <= 1)
                        {
                            e.CellStyle.ForeColor = Color.Green;
                            e.CellStyle.Font = new Font(dataGridView.DefaultCellStyle.Font, FontStyle.Bold);
                        }
                        else
                        {
                            e.CellStyle.ForeColor = Color.Red;
                            e.CellStyle.Font = new Font(dataGridView.DefaultCellStyle.Font, FontStyle.Bold);
                        }
                    }
                }
            }
        }   

    }
}
