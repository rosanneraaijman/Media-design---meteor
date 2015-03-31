using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meteooor.Models
{
    public class Crater
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public double Diameter { get; set; }
        public string Url { get; set; }
    }
}