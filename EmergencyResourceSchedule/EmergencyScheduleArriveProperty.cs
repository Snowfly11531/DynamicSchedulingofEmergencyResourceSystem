using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmergencyResourceSchedule
{
     public class EmergencyScheduleArriveProperty
    {
        private string startDepotName, endDestName;
        private int sequence;
        private double totalTime;


        public string StartDepotName
        {
            get { return startDepotName; }
            set { startDepotName = value; }
        }

        public string EndDestName
        {
            get { return endDestName; }
            set { endDestName = value; }
        }

        public int Sequence
        {
            get { return sequence; }
            set { sequence = value; }
        }

        public double TotalTime
        {
            get { return totalTime; }
            set { totalTime = value; }
        }
    }
}
