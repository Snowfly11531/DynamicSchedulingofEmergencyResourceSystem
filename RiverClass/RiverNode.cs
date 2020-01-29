using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiverClass
{
    //河流网络的连接点
    public class RiverNode
    {
        public string ID;  //节点的ID
        public List<RiverEdge> EdgeList;   //连接点为起点的边

        public RiverNode(string id)
        {
            this.ID = id;
            this.EdgeList = new List<RiverEdge>();
        }
    }
}
