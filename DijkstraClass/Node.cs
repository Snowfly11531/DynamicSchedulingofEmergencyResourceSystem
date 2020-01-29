using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DijkstraClass
{
    //路网的节点类
    public class Node
    {
        public string ID;  //节点的ID
        public List<Edge> EdgeList;   //某个节点为起点的边

        public Node(string id)
        {
            this.ID = id;
            this.EdgeList = new List<Edge>();
        }
    }
}
