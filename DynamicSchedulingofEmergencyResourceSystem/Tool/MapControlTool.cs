using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace Tool
{
    public class MapControlTool
    {
        public static double optimumMapScale = 4000; //最佳地图比例尺

        /// <summary>
        /// 缩放到要素
        /// </summary>
        /// <param name="feature">要素</param>
        public static void ZoomToFeature(IFeature feature, AxMapControl mapControl)
        {
            esriGeometryType geometryType = feature.Shape.GeometryType;
            // 线要素或者面要素
            if (geometryType == esriGeometryType.esriGeometryPolyline ||
                geometryType == esriGeometryType.esriGeometryPolygon)
            {
                mapControl.Extent = feature.Extent;
                if (mapControl.MapScale < optimumMapScale)  // 设置最佳地图比例尺
                    mapControl.MapScale = optimumMapScale;
            }
            // 点要素
            else if (geometryType == esriGeometryType.esriGeometryPoint)
            {
                mapControl.MapScale = optimumMapScale;
                mapControl.CenterAt((IPoint)feature.Shape);
            }
        }
        // 缩放到要素选中集
        /// </summary>
        /// <param name="featureLayer">要素图层</param>
        public static void ZoomToFeatureSelection(IFeatureLayer pFeatureLayer, AxMapControl MainMapControl)
        {
            IFeatureSelection pFeatureSelection = (IFeatureSelection)pFeatureLayer;
            ISelectionSet pSelectionSet = pFeatureSelection.SelectionSet;
            if (pSelectionSet.Count == 0) return;
            //只含有一个选中要素
            if (pSelectionSet.Count == 1)
            {
                IEnumIDs enumIDS = pSelectionSet.IDs;
                int pFid = enumIDS.Next();
                IFeature pFeature = pFeatureLayer.FeatureClass.GetFeature(pFid);
                ZoomToFeature(pFeature, MainMapControl);
            }
            else
            {

                IEnumGeometry enumGeometry = new EnumFeatureGeometry();
                IEnumGeometryBind enumGeometryBind = enumGeometry as IEnumGeometryBind;
                enumGeometryBind.BindGeometrySource(null, pSelectionSet);
                IGeometryFactory geometryFactory = new GeometryEnvironmentClass();
                IGeometry geometry = geometryFactory.CreateGeometryFromEnumerator(enumGeometry);  // 根据要素选中集，创建新的几何图形
                MainMapControl.Extent = geometry.Envelope;
                if (pFeatureLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPoint)  // 点图层，缩小地图比例尺
                    MainMapControl.MapScale *= 1.2;
                if (MainMapControl.MapScale < optimumMapScale)  // 设置最佳地图比例尺
                    MainMapControl.MapScale = optimumMapScale;
            }
        }
    }
}
