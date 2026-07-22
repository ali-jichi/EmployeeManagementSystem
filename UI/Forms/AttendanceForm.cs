using System;
using System.Windows.Forms;
using EmployeeManagementSystem.BLL;
using EmployeeManagementSystem.Utilities;

namespace EmployeeManagementSystem.UI.Forms
{
    public partial class AttendanceForm : Form
    {
        private readonly string currentRole;
        private readonly AttendanceBLL attendanceBLL;
        private readonly EmployeeBLL employeeBLL;

        public AttendanceForm(string role)
        {
            currentRole = role;
            attendanceBLL = new AttendanceBLL();
            employeeBLL = new EmployeeBLL();
            InitializeComponent();
            this.Text = "Attendance Management";
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void AttendanceForm_Load(object sender, EventArgs e)
        {
            dtpAttendanceDate.Value = DateTime.Now;
            LoadDailyAttendance();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            cmbEmployee.DataSource = employeeBLL.GetAllEmployees();
            cmbEmployee.DisplayMember = "FullName";
            cmbEmployee.ValueMember = "EmployeeID";
        }

        private void LoadDailyAttendance()
        {
            try
            {
                dgvAttendance.DataSource = attendanceBLL.GetDailyAttendance(dtpAttendanceDate.Value);
                dgvAttendance.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError(ex.Message);
            }
        }

        private void btnMarkAttendance_Click(object sender, EventArgs e)
        {
            try
            {
                var attendance = new Models.Attendance
                {
                    EmployeeID = (int)cmbEmployee.SelectedValue,
                    AttendanceDate = dtpAttendanceDate.Value,
                    Status = cmbStatus.SelectedItem?.ToString(),
                    CheckInTime = dtpCheckIn.Value.TimeOfDay,
                    CheckOutTime = dtpCheckOut.Value.TimeOfDay,
                    Notes = txtNotes.Text.Trim()
                };

                attendanceBLL.AddAttendance(attendance, currentRole);
                ValidationHelper.ShowSuccess("Attendance marked successfully!");
                LoadDailyAttendance();
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError(ex.Message);
            }
        }

        private void dtpAttendanceDate_ValueChanged(object sender, EventArgs e)
        {
            LoadDailyAttendance();
        }

        private void btnClose_Click(object sender, EventArgs e) { this.Close(); }
    }
}