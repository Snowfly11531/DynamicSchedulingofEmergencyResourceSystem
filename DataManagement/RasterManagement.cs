using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Controls;

namespace DataManagement
{
    public class RasterManagement
    {

        //XY坐标转换成栅格行列号
        public static void XYconvertNumber(IRasterLayer pRasterLayer, double X, double Y, ref int column, ref int row)
        {
            try
            {
                IRaster pRaster = pRasterLayer.Raster;
                IRaster2 pRaster2 = pRaster as IRaster2;
                pRaster2.MapToPixel(X, Y, out column, out row);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        //栅格行列号转换成XY坐标
        public static void NumbercovertXY(IRasterLayer pRasterLayer, ref double X, ref double Y, int column, int row)
        {
            try
            {
                IRaster pRaster = pRasterLayer.Raster;
                IRaster2 pRaster2 = pRaster as IRaster2;
                pRaster2.PixelToMap(column, row, out X, out Y);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        //获取某行列栅格单元的值
        public static object GetPixelValue(IRasterLayer pRasterLayer, int iBand, int column, int row)
        {
            try
            {
                IRaster pRaster = pRasterLayer.Raster;
                IRaster2 pRaster2 = pRaster as IRaster2;
                return pRaster2.GetPixelValue(iBand, column, row);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
                return null;
            }
        }

        //获取栅格图层的行列数
        public static void GetRasterCount(IRasterLayer pRasterLayer, ref int rowCount, ref int colCount)
        {
            try
            {
                IRaster pRaster = pRasterLayer.Raster;
                IRasterProps pRasterProps = pRaster as IRasterProps;
                rowCount = pRasterProps.Height;
                colCount = pRasterProps.Width;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }
    }
}
