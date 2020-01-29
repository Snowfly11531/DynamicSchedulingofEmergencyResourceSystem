using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Tool;
using WeifenLuo.WinFormsUI.Docking;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.SystemUI;
using DataManagement;
using AttributeTable;


namespace DynamicSchedulingofEmergencyResourceSystem
{
    public partial class TOCFrm : DockContent
    {
        private object pTOCBuddy;
        

        public TOCFrm()
        {
            InitializeComponent();
        }

        // TOCControl 控件
        public AxTOCControl TOCControl
        {
            get { return axTOCControl1; }
        }

        // 设置 Buddy 控件
        public void SetBuddyControl(object pTOCBuddy)
        {
            axTOCControl1.SetBuddyControl(pTOCBuddy);
            this.pTOCBuddy = pTOCBuddy;
        }

        private esriTOCControlItem ItemType = esriTOCControlItem.esriTOCControlItemNone;
        private IBasicMap BasicMap;
        private ILayer Layer;
        private object Unk;
        private object Data;
        private void axTOCControl1_OnMouseUp(object sender, ITOCControlEvents_OnMouseUpEvent e)
        {
            try
            {
                ITOCControl2 tOCControl2 = (ITOCControl2)axTOCControl1.Object;
                tOCControl2.HitTest(e.x,e.y,ref ItemType,ref BasicMap,ref Layer,ref Unk,ref Data);
                if (e.button != 2)
                    return;
                contextMenuStrip1.Show(MousePosition);//弹出快捷菜单               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
            
        }

        // 打开图层属性表
        private void TSMI_OpenAttributeTable_Click(object sender, EventArgs e)
        {
            try
            {      
                Variable.pAttributeTableFrm = new AttributeTableFrm();   
                Variable.pAttributeTableFeatureLayer = (IFeatureLayer)Layer;  
                Variable.pAttributeTableFrm.ShowAllAttribute();
                //显示 AttributeTableFrm
                Variable.pMainFrm = (MainFrm)System.Windows.Forms.Application.OpenForms["MainFrm"];
                if (Variable.pAttributeTableFrm.DockState == DockState.Hidden)
                {
                    Variable.pMainFrm.ShowDockContent(Variable.pAttributeTableFrm, DockState.DockBottomAutoHide);
                    Variable.pMainFrm.ShowDockContent(Variable.pAttributeTableFrm, DockState.DockBottom);
                }
                else
                {
                    Variable.pMainFrm.ShowDockContent(Variable.pAttributeTableFrm,DockState.Float);
                }
                Variable.pAttributeTableFrm.DockPanel.DockBottomPortion = 0.3;
                Variable.pAttributeTableFrm.AutoHidePortion = 0.3;
            }               
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");  
             }
        }

        //移除右击的图层数据
        private void TSMI_RemoveLayer_Click(object sender, EventArgs e)
        {
            try
            {
                if (Layer == null) return;
                DialogResult result = MessageBox.Show("是否移除[" + Layer.Name + "]图层", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    Variable.pMapFrm.mainMapControl.Map.DeleteLayer(Layer);
                }
                Variable.pMapFrm.mainMapControl.ActiveView.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }
        }

        //缩放至选定的图层
        private void ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (Layer == null) return;
                (Variable.pMapFrm.mainMapControl.Map as IActiveView).Extent = Layer.AreaOfInterest;
                (Variable.pMapFrm.mainMapControl.Map as IActiveView).PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "异常");
            }


        }
        
    }
}
