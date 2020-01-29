using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DynamicSchedulingofEmergencyResourceSystem;
using WeifenLuo.WinFormsUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Controls;

namespace RiverClass
{
    public class RiverUpstream
    {
        List<RiverNode> NodeList = new List<RiverNode>();

        public void InitializationRoadNetwork()
        {
            IFeature pFeaturePoint = null;
            IFeature pFeatureLine = null;
            int PointObjectIndex, LineStartIndex, LineEndIndex;
            string PointID, LineStartID, LineEndID;

            RiverNode pNode = null;


            //初始化路径网络的结点信息
            IFeatureLayer pFeatureLayerPoint = CDataImport.ImportFeatureLayerFromControltext(@"C:\Users\Administrator\Desktop\突发环境事件应急资源调度系统\data\riverNode.shp");
            IFeatureClass pFeatureClassPoint = pFeatureLayerPoint.FeatureClass;
            IFeatureCursor pFeatureCursorPoint = pFeatureClassPoint.Search(null, false);
            pFeaturePoint = pFeatureCursorPoint.NextFeature();

            try
            {
                while (pFeaturePoint != null)
                {
                    PointObjectIndex = pFeaturePoint.Fields.FindField("OBJECTID");
                    PointID = Convert.ToString(pFeaturePoint.get_Value(PointObjectIndex));
                    pNode = new RiverNode(PointID);
                    NodeList.Add(pNode);
                    pFeaturePoint = pFeatureCursorPoint.NextFeature();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
            }

            //初始化路径网络的路线信息
            IFeatureLayer pFeatureLayerLine = CDataImport.ImportFeatureLayerFromControltext(@"C:\Users\Administrator\Desktop\突发环境事件应急资源调度系统\data\river.shp");
            IFeatureClass pFeatureClassLine = pFeatureLayerLine.FeatureClass;
            IFeatureCursor pFeatureCursorLine = pFeatureClassLine.Search(null, false);
            pFeatureLine = pFeatureCursorLine.NextFeature();
            try
            {
                while (pFeatureLine != null)
                {
                    //LineObjectIndex = pFeatureLine.Fields.FindField("OBJECTID");
                    LineStartIndex = pFeatureLine.Fields.FindField("EndNodeID");
                    LineEndIndex = pFeatureLine.Fields.FindField("StartNodeI");

                    LineStartID = Convert.ToString(pFeatureLine.get_Value(LineStartIndex));
                    LineEndID = Convert.ToString(pFeatureLine.get_Value(LineEndIndex));

                    RiverEdge pEdge = new RiverEdge();
                    pEdge.StartNodeID = LineStartID;
                    pEdge.EndNodeID = LineEndID;
                    pEdge.line = pFeatureLine.Shape as IPolyline;
                    NodeList[Convert.ToInt32(LineStartID) - 1].EdgeList.Add(pEdge);

                    pFeatureLine = pFeatureCursorLine.NextFeature();
                }          
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");

            }
        }


        public List<IPolyline> pollutionpointupstreamriver(IFeatureLayer pullutionpoint, IFeatureLayer pFeatureLayer)
        {
            InitializationRoadNetwork();
            try
            {
                IPolyline[] lines = new IPolyline[2];
                IPoint point = new PointClass();
                IFeature pFeaturepullutionpoint;
                IFeatureLayer pFeatureLayerpullutionpoint = pullutionpoint;
                IFeatureClass pFeatureClasspullutionpoint = pFeatureLayerpullutionpoint.FeatureClass;
                IFeatureCursor pFeatureCursorpullutionpoint = pFeatureClasspullutionpoint.Search(null, false);
                pFeaturepullutionpoint = pFeatureCursorpullutionpoint.NextFeature();
                if (pFeaturepullutionpoint != null)
                {
                    point = pFeaturepullutionpoint.Shape as IPoint;
                }
                IFeature pointOverlapFeature = RiverManageMethod.GetpointoverlapFeature(point, pFeatureLayer);
                lines = RiverManageMethod.GetSubLine(point, pointOverlapFeature);
                string StartNodeID = Convert.ToString(pointOverlapFeature.get_Value(pointOverlapFeature.Fields.FindField("StartNodeI")));
                string EndNodeID = Convert.ToString(pointOverlapFeature.get_Value(pointOverlapFeature.Fields.FindField("EndNodeID")));
                //删除点叠加在线上起点边 
                for (int i = 0; i < NodeList[Convert.ToInt32(EndNodeID) - 1].EdgeList.Count; i++)
                {
                    if (NodeList[Convert.ToInt32(EndNodeID) - 1].EdgeList[i].StartNodeID == EndNodeID ||
                        NodeList[Convert.ToInt32(EndNodeID) - 1].EdgeList[i].EndNodeID == StartNodeID)
                    {
                        NodeList[Convert.ToInt32(EndNodeID) - 1].EdgeList.Remove(NodeList[Convert.ToInt32(EndNodeID) - 1].EdgeList[i]);
                    }
                }
                //添加节点
                RiverNode addNode = new RiverNode((NodeList.Count + 1).ToString());
                NodeList.Add(addNode);
                //添加污染点到起点的路段
                RiverEdge startEdge = new RiverEdge();
                startEdge.StartNodeID = NodeList.Count.ToString();
                startEdge.EndNodeID = StartNodeID;
                startEdge.line = lines[0];
                NodeList[Convert.ToInt32(NodeList.Count - 1)].EdgeList.Add(startEdge);
                //添加污染点到终点的路段
                RiverEdge endEdge = new RiverEdge();
                endEdge.StartNodeID = EndNodeID;
                endEdge.EndNodeID = NodeList.Count.ToString();
                endEdge.line = lines[1];
                NodeList[(Convert.ToInt32(EndNodeID) - 1)].EdgeList.Add(endEdge);

                List<IPolyline> resultLine = new List<IPolyline>();
                List<RiverNode> resultNodeList = new List<RiverNode>();
                RiverNode pStartNode = new RiverNode(NodeList.Count.ToString());
                resultNodeList.Add(pStartNode);

                while (resultNodeList.Count != 0)
                {
                    if (NodeList[(Convert.ToInt32(resultNodeList[0].ID) - 1)].EdgeList.Count != 0)
                    {
                        for (int j = 0; j < NodeList[(Convert.ToInt32(resultNodeList[0].ID) - 1)].EdgeList.Count; j++)
                        {
                            resultLine.Add(NodeList[(Convert.ToInt32(resultNodeList[0].ID) - 1)].EdgeList[j].line);
                            RiverNode node = new RiverNode(NodeList[(Convert.ToInt32(resultNodeList[0].ID) - 1)].EdgeList[j].EndNodeID);
                            resultNodeList.Add(node);
                        }
                        resultNodeList.Remove(resultNodeList[0]);
                    }
                    else
                    {
                        resultNodeList.Remove(resultNodeList[0]);
                    }
                }
                return resultLine;
                //for (int j = 0; j < resultNodeList.Count; j++)
                //{

                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
                return null;
            }
        }

       
    }
}
