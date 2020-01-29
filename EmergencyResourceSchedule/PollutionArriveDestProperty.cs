using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmergencyResourceSchedule
{
    public class PollutionArriveDestProperty
    {
        private string destname;
        private double arrivetime;
        private int sequence;

        public string Destname
        {
            get { return destname; }
            set { destname = value; }
        }

        public double Arrivetime
        {
            get { return arrivetime; }
            set { arrivetime = value; }
        }

        public int Sequence
        {
            get { return sequence; }
            set { sequence = value; }
        }
    }
}
