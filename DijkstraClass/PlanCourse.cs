using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using System.Linq;
using System.Text;

namespace DijkstraClass
{
    //PlanCourse缓存从源点到其它任一节点的最小权重值路径，记录规划的中间结果，管理了每一个节点的PassedPath
    //用一张表（PlanCourse）记录源点到任何其它一节点的最小权重值
    //初始化这张表时，如果源点能直通某节点，则权值设为对应的边的权，否则设为double.MaxValue
    //对于非源点，将源点加入路径
    public class PlanCourse
    {
        public Hashtable htPassedPath;//定义一个哈希表存储各节点的路径表

        public PlanCourse(List<Node> NodeList, string OriginID)
        {
            htPassedPath = new Hashtable();
            Node OriginNode = null;
            foreach (Node node in NodeList)
            {
                if (node.ID == OriginID)
                {
                    OriginNode = node;
                }
                else
                {
                    PassedPath pPath = new PassedPath(node.ID);
                    htPassedPath.Add(node.ID, pPath);
                }

                if (OriginNode == null)
                {
                    continue;
                }
            }
            InitializeWeight(OriginNode);
        }

        private void InitializeWeight(Node OriginNode)
        {
            if ((OriginNode.EdgeList == null) || (OriginNode.EdgeList.Count == 0))
            {
                return;
            }
            foreach (Edge edge in OriginNode.EdgeList)
            {
                PassedPath pPath = (PassedPath)htPassedPath[edge.EndNodeID];
                if (pPath == null)
                {
                    continue;
                }
                pPath.PathIDList.Add(OriginNode.ID);
                pPath.SumWeight = edge.Weight;
            }
        }

        public PassedPath this[string nodeID]
        {
            get
            {
                return (PassedPath)htPassedPath[nodeID];
            }
        }
    }
}
