using Meteooor.Models;
using Meteooor.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Meteooor.Controllers
{
    public class MarkerController : ApiController
    {
        public HttpResponseMessage GetMarkers()
        {
            List<Marker> markers = Markers.GetMarkers().ToList();

            string header = "Title,Latitude,Longitude,Diameter,Price,Source,EventID,Version\r\n";
            StringBuilder sb = new StringBuilder();

            foreach(var marker in markers)
            {
                sb.AppendFormat("{0},{1},{2},{3},{4},{5},{6},{7}\r\n",
                                "Marker", marker.Latitude.ToString().Replace(",", "."),
                                marker.Longitude.ToString().Replace(",", "."), marker.Diameter.Replace(",", "."),
                                marker.Price
                                ,"1234", "1235", "123456");
            }

            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Content = new StringContent(header + sb.ToString(), Encoding.UTF8, "text/plain");
            return resp;
        }
    }
}
