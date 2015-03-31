using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meteooor.Models
{
    public class Marker
    {
        public string Title { get; set; }
        public string Diameter { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public decimal Price { get; set; }
    }
}