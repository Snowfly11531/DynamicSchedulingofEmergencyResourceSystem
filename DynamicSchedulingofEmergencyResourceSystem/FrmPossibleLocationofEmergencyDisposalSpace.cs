using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using DataManagement;
using EmergencyResourceSchedule;
using DijkstraClass;

namespace DynamicSchedulingofEmergencyResourceSystem
{
    public partial class FrmPossibleLocationofEmergencyDisposalSpace : Form
    {
        public FrmPossibleLocationofEmergencyDisposalSpace()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Title = "请输入可供选择的首个应急处置空间位置数据：";
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
                label7.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                label7.Visible = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Title = "请输入即将污染的河流数据：";
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
                label8.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                label8.Visible = false;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "SHAPE shape(*.shp)|*.shp";
                saveFileDialog.Title = "输出可供选择的应急处置空间位置数据";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (saveFileDialog.FileName.ToString() == "")
                    {
                        MessageBox.Show("请输入图层名称！", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    }
                    else
                    {
                        comboBox6.Text = saveFileDialog.FileName.ToString();
                    }
                }
                else
                {
                    return;
                }
                label12.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                label12.Visible = false;
            }
        }

        //通过第一处置点和即将污染的水系，把第一处置点下游按照应急处置空间位置相距进行切分，获得每段的中心点
        //并判断这些点是否在可供选择的应急处置河段，则获得可供选择的应急处置空间位置
        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "")
                {
                    MessageBox.Show("请输入可供选择的首个应急处置空间位置数据", "输入可供选择的首个应急处置空间位置数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (comboBox2.Text == "")
                {
                    MessageBox.Show("请输入即将污染的河流数据", "输入即将污染的河流数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (comboBox3.Text == "")
                {
                    MessageBox.Show("请输入可选择应急处置河段数据", "输入可选择应急处置河段数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (textBox1.Text == "")
                {
                    MessageBox.Show("请输入应急处置空间位置相距的距离", "输入应急处置空间位置相距的距离", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (comboBox6.Text == "")
                {
                    MessageBox.Show("请输出应急处置空间位置的数据", "输出应急处置空间位置的数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else
                {
                    IFeatureLayer pFeatureLayerSpacepoint = CDataImport.ImportFeatureLayerFromControltext(comboBox1.Text);
                    IFeatureClass pFeatureClassSpacepoint = pFeatureLayerSpacepoint.FeatureClass;
                    IFeatureCursor pFeatureCursorSpacepoint = pFeatureClassSpacepoint.Search(null, false);
                    IFeature pFeatureSpacepoint = pFeatureCursorSpacepoint.NextFeature();
                    IPoint pFirstpoint = new PointClass();
                    while (pFeatureSpacepoint != null)
                    {
                        pFirstpoint = pFeatureSpacepoint.Shape as IPoint;
                        pFeatureSpacepoint = pFeatureCursorSpacepoint.NextFeature();
                    }

                    //根据应急处置点最短的距离，获取即将污染河流分段的各个点
                    double distance = Convert.ToDouble(textBox1.Text);
                    List<IPoint> pointlist = new List<IPoint>();
                    IFeatureLayer pFeatureLayerPollutionRiver = CDataImport.ImportFeatureLayerFromControltext(comboBox2.Text);
                    IFeatureClass pFeatureClassPollutionRiver = pFeatureLayerPollutionRiver.FeatureClass;
                    IFeatureCursor pFeatureCursorPollutionRiver = pFeatureClassPollutionRiver.Search(null, false);
                    IFeature pFeaturePollutionRiver = pFeatureCursorPollutionRiver.NextFeature();
                    while (pFeaturePollutionRiver != null)
                    {
                        IPolyline pLine = new PolylineClass();
                        pLine = pFeaturePollutionRiver.Shape as IPolyline;
                        pLine = GetPointSplitAtPoint(pLine, pFirstpoint);
                        IPolyline retureline = new PolylineClass();
                        IPoint returepoint = new PointClass();
                        for (double i = distance; i < pLine.Length; )
                        {
                            GetPointSplitAtDistance(pLine, i, ref retureline, ref returepoint);
                            pLine = retureline;
                            pointlist.Add(returepoint);                          
                        }

                        pFeaturePollutionRiver = pFeatureCursorPollutionRiver.NextFeature();
                    }

                    //SaveVector.pointtoFeatureLayer(comboBox6.Text, pointlist, pFeatureLayerPollutionRiver);

                    //根据可供选择的河段和即将污染的河流分段的点，获取可供选择的应急处置空间位置点
                    List<IPoint> pointresultlist = new List<IPoint>();
                    IFeatureLayer pFeatureLayerPossibleSelectionRiver = CDataImport.ImportFeatureLayerFromControltext(comboBox3.Text);
                    for (int j = 0; j < pointlist.Count; j++)
                    {
                        if (LineContainPoint(pointlist[j], pFeatureLayerPossibleSelectionRiver) == true)
                        {
                            pointresultlist.Add(pointlist[j]);
                        }

                    }

                    SaveVector.pointtoFeatureLayer(comboBox6.Text, pointresultlist, pFeatureLayerPossibleSelectionRiver);
                    MessageBox.Show("程序运行结束！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }    
        }
                   

        //取消该功能
        private void button8_Click(object sender, EventArgs e)
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

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Title = "请输入可选择应急处置河段数据：";
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
                label4.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                label4.Visible = false;
            }
        }

        //根据距离切割线段，获取切割的点和切割后尾段
        public void GetPointSplitAtDistance(IPolyline InputLine, double distance, ref IPolyline retureLine, ref IPoint returePoint)
        {
            try
            {
                IPolyline[] pLines = new IPolyline[2];
                IPolyline pSplitLine = new PolylineClass();
                pSplitLine = InputLine;
                IPolyline StarttoPointLine = new PolylineClass();
                IPolyline EndtoPointLine = new PolylineClass();
                bool splithappened;
                int partindex, segmentindex;
                object pObject = Type.Missing;
                pSplitLine.SplitAtDistance(distance, false, false, out splithappened, out partindex, out segmentindex);
                ISegmentCollection lineSegCol = pSplitLine as ISegmentCollection;
                ISegmentCollection startSegCol = StarttoPointLine as ISegmentCollection;
                ISegmentCollection endSegCol = EndtoPointLine as ISegmentCollection;
                for (int i = 0; i < segmentindex; i++)
                {
                    startSegCol.AddSegment(lineSegCol.get_Segment(i), ref pObject, ref pObject);
                }
                for (int j = segmentindex; j < lineSegCol.SegmentCount; j++)
                {
                    endSegCol.AddSegment(lineSegCol.get_Segment(j), ref pObject, ref pObject);
                }
                pLines[0] = endSegCol as IPolyline;
                pLines[1] = startSegCol as IPolyline;
                returePoint = new PointClass();
                bool asRatio = false;
                pLines[1].QueryPoint(esriSegmentExtension.esriNoExtension, pLines[1].Length / 2, asRatio, returePoint);
                retureLine = pLines[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        //根据第一应急处置空间位置点，将即将污染的河流切割成两段
        public IPolyline GetPointSplitAtPoint(IPolyline InputLine, IPoint point)
        {
            try
            {
                IPolyline[] pLines = new IPolyline[2];
                IPolyline pSplitLine = new PolylineClass();
                pSplitLine = InputLine;
                IPolyline StarttoPointLine = new PolylineClass();
                IPolyline EndtoPointLine = new PolylineClass();
                bool projectOnto=true, createPart=false;
                bool SplitHappened;
                int partindex, segmentindex;
                object pObject = Type.Missing;
                pSplitLine.SplitAtPoint(point,projectOnto,createPart,out SplitHappened,out partindex,out segmentindex);
                ISegmentCollection lineSegCol = pSplitLine as ISegmentCollection;
                ISegmentCollection startSegCol = StarttoPointLine as ISegmentCollection;
                ISegmentCollection endSegCol = EndtoPointLine as ISegmentCollection;
                for (int i = 0; i < segmentindex; i++)
                {
                    startSegCol.AddSegment(lineSegCol.get_Segment(i), ref pObject, ref pObject);
                }
                for (int j = segmentindex; j < lineSegCol.SegmentCount; j++)
                {
                    endSegCol.AddSegment(lineSegCol.get_Segment(j), ref pObject, ref pObject);
                }
                pLines[0] = endSegCol as IPolyline;
                pLines[1] = startSegCol as IPolyline;
                return pLines[0]; 
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                return null;
            }
        }
       

        //判断点是否在线上，主要是获取点距离线最短的点，两点是否重叠
        public bool LineContainPoint(IPoint point, IFeatureLayer pInputFeatureLayer)
        {
            try
            {
                bool returebool = false;
                IProximityOperator pProximityOperator = point as IProximityOperator;
                IFeatureLayer pFeatureLayer = pInputFeatureLayer;
                IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
                IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);
                IFeature pFeature = pFeatureCursor.NextFeature();
                while (pFeature != null)
                {
                    double distance = Math.Round(pProximityOperator.ReturnDistance(pFeature.Shape as IGeometry), 2);
                    if (distance == 0.0)
                    {
                        returebool = true;
                        break;
                    }
                    pFeature = pFeatureCursor.NextFeature();

                }
                return returebool;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                return false;
            }
            
           
        }

        //输入应急处置空间位置相距的距离只能是数字，不能是字母等字符
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                e.Handled = true;
                // 按下的为数字键、回车键或退格键（backspace）时允许输入
                if (Char.IsNumber(e.KeyChar) || e.KeyChar == (Char)13 || e.KeyChar == (Char)8)
                    e.Handled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        

       

       

    }
}
