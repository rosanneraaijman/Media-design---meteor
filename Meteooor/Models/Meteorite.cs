using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meteooor.Models
{
    public class Meteorite
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public List<PriceModel> Prices { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1})", Name, Url);
        }
    }
}