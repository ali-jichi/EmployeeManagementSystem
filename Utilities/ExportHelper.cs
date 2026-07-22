using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace EmployeeManagementSystem.Utilities
{
    /// <summary>
    /// Helper class for exporting data to various formats
    /// </summary>
    public static class ExportHelper
    {
        /// <summary>
        /// Exports DataTable to CSV file
        /// </summary>
        public static bool ExportToCSV(DataTable data, string defaultFileName)
        {
            if (data == null || data.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveDialog.FileName = defaultFileName;
                saveDialog.Title = "Export to CSV";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(saveDialog.FileName, false, System.Text.Encoding.UTF8))
                        {
                            // Write headers
                            string[] headers = new string[data.Columns.Count];
                            for (int i = 0; i < data.Columns.Count; i++)
                            {
                                headers[i] = EscapeCSV(data.Columns[i].ColumnName);
                            }
                            sw.WriteLine(string.Join(",", headers));

                            // Write data rows
                            foreach (DataRow row in data.Rows)
                            {
                                string[] fields = new string[data.Columns.Count];
                                for (int i = 0; i < data.Columns.Count; i++)
                                {
                                    fields[i] = EscapeCSV(row[i]?.ToString() ?? "");
                                }
                                sw.WriteLine(string.Join(",", fields));
                            }
                        }

                        MessageBox.Show($"Data exported successfully to:{saveDialog.FileName}",
                                      "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Export failed: {ex.Message}", "Export Error",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Exports DataGridView to CSV file
        /// </summary>
        public static bool ExportDataGridViewToCSV(DataGridView dgv, string defaultFileName)
        {
            if (dgv.Rows.Count == 0)
            {
                MessageBox.Show("No data to export.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveDialog.FileName = defaultFileName;
                saveDialog.Title = "Export to CSV";

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(saveDialog.FileName, false, System.Text.Encoding.UTF8))
                        {
                            // Write headers (excluding hidden columns)
                            var visibleColumns = new System.Collections.Generic.List<string>();
                            foreach (DataGridViewColumn col in dgv.Columns)
                            {
                                if (col.Visible)
                                    visibleColumns.Add(EscapeCSV(col.HeaderText));
                            }
                            sw.WriteLine(string.Join(",", visibleColumns));

                            // Write data rows
                            foreach (DataGridViewRow row in dgv.Rows)
                            {
                                if (!row.IsNewRow)
                                {
                                    var fields = new System.Collections.Generic.List<string>();
                                    foreach (DataGridViewColumn col in dgv.Columns)
                                    {
                                        if (col.Visible)
                                            fields.Add(EscapeCSV(row.Cells[col.Index].Value?.ToString() ?? ""));
                                    }
                                    sw.WriteLine(string.Join(",", fields));
                                }
                            }
                        }

                        MessageBox.Show($"Data exported successfully to:{saveDialog.FileName}",
                                      "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Export failed: {ex.Message}", "Export Error",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Escapes special characters for CSV format
        /// </summary>
        private static string EscapeCSV(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "";

            // If field contains special CSV characters
            if (field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
            {
                // Escape double quotes by doubling them
                field = field.Replace("\"", "\"\"");

                // Wrap in quotes
                return $"\"{field}\"";
            }

            return field;
        }
    }
}