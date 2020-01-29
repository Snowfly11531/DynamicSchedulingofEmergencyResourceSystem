using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Tool;
using FeatureEditor;
using IdentifyTool;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesRaster;
using DevExpress.XtraEditors.Repository;


namespace DynamicSchedulingofEmergencyResourceSystem
{
    public partial class MainFrm : DevExpress.XtraBars.Ribbon.RibbonForm
    {                   
        private ICommand pICommand = null;  //地图浏览工具
       
        public MainFrm()
        {
            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);            
            InitializeComponent();
            Variable.pEagleEyeMapFrm = new EagleEyeMapFrm();
            this.ShowMapFrm();
            this.ShowTOCFrm();
            //设置数据管理中要素管理未开始编辑状态，图层选择、添加要素、选择编辑要素、要素属性编辑、结束编辑等功能变成灰度，使得无法操作
            barEditItem1.Enabled = false;
            barButtonItem12.Enabled = false;
            barButtonItem14.Enabled = false;
            barButtonItem13.Enabled = false;
            barButtonItem15.Enabled = false;
            barButtonItem4.Enabled = false;
            barButtonItem20.Enabled = false;
            barButtonItem22.Enabled = false;
            barButtonItem21.Enabled = false;
            barButtonItem23.Enabled = false;                      
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        // 显示 DockContent(可停靠窗体)
        public void ShowDockContent(DockContent dockContent)
        {
            switch (dockContent.DockState)
            {
                case DockState.DockLeftAutoHide:
                    dockContent.Show(dockPanel1, DockState.DockLeft);
                    break;
                case DockState.DockRightAutoHide:
                    dockContent.Show(dockPanel1, DockState.DockRight);
                    break;
                case DockState.DockTopAutoHide:
                    dockContent.Show(dockPanel1, DockState.DockTop);
                    break;
                case DockState.DockBottomAutoHide:
                    dockContent.Show(dockPanel1, DockState.DockBottom);
                    break;
                case DockState.Hidden:
                    dockContent.Show(dockPanel1,DockState.Float);
                    break;
                default:
                    dockContent.Show(dockPanel1);
                    break;
            }
        }
      
        // 显示 DockContent(可停靠窗体)
        public void ShowDockContent(DockContent dockContent, DockState dockState)
        {
            dockContent.Show(dockPanel1, dockState);
        }
      
        // 显示地图窗口
        public void ShowMapFrm()
        {
            if (Variable.pMapFrm == null || Variable.pMapFrm.IsDisposed)
            {
                Variable.pMapFrm = new MapFrm();
                Variable.pMapFrm.CurrentTool = MapTools.None;
                this.ShowDockContent(Variable.pMapFrm, DockState.Document);
                EagleEyeClass.SynchronizeEagleEye();
            }
            else
            {
                this.ShowDockContent(Variable.pMapFrm, DockState.Document);
                EagleEyeClass.SynchronizeEagleEye();
            }
                
        }

        // 显示图层窗口
        public void ShowTOCFrm()
        {
            if (Variable.pTOCFrm == null || Variable.pTOCFrm.IsDisposed)
            {
                Variable.pTOCFrm = new TOCFrm();
                Variable.pTOCFrm.SetBuddyControl(Variable.pMapFrm.mainMapControl);
                
                this.ShowDockContent(Variable.pTOCFrm, DockState.DockLeftAutoHide);
                Variable.pTOCFrm.AutoHidePortion = 0.17;
                Variable.pTOCFrm.DockPanel.DockLeftPortion = 0.17;
            }
            else
                this.ShowDockContent(Variable.pTOCFrm);
        }

        // 显示鹰眼窗口
        public void ShowEagleEye()
        {
            if (Variable.pEagleEyeMapFrm == null || Variable.pEagleEyeMapFrm.IsDisposed)
            {
                Variable.pEagleEyeMapFrm = new EagleEyeMapFrm();               
                this.ShowDockContent(Variable.pEagleEyeMapFrm,DockState.Float);
            }
            else
                this.ShowDockContent(Variable.pEagleEyeMapFrm,DockState.Float);
        }

