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
    public class RiverManageMethod
    {
        public static IFeature GetpointoverlapFeature(IPoint inputpoint, IFeatureLayer pFeatureLayer)
        {
            try
            {
                IPoint outpoint = new PointClass();
                IPolyline polyline = new PolylineClass();
                double disAlongCurveFrom = 0.0;
                double disFromCurve = 0.0;
                bool isRighside = false;
                IFeature pFeature;
                IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
                IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);
                pFeature = pFeatureCursor.NextFeature();
                while (pFeature != null)
                {
                    polyline = pFeature.Shape as IPolyline;
                    polyline.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, inputpoint, false, outpoint,
                        ref disAlongCurveFrom, ref disFromCurve, ref isRighside);
                    if (disFromCurve == 0)
                    {
                        break;
                    }
                    pFeature = pFeatureCursor.NextFeature();
                }
                if (pFeature == null)
                {
                    MessageBox.Show("输入的污染点数据不在河流上，请检查数据");
                    return null;
                }
                else
                {
                    return pFeature;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
                return null;
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
    }
}
