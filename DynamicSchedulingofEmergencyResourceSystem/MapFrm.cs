using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Tool;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;

namespace DynamicSchedulingofEmergencyResourceSystem
{
    public partial class MapFrm : DockContent
    {
        public MapFrm()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref   Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_CLOSE = 0xF060;
            if (m.Msg == WM_SYSCOMMAND && (int)m.WParam == SC_CLOSE)
            {
                return;
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// MapControl 控件
        /// </summary>
        public AxMapControl mainMapControl
        {
            get { return axMapControl1; }
        }
        MapTools currentTool = MapTools.None;

        /// <summary>
        /// MapControl 的当前地图工具
        /// </summary>
        public MapTools CurrentTool
        {
            get { return currentTool; }
            set
            {
                if (currentTool == MapTools.MeasureLength && value != MapTools.MeasureLength)  // 由测距离工具切换到其他地图工具时，需要做的一些处理工作
                {
                    // 删除所有元素
                    axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, axMapControl1.Map, axMapControl1.Extent);
                    IGraphicsContainer graphicsContainer = axMapControl1.Map as IGraphicsContainer;
                    graphicsContainer.DeleteAllElements();
                }
                currentTool = value;
            }
        }

        private void axMapControl1_OnMapReplaced(object sender, IMapControlEvents2_OnMapReplacedEvent e)
        {
            EagleEyeClass.SynchronizeEagleEye();
        }

        private void axMapControl1_OnKeyDown(object sender, IMapControlEvents2_OnKeyDownEvent e)
        {
            EagleEyeClass.SynchronizeEagleEye();
        }

        private void axMapControl1_OnExtentUpdated(object sender, IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            if (Variable.pEagleEyeMapFrm == null || Variable.pEagleEyeMapFrm.IsDisposed)
            {
                Variable.pEnvelop = e.newEnvelope as IEnvelope;
            }
            else
            {
                Variable.pEnvelop = (IEnvelope)e.newEnvelope;
                EagleEyeClass.DrawRectangle(Variable.pEnvelop);
            }
            
        }

        
    }
}
