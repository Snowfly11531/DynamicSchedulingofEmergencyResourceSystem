using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Geometry;

namespace RiverClass
{
    //河流网络边的描述
    public class RiverEdge
    {
        public string StartNodeID;  //边的起始点节点的ID
        public string EndNodeID;    //边的终点节点的ID
        public IPolyline line;      //边的线
        public string name;         //边的名称
    }
}
