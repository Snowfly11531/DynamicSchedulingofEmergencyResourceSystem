using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;

namespace DynamicSchedulingofEmergencyResourceSystem
{
    public partial class EagleEyeMapFrm : DockContent
    {
        public EagleEyeMapFrm()
        {

            InitializeComponent();
            
        }
      
        // MapControl 控件
        public AxMapControl EagelEyeMapControl
        {
            get { return axMapControl2; }
        }

        private void axMapControl2_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (EagelEyeMapControl.Map.LayerCount > 0)
            {
                //按下鼠标左键移动矩形框
                if (e.button == 1)
                {
                    //如果指针落在鹰眼的矩形框中，标记可移动
                    if (e.mapX > Variable.pEnvelop.XMin && e.mapY > Variable.pEnvelop.YMin && e.mapX < Variable.pEnvelop.XMax && e.mapY < Variable.pEnvelop.YMax)
                    {
                        Variable.bCanDrag = true;
                    }
                    Variable.pMoveRectPoint = new PointClass();
                    Variable.pMoveRectPoint.PutCoords(e.mapX, e.mapY);  //记录点击的第一个点的坐标
                }
                //按下鼠标右键绘制矩形框
                else if (e.button == 2)
                {
                    IEnvelope pEnvelope = EagelEyeMapControl.TrackRectangle();
                    
                    
                    IPoint pTempPoint = new PointClass();
                    if (pEnvelope==null||pEnvelope.IsEmpty||pEnvelope.Height==0||pEnvelope.Width==0)
                    {
                        return;
                    }
                    else
                    {
                        pTempPoint.PutCoords(pEnvelope.XMin + pEnvelope.Width / 2, pEnvelope.YMin + pEnvelope.Height / 2);
                    }
                    Variable.pMapFrm.mainMapControl.Extent = pEnvelope;
                    //矩形框的高宽和数据试图的高宽不一定成正比，这里做一个中心调整
                    Variable.pMapFrm.mainMapControl.CenterAt(pTempPoint);
                }
            }
        }

        private void axMapControl2_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            if (e.mapX > Variable.pEnvelop.XMin && e.mapY > Variable.pEnvelop.YMin && e.mapX < Variable.pEnvelop.XMax && e.mapY < Variable.pEnvelop.YMax)
            {
                //如果鼠标移动到矩形框中，鼠标换成小手，表示可以拖动
                EagelEyeMapControl.MousePointer = esriControlsMousePointer.esriPointerHand;
                if (e.button == 2)//如果在内部按下鼠标右键，将鼠标演示设置为默认样式
                {
                    EagelEyeMapControl.MousePointer = esriControlsMousePointer.esriPointerDefault;
                }
            }
            else
            {
                //在其他位置将鼠标设为默认的样式
                EagelEyeMapControl.MousePointer = esriControlsMousePointer.esriPointerDefault;
            }
            if (Variable.bCanDrag)
            {
                double Dx, Dy;  //记录鼠标移动的距离
                Dx = e.mapX - Variable.pMoveRectPoint.X;
                Dy = e.mapY - Variable.pMoveRectPoint.Y;
                Variable.pEnvelop.Offset(Dx,Dy);//根据偏移量更改pEnvelop位置
                Variable.pMoveRectPoint.PutCoords(e.mapX,e.mapY);
                EagleEyeClass.DrawRectangle(Variable.pEnvelop);
                Variable.pMapFrm.mainMapControl.Extent = Variable.pEnvelop;
            }
        }

        private void axMapControl2_OnMouseUp(object sender, IMapControlEvents2_OnMouseUpEvent e)
        {
            if (e.button == 1 && Variable.pMoveRectPoint != null)
            {
                if (e.mapX == Variable.pMoveRectPoint.X && e.mapY == Variable.pMoveRectPoint.Y)
                {
                    Variable.pMapFrm.mainMapControl.CenterAt(Variable.pMoveRectPoint);
                }
                Variable.bCanDrag = false;
            }
        }
    }
}
