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
    [Route("api/Swarms")]
    public class SwarmController : Controller
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ObjectPartialVm>), 200)]
        public IActionResult GetSwarms()
        {
            SparqlParameterizedString queryString = new SparqlParameterizedString();
            queryString.Namespaces.AddNamespace("dbo", new Uri("http://dbpedia.org/ontology/"));
            queryString.Namespaces.AddNamespace("dbp", new Uri("http://dbpedia.org/property/"));
            queryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            queryString.Namespaces.AddNamespace("rdf", new Uri("https://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            queryString.CommandText = SparqlCommands.GetListOfSwarms();

            Console.WriteLine(queryString.ToString());

            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlQuery query = parser.ParseFromString(queryString);
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"), "http://dbpedia.org");

            SparqlResultSet results = endpoint.QueryWithResultSet(query.ToString());
            SparqlResult resultNode = results.FirstOrDefault();

            ICollection<ObjectPartialVm> model = new List<ObjectPartialVm>();

            foreach (SparqlResult result in results)
            {
                string subject = ((UriNode)result["subject"])?.Uri.ToSafeString();
                string label = ((LiteralNode)result["label"])?.Value.ToSafeString();
                string thumbnail = ((UriNode)result["thumbnail"])?.Uri.ToSafeString();
                string comment = ((LiteralNode)result["comment"])?.Value.ToSafeString();

                ObjectPartialVm objectPartialVm = new ObjectPartialVm
                {
                    Comment = comment,
                    Label = label,
                    Subject = subject,
                    Thumbnail = thumbnail
                };

                model.Add(objectPartialVm);
            }

            return Ok(model);
        }


        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(void), 200)]
        public IActionResult GetSwarm(string id)
        {
           


            return Ok();
        }
    }
}