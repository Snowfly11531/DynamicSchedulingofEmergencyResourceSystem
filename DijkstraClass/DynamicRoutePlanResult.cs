using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geometry;

namespace DijkstraClass
{
    public class DynamicRoutePlanResult
    {
        public IPoint Startpoint;
        public IPoint Endpoint;
        public IPolyline pLine;
    }
}
