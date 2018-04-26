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

            TripleStore store = new TripleStore();
            store.AddFromUri(uri);
            InMemoryDataset ds = new InMemoryDataset(store, true);
            ISparqlQueryProcessor processor = new LeviathanQueryProcessor(ds);

            SparqlParameterizedString queryString = new SparqlParameterizedString();
            queryString.Namespaces.AddNamespace("dct", new Uri("http://dublincore.org/2012/06/14/dcterms#"));
            queryString.Namespaces.AddNamespace("dbo", new Uri("http://dbpedia.org/ontology/"));
            queryString.Namespaces.AddNamespace("rdfs", new Uri("http://www.w3.org/2000/01/rdf-schema#"));
            queryString.Namespaces.AddNamespace("rdf", new Uri("https://www.w3.org/1999/02/22-rdf-syntax-ns#"));
            queryString.Namespaces.AddNamespace("foaf", new Uri("http://xmlns.com/foaf/0.1/"));
            queryString.Namespaces.AddNamespace("dbr", new Uri("http://dbpedia.org/resource/"));
            queryString.Namespaces.AddNamespace("dbp", new Uri("http://dbpedia.org/property/"));
            queryString.CommandText = SparqlCommands.GetPlanet();
            queryString.SetUri("planetUri", uri);

            SparqlQueryParser parser = new SparqlQueryParser();
            SparqlQuery query = parser.ParseFromString(queryString);
            SparqlResultSet results = (SparqlResultSet)processor.ProcessQuery(query);
            SparqlResult resultNode = results.FirstOrDefault();
            SparqlResult result = results.FirstOrDefault();

            string subject = ((UriNode)result["subject"])?.Uri.ToSafeString();
            string label = ((LiteralNode)result["label"])?.Value.ToSafeString();
            string name = ((LiteralNode)result["name"])?.Value.ToSafeString();
            string abstractNode = ((LiteralNode)result["abstract"])?.Value.ToSafeString();
            string averageSpeed = ((LiteralNode)result["averageSpeed"])?.Value.ToSafeString();
            string escapeVelocity = ((LiteralNode)result["escapeVelocity"])?.Value.ToSafeString();
            string meanTemperatureAggr = ((LiteralNode)result["meanTemperatureAggr"])?.Value.ToSafeString();
            string thumbnail = ((UriNode)result["thumbnail"])?.Uri.ToSafeString();
            string captionAggr = ((LiteralNode)result["captionAggr"])?.Value.ToSafeString();
            string angularSize = ((LiteralNode)result["angularSize"])?.Value.ToSafeString();
            string argPeri = ((LiteralNode)result["argPeri"])?.Value.ToSafeString();
            string ascNode = ((LiteralNode)result["ascNode"])?.Value.ToSafeString();
            string atmosphere = result["atmosphere"].ToSafeString();
            string atmosphereCompositionAggr = result["atmosphereCompositionAggr"].ToSafeString();
            string satelites = ((LiteralNode)result["satelites"])?.Value.ToSafeString();
            string siderealDay = ((LiteralNode)result["siderealDay"])?.Value.ToSafeString();
            string surfaceAreaAggr = ((LiteralNode)result["surfaceAreaAggr"])?.Value.ToSafeString();
            string surfaceGravAggr = ((LiteralNode)result["surfaceGravAggr"])?.Value.ToSafeString();

            PlanetVm planet = new PlanetVm
            {
                Subject = subject,
                Label = label,
                Name = name,
                Abstract = abstractNode,
                AverageSpeed = averageSpeed,
                EscapeVelocity = escapeVelocity,
                MeanTemperature = meanTemperatureAggr,
                Thumbnail = thumbnail,
                Caption = captionAggr,
                AngularSize = angularSize,
                ArgPeri = argPeri,
                AscNode = ascNode,
                Atmosphere = atmosphere,
                AtmosphereComposition = atmosphereCompositionAggr,
                Satelites = satelites,
                SiderealDay = siderealDay,
                SurfaceArea = surfaceAreaAggr,
                SurfaceGrav = surfaceGravAggr
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
