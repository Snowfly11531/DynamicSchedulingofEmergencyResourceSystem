﻿using System;
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
    public partial class FrmPossiblePollutionsource : Form
    {
        public FrmPossiblePollutionsource()
        {
            InitializeComponent();
        }

        //打开污染源数据库数据
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Title = "输入污染源数据库数据";
                openDialog.Filter = "SHAPE shape(*.shp)|*.shp";
                openDialog.Multiselect = false;
                openDialog.RestoreDirectory = false;
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    comboBox1.Text = openDialog.FileName.ToString();
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
                label6.Visible = false;
            }
        }

        //打开污染发现点上游水系数据
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
                label7.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                label7.Visible = true;
            }
            
        }

        //打开子流域数据
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
                label8.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                label8.Visible = false;
            }
        }

        //输出可能排放污染物的空间位置点数据
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "SHAPE shape(*.shp)|*.shp";
                saveFileDialog.Title = "输出可能排放污染的企业数据";
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
                label9.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                label9.Visible = false;
            }
        }

        //计算污染物排放的空间位置
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "")
                {
                    MessageBox.Show("请输入污染源数据库数据", "输入污染源数据库数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (comboBox2.Text == "")
                {
                    MessageBox.Show("请输入污染发现点上游水系数据", "输入污染源发现点上游水系数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (comboBox3.Text == "")
                {
                    MessageBox.Show("请输入子流域数据", "输入子流域数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (textBox1.Text == "")
                {
                    MessageBox.Show("请输入污染发现点所发生的污染类型", "输入污染发现点所发生的污染类型", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (comboBox4.Text == "")
                {
                    MessageBox.Show("请输出可能排放污染物的空间位置数据", "输出可能排放污染物的空间位置数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
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
                    IPolygon polygon = PolygonUnion(pFeaturelist);
                    //根据肯能已发生污染的子流域查找其可能引起的污染源的空间位置
                    IFeatureLayer pFeatureLayerpoint = CDataImport.ImportFeatureLayerFromControltext(comboBox1.Text);
                    IGeometry pGeometrypolygon = polygon as IGeometry;
                    List<IFeature> pFeatureListpoint = new List<IFeature>();
                    pFeatureListpoint = GetPolygonContainPoint(pFeatureLayerpoint, pGeometrypolygon, textBox1.Text);
                    SaveVector.pointtoFeatureLayer(comboBox4.Text, pFeatureListpoint, pFeatureLayerpoint);
                    MessageBox.Show("处理完毕！"); 
                }       
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "/n" + ex.ToString(), "异常");
            }                  
        }

        //取消该功能运行
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

        //图层的面要素的合并
        public IPolygon PolygonUnion(List<IFeature> pFeatureList)
        {
            try
            {
                ITopologicalOperator pTopologicalOperator;
                IFeature pFeatureTemp = pFeatureList[0];
                IGeometry pGeometry = null;
                if (pFeatureTemp != null)
                {
                    pGeometry = pFeatureTemp.Shape;
                }
                else
                {
                    return null;
                }
                for (int i = 1; i < pFeatureList.Count; i++)
                {
                    pFeatureTemp = pFeatureList[i];
                    pTopologicalOperator = pGeometry as ITopologicalOperator;
                    pGeometry = pFeatureTemp.Shape;
                    pGeometry = pTopologicalOperator.Union(pGeometry as IGeometry);
                }    
                IPolygon polygon = pGeometry as IPolygon;
                return polygon;
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

        //获取污染物名称并且在指定的pGeometry范围内的污染排放数据库的空间位置
        public List<IFeature> GetPolygonContainPoint(IFeatureLayer pFeatureLayer, IGeometry pGeometry,string pollutiontypename)
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
                    string strName=Convert.ToString(pFeature.get_Value(pFeature.Fields.FindField("污染物")));
                    if (strName == pollutiontypename)
                    {
                        listFeature.Add(pFeature);
                    }
                    
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
