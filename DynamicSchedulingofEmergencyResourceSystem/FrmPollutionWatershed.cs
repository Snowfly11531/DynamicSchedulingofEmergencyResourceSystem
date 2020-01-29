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
using DataManagement;

namespace DynamicSchedulingofEmergencyResourceSystem
{
    public partial class FrmPollutionWatershed : Form
    {
        public FrmPollutionWatershed()
        {
            InitializeComponent();
        }

     

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Title = "输入污染发现点上游水系数据";
                openDialog.Filter = "SHAPE shape(*.shp)|*.shp";
                openDialog.Multiselect = false;
                openDialog.RestoreDirectory = false;
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    comboBox2.Text = openDialog.FileName.ToString();
                }
                else
                {
                    return;
                }
                label6.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                label6.Visible = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try 
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Title = "输入子流域数据";
                openDialog.Filter = "SHAPE shape(*.shp)|*.shp";
                openDialog.Multiselect = false;
                openDialog.RestoreDirectory = false;
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    comboBox3.Text = openDialog.FileName.ToString();
                }
                else
                {
                    return;
                }
                label7.Visible = true;            
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                label7.Visible = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "SHAPE shape(*.shp)|*.shp";
                saveFileDialog.Title = "输出可能污染流域范围数据";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (saveFileDialog.FileName.ToString() == "")
                    {
                        MessageBox.Show("请输入图层名称！", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    }
                    else
                    {
                        comboBox4.Text = saveFileDialog.FileName.ToString();
                    }
                }
                else
                {
                    return;
                }
                label8.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                label8.Visible = false;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox2.Text == "")
                {
                    MessageBox.Show("请输入污染发现点上游水系数据", "输入污染源发现点上游水系数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (comboBox3.Text == "")
                {
                    MessageBox.Show("请输入子流域数据", "输入子流域数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (comboBox4.Text == "")
                {
                    MessageBox.Show("请输出可能发生污染的流域范围数据", "输出可能发生污染的流域范围数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else
                {
                    //将可能已发生污染的水系合并成一个要素
                    IFeatureLayer pFeatureLayerline = CDataImport.ImportFeatureLayerFromControltext(comboBox2.Text);
                    IPolyline polyline = new PolylineClass();
                    polyline = LineUnion(pFeatureLayerline);
                    IGeometry pGeometry = polyline as IGeometry;
                    //根据可能已发生的污染水系查找其子流域
                    IFeatureLayer pFeatureLayerpolygon = CDataImport.ImportFeatureLayerFromControltext(comboBox3.Text);
                    List<IFeature> pFeaturelist = new List<IFeature>();
                    pFeaturelist = GetLineOverlapPolygon(pFeatureLayerpolygon, pGeometry);
                    SaveVector.polygontoFeatureLayer(comboBox4.Text, pFeaturelist, pFeatureLayerline);
                    
                    MessageBox.Show("处理完毕！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "/n" + ex.ToString(), "异常");
            }                  
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //图层的线要素的合并
        public IPolyline LineUnion(IFeatureLayer pFeatureLayer)
        {
            try
            {
                ITopologicalOperator pTopologicalOperator;
                IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
                IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);
                IFeature pFeatureTemp = pFeatureCursor.NextFeature();
                IGeometry pGeometry = null;
                if (pFeatureTemp != null)
                {
                    pGeometry = pFeatureTemp.Shape;
                    pFeatureTemp = pFeatureCursor.NextFeature();
                }
                else
                {
                    return null;
                }
                while (pFeatureTemp != null)
                {
                    pTopologicalOperator = pGeometry as ITopologicalOperator;
                    pGeometry = pFeatureTemp.Shape;
                    pGeometry = pTopologicalOperator.Union(pGeometry as IGeometry);
                    pFeatureTemp = pFeatureCursor.NextFeature();
                }
                IPolyline polyline = pGeometry as IPolyline;
                return polyline;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "/n" + ex.ToString(), "异常");
                return null;
            }
        }

        //获取线要素覆盖的面要素
        public List<IFeature> GetLineOverlapPolygon(IFeatureLayer pFeatureLayer, IGeometry pGeometry)
        {
            try
            {
                List<IFeature> listFeature = new List<IFeature>();
                ISpatialFilter spatialFiter = new SpatialFilterClass();
                spatialFiter.Geometry = pGeometry;
                spatialFiter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                IFeatureCursor featureCursor = pFeatureLayer.Search(spatialFiter, false);
                IFeature pFeature;
                while ((pFeature = featureCursor.NextFeature()) != null)
                {
                    listFeature.Add(pFeature);
                }
                return listFeature;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                return null;
            }
        }
    }
}
