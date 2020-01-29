using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RiverClass;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.DataSourcesFile;
using DataManagement;


namespace DynamicSchedulingofEmergencyResourceSystem
{
    public partial class FrmDownStream : Form
    {
        public FrmDownStream()
        {
            InitializeComponent();
        }

        //打开污染发现的空间位置
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Title = "输入污染发现点数据";
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
                label4.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                label4.Visible = false;
            }
        }

        //输入河流网络数据
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Title = "输入河流网络数据";
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
                label5.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                label5.Visible = false;
            }
        }

        //输出污染点发现点下溯河流网络
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "SHAPE shape(*.shp)|*.shp";
                saveFileDialog.Title = "输出污染发现点下游河流网络";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (saveFileDialog.FileName.ToString() == "")
                    {
                        MessageBox.Show("请输入图层名称！", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    }
                    else
                    {
                        comboBox3.Text = saveFileDialog.FileName.ToString();
                    }
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

        //计算污染返现点下游水系
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "")
                {
                    MessageBox.Show("请输入污染发现点数据", "输入污染发现点数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (comboBox2.Text == "")
                {
                    MessageBox.Show("请输入河流网络数据", "输入河流网络数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (comboBox3.Text == "")
                {
                    MessageBox.Show("请选择输出污染发现点下游水系数据", "输出污染发现点下游水系数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else
                {
                    IFeatureLayer pFeatureLayerPollutionpoint = CDataImport.ImportFeatureLayerFromControltext(comboBox1.Text);
                    IFeatureLayer pFeatureLayerRivernetwork = CDataImport.ImportFeatureLayerFromControltext(comboBox2.Text);
                    RiverDownstream riverpath = new RiverDownstream();
                    List<IPolyline> listlines = new List<IPolyline>();
                    listlines = riverpath.pollutionpointdownstreamriver(pFeatureLayerPollutionpoint, pFeatureLayerRivernetwork);
                    SaveVector.polylinetoFeatureLayer(comboBox3.Text, listlines, pFeatureLayerRivernetwork);
                    MessageBox.Show("处理完毕！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
           
        }

        //取消该功能运行
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        
    }
}
