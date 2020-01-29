using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Display;

namespace Tool
{
    /// <summary>
    /// 显示工具
    /// 定义颜色、符号
    /// </summary>
    public class DisplayTool
    {
        #region 定义颜色

        /// <summary>
        /// 定义RGB颜色
        /// </summary>
        /// <param name="red">红色(0-255)</param>
        /// <param name="green">绿色(0-255)</param>
        /// <param name="blue">蓝色(0-255)</param>
        /// <returns></returns>
        public static IRgbColor DefineRgbColor(int red, int green, int blue)
        {
            IRgbColor rgbColor = new RgbColorClass();
            rgbColor.Red = red;
            rgbColor.Green = green;
            rgbColor.Blue = blue;
            return rgbColor;
        }

        /// <summary>
        /// 定义RGB颜色
        /// </summary>
        /// <param name="red">红色(0-255)</param>
        /// <param name="green">绿色(0-255)</param>
        /// <param name="blue">蓝色(0-255)</param>
        /// <param name="alpha">透明度(0-255)。0为透明，255为不透明</param>
        /// <returns></returns>
        public static IRgbColor DefineRgbColor(int red, int green, int blue, byte alpha)
        {
            IRgbColor rgbColor = new RgbColorClass();
            rgbColor.Red = red;
            rgbColor.Green = green;
            rgbColor.Blue = blue;
            rgbColor.Transparency = alpha;
            return rgbColor;
        }

        #endregion

        #region 定义符号

        /// <summary>
        /// 定义简单线符号
        /// </summary>
        /// <param name="color">颜色</param>
        /// <param name="width">线宽</param>
        /// <param name="simpleLineStyle">简单线样式</param>
        /// <returns></returns>
        public static ISimpleLineSymbol DefineSimpleLineSymbol(IColor color, double width, esriSimpleLineStyle simpleLineStyle)
        {
            ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbolClass();
            simpleLineSymbol.Color = color;
            simpleLineSymbol.Width = width;
            simpleLineSymbol.Style = simpleLineStyle;
            return simpleLineSymbol;
        }

        /// <summary>
        /// 定义简单点符号
        /// </summary>
        /// <param name="color">颜色</param>
        /// <param name="size">大小</param>
        /// <param name="simpleMarkerStyle">简单点样式</param>
        /// <param name="outLineSymbol">边界线</param>
        /// <returns></returns>
        public static ISimpleMarkerSymbol DefineSimpleMarkerSymbol(IColor color, double size, esriSimpleMarkerStyle simpleMarkerStyle, ILineSymbol outLineSymbol)
        {
            ISimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbolClass();
            simpleMarkerSymbol.Color = color;
            simpleMarkerSymbol.Size = size;
            simpleMarkerSymbol.Style = simpleMarkerStyle;
            if (outLineSymbol == null)
            {
                simpleMarkerSymbol.Outline = false;
            }
            else
            {
                simpleMarkerSymbol.Outline = true;
                simpleMarkerSymbol.OutlineColor = outLineSymbol.Color;
                simpleMarkerSymbol.OutlineSize = outLineSymbol.Width;
            }
            return simpleMarkerSymbol;
        }

        /// <summary>
        /// 定义带边界线的线符号
        /// </summary>
        /// <returns></returns>
        public static IMultiLayerLineSymbol DefineMultiLayerLineSymbol()
        {
            return DefineMultiLayerLineSymbol(DefineRgbColor(0, 128, 0), 4, esriSimpleLineStyle.esriSLSSolid,
                DefineRgbColor(0, 0, 0), 1.5, esriSimpleLineStyle.esriSLSSolid);
        }

        /// <summary>
        /// 定义带边界线的线符号
        /// </summary>
        /// <param name="color">颜色</param>
        /// <param name="width">线宽</param>
        /// <param name="simpleLineStyle">简单线样式</param>
        /// <param name="outLineColor">边界线颜色</param>
        /// <param name="outLineWidth">边界线宽</param>
        /// <param name="outLineStyle">边界线样式</param>
        /// <returns></returns>
        public static IMultiLayerLineSymbol DefineMultiLayerLineSymbol(IColor color, double width, esriSimpleLineStyle simpleLineStyle,
            IColor outLineColor, double outLineWidth, esriSimpleLineStyle outLineStyle)
        {
            // 线1
            ISimpleLineSymbol backLine = new SimpleLineSymbolClass();
            backLine.Color = outLineColor;
            backLine.Width = width + outLineWidth;
            backLine.Style = outLineStyle;
            // 线2
            ISimpleLineSymbol foreLine = new SimpleLineSymbolClass();
            foreLine.Color = color;
            foreLine.Width = width;
            foreLine.Style = simpleLineStyle;
            // 多层线（线1在线2底下）
            IMultiLayerLineSymbol multiLayerLineSymbol = new MultiLayerLineSymbolClass();
            multiLayerLineSymbol.AddLayer(backLine);
            multiLayerLineSymbol.AddLayer(foreLine);
            return multiLayerLineSymbol;
        }

        /// <summary>
        /// 定义简单填充符号
        /// </summary>
        /// <param name="color">颜色</param>
        /// <param name="simpleFillStyle">简单填充样式</param>
        /// <param name="outLineSymbol">边界线</param>
        /// <returns></returns>
        public static ISimpleFillSymbol DefineSimpleFillSymbol(IColor color, esriSimpleFillStyle simpleFillStyle, ILineSymbol outLineSymbol)
        {
            ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbolClass();
            simpleFillSymbol.Color = color;
            simpleFillSymbol.Style = simpleFillStyle;
            simpleFillSymbol.Outline = outLineSymbol;
            return simpleFillSymbol;
        }

        #endregion
    }
}
