using System.Windows.Forms;

namespace NaarNoor.Desktop.WinForms.Forms
{
    partial class DashboardForm : Form
    {
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            var lblTitle = new Label();
            var btnLogout = new Button();
            var pnlMetrics = new Panel();
            var lblTodayRevenue = new Label();
            var lblTotalOrders = new Label();
            var lblActiveReservations = new Label();
            var tabControl = new TabControl();
            var tabReservations = new TabPage();
            var tabMenu = new TabPage();
            var tabStaff = new TabPage();
            var tabReports = new TabPage();
            var btnRefresh = new Button();
            var lblCurrentSection = new Label();
            var lblLoading = new Label();
            var lblError = new Label();
            var lblCompletedOrders = new Label();
            var pnlTopBar = new Panel();
            var pnlNavigation = new Panel();
            var lblRevenueLabel = new Label();
            var lblOrdersLabel = new Label();
            var lblCompletedLabel = new Label();
            var lblReservationsLabel = new Label();
            var lblUserInfo = new Label();
            
            pnlMetrics.SuspendLayout();
            pnlTopBar.SuspendLayout();
            pnlNavigation.SuspendLayout();
            tabControl.SuspendLayout();
            this.SuspendLayout();

            // Top bar
            pnlTopBar.BackColor = Color.FromArgb(0, 120, 212);
            pnlTopBar.Controls.Add(lblTitle);
            pnlTopBar.Controls.Add(lblUserInfo);
            pnlTopBar.Controls.Add(btnLogout);
            pnlTopBar.Dock = DockStyle.Top;
            pnlTopBar.Height = 60;
            
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(12, 15);
            lblTitle.Text = "Naar-Noor";
            
            lblUserInfo.AutoSize = true;
            lblUserInfo.ForeColor = Color.White;
            lblUserInfo.Location = new Point(400, 22);
            lblUserInfo.Text = "Logged in";
            
            btnLogout.BackColor = Color.FromArgb(210, 50, 50);
            btnLogout.ForeColor = Color.White;
            btnLogout.Location = new Point(650, 15);
            btnLogout.Size = new Size(80, 30);
            btnLogout.Text = "Logout";

            // Metrics panel
            pnlMetrics.BackColor = Color.FromArgb(240, 240, 240);
            pnlMetrics.Dock = DockStyle.Top;
            pnlMetrics.Height = 100;
            pnlMetrics.Controls.Add(lblRevenueLabel);
            pnlMetrics.Controls.Add(lblTodayRevenue);
            pnlMetrics.Controls.Add(lblOrdersLabel);
            pnlMetrics.Controls.Add(lblTotalOrders);
            pnlMetrics.Controls.Add(lblCompletedLabel);
            pnlMetrics.Controls.Add(lblCompletedOrders);
            pnlMetrics.Controls.Add(lblReservationsLabel);
            pnlMetrics.Controls.Add(lblActiveReservations);
            
            lblRevenueLabel.Location = new Point(20, 20);
            lblRevenueLabel.Text = "Revenue Today:";
            lblTodayRevenue.Location = new Point(150, 20);
            lblTodayRevenue.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTodayRevenue.Text = "$0.00";
            
            lblOrdersLabel.Location = new Point(300, 20);
            lblOrdersLabel.Text = "Total Orders:";
            lblTotalOrders.Location = new Point(420, 20);
            lblTotalOrders.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTotalOrders.Text = "0";
            
            lblCompletedLabel.Location = new Point(500, 20);
            lblCompletedLabel.Text = "Completed:";
            lblCompletedOrders.Location = new Point(600, 20);
            lblCompletedOrders.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblCompletedOrders.Text = "0";
            
            lblReservationsLabel.Location = new Point(700, 20);
            lblReservationsLabel.Text = "Active Reservations:";
            lblActiveReservations.Location = new Point(850, 20);
            lblActiveReservations.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblActiveReservations.Text = "0";

