using System.Windows.Forms;

namespace NaarNoor.Desktop.WinForms.Forms
{
    partial class LoginForm : Form
    {
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.lblUsername = new Label();
            this.txtUsername = new TextBox();
            this.lblPassword = new Label();
            this.txtPassword = new TextBox();
            this.btnLogin = new Button();
            this.lblError = new Label();
            this.progressSpinner = new ProgressBar();
            
            this.SuspendLayout();
            
            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.lblTitle.Location = new Point(12, 20);
            this.lblTitle.Text = "Naar-Noor Login";
            
            // lblUsername
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new Point(12, 60);
            this.lblUsername.Text = "Username:";
            
            // txtUsername
            this.txtUsername.Location = new Point(12, 80);
            this.txtUsername.Size = new Size(250, 23);
            
            // lblPassword
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new Point(12, 110);
            this.lblPassword.Text = "Password:";
            
            // txtPassword
            this.txtPassword.Location = new Point(12, 130);
            this.txtPassword.Size = new Size(250, 23);
            this.txtPassword.PasswordChar = '*';
            
            // btnLogin
            this.btnLogin.Location = new Point(12, 170);
            this.btnLogin.Size = new Size(250, 40);
            this.btnLogin.Text = "Login";
            this.btnLogin.BackColor = Color.FromArgb(0, 120, 212);
            this.btnLogin.ForeColor = Color.White;
            
            // lblError
            this.lblError.AutoSize = true;
            this.lblError.ForeColor = Color.Red;
            this.lblError.Location = new Point(12, 220);
            this.lblError.Visible = false;
            
            // progressSpinner
            this.progressSpinner.Location = new Point(135, 200);
            this.progressSpinner.Size = new Size(20, 20);
            this.progressSpinner.Style = ProgressBarStyle.Marquee;
            this.progressSpinner.Visible = false;
            
            // Add controls to form
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.progressSpinner);
            
            // LoginForm
            this.ClientSize = new Size(280, 270);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Naar-Noor - Login";
            
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        public Label lblTitle;
        public Label lblUsername;
        public TextBox txtUsername;
        public Label lblPassword;
        public TextBox txtPassword;
        public Button btnLogin;
        public Label lblError;
        public ProgressBar progressSpinner;
    }
}
