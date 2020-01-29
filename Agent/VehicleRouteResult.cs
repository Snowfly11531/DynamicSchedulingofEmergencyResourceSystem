using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geometry;

namespace Agent
{
    public class VehicleRouteResult
    {
        public IPoint Startpoint;
        public IPoint Endpoint;
        public IPolyline pLine;
        public string pLineID;
        public string StartpointID;
        public string EndpointID;
    }
}
