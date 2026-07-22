using System;
using System.Windows.Forms;
using EmployeeManagementSystem.BLL;
using EmployeeManagementSystem.Utilities;

namespace EmployeeManagementSystem.UI.Forms
{
    public partial class DepartmentForm : Form
    {
        private readonly string currentRole;
        private readonly DepartmentBLL departmentBLL;

        public DepartmentForm(string role)
        {
            currentRole = role;
            departmentBLL = new DepartmentBLL();
            InitializeComponent();
            this.Text = "Department Management";
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void DepartmentForm_Load(object sender, EventArgs e)
        {
            LoadDepartments();
            if (currentRole == "HR")
            {
                btnDelete.Enabled = false;
                btnDelete.Visible = false;
            }
        }

        private void LoadDepartments()
        {
            try
            {
                dgvDepartments.DataSource = departmentBLL.GetDepartmentSummary();
                dgvDepartments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError(ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidationHelper.ValidateRequiredField(txtDeptName, "Department Name")) return;

            try
            {
                var dept = new Models.Department
                {
                    DepartmentName = txtDeptName.Text.Trim(),
                    Description = txtDescription.Text.Trim()
                };

                departmentBLL.AddDepartment(dept, currentRole);
                ValidationHelper.ShowSuccess("Department added successfully!");
                LoadDepartments();
                ClearForm();
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvDepartments.SelectedRows.Count == 0) return;

            int deptID = Convert.ToInt32(dgvDepartments.SelectedRows[0].Cells["DepartmentID"].Value);
            if (ValidationHelper.ConfirmAction("Delete this department?"))
            {
                try
                {
                    departmentBLL.DeleteDepartment(deptID, currentRole);
                    ValidationHelper.ShowSuccess("Department deleted!");
                    LoadDepartments();
                }
                catch (Exception ex)
                {
                    ValidationHelper.ShowError(ex.Message);
                }
            }
        }

        private void ClearForm()
        {
            txtDeptName.Clear();
            txtDescription.Clear();
        }

        private void btnClose_Click(object sender, EventArgs e) { this.Close(); }
    }
}