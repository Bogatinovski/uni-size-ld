﻿using System;
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
    [Route("api/Stars")]
    public class StarController : Controller
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ObjectPartialVm>), 200)]
        public IActionResult GetStars()
        {
            SparqlParameterizedString queryString = new SparqlParameterizedString();
            queryString.Namespaces.AddNamespace("dbo", new Uri("http://dbpedia.org/ontology/"));
            queryString.Namespaces.AddNamespace("dbp", new Uri("http://dbpedia.org/property/"));
            queryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            queryString.Namespaces.AddNamespace("rdf", new Uri("https://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            queryString.CommandText = SparqlCommands.GetListOfStars();

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
        [ProducesResponseType(typeof(StarVm), 200)]
        public IActionResult GetStar(string id)
        {
            Uri uri = new Uri(WebUtility.UrlDecode(id));

            SparqlParameterizedString queryString = new SparqlParameterizedString();
            queryString.Namespaces.AddNamespace("dbo", new Uri("http://dbpedia.org/ontology/"));
            queryString.Namespaces.AddNamespace("dbp", new Uri("http://dbpedia.org/property/"));
            queryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));

            queryString.CommandText = SparqlCommands.GetSingleStar();
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
            string luminosity = ((LiteralNode)resultNode["luminosityAggr"])?.Value.ToSafeString();
            string radius = ((LiteralNode)resultNode["radiusAggr"])?.Value.ToSafeString();
            string temperature = ((LiteralNode)resultNode["temperatureAggr"])?.Value.ToSafeString();
            string mass = ((LiteralNode)resultNode["massAggr"])?.Value.ToSafeString();
            string gravity = ((LiteralNode)resultNode["gravityAggr"])?.Value.ToSafeString();
            string epoch = resultNode["epochAggr"] is UriNode ? "" : ((LiteralNode)resultNode["epochAggr"])?.Value.ToSafeString();
            string constel = ((LiteralNode)resultNode["constelLabel"])?.Value.ToSafeString();
            string constelUri = ((UriNode)resultNode["constel"])?.Uri.ToSafeString();

            StarVm star = new StarVm
            {
                Subject = subject,
                Label = label,
                Abstract = abstractValue,
                Thumbnail = thumbnail,
                ConstellationName = constel,
                ConstellationUri = constelUri,
                Epoch = epoch,
                Gravity = gravity,
                Luminosity = luminosity,
                Mass = mass,
                Radius = radius,
                Temperature = temperature
            };

            return Ok(star);
        }
    }
}