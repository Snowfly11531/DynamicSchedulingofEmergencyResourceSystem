using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using DataManagement;
using EmergencyResourceSchedule;
using DijkstraClass;
namespace ERSModel
{
    public class StaticERSModel
    {
        public static List<Result> GetAgentRoad(IFeatureLayer pFeatureLayerDepot, IFeatureLayer pFeatureLayerDest, IRasterLayer pRasterLayer)
        {
            return null;
        }


        //污染物达到应急处置空间位置的时间进行冒泡法排序
        public void BubbleSort(List<PollutionArriveDestProperty> parrivetime)
        {
            for (int m = parrivetime.Count - 1; m > 0; m--)
            {
                for (int n = 0; n < m; n++)
                {
                    if (parrivetime[n].Arrivetime > parrivetime[n + 1].Arrivetime)
                    {
                        PollutionArriveDestProperty temp = parrivetime[n];
                        parrivetime[n] = parrivetime[n + 1];
                        parrivetime[n + 1] = temp;
                    }
                }
            }
        }

        //应急资源到达各个应急处置空间位置的时间进行冒泡法排序
        public void BubbleSort(List<EmergencyScheduleArriveProperty> parrivetime)
        {
            for (int m = parrivetime.Count - 1; m > 0; m--)
            {
                for (int n = 0; n < m; n++)
                {
                    if (parrivetime[n].TotalTime > parrivetime[n + 1].TotalTime)
                    {
                        EmergencyScheduleArriveProperty temp = parrivetime[n];
                        parrivetime[n] = parrivetime[n + 1];
                        parrivetime[n + 1] = temp;
                    }
                }
            }
        }
    }
}
