using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using usld_web.ViewModels;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Query.Datasets;

namespace usld_web.Controllers
{
    [Route("api/[controller]")]
    public class UniverseController : Controller
    {
        // GET api/Home
        [HttpGet]
        [ProducesResponseType(typeof(HomeVm), 200)]
        public IActionResult Home()
        {
            TripleStore store = new TripleStore();
            store.AddFromUri(new Uri("http://dbpedia.org/page/Astronomical_object"));
            InMemoryDataset ds = new InMemoryDataset(store, true);
            ISparqlQueryProcessor processor = new LeviathanQueryProcessor(ds);

            SparqlParameterizedString queryString = new SparqlParameterizedString();
            queryString.Namespaces.AddNamespace("dct", new Uri("http://dublincore.org/2012/06/14/dcterms#"));
            queryString.Namespaces.AddNamespace("dbo", new Uri("http://dbpedia.org/ontology/"));
            queryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            queryString.Namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/0.1/"));
            queryString.CommandText = "SELECT ?abstract ?depiction WHERE { ?subject dbo:abstract ?abstract; foaf:depiction ?depiction . FILTER (lang(?abstract) = 'en') }";

            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlQuery query = parser.ParseFromString(queryString);
            SparqlResultSet results = (SparqlResultSet)processor.ProcessQuery(query);
            SparqlResult resultNode = results.FirstOrDefault();

            string description = ((LiteralNode)resultNode["abstract"]).Value;
            string depiction = ((UriNode)resultNode["depiction"]).Uri.ToString();

            HomeVm model = new HomeVm
            {
                Abstract = description,
                Depiction = depiction,
                Categories = new List<HomeCategoryVm>()
            };


            queryString.CommandText = SparqlCommands.GetHomePage();

            query = parser.ParseFromString(queryString);

            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"), "http://dbpedia.org");
            results = endpoint.QueryWithResultSet(query.ToString());

            foreach (SparqlResult result in results)
            {
                string uri = ((UriNode)result["subject"]).Uri.ToString();
                string comment = ((LiteralNode)result["comment"])?.Value;
                string name = ((LiteralNode)result["label"])?.Value;

                HomeCategoryVm category = new HomeCategoryVm
                {
                    Uri = uri,
                    Comment = comment,
                    Name = name
                };

                model.Categories.Add(category);
            }

            return Ok(model);
        }

        [HttpGet]
        [Route("Body/{bodyTypeUri}")]
        [ProducesResponseType(typeof(IEnumerable<ObjectPartialVm>), 200)]
        public IActionResult GetPlanets([FromRoute]string bodyTypeUri)
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

            foreach(SparqlResult result in results)
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
        [Route("Planet/{planetUri}")]
        [ProducesResponseType(typeof(PlanetVm), 200)]
        public IActionResult GetPlanet(string planetUri)
        {
            planetUri = WebUtility.UrlDecode(planetUri);
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
            
            IUriNode captionNode = g.CreateUriNode(new Uri("dbp:caption"));
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

            return Ok(planet);
        }

        // GET api/Home
       /* [HttpGet]
        public IEnumerable<string> Get()
        {
            //Create a Parameterized String
            SparqlParameterizedString queryString = new SparqlParameterizedString();
            queryString.Namespaces.AddNamespace("dct", new Uri("http://dublincore.org/2012/06/14/dcterms#"));
            queryString.Namespaces.AddNamespace("dbo", new Uri("http://dbpedia.org/ontology/"));
            queryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            queryString.CommandText = "SELECT ?subject WHERE { ?subject rdfs:subClassOf dbo:CelestialBody  }";

            Console.WriteLine(queryString.ToString());

            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlQuery query = parser.ParseFromString(queryString);

            SparqlRemoteEndpoint endpoint = new SparqlRemoteEndpoint(new Uri("http://dbpedia.org/sparql"), "http://dbpedia.org");
            SparqlResultSet results = endpoint.QueryWithResultSet(query.ToString());

            foreach (SparqlResult result in results)
            {
                Console.WriteLine(result.ToString());
                yield return result.ToString();
            }
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }*/
    }
}
