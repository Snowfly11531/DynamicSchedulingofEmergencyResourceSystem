using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DijkstraClass
{
    //路径规划的结果类
    public class RoutePlanResult
    {
        private string[] ResultNode;


        private double WeightValue;

        public RoutePlanResult(string[] passedNodes, double value)
        {
            ResultNode = passedNodes;
            WeightValue = value;
        }

        public string[] ResultNodes
        {
            get { return ResultNode; }
            set { ResultNode = value; }
        }

        public double WeightValues
        {
            get { return WeightValue; }
            set { WeightValues = value; }
        }
    }
}
