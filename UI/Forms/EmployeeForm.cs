using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EmployeeManagementSystem.BLL;
using EmployeeManagementSystem.DAL;
using EmployeeManagementSystem.Models;
using EmployeeManagementSystem.Utilities;

namespace EmployeeManagementSystem.UI.Forms
{
    public partial class EmployeeForm : Form
    {
        private readonly EmployeeBLL employeeBLL;
        private readonly DepartmentDAL departmentDAL;
        private readonly string currentRole;
        private int selectedEmployeeID = 0;

        public EmployeeForm(string role)
        {
            InitializeComponent();
            currentRole = role;
            employeeBLL = new EmployeeBLL();
            departmentDAL = new DepartmentDAL();

            this.Text = "Employee Management";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
        }

        private void EmployeeForm_Load(object sender, EventArgs e)
        {
            LoadDepartments();
            LoadEmployees();
            ConfigureButtons();
            ClearForm();
        }

        private void ConfigureButtons()
        {
            if (currentRole == "HR")
            {
                btnDelete.Enabled = false;
                btnDelete.Visible = false;
                lblMode.Text = "HR Mode - Edit Only";
                lblMode.ForeColor = Color.Orange;
            }
            else
            {
                lblMode.Text = "Admin Mode - Full Control";
                lblMode.ForeColor = Color.Green;
            }
        }

        private void LoadDepartments()
        {
            try
            {
                List<Department> departments = departmentDAL.GetAllDepartments();
                cmbDepartment.DataSource = departments;
                cmbDepartment.DisplayMember = "DepartmentName";
                cmbDepartment.ValueMember = "DepartmentID";
                cmbDepartment.SelectedIndex = -1;
                cmbSearchDepartment.DataSource = new List<Department>(departments);
                cmbSearchDepartment.DisplayMember = "DepartmentName";
                cmbSearchDepartment.ValueMember = "DepartmentID";
                cmbSearchDepartment.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError($"Error loading departments: {ex.Message}");
            }
        }

        private void LoadEmployees()
        {
            try
            {
                List<Employee> employees = employeeBLL.GetAllEmployees();
                dgvEmployees.DataSource = employees;

                // Format grid
                if (dgvEmployees.Columns.Count > 0)
                {
                    dgvEmployees.Columns["EmployeeID"].HeaderText = "ID";
                    dgvEmployees.Columns["FullName"].HeaderText = "Name";
                    dgvEmployees.Columns["DepartmentName"].HeaderText = "Department";
                    dgvEmployees.Columns["JobTitle"].HeaderText = "Job Title";
                    dgvEmployees.Columns["DateOfJoining"].HeaderText = "Joining Date";
                    dgvEmployees.Columns["Salary"].HeaderText = "Salary";
                    dgvEmployees.Columns["Salary"].DefaultCellStyle.Format = "C2";
                    dgvEmployees.Columns["EmploymentStatus"].HeaderText = "Status";

                    // Hide unnecessary columns
                    dgvEmployees.Columns["FirstName"].Visible = false;
                    dgvEmployees.Columns["LastName"].Visible = false;
                    dgvEmployees.Columns["Address"].Visible = false;
                    dgvEmployees.Columns["Phone"].Visible = false;
                    dgvEmployees.Columns["Email"].Visible = false;
                    dgvEmployees.Columns["DateOfBirth"].Visible = false;
                    dgvEmployees.Columns["Gender"].Visible = false;
                    dgvEmployees.Columns["DepartmentID"].Visible = false;
                    dgvEmployees.Columns["EmergencyContact"].Visible = false;
                    dgvEmployees.Columns["EmergencyPhone"].Visible = false;
                    dgvEmployees.Columns["CreatedDate"].Visible = false;
                    dgvEmployees.Columns["ModifiedDate"].Visible = false;
                    dgvEmployees.Columns["YearsOfService"].Visible = false;
                }

                dgvEmployees.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                lblRecordCount.Text = $"Total Records: {employees.Count}";
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError($"Error loading employees: {ex.Message}");
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                Employee employee = GetEmployeeFromForm();
                int newID = employeeBLL.AddEmployee(employee, currentRole);

                ValidationHelper.ShowSuccess($"Employee added successfully! ID: {newID}");
                ClearForm();
                LoadEmployees();
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError($"Error adding employee: {ex.Message}");
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedEmployeeID == 0)
            {
                ValidationHelper.ShowError("Please select an employee to update.");
                return;
            }

            if (!ValidateForm()) return;

            try
            {
                Employee employee = GetEmployeeFromForm();
                employee.EmployeeID = selectedEmployeeID;

                if (employeeBLL.UpdateEmployee(employee, currentRole))
                {
                    ValidationHelper.ShowSuccess("Employee updated successfully!");
                    ClearForm();
                    LoadEmployees();
                }
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError($"Error updating employee: {ex.Message}");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedEmployeeID == 0)
            {
                ValidationHelper.ShowError("Please select an employee to delete.");
                return;
            }

            if (ValidationHelper.ConfirmAction($"Are you sure you want to delete Employee ID: {selectedEmployeeID}?\n\nThis action cannot be undone."))
            {
                try
                {
                    if (employeeBLL.DeleteEmployee(selectedEmployeeID, currentRole))
                    {
                        ValidationHelper.ShowSuccess("Employee deleted successfully!");
                        ClearForm();
                        LoadEmployees();
                    }
                }
                catch (Exception ex)
                {
                    ValidationHelper.ShowError($"Error deleting employee: {ex.Message}");
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearForm();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = txtSearch.Text.Trim();
                int? deptID = cmbSearchDepartment.SelectedIndex >= 0 ? (int?)cmbSearchDepartment.SelectedValue : null;

                List<Employee> results = employeeBLL.SearchEmployees(searchTerm, deptID);
                dgvEmployees.DataSource = results;
                lblRecordCount.Text = $"Found: {results.Count} records";
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError($"Search error: {ex.Message}");
            }
        }

        private void dgvEmployees_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvEmployees.Rows[e.RowIndex];
                selectedEmployeeID = Convert.ToInt32(row.Cells["EmployeeID"].Value);

                txtFirstName.Text = row.Cells["FirstName"].Value?.ToString();
                txtLastName.Text = row.Cells["LastName"].Value?.ToString();
                txtAddress.Text = row.Cells["Address"].Value?.ToString();
                txtPhone.Text = row.Cells["Phone"].Value?.ToString();
                txtEmail.Text = row.Cells["Email"].Value?.ToString();
                txtJobTitle.Text = row.Cells["JobTitle"].Value?.ToString();
                txtSalary.Text = row.Cells["Salary"].Value?.ToString();
                txtEmergencyContact.Text = row.Cells["EmergencyContact"].Value?.ToString();
                txtEmergencyPhone.Text = row.Cells["EmergencyPhone"].Value?.ToString();

                if (row.Cells["DepartmentID"].Value != null && row.Cells["DepartmentID"].Value != DBNull.Value)
                    cmbDepartment.SelectedValue = Convert.ToInt32(row.Cells["DepartmentID"].Value);
                else
                    cmbDepartment.SelectedIndex = -1;

                if (row.Cells["DateOfJoining"].Value != null && row.Cells["DateOfJoining"].Value != DBNull.Value)
                    dtpJoiningDate.Value = Convert.ToDateTime(row.Cells["DateOfJoining"].Value);

                if (row.Cells["DateOfBirth"].Value != null && row.Cells["DateOfBirth"].Value != DBNull.Value)
                    dtpDateOfBirth.Value = Convert.ToDateTime(row.Cells["DateOfBirth"].Value);

                cmbGender.Text = row.Cells["Gender"].Value?.ToString();
                cmbStatus.Text = row.Cells["EmploymentStatus"].Value?.ToString();

                lblSelectedID.Text = $"Selected ID: {selectedEmployeeID}";
            }
        }

