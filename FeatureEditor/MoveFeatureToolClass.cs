using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using System.Windows.Forms;

namespace FeatureEditor
{
    public class MoveFeatureToolClass : ICommand, ITool
    {
        private IMap m_Map = null;
        private bool bEnable = true;
        private IPoint m_startPt = null;
        private IPoint m_EndPoint = null;
        private IHookHelper m_hookHelper = null;
        private IActiveView m_activeView = null;
        private IEngineEditor m_EngineEditor = null;
        private IEngineEditLayers m_EngineEditLayers = null;
        private IMoveGeometryFeedback m_moveGeoFeedBack = null;

        #region ICommand 成员

        public int Bitmap
        {
            get { return -1; }
        }

        public string Caption
        {
            get { return "移动要素"; }
        }

        public string Category
        {
            get { return "编辑工具"; }
        }

        public bool Checked
        {
            get { return false; }
        }

        public bool Enabled
        {
            get { return bEnable; }
        }

        public int HelpContextID
        {
            get { return -1; }
        }

        public string HelpFile
        {
            get { return ""; }
        }

        public string Message
        {
            get { return "整体移动要素"; }
        }

        public string Name
        {
            get { return "MoveTool"; }
        }

        public void OnClick()
        {
            m_Map = m_hookHelper.FocusMap;
            m_activeView = m_Map as IActiveView;
            m_EngineEditor = MapManagement.EngineEditor;
            m_EngineEditLayers = MapManagement.EngineEditor as IEngineEditLayers;

            EditVertexClass.ClearResource();
        }

        public void OnCreate(object Hook)
        {
            if (Hook == null) return;
            try
            {
                m_hookHelper = new HookHelperClass();
                m_hookHelper.Hook = Hook;
                if (m_hookHelper.ActiveView == null)
                    m_hookHelper = null;
            }
            catch
            {
                m_hookHelper = null;
            }

            if (m_hookHelper == null)
                bEnable = false;
            else
                bEnable = true;
        }

        public string Tooltip
        {
            get { return "整体移动要素"; }
        }

        #endregion

        #region ITool 成员

        public int Cursor
        {
            get { return -1; }
        }

        public bool Deactivate()
        {
            return true;
        }

        public bool OnContextMenu(int x, int y)
        {
            return false;
        }

        public void OnDblClick()
        {
        }

        public void OnKeyDown(int keyCode, int shift)
        {

        }

        public void OnKeyUp(int keyCode, int shift)
        {

        }