        //打开地图文档
        private void barButtonItem25_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ICommand command = new ControlsOpenDocCommandClass();
            command.OnCreate(Variable.pMapFrm.mainMapControl.Object);
            command.OnClick();
        }

        //保存地图文档
        private void barButtonItem26_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                string sMxdFileName = Variable.pMapFrm.mainMapControl.DocumentFilename;
                IMapDocument pMapDocument = new MapDocumentClass();
                if (sMxdFileName != null && Variable.pMapFrm.mainMapControl.CheckMxFile(sMxdFileName))
                {
                    if (pMapDocument.get_IsReadOnly(sMxdFileName))
                    {
                        MessageBox.Show("本地图文档是只读的，不能保存!");
                        pMapDocument.Close();
                        return;
                    }
                }
                else
                {
                    SaveFileDialog pSaveFileDialog = new SaveFileDialog();
                    pSaveFileDialog.Title = "请选择保存路径";
                    pSaveFileDialog.OverwritePrompt = true;
                    pSaveFileDialog.Filter = "ArcMap文档（*.mxd）|*.mxd|ArcMap模板（*.mxt）|*.mxt";
                    pSaveFileDialog.RestoreDirectory = true;
                    if (pSaveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        sMxdFileName = pSaveFileDialog.FileName;
                    }
                    else
                    {
                        return;
                    }
                }

