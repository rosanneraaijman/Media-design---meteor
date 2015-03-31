using Meteooor.Models;
using Meteooor.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Meteooor.Controllers
{
    public class EarthImpactController : ApiController
    {
        public HttpResponseMessage GetImpactCraters()
        {
            List<Crater> result = CraterRepository.GetAll();

            return Request.CreateResponse<List<Crater>>(result);
        }
    }
}
