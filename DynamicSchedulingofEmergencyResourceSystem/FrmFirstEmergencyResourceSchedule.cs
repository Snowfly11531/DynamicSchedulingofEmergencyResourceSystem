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
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;
using DataManagement;
using DijkstraClass;


namespace DynamicSchedulingofEmergencyResourceSystem
{
    public partial class FrmFirstEmergencyResourceSchedule: Form
    {
        public FrmFirstEmergencyResourceSchedule()
        {
            InitializeComponent();
        }

        //打开应急资源仓库数据
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Title = "输入应急资源仓库数据";
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
        //打开道路网络数据
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Title = "输入道路网络数据";
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
                label7.Visible = false;
            }
        } 

        //打开污染物扩散模拟栅格数据
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "IMAGINE Image(*.img)|*.img";
                openFileDialog.Title = "输入污染扩散模拟栅格数据";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    comboBox3.Text = openFileDialog.FileName;
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

        //存储可供选择的首个应急处置空间位置数据
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "SHAPE shape(*.shp)|*.shp";
                saveFileDialog.Title = "输出可供选择首个应急处置空间位置数据";
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

        //进行计算时间上可行的第一个空间位置
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "")
                {
                    MessageBox.Show("请输入应急资源仓库数据", "输入应急资源仓库数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (comboBox2.Text == "")
                {
                    MessageBox.Show("请输入道路网络数据", "输入道路网络数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (comboBox3.Text == "")
                {
                    MessageBox.Show("请输入污染物扩散模拟过程数据", "输入污染物扩散模拟过程数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (textBox1.Text == "")
                {
                    MessageBox.Show("请输入应急处置工程建设时间(单位：分钟)：", "输入应急处置工程建设时间(单位：分钟)：", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (comboBox4.Text == "")
                {
                    MessageBox.Show("请输出可供选择首个应急处置空间位置", "输出可供选择首个应急处置空间位置", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else
                {
                    int row = 0, column = 0;
                    double X = 0.0, Y = 0.0;
                    double pixelvalue = 0.0;

                    IRasterLayer pRasterLayer = CDataImport.ImportRasterLayerFromControltext(comboBox3.Text);
                    IFeatureLayer pFeatureLayerDepot = CDataImport.ImportFeatureLayerFromControltext(comboBox1.Text);
                    IFeatureLayer pFeatureLayerroad = CDataImport.ImportFeatureLayerFromControltext(comboBox2.Text);
                    //获取污染物模拟河流的外包络线
                    IRaster pRaster = pRasterLayer.Raster;
                    IRasterProps pRasterprops = pRaster as IRasterProps;
                    IEnvelope pEnvelope = pRasterprops.Extent;
                    IPoint p1 = new PointClass();
                    IPoint p2 = new PointClass();
                    IPoint p3 = new PointClass();
                    IPoint p4 = new PointClass();
                    p1 = pEnvelope.UpperLeft;
                    p2 = pEnvelope.LowerLeft;
                    p3 = pEnvelope.LowerRight;
                    p4 = pEnvelope.UpperRight;
                    IPointCollection pointCollection = new PolygonClass();
                    object missing = Type.Missing;
                    pointCollection.AddPoint(p1, ref missing, ref missing);
                    pointCollection.AddPoint(p2, ref missing, ref missing);
                    pointCollection.AddPoint(p3, ref missing, ref missing);
                    pointCollection.AddPoint(p4, ref missing, ref missing);
                    pointCollection.AddPoint(p1, ref missing, ref missing);
                    IPolygon polygon = (IPolygon)pointCollection;
                    IGeometry pGeometry = new PolygonClass();
                    pGeometry = polygon as IGeometry;
                    //获取应急资源仓库到达各个节点的最短路径
                    List<IFeature> listFeature = new List<IFeature>();
                    listFeature = GetPolygonFeatureList(pFeatureLayerroad, pGeometry);
                    RoutePlanResult startNoderesult = null, EndNoderesult = null, minStartNodeResult = null, minEndNodeResult = null;
                    DijkstraDepotToAllNodeMethod path = new DijkstraDepotToAllNodeMethod();
                    path.InitializationRoadNetwork(pFeatureLayerDepot);
                    List<PlanCourse> resultplancourse = path.DepotToAllNode();

                    RasterManagement.GetRasterCount(pRasterLayer, ref row, ref column);
                    double[] desttime = new double[2];
                    int sign = 0;
                    //IFeature pFeature = null;
                    string strStartID = null, strEndID = null;
                    double time = 0.0;
                    double returnX = 0.0, returnY = 0.0;
                    double pFrompointToOutpointLength = 0.0, pOutpointToEndpointLength = 0.0;
                    IPolyline polyline = new PolylineClass();
                    //对污染物模拟栅格从[0，0]进行循环，选取从时间上作为应急处置空间位置的可行性的点
                    for (int i = 0; i < row; i++)
                    {
                        for (int j = 0; j < column; j++)
                        {
                            pixelvalue = Convert.ToDouble(RasterManagement.GetPixelValue(pRasterLayer, 0, j, i));
                            if (pixelvalue != 0)
                            {
                                IFeature pFeature = null;
                                RasterManagement.NumbercovertXY(pRasterLayer, ref X, ref Y, j, i);
                                IPoint point = new PointClass();
                                point.PutCoords(X, Y);

                                IPoint ppoint = new PointClass();
                                GetMindistanceandpoint(point, listFeature, ref returnX, ref returnY, ref pFeature, ref pFrompointToOutpointLength);
                                ppoint.PutCoords(returnX, returnY);
                                strStartID = Convert.ToString(pFeature.get_Value(pFeature.Fields.FindField("StartNodeI")));
                                strEndID = Convert.ToString(pFeature.get_Value(pFeature.Fields.FindField("EndNodeID")));
                                time = Convert.ToDouble(pFeature.get_Value(pFeature.Fields.FindField("Minutes")));
                                polyline = pFeature.Shape as IPolyline;
                                pOutpointToEndpointLength = polyline.Length - pFrompointToOutpointLength;
                                //获取应急仓库到达应急处置点最短时间的点
                                minStartNodeResult = startNoderesult = RoutePlanner.GetResult(resultplancourse[0], strStartID);
                                minEndNodeResult = EndNoderesult = RoutePlanner.GetResult(resultplancourse[0], strEndID);
                                for (int m = 1; m < resultplancourse.Count; m++)
                                {
                                    startNoderesult = RoutePlanner.GetResult(resultplancourse[m], strStartID);
                                    EndNoderesult = RoutePlanner.GetResult(resultplancourse[m], strEndID);
                                    if (minStartNodeResult.WeightValues > startNoderesult.WeightValues)
                                    {
                                        minStartNodeResult = startNoderesult;
                                    }
                                    if (minEndNodeResult.WeightValues > EndNoderesult.WeightValues)
                                    {
                                        minEndNodeResult = EndNoderesult;
                                    }
                                }
                                desttime[0] = time * (pFrompointToOutpointLength / (pFrompointToOutpointLength + pOutpointToEndpointLength));
                                desttime[1] = time - desttime[0];
                                //判断污染物到达该应急处置空间位置的时间是否大于应急资源调度时间与工程措施建设时间之和
                                if (minStartNodeResult.WeightValues + desttime[0] > minEndNodeResult.WeightValues + desttime[1])
                                {
                                    if ((minEndNodeResult.WeightValues + desttime[1] + Convert.ToDouble(textBox1.Text)) < pixelvalue)
                                    {
                                        List<IPoint> pointlist = new List<IPoint>();
                                        pointlist.Add(point);
                                        SaveVector.pointtoFeatureLayer(comboBox4.Text, pointlist, pFeatureLayerDepot);
                                        sign = 1;
                                        break;
                                    }

                                }
                                else
                                {
                                    if ((minStartNodeResult.WeightValues + desttime[0] + Convert.ToDouble(textBox1.Text)) < pixelvalue)
                                    {
                                        List<IPoint> pointlist = new List<IPoint>();
                                        pointlist.Add(point);
                                        SaveVector.pointtoFeatureLayer(comboBox4.Text, pointlist, pFeatureLayerDepot);
                                        sign = 1;
                                        break;
                                    }

                                }
                            }
                        }
                        if (sign == 1)
                        {
                            break;
                        }
                    }
                    MessageBox.Show("处理完毕！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
            
        }

        //取消该功能运行
        private void button6_Click(object sender, EventArgs e)
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

        //根据输入点和输入的线要素，获取该点距离最近的线的点、该线从起点到该点的距离以及该要素
        public void GetMindistanceandpoint(IPoint inputpoint, List<IFeature> pFeatureline, ref double X, ref double Y, ref IFeature pFeature, ref double pFrompointtoOutpointLength)
        {
            try
            {
                IPoint outpoint = new PointClass();
                double disAlongCurveFrom = 0.0;
                double disFromCurve = 0.0;
                bool isRighside = false;
                double minDistance = 0.0;

                IPolyline Startline = pFeatureline[0].Shape as IPolyline;
                Startline.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, inputpoint, false, outpoint,
                       ref disAlongCurveFrom, ref disFromCurve, ref isRighside);
                minDistance = disFromCurve;
                pFrompointtoOutpointLength = disAlongCurveFrom;
                pFeature = pFeatureline[0];
                X = outpoint.X;
                Y = outpoint.Y;
                for (int i = 1; i < pFeatureline.Count; i++)
                {
                    IPolyline ppolyline = pFeatureline[i].Shape as IPolyline;
                    ppolyline.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, inputpoint, false, outpoint,
                        ref disAlongCurveFrom, ref disFromCurve, ref isRighside);
                    if (minDistance > disFromCurve)
                    {
                        pFeature = pFeatureline[i];
                        X = outpoint.X;
                        Y = outpoint.Y;
                        minDistance = disFromCurve;
                        pFrompointtoOutpointLength = disAlongCurveFrom;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        //获取包络线范围内的路网
        public List<IFeature> GetPolygonFeatureList(IFeatureLayer pFeatureLayer, IGeometry pGeometry)
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

        //输入构建应急工程措施的时间，只能是数字
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
