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
    [Route("api/Constellations")]
    public class ConstellationController : Controller
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ObjectPartialVm>), 200)]
        public IActionResult GetConstellations()
        {
            
            return Ok();
        }


        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        public IActionResult GetConstellation(string id)
        {
           


            return Ok();
        }
    }
}