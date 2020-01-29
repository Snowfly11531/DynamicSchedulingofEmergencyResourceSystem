using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geometry;

namespace Agent
{
    public class OrdinaryVehicle
    {
        public int ID;
        public int Origination;
        public int Destination;
        public double Velocity;
        public IPoint OriginationPoint;
        public IPoint DestinationPoint;
        public IPoint CurrentPoint;
        public IPolyline CurrentLine;
        public List<IPolyline> PlanRoute;
        public IList<VehicleRouteResult> PlanRouteResult;
        public double Congestion;
    }
}