        public void OnMouseDown(int button, int shift, int x, int y)
        {
            try
            {
                m_Map.ClearSelection();

                if (m_EngineEditor == null) return;
                if (m_EngineEditor.EditState != esriEngineEditState.esriEngineStateEditing) return;
                if (m_EngineEditLayers == null) return;


                //获取目标图层
                IFeatureLayer pFeatLyr = m_EngineEditLayers.TargetLayer;
                IFeatureClass pFeatCls = pFeatLyr.FeatureClass;
                //获取地图上的坐标点
                m_startPt = m_activeView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);
                IGeometry pGeometry = m_startPt as IGeometry;
                double dLength = 0;
                ITopologicalOperator pTopo = pGeometry as ITopologicalOperator;
                //设置选择过滤条件
                ISpatialFilter pSpatialFilter = new SpatialFilterClass();
                //不同的图层类型设置不同的过滤条件
                switch (pFeatCls.ShapeType)
                {
                    case esriGeometryType.esriGeometryPoint:
                        //将像素距离转换为地图单位距离
                        dLength = MapManagement.ConvertPixelsToMapUnits(m_activeView, 8);
                        pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                        break;
                    case esriGeometryType.esriGeometryPolygon:
                        dLength = MapManagement.ConvertPixelsToMapUnits(m_activeView, 4);
                        pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                        break;
                    case esriGeometryType.esriGeometryPolyline:
                        dLength = MapManagement.ConvertPixelsToMapUnits(m_activeView, 4);
                        pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelCrosses;
                        break;
                }
                //根据过滤条件进行缓冲
                IGeometry pBuffer = null;
                pBuffer = pTopo.Buffer(dLength);
                pGeometry = pBuffer.Envelope as IGeometry;
                pSpatialFilter.Geometry = pGeometry;
                pSpatialFilter.GeometryField = pFeatCls.ShapeFieldName;
                IQueryFilter pQueryFilter = pSpatialFilter as IQueryFilter;
                //根据过滤条件进行查询
                IFeatureCursor pFeatCursor = null;
                pFeatCursor = pFeatCls.Search(pQueryFilter, false);

                IFeature pFeature = pFeatCursor.NextFeature();
                while (pFeature != null)
                {
                    //获取地图选择集
                    m_Map.SelectFeature(pFeatLyr as ILayer, pFeature);
                    pFeature = pFeatCursor.NextFeature();
                }
                m_activeView.Refresh();
                
                
                if (pFeatLyr == null) return;
                //获取要移动几何对象
                IFeatureCursor pFeatCur = MapManagement.GetSelectedFeatures(pFeatLyr);
                if (pFeatCur == null)
                {
                    MessageBox.Show("请选择要移动要素！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                IFeature pFeature1 = pFeatCur.NextFeature();
                //当移动的对象为空时，首先进行对象实例化
                if (m_moveGeoFeedBack == null)
                    m_moveGeoFeedBack = new MoveGeometryFeedbackClass();
                m_moveGeoFeedBack.Display = m_activeView.ScreenDisplay;
                while (pFeature1 != null)
                {
                    m_moveGeoFeedBack.AddGeometry(pFeature1.Shape);
                    pFeature1 = pFeatCur.NextFeature();
                }
                //添加起始点
                m_moveGeoFeedBack.Start(m_startPt);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatCur);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        public void OnMouseMove(int button, int shift, int x, int y)
        {
            try
            {
                IPoint pMovePt = null;
                pMovePt = m_activeView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);
                if (m_moveGeoFeedBack == null) return;
                m_moveGeoFeedBack.MoveTo(pMovePt);

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void OnMouseUp(int button, int shift, int x, int y)
        {
            try
            {
                m_EndPoint = m_activeView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);
                if (m_moveGeoFeedBack == null) return;
                m_moveGeoFeedBack.MoveTo(m_EndPoint);
                MoveFeatures(m_EndPoint, m_startPt);
                m_moveGeoFeedBack.ClearGeometry();
                m_moveGeoFeedBack = null;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public void Refresh(int hdc)
        {

        }
        #endregion

        #region 操作函数

        private void MoveFeatures(IPoint lastpoint, IPoint firstpoint)
        {
            if (m_EngineEditor == null) return;
            //开始一个编辑流程
            m_EngineEditor.StartOperation();
            if (m_EngineEditLayers == null) return;
            //获取移动的图层
            IFeatureLayer pFeatLyr = m_EngineEditLayers.TargetLayer;
            if (pFeatLyr == null) return;
            //获取选择要素指针
            IFeatureCursor pFeatCur = MapManagement.GetSelectedFeatures(pFeatLyr);
            IFeature pFeature = pFeatCur.NextFeature();
            while (pFeature != null)
            {
                //实现要素的移动
                MoveFeature(pFeature, lastpoint, firstpoint);
                pFeature = pFeatCur.NextFeature();
            }
            //停止编辑流程
            m_EngineEditor.StopOperation("MoveTool");
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatCur);
            m_activeView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection | esriViewDrawPhase.esriViewGeography, null, null);
        }

        private void MoveFeature(IFeature pFeature, IPoint lastpoint, IPoint firstpoint)
        {
            double deltax; double deltay;
            IGeoDataset pGeoDataSet;
            ITransform2D transform;
            IGeometry pGeometry;
            IFeatureClass pClass = pFeature.Class as IFeatureClass;
            pGeoDataSet = pClass as IGeoDataset;

            pGeometry = pFeature.Shape;
            if (pGeometry.GeometryType == esriGeometryType.esriGeometryMultiPatch
                || pGeometry.GeometryType == esriGeometryType.esriGeometryPoint
                || pGeometry.GeometryType == esriGeometryType.esriGeometryPolyline
                || pGeometry.GeometryType == esriGeometryType.esriGeometryPolygon)
            {
                pGeometry = pFeature.Shape;
                transform = pGeometry as ITransform2D;
                if (!MapManagement.CalDistance(lastpoint, firstpoint, out deltax, out deltay))
                {
                    MessageBox.Show("计算距离出现错误", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                //根据两点在X轴和Y轴上的距离差，对要素进行移动
                transform.Move(deltax, deltay);
                pGeometry = (IGeometry)transform;
                if (pGeoDataSet.SpatialReference != null)
                {
                    pGeometry.Project(pGeoDataSet.SpatialReference);
                }
                pFeature.Shape = SupportZMFeatureClass.ModifyGeomtryZMValue(pClass, pGeometry);
                //保存移动后的对象 
                pFeature.Store();
            }
        }



        #endregion
    }
}
