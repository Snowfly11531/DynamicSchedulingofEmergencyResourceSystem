using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DijkstraClass
{
    //经过路网的节点
    public class RouteNode
    {
        private string nodeID;

        public RouteNode(string nodeId)
        {
            this.nodeID = nodeId;
        }

        public string NodeID
        {
            get { return nodeID; }
            set { nodeID = value; }
        }
    }
}
