using System;
using System.Drawing;
using System.Windows.Forms;
using EmployeeManagementSystem.BLL;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Utilities;

namespace EmployeeManagementSystem.UI.Forms
{
    public partial class LoginForm : Form
    {
        private readonly UserBLL userBLL;

        public User CurrentUser { get; private set; }
        public string CurrentRole => CurrentUser?.Role;

        public LoginForm()
        {
            InitializeComponent();
            userBLL = new UserBLL();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text;

                // Validation
                if (string.IsNullOrWhiteSpace(username))
                {
                    ValidationHelper.ShowError("Please enter username.");
                    txtUsername.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    ValidationHelper.ShowError("Please enter password.");
                    txtPassword.Focus();
                    return;
                }

                // Attempt authentication
                CurrentUser = userBLL.AuthenticateUser(username, password);

                if (CurrentUser != null)
                {
                    // Start session
                    SessionManager.Instance.StartSession(CurrentUser);

                    ValidationHelper.ShowSuccess($"Welcome, {CurrentUser.Username}!\nRole: {CurrentUser.Role}");
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    ValidationHelper.ShowError("Invalid username or password.\n\nPlease try again.");
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError($"Login error: {ex.Message}");
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (ValidationHelper.ConfirmAction("Are you sure you want to exit?"))
            {
                Application.Exit();
            }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        private void lblForgotPassword_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Please contact your system administrator to reset your password.", 
                          "Password Reset", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}