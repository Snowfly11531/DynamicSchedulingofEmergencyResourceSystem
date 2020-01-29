using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using WeifenLuo.WinFormsUI.Docking;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using DynamicSchedulingofEmergencyResourceSystem;
using Tool;

namespace AttributeTable
{
    public partial class AttributeTableFrm : DockContent
    {
        private FlashShapeTool pFlashShapeTool=new FlashShapeTool();
        
        public AttributeTableFrm()
        {
            InitializeComponent();
        }


        ///// <summary>
        ///// 属性表的要素图层，只写
        ///// </summary>
        //public IFeatureLayer pFeatureLayer
        //{
        //    set
        //    {
        //        this.pIFeatureLayer = value;
        //        this.Text = this.pIFeatureLayer.Name + "- 属性表";
        //    }
        //    get { return pIFeatureLayer; }
        //}

        private enum AtrributesMode {AllAttribute,SelectionAttribute }
        private AtrributesMode pAttributeMode;
        private void tSB_AllAttribute_Click(object sender, EventArgs e)
        {
            try 
            {
                this.ShowAllAttribute();  // 显示所有属性
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }
        private void tSB_SelectAttribute_Click(object sender, EventArgs e)
        {
            try
            {
                this.ShowSelectionAttribute();  //显示选中属性 
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        #region 设置 DataGridView 的列
        /// <summary>
        /// 设置 DataGridView 的列
        /// </summary>
        private void SetDataGridViewColumn()
        {
            this.dataGridView1.Rows.Clear();  //清楚DataGridView行和列
            this.dataGridView1.Columns.Clear();
            IFeatureClass pFeatureClass = Variable.pAttributeTableFeatureLayer.FeatureClass;
            IFields pFields = pFeatureClass.Fields;
            this.dataGridView1.ColumnCount = pFields.FieldCount;  //设置列数


            for (int i = 0; i < this.dataGridView1.ColumnCount;i++ )
            {
                IField pField = pFields.get_Field(i);
                if (pField.Name.Equals(pFeatureClass.OIDFieldName))
                {
                    this.dataGridView1.Columns[i].Name = "编号";
                }
                else if(pField.Name.Equals(pFeatureClass.ShapeFieldName))
                {
                    this.dataGridView1.Columns[i].Name = "类型";
                    this.dataGridView1.Columns[i].Visible = false;
                }
                else if (pField.Name.Equals("shape_length", StringComparison.OrdinalIgnoreCase))
                {
                    this.dataGridView1.Columns[i].Name = "长度";
                    this.dataGridView1.Columns[i].Visible = false;
                }
                else if (pField.Name.Equals("shape_area", StringComparison.OrdinalIgnoreCase))
                {
                    this.dataGridView1.Columns[i].Name = "面积";
                    this.dataGridView1.Columns[i].Visible = false;
                }
                else
                { 
                    this.dataGridView1.Columns[i].Name = pField.AliasName;
                }
                this.dataGridView1.Columns[i].ValueType = this.GetColumnType(pField.Type);
                this.dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.Programmatic;
                //this.SetColumnWidth(dataGridView1.Columns[i]);
            }
            if (dataGridView1.Width > this.dataGridView1.Columns[0].Width * this.dataGridView1.ColumnCount)
            {
                this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            else
            {
                this.dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
        }

        /// <summary>
        /// 获取 DataGridView 列的类型
        /// </summary>
        /// <param name="fieldType">ESRI字段类型</param>
        private System.Type GetColumnType(esriFieldType fieldType)
        {
            string typeName;
            switch (fieldType)
            {
                case esriFieldType.esriFieldTypeOID:
                    typeName = "System.Int32";
                    break;
                case esriFieldType.esriFieldTypeSmallInteger:
                    typeName = "System.Int16";
                    break;
                case esriFieldType.esriFieldTypeInteger:
                    typeName = "System.Int32";
                    break;
                case esriFieldType.esriFieldTypeSingle:
                    typeName = "System.Single";
                    break;
                case esriFieldType.esriFieldTypeDouble:
                    typeName = "System.Double";
                    break;
                case esriFieldType.esriFieldTypeString:
                    typeName = "System.String";
                    break;
                case esriFieldType.esriFieldTypeDate:
                    typeName = "System.DateTime";
                    break;
                default:
                    typeName = "System.String";
                    break;
            }
            return System.Type.GetType(typeName);
        }


        /// <summary>
        /// 设置列宽
        /// </summary>
        /// <param name="column">列</param>
        private void SetColumnWidth(DataGridViewColumn column)
        {
            string typeName = column.ValueType.FullName;
            if (typeName == "System.String")
            {               
                column.Width = 100;                       
            }
            else
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
        }
        #endregion

        /// <summary>
        /// 显示所有属性
        /// </summary>
        public void ShowAllAttribute()
        {
            this.pAttributeMode = AtrributesMode.AllAttribute;
            this.tSB_AllAttribute.Checked = true;
            this.tSB_SelectAttribute.Checked = false;
            this.SetDataGridViewColumn();  //设置DataGridView的列
            //设置DataGridView的行
            IFeatureCursor pFeatureCursor = Variable.pAttributeTableFeatureLayer.FeatureClass.Search(null, true);//true表示每执行一次NextFeature(),上一个Feature将立即被回收
            IFeature pFeature = pFeatureCursor.NextFeature();
            while (pFeature != null)
            {
                object[] cellValue=new object[this.dataGridView1.ColumnCount];//新建一行
                for (int i = 0; i < this.dataGridView1.ColumnCount; i++)
                {
                    cellValue[i] = pFeature.get_Value(i);
                    if (cellValue[i] != null && cellValue[i].ToString().Trim() == "")
                        cellValue[i] = null;
                }
                this.dataGridView1.Rows.Add(cellValue);
                pFeature = pFeatureCursor.NextFeature();
            }
            this.dataGridView1.ClearSelection();  //清除所有选中单元格
            toolStripStatusLabel1.Text = string.Format("选中了 {0} 条记录，共 {1} 条记录", this.dataGridView1.SelectedRows.Count, this.dataGridView1.RowCount);
            
        }

        /// <summary>
        /// 显示选中属性
        /// </summary>
        public void ShowSelectionAttribute()
        {
            this.pAttributeMode = AtrributesMode.SelectionAttribute;
            this.tSB_AllAttribute.Checked = false;
            this.tSB_SelectAttribute.Checked = true;

            this.SetDataGridViewColumn();  //设置DataGridView的列
            
             //设置 DataGridView的行
            IFeatureSelection pFeatureSelection = (IFeatureSelection)Variable.pAttributeTableFeatureLayer; 
            IEnumIDs pEnumIDs = pFeatureSelection.SelectionSet.IDs;           
            int pFid = pEnumIDs.Next();
            while (pFid != -1)
            {
                IFeature pFeature = Variable.pAttributeTableFeatureLayer.FeatureClass.GetFeature(pFid);
                object[] cellValue = new object[dataGridView1.ColumnCount];  //新建一行
                for (int i = 0; i < this.dataGridView1.ColumnCount; i++)
                {
                    cellValue[i] = pFeature.get_Value(i);
                    if (cellValue[i] != null && cellValue[i].ToString().Trim() == "")
                        cellValue[i] = null;
                }
                this.dataGridView1.Rows.Add(cellValue);
                pFid = pEnumIDs.Next();
            }
            dataGridView1.ClearSelection();// 清除所有选中单元格
            toolStripStatusLabel1.Text = string.Format("选中了 {0} 条记录，共 {1} 条记录", this.dataGridView1.SelectedRows.Count, this.dataGridView1.RowCount);
        }


        /// <summary>
        /// 刷新属性表
        /// </summary>
        private void RefreshAttributeTable()
        {
            switch (this.pAttributeMode)
            {
                case AtrributesMode.AllAttribute:
                    this.ShowAllAttribute();
                    break;
                case AtrributesMode.SelectionAttribute:
                    this.ShowSelectionAttribute();
                    break;
            }
        }
        

        #region DataGridView 的 SelectionChanged事件
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {            
            try
            {
                toolStripStatusLabel1.Text = string.Format("选中了 {0} 条记录，共 {1} 条记录", dataGridView1.SelectedRows.Count, dataGridView1.RowCount);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message+"\n"+ex.ToString(),"异常");
            }
        }
        #endregion


       
        private void TSMI_Selection_Click(object sender, EventArgs e)
        {
            try
            {
                if (!dataGridView1.CurrentRow.Selected)
                { 
                    this.dataGridView1.CurrentRow.Selected = true;
                }
                
                this.SelectBySelectedRows();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }
        /// <summary>
        /// 根据 DataGridView 的选中行来选中要素
        /// </summary>
        private void SelectBySelectedRows()
        {
            IFeatureSelection pFeatureSelection = (IFeatureSelection)Variable.pAttributeTableFeatureLayer;
            Variable.pMapFrm.mainMapControl.Map.ClearSelection();
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {                                  
                int pFid = Convert.ToInt32(row.Cells[0].Value);                   
                pFeatureSelection.SelectionSet.Add(pFid);
            }           
        }

        /// <summary>
        /// 缩放到要素选中集和闪烁图层中的所有选中要素
        /// </summary>
        private void ZoomToAndFlashShapes()
        {
            MapControlTool.ZoomToFeatureSelection(Variable.pAttributeTableFeatureLayer,Variable.pMapFrm.mainMapControl);  //缩放到要素选中集
            this.pFlashShapeTool.FlashShapes(Variable.pAttributeTableFeatureLayer, 300);  //闪烁图层中的所有选中要素

        }

        private void dataGridView1_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == -1)                // 单击的是行标题（包括全选单元格）
                {
                    if (e.Button == MouseButtons.Right)      // 右击（不会自动选中行）
                    {                      
                        
                        if (e.RowIndex == -1)                // 单击的是全选单元格
                            dataGridView1.SelectAll();
                        else                                 // 单击的是行标题（不包括全选单元格）
                        {
                            this.dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[0];                            
                            contextMenuStrip_RowHeaders.Show(MousePosition);   // 弹出快捷菜单
                        }
                       
                    }
                    else if (e.Button == MouseButtons.Left)  // 左击（会自动选中行）
                    {
                        this.SelectBySelectedRows();  // 根据 DataGridView 的选中行来选中要素
                        this.Activate();

                       
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            this.ZoomToAndFlashShapes();  // 缩放到要素选中集和闪烁图层中的所有选中要素
        }

        private void tSB_AddField_Click(object sender, EventArgs e)
        {
            try
            {
                IDataset dataset = (IDataset)Variable.pAttributeTableFeatureLayer.FeatureClass;
                IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)dataset.Workspace;
                if (workspaceEdit.IsBeingEdited())
                {
                    MessageBox.Show("地图处于编辑状态，暂时无法新建字段！");
                    return;
                }
                NewFieldFrm newFieldForm = new NewFieldFrm();
                if (newFieldForm.ShowDialog() == DialogResult.OK)
                {
                    this.RefreshAttributeTable();
                    int newColumnIndex = dataGridView1.Columns.Count - 1;
                    dataGridView1.FirstDisplayedScrollingColumnIndex = newColumnIndex;  // 滚动到新字段对应的列
                    MessageBox.Show(string.Format("{0} 字段新建成功！", this.dataGridView1.Columns[newColumnIndex].Name));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                IDataset dataset = (IDataset)Variable.pAttributeTableFeatureLayer.FeatureClass;
                IWorkspaceEdit workspaceEdit = (IWorkspaceEdit)dataset.Workspace;
                if (workspaceEdit.IsBeingEdited())
                {
                    MessageBox.Show("地图处于编辑状态，暂时无法删除字段！");
                    return;
                }
                if (MessageBox.Show(string.Format("确定要删除 {0} 字段吗？", this.columnName), "", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    IFeatureClass featureClass = Variable.pAttributeTableFeatureLayer.FeatureClass;
                    IFields fields = featureClass.Fields;
                    IField field = fields.get_Field(this.columnIndex);
                    featureClass.DeleteField(field);                   // 删除字段
                    dataGridView1.Columns.RemoveAt(this.columnIndex);  // 删除列
                    MessageBox.Show(string.Format("{0} 字段已删除！", this.columnName));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }


        private string columnName;
        private int columnIndex;

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                this.columnName = dataGridView1.Columns[e.ColumnIndex].Name;
                this.columnIndex = e.ColumnIndex;
                if (e.Button == MouseButtons.Right)  // 右击列标题
                {
                    string featureClassName = ((IDataset)Variable.pAttributeTableFeatureLayer.FeatureClass).Name;

                    ContextMenuStrip_ColumnHeaders.Show(MousePosition);  // 弹出快捷菜单
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }



       

       

       



    }
}
