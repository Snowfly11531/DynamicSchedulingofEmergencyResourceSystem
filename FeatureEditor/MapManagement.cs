using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;

namespace FeatureEditor
{
    public class MapManagement
    {
        public MapManagement()
        { }

        #region 变量定义
        public static Form ToolPlatForm = null;
        private static IEngineEditor engineEditor;
        public static IEngineEditor EngineEditor
        {
            get { return MapManagement.engineEditor; }
            set { MapManagement.engineEditor = value; }
        }
        #endregion

        #region 操作函数

        //获取当前地图文档所有图层集合
        public static IList<ILayer> GetMapLayers(IMap pMap)
        {
            ILayer pLayer = null;
            IList<ILayer> pLayerlist = null;
            try
            {
                pLayerlist = new List<ILayer>();
                for (int i = 0; i < pMap.LayerCount; i++)
                {
                    pLayer = pMap.get_Layer(i);
                    if (!pLayerlist.Contains(pLayer))
                    {
                        pLayerlist.Add(pLayer);
                    }
                }
                return pLayerlist;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
                return null;
            }
        }

        //根据图层名称获取图层
        public static ILayer GetLayerByName(IMap pMap, string layerName)
        {
            ILayer pLayer = null;
            ILayer layer = null;
            try
            {
                for (int i = 0; i < pMap.LayerCount; i++)
                {
                    layer = pMap.get_Layer(i);
                    if (layer.Name.ToUpper() == layerName.ToUpper())
                    {
                        pLayer = layer;
                        break;
                    }
                }
                return pLayer;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
                return null;
            }
        }


        //获取颜色
        public static IRgbColor GetRgbColor(int intR, int intG, int intB)
        {
            IRgbColor pRgbColor = null;
            pRgbColor = new RgbColorClass();
            if (intR < 0) pRgbColor.Red = 0;
            else pRgbColor.Red = intR;
            if (intG < 0) pRgbColor.Green = 0;
            else pRgbColor.Green = intG;
            if (intB < 0) pRgbColor.Blue = 0;
            else pRgbColor.Blue = intB;
            return pRgbColor;
        }

        //获取选择要素
        public static IFeatureCursor GetSelectedFeatures(IFeatureLayer pFeatLyr)
        {
            ICursor pCursor = null;
            IFeatureCursor pFeatCur = null;
            if (pFeatLyr == null) return null;

            IFeatureSelection pFeatSel = pFeatLyr as IFeatureSelection;
            ISelectionSet pSelSet = pFeatSel.SelectionSet;
            if (pSelSet.Count == 0) return null;
            pSelSet.Search(null, false, out pCursor);
            pFeatCur = pCursor as IFeatureCursor;
            return pFeatCur;
        }

        //单位转换
        public static double ConvertPixelsToMapUnits(IActiveView activeView, double pixelUnits)
        {
            int pixelExtent = activeView.ScreenDisplay.DisplayTransformation.get_DeviceFrame().right
                - activeView.ScreenDisplay.DisplayTransformation.get_DeviceFrame().left;

            double realWorldDisplayExtent = activeView.ScreenDisplay.DisplayTransformation.VisibleBounds.Width;
            double sizeOfOnePixel = realWorldDisplayExtent / pixelExtent;

            return pixelUnits * sizeOfOnePixel;
        }

        //计算两点之间X轴和Y轴方向上的距离
        public static bool CalDistance(IPoint lastpoint, IPoint firstpoint, out double deltaX, out double deltaY)
        {
            deltaX = 0; deltaY = 0;
            if (lastpoint == null || firstpoint == null)
                return false;
            deltaX = lastpoint.X - firstpoint.X;
            deltaY = lastpoint.Y - firstpoint.Y;
            return true;
        }
        #endregion
    }
}
