using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geometry;


namespace Agent
{
    public class EmergencyVehicle
    {
        public int ID;
        public int Origination;
        public int Destination;
        public int CurrentOriginationID;
        public int CurrentDestinationID;
        public double Distance;
        public double Velocity;
        public IPoint OriginationPoint;//起始点，即应急资源仓库
        public IPoint DestinationPoint;//目的地，即应急处置空间位置
        public IPoint CurrentPoint;//智能体车辆当前所在的点
        public IPoint CurrentOrigination;//
        public IPoint CurrentDestination;
        public IPolyline CurrentLine;
        public IPolyline RunVehicleLine;
        public IPolyline RetureLine;
        public IList<VehicleRouteResult> PlanRouteResult;
        public IList<IPolyline> PlanRoute;
        public double Congestion;
    }
}
