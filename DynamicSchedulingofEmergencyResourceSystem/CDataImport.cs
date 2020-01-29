using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.DataSourcesFile;


namespace DynamicSchedulingofEmergencyResourceSystem
{
    public class CDataImport
    {
        //将从下来ComboBox下拉框文件路径添加地图图层到系统中
        public static IFeatureLayer ImportFeatureLayerFromControltext(string strControltext)
        {
            try
            {
                IFeatureLayer pFeatureLayer = null;
                if (JudgeControltextType(strControltext) == 1)
                {
                    string strFilePath = strControltext;
                    string strFileDirectory = strFilePath.Substring(0, strFilePath.LastIndexOf('\\'));
                    string strFileName = strFilePath.Substring(strFilePath.LastIndexOf('\\') + 1);
                    pFeatureLayer = ImportFeatrueLayerFromFilePath(strFileDirectory, strFileName);
                }
                else if (JudgeControltextType(strControltext) == 2)
                {
                    for (int i = 0; i < Variable.pMapFrm.mainMapControl.LayerCount; i++)
                    {
                        if (strControltext == Variable.pMapFrm.mainMapControl.get_Layer(i).Name)
                        {
                            pFeatureLayer = Variable.pMapFrm.mainMapControl.get_Layer(i) as IFeatureLayer;
                        }
                    }
                }
                return pFeatureLayer;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
                return null;
            }
        }

        //将从下来ComboBox下拉框文件路径添加地图图层到系统中
        public static IRasterLayer ImportRasterLayerFromControltext(string strControltext)
        {
            IRasterLayer pRasterLayer = null;
            if (JudgeControltextType(strControltext) == 1)
            {
                string strFilePath = strControltext;
                string strFileDirectory = strFilePath.Substring(0, strFilePath.LastIndexOf('\\'));
                string strFileName = strFilePath.Substring(strFilePath.LastIndexOf('\\') + 1);
                pRasterLayer = ImportRasterLayerFromFilePath(strFileDirectory, strFileName);
            }
            else if (JudgeControltextType(strControltext) == 2)
            {
                for (int i = 0; i < Variable.pMapFrm.mainMapControl.LayerCount; i++)
                {
                    if (strControltext == Variable.pMapFrm.mainMapControl.get_Layer(i).Name)
                    {
                        pRasterLayer = Variable.pMapFrm.mainMapControl.get_Layer(i) as IRasterLayer;
                    }
                }
            }
            return pRasterLayer;
        }

        //将地图中的栅格图层添加到ComboBox下拉框
        public static void AddRasterLayerNameToComboBox(ComboBox pComboBox)
        {
            if (Variable.pMapFrm.mainMapControl.LayerCount != 0)
            {
                for (int i = 0; i < Variable.pMapFrm.mainMapControl.LayerCount; i++)
                {
                    ILayer pLayer = Variable.pMapFrm.mainMapControl.get_Layer(i);
                    if (pLayer is IRasterLayer)
                    {
                        pComboBox.Items.Add(Variable.pMapFrm.mainMapControl.get_Layer(i).Name.ToString());
                    }                    
                }
            }
        }     

        //判断添加数据是从文件路径还是从主视图中的文件明名
        public static int JudgeControltextType(string strControlText)
        {
            int iControltextType = 0;
            try
            {
                if(File.Exists(strControlText))
                {
                    iControltextType=1;
                }
                else
                {
                    for (int i = 0; i < Variable.pMapFrm.mainMapControl.LayerCount; i++)
                    {
                        if(strControlText==Variable.pMapFrm.mainMapControl.get_Layer(i).Name)
                        {
                            iControltextType = 2;
                        }
                    }
                }
                return iControltextType;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(),"异常");
                return iControltextType;
            }
        }
     
        //从文件路径中添加矢量图层数据
        public static IFeatureLayer ImportFeatrueLayerFromFilePath(string strFilePath,string strFileName)
        {
            try
            {
                IFeatureWorkspace pFeatureWorkspace = GetShapeWorkspaceFromPath(strFilePath) as IFeatureWorkspace;
                IFeatureLayer pFeatureLayer = new FeatureLayerClass();
                pFeatureLayer.FeatureClass = pFeatureWorkspace.OpenFeatureClass(strFileName);
                pFeatureLayer.Name = pFeatureLayer.FeatureClass.AliasName;
                return pFeatureLayer;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
                return null;
            }
        }

        //从路径获取矢量图层的工作空间
        public static IWorkspace GetShapeWorkspaceFromPath(string strPath)
        {
            try
            {
                IWorkspaceFactory pWorkspaceFatory = new ShapefileWorkspaceFactoryClass();
                IWorkspace pWorkspace = pWorkspaceFatory.OpenFromFile(strPath, 0);
                return pWorkspace;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
                return null;
            }
        }

        //从路径获取栅格图层的工作空间
        public static IWorkspace GetRasterWorkspaceFromPath(string strPath)
        {
            try 
            {
                IWorkspaceFactory pRasterWorkspaceFactory = new RasterWorkspaceFactoryClass();
                IWorkspace pWorkspace = pRasterWorkspaceFactory.OpenFromFile(strPath, 0);
                return pWorkspace;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
                return null;
            }
        }

        //从栅格数据集中创建栅格图层
        public static IRasterLayer GetRasterLayerFromRasterDataset(IRasterDataset pRasterDataset)
        {
            try 
            {
                IRasterLayer pRasterLayer = new RasterLayerClass();
                pRasterLayer.CreateFromDataset(pRasterDataset);
                return pRasterLayer;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
                return null;
            }
        }

        //从文件路径中添加栅格图层数据
        public static IRasterLayer ImportRasterLayerFromFilePath(string strFilePath, string strFileName)
        {
            try
            {
                IRasterWorkspace pRasterWorkspace = GetRasterWorkspaceFromPath(strFilePath) as IRasterWorkspace;
                IRasterDataset pRasterDataset = pRasterWorkspace.OpenRasterDataset(strFileName);
                IRasterLayer pRasterLayer = new RasterLayerClass();
                pRasterLayer = GetRasterLayerFromRasterDataset(pRasterDataset);
                return pRasterLayer;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(),"异常");
                return null;
            }
        }

    }
}
