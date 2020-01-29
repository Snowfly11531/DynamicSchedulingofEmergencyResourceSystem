using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geometry;

namespace RandomRoute
{
    public class Edge
    {
        public string StartNodeID;  //边的起始点节点的ID
        public string EndNodeID;    //边的终点节点的ID
        public double Weight;       //边的权重值
        public IPolyline line;      //边的线
        public string pLineID;      //边的线的ID值
        public string name;         //边的名称
    }
}
