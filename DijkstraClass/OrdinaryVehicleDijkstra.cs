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
using Agent;


namespace DijkstraClass
{
    public class OrdinaryVehicleDijkstra
    {
        List<Node> NodeList = new List<Node>();
        List<Result> ResultList = new List<Result>();



        private void InitializationRoadNetwork(List<RoadNetwork> pRoadNetwork)
        {
            IFeature pFeaturePoint = null;
            IFeature pFeatureLine = null;
            int PointObjectIndex, LineStartIndex, LineEndIndex, LineWeightIndex, LineNameIndex, LineObjectIndex;
            string PointID, LineStartID, LineEndID, LineName, pLineID;
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
                    LineObjectIndex = pFeatureLine.Fields.FindField("OBJECTID");
                    LineStartIndex = pFeatureLine.Fields.FindField("StartNodeI");
                    LineEndIndex = pFeatureLine.Fields.FindField("EndNodeID");
                    LineWeightIndex = pFeatureLine.Fields.FindField("Minutes");
                    LineNameIndex = pFeatureLine.Fields.FindField("Name");
                    LineName = Convert.ToString(pFeatureLine.get_Value(LineNameIndex));
                    pLineID = Convert.ToString(pFeatureLine.get_Value(LineObjectIndex));
                    if (pRoadNetwork[Convert.ToInt32(pLineID)-1].VehicleNumber != 0)
                    {
                        Length = (Convert.ToDouble(pFeatureLine.get_Value(LineWeightIndex))) * pRoadNetwork[Convert.ToInt32(pLineID)-1].VehicleNumber;
                    }
                    else
                    {
                        Length = Convert.ToDouble(pFeatureLine.get_Value(LineWeightIndex));
                    }
                   
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
                            pEdge.pLineID = pLineID;
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
                            pEdge.pLineID = pLineID;
                            NodeList[Convert.ToInt32(LineStartID) - 1].EdgeList.Add(pEdge);
                        }
                    }
                    pFeatureLine = pFeatureCursorLine.NextFeature();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        public List<Result> DepotToDest(List<OrdinaryVehicle> pOrdinaryVehicle,List<RoadNetwork> pRoadNetwork)
        {
            InitializationRoadNetwork(pRoadNetwork);


            //应急资源仓库最近路段的管理
            for (int i = 0; i < pOrdinaryVehicle.Count; i++)
            {
                FindPath(pOrdinaryVehicle[i].Origination.ToString(), pOrdinaryVehicle[i].Destination.ToString());
            } 
            return ResultList;

        }


        private void FindPath(string StartID, string EndID)
        {
            RoutePlanner planner = new RoutePlanner();
            RoutePlanResult routeresult = planner.Plan(NodeList, StartID, EndID);
            Result result = new Result();
            result.StrResultNode = routeresult.ResultNodes;
            result.EndNodeID = EndID;
            if (result.StrResultNode == null)
            {
                int i = Convert.ToInt32(StartID);
                int j = Convert.ToInt32(EndID);
                MessageBox.Show(i.ToString() + ":" + j.ToString());
            }
            ResultList.Add(result);

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
        public List<VehicleRouteResult> GetVehicleRoad(Collection<RouteNode> routeNodes, IPoint Depotpoint, IPoint Deptpoint)
        {

            //try
            //{
                List<VehicleRouteResult> pVehicleRouteResultList = new List<VehicleRouteResult>();
                IFeatureLayer pFeatureLayerPoint = CDataImport.ImportFeatureLayerFromControltext(@"C:\Users\Administrator\Desktop\突发环境事件应急资源调度系统\data\point.shp");
                IFeatureClass pFeatureClassPoint = pFeatureLayerPoint.FeatureClass;
                object pObject = Type.Missing;

                for (int i = 0; i < routeNodes.Count; i++)
                {
                    VehicleRouteResult vehiclerouteresult = new VehicleRouteResult();
                    if (i == routeNodes.Count - 1)
                    {
                        break;
                    }
                    int StartID = Convert.ToInt32(routeNodes[i].NodeID);
                    string EndID = Convert.ToString(routeNodes[i + 1].NodeID);
                    //当路线的起点是应急资源仓库时，添加应急资源仓库为起点
                    if (i == 0)
                    {
                        vehiclerouteresult.Startpoint = Depotpoint;
                    }
                    //当路线的终点是应急处置空间位置时，添加应急处置空间位置为终点
                    if (i == routeNodes.Count - 2)
                    {
                        vehiclerouteresult.Endpoint = Deptpoint;
                    }

                    //添加线的结束点
                    IQueryFilter pQueryFilterEndNodeID = new QueryFilterClass();
                    pQueryFilterEndNodeID.WhereClause = "OBJECTID=" + "'" + Convert.ToString(EndID) + "'";
                    IFeatureCursor pFeatureCursorEndNodeID = pFeatureClassPoint.Search(pQueryFilterEndNodeID, false);
                    IFeature pFeatureEndNodeID = pFeatureCursorEndNodeID.NextFeature();
                    while (pFeatureEndNodeID != null)
                    {
                        vehiclerouteresult.Endpoint = pFeatureEndNodeID.Shape as IPoint;
                        pFeatureEndNodeID = pFeatureCursorEndNodeID.NextFeature();
                    }

                    //添加线的开始点
                    IQueryFilter pQueryFilterStartNodeID = new QueryFilterClass();
                    pQueryFilterStartNodeID.WhereClause = "OBJECTID=" + "'" + Convert.ToString(StartID) + "'";
                    IFeatureCursor pFeautreCursorStartNodeID = pFeatureClassPoint.Search(pQueryFilterStartNodeID, false);
                    IFeature pFeatureStartNodeID = pFeautreCursorStartNodeID.NextFeature();
                    while (pFeatureStartNodeID != null)
                    {
                        vehiclerouteresult.Startpoint = pFeatureStartNodeID.Shape as IPoint;
                        pFeatureStartNodeID = pFeautreCursorStartNodeID.NextFeature();
                    }

                    //添加路线
                    Node pNode = NodeList[StartID - 1];
                    for (int j = 0; j < pNode.EdgeList.Count; j++)
                    {
                        if (pNode.EdgeList[j].EndNodeID == EndID)
                        {
                            vehiclerouteresult.pLine = pNode.EdgeList[j].line;
                            vehiclerouteresult.pLineID = pNode.EdgeList[j].pLineID;
                            vehiclerouteresult.StartpointID = pNode.EdgeList[j].StartNodeID;
                            vehiclerouteresult.EndpointID = pNode.EdgeList[j].EndNodeID;
                            break;
                        }
                    }
                    pVehicleRouteResultList.Add(vehiclerouteresult);
                }
                return pVehicleRouteResultList;

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            //    return null;
            //}
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
}
