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
    [Route("api/Planets")]
    public class PlanetController : Controller
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ObjectPartialVm>), 200)]
        public IActionResult GetPlanets()
        {
            SparqlParameterizedString queryString = new SparqlParameterizedString();
            queryString.Namespaces.AddNamespace("dbo", new Uri("http://dbpedia.org/ontology/"));
            queryString.Namespaces.AddNamespace("dbr", new Uri("http://dbpedia.org/resource/"));
            queryString.Namespaces.AddNamespace("dbp", new Uri("http://dbpedia.org/property/"));
            queryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            queryString.Namespaces.AddNamespace("rdf", new Uri("https://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            queryString.Namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/0.1/"));
            queryString.CommandText = SparqlCommands.GetListOfPlanets();

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

        #region Depricated Action
        /*[HttpGet]
        [Route("Body/{bodyTypeUri}")]
        [ProducesResponseType(typeof(IEnumerable<ObjectPartialVm>), 200)]
        public IActionResult GetBody([FromRoute]string bodyTypeUri)
        {
            Uri uri = new Uri(WebUtility.UrlDecode(bodyTypeUri));

            SparqlParameterizedString queryString = new SparqlParameterizedString();
            queryString.Namespaces.AddNamespace("dbo", new Uri("http://dbpedia.org/ontology/"));
            queryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            queryString.Namespaces.AddNamespace("rdf", new Uri("https://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            queryString.Namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/0.1/"));
            queryString.CommandText = SparqlCommands.GetListOfObjects();
            queryString.SetUri("bodyType", uri);

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
        }*/
        #endregion

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(PlanetVm), 200)]
        public IActionResult GetPlanet(string id)
        {
            #region Depricated Code
            /*planetUri = WebUtility.UrlDecode(planetUri);
            planetUri = planetUri.Replace("/resource/", "/page/");
            Uri uri = new Uri(planetUri);

            IGraph g = new Graph();
            UriLoader.Load(g, uri);

            g.NamespaceMap.AddNamespace("dbo", new Uri("http://dbpedia.org/ontology/"));
            g.NamespaceMap.AddNamespace("dbp", new Uri("http://dbpedia.org/property/"));
            g.NamespaceMap.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            g.NamespaceMap.AddNamespace("rdf", new Uri("https://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            g.NamespaceMap.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/0.1/"));
            g.NamespaceMap.AddNamespace("owl", new Uri("http://www.w3.org/2002/07/owl#"));

            IEnumerable<Triple> predicates = null;
            IUriNode labelNode = g.CreateUriNode("rdfs:label");
            predicates = g.GetTriplesWithPredicate(labelNode);
            string label = predicates.Select(p => p.Object as ILiteralNode).Where(n => n.Language.ToLower() == "en").FirstOrDefault().ToSafeString();
            string subject = predicates.FirstOrDefault()?.Subject.ToSafeString();

            IUriNode nameNode = g.CreateUriNode("foaf:name");
            predicates = g.GetTriplesWithPredicate(nameNode);
            string name = predicates.FirstOrDefault()?.Object.ToSafeString();

            IUriNode abstractNode = g.CreateUriNode("dbo:abstract");
            predicates = g.GetTriplesWithPredicate(abstractNode);
            string abstractValue = predicates.Select(p => p.Object as ILiteralNode).Where(n => n.Language.ToLower() == "en").FirstOrDefault().ToSafeString();

            IUriNode averageSpeedNode = g.CreateUriNode("dbo:averageSpeed");
            predicates = g.GetTriplesWithPredicate(averageSpeedNode);
            string averageSpeed = predicates.FirstOrDefault()?.Object.ToSafeString();

            IUriNode escapeVelocityNode = g.CreateUriNode("dbo:escapeVelocity");
            predicates = g.GetTriplesWithPredicate(escapeVelocityNode);
            string escapeVelocity = predicates.FirstOrDefault()?.Object.ToSafeString();

            IUriNode meanTemperatureNode = g.CreateUriNode("dbo:meanTemperature");
            predicates = g.GetTriplesWithPredicate(meanTemperatureNode);
            string meanTemperature = predicates.FirstOrDefault()?.Object.ToSafeString();

            IUriNode thumbnailNode = g.CreateUriNode("dbo:thumbnail");
            predicates = g.GetTriplesWithPredicate(thumbnailNode);
            string thumbnail = predicates.FirstOrDefault()?.Object.ToSafeString();
            
            IUriNode captionNode = g.CreateUriNode("dbp:caption");
            predicates = g.GetTriplesWithPredicate(captionNode);
            string caption = predicates?.Select(p => p.Object as ILiteralNode).Where(n => n.Language.ToLower() == "en").FirstOrDefault().ToSafeString();

            IUriNode angularSizeNode = g.CreateUriNode("dbp:angularSize");
            predicates = g.GetTriplesWithPredicate(angularSizeNode);
            string angularSize = predicates?.FirstOrDefault()?.Object.ToSafeString();

            IUriNode argPeriNode = g.CreateUriNode("dbp:argPeri");
            predicates = g.GetTriplesWithPredicate(argPeriNode);
            string argPeri = predicates.FirstOrDefault()?.Object.ToSafeString();

            IUriNode ascNodeNode = g.CreateUriNode("dbp:ascNode");
            predicates = g.GetTriplesWithPredicate(ascNodeNode);
            string ascNode = predicates.FirstOrDefault()?.Object.ToSafeString();

            IUriNode atmosphereNode = g.CreateUriNode("dbp:atmosphere");
            predicates = g.GetTriplesWithPredicate(atmosphereNode);
            string atmosphere = predicates.FirstOrDefault()?.Object.ToSafeString();

            IUriNode atmosphereCompositionNode = g.CreateUriNode("dbp:atmosphereComposition");
            predicates = g.GetTriplesWithPredicate(atmosphereCompositionNode);
            string atmosphereComposition = predicates.FirstOrDefault()?.Object.ToSafeString();

            IUriNode satelitesNode = g.CreateUriNode("dbp:satelites");
            predicates = g.GetTriplesWithPredicate(satelitesNode);
            string satelites = predicates.FirstOrDefault()?.Object.ToSafeString();

            IUriNode siderealDayNode = g.CreateUriNode("dbp:siderealDay");
            predicates = g.GetTriplesWithPredicate(siderealDayNode);
            string siderealDay = predicates.FirstOrDefault()?.Object.ToSafeString();

            IUriNode surfaceAreaNode = g.CreateUriNode("dbp:surfaceArea");
            predicates = g.GetTriplesWithPredicate(surfaceAreaNode);
            string surfaceArea = predicates.FirstOrDefault()?.Object.ToSafeString();

            IUriNode surfaceGravNode = g.CreateUriNode("dbp:surfaceGrav");
            predicates = g.GetTriplesWithPredicate(surfaceGravNode);
            string surfaceGrav = predicates.FirstOrDefault()?.Object.ToSafeString();

            PlanetVm planet = new PlanetVm
            {
                Subject = subject,
                Label = label,
                Name = name,
                Abstract = abstractValue,
                AverageSpeed = averageSpeed,
                EscapeVelocity = escapeVelocity,
                MeanTemperature = meanTemperature,
                Thumbnail = thumbnail,
                Caption = caption,
                AngularSize = angularSize,
                ArgPeri = argPeri,
                AscNode = ascNode,
                Atmosphere = atmosphere,
                AtmosphereComposition = atmosphereComposition,
                Satelites = satelites,
                SiderealDay = siderealDay,
                SurfaceArea = surfaceArea,
                SurfaceGrav = surfaceGrav
            };

            return Ok(planet); */
            #endregion
            Uri uri = new Uri(WebUtility.UrlDecode(id));

            SparqlParameterizedString queryString = new SparqlParameterizedString();
            queryString.Namespaces.AddNamespace("dbo", new Uri("http://dbpedia.org/ontology/"));
            queryString.Namespaces.AddNamespace("dbp", new Uri("http://dbpedia.org/property/"));
            queryString.Namespaces.AddNamespace("dbr", new Uri("http://dbpedia.org/resource/"));
            queryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            queryString.Namespaces.AddNamespace("rdf", new Uri("https://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            queryString.Namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/0.1/"));
            queryString.Namespaces.AddNamespace("owl", new Uri("http://www.w3.org/2002/07/owl#"));
            queryString.Namespaces.AddNamespace("xsd", new Uri("http://www.w3.org/2001/XMLSchema#"));
            queryString.CommandText = SparqlCommands.GetSinglePlanet();
            queryString.SetUri("subjectUri", uri);
            //queryString.BaseUri = uri;

            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlQuery query = parser.ParseFromString(queryString);
            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"), "http://dbpedia.org");

            SparqlResultSet results = endpoint.QueryWithResultSet(query.ToString());
            SparqlResult resultNode = results.FirstOrDefault();

            string subject = ((UriNode)resultNode["subject"])?.Uri.ToSafeString();
            string label = ((LiteralNode)resultNode["label"])?.Value.ToSafeString();
            string name = ((LiteralNode)resultNode["name"])?.Value.ToSafeString();
            string comment = ((LiteralNode)resultNode["comment"])?.Value.ToSafeString();
            string abstractValue = ((LiteralNode)resultNode["abstract"])?.Value.ToSafeString();
            string averageSpeed = ((LiteralNode)resultNode["averageSpeed"])?.Value.ToSafeString();
            string meanTemperature = ((LiteralNode)resultNode["meanTemperatureAggr"])?.Value.ToSafeString();
            string thumbnail = ((UriNode)resultNode["thumbnail"])?.Uri.ToSafeString();
            string atmosphere = (resultNode["atmosphere"] is UriNode) ? ((UriNode)resultNode["atmosphere"])?.Uri.ToSafeString() : ((LiteralNode)resultNode["atmosphere"])?.Value.ToSafeString();
            string atmosphereComposition = (resultNode["atmosphereCompositionAggr"] is UriNode) ? ((UriNode)resultNode["atmosphereCompositionAggr"])?.Uri.ToSafeString() : ((LiteralNode)resultNode["atmosphereCompositionAggr"])?.Value.ToSafeString();
            string satelites = (resultNode["satellitesAggr"] is UriNode) ? ((LiteralNode)resultNode["satelliteLabel"])?.Value.ToSafeString() : ((LiteralNode)resultNode["satellitesAggr"])?.Value.ToSafeString();
            string surfaceArea = ((LiteralNode)resultNode["surfaceAreaAggr"])?.Value.ToSafeString();
            string surfaceArea2 = ((LiteralNode)resultNode["surfaceAreaAggr2"])?.Value.ToSafeString();

            PlanetVm planet = new PlanetVm
            {
                Subject = subject,
                Label = label,
                Name = name,
                Abstract = abstractValue,
                AverageSpeed = averageSpeed,
                MeanTemperature = meanTemperature,
                Thumbnail = thumbnail,
                Atmosphere = atmosphere,
                AtmosphereComposition = atmosphereComposition,
                Satelites = satelites,
                SurfaceArea = string.IsNullOrEmpty(surfaceArea) ? surfaceArea2 : surfaceArea,
            };


            return Ok(planet);
        }
    }
}