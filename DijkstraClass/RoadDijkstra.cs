using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI;
using DynamicSchedulingofEmergencyResourceSystem;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Controls;

namespace DijkstraClass
{
    public class DijkstraDepotToDestMethod
    {
        List<Node> NodeList = new List<Node>();
        IPolyline[] startlines = new IPolyline[2];
        IPolyline[] endlines = new IPolyline[2];
        IPointCollection DepotPoint = new MultipointClass(); //输入应急资源仓库点的集合
        IPointCollection DestPoint = new MultipointClass(); //输入应急处置空间位置点的集合
        List<Result> ResultList = new List<Result>();           //存储应急资源仓库到达应急处置空间位置的信息
        string[] DestName;
        string[] DepotName;


        private void InitializationRoadNetwork(IFeatureLayer DepotLayer, IFeatureLayer DestLayer)
        {
            IFeature pFeaturePoint = null;
            IFeature pFeatureLine = null;
            int PointObjectIndex, LineStartIndex, LineEndIndex, LineWeightIndex, LineNameIndex;
            string PointID, LineStartID, LineEndID, LineName;
            double Length;

            Node pNode = null;


            //初始化路径网络的结点信息
            IFeatureLayer pFeatureLayerPoint = CDataImport.ImportFeatureLayerFromControltext(@"C:\Users\Administrator\Desktop\突发环境事件应急资源调度系统\data\point.shp");
            IFeatureClass pFeatureClassPoint = pFeatureLayerPoint.FeatureClass;
            IFeatureCursor pFeatureCursorPoint = pFeatureClassPoint.Search(null, false);
            pFeaturePoint = pFeatureCursorPoint.NextFeature();

            try
            {
                while (pFeaturePoint != null)
                {
                    PointObjectIndex = pFeaturePoint.Fields.FindField("OBJECTID");
                    PointID = Convert.ToString(pFeaturePoint.get_Value(PointObjectIndex));
                    pNode = new Node(PointID);
                    NodeList.Add(pNode);
                    pFeaturePoint = pFeatureCursorPoint.NextFeature();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
            }

            //初始化路径网络的路线信息
            IFeatureLayer pFeatureLayerLine = CDataImport.ImportFeatureLayerFromControltext(@"C:\Users\Administrator\Desktop\突发环境事件应急资源调度系统\data\road.shp");
            IFeatureClass pFeatureClassLine = pFeatureLayerLine.FeatureClass;
            IFeatureCursor pFeatureCursorLine = pFeatureClassLine.Search(null, false);
            pFeatureLine = pFeatureCursorLine.NextFeature();
            try
            {
                while (pFeatureLine != null)
                {
                    //LineObjectIndex = pFeatureLine.Fields.FindField("OBJECTID");
                    LineStartIndex = pFeatureLine.Fields.FindField("StartNodeI");
                    LineEndIndex = pFeatureLine.Fields.FindField("EndNodeID");
                    LineWeightIndex = pFeatureLine.Fields.FindField("Minutes");
                    LineNameIndex = pFeatureLine.Fields.FindField("Name");
                    Length = (double)pFeatureLine.get_Value(LineWeightIndex);
                    LineName = Convert.ToString(pFeatureLine.get_Value(LineNameIndex));
                    for (int index = 0; index < 2; index++)
                    {
                        if (index == 0)
                        {
                            LineStartID = Convert.ToString(pFeatureLine.get_Value(LineStartIndex));
                            LineEndID = Convert.ToString(pFeatureLine.get_Value(LineEndIndex));

                            Edge pEdge = new Edge();
                            pEdge.StartNodeID = LineStartID;
                            pEdge.EndNodeID = LineEndID;
                            pEdge.Weight = Length;
                            pEdge.name = LineName;
                            pEdge.line = pFeatureLine.Shape as IPolyline;
                            NodeList[Convert.ToInt32(LineStartID) - 1].EdgeList.Add(pEdge);
                        }
                        else if (index == 1)
                        {
                            LineStartID = Convert.ToString(pFeatureLine.get_Value(LineEndIndex));
                            LineEndID = Convert.ToString(pFeatureLine.get_Value(LineStartIndex));
                            Edge pEdge = new Edge();
                            pEdge.StartNodeID = LineStartID;
                            pEdge.EndNodeID = LineEndID;
                            pEdge.Weight = Length;
                            pEdge.name = LineName;
                            pEdge.line = pFeatureLine.Shape as IPolyline;
                            NodeList[Convert.ToInt32(LineStartID) - 1].EdgeList.Add(pEdge);
                        }
                    }
                    pFeatureLine = pFeatureCursorLine.NextFeature();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
            }

            //读取应急资源仓库信息
            IFeature pFeatureDepot;
            IFeatureLayer pFeatureLayerDepot = DepotLayer;
            IFeatureClass pFeatureClassDepot = pFeatureLayerDepot.FeatureClass;
            int DepotFeatureCount = pFeatureClassDepot.FeatureCount(null);
            DepotName = new string[DepotFeatureCount];
            int DepotCount = 0;
            IFeatureCursor pFeatureCursorDepot = pFeatureClassDepot.Search(null, false);
            pFeatureDepot = pFeatureCursorDepot.NextFeature();
            while (pFeatureDepot != null)
            {
                DepotName[DepotCount] = Convert.ToString(pFeatureDepot.get_Value(pFeatureDepot.Fields.FindField("Name")));
                DepotCount++;
                IPoint pDepotpoint = pFeatureDepot.Shape as IPoint;
                object O = Type.Missing;
                DepotPoint.AddPoint(pDepotpoint, ref O, ref O);
                pFeatureDepot = pFeatureCursorDepot.NextFeature();
            }

            //读取应急处置空间位置点数据
            IFeature pFeatureDest;
            IFeatureLayer pFeatureLayerDest = DestLayer;
            IFeatureClass pFeatureClassDest = pFeatureLayerDest.FeatureClass;
            int DestFeatureCount = pFeatureClassDest.FeatureCount(null);
            DestName = new string[DestFeatureCount];
            int DestCount = 0;
            IFeatureCursor pFeatureCursorDest = pFeatureClassDest.Search(null, false);
            pFeatureDest = pFeatureCursorDest.NextFeature();
            while (pFeatureDest != null)
            {
                DestName[DestCount] = Convert.ToString(pFeatureDest.get_Value(pFeatureDest.Fields.FindField("Name")));
                DestCount++;
                IPoint pDestpoint = pFeatureDest.Shape as IPoint;
                object O = Type.Missing;
                DestPoint.AddPoint(pDestpoint, ref O, ref O);
                pFeatureDest = pFeatureCursorDest.NextFeature();
            }
        }

        public List<Result> DepotToDest(IFeatureLayer DepotLayer, IFeatureLayer DestLayer)
        {
            InitializationRoadNetwork(DepotLayer, DestLayer);
            IPolyline[] depotlines = new IPolyline[2];
            IPolyline[] destlines = new IPolyline[2];
            double[] depottime = new double[2];
            double[] desttime = new double[2];

            string[] DestPointID = new string[DestPoint.PointCount];
            for (int j = 0; j < DestPoint.PointCount; j++)
            {
                string[] StartdestPoint = new string[DestPoint.PointCount];
                string[] EnddestPoint = new string[DestPoint.PointCount];

                IFeature pDestpointFeature;
                IPoint destPoint = DestPoint.get_Point(j);
                pDestpointFeature = GetNearestFeature(destPoint);
                StartdestPoint[j] = Convert.ToString(pDestpointFeature.get_Value(pDestpointFeature.Fields.FindField("StartNodeI")));
                EnddestPoint[j] = Convert.ToString(pDestpointFeature.get_Value(pDestpointFeature.Fields.FindField("EndNodeID")));
                double time = Convert.ToDouble(pDestpointFeature.get_Value(pDestpointFeature.Fields.FindField("Minutes")));
                string SpaceFeatureName = Convert.ToString(pDestpointFeature.get_Value(pDestpointFeature.Fields.FindField("Name")));




                DestPointID[j] = (NodeList.Count + 1).ToString();


                destlines = GetSubLine(destPoint, pDestpointFeature);
                desttime[0] = time * (destlines[0].Length / (destlines[0].Length + destlines[1].Length));
                desttime[1] = time - desttime[0];

                //移除应急处置空间位置点最近的路径要素
                for (int i = 0; i < 2; i++)
                {
                    if (i == 0)
                    {
                        Edge pedge = new Edge();
                        pedge.StartNodeID = StartdestPoint[j];
                        pedge.EndNodeID = EnddestPoint[j];
                        pedge.Weight = time;
                        pedge.line = pDestpointFeature.Shape as IPolyline;
                        pedge.name = SpaceFeatureName;
                        NodeList[Convert.ToInt32(StartdestPoint[j]) - 1].EdgeList.Remove(pedge);
                    }
                    if (i == 1)
                    {
                        Edge pedge = new Edge();
                        pedge.StartNodeID = EnddestPoint[j];
                        pedge.EndNodeID = StartdestPoint[j];
                        pedge.Weight = time;
                        pedge.line = pDestpointFeature.Shape as IPolyline;
                        pedge.name = SpaceFeatureName;
                        NodeList[Convert.ToInt32(EnddestPoint[j]) - 1].EdgeList.Remove(pedge);
                    }
                }

                Node addNode = new Node((NodeList.Count + 1).ToString());
                NodeList.Add(addNode);
                //添加应急处置空间位置点最近的路径要素从起点到最近点的路径要素
                for (int k = 0; k < 2; k++)
                {
                    if (k == 0)
                    {
                        Edge pedge = new Edge();
                        pedge.StartNodeID = StartdestPoint[j];
                        pedge.EndNodeID = NodeList.Count.ToString();
                        pedge.Weight = desttime[0];
                        pedge.line = destlines[0];
                        pedge.name = SpaceFeatureName;
                        NodeList[Convert.ToInt32(StartdestPoint[j]) - 1].EdgeList.Add(pedge);
                    }
                    if (k == 1)
                    {
                        Edge pedge = new Edge();
                        pedge.StartNodeID = NodeList.Count.ToString();
                        pedge.EndNodeID = StartdestPoint[j];
                        pedge.Weight = desttime[0];
                        pedge.line = destlines[0];
                        pedge.name = SpaceFeatureName;
                        NodeList[NodeList.Count - 1].EdgeList.Add(pedge);
                    }
                }
                //添加应急处置空间位置点最近的路径要素从终点到最近点的路径要素
                for (int k = 0; k < 2; k++)
                {
                    if (k == 0)
                    {
                        Edge pedge = new Edge();
                        pedge.StartNodeID = EnddestPoint[j];
                        pedge.EndNodeID = NodeList.Count.ToString();
                        pedge.Weight = desttime[1];
                        pedge.line = destlines[1];
                        pedge.name = SpaceFeatureName;
                        NodeList[Convert.ToInt32(EnddestPoint[j]) - 1].EdgeList.Add(pedge);
                    }
                    if (k == 1)
                    {
                        Edge pedge = new Edge();
                        pedge.StartNodeID = NodeList.Count.ToString();
                        pedge.EndNodeID = EnddestPoint[j];
                        pedge.Weight = desttime[1];
                        pedge.line = destlines[1];
                        pedge.name = SpaceFeatureName;
                        NodeList[NodeList.Count - 1].EdgeList.Add(pedge);
                    }
                }
            }


            //应急资源仓库最近路段的管理
            for (int i = 0; i < DepotPoint.PointCount; i++)
            {
                IFeature pDepotpointFeature;
                IPoint depotPoint = DepotPoint.get_Point(i);
                pDepotpointFeature = GetNearestFeature(depotPoint);
                string StartdepotPoint = Convert.ToString(pDepotpointFeature.get_Value(pDepotpointFeature.Fields.FindField("StartNodeI")));
                string EnddepotPoint = Convert.ToString(pDepotpointFeature.get_Value(pDepotpointFeature.Fields.FindField("EndNodeID")));
                double time = Convert.ToDouble(pDepotpointFeature.get_Value(pDepotpointFeature.Fields.FindField("Minutes")));
                string DepotFeatureName = Convert.ToString(pDepotpointFeature.get_Value(pDepotpointFeature.Fields.FindField("Name")));
                depotlines = GetSubLine(depotPoint, pDepotpointFeature);
                depottime[0] = time * (depotlines[0].Length / (depotlines[0].Length + depotlines[1].Length));
                depottime[1] = time - desttime[0];
                //移除应急处置空间位置点最近的路径要素
                for (int j = 0; j < 2; j++)
                {
                    if (j == 0)
                    {
                        Edge pedge = new Edge();
                        pedge.StartNodeID = StartdepotPoint;
                        pedge.EndNodeID = EnddepotPoint;
                        pedge.Weight = time;
                        pedge.line = pDepotpointFeature.Shape as IPolyline;
                        pedge.name = DepotFeatureName;
                        NodeList[Convert.ToInt32(StartdepotPoint) - 1].EdgeList.Remove(pedge);
                    }
                    if (j == 1)
                    {
                        Edge pedge = new Edge();
                        pedge.StartNodeID = EnddepotPoint;
                        pedge.EndNodeID = StartdepotPoint;
                        pedge.Weight = time;
                        pedge.line = pDepotpointFeature.Shape as IPolyline;
                        pedge.name = DepotFeatureName;
                        NodeList[Convert.ToInt32(EnddepotPoint) - 1].EdgeList.Remove(pedge);
                    }
                }

                Node addNode = new Node((NodeList.Count + 1).ToString());
                NodeList.Add(addNode);
                //添加应急处置空间位置点最近的路径要素从起点到最近点的路径要素
                for (int k = 0; k < 2; k++)
                {
                    if (k == 0)
                    {
                        Edge pedge = new Edge();
                        pedge.StartNodeID = StartdepotPoint;
                        pedge.EndNodeID = NodeList.Count.ToString();
                        pedge.Weight = depottime[0];
                        pedge.line = depotlines[0];
                        pedge.name = DepotFeatureName;
                        NodeList[Convert.ToInt32(StartdepotPoint) - 1].EdgeList.Add(pedge);
                    }
                    if (k == 1)
                    {
                        Edge pedge = new Edge();
                        pedge.StartNodeID = NodeList.Count.ToString();
                        pedge.EndNodeID = StartdepotPoint;
                        pedge.Weight = depottime[0];
                        pedge.line = depotlines[0];
                        pedge.name = DepotFeatureName;
                        NodeList[NodeList.Count - 1].EdgeList.Add(pedge);
                    }
                }
                //添加应急处置空间位置点最近的路径要素从终点到最近点的路径要素
                for (int k = 0; k < 2; k++)
                {
                    if (k == 0)
                    {
                        Edge pedge = new Edge();
                        pedge.StartNodeID = EnddepotPoint;
                        pedge.EndNodeID = NodeList.Count.ToString();
                        pedge.Weight = depottime[1];
                        pedge.line = depotlines[1];
                        pedge.name = DepotFeatureName;
                        NodeList[Convert.ToInt32(EnddepotPoint) - 1].EdgeList.Add(pedge);
                    }
                    if (k == 1)
                    {
                        Edge pedge = new Edge();
                        pedge.StartNodeID = NodeList.Count.ToString();
                        pedge.EndNodeID = EnddepotPoint;
                        pedge.Weight = depottime[1];
                        pedge.line = depotlines[1];
                        pedge.name = DepotFeatureName;
                        NodeList[NodeList.Count - 1].EdgeList.Add(pedge);
                    }
                }
                FindPath(NodeList.Count.ToString(), DepotName[i], DestPointID);
            }
            return ResultList;



        }
        public void FindPath(string StartID, string depotName, string[] EndID)
        {

            RoutePlanner planner = new RoutePlanner();
            RoutePlanResult[] RouteResult = new RoutePlanResult[EndID.Length];
            RouteResult = planner.Plan(NodeList, StartID, EndID);
            for (int i = 0; i < EndID.Length; i++)
            {
                Result result = new Result();
                result.DepotName = depotName;
                result.DestName = DestName[i];
                result.WeightTime = RouteResult[i].WeightValues;
                result.dynamicschedule = false;
                result.EndNodeID = EndID[i];
                result.StrResultNode = RouteResult[i].ResultNodes;
                ResultList.Add(result);
            }
        }


        //将最短路径的网络的结点添加至
        public static void addRouteNodes(Collection<RouteNode> routeNodes, string str)
        {
            RouteNode routeNode = new RouteNode(str);
            routeNodes.Add(routeNode);
        }
        
        //获取应急资源调度路线
        public void GetIElement(Collection<RouteNode> routeNodes)
        {
            try
            {
                IPolyline pPolyLine = new PolylineClass();
                IGeometryCollection pGeometryCollection = pPolyLine as IGeometryCollection;
                object pObject = Type.Missing;
                IGeometry pGeometry = null;
                for (int i = 0; i < routeNodes.Count; i++)
                {
                    if (i == routeNodes.Count - 1)
                    {
                        break;
                    }
                    int StartID = Convert.ToInt32(routeNodes[i].NodeID);
                    string EndID = Convert.ToString(routeNodes[i + 1].NodeID);
                    Node pNode = NodeList[StartID - 1];

                    for (int j = 0; j < pNode.EdgeList.Count; j++)
                    {
                        if (pNode.EdgeList[j].EndNodeID == EndID)
                        {
                            pGeometry = pNode.EdgeList[j].line as IGeometry;
                            pGeometryCollection.AddGeometryCollection(pGeometry as IGeometryCollection);
                            break;
                        }
                    }
                }
                Variable.PElement.Geometry = pPolyLine;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        //获取应急资源调度路线
        List<IPolyline> listPolyline = new List<IPolyline>();
        public List<IPolyline> GetVehicleRoad(Collection<RouteNode> routeNodes)
        {
            try
            {             
                object pObject = Type.Missing;
                for (int i = 0; i < routeNodes.Count; i++)
                {
                    if (i == routeNodes.Count - 1)
                    {
                        break;
                    }
                    int StartID = Convert.ToInt32(routeNodes[i].NodeID);
                    string EndID = Convert.ToString(routeNodes[i + 1].NodeID);
                    Node pNode = NodeList[StartID - 1];

                    for (int j = 0; j < pNode.EdgeList.Count; j++)
                    {
                        if (pNode.EdgeList[j].EndNodeID == EndID)
                        {
                            listPolyline.Add(pNode.EdgeList[j].line);                                               
                            break;
                        }
                    }
                }
                return listPolyline;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                return null;
            }
        }

        //获取应急资源调度路线
        public static void GetIElement(List<IPolyline> routeroad)
        {
            try
            {
                IPolyline pPolyLine = new PolylineClass();
                IGeometryCollection pGeometryCollection = pPolyLine as IGeometryCollection;
                object pObject = Type.Missing;
                IGeometry pGeometry = null;
                for (int i = 0; i < routeroad.Count; i++)
                {
                    pGeometry = routeroad[i] as IGeometry;
                    pGeometryCollection.AddGeometryCollection(pGeometry as IGeometryCollection);
                }
                Variable.PElement.Geometry = pPolyLine;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        //获取应急资源调度路线
        public IPolyline GetRoutePolyline(Collection<RouteNode> routeNodes)
        {
            try
            {
                IPolyline pPolyLine = new PolylineClass();
                IGeometryCollection pGeometryCollection = pPolyLine as IGeometryCollection;
                object pObject = Type.Missing;
                IGeometry pGeometry = null;
                for (int i = 0; i < routeNodes.Count; i++)
                {
                    if (i == routeNodes.Count - 1)
                    {
                        break;
                    }
                    int StartID = Convert.ToInt32(routeNodes[i].NodeID);
                    string EndID = Convert.ToString(routeNodes[i + 1].NodeID);
                    Node pNode = NodeList[StartID - 1];

                    for (int j = 0; j < pNode.EdgeList.Count; j++)
                    {
                        if (pNode.EdgeList[j].EndNodeID == EndID)
                        {
                            pGeometry = pNode.EdgeList[j].line as IGeometry;
                            pGeometryCollection.AddGeometryCollection(pGeometry as IGeometryCollection);
                            break;
                        }
                    }
                }
                return pPolyLine;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                return null;
            }
        }



        //显示应急资源调度路线
        public static void displayElement()
        {
            try
            {
                IGraphicsContainer pGraphicsContainer = Variable.pMapFrm.mainMapControl.ActiveView as IGraphicsContainer;
                IRgbColor pColor = new RgbColorClass();
                pColor.Red = 255;
                pColor.Green = 255;
                pColor.Blue = 255;
                ILineSymbol pLineSymbol = new SimpleLineSymbolClass();
                pLineSymbol.Color = pColor as IColor;
                pLineSymbol.Width = 5;
                pGraphicsContainer.AddElement(Variable.PElement, 0);
                Variable.pMapFrm.mainMapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                Variable.pMapFrm.mainMapControl.Refresh(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
           

        }

        //删除应急资源调度路线
        public static void deleteElement()
        {
            try
            {
                IGraphicsContainer pGraphicsContainer = Variable.pMapFrm.mainMapControl.ActiveView as IGraphicsContainer;
                pGraphicsContainer.DeleteAllElements();
                Variable.pMapFrm.mainMapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
                Variable.pMapFrm.mainMapControl.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }  
        }

       


        //获取将输入点最近点，该线段分割成两段
        public static IPolyline[] GetSubLine(IPoint point, IFeature pFeature)
        {
            try
            {
                IPolyline pLine = pFeature.Shape as IPolyline;
                IPolyline[] pLines = new IPolyline[2];
                IPolyline StarttoPointLine = new PolylineClass();
                IPolyline EndtoPointLine = new PolylineClass();
                bool splithappened;
                int partindex, segmentindex;
                object pObject = Type.Missing;
                //IPoint IPoint = GetNearestPoint(point);
                pLine.SplitAtPoint(point, false, false, out splithappened, out partindex, out segmentindex);
                ISegmentCollection lineSegCol = (ISegmentCollection)pLine;
                ISegmentCollection newSegCol = (ISegmentCollection)StarttoPointLine;
                ISegmentCollection endSegCol = EndtoPointLine as ISegmentCollection;
                for (int i = 0; i < segmentindex; i++)
                {
                    newSegCol.AddSegment(lineSegCol.get_Segment(i), ref pObject, ref pObject);
                }
                for (int j = segmentindex; j < lineSegCol.SegmentCount; j++)
                {
                    endSegCol.AddSegment(lineSegCol.get_Segment(j), ref pObject, ref pObject);
                }

                lineSegCol.RemoveSegments(0, segmentindex, true);
                pLines[0] = newSegCol as IPolyline;
                pLines[1] = endSegCol as IPolyline;
                return pLines;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
                return null;
            }
        }


        //查找最近某点最近的线
        public static IFeature GetNearestFeature(IPoint pPoint)
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

        //查找点到最近线的交点
        public static IPoint GetNearestPoint(IPoint pPoint)
        {
            try
            {
                IFeature pFeatureNearst = GetNearestFeature(pPoint);
                IProximityOperator pProximityOperator = pFeatureNearst.Shape as IProximityOperator;
                return pProximityOperator.ReturnNearestPoint(pPoint, esriSegmentExtension.esriNoExtension);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
                return null;
            }
        }     


    }

    public class DijkstraDepotToAllNodeMethod
    {
        List<Node> NodeList = new List<Node>();
        IPolyline[] startlines = new IPolyline[2];
        IPolyline[] endlines = new IPolyline[2];
        IPointCollection DepotPoint = new MultipointClass(); //输入应急资源仓库点的集合
        IPointCollection DestPoint = new MultipointClass(); //输入应急处置空间位置点的集合
        List<Result> ResultList = new List<Result>();           //存储应急资源仓库到达应急处置空间位置的信息
        string[] DepotName;


        public void InitializationRoadNetwork(IFeatureLayer DepotLayer)
        {
            IFeature pFeaturePoint = null;
            IFeature pFeatureLine = null;
            int PointObjectIndex, LineStartIndex, LineEndIndex, LineWeightIndex, LineNameIndex;
            string PointID, LineStartID, LineEndID, LineName;
            double Length;

            Node pNode = null;


            //初始化路径网络的结点信息
            IFeatureLayer pFeatureLayerPoint = CDataImport.ImportFeatureLayerFromControltext(@"C:\Users\Administrator\Desktop\突发环境事件应急资源调度系统\data\point.shp");
            IFeatureClass pFeatureClassPoint = pFeatureLayerPoint.FeatureClass;
            IFeatureCursor pFeatureCursorPoint = pFeatureClassPoint.Search(null, false);
            pFeaturePoint = pFeatureCursorPoint.NextFeature();

            try
            {
                while (pFeaturePoint != null)
                {
                    PointObjectIndex = pFeaturePoint.Fields.FindField("OBJECTID");
                    PointID = Convert.ToString(pFeaturePoint.get_Value(PointObjectIndex));
                    pNode = new Node(PointID);
                    NodeList.Add(pNode);
                    pFeaturePoint = pFeatureCursorPoint.NextFeature();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
            }

            //初始化路径网络的路线信息
            IFeatureLayer pFeatureLayerLine = CDataImport.ImportFeatureLayerFromControltext(@"C:\Users\Administrator\Desktop\突发环境事件应急资源调度系统\data\road.shp");
            IFeatureClass pFeatureClassLine = pFeatureLayerLine.FeatureClass;
            IFeatureCursor pFeatureCursorLine = pFeatureClassLine.Search(null, false);
            pFeatureLine = pFeatureCursorLine.NextFeature();
            try
            {
                while (pFeatureLine != null)
                {
                    LineStartIndex = pFeatureLine.Fields.FindField("StartNodeI");
                    LineEndIndex = pFeatureLine.Fields.FindField("EndNodeID");
                    LineWeightIndex = pFeatureLine.Fields.FindField("Minutes");
                    LineNameIndex = pFeatureLine.Fields.FindField("Name");
                    Length = (double)pFeatureLine.get_Value(LineWeightIndex);
                    LineName = Convert.ToString(pFeatureLine.get_Value(LineNameIndex));
                    for (int index = 0; index < 2; index++)
                    {
                        if (index == 0)
                        {
                            LineStartID = Convert.ToString(pFeatureLine.get_Value(LineStartIndex));
                            LineEndID = Convert.ToString(pFeatureLine.get_Value(LineEndIndex));

                            Edge pEdge = new Edge();
                            pEdge.StartNodeID = LineStartID;
                            pEdge.EndNodeID = LineEndID;
                            pEdge.Weight = Length;
                            pEdge.name = LineName;
                            pEdge.line = pFeatureLine.Shape as IPolyline;
                            NodeList[Convert.ToInt32(LineStartID) - 1].EdgeList.Add(pEdge);
                        }
                        else if (index == 1)
                        {
                            LineStartID = Convert.ToString(pFeatureLine.get_Value(LineEndIndex));
                            LineEndID = Convert.ToString(pFeatureLine.get_Value(LineStartIndex));
                            Edge pEdge = new Edge();
                            pEdge.StartNodeID = LineStartID;
                            pEdge.EndNodeID = LineEndID;
                            pEdge.Weight = Length;
                            pEdge.name = LineName;
                            pEdge.line = pFeatureLine.Shape as IPolyline;
                            NodeList[Convert.ToInt32(LineStartID) - 1].EdgeList.Add(pEdge);
                        }
                    }
                    pFeatureLine = pFeatureCursorLine.NextFeature();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
            }

            //读取应急资源仓库信息
            IFeature pFeatureDepot;
            IFeatureLayer pFeatureLayerDepot = DepotLayer;
            IFeatureClass pFeatureClassDepot = pFeatureLayerDepot.FeatureClass;
            int DepotFeatureCount = pFeatureClassDepot.FeatureCount(null);
            DepotName = new string[DepotFeatureCount];
            int DepotCount = 0;
            IFeatureCursor pFeatureCursorDepot = pFeatureClassDepot.Search(null, false);
            pFeatureDepot = pFeatureCursorDepot.NextFeature();
            while (pFeatureDepot != null)
            {
                DepotName[DepotCount] = Convert.ToString(pFeatureDepot.get_Value(pFeatureDepot.Fields.FindField("Name")));
                DepotCount++;
                IPoint pDepotpoint = pFeatureDepot.Shape as IPoint;
                object O = Type.Missing;
                DepotPoint.AddPoint(pDepotpoint, ref O, ref O);
                pFeatureDepot = pFeatureCursorDepot.NextFeature();
            }
        }

        public List<PlanCourse> DepotToAllNode()
        {
            IPolyline[] depotlines = new IPolyline[2];
            double[] depottime = new double[2];

            //应急资源仓库最近路段的管理
            for (int i = 0; i < DepotPoint.PointCount; i++)
            {
                IFeature pDepotpointFeature;
                IPoint depotPoint = DepotPoint.get_Point(i);
                pDepotpointFeature = DijkstraDepotToDestMethod.GetNearestFeature(depotPoint);
                string StartdepotPoint = Convert.ToString(pDepotpointFeature.get_Value(pDepotpointFeature.Fields.FindField("StartNodeI")));
                string EnddepotPoint = Convert.ToString(pDepotpointFeature.get_Value(pDepotpointFeature.Fields.FindField("EndNodeID")));
                double time = Convert.ToDouble(pDepotpointFeature.get_Value(pDepotpointFeature.Fields.FindField("Minutes")));
                string DepotFeatureName = Convert.ToString(pDepotpointFeature.get_Value(pDepotpointFeature.Fields.FindField("Name")));
                depotlines = DijkstraDepotToDestMethod.GetSubLine(depotPoint, pDepotpointFeature);
                depottime[0] = time * (depotlines[0].Length / (depotlines[0].Length + depotlines[1].Length));
                depottime[1] = time - depottime[0];
                //移除应急处置空间位置点最近的路径要素
                for (int j = 0; j < 2; j++)
                {
                    if (j == 0)
                    {
                        Edge pedge = new Edge();
                        pedge.StartNodeID = StartdepotPoint;
                        pedge.EndNodeID = EnddepotPoint;
                        pedge.Weight = time;
                        pedge.line = pDepotpointFeature.Shape as IPolyline;
                        pedge.name = DepotFeatureName;
                        NodeList[Convert.ToInt32(StartdepotPoint) - 1].EdgeList.Remove(pedge);
                    }
                    if (j == 1)
                    {
                        Edge pedge = new Edge();
                        pedge.StartNodeID = EnddepotPoint;
                        pedge.EndNodeID = StartdepotPoint;
                        pedge.Weight = time;
                        pedge.line = pDepotpointFeature.Shape as IPolyline;
                        pedge.name = DepotFeatureName;
                        NodeList[Convert.ToInt32(EnddepotPoint) - 1].EdgeList.Remove(pedge);
                    }
                }

                Node addNode = new Node((NodeList.Count + 1).ToString());
                NodeList.Add(addNode);
                //添加应急处置空间位置点最近的路径要素从起点到最近点的路径要素
                for (int k = 0; k < 2; k++)
                {
                    if (k == 0)
                    {
                        Edge pedge = new Edge();
                        pedge.StartNodeID = StartdepotPoint;
                        pedge.EndNodeID = NodeList.Count.ToString();
                        pedge.Weight = depottime[0];
                        pedge.line = depotlines[0];
                        pedge.name = DepotFeatureName;
                        NodeList[Convert.ToInt32(StartdepotPoint) - 1].EdgeList.Add(pedge);
                    }
                    if (k == 1)
                    {
                        Edge pedge = new Edge();
                        pedge.StartNodeID = NodeList.Count.ToString();
                        pedge.EndNodeID = StartdepotPoint;
                        pedge.Weight = depottime[0];
                        pedge.line = depotlines[0];
                        pedge.name = DepotFeatureName;
                        NodeList[NodeList.Count - 1].EdgeList.Add(pedge);
                    }
                }
                //添加应急处置空间位置点最近的路径要素从终点到最近点的路径要素
                for (int k = 0; k < 2; k++)
                {
                    if (k == 0)
                    {
                        Edge pedge = new Edge();
                        pedge.StartNodeID = EnddepotPoint;
                        pedge.EndNodeID = NodeList.Count.ToString();
                        pedge.Weight = depottime[1];
                        pedge.line = depotlines[1];
                        pedge.name = DepotFeatureName;
                        NodeList[Convert.ToInt32(EnddepotPoint) - 1].EdgeList.Add(pedge);
                    }
                    if (k == 1)
                    {
                        Edge pedge = new Edge();
                        pedge.StartNodeID = NodeList.Count.ToString();
                        pedge.EndNodeID = EnddepotPoint;
                        pedge.Weight = depottime[1];
                        pedge.line = depotlines[1];
                        pedge.name = DepotFeatureName;
                        NodeList[NodeList.Count - 1].EdgeList.Add(pedge);
                    }
                }
                FindAllNodePath(NodeList.Count.ToString(), DepotName[i]);
            }
            return plancourse;
        }
        List<PlanCourse> plancourse = new List<PlanCourse>();
        public void FindAllNodePath(string StartID, string depotName)
        {
            RoutePlanner planner = new RoutePlanner();
            plancourse.Add(planner.Plan(NodeList, StartID));
           
        }
    }



    public class DijkstraDepotToAllNodeMethod1
    {
        List<Node> NodeList = new List<Node>();
        IPolyline[] startlines = new IPolyline[2];
        IPolyline[] endlines = new IPolyline[2];
        IPointCollection DepotPoint = new MultipointClass(); //输入应急资源仓库点的集合
        IPointCollection DestPoint = new MultipointClass(); //输入应急处置空间位置点的集合
        List<Result> ResultList = new List<Result>();           //存储应急资源仓库到达应急处置空间位置的信息
        string[] DepotName;


        public void InitializationRoadNetwork(IFeatureLayer DepotLayer)
        {
            IFeature pFeaturePoint = null;
            IFeature pFeatureLine = null;
            int PointObjectIndex, LineStartIndex, LineEndIndex, LineWeightIndex, LineNameIndex;
            string PointID, LineStartID, LineEndID, LineName;
            double Length;

            Node pNode = null;


            //初始化路径网络的结点信息
            IFeatureLayer pFeatureLayerPoint = CDataImport.ImportFeatureLayerFromControltext(@"C:\Users\Administrator\Desktop\突发环境事件应急资源调度系统\data\point.shp");
            IFeatureClass pFeatureClassPoint = pFeatureLayerPoint.FeatureClass;
            IFeatureCursor pFeatureCursorPoint = pFeatureClassPoint.Search(null, false);
            pFeaturePoint = pFeatureCursorPoint.NextFeature();

            try
            {
                while (pFeaturePoint != null)
                {
                    PointObjectIndex = pFeaturePoint.Fields.FindField("OBJECTID");
                    PointID = Convert.ToString(pFeaturePoint.get_Value(PointObjectIndex));
                    pNode = new Node(PointID);
                    NodeList.Add(pNode);
                    pFeaturePoint = pFeatureCursorPoint.NextFeature();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
            }

            //初始化路径网络的路线信息
            IFeatureLayer pFeatureLayerLine = CDataImport.ImportFeatureLayerFromControltext(@"C:\Users\Administrator\Desktop\突发环境事件应急资源调度系统\data\road.shp");
            IFeatureClass pFeatureClassLine = pFeatureLayerLine.FeatureClass;
            IFeatureCursor pFeatureCursorLine = pFeatureClassLine.Search(null, false);
            pFeatureLine = pFeatureCursorLine.NextFeature();
            try
            {
                while (pFeatureLine != null)
                {
                    LineStartIndex = pFeatureLine.Fields.FindField("StartNodeI");
                    LineEndIndex = pFeatureLine.Fields.FindField("EndNodeID");
                    LineWeightIndex = pFeatureLine.Fields.FindField("Minutes");
                    LineNameIndex = pFeatureLine.Fields.FindField("Name");
                    Length = (double)pFeatureLine.get_Value(LineWeightIndex);
                    LineName = Convert.ToString(pFeatureLine.get_Value(LineNameIndex));
                    for (int index = 0; index < 2; index++)
                    {
                        if (index == 0)
                        {
                            LineStartID = Convert.ToString(pFeatureLine.get_Value(LineStartIndex));
                            LineEndID = Convert.ToString(pFeatureLine.get_Value(LineEndIndex));

                            Edge pEdge = new Edge();
                            pEdge.StartNodeID = LineStartID;
                            pEdge.EndNodeID = LineEndID;
                            pEdge.Weight = Length;
                            pEdge.name = LineName;
                            pEdge.line = pFeatureLine.Shape as IPolyline;
                            NodeList[Convert.ToInt32(LineStartID) - 1].EdgeList.Add(pEdge);
                        }
                        else if (index == 1)
                        {
                            LineStartID = Convert.ToString(pFeatureLine.get_Value(LineEndIndex));
                            LineEndID = Convert.ToString(pFeatureLine.get_Value(LineStartIndex));
                            Edge pEdge = new Edge();
                            pEdge.StartNodeID = LineStartID;
                            pEdge.EndNodeID = LineEndID;
                            pEdge.Weight = Length;
                            pEdge.name = LineName;
                            pEdge.line = pFeatureLine.Shape as IPolyline;
                            NodeList[Convert.ToInt32(LineStartID) - 1].EdgeList.Add(pEdge);
                        }
                    }
                    pFeatureLine = pFeatureCursorLine.NextFeature();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
            }

            //读取应急资源仓库信息
            IFeature pFeatureDepot;
            IFeatureLayer pFeatureLayerDepot = DepotLayer;
            IFeatureClass pFeatureClassDepot = pFeatureLayerDepot.FeatureClass;
            int DepotFeatureCount = pFeatureClassDepot.FeatureCount(null);
            DepotName = new string[DepotFeatureCount];
            int DepotCount = 0;
            IFeatureCursor pFeatureCursorDepot = pFeatureClassDepot.Search(null, false);
            pFeatureDepot = pFeatureCursorDepot.NextFeature();
            while (pFeatureDepot != null)
            {
                DepotName[DepotCount] = Convert.ToString(pFeatureDepot.get_Value(pFeatureDepot.Fields.FindField("Name")));
                DepotCount++;
                IPoint pDepotpoint = pFeatureDepot.Shape as IPoint;
                object O = Type.Missing;
                DepotPoint.AddPoint(pDepotpoint, ref O, ref O);
                pFeatureDepot = pFeatureCursorDepot.NextFeature();
            }
        }

        public List<DepotToAllNode> DepotToAllNode()
        {
            IPolyline[] depotlines = new IPolyline[2];
            double[] depottime = new double[2];

            //应急资源仓库最近路段的管理
            for (int i = 0; i < DepotPoint.PointCount; i++)
            {
                IFeature pDepotpointFeature;
                IPoint depotPoint = DepotPoint.get_Point(i);
                pDepotpointFeature = DijkstraDepotToDestMethod.GetNearestFeature(depotPoint);
                string StartdepotPoint = Convert.ToString(pDepotpointFeature.get_Value(pDepotpointFeature.Fields.FindField("StartNodeI")));
                string EnddepotPoint = Convert.ToString(pDepotpointFeature.get_Value(pDepotpointFeature.Fields.FindField("EndNodeID")));
                double time = Convert.ToDouble(pDepotpointFeature.get_Value(pDepotpointFeature.Fields.FindField("Minutes")));
                string DepotFeatureName = Convert.ToString(pDepotpointFeature.get_Value(pDepotpointFeature.Fields.FindField("Name")));
                depotlines = DijkstraDepotToDestMethod.GetSubLine(depotPoint, pDepotpointFeature);
                depottime[0] = time * (depotlines[0].Length / (depotlines[0].Length + depotlines[1].Length));
                depottime[1] = time - depottime[0];
                //移除应急处置空间位置点最近的路径要素
                for (int j = 0; j < 2; j++)
                {
                    if (j == 0)
                    {
                        Edge pedge = new Edge();
                        pedge.StartNodeID = StartdepotPoint;
                        pedge.EndNodeID = EnddepotPoint;
                        pedge.Weight = time;
                        pedge.line = pDepotpointFeature.Shape as IPolyline;
                        pedge.name = DepotFeatureName;
                        NodeList[Convert.ToInt32(StartdepotPoint) - 1].EdgeList.Remove(pedge);
                    }
                    if (j == 1)
                    {
                        Edge pedge = new Edge();
                        pedge.StartNodeID = EnddepotPoint;
                        pedge.EndNodeID = StartdepotPoint;
                        pedge.Weight = time;
                        pedge.line = pDepotpointFeature.Shape as IPolyline;
                        pedge.name = DepotFeatureName;
                        NodeList[Convert.ToInt32(EnddepotPoint) - 1].EdgeList.Remove(pedge);
                    }
                }

                Node addNode = new Node((NodeList.Count + 1).ToString());
                NodeList.Add(addNode);
                //添加应急处置空间位置点最近的路径要素从起点到最近点的路径要素
                for (int k = 0; k < 2; k++)
                {
                    if (k == 0)
                    {
                        Edge pedge = new Edge();
                        pedge.StartNodeID = StartdepotPoint;
                        pedge.EndNodeID = NodeList.Count.ToString();
                        pedge.Weight = depottime[0];
                        pedge.line = depotlines[0];
                        pedge.name = DepotFeatureName;
                        NodeList[Convert.ToInt32(StartdepotPoint) - 1].EdgeList.Add(pedge);
                    }
                    if (k == 1)
                    {
                        Edge pedge = new Edge();
                        pedge.StartNodeID = NodeList.Count.ToString();
                        pedge.EndNodeID = StartdepotPoint;
                        pedge.Weight = depottime[0];
                        pedge.line = depotlines[0];
                        pedge.name = DepotFeatureName;
                        NodeList[NodeList.Count - 1].EdgeList.Add(pedge);
                    }
                }
                //添加应急处置空间位置点最近的路径要素从终点到最近点的路径要素
                for (int k = 0; k < 2; k++)
                {
                    if (k == 0)
                    {
                        Edge pedge = new Edge();
                        pedge.StartNodeID = EnddepotPoint;
                        pedge.EndNodeID = NodeList.Count.ToString();
                        pedge.Weight = depottime[1];
                        pedge.line = depotlines[1];
                        pedge.name = DepotFeatureName;
                        NodeList[Convert.ToInt32(EnddepotPoint) - 1].EdgeList.Add(pedge);
                    }
                    if (k == 1)
                    {
                        Edge pedge = new Edge();
                        pedge.StartNodeID = NodeList.Count.ToString();
                        pedge.EndNodeID = EnddepotPoint;
                        pedge.Weight = depottime[1];
                        pedge.line = depotlines[1];
                        pedge.name = DepotFeatureName;
                        NodeList[NodeList.Count - 1].EdgeList.Add(pedge);
                    }
                }
                FindAllNodePath(NodeList.Count.ToString(), DepotName[i]);
            }
            return pDepotToAllNode;
        }

        List<DepotToAllNode> pDepotToAllNode = new List<DepotToAllNode>();
        public void FindAllNodePath(string StartID, string depotName)
        {
            RoutePlanner planner = new RoutePlanner();
            DepotToAllNode depottoallnode = new DepotToAllNode();
            depottoallnode.plancourse = planner.Plan(NodeList, StartID);
            depottoallnode.Name = depotName;
            pDepotToAllNode.Add(depottoallnode);
        }   
    }
}