        private Employee GetEmployeeFromForm()
        {
            return new Employee
            {
                FirstName = txtFirstName.Text.Trim(),
                LastName = txtLastName.Text.Trim(),
                DateOfBirth = dtpDateOfBirth.Value > dtpDateOfBirth.MinDate ? dtpDateOfBirth.Value : (DateTime?)null,
                Gender = cmbGender.SelectedItem?.ToString(),
                Address = txtAddress.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                DepartmentID = cmbDepartment.SelectedIndex >= 0 ? (int?)cmbDepartment.SelectedValue : null,
                JobTitle = txtJobTitle.Text.Trim(),
                DateOfJoining = dtpJoiningDate.Value,
                Salary = decimal.Parse(txtSalary.Text),
                EmploymentStatus = cmbStatus.SelectedItem?.ToString() ?? "Active",
                EmergencyContact = txtEmergencyContact.Text.Trim(),
                EmergencyPhone = txtEmergencyPhone.Text.Trim()
            };
        }

        private bool ValidateForm()
        {
            if (!ValidationHelper.ValidateRequiredField(txtFirstName, "First Name")) return false;
            if (!ValidationHelper.ValidateRequiredField(txtLastName, "Last Name")) return false;
            if (!ValidationHelper.ValidateRequiredField(txtJobTitle, "Job Title")) return false;

            if (!string.IsNullOrEmpty(txtEmail.Text) && !ValidationHelper.ValidateEmail(txtEmail.Text))
            {
                ValidationHelper.ShowError("Invalid email format.");
                txtEmail.Focus();
                return false;
            }

            if (!string.IsNullOrEmpty(txtPhone.Text) && !ValidationHelper.ValidatePhone(txtPhone.Text))
            {
                ValidationHelper.ShowError("Invalid phone format.");
                txtPhone.Focus();
                return false;
            }

            if (!decimal.TryParse(txtSalary.Text, out decimal salary) || salary < 0)
            {
                ValidationHelper.ShowError("Salary must be a valid positive number.");
                txtSalary.Focus();
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            selectedEmployeeID = 0;
            txtFirstName.Clear();
            txtLastName.Clear();
            txtAddress.Clear();
            txtPhone.Clear();
            txtEmail.Clear();
            txtJobTitle.Clear();
            txtSalary.Clear();
            txtEmergencyContact.Clear();
            txtEmergencyPhone.Clear();
            txtSearch.Clear();

            cmbDepartment.SelectedIndex = -1;
            cmbGender.SelectedIndex = -1;
            cmbStatus.SelectedIndex = 0;
            cmbSearchDepartment.SelectedIndex = -1;

            dtpJoiningDate.Value = DateTime.Now;
            dtpDateOfBirth.Value = dtpDateOfBirth.MinDate;

            lblSelectedID.Text = "Selected ID: None";
            dgvEmployees.ClearSelection();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbSearchDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}