using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmergencyResourceSchedule
{
    public class DestProperty
    {
        private int row, column;
        private double number;
        private string destname, resourcename;

        public int Row
        {
            get { return row; }
            set { row = value; }
        }

        public int Column
        {
            get { return column; }
            set { column = value; }
        }

        public double Number
        {
            get { return number; }
            set { number = value; }
        }

        public string Destname
        {
            get { return destname; }
            set { destname = value; }
        }

        public string Resourcename
        {
            get { return resourcename; }
            set { resourcename = value; }
        }
    }
}
