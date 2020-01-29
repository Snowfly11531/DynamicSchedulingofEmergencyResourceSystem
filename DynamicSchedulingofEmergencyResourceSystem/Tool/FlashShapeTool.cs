using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using DynamicSchedulingofEmergencyResourceSystem;

namespace Tool
{
    public class FlashShapeTool
    {
        #region 符号

        private IMarkerSymbol pointSymbol;  // 点符号
        private ILineSymbol lineSymbol;     // 线符号
        private IFillSymbol polygonSymbol;  // 面符号

        /// <summary>
        /// 点符号
        /// </summary>
        public IMarkerSymbol PointSymbol
        {
            get
            {
                if (this.pointSymbol == null)
                {
                    IColor color = DisplayTool.DefineRgbColor(0, 128, 0);       // (0, 128, 0)
                    IColor outLineColor = DisplayTool.DefineRgbColor(0, 0, 0);  // (0, 0, 0)
                    ILineSymbol outLineSymbol = DisplayTool.DefineSimpleLineSymbol(outLineColor, 1, esriSimpleLineStyle.esriSLSSolid);
                    this.pointSymbol = DisplayTool.DefineSimpleMarkerSymbol(color, 13, esriSimpleMarkerStyle.esriSMSCircle, outLineSymbol);
                }
                return this.pointSymbol;
            }
            set { this.pointSymbol = value; }
        }

        /// <summary>
        /// 线符号
        /// </summary>
        public ILineSymbol LineSymbol
        {
            get
            {
                if (this.lineSymbol == null)
                {
                    this.lineSymbol = DisplayTool.DefineMultiLayerLineSymbol();
                }
                return this.lineSymbol;
            }
            set { this.lineSymbol = value; }
        }

        /// <summary>
        /// 面符号
        /// </summary>
        public IFillSymbol PolygonSymbol
        {
            get
            {
                if (this.polygonSymbol == null)
                {
                    IColor color = DisplayTool.DefineRgbColor(0, 128, 0);       // (0, 128, 0)
                    IColor outLineColor = DisplayTool.DefineRgbColor(0, 0, 0);  // (0, 0, 0)
                    ILineSymbol outLineSymbol = DisplayTool.DefineSimpleLineSymbol(outLineColor, 1, esriSimpleLineStyle.esriSLSSolid);
                    this.polygonSymbol = DisplayTool.DefineSimpleFillSymbol(color, esriSimpleFillStyle.esriSFSSolid, outLineSymbol);
                }
                return this.polygonSymbol;
            }
            set { this.polygonSymbol = value; }
        }

        #endregion
        /// <summary>
        /// 闪烁图层中的所有选中要素
        /// </summary>
        /// <param name="featureLayer">要素图层</param>
        /// <param name="interval">闪烁时长</param>
        public void FlashShapes(IFeatureLayer featureLayer, int interval)
        {
            System.Windows.Forms.Application.DoEvents();  // 处理当前在消息队列中的所有Windows消息
            IScreenDisplay screenDisplay = Variable.pMapFrm.mainMapControl.ActiveView.ScreenDisplay;
            screenDisplay.StartDrawing(screenDisplay.hDC, (System.Int16)ESRI.ArcGIS.Display.esriScreenCache.esriNoScreenCache);
            // 设置符号
            switch (featureLayer.FeatureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPoint:
                    screenDisplay.SetSymbol((ISymbol)this.PointSymbol);
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    screenDisplay.SetSymbol((ISymbol)this.LineSymbol);
                    break;
                case esriGeometryType.esriGeometryPolygon:
                    screenDisplay.SetSymbol((ISymbol)this.PolygonSymbol);
                    break;
                default:
                    return;
            }
            // 画图
            IFeatureSelection featureSelection = (IFeatureSelection)featureLayer;
            IEnumIDs enumIDs = featureSelection.SelectionSet.IDs;
            int fid = enumIDs.Next();
            while (fid != -1)
            {
                IFeature feature = featureLayer.FeatureClass.GetFeature(fid);
                switch (featureLayer.FeatureClass.ShapeType)
                {
                    case esriGeometryType.esriGeometryPoint:
                        screenDisplay.DrawPoint(feature.Shape);
                        break;
                    case esriGeometryType.esriGeometryPolyline:
                        screenDisplay.DrawPolyline(feature.Shape);
                        break;
                    case esriGeometryType.esriGeometryPolygon:
                        screenDisplay.DrawPolygon(feature.Shape);
                        break;
                    default:
                        return;
                }
                fid = enumIDs.Next();
            }
            Thread.Sleep(interval);
            screenDisplay.FinishDrawing();
            Variable.pMapFrm.mainMapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewForeground, null, null);
        }
    }
}
