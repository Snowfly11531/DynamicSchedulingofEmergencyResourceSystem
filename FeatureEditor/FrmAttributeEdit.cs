using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using System.Collections;
using ESRI.ArcGIS.Controls;

namespace FeatureEditor
{
    internal partial class FrmAttributeEdit : Form
    {
        #region 变量定义

        IFeature pFeature = null;
        private DataTable pFeatTable = null;
        private Hashtable _FeatHashtable = null;

        private IEngineEditor _gisEdit;
        public IEngineEditor GisEdit
        {
            get { return _gisEdit; }
            set { _gisEdit = value; }
        }

        private List<IFeature> _lstFeature;
        public List<IFeature> LstFeature
        {
            get { return _lstFeature; }
            set { _lstFeature = value; }
        }

        private IFeatureLayer _featLyr;
        public IFeatureLayer FeatLyr
        {
            get { return _featLyr; }
            set { _featLyr = value; }
        }

        #endregion


        public FrmAttributeEdit()
        {
            InitializeComponent();
            pFeatTable = new DataTable();
            InitTable(pFeatTable);
           
        }

        #region 操作函数

        /// <summary>
        /// 初始化数据表格
        /// </summary>
        /// <param name="pTable"></param>
        private void InitTable(DataTable pTable)
        {
            //创建字段名称列
            DataColumn pDataColumn = new DataColumn();
            pDataColumn.ColumnName = "字段名称";
            pDataColumn.DataType = System.Type.GetType("System.String");
            pTable.Columns.Add(pDataColumn);
            
            //创建字段值列
            pDataColumn = new DataColumn();
            pDataColumn.ColumnName = "字段值";
            pDataColumn.DataType = System.Type.GetType("System.Object");        
            pTable.Columns.Add(pDataColumn);          
        }

        /// <summary>
        /// 初始化目录树控件
        /// </summary>
        public void InitTreeView()
        {
            try
            {
                int pDisPlayFieldIndex = -1;
                _FeatHashtable = new Hashtable();
                string sFieldValue = string.Empty;
                string sDisPlayName = string.Empty;
                TreeNode pRootNode = new TreeNode();
                TreeNode pNode = null; IFeature pFeat;
                if (_featLyr == null) return;
                pFeatTable.Rows.Clear();
                treeView1.Nodes.Clear();
                treeView1.ExpandAll();
                //添加根目录节点
                pRootNode.Text = _featLyr.Name.ToString();
                pRootNode.ExpandAll();
                treeView1.Nodes.Add(pRootNode);

                sDisPlayName = _featLyr.DisplayField;
                pDisPlayFieldIndex = _featLyr.FeatureClass.Fields.FindField(sDisPlayName);
                for (int i = 0; i < _lstFeature.Count; i++)
                {
                    pNode = new TreeNode();
                    pFeat = _lstFeature[i];
                    sFieldValue = pFeat.get_Value(pDisPlayFieldIndex).ToString();
                    pNode.Text = sFieldValue;
                    pNode.ExpandAll();
                    //添加子节点
                    pRootNode.Nodes.Add(pNode);

                    if (!_FeatHashtable.Contains(pFeat))
                    {
                        _FeatHashtable.Add(pFeat, sFieldValue);
                    }
                }
                treeView1.Refresh();
                InitGridView(pNode);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
            }
        }

        /// <summary>
        /// 初始化表格控件
        /// </summary>
        /// <param name="pNode"></param>
        private void InitGridView(TreeNode pNode)
        {
            try
            {
                DataRow pDataRow = null;
                pFeatTable.Rows.Clear();
                if (pNode != null)
                {
                    string sNodeValue = pNode.Text.ToString();
                    //遍历Feature哈希表获取选中的Feature要素
                    foreach (DictionaryEntry de in _FeatHashtable)
                    {
                        if (de.Value.ToString().ToUpper() == sNodeValue.ToUpper())
                        {
                            pFeature = de.Key as IFeature;
                            break;
                        }
                    }
                    if (pFeature != null)
                    {
                        for (int i = 0; i < pFeature.Fields.FieldCount; i++)
                        {
                            string strFieldName = pFeature.Fields.get_Field(i).Name;
                            if (strFieldName.ToUpper() == "SHAPE") continue;
                            string strFieldValue = pFeature.get_Value(i).ToString();
                            pDataRow = pFeatTable.NewRow();
                            pDataRow["字段名称"] = strFieldName;
                            pDataRow["字段值"] = strFieldValue;
                            pFeatTable.Rows.Add(pDataRow);
                        }
                    }
                    //关联属性表
                    dataGridView1.DataSource = pFeatTable;
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        //设置属性表列属性为不可排序
                        dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;

                    }
                    //设置属性表列为只读属性
                    dataGridView1.Columns[0].ReadOnly = true;
                    this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
                }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
            }
        }

        #endregion

        

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                TreeNode pFocusedNode = e.Node;
                if (pFocusedNode.Nodes.Count == 0)
                    InitGridView(pFocusedNode);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (_gisEdit == null) return;
                if (pFeature == null) return;
                _gisEdit.StartOperation();
                int pIndex = dataGridView1.CurrentCell.RowIndex;
                object sFieldValue = dataGridView1.Rows[pIndex].Cells[1].Value;
                string sFieldName = dataGridView1.Rows[pIndex].Cells[0].Value.ToString();
                pFeature.set_Value(pFeature.Fields.FindField(sFieldName), sFieldValue);
                pFeature.Store();
                _gisEdit.StopOperation("属性编辑");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
            }
        }
    }
}
