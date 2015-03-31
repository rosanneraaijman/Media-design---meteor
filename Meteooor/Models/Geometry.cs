using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meteooor.Models
{
    public class Geometry
    {
        public string Type
        {
            get
            {
                return "Point";
            }
        }

        public double[] Coordinates
        {
            get
            {
                return new double[2] { 125.6, 10.1 };
            }
        }
    }
}
