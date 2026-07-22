using System;
using System.Windows.Forms;
using EmployeeManagementSystem.BLL;
using EmployeeManagementSystem.Utilities;

namespace EmployeeManagementSystem.UI.Forms
{
    public partial class UserManagementForm : Form
    {
        private readonly UserBLL userBLL;

        public UserManagementForm()
        {
            userBLL = new UserBLL();
            InitializeComponent();
            this.Text = "User Management (Admin Only)";
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void UserManagementForm_Load(object sender, EventArgs e)
        {
            LoadUsers();
            cmbRole.Items.AddRange(new string[] { "Admin", "HR" });
            cmbRole.SelectedIndex = 1;
        }

        private void LoadUsers()
        {
            try
            {
                dgvUsers.DataSource = userBLL.GetAllUsers("Admin");
                dgvUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError(ex.Message);
            }
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            if (!ValidationHelper.ValidateRequiredField(txtUsername, "Username")) return;
            if (!ValidationHelper.ValidateRequiredField(txtPassword, "Password")) return;

            try
            {
                userBLL.CreateUser(txtUsername.Text.Trim(), txtPassword.Text, cmbRole.SelectedItem.ToString(), "Admin");
                ValidationHelper.ShowSuccess("User created successfully!");
                LoadUsers();
                txtUsername.Clear();
                txtPassword.Clear();
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError(ex.Message);
            }
        }

        private void btnClose_Click(object sender, EventArgs e) { this.Close(); }
    }
}