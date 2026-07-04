using System.Windows.Forms;

namespace NaarNoor.Desktop.WinForms.Forms
{
    partial class ReservationForm : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            var lblTitle = new Label();
            var pnlFilters = new Panel();
            var lblStatusFilter = new Label();
            var cmbStatusFilter = new ComboBox();
            var lblSearchCustomer = new Label();
            var txtSearchCustomer = new TextBox();
            var btnNewReservation = new Button();
            var dataGridReservations = new DataGridView();
            var pnlPagination = new Panel();
            var lblPageInfo = new Label();
            var btnPrevious = new Button();
            var btnNext = new Button();
            var pnlNewReservationForm = new Panel();
            var lblFormTitle = new Label();
            var lblFormCustomerName = new Label();
            var txtFormCustomerName = new TextBox();
            var lblFormBookingTime = new Label();
            var dtpFormBookingTime = new DateTimePicker();
            var lblFormPartySize = new Label();
            var nudFormPartySize = new NumericUpDown();
            var lblFormCustomerEmail = new Label();
            var txtFormCustomerEmail = new TextBox();
            var lblFormCustomerPhone = new Label();
            var txtFormCustomerPhone = new TextBox();
            var lblFormSpecialRequests = new Label();
            var txtFormSpecialRequests = new TextBox();
            var btnCreate = new Button();
            var btnCancel = new Button();
            var pnlActions = new Panel();
            var btnDelete = new Button();
            var btnUpdate = new Button();
            var lblError = new Label();
            var lblLoading = new Label();
            
