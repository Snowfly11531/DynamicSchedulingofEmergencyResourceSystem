namespace AttributeTable
{
    partial class AttributeTableFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AttributeTableFrm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tSB_AllAttribute = new System.Windows.Forms.ToolStripButton();
            this.tSB_SelectAttribute = new System.Windows.Forms.ToolStripButton();
            this.tSB_AddField = new System.Windows.Forms.ToolStripButton();
            this.tSB_SaveAsExcel = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip_RowHeaders = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TSMI_Selection = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_ClearSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.ContextMenuStrip_ColumnHeaders = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip_RowHeaders.SuspendLayout();
            this.ContextMenuStrip_ColumnHeaders.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tSB_AllAttribute,
            this.tSB_SelectAttribute,
            this.tSB_AddField,
            this.tSB_SaveAsExcel});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(751, 40);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tSB_AllAttribute
            // 
            this.tSB_AllAttribute.Image = ((System.Drawing.Image)(resources.GetObject("tSB_AllAttribute.Image")));
            this.tSB_AllAttribute.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tSB_AllAttribute.Name = "tSB_AllAttribute";
            this.tSB_AllAttribute.Size = new System.Drawing.Size(76, 37);
            this.tSB_AllAttribute.Text = "所有属性";
            this.tSB_AllAttribute.Click += new System.EventHandler(this.tSB_AllAttribute_Click);
            // 
            // tSB_SelectAttribute
            // 
            this.tSB_SelectAttribute.Image = ((System.Drawing.Image)(resources.GetObject("tSB_SelectAttribute.Image")));
            this.tSB_SelectAttribute.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tSB_SelectAttribute.Name = "tSB_SelectAttribute";
            this.tSB_SelectAttribute.Size = new System.Drawing.Size(76, 37);
            this.tSB_SelectAttribute.Text = "选中属性";
            this.tSB_SelectAttribute.Click += new System.EventHandler(this.tSB_SelectAttribute_Click);
            // 
            // tSB_AddField
            // 
            this.tSB_AddField.Image = ((System.Drawing.Image)(resources.GetObject("tSB_AddField.Image")));
            this.tSB_AddField.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tSB_AddField.Name = "tSB_AddField";
            this.tSB_AddField.Size = new System.Drawing.Size(76, 37);
            this.tSB_AddField.Text = "新建字段";
            this.tSB_AddField.Click += new System.EventHandler(this.tSB_AddField_Click);
            // 
            // tSB_SaveAsExcel
            // 
            this.tSB_SaveAsExcel.Image = ((System.Drawing.Image)(resources.GetObject("tSB_SaveAsExcel.Image")));
            this.tSB_SaveAsExcel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tSB_SaveAsExcel.Name = "tSB_SaveAsExcel";
            this.tSB_SaveAsExcel.Size = new System.Drawing.Size(105, 37);
            this.tSB_SaveAsExcel.Text = "导出为Excel表";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 379);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(751, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(170, 17);
            this.toolStripStatusLabel1.Text = "选中了 0 条记录，共 0 条记录";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeColumns = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 40);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(751, 339);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_CellMouseUp);
            this.dataGridView1.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_ColumnHeaderMouseClick);
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            this.dataGridView1.DoubleClick += new System.EventHandler(this.dataGridView1_DoubleClick);
            // 
            // contextMenuStrip_RowHeaders
            // 
            this.contextMenuStrip_RowHeaders.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMI_Selection,
            this.TSMI_ClearSelection});
            this.contextMenuStrip_RowHeaders.Name = "contextMenuStrip_RowHeaders";
            this.contextMenuStrip_RowHeaders.Size = new System.Drawing.Size(125, 48);
            // 
            // TSMI_Selection
            // 
            this.TSMI_Selection.Image = ((System.Drawing.Image)(resources.GetObject("TSMI_Selection.Image")));
            this.TSMI_Selection.Name = "TSMI_Selection";
            this.TSMI_Selection.Size = new System.Drawing.Size(124, 22);
            this.TSMI_Selection.Text = "选中要素";
            this.TSMI_Selection.Click += new System.EventHandler(this.TSMI_Selection_Click);
            // 
            // TSMI_ClearSelection
            // 
            this.TSMI_ClearSelection.Name = "TSMI_ClearSelection";
            this.TSMI_ClearSelection.Size = new System.Drawing.Size(124, 22);
            this.TSMI_ClearSelection.Text = "清除要素";
            // 
            // ContextMenuStrip_ColumnHeaders
            // 
            this.ContextMenuStrip_ColumnHeaders.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem});
            this.ContextMenuStrip_ColumnHeaders.Name = "ContextMenuStrip_ColumnHeaders";
            this.ContextMenuStrip_ColumnHeaders.Size = new System.Drawing.Size(153, 48);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(288, 148);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(8, 20);
            this.comboBox1.TabIndex = 3;
            // 
            // ToolStripMenuItem
            // 
            this.ToolStripMenuItem.Name = "ToolStripMenuItem";
            this.ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.ToolStripMenuItem.Text = "删除字段";
            this.ToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            // 
            // AttributeTableFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(751, 401);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "AttributeTableFrm";
            this.Text = "属性表";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStrip_RowHeaders.ResumeLayout(false);
            this.ContextMenuStrip_ColumnHeaders.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripButton tSB_AllAttribute;
        private System.Windows.Forms.ToolStripButton tSB_SelectAttribute;
        private System.Windows.Forms.ToolStripButton tSB_AddField;
        private System.Windows.Forms.ToolStripButton tSB_SaveAsExcel;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_RowHeaders;
        private System.Windows.Forms.ToolStripMenuItem TSMI_Selection;
        private System.Windows.Forms.ContextMenuStrip ContextMenuStrip_ColumnHeaders;
        private System.Windows.Forms.ToolStripMenuItem TSMI_ClearSelection;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem;
    }
}