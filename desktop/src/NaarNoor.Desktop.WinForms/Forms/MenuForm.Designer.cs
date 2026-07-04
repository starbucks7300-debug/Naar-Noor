using System.Windows.Forms;

namespace NaarNoor.Desktop.WinForms.Forms
{
    partial class MenuForm : Form
    {
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.txtSearch = new TextBox();
            this.btnSearch = new Button();
            this.ddlCategory = new ComboBox();
            this.btnNew = new Button();
            this.dgvMenuItems = new DataGridView();
            this.btnEdit = new Button();
            this.btnDelete = new Button();
            this.pnlForm = new Panel();
            this.btnCancel = new Button();
            this.btnSave = new Button();
            this.txtNameEN = new TextBox();
            this.txtNameAR = new TextBox();
            this.numPrice = new NumericUpDown();
            this.chkAvailable = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMenuItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPrice)).BeginInit();
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Arial", 14F, FontStyle.Bold);
            this.lblTitle.Location = new Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(93, 22);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Menu Items";

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

            // ddlCategory
            this.ddlCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            this.ddlCategory.Location = new Point(305, 35);
            this.ddlCategory.Name = "ddlCategory";
            this.ddlCategory.Size = new Size(150, 23);
            this.ddlCategory.TabIndex = 3;

            // btnNew
            this.btnNew.Location = new Point(465, 35);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new Size(100, 23);
            this.btnNew.TabIndex = 4;
            this.btnNew.Text = "New Item";
            this.btnNew.UseVisualStyleBackColor = true;

            // dgvMenuItems
            this.dgvMenuItems.AllowUserToAddRows = false;
            this.dgvMenuItems.AllowUserToDeleteRows = false;
            this.dgvMenuItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMenuItems.Location = new Point(12, 70);
            this.dgvMenuItems.Name = "dgvMenuItems";
            this.dgvMenuItems.ReadOnly = true;
            this.dgvMenuItems.RowTemplate.Height = 25;
            this.dgvMenuItems.Size = new Size(760, 300);
            this.dgvMenuItems.TabIndex = 5;

            // btnEdit
            this.btnEdit.Location = new Point(12, 380);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new Size(75, 23);
            this.btnEdit.TabIndex = 6;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;

            // btnDelete
            this.btnDelete.Location = new Point(93, 380);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new Size(75, 23);
            this.btnDelete.TabIndex = 7;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;

            // pnlForm (hidden by default)
            this.pnlForm.BackColor = SystemColors.Window;
            this.pnlForm.BorderStyle = BorderStyle.FixedSingle;
            this.pnlForm.Controls.Add(new Label { Text = "Name (EN):", Location = new Point(10, 10), Size = new Size(80, 20) });
            this.pnlForm.Controls.Add(this.txtNameEN);
            this.pnlForm.Controls.Add(new Label { Text = "Name (AR):", Location = new Point(10, 45), Size = new Size(80, 20) });
            this.pnlForm.Controls.Add(this.txtNameAR);
            this.pnlForm.Controls.Add(new Label { Text = "Category:", Location = new Point(10, 80), Size = new Size(80, 20) });
            this.pnlForm.Controls.Add(new Label { Text = "Price:", Location = new Point(10, 115), Size = new Size(80, 20) });
            this.pnlForm.Controls.Add(this.numPrice);
            this.pnlForm.Controls.Add(this.chkAvailable);
            this.pnlForm.Controls.Add(this.btnSave);
            this.pnlForm.Controls.Add(this.btnCancel);
            this.pnlForm.Location = new Point(12, 420);
            this.pnlForm.Name = "pnlForm";
            this.pnlForm.Size = new Size(350, 180);
            this.pnlForm.TabIndex = 8;
            this.pnlForm.Visible = false;

            // txtNameEN
            this.txtNameEN.Location = new Point(100, 10);
            this.txtNameEN.Name = "txtNameEN";
            this.txtNameEN.Size = new Size(230, 23);
            this.txtNameEN.TabIndex = 9;

            // txtNameAR
            this.txtNameAR.Location = new Point(100, 45);
            this.txtNameAR.Name = "txtNameAR";
            this.txtNameAR.Size = new Size(230, 23);
            this.txtNameAR.TabIndex = 10;

            // numPrice
            this.numPrice.Location = new Point(100, 115);
            this.numPrice.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            this.numPrice.Name = "numPrice";
            this.numPrice.Size = new Size(80, 23);
            this.numPrice.TabIndex = 11;

            // chkAvailable
            this.chkAvailable.AutoSize = true;
            this.chkAvailable.Location = new Point(100, 150);
            this.chkAvailable.Name = "chkAvailable";
            this.chkAvailable.Size = new Size(89, 19);
            this.chkAvailable.TabIndex = 12;
            this.chkAvailable.Text = "Available";
            this.chkAvailable.UseVisualStyleBackColor = true;

            // btnSave
            this.btnSave.Location = new Point(190, 150);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new Size(75, 23);
            this.btnSave.TabIndex = 13;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;

            // btnCancel
            this.btnCancel.Location = new Point(270, 150);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(75, 23);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;

            // MenuForm
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(800, 620);
            this.Controls.Add(this.pnlForm);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.dgvMenuItems);
            this.Controls.Add(this.btnNew);
            this.Controls.Add(this.ddlCategory);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.lblTitle);
            this.Name = "MenuForm";
            this.Text = "Menu Management";
            ((System.ComponentModel.ISupportInitialize)(this.dgvMenuItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPrice)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Label lblTitle;
        private TextBox txtSearch;
        private Button btnSearch;
        private ComboBox ddlCategory;
        private Button btnNew;
        private DataGridView dgvMenuItems;
        private Button btnEdit;
        private Button btnDelete;
        private Panel pnlForm;
        private TextBox txtNameEN;
        private TextBox txtNameAR;
        private NumericUpDown numPrice;
        private CheckBox chkAvailable;
        private Button btnSave;
        private Button btnCancel;
    }
}
