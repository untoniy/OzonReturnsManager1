namespace OzonReturnsManager1
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dgvReturns = new System.Windows.Forms.DataGridView();
            this.lblDateFrom = new System.Windows.Forms.Label();
            this.lblDateTo = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.btnRequest = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.lblOrgType = new System.Windows.Forms.Label();
            this.cmbOrgType = new System.Windows.Forms.ComboBox();
            this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colShopName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colOfferId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReturns)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvReturns
            // 
            this.dgvReturns.AllowUserToAddRows = false;
            this.dgvReturns.AllowUserToDeleteRows = false;
            this.dgvReturns.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvReturns.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvReturns.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReturns.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colId,
            this.colDate,
            this.colType,
            this.colShopName,
            this.colOfferId,
            this.colBrand,
            this.colArticle,
            this.colSku,
            this.colQuantity});
            this.dgvReturns.Location = new System.Drawing.Point(12, 80);
            this.dgvReturns.Name = "dgvReturns";
            this.dgvReturns.ReadOnly = true;
            this.dgvReturns.Size = new System.Drawing.Size(760, 358);
            this.dgvReturns.TabIndex = 0;
            // 
            // lblDateFrom
            // 
            this.lblDateFrom.AutoSize = true;
            this.lblDateFrom.Location = new System.Drawing.Point(12, 15);
            this.lblDateFrom.Name = "lblDateFrom";
            this.lblDateFrom.Size = new System.Drawing.Size(46, 13);
            this.lblDateFrom.TabIndex = 1;
            this.lblDateFrom.Text = "Дата с:";
            // 
            // lblDateTo
            // 
            this.lblDateTo.AutoSize = true;
            this.lblDateTo.Location = new System.Drawing.Point(180, 15);
            this.lblDateTo.Name = "lblDateTo";
            this.lblDateTo.Size = new System.Drawing.Size(41, 13);
            this.lblDateTo.TabIndex = 2;
            this.lblDateTo.Text = "Дата по:";
            // 
            // dtpFrom
            // 
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFrom.Location = new System.Drawing.Point(15, 31);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(150, 20);
            this.dtpFrom.TabIndex = 3;
            // 
            // dtpTo
            // 
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTo.Location = new System.Drawing.Point(183, 31);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(150, 20);
            this.dtpTo.TabIndex = 4;
            // 
            // btnRequest
            // 
            this.btnRequest.Location = new System.Drawing.Point(350, 28);
            this.btnRequest.Name = "btnRequest";
            this.btnRequest.Size = new System.Drawing.Size(100, 25);
            this.btnRequest.TabIndex = 5;
            this.btnRequest.Text = "Запросить";
            this.btnRequest.UseVisualStyleBackColor = true;
            this.btnRequest.Click += new System.EventHandler(this.btnRequest_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(470, 15);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(43, 13);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Статус:";
            // 
            // cmbStatus
            // 
            this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatus.FormattingEnabled = true;
            this.cmbStatus.Location = new System.Drawing.Point(473, 31);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(130, 21);
            this.cmbStatus.TabIndex = 7;
            // 
            // lblOrgType
            // 
            this.lblOrgType.AutoSize = true;
            this.lblOrgType.Location = new System.Drawing.Point(620, 15);
            this.lblOrgType.Name = "lblOrgType";
            this.lblOrgType.Size = new System.Drawing.Size(76, 13);
            this.lblOrgType.TabIndex = 8;
            this.lblOrgType.Text = "Тип организац.:";
            // 
            // cmbOrgType
            // 
            this.cmbOrgType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbOrgType.FormattingEnabled = true;
            this.cmbOrgType.Items.AddRange(new object[] {
            "Все",
            "Процесс-Лайн",
            "Время"});
            this.cmbOrgType.Location = new System.Drawing.Point(623, 31);
            this.cmbOrgType.Name = "cmbOrgType";
            this.cmbOrgType.Size = new System.Drawing.Size(121, 21);
            this.cmbOrgType.TabIndex = 9;
            // 
            // colId
            // 
            this.colId.DataPropertyName = "Id";
            this.colId.HeaderText = "ID";
            this.colId.Name = "colId";
            this.colId.ReadOnly = true;
            // 
            // colDate
            // 
            this.colDate.DataPropertyName = "Date";
            this.colDate.HeaderText = "Дата";
            this.colDate.Name = "colDate";
            this.colDate.ReadOnly = true;
            // 
            // colType
            // 
            this.colType.DataPropertyName = "Type";
            this.colType.HeaderText = "Тип";
            this.colType.Name = "colType";
            this.colType.ReadOnly = true;
            // 
            // colShopName
            // 
            this.colShopName.DataPropertyName = "ShopName";
            this.colShopName.HeaderText = "Магазин";
            this.colShopName.Name = "colShopName";
            this.colShopName.ReadOnly = true;
            // 
            // colOfferId
            // 
            this.colOfferId.DataPropertyName = "OfferId";
            this.colOfferId.HeaderText = "Offer ID";
            this.colOfferId.Name = "colOfferId";
            this.colOfferId.ReadOnly = true;
            // 
            // colBrand
            // 
            this.colBrand.DataPropertyName = "Brand";
            this.colBrand.HeaderText = "Бренд";
            this.colBrand.Name = "colBrand";
            this.colBrand.ReadOnly = true;
            // 
            // colArticle
            // 
            this.colArticle.DataPropertyName = "Article";
            this.colArticle.HeaderText = "Артикул";
            this.colArticle.Name = "colArticle";
            this.colArticle.ReadOnly = true;
            // 
            // colSku
            // 
            this.colSku.DataPropertyName = "Sku";
            this.colSku.HeaderText = "Sku";
            this.colSku.Name = "colSku";
            this.colSku.ReadOnly = true;
            this.colSku.Visible = false;
            // 
            // colQuantity
            // 
            this.colQuantity.DataPropertyName = "Quantity";
            this.colQuantity.HeaderText = "Количество";
            this.colQuantity.Name = "colQuantity";
            this.colQuantity.ReadOnly = true;
            this.colQuantity.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 450);
            this.Controls.Add(this.cmbOrgType);
            this.Controls.Add(this.lblOrgType);
            this.Controls.Add(this.cmbStatus);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnRequest);
            this.Controls.Add(this.dtpTo);
            this.Controls.Add(this.dtpFrom);
            this.Controls.Add(this.lblDateTo);
            this.Controls.Add(this.lblDateFrom);
            this.Controls.Add(this.dgvReturns);
            this.Name = "Form1";
            this.Text = "Управление возвратами Ozon";
            ((System.ComponentModel.ISupportInitialize)(this.dgvReturns)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvReturns;
        private System.Windows.Forms.Label lblDateFrom;
        private System.Windows.Forms.Label lblDateTo;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Button btnRequest;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.Label lblOrgType;
        private System.Windows.Forms.ComboBox cmbOrgType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colShopName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colOfferId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBrand;
        private System.Windows.Forms.DataGridViewTextBoxColumn colArticle;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSku;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQuantity;
    }
}

