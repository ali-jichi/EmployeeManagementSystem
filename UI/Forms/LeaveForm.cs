using System;
using System.Windows.Forms;
using EmployeeManagementSystem.BLL;
using EmployeeManagementSystem.Utilities;

namespace EmployeeManagementSystem.UI.Forms
{
    public partial class LeaveForm : Form
    {
        private readonly string currentRole;
        private readonly LeaveBLL leaveBLL;
        private readonly EmployeeBLL employeeBLL;

        public LeaveForm(string role)
        {
            currentRole = role;
            leaveBLL = new LeaveBLL();
            employeeBLL = new EmployeeBLL();
            InitializeComponent();
            this.Text = "Leave Management";
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void LeaveForm_Load(object sender, EventArgs e)
        {
            LoadLeaveRecords();
            LoadEmployees();

            if (currentRole != "Admin" && currentRole != "HR")
            {
                btnApprove.Enabled = false;
                btnReject.Enabled = false;
            }
        }

        private void LoadEmployees()
        {
            cmbEmployee.DataSource = employeeBLL.GetAllEmployees();
            cmbEmployee.DisplayMember = "FullName";
            cmbEmployee.ValueMember = "EmployeeID";
        }

        private void LoadLeaveRecords()
        {
            try
            {
                dgvLeaves.DataSource = leaveBLL.GetAllLeaveRecords();
                dgvLeaves.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError(ex.Message);
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                var leave = new Models.LeaveRecord
                {
                    EmployeeID = (int)cmbEmployee.SelectedValue,
                    LeaveType = cmbLeaveType.SelectedItem?.ToString(),
                    StartDate = dtpStartDate.Value,
                    EndDate = dtpEndDate.Value,
                    Reason = txtReason.Text.Trim()
                };

                leaveBLL.AddLeave(leave, currentRole);
                ValidationHelper.ShowSuccess("Leave applied successfully!");
                LoadLeaveRecords();
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError(ex.Message);
            }
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            ProcessLeave("Approved");
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            ProcessLeave("Rejected");
        }

        private void ProcessLeave(string status)
        {
            if (dgvLeaves.SelectedRows.Count == 0) return;

            int leaveID = Convert.ToInt32(dgvLeaves.SelectedRows[0].Cells["LeaveID"].Value);
            try
            {
                leaveBLL.ProcessLeave(leaveID, status, SessionManager.Instance.CurrentUsername, currentRole);
                ValidationHelper.ShowSuccess($"Leave {status.ToLower()}!");
                LoadLeaveRecords();
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError(ex.Message);
            }
        }

        private void btnClose_Click(object sender, EventArgs e) { this.Close(); }
    }
}