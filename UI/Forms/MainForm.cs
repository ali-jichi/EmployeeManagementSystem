using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using EmployeeManagementSystem.BLL;
using EmployeeManagementSystem.Utilities;

namespace EmployeeManagementSystem.UI.Forms
{
    public partial class MainForm : Form
    {
        private readonly string currentUser;
        private readonly string currentRole;
        private readonly ReportBLL reportBLL;

        public MainForm(string user, string role)
        {
            InitializeComponent();
            currentUser = user;
            currentRole = role;
            reportBLL = new ReportBLL();

            this.Text = $"Employee Management System - {currentUser} ({currentRole})";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.FormClosing += MainForm_FormClosing;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            lblWelcome.Text = $"Welcome, {currentUser}";
            lblRole.Text = $"Role: {currentRole}";
            lblDate.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy");

            ConfigureRoleBasedAccess();
            LoadDashboardStats();
        }

        private void ConfigureRoleBasedAccess()
        {
            if (currentRole == "HR")
            {
                btnUsers.Enabled = false;
                btnUsers.Visible = false;
                lblRestriction.Text = "HR Mode: Limited Access";
                lblRestriction.ForeColor = Color.Orange;
            }
            else if (currentRole == "Admin")
            {
                lblRestriction.Text = "Admin Mode: Full Access";
                lblRestriction.ForeColor = Color.Green;
            }
        }

        private void LoadDashboardStats()
        {
            try
            {
                DataTable stats = reportBLL.GetDashboardStatistics();
                if (stats.Rows.Count > 0)
                {
                    DataRow row = stats.Rows[0];
                    lblTotalEmployees.Text = $"Total Employees: {row["TotalEmployees"]}";
                    lblTotalDepartments.Text = $"Departments: {row["TotalDepartments"]}";
                    lblPresentToday.Text = $"Present Today: {row["PresentToday"]}";
                    lblPendingLeaves.Text = $"Pending Leaves: {row["PendingLeaves"]}";

                    decimal avgSalary = row["AverageSalary"] != DBNull.Value ? Convert.ToDecimal(row["AverageSalary"]) : 0;
                    decimal totalPayroll = row["TotalPayroll"] != DBNull.Value ? Convert.ToDecimal(row["TotalPayroll"]) : 0;

                    lblAvgSalary.Text = $"Avg Salary: {avgSalary:C0}";
                    lblTotalPayroll.Text = $"Total Payroll: {totalPayroll:C0}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading dashboard stats: {ex.Message}");
            }
        }

        private void btnEmployees_Click(object sender, EventArgs e)
        {
            EmployeeForm empForm = new EmployeeForm(currentRole);
            empForm.ShowDialog();
            LoadDashboardStats();
        }

        private void btnDepartments_Click(object sender, EventArgs e)
        {
            DepartmentForm deptForm = new DepartmentForm(currentRole);
            deptForm.ShowDialog();
            LoadDashboardStats();
        }

        private void btnAttendance_Click(object sender, EventArgs e)
        {
            AttendanceForm attForm = new AttendanceForm(currentRole);
            attForm.ShowDialog();
            LoadDashboardStats();
        }

        private void btnLeaves_Click(object sender, EventArgs e)
        {
            LeaveForm leaveForm = new LeaveForm(currentRole);
            leaveForm.ShowDialog();
            LoadDashboardStats();
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            ReportsForm repForm = new ReportsForm(currentRole);
            repForm.ShowDialog();
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            if (currentRole == "Admin")
            {
                UserManagementForm userForm = new UserManagementForm();
                userForm.ShowDialog();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchForm searchForm = new SearchForm();
            searchForm.ShowDialog();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadDashboardStats();
            ValidationHelper.ShowSuccess("Dashboard refreshed successfully!");
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (ValidationHelper.ConfirmAction("Are you sure you want to logout?"))
            {
                SessionManager.Instance.EndSession();
                this.Hide();

                LoginForm loginForm = new LoginForm();
                if (loginForm.ShowDialog() == DialogResult.OK)
                {
                    MainForm newMain = new MainForm(loginForm.CurrentUser.Username, loginForm.CurrentUser.Role);
                    newMain.Show();
                }
                else
                {
                    Application.Exit();
                }
                this.Close();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (!ValidationHelper.ConfirmAction("Are you sure you want to exit the application?"))
                {
                    e.Cancel = true;
                }
                else
                {
                    SessionManager.Instance.EndSession();
                }
            }
        }

        private void timerClock_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToString("HH:mm:ss");
        }
    }
}