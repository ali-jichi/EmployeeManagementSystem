using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace EmployeeManagementSystem.Utilities
{
    /// <summary>
    /// Helper class for common validations
    /// </summary>
    public static class ValidationHelper
    {
        public static bool ValidateRequiredField(Control control, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(control.Text))
            {
                MessageBox.Show($"{fieldName} is required.", "Validation Error", 
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                control.Focus();
                return false;
            }
            return true;
        }

        public static bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return true; // Optional

            string pattern = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
            return Regex.IsMatch(email, pattern);
        }

        public static bool ValidatePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return true;

            string pattern = @"^[\d\s\-\+\(\)]{7,25}$";
            return Regex.IsMatch(phone, pattern);
        }

        public static bool ValidateDecimal(string value, string fieldName, out decimal result)
        {
            result = 0;
            if (string.IsNullOrWhiteSpace(value))
            {
                MessageBox.Show($"{fieldName} is required.", "Validation Error", 
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!decimal.TryParse(value, out result) || result < 0)
            {
                MessageBox.Show($"{fieldName} must be a valid positive number.", "Validation Error", 
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        public static bool ValidateDateRange(DateTime startDate, DateTime endDate, string rangeName)
        {
            if (startDate > endDate)
            {
                MessageBox.Show($"{rangeName}: Start date cannot be after end date.", "Validation Error", 
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        public static bool ConfirmAction(string message, string title = "Confirm")
        {
            return MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        public static void ShowError(string message, string title = "Error")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void ShowSuccess(string message, string title = "Success")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}