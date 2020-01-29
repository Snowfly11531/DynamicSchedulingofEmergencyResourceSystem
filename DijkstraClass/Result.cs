using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DijkstraClass
{
    //应急资源仓库到达应急处置空间位置的信息
    public class Result
    {
        public string DepotName;
        public string DestName;
        public string EndNodeID;
        public double WeightTime;
        public bool dynamicschedule;
        public string[] StrResultNode;
    }
}
