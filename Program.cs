using System;
using System.Windows.Forms;
using EmployeeManagementSystem.UI.Forms;

namespace EmployeeManagementSystem
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ApplicationExit += OnApplicationExit;

            LoginForm loginForm = new LoginForm();
            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                MainForm mainForm = new MainForm(loginForm.CurrentUser.Username, loginForm.CurrentUser.Role);
                Application.Run(mainForm);
            }
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Application exited successfully.");
        }
    }

}