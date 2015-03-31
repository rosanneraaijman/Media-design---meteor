using Meteooor.Helpers;
using Meteooor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Meteooor.Repositories
{
    public static class Markers
    {
        public static IEnumerable<Marker> GetMarkers(int radius = 100)
        {
            var craters = CraterRepository.GetAll();
            var meteorites = MeteoriteRepository.GetMeteorites("http://www.meteoritemarket.com/");

            foreach(Crater crater in craters)
            {
                Coordinate craterCoord = GeoCodingHelper.GetCoordinate(crater.Latitude, crater.Longitude);

                foreach(var meteor in meteorites)
                {
                    Coordinate mCoord = GeoCodingHelper.GetCoordinate(meteor.Latitude, meteor.Longitude);

                    if(craterCoord.GetDistance(mCoord) <= radius * 1000)
                    {
                        var price = meteor.Prices.FirstOrDefault(x => x.PricePerGram > -1);

                        yield return new Marker
                        {
                            Latitude = craterCoord.Latitude,
                            Longitude = craterCoord.Longitude,
                            Diameter = crater.Diameter.ToString()
                        };
                    }
                }
            }
        }
    }
}