            ((System.ComponentModel.ISupportInitialize)dataGridReservations).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudFormPartySize).BeginInit();
            pnlFilters.SuspendLayout();
            pnlPagination.SuspendLayout();
            pnlNewReservationForm.SuspendLayout();
            pnlActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
            lblTitle.Location = new Point(12, 9);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(168, 25);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Reservations";
            // 
            // pnlFilters
            // 
            pnlFilters.BackColor = Color.FromArgb(240, 240, 240);
            pnlFilters.Controls.Add(lblStatusFilter);
            pnlFilters.Controls.Add(cmbStatusFilter);
            pnlFilters.Controls.Add(lblSearchCustomer);
            pnlFilters.Controls.Add(txtSearchCustomer);
            pnlFilters.Controls.Add(btnNewReservation);
            pnlFilters.Dock = DockStyle.Top;
            pnlFilters.Location = new Point(0, 0);
            pnlFilters.Name = "pnlFilters";
            pnlFilters.Size = new Size(1000, 80);
            pnlFilters.TabIndex = 1;
            pnlFilters.Padding = new Padding(12, 12, 12, 12);
            // 
            // lblStatusFilter
            // 
            lblStatusFilter.AutoSize = true;
            lblStatusFilter.Location = new Point(12, 50);
            lblStatusFilter.Name = "lblStatusFilter";
            lblStatusFilter.Size = new Size(42, 15);
            lblStatusFilter.TabIndex = 4;
            lblStatusFilter.Text = "Status:";
            // 
            // cmbStatusFilter
            // 
            cmbStatusFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatusFilter.Location = new Point(65, 47);
            cmbStatusFilter.Name = "cmbStatusFilter";
            cmbStatusFilter.Size = new Size(150, 23);
            cmbStatusFilter.TabIndex = 3;
            // 
            // lblSearchCustomer
            // 
            lblSearchCustomer.AutoSize = true;
            lblSearchCustomer.Location = new Point(12, 15);
            lblSearchCustomer.Name = "lblSearchCustomer";
            lblSearchCustomer.Size = new Size(84, 15);
            lblSearchCustomer.TabIndex = 2;
            lblSearchCustomer.Text = "Search Name:";
            // 
            // txtSearchCustomer
            // 
            txtSearchCustomer.Location = new Point(100, 12);
            txtSearchCustomer.Name = "txtSearchCustomer";
            txtSearchCustomer.Size = new Size(200, 23);
            txtSearchCustomer.TabIndex = 1;
            // 
            // btnNewReservation
            // 
            btnNewReservation.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnNewReservation.BackColor = Color.FromArgb(0, 120, 212);
            btnNewReservation.FlatAppearance.BorderSize = 0;
            btnNewReservation.FlatStyle = FlatStyle.Flat;
            btnNewReservation.ForeColor = Color.White;
            btnNewReservation.Location = new Point(850, 30);
            btnNewReservation.Name = "btnNewReservation";
            btnNewReservation.Size = new Size(138, 30);
            btnNewReservation.TabIndex = 0;
            btnNewReservation.Text = "New Reservation";
            btnNewReservation.UseVisualStyleBackColor = false;
            // 
            // dataGridReservations
            // 
            dataGridReservations.AllowUserToAddRows = false;
            dataGridReservations.AllowUserToDeleteRows = false;
            dataGridReservations.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridReservations.Dock = DockStyle.Fill;
            dataGridReservations.Location = new Point(0, 80);
            dataGridReservations.Name = "dataGridReservations";
            dataGridReservations.ReadOnly = true;
            dataGridReservations.RowHeadersWidth = 51;
            dataGridReservations.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridReservations.Size = new Size(1000, 350);
            dataGridReservations.TabIndex = 2;
            // 
            // pnlPagination
            // 
            pnlPagination.BackColor = Color.FromArgb(240, 240, 240);
            pnlPagination.Controls.Add(lblPageInfo);
            pnlPagination.Controls.Add(btnPrevious);
            pnlPagination.Controls.Add(btnNext);
            pnlPagination.Dock = DockStyle.Bottom;
            pnlPagination.Location = new Point(0, 430);
            pnlPagination.Name = "pnlPagination";
            pnlPagination.Size = new Size(1000, 50);
            pnlPagination.TabIndex = 3;
            // 
            // lblPageInfo
            // 
            lblPageInfo.AutoSize = true;
            lblPageInfo.Location = new Point(400, 15);
            lblPageInfo.Name = "lblPageInfo";
            lblPageInfo.Size = new Size(80, 15);
            lblPageInfo.TabIndex = 2;
            lblPageInfo.Text = "Page 1 of 1";
            // 
            // btnPrevious
            // 
            btnPrevious.Location = new Point(20, 12);
            btnPrevious.Name = "btnPrevious";
            btnPrevious.Size = new Size(80, 23);
            btnPrevious.TabIndex = 0;
            btnPrevious.Text = "Previous";
            btnPrevious.UseVisualStyleBackColor = true;
            // 
            // btnNext
            // 
            btnNext.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnNext.Location = new Point(900, 12);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(80, 23);
            btnNext.TabIndex = 1;
            btnNext.Text = "Next";
            btnNext.UseVisualStyleBackColor = true;
            // 
            // pnlNewReservationForm
            // 
            pnlNewReservationForm.BackColor = Color.FromArgb(250, 250, 250);
            pnlNewReservationForm.BorderStyle = BorderStyle.FixedSingle;
            pnlNewReservationForm.Controls.Add(lblFormTitle);
            pnlNewReservationForm.Controls.Add(lblFormCustomerName);
            pnlNewReservationForm.Controls.Add(txtFormCustomerName);
            pnlNewReservationForm.Controls.Add(lblFormBookingTime);
            pnlNewReservationForm.Controls.Add(dtpFormBookingTime);
            pnlNewReservationForm.Controls.Add(lblFormPartySize);
            pnlNewReservationForm.Controls.Add(nudFormPartySize);
            pnlNewReservationForm.Controls.Add(lblFormCustomerEmail);
            pnlNewReservationForm.Controls.Add(txtFormCustomerEmail);
            pnlNewReservationForm.Controls.Add(lblFormCustomerPhone);
            pnlNewReservationForm.Controls.Add(txtFormCustomerPhone);
            pnlNewReservationForm.Controls.Add(lblFormSpecialRequests);
            pnlNewReservationForm.Controls.Add(txtFormSpecialRequests);
            pnlNewReservationForm.Controls.Add(btnCreate);
            pnlNewReservationForm.Controls.Add(btnCancel);
            pnlNewReservationForm.Location = new Point(550, 150);
            pnlNewReservationForm.Name = "pnlNewReservationForm";
            pnlNewReservationForm.Size = new Size(400, 300);
            pnlNewReservationForm.TabIndex = 4;
            pnlNewReservationForm.Visible = false;
            // 
            // lblFormTitle
            // 
            lblFormTitle.AutoSize = true;
            lblFormTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            lblFormTitle.Location = new Point(120, 10);
            lblFormTitle.Name = "lblFormTitle";
            lblFormTitle.Size = new Size(152, 21);
            lblFormTitle.TabIndex = 0;
            lblFormTitle.Text = "New Reservation";
            // 
            // lblFormCustomerName
            // 
            lblFormCustomerName.AutoSize = true;
            lblFormCustomerName.Location = new Point(10, 40);
            lblFormCustomerName.Name = "lblFormCustomerName";
            lblFormCustomerName.Size = new Size(87, 15);
            lblFormCustomerName.TabIndex = 1;
            lblFormCustomerName.Text = "Customer Name:";
            // 
            // txtFormCustomerName
            // 
            txtFormCustomerName.Location = new Point(110, 37);
            txtFormCustomerName.Name = "txtFormCustomerName";
            txtFormCustomerName.Size = new Size(280, 23);
            txtFormCustomerName.TabIndex = 2;
            // 
            // lblFormBookingTime
            // 
            lblFormBookingTime.AutoSize = true;
            lblFormBookingTime.Location = new Point(10, 68);
            lblFormBookingTime.Name = "lblFormBookingTime";
            lblFormBookingTime.Size = new Size(88, 15);
            lblFormBookingTime.TabIndex = 3;
            lblFormBookingTime.Text = "Booking Time:";
            // 
            // dtpFormBookingTime
            // 
            dtpFormBookingTime.Location = new Point(110, 65);
            dtpFormBookingTime.Name = "dtpFormBookingTime";
            dtpFormBookingTime.Size = new Size(280, 23);
            dtpFormBookingTime.TabIndex = 4;
            // 
            // lblFormPartySize
            // 
            lblFormPartySize.AutoSize = true;
            lblFormPartySize.Location = new Point(10, 96);
            lblFormPartySize.Name = "lblFormPartySize";
            lblFormPartySize.Size = new Size(66, 15);
            lblFormPartySize.TabIndex = 5;
            lblFormPartySize.Text = "Party Size:";
            // 
            // nudFormPartySize
            // 
            nudFormPartySize.Location = new Point(110, 93);
            nudFormPartySize.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudFormPartySize.Name = "nudFormPartySize";
            nudFormPartySize.Size = new Size(280, 23);
            nudFormPartySize.TabIndex = 6;
            nudFormPartySize.Value = new decimal(new int[] { 2, 0, 0, 0 });
            // 
            // lblFormCustomerEmail
            // 
            lblFormCustomerEmail.AutoSize = true;
            lblFormCustomerEmail.Location = new Point(10, 124);
            lblFormCustomerEmail.Name = "lblFormCustomerEmail";
            lblFormCustomerEmail.Size = new Size(73, 15);
            lblFormCustomerEmail.TabIndex = 7;
            lblFormCustomerEmail.Text = "Email (opt):";
            // 
            // txtFormCustomerEmail
            // 
            txtFormCustomerEmail.Location = new Point(110, 121);
            txtFormCustomerEmail.Name = "txtFormCustomerEmail";
            txtFormCustomerEmail.Size = new Size(280, 23);
            txtFormCustomerEmail.TabIndex = 8;
            // 
            // lblFormCustomerPhone
            // 
            lblFormCustomerPhone.AutoSize = true;
            lblFormCustomerPhone.Location = new Point(10, 152);
            lblFormCustomerPhone.Name = "lblFormCustomerPhone";
            lblFormCustomerPhone.Size = new Size(72, 15);
            lblFormCustomerPhone.TabIndex = 9;
            lblFormCustomerPhone.Text = "Phone (opt):";
            // 
            // txtFormCustomerPhone
            // 
            txtFormCustomerPhone.Location = new Point(110, 149);
            txtFormCustomerPhone.Name = "txtFormCustomerPhone";
            txtFormCustomerPhone.Size = new Size(280, 23);
            txtFormCustomerPhone.TabIndex = 10;
            // 
            // lblFormSpecialRequests
            // 
            lblFormSpecialRequests.AutoSize = true;
            lblFormSpecialRequests.Location = new Point(10, 180);
            lblFormSpecialRequests.Name = "lblFormSpecialRequests";
            lblFormSpecialRequests.Size = new Size(94, 15);
            lblFormSpecialRequests.TabIndex = 11;
            lblFormSpecialRequests.Text = "Special Req. (opt):";
            // 
            // txtFormSpecialRequests
            // 
            txtFormSpecialRequests.Location = new Point(110, 177);
            txtFormSpecialRequests.Multiline = true;
            txtFormSpecialRequests.Name = "txtFormSpecialRequests";
            txtFormSpecialRequests.Size = new Size(280, 60);
            txtFormSpecialRequests.TabIndex = 12;
            // 
            // btnCreate
            // 
            btnCreate.BackColor = Color.FromArgb(0, 120, 212);
            btnCreate.FlatAppearance.BorderSize = 0;
            btnCreate.FlatStyle = FlatStyle.Flat;
            btnCreate.ForeColor = Color.White;
            btnCreate.Location = new Point(110, 260);
            btnCreate.Name = "btnCreate";
            btnCreate.Size = new Size(100, 30);
            btnCreate.TabIndex = 13;
            btnCreate.Text = "Create";
            btnCreate.UseVisualStyleBackColor = false;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.FromArgb(200, 200, 200);
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point(220, 260);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(100, 30);
            btnCancel.TabIndex = 14;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            // 
            // pnlActions
            // 
            pnlActions.BackColor = Color.FromArgb(240, 240, 240);
            pnlActions.Controls.Add(btnDelete);
            pnlActions.Controls.Add(btnUpdate);
            pnlActions.Dock = DockStyle.Bottom;
            pnlActions.Location = new Point(0, 480);
            pnlActions.Name = "pnlActions";
            pnlActions.Size = new Size(1000, 50);
            pnlActions.TabIndex = 5;
            // 
            // btnDelete
            // 
            btnDelete.BackColor = Color.FromArgb(210, 50, 50);
            btnDelete.FlatAppearance.BorderSize = 0;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.ForeColor = Color.White;
            btnDelete.Location = new Point(900, 12);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(80, 30);
            btnDelete.TabIndex = 1;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = false;
            // 
            // btnUpdate
            // 
            btnUpdate.BackColor = Color.FromArgb(0, 120, 212);
            btnUpdate.FlatAppearance.BorderSize = 0;
            btnUpdate.FlatStyle = FlatStyle.Flat;
            btnUpdate.ForeColor = Color.White;
            btnUpdate.Location = new Point(810, 12);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(80, 30);
            btnUpdate.TabIndex = 0;
            btnUpdate.Text = "Update";
            btnUpdate.UseVisualStyleBackColor = false;
            // 
            // lblError
            // 
            lblError.AutoSize = true;
            lblError.ForeColor = Color.Red;
            lblError.Location = new Point(12, 460);
            lblError.Name = "lblError";
            lblError.Size = new Size(0, 15);
            lblError.TabIndex = 6;
            lblError.Visible = false;
            // 
            // lblLoading
            // 
            lblLoading.AutoSize = true;
            lblLoading.ForeColor = Color.FromArgb(0, 120, 212);
            lblLoading.Location = new Point(12, 440);
            lblLoading.Name = "lblLoading";
            lblLoading.Size = new Size(74, 15);
            lblLoading.TabIndex = 7;
            lblLoading.Text = "Loading...";
            lblLoading.Visible = false;
            // 
            // ReservationForm
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1000, 530);
            this.Controls.Add(lblError);
            this.Controls.Add(lblLoading);
            this.Controls.Add(pnlActions);
            this.Controls.Add(pnlNewReservationForm);
            this.Controls.Add(pnlPagination);
            this.Controls.Add(dataGridReservations);
            this.Controls.Add(pnlFilters);
            this.Name = "ReservationForm";
            this.Text = "Naar-Noor - Reservations";
            ((System.ComponentModel.ISupportInitialize)dataGridReservations).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudFormPartySize).EndInit();
            pnlFilters.ResumeLayout(false);
            pnlFilters.PerformLayout();
            pnlPagination.ResumeLayout(false);
            pnlPagination.PerformLayout();
            pnlNewReservationForm.ResumeLayout(false);
            pnlNewReservationForm.PerformLayout();
            pnlActions.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
            
            // Store controls as instance fields
            this.lblTitle = lblTitle;
            this.pnlFilters = pnlFilters;
            this.lblStatusFilter = lblStatusFilter;
            this.cmbStatusFilter = cmbStatusFilter;
            this.lblSearchCustomer = lblSearchCustomer;
            this.txtSearchCustomer = txtSearchCustomer;
            this.btnNewReservation = btnNewReservation;
            this.dataGridReservations = dataGridReservations;
            this.pnlPagination = pnlPagination;
            this.lblPageInfo = lblPageInfo;
            this.btnPrevious = btnPrevious;
            this.btnNext = btnNext;
            this.pnlNewReservationForm = pnlNewReservationForm;
            this.lblFormTitle = lblFormTitle;
            this.lblFormCustomerName = lblFormCustomerName;
            this.txtFormCustomerName = txtFormCustomerName;
            this.lblFormBookingTime = lblFormBookingTime;
            this.dtpFormBookingTime = dtpFormBookingTime;
            this.lblFormPartySize = lblFormPartySize;
            this.nudFormPartySize = nudFormPartySize;
            this.lblFormCustomerEmail = lblFormCustomerEmail;
            this.txtFormCustomerEmail = txtFormCustomerEmail;
            this.lblFormCustomerPhone = lblFormCustomerPhone;
            this.txtFormCustomerPhone = txtFormCustomerPhone;
            this.lblFormSpecialRequests = lblFormSpecialRequests;
            this.txtFormSpecialRequests = txtFormSpecialRequests;
            this.btnCreate = btnCreate;
            this.btnCancel = btnCancel;
            this.pnlActions = pnlActions;
            this.btnDelete = btnDelete;
            this.btnUpdate = btnUpdate;
            this.lblError = lblError;
            this.lblLoading = lblLoading;
        }

        #endregion

        public Label lblTitle;
        public Panel pnlFilters;
        public Label lblStatusFilter;
        public ComboBox cmbStatusFilter;
        public Label lblSearchCustomer;
        public TextBox txtSearchCustomer;
        public Button btnNewReservation;
        public DataGridView dataGridReservations;
        public Panel pnlPagination;
        public Label lblPageInfo;
        public Button btnPrevious;
        public Button btnNext;
        public Panel pnlNewReservationForm;
        public Label lblFormTitle;
        public Label lblFormCustomerName;
        public TextBox txtFormCustomerName;
        public Label lblFormBookingTime;
        public DateTimePicker dtpFormBookingTime;
        public Label lblFormPartySize;
        public NumericUpDown nudFormPartySize;
        public Label lblFormCustomerEmail;
        public TextBox txtFormCustomerEmail;
        public Label lblFormCustomerPhone;
        public TextBox txtFormCustomerPhone;
        public Label lblFormSpecialRequests;
        public TextBox txtFormSpecialRequests;
        public Button btnCreate;
        public Button btnCancel;
        public Panel pnlActions;
        public Button btnDelete;
        public Button btnUpdate;
        public Label lblError;
        public Label lblLoading;
    }
}
