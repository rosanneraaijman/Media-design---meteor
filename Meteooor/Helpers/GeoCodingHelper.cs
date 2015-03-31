using Meteooor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Meteooor.Helpers
{
    public static class GeoCodingHelper
    {
        public static Coordinate GetCoordinate(string coor1, string coor2)
        {
            return new Coordinate
            {
                Latitude = GeoCodingHelper.GetCoordinate(coor1),
                Longitude =  GeoCodingHelper.GetCoordinate(coor2)
            };
        }

        public static double GetCoordinate(string coor)
        {
            Regex dmsRe = new Regex(@"([NSEW]) (\d+)° (\d+)");

            var match = dmsRe.Match(coor);

            if (match.Success)
            {
                double degrees = double.Parse(match.Groups[2].Value);
                double minutes = !string.IsNullOrEmpty(match.Groups[3].Value) ? double.Parse(match.Groups[3].Value) / 60 : 0;

                string hemisphere = match.Groups[0].Value;

                if (hemisphere == "S" || hemisphere == "W")
                {
                    degrees = Math.Abs(degrees) * -1;
                }

                if (degrees < 0) {
                    return degrees - minutes;
                } else {
                    return degrees + minutes;
                }
            }

            return -1;
        }
    }
}