using System;
using System.Windows.Forms;
using EmployeeManagementSystem.BLL;
using EmployeeManagementSystem.DAL;
using EmployeeManagementSystem.Utilities;

namespace EmployeeManagementSystem.UI.Forms
{
    public partial class SearchForm : Form
    {
        private readonly EmployeeBLL employeeBLL;
        private readonly DepartmentDAL departmentDAL;

        public SearchForm()
        {
            employeeBLL = new EmployeeBLL();
            departmentDAL = new DepartmentDAL();
            InitializeComponent();
            this.Text = "Advanced Search";
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void SearchForm_Load(object sender, EventArgs e)
        {
            cmbDepartment.DataSource = departmentDAL.GetAllDepartments();
            cmbDepartment.DisplayMember = "DepartmentName";
            cmbDepartment.ValueMember = "DepartmentID";
            cmbDepartment.SelectedIndex = -1;

            cmbStatus.Items.AddRange(new string[] { "Active", "Inactive", "OnLeave", "Terminated" });
            cmbStatus.SelectedIndex = -1;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchTerm = txtSearch.Text.Trim();
                int? deptID = cmbDepartment.SelectedIndex >= 0 ? (int?)cmbDepartment.SelectedValue : null;
                string status = cmbStatus.SelectedIndex >= 0 ? cmbStatus.SelectedItem.ToString() : null;
                string jobTitle = txtJobTitle.Text.Trim();

                var results = employeeBLL.SearchEmployees(searchTerm, deptID, jobTitle, status);
                dgvResults.DataSource = results;
                dgvResults.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                lblResultCount.Text = $"Found: {results.Count} records";
            }
            catch (Exception ex)
            {
                ValidationHelper.ShowError(ex.Message);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            txtJobTitle.Clear();
            cmbDepartment.SelectedIndex = -1;
            cmbStatus.SelectedIndex = -1;
            dgvResults.DataSource = null;
            lblResultCount.Text = "Found: 0 records";
        }

        private void btnClose_Click(object sender, EventArgs e) { this.Close(); }
    }
}