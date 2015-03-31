using Meteooor.Models;
using Meteooor.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Meteooor.Controllers
{
    public class MeteoriteMarketController : ApiController
    {
        public HttpResponseMessage GetMeteorites()
        {
            List<Meteorite> result = MeteoriteRepository.GetMeteorites("http://www.meteoritemarket.com/");

            return Request.CreateResponse<List<Meteorite>>(result);
        }
    }
}