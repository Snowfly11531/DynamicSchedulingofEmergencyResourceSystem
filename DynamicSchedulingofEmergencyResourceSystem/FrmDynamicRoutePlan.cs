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
using Agent;
using ESRI.ArcGIS.SystemUI;

namespace DynamicSchedulingofEmergencyResourceSystem
{
    public partial class FrmDynamicRoutePlan : Form
    {

        //IPolyline[] OneRouteline = new IPolyline[2];



        //Timer pTimer = new Timer();
        List<EmergencyVehicle> pEmergencyVehicleAgent = new List<EmergencyVehicle>();
        List<RoadNetwork> pRoadAgent = new List<RoadNetwork>();
        List<OrdinaryVehicle> pOrdinaryVehicleAgent = new List<OrdinaryVehicle>();

        List<List<VehicleRouteResult>> RouteResult = null;
        int VehicleCount = 0;
        int[] ProgressCount = { };


        public FrmDynamicRoutePlan()
        {
            
            InitializeComponent();
            timer1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Title = "输入输送应急资源仓库数据";
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
                label2.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                label2.Visible = false;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Title = "输入应急处置空间位置数据";
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
                label4.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                label4.Visible = false;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "")
                {
                    MessageBox.Show("请输入应急资源仓库数据", "输入应急资源仓库数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else if (comboBox2.Text == "")
                {
                    MessageBox.Show("请输入应急处置空间位置数据", "输入应急处置空间位置数据", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                }
                else
                {
                    IFeatureLayer pFeatureLayerDepot = CDataImport.ImportFeatureLayerFromControltext(comboBox1.Text);

                    List<IPoint> Depotpoint = new List<IPoint>();
                    IFeatureClass pFeatureClassDepot = pFeatureLayerDepot.FeatureClass;
                    IFeatureCursor pFeatureCursorDepot = pFeatureClassDepot.Search(null, false);
                    IFeature pFeatureDepot = pFeatureCursorDepot.NextFeature();
                    while (pFeatureDepot != null)
                    {
                        Depotpoint.Add(pFeatureDepot.Shape as IPoint);
                        pFeatureDepot = pFeatureCursorDepot.NextFeature();
                    }

                    IFeatureLayer pFeatureLayerDept = CDataImport.ImportFeatureLayerFromControltext(comboBox2.Text);

                    List<IPoint> Deptpoint = new List<IPoint>();
                    IFeatureClass pFeatureClassDept = pFeatureLayerDept.FeatureClass;
                    IFeatureCursor pFeatureCursorDept = pFeatureClassDept.Search(null, false);
                    IFeature pFeatureDept = pFeatureCursorDept.NextFeature();
                    while (pFeatureDept != null)
                    {
                        Deptpoint.Add(pFeatureDept.Shape as IPoint);
                        pFeatureDept = pFeatureCursorDept.NextFeature();
                    }

                    //获取应急资源仓库到达应急处置空间位置的路径
                    DijkstraClass.DynamicRoutePlanDijkstra pDynamicRoutePlan = new DijkstraClass.DynamicRoutePlanDijkstra();
                    List<Result> resultlist = new List<Result>();
                    resultlist = pDynamicRoutePlan.DepotToDest(pFeatureLayerDepot, pFeatureLayerDept, pRoadAgent);

                    //将路网初始化成IPolyline
                    Collection<RouteNode> routeNodes = new Collection<RouteNode>();//存储某起点和终点最短路径网络所经过的结点
                    List<IPolyline> routeroad = new List<IPolyline>();
                    RouteResult = new List<List<VehicleRouteResult>>();
                    for (int i = 0; i < resultlist.Count; i++)
                    {
                        foreach (string str in resultlist[i].StrResultNode)
                        {
                            DynamicRoutePlanDijkstra.addRouteNodes(routeNodes, str);
                        }
                        DynamicRoutePlanDijkstra.addRouteNodes(routeNodes, resultlist[i].EndNodeID);
                        List<VehicleRouteResult> ListVehicleRouteResult = new List<VehicleRouteResult>();
                        ListVehicleRouteResult = pDynamicRoutePlan.GetVehicleRoad(routeNodes, Depotpoint[i], Deptpoint[0]);
                        RouteResult.Add(ListVehicleRouteResult);
                        ListVehicleRouteResult = null;

                        //显示路线在主页面上
                        pDynamicRoutePlan.GetIElement(routeNodes);
                        routeroad.Add(Variable.PElement.Geometry as IPolyline);

                        routeNodes.Clear();
                    }
                    //显示路线在主页面上
                    DynamicRoutePlanDijkstra.GetIElement(routeroad);
                    DynamicRoutePlanDijkstra.displayElement();


                    //道路智能体的初始化



                    //进行车辆智能体的运行
                    for (int k = 0; k < Depotpoint.Count; k++)
                    {
                        EmergencyVehicle emergencyVehicle = new EmergencyVehicle();
                        emergencyVehicle.OriginationPoint = Depotpoint[k];
                        emergencyVehicle.DestinationPoint = Deptpoint[0];
                        emergencyVehicle.PlanRouteResult = RouteResult[k];
                        pEmergencyVehicleAgent.Add(emergencyVehicle);
                    }

                    VehicleCount = pEmergencyVehicleAgent.Count;
                    ProgressCount = new int[VehicleCount];
                    
                    //初始化车辆所在的路网中
                    for (int j = 0; j < VehicleCount; j++)
                    {
                        ProgressCount[j] = 0;
                    }

                    //获取车辆当前所在路网的路段
                    for (int a = 0; a < VehicleCount; a++)
                    {
                        pEmergencyVehicleAgent[a].RunVehicleLine = pEmergencyVehicleAgent[a].PlanRouteResult[ProgressCount[a]].pLine;
                        pEmergencyVehicleAgent[a].CurrentLine = pEmergencyVehicleAgent[a].PlanRouteResult[ProgressCount[a]].pLine;
                        pEmergencyVehicleAgent[a].CurrentOrigination = pEmergencyVehicleAgent[a].PlanRouteResult[ProgressCount[a]].Startpoint;
                        pEmergencyVehicleAgent[a].CurrentDestination = pEmergencyVehicleAgent[a].PlanRouteResult[ProgressCount[a]].Endpoint;
                        pEmergencyVehicleAgent[a].Distance = 1550.5;
                      

                    }

                    //获取车辆所在的路网的的点
                    for (int b = 0; b < VehicleCount; b++)
                    {

                        if (pEmergencyVehicleAgent[b].CurrentLine.FromPoint.X == pEmergencyVehicleAgent[b].CurrentOrigination.X && pEmergencyVehicleAgent[b].CurrentLine.FromPoint.Y == pEmergencyVehicleAgent[b].CurrentOrigination.Y)
                        {
                            pEmergencyVehicleAgent[b].CurrentPoint = pEmergencyVehicleAgent[b].CurrentLine.FromPoint;
                        }
                        else
                        {
                            pEmergencyVehicleAgent[b].CurrentPoint = pEmergencyVehicleAgent[b].CurrentLine.ToPoint;
                        }
                    }
                    for (int c = 0; c < VehicleCount; c++)
                    {
                        DrawPointElement(pEmergencyVehicleAgent[c].CurrentPoint);
                    }


                    timer1.Enabled = true;
                    timer1.Interval = 1500;
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
           

           

        }



       

        
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < pEmergencyVehicleAgent.Count; i++)
                {
                    //如果路网有障碍，则重新规划路线


                    if (pEmergencyVehicleAgent[i].RunVehicleLine.Length >= pEmergencyVehicleAgent[i].Distance)
                    {
                        if (pEmergencyVehicleAgent[i].CurrentLine.FromPoint.X == pEmergencyVehicleAgent[i].CurrentOrigination.X && pEmergencyVehicleAgent[i].CurrentLine.FromPoint.Y == pEmergencyVehicleAgent[i].CurrentOrigination.Y)
                        {
                            GetFromPointMovepointChangeLocation(pEmergencyVehicleAgent[i].RunVehicleLine, pEmergencyVehicleAgent[i].Distance, ref pEmergencyVehicleAgent[i].CurrentPoint, ref pEmergencyVehicleAgent[i].RetureLine);
                        }
                        else
                        {
                            GetToPointMovepointChangeLocation(pEmergencyVehicleAgent[i].RunVehicleLine, pEmergencyVehicleAgent[i].Distance, ref pEmergencyVehicleAgent[i].CurrentPoint, ref pEmergencyVehicleAgent[i].RetureLine);
                        }
                        pEmergencyVehicleAgent[i].Distance = 1550.5;
                        MovePointUpdateElement(pEmergencyVehicleAgent[i].CurrentPoint);
                        pEmergencyVehicleAgent[i].RunVehicleLine = pEmergencyVehicleAgent[i].RetureLine;

                    }
                    else
                    {
                        pEmergencyVehicleAgent[i].Distance -= pEmergencyVehicleAgent[i].RunVehicleLine.Length;
                        ProgressCount[i]++;
                        if (ProgressCount[i] == pEmergencyVehicleAgent[i].PlanRouteResult.Count)
                        {
                            timer1.Stop();
                        }
                        if (ProgressCount[i] < pEmergencyVehicleAgent[i].PlanRouteResult.Count)
                        {
                            pEmergencyVehicleAgent[i].CurrentLine = pEmergencyVehicleAgent[i].PlanRouteResult[ProgressCount[i]].pLine;
                            pEmergencyVehicleAgent[i].RunVehicleLine = pEmergencyVehicleAgent[i].PlanRouteResult[ProgressCount[i]].pLine;
                            pEmergencyVehicleAgent[i].CurrentOrigination = pEmergencyVehicleAgent[i].PlanRouteResult[ProgressCount[i]].Startpoint;
                            pEmergencyVehicleAgent[i].CurrentDestination = pEmergencyVehicleAgent[i].PlanRouteResult[ProgressCount[i]].Endpoint;
                            if (pEmergencyVehicleAgent[i].RunVehicleLine.Length >= pEmergencyVehicleAgent[i].Distance)
                            {
                                if (pEmergencyVehicleAgent[i].CurrentLine.FromPoint.X == pEmergencyVehicleAgent[i].CurrentOrigination.X && pEmergencyVehicleAgent[i].CurrentLine.FromPoint.Y == pEmergencyVehicleAgent[i].CurrentOrigination.Y)
                                {
                                    GetFromPointMovepointChangeLocation(pEmergencyVehicleAgent[i].RunVehicleLine, pEmergencyVehicleAgent[i].Distance, ref pEmergencyVehicleAgent[i].CurrentPoint, ref pEmergencyVehicleAgent[i].RetureLine);
                                }
                                else
                                {
                                    GetToPointMovepointChangeLocation(pEmergencyVehicleAgent[i].RunVehicleLine, pEmergencyVehicleAgent[i].Distance, ref pEmergencyVehicleAgent[i].CurrentPoint, ref pEmergencyVehicleAgent[i].RetureLine);
                                }
                                   
                                pEmergencyVehicleAgent[i].Distance = 1550.5;
                                MovePointUpdateElement(pEmergencyVehicleAgent[i].CurrentPoint);
                                pEmergencyVehicleAgent[i].RunVehicleLine = pEmergencyVehicleAgent[i].RetureLine;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            } 
        }


        private void timer2_Tick(object sender, EventArgs e)
        {

        }



        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "/n" + ex.ToString(), "异常");
            }
        }


      


        private void DrawPointElement(IPoint point)
        {
            try
            {
                IGraphicsContainer pGraphicsContainer = Variable.pMapFrm.mainMapControl.ActiveView as IGraphicsContainer;
                //pGraphicsContainer.DeleteAllElements();
                //IMarkerElement用来获取Symbol属性
                IMarkerElement pMarkerElement = new MarkerElementClass();
                //ISimpleMarkerSymbol来设置点的属性
                ISimpleMarkerSymbol pSymbol = new SimpleMarkerSymbolClass();
                IRgbColor pRGBcolor = new RgbColorClass();
                pRGBcolor.Red = 255;
                pRGBcolor.Green = 0;
                pRGBcolor.Blue = 0;
                IPictureMarkerSymbol pms = new PictureMarkerSymbolClass();
                pms.BitmapTransparencyColor = pRGBcolor;
                pms.CreateMarkerSymbolFromFile(esriIPictureType.esriIPicturePNG, @"C:\Users\Administrator\Desktop\突发环境事件应急资源调度系统\DynamicSchedulingofEmergencyResourceSystem\DynamicSchedulingofEmergencyResourceSystem\Resources\vehicle.png");
                pms.Size = 6;
                pMarkerElement.Symbol = pms as IMarkerSymbol;

                //pSymbol.Color = pRGBcolor;
                //pMarkerElement.Symbol = pSymbol;
                Variable.PElement = pMarkerElement as IElement;
                Variable.PElement.Geometry = point;
                pGraphicsContainer.AddElement(Variable.PElement, 0);
                Variable.pMapFrm.mainMapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                Variable.pMapFrm.mainMapControl.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
           
        }

        //更新车辆的位置
        private void MovePointUpdateElement(IPoint point)
        {
            try
            {
                IGraphicsContainer pGraphicsContainer = Variable.pMapFrm.mainMapControl.ActiveView as IGraphicsContainer;
                //pGraphicsContainer.DeleteAllElements();
                IMarkerElement pMarkerElement = new MarkerElementClass();
                //ISimpleMarkerSymbol来设置点的属性
                ISimpleMarkerSymbol pSymbol = new SimpleMarkerSymbolClass();
                IRgbColor pRGBcolor = new RgbColorClass();
                pRGBcolor.Red = 255;
                pRGBcolor.Green = 0;
                pRGBcolor.Blue = 0;

                IPictureMarkerSymbol pms = new PictureMarkerSymbolClass();
                pms.BitmapTransparencyColor = pRGBcolor;
                pms.CreateMarkerSymbolFromFile(esriIPictureType.esriIPicturePNG, @"C:\Users\Administrator\Desktop\突发环境事件应急资源调度系统\DynamicSchedulingofEmergencyResourceSystem\DynamicSchedulingofEmergencyResourceSystem\Resources\vehicle.png");
                pms.Size = 6;
                pMarkerElement.Symbol = pms as IMarkerSymbol;

                //pSymbol.Color = pRGBcolor;
                //pMarkerElement.Symbol = pSymbol;
                Variable.PElement = pMarkerElement as IElement;
                Variable.PElement.Geometry = point;
                pGraphicsContainer.AddElement(Variable.PElement, 0);
                //pGraphicsContainer.UpdateElement(Variable.PElement);
                //Variable.pMapFrm.mainMapControl.CenterAt(point);
                Variable.pMapFrm.mainMapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                //Variable.pMapFrm.mainMapControl.Refresh();             
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
                    

        }

        //根据线的方向获取其更新的位置
        private void GetFromPointMovepointChangeLocation(IPolyline line, double distance, ref IPoint point, ref IPolyline retureline)
        {
            try
            {
                IPolyline[] pLines = new IPolyline[2];
                IPolyline pSplitLine = new PolylineClass();
                pSplitLine = line;
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
                point = new PointClass();
                point = pLines[0].FromPoint;
                retureline = pLines[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "/n" + ex.ToString(), "异常");
            } 
        }

        //获得车辆运动的下一个点
        private void GetToPointMovepointChangeLocation(IPolyline line, double distance, ref IPoint point, ref IPolyline retureline)
        {
            try
            {
                distance = line.Length - distance;
                IPolyline[] pLines = new IPolyline[2];
                IPolyline pSplitLine = new PolylineClass();
                pSplitLine = line;
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
                point = new PointClass();
                point = pLines[0].FromPoint;
                retureline = pLines[1];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "/n" + ex.ToString(), "异常");
            }
        }







        private void button5_Click(object sender, EventArgs e)
        {
            FrmEmergencyVehicleInformation pEmergencyVehicleInformation = new FrmEmergencyVehicleInformation();
            pEmergencyVehicleInformation.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            InitializeOrdinaryVehicleAgent(10);
            InitializeRoadAgent();
            OrdinaryVehiclePlanRoute();
            MessageBox.Show("程序运行结束！");
        }




      

        //初始化普通车辆智能体
        //随机产生起点、终点构建车辆智能体
        public void InitializeOrdinaryVehicleAgent(int OrdinaryVehicleNumber)
        {
            IFeatureLayer pFeatureLayer = CDataImport.ImportFeatureLayerFromControltext(@"C:\Users\Administrator\Desktop\突发环境事件应急资源调度系统\data\point.shp");
            IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
            int pointNumber = pFeatureClass.FeatureCount(null);
            Random random = new Random();
            
            for (int i = 0; i < OrdinaryVehicleNumber; i++)
            {
                OrdinaryVehicle pOrdinaryVehicle = new OrdinaryVehicle();
                pOrdinaryVehicle.ID = i + 1;
                pOrdinaryVehicle.Origination = random.Next(1, pointNumber);
                pOrdinaryVehicle.Destination = random.Next(1, pointNumber);
                if (pOrdinaryVehicle.Origination != pOrdinaryVehicle.Destination)
                {
                    IQueryFilter pQueryFilterOrigination = new QueryFilterClass();
                    pQueryFilterOrigination.WhereClause = "OBJECTID=" + "'" + pOrdinaryVehicle.Origination + "'";
                    IFeatureCursor pFeatureCursorOrigination = pFeatureClass.Search(pQueryFilterOrigination, false);
                    IFeature pFeatureOrigination = pFeatureCursorOrigination.NextFeature();
                    while (pFeatureOrigination != null)
                    {
                        pOrdinaryVehicle.OriginationPoint = pFeatureOrigination.Shape as IPoint;
                        pFeatureOrigination = pFeatureCursorOrigination.NextFeature();
                    }
                    IQueryFilter pQueryFilterDestination = new QueryFilterClass();
                    pQueryFilterDestination.WhereClause = "OBJECTID=" + "'" + pOrdinaryVehicle.Destination + "'";
                    IFeatureCursor pFeatureCursorDestination = pFeatureClass.Search(pQueryFilterDestination, false);
                    IFeature pFeatureDestination = pFeatureCursorDestination.NextFeature();
                    while (pFeatureDestination != null)
                    {
                        pOrdinaryVehicle.DestinationPoint = pFeatureDestination.Shape as IPoint;
                        pFeatureDestination = pFeatureCursorDestination.NextFeature();
                    }
                    pOrdinaryVehicleAgent.Add(pOrdinaryVehicle);
                }
                
            }
        }


        //初始化道路智能体，获取每个道路智能体的车辆数以及ID、起点、终点、道路等级的信息
        public void InitializeRoadAgent()
        {
            //设置道路智能体属性
            IFeatureLayer pFeatureLayer = CDataImport.ImportFeatureLayerFromControltext(@"C:\Users\Administrator\Desktop\突发环境事件应急资源调度系统\data\road.shp");
            IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
            IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);
            IFeature pFeature = pFeatureCursor.NextFeature();
            while (pFeature != null)
            {
                RoadNetwork pRoadNetwork = new RoadNetwork();
                pRoadNetwork.ID = Convert.ToInt32(pFeature.get_Value(pFeature.Fields.FindField("OBJECTID")));
                pRoadNetwork.StartNodeID = Convert.ToString(pFeature.get_Value(pFeature.Fields.FindField("StartNodeI")));
                pRoadNetwork.EndNodeID = Convert.ToString(pFeature.get_Value(pFeature.Fields.FindField("EndNodeID")));
                pRoadNetwork.Rank = Convert.ToString(pFeature.get_Value(pFeature.Fields.FindField("Rank")));
                pRoadNetwork.Length = Convert.ToDouble((pFeature.Shape as IPolyline).Length);
                pRoadAgent.Add(pRoadNetwork);
                pFeature = pFeatureCursor.NextFeature();
            }

            //初始化道路智能体行驶的车辆数
            for (int i = 0; i < pOrdinaryVehicleAgent.Count; i++)
            {
                pRoadAgent[pOrdinaryVehicleAgent[i].Origination].VehicleNumber++;
            }
        }


        //规划车辆智能体行驶的道路
        private void OrdinaryVehiclePlanRoute()
        {
            List<Result> pResultList = new List<Result>();
            OrdinaryVehicleDijkstra pOrdinaryVehicleDijkstra = new OrdinaryVehicleDijkstra();
            pResultList = pOrdinaryVehicleDijkstra.DepotToDest(pOrdinaryVehicleAgent,pRoadAgent);
            //将路网初始化成IPolyline
            Collection<RouteNode> routeNodes = new Collection<RouteNode>();//存储某起点和终点最短路径网络所经过的结点
            List<IPolyline> routeroad = new List<IPolyline>();
            RouteResult = new List<List<VehicleRouteResult>>();
            for (int j = 0; j < pResultList.Count; j++)
            {
                foreach (string str in pResultList[j].StrResultNode)
                {
                    DynamicRoutePlanDijkstra.addRouteNodes(routeNodes, str);
                }
                DynamicRoutePlanDijkstra.addRouteNodes(routeNodes, pResultList[j].EndNodeID);
                List<VehicleRouteResult> ListVehicleRouteResult = new List<VehicleRouteResult>();
                ListVehicleRouteResult = pOrdinaryVehicleDijkstra.GetVehicleRoad(routeNodes, pOrdinaryVehicleAgent[j].OriginationPoint, pOrdinaryVehicleAgent[j].DestinationPoint);
                RouteResult.Add(ListVehicleRouteResult);
                ListVehicleRouteResult = null;
                routeNodes.Clear();
            }

            for (int k = 0; k < pOrdinaryVehicleAgent.Count; k++)
            {
                pOrdinaryVehicleAgent[k].PlanRouteResult = RouteResult[k];
            }
        }

        //路网智能体变化，当车辆到达路口，前段和接下来行走的路线车辆数量发生变化
        //路网是按照ID号顺序排序的
        public void RoadVehicleNumber(int CurrentLineID,int NextLineID)
        {
            try
            {
                pRoadAgent[CurrentLineID - 1].VehicleNumber--;
                pRoadAgent[NextLineID - 1].VehicleNumber++;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }




         //查找最近某点最近的线
        public IFeature GetNearestFeature(IPoint pPoint)
        {
            try
            {
                double minDistince, Distiance;
                IFeature pFeatureNearst, pFeature;
                IProximityOperator pProximityOperator = pPoint as IProximityOperator;
                IFeatureLayer pFeatureLayer = CDataImport.ImportFeatureLayerFromControltext(@"C:\Users\Administrator\Desktop\突发环境事件应急资源调度系统\data\road.shp");
                IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
                IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);
                pFeature = pFeatureNearst = pFeatureCursor.NextFeature();

                if (pFeature == null)
                    return null;
                minDistince = Distiance = pProximityOperator.ReturnDistance(pFeature.Shape as IGeometry);
                pFeature = pFeatureCursor.NextFeature();
                while (pFeature != null)
                {
                    Distiance = pProximityOperator.ReturnDistance(pFeature.Shape as IGeometry);
                    if (minDistince > Distiance)
                    {
                        minDistince = Distiance;
                        pFeatureNearst = pFeature;
                    }
                    pFeature = pFeatureCursor.NextFeature();
                }
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
               
                return pFeatureNearst;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
                return null;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            ICommand pCommand;
            pCommand = new AddBarriesTool();
            pCommand.OnCreate(Variable.pMapFrm.mainMapControl.Object);
            Variable.pMapFrm.mainMapControl.CurrentTool = pCommand as ITool;
            pCommand = null;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                Variable.pMapFrm.mainMapControl.CurrentTool = null;
                IGraphicsContainer pGraphicsContainer = Variable.pMapFrm.mainMapControl.ActiveView as IGraphicsContainer;
                pGraphicsContainer.DeleteAllElements();
                IFeatureLayer pFeatureLayer = CDataImport.ImportFeatureLayerFromControltext(@"C:\Users\Administrator\Desktop\突发环境事件应急资源调度系统\data\Barries.shp");
                IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
                if (pFeatureClass.FeatureCount(null) > 0)
                {
                    ITable pTable = pFeatureClass as ITable;
                    pTable.DeleteSearchedRows(null);
                }
                Variable.pMapFrm.mainMapControl.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
            
        }

       
    }
}
