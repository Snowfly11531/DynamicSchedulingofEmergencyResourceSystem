using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmergencyResourceSchedule
{
    public class DepotProperty
    {
        private int row, column;
        private string depotname, resourcename;
        private double number;

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

        public string Depotname
        {
            get { return depotname; }
            set { depotname = value; }
        }

        public string Resourcename
        {
            get { return resourcename; }
            set { resourcename = value; }
        }

        public double Number
        {
            get { return number; }
            set { number = value; }
        }
    }
}
