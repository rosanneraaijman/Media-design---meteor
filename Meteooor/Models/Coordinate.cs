using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meteooor.Models
{
    public class Coordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        const Double _earthRadiusInMiles = 3956.0;

        public double GetDistance(Coordinate relativeTo)
        {
            return _earthRadiusInMiles * 2 *
            (
                Math.Asin(
                    Math.Min(1,
                        Math.Sqrt(
                            (
                                Math.Pow(Math.Sin((DiffRadian(Latitude, relativeTo.Latitude)) / 2.0), 2.0) +
                                Math.Cos(ToRadian(Latitude)) * Math.Cos(ToRadian(relativeTo.Latitude)) *
                                Math.Pow(Math.Sin((DiffRadian(Longitude, relativeTo.Longitude)) / 2.0), 2.0)
                            )
                        )
                    )
                )
            ) * 1609.344; // Mile to kilometer to meter
        }

        static double ToRadian(double val)
        {
            return val * (Math.PI / 180);
        }

        static double DiffRadian(double val1, double val2)
        {
            return ToRadian(val2) - ToRadian(val1);
        }
    }
}