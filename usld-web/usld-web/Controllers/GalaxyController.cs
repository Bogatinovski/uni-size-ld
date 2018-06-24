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
    [Route("api/Galaxies")]
    public class GalaxyController : Controller
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ObjectPartialVm>), 200)]
        public IActionResult GetGalaxies()
        {
            SparqlParameterizedString queryString = new SparqlParameterizedString();
            queryString.Namespaces.AddNamespace("dbo", new Uri("http://dbpedia.org/ontology/"));
            queryString.Namespaces.AddNamespace("dbp", new Uri("http://dbpedia.org/property/"));
            queryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            queryString.Namespaces.AddNamespace("rdf", new Uri("https://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            queryString.CommandText = SparqlCommands.GetListOfGalaxies();

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
        [ProducesResponseType(typeof(GalaxyVm), 200)]
        public IActionResult GetGalaxy(string id)
        {
            Uri uri = new Uri(WebUtility.UrlDecode(id));

            SparqlParameterizedString queryString = new SparqlParameterizedString();
            queryString.Namespaces.AddNamespace("dbo", new Uri("http://dbpedia.org/ontology/"));
            queryString.Namespaces.AddNamespace("dbp", new Uri("http://dbpedia.org/property/"));
            queryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));

            queryString.CommandText = SparqlCommands.GetSingleGalaxy();
            queryString.SetUri("subjectUri", uri);

            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlQuery query = parser.ParseFromString(queryString);
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"), "http://dbpedia.org");

            SparqlResultSet results = endpoint.QueryWithResultSet(query.ToString());
            SparqlResult resultNode = results.FirstOrDefault();

            string subject = ((UriNode)resultNode["subject"])?.Uri.ToSafeString();
            string label = ((LiteralNode)resultNode["label"])?.Value.ToSafeString();
            string abstractValue = ((LiteralNode)resultNode["abstract"])?.Value.ToSafeString();
            string thumbnail = ((UriNode)resultNode["thumbnail"])?.Uri.ToSafeString();
            string size = ((LiteralNode)resultNode["size"])?.Value.ToSafeString();

            GalaxyVm galaxy = new GalaxyVm
            {
                Subject = subject,
                Label = label,
                Abstract = abstractValue,
                Thumbnail = thumbnail,
                Size = size
            };

            return Ok(galaxy);
        }
    }
}