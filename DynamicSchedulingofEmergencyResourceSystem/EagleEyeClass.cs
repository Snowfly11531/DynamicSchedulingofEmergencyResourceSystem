using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;

namespace DynamicSchedulingofEmergencyResourceSystem
{
    public class EagleEyeClass
    {
        public static void SynchronizeEagleEye()
        {
            //若鹰眼视图存在图层，清除鹰眼视图的图层
            if (Variable.pEagleEyeMapFrm.EagelEyeMapControl.LayerCount > 0)
            {
                Variable.pEagleEyeMapFrm.EagelEyeMapControl.ClearLayers();
            }
            //设置鹰眼和主地图的坐标系统一致
            Variable.pEagleEyeMapFrm.EagelEyeMapControl.SpatialReference = Variable.pMapFrm.mainMapControl.SpatialReference;
            for (int i = Variable.pMapFrm.mainMapControl.LayerCount - 1; i >= 0; i--)
            {
                //使鹰眼视图与数据视图的图层上下顺序保持一致
                ILayer pLayer = Variable.pMapFrm.mainMapControl.get_Layer(i);
                if (pLayer is IGroupLayer || pLayer is ICompositeLayer)
                {
                    ICompositeLayer pCompositeLayer = (ICompositeLayer)pLayer;
                    for (int j = pCompositeLayer.Count - 1; j >= 0; j--)
                    {
                        ILayer pSubLayer = pCompositeLayer.get_Layer(i);
                        IFeatureLayer pFeatureLayer = pSubLayer as IFeatureLayer;
                        if (pFeatureLayer != null)
                        {
                            //由于鹰眼地图较小，所以过滤点图层不添加     
                            Variable.pEagleEyeMapFrm.EagelEyeMapControl.AddLayer(pLayer);
                        }
                    }
                }
                else
                {
                    IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
                    if (pFeatureLayer != null)
                    {                                                
                        Variable.pEagleEyeMapFrm.EagelEyeMapControl.AddLayer(pLayer);
                    }
                }
                //设置鹰眼地图全图显示
                Variable.pEagleEyeMapFrm.EagelEyeMapControl.Extent = Variable.pMapFrm.mainMapControl.FullExtent;
                Variable.pEnvelop = Variable.pMapFrm.mainMapControl.Extent as IEnvelope;
                Variable.pEagleEyeMapFrm.EagelEyeMapControl.ActiveView.Refresh();

            }
        }

        //在鹰眼地图上面画矩形框
        public static void DrawRectangle(IEnvelope pIEnvelope)
        {
            //在绘制前，清除鹰眼中之前绘制的矩形框
            IGraphicsContainer pGraphicsContainer = Variable.pEagleEyeMapFrm.EagelEyeMapControl.Map as IGraphicsContainer;
            IActiveView pActiveView = pGraphicsContainer as IActiveView;
            pGraphicsContainer.DeleteAllElements();
            //得到当前视图范围
            IRectangleElement pRectangleElement = new RectangleElementClass();
            IElement pElement = pRectangleElement as IElement;
            pElement.Geometry = pIEnvelope;
            //设置矩阵框（实质为中间透明度面）
            IRgbColor pRgbColor = new RgbColorClass();
            pRgbColor = GetRgbColor(255, 0, 0);
            pRgbColor.Transparency = 255;
            ILineSymbol pOutLine = new SimpleLineSymbolClass();
            pOutLine.Width = 2;            pOutLine.Color = pRgbColor;

            IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            pRgbColor = new RgbColorClass();
            pRgbColor.Transparency = 0;
            pFillSymbol.Color = pRgbColor;
            pFillSymbol.Outline = pOutLine;
            //向鹰眼中添加矩形框
            IFillShapeElement pFillShapeElement = pElement as IFillShapeElement;
            pFillShapeElement.Symbol = pFillSymbol;
            pGraphicsContainer.AddElement((IElement)pFillShapeElement, 0);
            //刷新
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }
      
        // 获取RGB颜色       
        public static IRgbColor GetRgbColor(int intR, int intG, int intB)
        {
            IRgbColor pIRgbColor = null;
            if (intR < 0 || intR > 255 || intG < 0 || intG > 255 || intB < 0 || intB > 255)
            {
                return pIRgbColor;
            }
            pIRgbColor = new RgbColorClass();
            pIRgbColor.Red = intR;
            pIRgbColor.Green = intG;
            pIRgbColor.Blue = intB;
            return pIRgbColor;
        }
    }
}
