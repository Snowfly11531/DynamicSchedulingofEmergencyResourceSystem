using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Controls;

namespace DataManagement
{
    public class SaveVector
    {
        public static void pointtoFeatureLayer(string outfileNamePath, List<IPoint> pointlist, IFeatureLayer referenceCoordinateLayer)
        {
            //获取图层的路径和名称以及空间
            int index = outfileNamePath.LastIndexOf('\\');
            string folder = outfileNamePath.Substring(0, index);
            string shapeFileFullName = outfileNamePath.Substring(index + 1);
            IWorkspaceFactory pWSF = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace pFWS = (IFeatureWorkspace)pWSF.OpenFromFile(folder, 0);

            //获得图层的空间参考
            IFeatureLayer lineFeatureLayer = referenceCoordinateLayer;
            IFeatureClass pRFeatureClass = lineFeatureLayer.FeatureClass;
            IGeoDataset pGeoDataset = pRFeatureClass as IGeoDataset;
            ISpatialReference pSpatialReference = pGeoDataset.SpatialReference;
            //创建图层所要存储的要素类型
            IFeatureClass pFeatureClass = CreateFeaturepointClass(shapeFileFullName, pFWS, pSpatialReference);
            //要素存储到图层中
            for (int i = 0; i < pointlist.Count; i++)
            {
                IFeature pfeature = pFeatureClass.CreateFeature();
                pfeature.Shape = pointlist[i];
                pfeature.Store();
            }
        }

        public static void pointtoFeatureLayer(string outfileNamePath, List<IFeature> pFeatureList, IFeatureLayer referenceCoordinateLayer)
        {
            //获取图层的路径和名称以及空间
            int index = outfileNamePath.LastIndexOf('\\');
            string folder = outfileNamePath.Substring(0, index);
            string shapeFileFullName = outfileNamePath.Substring(index + 1);
            IWorkspaceFactory pWSF = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace pFWS = (IFeatureWorkspace)pWSF.OpenFromFile(folder, 0);

            //获得图层的空间参考
            IFeatureLayer lineFeatureLayer = referenceCoordinateLayer;
            IFeatureClass pRFeatureClass = lineFeatureLayer.FeatureClass;
            IGeoDataset pGeoDataset = pRFeatureClass as IGeoDataset;
            ISpatialReference pSpatialReference = pGeoDataset.SpatialReference;
            //创建图层所要存储的要素类型
            IFeatureClass pFeatureClass = CreateFeaturepointClass(shapeFileFullName, pFWS, pSpatialReference);
            //要素存储到图层中
            for (int i = 0; i < pFeatureList.Count; i++)
            {
                IFeature pfeature = pFeatureClass.CreateFeature();
                pfeature.Shape = pFeatureList[i].Shape;
                pfeature.Store();
            }
        }

        private static IFeatureClass CreateFeaturepointClass(String featureClassName, IFeatureWorkspace featureWorkspace, ISpatialReference spatialReference)
        {
            try
            {

                IFeatureClassDescription fcDescription = new FeatureClassDescriptionClass();
                IObjectClassDescription ocDescription = (IObjectClassDescription)fcDescription;
                IFields fields = ocDescription.RequiredFields;

                int shapeFieldIndex = fields.FindField(fcDescription.ShapeFieldName);
                IField field = fields.get_Field(shapeFieldIndex);
                IGeometryDef geometryDef = field.GeometryDef;
                IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
                geometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint;
                geometryDefEdit.SpatialReference_2 = spatialReference;

                IFeatureClass featureClass = featureWorkspace.CreateFeatureClass(featureClassName, fields,
                  ocDescription.ClassExtensionCLSID, ocDescription.InstanceCLSID, esriFeatureType.esriFTSimple,
                  fcDescription.ShapeFieldName, "");
                return featureClass;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
                return null;
            }

        }



        public static void polylinetoFeatureLayer(string outfileNamePath, List<IPolyline> pFeature,IFeatureLayer referenceCoordinateLayer)
        {
            //获取图层的路径和名称以及空间
            int index = outfileNamePath.LastIndexOf('\\');
            string folder = outfileNamePath.Substring(0, index);
            string shapeFileFullName = outfileNamePath.Substring(index + 1);
            IWorkspaceFactory pWSF = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace pFWS = (IFeatureWorkspace)pWSF.OpenFromFile(folder, 0);

            //获得图层的空间参考
            IFeatureLayer lineFeatureLayer = referenceCoordinateLayer;
            IFeatureClass pRFeatureClass = lineFeatureLayer.FeatureClass;
            IGeoDataset pGeoDataset = pRFeatureClass as IGeoDataset;
            ISpatialReference pSpatialReference = pGeoDataset.SpatialReference;
            //创建图层所要存储的要素类型
            IFeatureClass pFeatureClass = CreateFeaturepolylineClass(shapeFileFullName, pFWS, pSpatialReference);
            //要素存储到图层中
            for (int i = 0; i < pFeature.Count; i++)
            {
                IFeature pfeature = pFeatureClass.CreateFeature();
                pfeature.Shape = pFeature[i];
                pfeature.Store();
            }
        }