            // Navigation
            pnlNavigation.BackColor = Color.FromArgb(50, 50, 50);
            pnlNavigation.Dock = DockStyle.Top;
            pnlNavigation.Height = 50;
            pnlNavigation.Controls.Add(lblCurrentSection);
            pnlNavigation.Controls.Add(btnRefresh);
            
            lblCurrentSection.AutoSize = true;
            lblCurrentSection.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblCurrentSection.ForeColor = Color.White;
            lblCurrentSection.Location = new Point(12, 15);
            lblCurrentSection.Text = "Dashboard";
            
            btnRefresh.BackColor = Color.FromArgb(0, 120, 212);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.Location = new Point(750, 10);
            btnRefresh.Size = new Size(80, 30);
            btnRefresh.Text = "Refresh";

            // Tabs
            tabControl.Dock = DockStyle.Fill;
            tabControl.Controls.Add(tabReservations);
            tabControl.Controls.Add(tabMenu);
            tabControl.Controls.Add(tabStaff);
            tabControl.Controls.Add(tabReports);
            
            tabReservations.Text = "Reservations";
            tabMenu.Text = "Menu";
            tabStaff.Text = "Staff";
            tabReports.Text = "Reports";

            lblLoading.ForeColor = Color.FromArgb(0, 120, 212);
            lblLoading.Location = new Point(12, 12);
            lblLoading.Text = "Loading...";
            lblLoading.Visible = false;
            
            lblError.ForeColor = Color.Red;
            lblError.Location = new Point(12, 30);
            lblError.Visible = false;

            this.Controls.Add(tabControl);
            this.Controls.Add(lblError);
            this.Controls.Add(lblLoading);
            this.Controls.Add(pnlNavigation);
            this.Controls.Add(pnlMetrics);
            this.Controls.Add(pnlTopBar);

            this.ClientSize = new Size(1024, 660);
            this.Text = "Naar-Noor - Dashboard";
            this.StartPosition = FormStartPosition.CenterScreen;

            pnlMetrics.ResumeLayout(false);
            pnlTopBar.ResumeLayout(false);
            pnlNavigation.ResumeLayout(false);
            tabControl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
            
            // Store controls as instance fields
            this.lblTitle = lblTitle;
            this.btnLogout = btnLogout;
            this.pnlMetrics = pnlMetrics;
            this.lblTodayRevenue = lblTodayRevenue;
            this.lblTotalOrders = lblTotalOrders;
            this.lblActiveReservations = lblActiveReservations;
            this.tabControl = tabControl;
            this.tabReservations = tabReservations;
            this.tabMenu = tabMenu;
            this.tabStaff = tabStaff;
            this.tabReports = tabReports;
            this.btnRefresh = btnRefresh;
            this.lblCurrentSection = lblCurrentSection;
            this.lblLoading = lblLoading;
            this.lblError = lblError;
            this.lblCompletedOrders = lblCompletedOrders;
            this.pnlTopBar = pnlTopBar;
            this.pnlNavigation = pnlNavigation;
            this.lblRevenueLabel = lblRevenueLabel;
            this.lblOrdersLabel = lblOrdersLabel;
            this.lblCompletedLabel = lblCompletedLabel;
            this.lblReservationsLabel = lblReservationsLabel;
            this.lblUserInfo = lblUserInfo;
        }

        #endregion

        public Label lblTitle;
        public Button btnLogout;
        public Panel pnlMetrics;
        public Label lblTodayRevenue;
        public Label lblTotalOrders;
        public Label lblActiveReservations;
        public TabControl tabControl;
        public TabPage tabReservations;
        public TabPage tabMenu;
        public TabPage tabStaff;
        public TabPage tabReports;
        public Button btnRefresh;
        public Label lblCurrentSection;
        public Label lblLoading;
        public Label lblError;
        public Label lblCompletedOrders;
        public Panel pnlTopBar;
        public Panel pnlNavigation;
        public Label lblRevenueLabel;
        public Label lblOrdersLabel;
        public Label lblCompletedLabel;
        public Label lblReservationsLabel;
        public Label lblUserInfo;
    }
}