                pMapDocument.New(sMxdFileName);
                pMapDocument.ReplaceContents(Variable.pMapFrm.mainMapControl.Map as IMxdContents);
                pMapDocument.Save(pMapDocument.UsesRelativePaths, true);
                pMapDocument.Close();
                MessageBox.Show("保存地图文档成功!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "/n" + ex.ToString(), "异常");
            }
        }

        //另存为地图文档
        private void barButtonItem27_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog pSaveFileDialog = new SaveFileDialog();
                pSaveFileDialog.Title = "另存为";
                pSaveFileDialog.OverwritePrompt = true;
                pSaveFileDialog.Filter = "ArcMap文档（*.mxd）|*.mxd|ArcMap模板（*.mxt）|*.mxt";
                pSaveFileDialog.RestoreDirectory = true;
                if (pSaveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string sFilePath = pSaveFileDialog.FileName;

                    IMapDocument pMapDocument = new MapDocumentClass();
                    pMapDocument.New(sFilePath);
                    pMapDocument.ReplaceContents(Variable.pMapFrm.mainMapControl.Map as IMxdContents);
                    pMapDocument.Save(true, true);
                    pMapDocument.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "/n" + ex.ToString(), "异常");
            }
        }


        //添加矢量和栅格数据
        private void barButtonItem28_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.CheckFileExists = true;
                openFileDialog.Title = "打开Shape和Raster文件";
                openFileDialog.Filter = "ESRI Shapefile（*.shp）|*.shp|栅格文件 (*.*)|*.bmp;*.tif;*.jpg;*.img|(*.bmp)|*.bmp|(*.tif)|*.tif|(*.jpg)|*.jpg|(*.img)|*.img";


                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string pFullPath = openFileDialog.FileName;
                    if (pFullPath == "") return;
                    int pIndex = pFullPath.LastIndexOf("\\");
                    string pFilePath = pFullPath.Substring(0, pIndex); //文件路径
                    string pFileName = pFullPath.Substring(pIndex + 1); //文件名
                    if (pFileName.Contains("shp"))
                    {
                        IWorkspaceFactory pWorkspaceFactory;
                        IFeatureWorkspace pFeatureWorkspace;
                        IFeatureLayer pFeatureLayer;



                        //实例化ShapefileWorkspaceFactory工作空间，打开Shape文件
                        pWorkspaceFactory = new ShapefileWorkspaceFactory();
                        pFeatureWorkspace = (IFeatureWorkspace)pWorkspaceFactory.OpenFromFile(pFilePath, 0);
                        //创建并实例化要素集
                        IFeatureClass pFeatureClass = pFeatureWorkspace.OpenFeatureClass(pFileName);
                        pFeatureLayer = new FeatureLayer();
                        pFeatureLayer.FeatureClass = pFeatureClass;
                        pFeatureLayer.Name = pFeatureLayer.FeatureClass.AliasName;

                        //ClearAllData();    //新增删除数据

                        Variable.pMapFrm.mainMapControl.Map.AddLayer(pFeatureLayer);
                        Variable.pMapFrm.mainMapControl.ActiveView.Refresh();
                    }
                    else
                    {
                        IWorkspaceFactory pWorkspaceFactory = new RasterWorkspaceFactory();
                        IWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile(pFilePath, 0);
                        IRasterWorkspace pRasterWorkspace = pWorkspace as IRasterWorkspace;
                        IRasterDataset pRasterDataset = pRasterWorkspace.OpenRasterDataset(pFileName);
                        //影像金字塔判断与创建
                        IRasterPyramid3 pRasPyrmid;
                        pRasPyrmid = pRasterDataset as IRasterPyramid3;
                        if (pRasPyrmid != null)
                        {
                            if (!(pRasPyrmid.Present))
                            {
                                pRasPyrmid.Create(); //创建金字塔
                            }
                        }
                        IRaster pRaster;
                        pRaster = pRasterDataset.CreateDefaultRaster();
                        IRasterLayer pRasterLayer;
                        pRasterLayer = new RasterLayerClass();
                        pRasterLayer.CreateFromRaster(pRaster);
                        ILayer pLayer = pRasterLayer as ILayer;
                        Variable.pMapFrm.mainMapControl.AddLayer(pLayer, 0);
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }


        // 放大
        private void bBI_ZoomIn_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            pICommand = new ControlsMapZoomInToolClass();
            pICommand.OnCreate(Variable.pMapFrm.mainMapControl.Object);
            Variable.pMapFrm.mainMapControl.CurrentTool = pICommand as ITool;
        }

        // 缩小
        private void bBI_ZoomOut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            pICommand = new ControlsMapZoomOutToolClass();
            pICommand.OnCreate(Variable.pMapFrm.mainMapControl.Object);
            Variable.pMapFrm.mainMapControl.CurrentTool = pICommand as ITool;
        }

        // 漫游
        private void bBI_MapPan_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            pICommand = new ControlsMapPanToolClass();
            pICommand.OnCreate(Variable.pMapFrm.mainMapControl.Object);
            Variable.pMapFrm.mainMapControl.CurrentTool = pICommand as ITool;
        }

        // 全图
        private void bBI_MapFullExtent_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            pICommand = new ControlsMapFullExtentCommandClass();
            pICommand.OnCreate(Variable.pMapFrm.mainMapControl.Object);
            pICommand.OnClick();
        }

        // 前一视图    
        private void bBI_LastExtentBack_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            pICommand = new ControlsMapZoomToLastExtentBackCommandClass();
            pICommand.OnCreate(Variable.pMapFrm.mainMapControl.Object);
            pICommand.OnClick();
        }

        // 后一视图     
        private void bBI_LastExtentForward_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            pICommand = new ControlsMapZoomToLastExtentForwardCommandClass();
            pICommand.OnCreate(Variable.pMapFrm.mainMapControl.Object);
            pICommand.OnClick();
        }

        // 显示图层窗口
        private void bBI_ShowTOCFrm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.ShowTOCFrm();
        }

        // 显示鹰眼窗口
        private void barButtonItem10_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (Variable.pMapFrm.mainMapControl.Map.LayerCount == 0)
            {
                Variable.pEnvelop = Variable.pEagleEyeMapFrm.EagelEyeMapControl.FullExtent;
            }
            this.ShowEagleEye();       
            EagleEyeClass.SynchronizeEagleEye();
            EagleEyeClass.DrawRectangle(Variable.pEnvelop);
            
        }

        // 退出系统
        private void barButtonItem30_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Application.Exit();
        }


        private IMap pMap = null;
        private IList<ILayer> pLayerlist = null;
        private IActiveView pActiveView = null;
        private IEngineEditor pEngineEditor = null;
        private IEngineEditTask pEngineEditTask = null;
        private IEngineEditLayers pEngineEditLayers = null;
        private IFeatureLayer pCurrentLayer = null;

        private void barButtonItem8_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            pEngineEditor = new EngineEditorClass();
            MapManagement.EngineEditor = pEngineEditor;
            pEngineEditTask = pEngineEditor as IEngineEditTask;
            pEngineEditLayers = pEngineEditor as IEngineEditLayers;

            pMap = Variable.pMapFrm.mainMapControl.Map;
            pActiveView = pMap as IActiveView;
            pLayerlist = MapManagement.GetMapLayers(pMap);
            try
            {
                if (pLayerlist == null || pLayerlist.Count == 0)
                {
                    MessageBox.Show("请在主视图中加载编辑图层！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                for (int i = 0; i < pLayerlist.Count; i++)
                {
                    ((DevExpress.XtraEditors.Repository.RepositoryItemComboBox)this.barEditItem1.Edit).Items.Add(pLayerlist[i].Name);
                }
                this.barEditItem1.EditValue = ((DevExpress.XtraEditors.Repository.RepositoryItemComboBox)this.barEditItem1.Edit).Items[0];

                pMap.ClearSelection();
                pActiveView.Refresh();
                //设置数据管理中要素管理未开始编辑状态，图层选择、添加要素、选择编辑要素、要素属性编辑、结束编辑等功能变成正常状态，使得可以操作。
                barEditItem1.Enabled = true;
                barButtonItem12.Enabled = true;
                barButtonItem14.Enabled = true;
                barButtonItem13.Enabled = true;
                barButtonItem15.Enabled = true;
                barButtonItem4.Enabled = true;
                barButtonItem20.Enabled = true;
                barButtonItem22.Enabled = true;
                barButtonItem21.Enabled = true;
                barButtonItem23.Enabled = true;
                if (pEngineEditor.EditState != esriEngineEditState.esriEngineStateNotEditing)
                {
                    return;
                }
                if (pCurrentLayer == null)
                {
                    return;
                }

                IDataset pDataSet = pCurrentLayer.FeatureClass as IDataset;
                IWorkspace pWorkspace = pDataSet.Workspace;
                if (pWorkspace.Type == esriWorkspaceType.esriRemoteDatabaseWorkspace)
                {
                    pEngineEditor.EditSessionMode = esriEngineEditSessionMode.esriEngineEditSessionModeVersioned;
                }
                else
                {
                    pEngineEditor.EditSessionMode = esriEngineEditSessionMode.esriEngineEditSessionModeVersioned;
                }
                pEngineEditTask = pEngineEditor.GetTaskByUniqueName("ControlToolsEditing_CreateNewFeatureTask");
                pEngineEditor.CurrentTask = pEngineEditTask;
                pEngineEditor.EnableUndoRedo(true);
                pEngineEditor.StartEditing(pWorkspace, pMap);




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        //添加要素
        private void barButtonItem12_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //Variable.pMapFrm.mainMapControl.MousePointer = esriControlsMousePointer.esriPointerCrosshair;
                ICommand m_CreateFeatTool = new CreateFeatureToolClass();
                m_CreateFeatTool.OnCreate(Variable.pMapFrm.mainMapControl.Object);
                m_CreateFeatTool.OnClick();
                Variable.pMapFrm.mainMapControl.CurrentTool = m_CreateFeatTool as ITool;
                Variable.pMapFrm.mainMapControl.MousePointer = esriControlsMousePointer.esriPointerCrosshair;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
            }
        }

        //获取图层选择的选择框中的图层的名字
        private void barEditItem1_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                string strLayerName = this.barEditItem1.EditValue.ToString();
                pCurrentLayer = MapManagement.GetLayerByName(pMap, strLayerName) as IFeatureLayer;
                pEngineEditLayers.SetTargetLayer(pCurrentLayer, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
            }
        }

        //删除要素
        private void barButtonItem4_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                Variable.pMapFrm.mainMapControl.MousePointer = esriControlsMousePointer.esriPointerArrow;
                ICommand m_delFeatCom = new DeleteFeatureCommandClass();
                m_delFeatCom.OnCreate(Variable.pMapFrm.mainMapControl.Object);
                m_delFeatCom.OnClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
            }
        }

        //要素选择
        private void barButtonItem14_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ICommand m_SelTool = new SelectFeatureToolClass();
                m_SelTool.OnCreate(Variable.pMapFrm.mainMapControl.Object);
                m_SelTool.OnClick();
                Variable.pMapFrm.mainMapControl.CurrentTool = m_SelTool as ITool;
                Variable.pMapFrm.mainMapControl.MousePointer = esriControlsMousePointer.esriPointerHand;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
            }
        }

        //属性编辑
        private void barButtonItem13_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ICommand m_AtrributeCom = new EditAtrributeToolClass();
                m_AtrributeCom.OnCreate(Variable.pMapFrm.mainMapControl.Object);
                m_AtrributeCom.OnClick();
                Variable.pMapFrm.mainMapControl.CurrentTool = m_AtrributeCom as ITool;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
            }
        }

        //保存要素编辑
        private void barButtonItem20_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ICommand m_saveEditCom = new SaveEditCommandClass();
                m_saveEditCom.OnCreate(Variable.pMapFrm.mainMapControl.Object);
                m_saveEditCom.OnClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
            }
        }

        //结束编辑
        private void barButtonItem15_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ICommand m_stopEditCom = new StopEditCommandClass();
                m_stopEditCom.OnCreate(Variable.pMapFrm.mainMapControl.Object);
                m_stopEditCom.OnClick();
                Variable.pMapFrm.mainMapControl.CurrentTool = null;
                Variable.pMapFrm.mainMapControl.MousePointer = esriControlsMousePointer.esriPointerDefault;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
            }

            //设置数据管理中要素管理未开始编辑状态，图层选择、添加要素、选择编辑要素、要素属性编辑、结束编辑等功能变成灰色，使得无法进行操作
            barEditItem1.Enabled = false;
            barButtonItem12.Enabled = false;
            barButtonItem14.Enabled = false;
            barButtonItem13.Enabled = false;
            barButtonItem15.Enabled = false;
            barButtonItem4.Enabled = false;
            barButtonItem20.Enabled = false;
            barButtonItem22.Enabled = false;
            barButtonItem21.Enabled = false;
            barButtonItem23.Enabled = false;
        }

        //撤销
        private void barButtonItem22_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ICommand m_undoCommand = new UndoCommandClass();
                m_undoCommand.OnCreate(Variable.pMapFrm.mainMapControl.Object);
                m_undoCommand.OnClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
            }
        }

        //恢复
        private void barButtonItem21_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ICommand m_redoCommand = new RedoCommandClass();
                m_redoCommand.OnCreate(Variable.pMapFrm.mainMapControl.Object);
                m_redoCommand.OnClick();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
            }
        }

        //移动要素
        private void barButtonItem23_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                ICommand m_moveTool = new MoveFeatureToolClass();
                m_moveTool.OnCreate(Variable.pMapFrm.mainMapControl.Object);
                m_moveTool.OnClick();
                Variable.pMapFrm.mainMapControl.CurrentTool = m_moveTool as ITool;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "异常");
            }
        }

        //点选查询
        IdentifyDialog pIdentifyDialog;
        private void barButtonItem16_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Variable.pMapFrm.mainMapControl.CurrentTool = null;
            pIdentifyDialog = IdentifyDialog.CreateInstance(Variable.pMapFrm.mainMapControl);
            pIdentifyDialog.Owner = this;
            pIdentifyDialog.Show();


        }

        //要素选择
        private void barButtonItem17_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ICommand pSelectionCommand = new ControlsSelectFeaturesToolClass();
            pSelectionCommand.OnCreate(Variable.pMapFrm.mainMapControl.Object);
            Variable.pMapFrm.mainMapControl.CurrentTool = pSelectionCommand as ITool;
        }

        //清除选择的要素
        private void barButtonItem18_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            IActiveView pActiveView = Variable.pMapFrm.mainMapControl.ActiveView;
            pActiveView.FocusMap.ClearSelection();
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, pActiveView.Extent);
        }

        //缩放至选择的要素
        private void barButtonItem24_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            int nSlection = Variable.pMapFrm.mainMapControl.Map.SelectionCount;
            if (nSlection == 0)
            {
                MessageBox.Show("请先选择要素！", "提示");
            }
            else
            {
                ISelection selection = Variable.pMapFrm.mainMapControl.Map.FeatureSelection;
                IEnumFeature enumFeature = (IEnumFeature)selection;
                enumFeature.Reset();
                IEnvelope pEnvelope = new EnvelopeClass();
                IFeature pFeature = enumFeature.Next();
                while (pFeature != null)
                {
                    pEnvelope.Union(pFeature.Extent);
                    pFeature = enumFeature.Next();
                }
                pEnvelope.Expand(1.1, 1.1, true);
                Variable.pMapFrm.mainMapControl.ActiveView.Extent = pEnvelope;
                Variable.pMapFrm.mainMapControl.ActiveView.Refresh();
            }
        }

        //打开计算可能已经污染的水系界面
        private void barButtonItem11_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FrmUpstream pUpstream = new FrmUpstream();
            pUpstream.Show();
        }

        //打开计算可能即将污染的水系界面
        private void barButtonItem19_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FrmDownStream pDownStream = new FrmDownStream();
            pDownStream.Show();
        }

        //打开寻找污染流域的界面
        private void barButtonItem29_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FrmPollutionWatershed pPollutionWatershed = new FrmPollutionWatershed();
            pPollutionWatershed.Show();
        }

        //打开寻找可能排放污染物的污染源界面
        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FrmPossiblePollutionsource pPossiblepollutionsource = new FrmPossiblePollutionsource();
            pPossiblepollutionsource.Show();
        }        

        
      

        //打开计算应急资源调度界面
        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FrmEmergencyResourceSchedule pEmergencyResourceSchedule = new FrmEmergencyResourceSchedule();
            pEmergencyResourceSchedule.Show();
        }

        private void barButtonItem31_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {          
            DijkstraClass.DijkstraDepotToDestMethod.deleteElement();
        }

       

        //应急处置空间位置的选择
        private void barButtonItem33_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FrmPossibleLocationofEmergencyDisposalSpace pLoacationofemergencydisposalspace = new FrmPossibleLocationofEmergencyDisposalSpace();
            pLoacationofemergencydisposalspace.Show();
        }

        //应急资源需求设置
        private void barButtonItem34_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FrmSetEmergencyResourceDemand pSetEmergencyResourceDemand = new FrmSetEmergencyResourceDemand();
            pSetEmergencyResourceDemand.Show();
        }

        //应急处置空间位置选择
        private void barButtonItem35_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FrmLocationofEmergencyDisposalSpace pLocationofEmergencyDisposalSpace = new FrmLocationofEmergencyDisposalSpace();
            pLocationofEmergencyDisposalSpace.Show();
        }

        //应急资源调度过程动态路径规划
        private void barButtonItem36_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FrmDynamicRoutePlan pDynamicEmergencyResourceSchedule = new FrmDynamicRoutePlan();
            pDynamicEmergencyResourceSchedule.Show();
        }

        //第一应急处置空间位置选择
        private void barButtonItem37_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FrmFirstEmergencyResourceSchedule pFirstEmergencyResourceSchedule = new FrmFirstEmergencyResourceSchedule();
            pFirstEmergencyResourceSchedule.Show();
        }

        //读取应急处置空间位置信息
        private void barButtonItem38_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FrmEmergencyDispositionPointInformation pEmergencyDispositionPointInformation = new FrmEmergencyDispositionPointInformation();
            pEmergencyDispositionPointInformation.Show();
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            DijkstraClass.DijkstraDepotToDestMethod.deleteElement();
        }

        private void ribbonControl1_Click(object sender, EventArgs e)
        {

        }                       
    }
}
