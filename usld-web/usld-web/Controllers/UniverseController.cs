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
    [Route("api/Universe")]
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
    }
}
