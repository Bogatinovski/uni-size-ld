using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using usld_web.ViewModels;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;

namespace usld_web.Controllers
{
    [Route("api/Asteroids")]
    public class AsteroidController : Controller
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ObjectPartialVm>), 200)]
        public IActionResult GetAsteroids()
        {
            
            return Ok();
        }


        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        public IActionResult GetAsteroid(string id)
        {
           


            return Ok();
        }
    }
}