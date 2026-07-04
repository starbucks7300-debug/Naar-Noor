using System.Windows.Forms;

namespace NaarNoor.Desktop.WinForms.Forms
{
    partial class StaffForm : Form
    {
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.txtSearch = new TextBox();
            this.btnSearch = new Button();
            this.dgvStaff = new DataGridView();
            this.ddlStatusFilter = new ComboBox();
            this.lblStatus = new Label();
            this.btnRefresh = new Button();
            this.pnlDetails = new Panel();
            this.lblScheduleTitle = new Label();
            this.lblScheduleInfo = new Label();
            this.lblShiftTitle = new Label();
            this.lblShiftInfo = new Label();
            this.ddlStaffStatus = new ComboBox();
            this.btnUpdateStatus = new Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStaff)).BeginInit();
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Arial", 14F, FontStyle.Bold);
            this.lblTitle.Location = new Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(102, 22);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Staff Members";

            // txtSearch
            this.txtSearch.Location = new Point(12, 35);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.PlaceholderText = "Search by name...";
            this.txtSearch.Size = new Size(200, 23);
            this.txtSearch.TabIndex = 1;

            // btnSearch
            this.btnSearch.Location = new Point(220, 35);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new Size(75, 23);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;

            // lblStatus
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new Point(305, 37);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new Size(42, 15);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Status:";

            // ddlStatusFilter
            this.ddlStatusFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            this.ddlStatusFilter.Location = new Point(355, 35);
            this.ddlStatusFilter.Name = "ddlStatusFilter";
            this.ddlStatusFilter.Size = new Size(120, 23);
            this.ddlStatusFilter.TabIndex = 4;
            this.ddlStatusFilter.Items.AddRange(new object[] { "All", "Available", "Busy", "Break", "Off Duty" });

            // btnRefresh
            this.btnRefresh.Location = new Point(485, 35);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new Size(75, 23);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;

            // dgvStaff
            this.dgvStaff.AllowUserToAddRows = false;
            this.dgvStaff.AllowUserToDeleteRows = false;
            this.dgvStaff.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStaff.Location = new Point(12, 70);
            this.dgvStaff.Name = "dgvStaff";
            this.dgvStaff.ReadOnly = true;
            this.dgvStaff.RowTemplate.Height = 25;
            this.dgvStaff.Size = new Size(560, 300);
            this.dgvStaff.TabIndex = 6;

            // pnlDetails
            this.pnlDetails.BackColor = SystemColors.Window;
            this.pnlDetails.BorderStyle = BorderStyle.FixedSingle;
            this.pnlDetails.Location = new Point(580, 70);
            this.pnlDetails.Name = "pnlDetails";
            this.pnlDetails.Size = new Size(220, 300);
            this.pnlDetails.TabIndex = 7;

            // lblScheduleTitle
            this.lblScheduleTitle.AutoSize = true;
            this.lblScheduleTitle.Font = new Font("Arial", 10F, FontStyle.Bold);
            this.lblScheduleTitle.Location = new Point(10, 10);
            this.lblScheduleTitle.Name = "lblScheduleTitle";
            this.lblScheduleTitle.Size = new Size(107, 16);
            this.lblScheduleTitle.TabIndex = 0;
            this.lblScheduleTitle.Text = "Scheduled Hours";
            this.pnlDetails.Controls.Add(this.lblScheduleTitle);

            // lblScheduleInfo
            this.lblScheduleInfo.AutoSize = true;
            this.lblScheduleInfo.Location = new Point(10, 35);
            this.lblScheduleInfo.Name = "lblScheduleInfo";
            this.lblScheduleInfo.Size = new Size(51, 15);
            this.lblScheduleInfo.TabIndex = 1;
            this.lblScheduleInfo.Text = "9:00 - 17:00";
            this.pnlDetails.Controls.Add(this.lblScheduleInfo);

            // lblShiftTitle
            this.lblShiftTitle.AutoSize = true;
            this.lblShiftTitle.Font = new Font("Arial", 10F, FontStyle.Bold);
            this.lblShiftTitle.Location = new Point(10, 65);
            this.lblShiftTitle.Name = "lblShiftTitle";
            this.lblShiftTitle.Size = new Size(82, 16);
            this.lblShiftTitle.TabIndex = 2;
            this.lblShiftTitle.Text = "Current Shift";
            this.pnlDetails.Controls.Add(this.lblShiftTitle);

            // lblShiftInfo
            this.lblShiftInfo.AutoSize = true;
            this.lblShiftInfo.Location = new Point(10, 90);
            this.lblShiftInfo.Name = "lblShiftInfo";
            this.lblShiftInfo.Size = new Size(77, 15);
            this.lblShiftInfo.TabIndex = 3;
            this.lblShiftInfo.Text = "Morning Shift";
            this.pnlDetails.Controls.Add(this.lblShiftInfo);

            // lblStatusTitle (added to panel)
            var lblStatusTitle = new Label
            {
                AutoSize = true,
                Font = new Font("Arial", 10F, FontStyle.Bold),
                Location = new Point(10, 120),
                Name = "lblStatusTitle",
                Size = new Size(50, 16),
                TabIndex = 4,
                Text = "Status:"
            };
            this.pnlDetails.Controls.Add(lblStatusTitle);

            // ddlStaffStatus
            this.ddlStaffStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.ddlStaffStatus.Location = new Point(10, 145);
            this.ddlStaffStatus.Name = "ddlStaffStatus";
            this.ddlStaffStatus.Size = new Size(190, 23);
            this.ddlStaffStatus.TabIndex = 5;
            this.ddlStaffStatus.Items.AddRange(new object[] { "Available", "Busy", "Break", "Off Duty" });
            this.pnlDetails.Controls.Add(this.ddlStaffStatus);

            // btnUpdateStatus
            this.btnUpdateStatus.Location = new Point(10, 180);
            this.btnUpdateStatus.Name = "btnUpdateStatus";
            this.btnUpdateStatus.Size = new Size(190, 23);
            this.btnUpdateStatus.TabIndex = 6;
            this.btnUpdateStatus.Text = "Update Status";
            this.btnUpdateStatus.UseVisualStyleBackColor = true;
            this.pnlDetails.Controls.Add(this.btnUpdateStatus);

            // StaffForm
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(820, 400);
            this.Controls.Add(this.pnlDetails);
            this.Controls.Add(this.dgvStaff);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.ddlStatusFilter);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.lblTitle);
            this.Name = "StaffForm";
            this.Text = "Staff Management";
            ((System.ComponentModel.ISupportInitialize)(this.dgvStaff)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private TextBox txtSearch;
        private Button btnSearch;
        private DataGridView dgvStaff;
        private ComboBox ddlStatusFilter;
        private Label lblStatus;
        private Button btnRefresh;
        private Panel pnlDetails;
        private Label lblScheduleTitle;
        private Label lblScheduleInfo;
        private Label lblShiftTitle;
        private Label lblShiftInfo;
        private ComboBox ddlStaffStatus;
        private Button btnUpdateStatus;
    }
}
