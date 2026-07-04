using System.Windows.Forms;

namespace NaarNoor.Desktop.WinForms.Forms
{
    partial class ReportForm : Form
    {
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.dtpStartDate = new DateTimePicker();
            this.dtpEndDate = new DateTimePicker();
            this.btnFilter = new Button();
            this.btnExport = new Button();
            this.tabControl = new TabControl();
            this.tabRevenue = new TabPage();
            this.tabOrders = new TabPage();
            this.tabReservations = new TabPage();
            this.pnlMetrics = new Panel();
            this.lblRevenue = new Label();
            this.lblRevenueValue = new Label();
            this.lblOrders = new Label();
            this.lblOrdersValue = new Label();
            this.lblReservations = new Label();
            this.lblReservationsValue = new Label();
            this.dgvDetails = new DataGridView();
            this.lblLabelStart = new Label();
            this.lblLabelEnd = new Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetails)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabRevenue.SuspendLayout();
            this.tabOrders.SuspendLayout();
            this.tabReservations.SuspendLayout();
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Arial", 14F, FontStyle.Bold);
            this.lblTitle.Location = new Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(85, 22);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Analytics";

            // lblLabelStart
            this.lblLabelStart.AutoSize = true;
            this.lblLabelStart.Location = new Point(12, 37);
            this.lblLabelStart.Name = "lblLabelStart";
            this.lblLabelStart.Size = new Size(58, 15);
            this.lblLabelStart.TabIndex = 1;
            this.lblLabelStart.Text = "Start Date:";

            // dtpStartDate
            this.dtpStartDate.Location = new Point(75, 33);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new Size(120, 23);
            this.dtpStartDate.TabIndex = 2;

            // lblLabelEnd
            this.lblLabelEnd.AutoSize = true;
            this.lblLabelEnd.Location = new Point(205, 37);
            this.lblLabelEnd.Name = "lblLabelEnd";
            this.lblLabelEnd.Size = new Size(56, 15);
            this.lblLabelEnd.TabIndex = 3;
            this.lblLabelEnd.Text = "End Date:";

            // dtpEndDate
            this.dtpEndDate.Location = new Point(265, 33);
            this.dtpEndDate.Name = "dtpEndDate";
            this.dtpEndDate.Size = new Size(120, 23);
            this.dtpEndDate.TabIndex = 4;

            // btnFilter
            this.btnFilter.Location = new Point(395, 33);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new Size(75, 23);
            this.btnFilter.TabIndex = 5;
            this.btnFilter.Text = "Filter";
            this.btnFilter.UseVisualStyleBackColor = true;

            // btnExport
            this.btnExport.Location = new Point(480, 33);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new Size(100, 23);
            this.btnExport.TabIndex = 6;
            this.btnExport.Text = "Export to CSV";
            this.btnExport.UseVisualStyleBackColor = true;

            // pnlMetrics
            this.pnlMetrics.BackColor = SystemColors.Control;
            this.pnlMetrics.Location = new Point(12, 70);
            this.pnlMetrics.Name = "pnlMetrics";
            this.pnlMetrics.Size = new Size(760, 100);
            this.pnlMetrics.TabIndex = 7;

            // lblRevenue
            this.lblRevenue.AutoSize = true;
            this.lblRevenue.Font = new Font("Arial", 10F, FontStyle.Bold);
            this.lblRevenue.Location = new Point(20, 15);
            this.lblRevenue.Name = "lblRevenue";
            this.lblRevenue.Size = new Size(67, 16);
            this.lblRevenue.TabIndex = 0;
            this.lblRevenue.Text = "Revenue:";
            this.pnlMetrics.Controls.Add(this.lblRevenue);

            // lblRevenueValue
            this.lblRevenueValue.AutoSize = true;
            this.lblRevenueValue.Font = new Font("Arial", 16F, FontStyle.Bold);
            this.lblRevenueValue.ForeColor = Color.Green;
            this.lblRevenueValue.Location = new Point(20, 35);
            this.lblRevenueValue.Name = "lblRevenueValue";
            this.lblRevenueValue.Size = new Size(85, 25);
            this.lblRevenueValue.TabIndex = 1;
            this.lblRevenueValue.Text = "$0.00";
            this.pnlMetrics.Controls.Add(this.lblRevenueValue);

            // lblOrders
            this.lblOrders.AutoSize = true;
            this.lblOrders.Font = new Font("Arial", 10F, FontStyle.Bold);
            this.lblOrders.Location = new Point(260, 15);
            this.lblOrders.Name = "lblOrders";
            this.lblOrders.Size = new Size(51, 16);
            this.lblOrders.TabIndex = 2;
            this.lblOrders.Text = "Orders:";
            this.pnlMetrics.Controls.Add(this.lblOrders);

