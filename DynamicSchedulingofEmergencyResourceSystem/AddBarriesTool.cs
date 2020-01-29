using System;
using System.Drawing;
using System.Runtime.InteropServices;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;

namespace DynamicSchedulingofEmergencyResourceSystem
{
    /// <summary>
    /// Summary description for AddBarriesTool.
    /// </summary>
    [Guid("e4c19045-812f-449f-9b70-405eb3fc3744")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("DynamicSchedulingofEmergencyResourceSystem.AddBarriesTool")]
    public sealed class AddBarriesTool : BaseTool
    {
        #region COM Registration Function(s)
        [ComRegisterFunction()]
        [ComVisible(false)]
        static void RegisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryRegistration(registerType);

            //
            // TODO: Add any COM registration code here
            //
        }

        [ComUnregisterFunction()]
        [ComVisible(false)]
        static void UnregisterFunction(Type registerType)
        {
            // Required for ArcGIS Component Category Registrar support
            ArcGISCategoryUnregistration(registerType);

            //
            // TODO: Add any COM unregistration code here
            //
        }

        #region ArcGIS Component Category Registrar generated code
        /// <summary>
        /// Required method for ArcGIS Component Category registration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryRegistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Register(regKey);

        }
        /// <summary>
        /// Required method for ArcGIS Component Category unregistration -
        /// Do not modify the contents of this method with the code editor.
        /// </summary>
        private static void ArcGISCategoryUnregistration(Type registerType)
        {
            string regKey = string.Format("HKEY_CLASSES_ROOT\\CLSID\\{{{0}}}", registerType.GUID);
            MxCommands.Unregister(regKey);

        }

        #endregion
        #endregion

        private IHookHelper m_hookHelper = null;
        private IApplication m_application;
        private IFeatureClass pFeatureClass;
        public AddBarriesTool()
        {
            //
            // TODO: Define values for the public properties
            //
            base.m_category = ""; //localizable text 
            base.m_caption = "添加障碍";  //localizable text 
            base.m_message = "鼠标在地图上单击即可";  //localizable text
            base.m_toolTip = "添加障碍";  //localizable text
            base.m_name = "AddBarriesTool";   //unique id, non-localizable (e.g. "MyCategory_ArcMapTool")
            try
            {
                //
                // TODO: change resource name if necessary
                //
                string bitmapResourceName = GetType().Name + ".bmp";
                base.m_bitmap = new Bitmap(GetType(), bitmapResourceName);
                base.m_cursor = new System.Windows.Forms.Cursor(GetType(), GetType().Name + ".cur");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message, "Invalid Bitmap");
            }
        }

        #region Overridden Class Methods

        /// <summary>
        /// Occurs when this tool is created
        /// </summary>
        /// <param name="hook">Instance of the application</param>
        public override void OnCreate(object hook)
        {
            try
            {
                m_hookHelper = new HookHelperClass();
                m_hookHelper.Hook = hook;
                if (m_hookHelper.ActiveView == null)
                {
                    m_hookHelper = null;
                }
            }
            catch (Exception ex)
            {
                m_hookHelper = null;
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }

            //Disable if it is not ArcMap
            if (hook is IMxApplication)
                base.m_enabled = true;
            else
                base.m_enabled = false;

            // TODO:  Add other initialization code
        }

        /// <summary>
        /// Occurs when this tool is clicked
        /// </summary>
        public override void OnClick()
        {
            // TODO: Add AddBarriesTool.OnClick implementation
            IFeatureLayer pFeatureLayer=CDataImport.ImportFeatureLayerFromControltext(@"C:\Users\Administrator\Desktop\突发环境事件应急资源调度系统\data\Barries.shp");
            pFeatureClass=pFeatureLayer.FeatureClass;
            if(pFeatureClass.FeatureCount(null)>0)
            {
                ITable pTable=pFeatureClass as ITable;
                pTable.DeleteSearchedRows(null);
            }
        }

        public override void OnMouseDown(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add AddBarriesTool.OnMouseDown implementation
            try
            {
                IPoint pStopsPoint = new PointClass();
                pStopsPoint = m_hookHelper.ActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(X, Y);
                IFeature newPointFeature=pFeatureClass.CreateFeature();
                try
                {
                    newPointFeature.Shape=pStopsPoint;
                }
                catch
                {
                    IGeometry pGeo=pStopsPoint;
                    IZAware pZAware=pGeo as IZAware;
                    pZAware.ZAware=false;
                    newPointFeature.Shape=pGeo;
                }
                newPointFeature.Store();
                IGraphicsContainer pGrap = m_hookHelper.ActiveView as IGraphicsContainer;
                IColor pColor;
                IRgbColor pRgbColor = new RgbColorClass();
                pRgbColor.Red = 255;
                pRgbColor.Green = 255;
                pRgbColor.Blue = 255;
                pColor = pRgbColor as IColor;
                IPictureMarkerSymbol pms = new PictureMarkerSymbolClass();
                pms.BitmapTransparencyColor = pColor;
                pms.CreateMarkerSymbolFromFile(esriIPictureType.esriIPicturePNG, @"C:\Users\Administrator\Desktop\突发环境事件应急资源调度系统\DynamicSchedulingofEmergencyResourceSystem\DynamicSchedulingofEmergencyResourceSystem\Resources\barries.png");
                pms.Size = 18;
                IMarkerElement pMarkerEle = new MarkerElementClass();
                pMarkerEle.Symbol = pms as IMarkerSymbol;
                pStopsPoint.SpatialReference = m_hookHelper.ActiveView.FocusMap.SpatialReference;
                IElement pEle = pMarkerEle as IElement;
                pEle.Geometry = pStopsPoint;
                pGrap.AddElement(pEle, 1);
                m_hookHelper.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        public override void OnMouseMove(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add AddBarriesTool.OnMouseMove implementation
        }

        public override void OnMouseUp(int Button, int Shift, int X, int Y)
        {
            // TODO:  Add AddBarriesTool.OnMouseUp implementation
        }
        #endregion
    }
}
