using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DijkstraClass
{
    public class RoutePlanner
    {
        //获取从起点到终点的权值最小的路径，DIJSTRA算法
        public PlanCourse Plan(List<Node> NodeList, string OriginID)
        {
            //获取源点到与其相邻点的路径和权重值
            PlanCourse planCourse = new PlanCourse(NodeList, OriginID);
            //获取当前累积权重值最小，并且没有被处理过的节点
            Node curNode = GetMinWeightRouteNode(planCourse, NodeList, OriginID);
            while (curNode != null)
            {
                PassedPath curPath = planCourse[curNode.ID];
                foreach (Edge edge in curNode.EdgeList)
                {
                    if (edge.EndNodeID != OriginID)
                    {
                        PassedPath targetPath = planCourse[edge.EndNodeID];
                        double tempWeight = curPath.SumWeight + edge.Weight;
                        if (tempWeight < targetPath.SumWeight)
                        {
                            targetPath.SumWeight = tempWeight;
                            targetPath.PathIDList.Clear();
                            for (int i = 0; i < curPath.PathIDList.Count; i++)
                            {
                                targetPath.PathIDList.Add(curPath.PathIDList[i].ToString());
                            }
                            targetPath.PathIDList.Add(curNode.ID);
                        }
                    }
                }
                //标记该节点已经处理了
                planCourse[curNode.ID].BeProcessed = true;
                ///获取下一个累积权重值最小，并且没有被处理过的节点
                curNode = GetMinWeightRouteNode(planCourse, NodeList, OriginID);
            }

            PlanCourse resultplancourse = planCourse;
            return resultplancourse;
            //RoutePlanResult[] result = new RoutePlanResult[DestID.Length];
            //for (int i = 0; i < DestID.Length; i++)
            //{
            //    if (!DestID[i].Equals(OriginID))
            //    {
            //        result[i] = GetResult(planCourse, DestID[i]);
            //    }
            //    else
            //    {
            //        string[] passedNodeID = new string[1];
            //        passedNodeID[0] = DestID[i];
            //        result[i] = new RoutePlanResult(passedNodeID, 0);
            //    }

            //}

            //return result;

            //return GetResult(planCourse,DestID);

        }


        //获取权值最小的路径，DIJISTRA算法
        public RoutePlanResult Plan(List<Node> nodeList, string originID, string destID)
        {
            PlanCourse planCourse = new PlanCourse(nodeList, originID);//获得源点到与其相邻点的路径和权值

            Node curNode = GetMinWeightRouteNode(planCourse, nodeList, originID);//当前累积权值最小，并且没有被处理过的节点


            while (curNode != null)
            {
                PassedPath curPath = planCourse[curNode.ID];
                foreach (Edge edge in curNode.EdgeList)
                {
                    if (edge.EndNodeID != originID)
                    {
                        PassedPath targetPath = planCourse[edge.EndNodeID];//选取没有被处理并且当前累积权值最小的节点TargetNode
                        double tempWeight = curPath.SumWeight + edge.Weight;
                        if (tempWeight < targetPath.SumWeight)
                        {
                            targetPath.SumWeight = tempWeight;//以此点为终点的路径长度
                            targetPath.PathIDList.Clear();//将之前的路径清空

                            for (int i = 0; i < curPath.PathIDList.Count; i++)
                            {
                                targetPath.PathIDList.Add(curPath.PathIDList[i].ToString());
                            }

                            targetPath.PathIDList.Add(curNode.ID);
                        }
                    }

                }

                //标志为已处理
                planCourse[curNode.ID].BeProcessed = true;
                //获取下一个未处理节点
                curNode = GetMinWeightRouteNode(planCourse, nodeList, originID);
            }


            //表示规划结束
            return GetResult(planCourse, destID);
        }


        //获取从起点到终点的权值最小的路径，DIJSTRA算法
        public RoutePlanResult[] Plan(List<Node> NodeList, string OriginID, string[] DestID)
        {
            //获取源点到与其相邻点的路径和权重值
            PlanCourse planCourse = new PlanCourse(NodeList, OriginID);
            //获取当前累积权重值最小，并且没有被处理过的节点
            Node curNode = GetMinWeightRouteNode(planCourse, NodeList, OriginID);
            while (curNode != null)
            {
                PassedPath curPath = planCourse[curNode.ID];
                foreach (Edge edge in curNode.EdgeList)
                {
                    if (edge.EndNodeID != OriginID)
                    {
                        PassedPath targetPath = planCourse[edge.EndNodeID];
                        double tempWeight = curPath.SumWeight + edge.Weight;
                        if (tempWeight < targetPath.SumWeight)
                        {
                            targetPath.SumWeight = tempWeight;
                            targetPath.PathIDList.Clear();
                            for (int i = 0; i < curPath.PathIDList.Count; i++)
                            {
                                targetPath.PathIDList.Add(curPath.PathIDList[i].ToString());
                            }
                            targetPath.PathIDList.Add(curNode.ID);
                        }
                    }
                }
                //标记该节点已经处理了
                planCourse[curNode.ID].BeProcessed = true;
                ///获取下一个累积权重值最小，并且没有被处理过的节点
                curNode = GetMinWeightRouteNode(planCourse, NodeList, OriginID);
            }

            RoutePlanResult[] result = new RoutePlanResult[DestID.Length];
            for (int i = 0; i < DestID.Length; i++)
            {
                if (!DestID[i].Equals(OriginID))
                {
                    result[i] = GetResult(planCourse, DestID[i]);
                }
                else
                {
                    string[] passedNodeID = new string[1];
                    passedNodeID[0] = DestID[i];
                    result[i] = new RoutePlanResult(passedNodeID, 0);
                }

            }

            return result;

            //return GetResult(planCourse,DestID);

        }

      


        public static RoutePlanResult GetResult(PlanCourse planCourse, string destID)
        {
            PassedPath pPath = planCourse[destID];
            if (pPath.SumWeight == double.MaxValue)
            {
                RoutePlanResult routePlanResult = new RoutePlanResult(null, double.MaxValue);
                return routePlanResult;
            }

            string[] passedNodeIDs = new string[pPath.PathIDList.Count];
            for (int i = 0; i < passedNodeIDs.Length; i++)
            {
                passedNodeIDs[i] = pPath.PathIDList[i].ToString();
            }
            RoutePlanResult result = new RoutePlanResult(passedNodeIDs, pPath.SumWeight);
            return result;
        }

        //从PlanCourse取出一个当前累积权值最小并没有被处理过的节点
        public static Node GetMinWeightRouteNode(PlanCourse planCourse, List<Node> NodeList, string OriginID)
        {
            double minWeight = double.MaxValue;
            Node destNode = null;


            foreach (Node node in NodeList)
            {
                if (node.ID == OriginID)
                {
                    continue;
                }
                PassedPath pPath = planCourse[node.ID]; //寻找出节点的最短路径
                if (pPath.BeProcessed)
                {
                    continue;
                }
                if (pPath.SumWeight < minWeight)
                {
                    minWeight = pPath.SumWeight;
                    destNode = node;
                }
            }
            return destNode;
        }
    }
}