            // lblOrdersValue
            this.lblOrdersValue.AutoSize = true;
            this.lblOrdersValue.Font = new Font("Arial", 16F, FontStyle.Bold);
            this.lblOrdersValue.ForeColor = Color.Blue;
            this.lblOrdersValue.Location = new Point(260, 35);
            this.lblOrdersValue.Name = "lblOrdersValue";
            this.lblOrdersValue.Size = new Size(26, 25);
            this.lblOrdersValue.TabIndex = 3;
            this.lblOrdersValue.Text = "0";
            this.pnlMetrics.Controls.Add(this.lblOrdersValue);

            // lblReservations
            this.lblReservations.AutoSize = true;
            this.lblReservations.Font = new Font("Arial", 10F, FontStyle.Bold);
            this.lblReservations.Location = new Point(500, 15);
            this.lblReservations.Name = "lblReservations";
            this.lblReservations.Size = new Size(92, 16);
            this.lblReservations.TabIndex = 4;
            this.lblReservations.Text = "Reservations:";
            this.pnlMetrics.Controls.Add(this.lblReservations);

            // lblReservationsValue
            this.lblReservationsValue.AutoSize = true;
            this.lblReservationsValue.Font = new Font("Arial", 16F, FontStyle.Bold);
            this.lblReservationsValue.ForeColor = Color.Purple;
            this.lblReservationsValue.Location = new Point(500, 35);
            this.lblReservationsValue.Name = "lblReservationsValue";
            this.lblReservationsValue.Size = new Size(26, 25);
            this.lblReservationsValue.TabIndex = 5;
            this.lblReservationsValue.Text = "0";
            this.pnlMetrics.Controls.Add(this.lblReservationsValue);

            // tabControl
            this.tabControl.Location = new Point(12, 180);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new Size(760, 300);
            this.tabControl.TabIndex = 8;

            // tabRevenue
            this.tabRevenue.Controls.Add(this.dgvDetails);
            this.tabRevenue.Location = new Point(4, 24);
            this.tabRevenue.Name = "tabRevenue";
            this.tabRevenue.Padding = new Padding(3);
            this.tabRevenue.Size = new Size(752, 272);
            this.tabRevenue.TabIndex = 0;
            this.tabRevenue.Text = "Revenue";
            this.tabRevenue.UseVisualStyleBackColor = true;

            // tabOrders
            this.tabOrders.Location = new Point(4, 24);
            this.tabOrders.Name = "tabOrders";
            this.tabOrders.Padding = new Padding(3);
            this.tabOrders.Size = new Size(752, 272);
            this.tabOrders.TabIndex = 1;
            this.tabOrders.Text = "Orders";
            this.tabOrders.UseVisualStyleBackColor = true;

            // tabReservations
            this.tabReservations.Location = new Point(4, 24);
            this.tabReservations.Name = "tabReservations";
            this.tabReservations.Padding = new Padding(3);
            this.tabReservations.Size = new Size(752, 272);
            this.tabReservations.TabIndex = 2;
            this.tabReservations.Text = "Reservations";
            this.tabReservations.UseVisualStyleBackColor = true;

            // dgvDetails
            this.dgvDetails.AllowUserToAddRows = false;
            this.dgvDetails.AllowUserToDeleteRows = false;
            this.dgvDetails.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetails.Dock = DockStyle.Fill;
            this.dgvDetails.Location = new Point(3, 3);
            this.dgvDetails.Name = "dgvDetails";
            this.dgvDetails.ReadOnly = true;
            this.dgvDetails.RowTemplate.Height = 25;
            this.dgvDetails.Size = new Size(746, 266);
            this.dgvDetails.TabIndex = 0;

            // ReportForm
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(800, 500);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.pnlMetrics);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.btnFilter);
            this.Controls.Add(this.dtpEndDate);
            this.Controls.Add(this.lblLabelEnd);
            this.Controls.Add(this.dtpStartDate);
            this.Controls.Add(this.lblLabelStart);
            this.Controls.Add(this.lblTitle);
            this.Name = "ReportForm";
            this.Text = "Analytics & Reports";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetails)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabRevenue.ResumeLayout(false);
            this.tabOrders.ResumeLayout(false);
            this.tabReservations.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private Button btnFilter;
        private Button btnExport;
        private TabControl tabControl;
        private TabPage tabRevenue;
        private TabPage tabOrders;
        private TabPage tabReservations;
        private Panel pnlMetrics;
        private Label lblRevenue;
        private Label lblRevenueValue;
        private Label lblOrders;
        private Label lblOrdersValue;
        private Label lblReservations;
        private Label lblReservationsValue;
        private DataGridView dgvDetails;
        private Label lblLabelStart;
        private Label lblLabelEnd;
    }
}
