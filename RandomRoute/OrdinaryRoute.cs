using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DynamicSchedulingofEmergencyResourceSystem;
using Agent;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Controls;

namespace RandomRoute
{
    public class OrdinaryRoute
    {
        List<Node> NodeList = new List<Node>();
        List<Result> ResultList = new List<Result>();

        public void InitializationRoadNetwork()
        {
            try
            {
                IFeature pFeaturePoint = null;
                IFeature pFeatureLine = null;
                int PointObjectIndex, LineStartIndex, LineEndIndex, LineObjectIndex;
                string PointID, LineStartID, LineEndID, pLineID;

                Node pNode = null;


                //初始化路径网络的结点信息
                IFeatureLayer pFeatureLayerPoint = CDataImport.ImportFeatureLayerFromControltext(@"C:\Users\Administrator\Desktop\突发环境事件应急资源调度系统\data\point.shp");
                IFeatureClass pFeatureClassPoint = pFeatureLayerPoint.FeatureClass;
                IFeatureCursor pFeatureCursorPoint = pFeatureClassPoint.Search(null, false);
                pFeaturePoint = pFeatureCursorPoint.NextFeature();

                while (pFeaturePoint != null)
                {
                    PointObjectIndex = pFeaturePoint.Fields.FindField("OBJECTID");
                    PointID = Convert.ToString(pFeaturePoint.get_Value(PointObjectIndex));
                    pNode = new Node(PointID);
                    NodeList.Add(pNode);
                    pFeaturePoint = pFeatureCursorPoint.NextFeature();
                }

                //初始化路径网络的路线信息
                IFeatureLayer pFeatureLayerLine = CDataImport.ImportFeatureLayerFromControltext(@"C:\Users\Administrator\Desktop\突发环境事件应急资源调度系统\data\road.shp");
                IFeatureClass pFeatureClassLine = pFeatureLayerLine.FeatureClass;
                IFeatureCursor pFeatureCursorLine = pFeatureClassLine.Search(null, false);
                pFeatureLine = pFeatureCursorLine.NextFeature();
                while (pFeatureLine != null)
                {
                    LineObjectIndex = pFeatureLine.Fields.FindField("OBJECTID");
                    LineStartIndex = pFeatureLine.Fields.FindField("StartNodeI");
                    LineEndIndex = pFeatureLine.Fields.FindField("EndNodeID");
                    pLineID = Convert.ToString(pFeatureLine.get_Value(LineObjectIndex));
                    for (int index = 0; index < 2; index++)
                    {
                        if (index == 0)
                        {
                            LineStartID = Convert.ToString(pFeatureLine.get_Value(LineStartIndex));
                            LineEndID = Convert.ToString(pFeatureLine.get_Value(LineEndIndex));
                            Edge pEdge = new Edge();
                            pEdge.StartNodeID = LineStartID;
                            pEdge.EndNodeID = LineEndID;
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

        public void OrdinaryPlanRoute(List<OrdinaryVehicle> pOrdinaryVehicle)
        {
            List<Result> pResult = new List<Result>();
            for (int i = 0; i < pOrdinaryVehicle.Count; i++)
            {

                Result result = new Result();
                result.StartNodeID = Convert.ToString(pOrdinaryVehicle[i].Origination);
                result.EndNodeID = Convert.ToString(pOrdinaryVehicle[i].Destination);
                string startNodeID = result.StartNodeID;
            }
        }
    }
}
