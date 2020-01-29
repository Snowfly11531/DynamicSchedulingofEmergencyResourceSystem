using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geometry;

namespace Agent
{
    public class Vehicle
    {
        public double OriginPosition;
        public double TargetPosition;
        public double CurrentPosition;
        public double Velocity;
        List<IPolyline> PlanRoad;
    }
}
