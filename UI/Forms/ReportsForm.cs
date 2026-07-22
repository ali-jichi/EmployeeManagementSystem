using System;
using System.Data;
using System.Windows.Forms;
using EmployeeManagementSystem.BLL;
using EmployeeManagementSystem.DAL;
using EmployeeManagementSystem.Utilities;

namespace EmployeeManagementSystem.UI.Forms
{
    public partial class ReportsForm : Form
    {
        private readonly string currentRole;
        private readonly ReportBLL reportBLL;
        private readonly DepartmentDAL departmentDAL;

        public ReportsForm(string role)
        {
            currentRole = role;
            reportBLL = new ReportBLL();
            departmentDAL = new DepartmentDAL();
            InitializeComponent();
            this.Text = "Reports & Analytics";
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void ReportsForm_Load(object sender, EventArgs e)
        {
            LoadDepartments();
            dtpStartDate.Value = DateTime.Now.AddMonths(-1);
            dtpEndDate.Value = DateTime.Now;
        }

        private void LoadDepartments()
        {
            cmbDepartment.DataSource = departmentDAL.GetAllDepartments();
            cmbDepartment.DisplayMember = "DepartmentName";
            cmbDepartment.ValueMember = "DepartmentID";
            cmbDepartment.SelectedIndex = -1;
        }

        private void btnEmployeeReport_Click(object sender, EventArgs e)
        {
            try
            {
                int? deptID = cmbDepartment.SelectedIndex >= 0 ? (int?)cmbDepartment.SelectedValue : null;
                DataTable dt = reportBLL.GenerateEmployeeListReport(deptID);
                dgvReports.DataSource = dt;
                lblReportTitle.Text = "Employee List Report";
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError(ex.Message);
            }
        }

        private void btnDepartmentReport_Click(object sender, EventArgs e)
        {
            try
            {
                int? deptID = cmbDepartment.SelectedIndex >= 0 ? (int?)cmbDepartment.SelectedValue : null;
                DataTable dt = reportBLL.GenerateDepartmentReport(deptID);
                dgvReports.DataSource = dt;
                lblReportTitle.Text = "Department Report";
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError(ex.Message);
            }
        }

        private void btnAttendanceReport_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable dt = reportBLL.GenerateAttendanceReport(dtpStartDate.Value, dtpEndDate.Value);
                dgvReports.DataSource = dt;
                lblReportTitle.Text = $"Attendance Report ({dtpStartDate.Value.ToShortDateString()} - {dtpEndDate.Value.ToShortDateString()})";
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError(ex.Message);
            }
        }

        private void btnSalaryReport_Click(object sender, EventArgs e)
        {
            try
            {
                int? deptID = cmbDepartment.SelectedIndex >= 0 ? (int?)cmbDepartment.SelectedValue : null;
                DataTable dt = reportBLL.GenerateSalaryReport(deptID);
                dgvReports.DataSource = dt;
                lblReportTitle.Text = "Salary Report";
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError(ex.Message);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (dgvReports.DataSource is DataTable dt)
            {
                string fileName = lblReportTitle.Text.Replace(" ", "_").Replace("/", "-") + "_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
                ExportHelper.ExportToCSV(dt, fileName);
            }
        }

        private void btnClose_Click(object sender, EventArgs e) { this.Close(); }
    }
}