namespace DynamicSchedulingofEmergencyResourceSystem
{
    partial class TOCFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TOCFrm));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TSMI_OpenAttributeTable = new System.Windows.Forms.ToolStripMenuItem();
            this.TSMI_RemoveLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.axTOCControl1 = new ESRI.ArcGIS.Controls.AxTOCControl();
            this.ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axTOCControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TSMI_OpenAttributeTable,
            this.TSMI_RemoveLayer,
            this.ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(153, 92);
            // 
            // TSMI_OpenAttributeTable
            // 
            this.TSMI_OpenAttributeTable.Image = ((System.Drawing.Image)(resources.GetObject("TSMI_OpenAttributeTable.Image")));
            this.TSMI_OpenAttributeTable.Name = "TSMI_OpenAttributeTable";
            this.TSMI_OpenAttributeTable.Size = new System.Drawing.Size(152, 22);
            this.TSMI_OpenAttributeTable.Text = "打开属性表";
            this.TSMI_OpenAttributeTable.Click += new System.EventHandler(this.TSMI_OpenAttributeTable_Click);
            // 
            // TSMI_RemoveLayer
            // 
            this.TSMI_RemoveLayer.Image = ((System.Drawing.Image)(resources.GetObject("TSMI_RemoveLayer.Image")));
            this.TSMI_RemoveLayer.Name = "TSMI_RemoveLayer";
            this.TSMI_RemoveLayer.Size = new System.Drawing.Size(152, 22);
            this.TSMI_RemoveLayer.Text = "移除图层";
            this.TSMI_RemoveLayer.Click += new System.EventHandler(this.TSMI_RemoveLayer_Click);
            // 
            // axTOCControl1
            // 
            this.axTOCControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axTOCControl1.Location = new System.Drawing.Point(0, 0);
            this.axTOCControl1.Name = "axTOCControl1";
            this.axTOCControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axTOCControl1.OcxState")));
            this.axTOCControl1.Size = new System.Drawing.Size(284, 262);
            this.axTOCControl1.TabIndex = 0;
            this.axTOCControl1.OnMouseUp += new ESRI.ArcGIS.Controls.ITOCControlEvents_Ax_OnMouseUpEventHandler(this.axTOCControl1_OnMouseUp);
            // 
            // ToolStripMenuItem
            // 
            this.ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("ToolStripMenuItem.Image")));
            this.ToolStripMenuItem.Name = "ToolStripMenuItem";
            this.ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.ToolStripMenuItem.Text = "缩放至图层";
            this.ToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            // 
            // TOCFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.axTOCControl1);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "TOCFrm";
            this.Text = "图层";
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axTOCControl1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ESRI.ArcGIS.Controls.AxTOCControl axTOCControl1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem TSMI_OpenAttributeTable;
        private System.Windows.Forms.ToolStripMenuItem TSMI_RemoveLayer;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem;
    }
}