        public static void polylinetoFeatureLayer(string outfileNamePath, List<IFeature> pFeature, IFeatureLayer referenceCoordinateLayer)
        {
            //获取图层的路径和名称以及空间
            int index = outfileNamePath.LastIndexOf('\\');
            string folder = outfileNamePath.Substring(0, index);
            string shapeFileFullName = outfileNamePath.Substring(index + 1);
            IWorkspaceFactory pWSF = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace pFWS = (IFeatureWorkspace)pWSF.OpenFromFile(folder, 0);

            //获得图层的空间参考
            IFeatureLayer lineFeatureLayer = referenceCoordinateLayer;
            IFeatureClass pRFeatureClass = lineFeatureLayer.FeatureClass;
            IGeoDataset pGeoDataset = pRFeatureClass as IGeoDataset;
            ISpatialReference pSpatialReference = pGeoDataset.SpatialReference;
            //创建图层所要存储的要素类型
            IFeatureClass pFeatureClass = CreateFeaturepolylineClass(shapeFileFullName, pFWS, pSpatialReference);
            //要素存储到图层中
            for (int i = 0; i < pFeature.Count; i++)
            {
                IFeature pfeature = pFeatureClass.CreateFeature();
                pfeature.Shape = pFeature[i].Shape;
                pfeature.Store();
            }
        }

        private static IFeatureClass CreateFeaturepolylineClass(String featureClassName, IFeatureWorkspace featureWorkspace, ISpatialReference spatialReference)
        {
            try
            {
                

                IFeatureClassDescription fcDescription = new FeatureClassDescriptionClass();
                IObjectClassDescription ocDescription = (IObjectClassDescription)fcDescription;
                IFields fields = ocDescription.RequiredFields;

                int shapeFieldIndex = fields.FindField(fcDescription.ShapeFieldName);
                IField field = fields.get_Field(shapeFieldIndex);
                IGeometryDef geometryDef = field.GeometryDef;
                IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
                geometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolyline;
                geometryDefEdit.SpatialReference_2 = spatialReference;
                
                IFeatureClass featureClass = featureWorkspace.CreateFeatureClass(featureClassName, fields,
                  ocDescription.ClassExtensionCLSID, ocDescription.InstanceCLSID, esriFeatureType.esriFTSimple,
                  fcDescription.ShapeFieldName, "");
                return featureClass;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
                return null;
            }
            
        }



        public static void polygontoFeatureLayer(string outfileNamePath, List<IPolygon> pFeature, IFeatureLayer referenceCoordinateLayer)
        {
            //获取图层的路径和名称以及空间
            int index = outfileNamePath.LastIndexOf('\\');
            string folder = outfileNamePath.Substring(0, index);
            string shapeFileFullName = outfileNamePath.Substring(index + 1);
            IWorkspaceFactory pWSF = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace pFWS = (IFeatureWorkspace)pWSF.OpenFromFile(folder, 0);

            //获得图层的空间参考
            IFeatureLayer lineFeatureLayer = referenceCoordinateLayer;
            IFeatureClass pRFeatureClass = lineFeatureLayer.FeatureClass;
            IGeoDataset pGeoDataset = pRFeatureClass as IGeoDataset;
            ISpatialReference pSpatialReference = pGeoDataset.SpatialReference;
            //创建图层所要存储的要素类型
            IFeatureClass pFeatureClass = CreateFeaturepolygonClass(shapeFileFullName, pFWS, pSpatialReference);
            //要素存储到图层中
            for (int i = 0; i < pFeature.Count; i++)
            {
                IFeature pfeature = pFeatureClass.CreateFeature();
                pfeature.Shape = pFeature[i];
                pfeature.Store();
            }
        }

        public static void polygontoFeatureLayer(string outfileNamePath, List<IFeature> pFeature, IFeatureLayer referenceCoordinateLayer)
        {
            //获取图层的路径和名称以及空间
            int index = outfileNamePath.LastIndexOf('\\');
            string folder = outfileNamePath.Substring(0, index);
            string shapeFileFullName = outfileNamePath.Substring(index + 1);
            IWorkspaceFactory pWSF = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace pFWS = (IFeatureWorkspace)pWSF.OpenFromFile(folder, 0);

            //获得图层的空间参考
            IFeatureLayer lineFeatureLayer = referenceCoordinateLayer;
            IFeatureClass pRFeatureClass = lineFeatureLayer.FeatureClass;
            IGeoDataset pGeoDataset = pRFeatureClass as IGeoDataset;
            ISpatialReference pSpatialReference = pGeoDataset.SpatialReference;
            //创建图层所要存储的要素类型
            IFeatureClass pFeatureClass = CreateFeaturepolygonClass(shapeFileFullName, pFWS, pSpatialReference);
            //要素存储到图层中
            for (int i = 0; i < pFeature.Count; i++)
            {
                IFeature pfeature = pFeatureClass.CreateFeature();
                pfeature.Shape = pFeature[i].Shape;
                pfeature.Store();
            }
        }

        private static IFeatureClass CreateFeaturepolygonClass(String featureClassName, IFeatureWorkspace featureWorkspace, ISpatialReference spatialReference)
        {
            try
            {


                IFeatureClassDescription fcDescription = new FeatureClassDescriptionClass();
                IObjectClassDescription ocDescription = (IObjectClassDescription)fcDescription;
                IFields fields = ocDescription.RequiredFields;

                int shapeFieldIndex = fields.FindField(fcDescription.ShapeFieldName);
                IField field = fields.get_Field(shapeFieldIndex);
                IGeometryDef geometryDef = field.GeometryDef;
                IGeometryDefEdit geometryDefEdit = (IGeometryDefEdit)geometryDef;
                geometryDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon;
                geometryDefEdit.SpatialReference_2 = spatialReference;

                IFeatureClass featureClass = featureWorkspace.CreateFeatureClass(featureClassName, fields,
                  ocDescription.ClassExtensionCLSID, ocDescription.InstanceCLSID, esriFeatureType.esriFTSimple,
                  fcDescription.ShapeFieldName, "");
                return featureClass;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
                return null;
            }

        }
    }
}
