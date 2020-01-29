using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DijkstraClass
{
    //存储源点到各已处理节点的权重最小的路径
    public class PassedPath
    {
        private string curNodeID;   //记录是哪个节点
        private bool beProcessed;   //判断该节点是否被处理
        private double sumWeight;   //路径累积的权重值
        private ArrayList pathIDList; //存储路径经过的节点


        //初始化
        public PassedPath(string ID)
        {
            this.curNodeID = ID;
            this.sumWeight = double.MaxValue;
            this.pathIDList = new ArrayList();
            this.beProcessed = false;
        }
        public bool BeProcessed
        {
            get { return this.beProcessed; }
            set { this.beProcessed = value; }
        }

        public string CurNodeID
        {
            get { return this.curNodeID; }
        }

        public double SumWeight
        {
            get { return this.sumWeight; }
            set { this.sumWeight = value; }
        }

        public ArrayList PathIDList
        {
            get { return this.pathIDList; }
        }
    }
}
