﻿using System;
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

                GetThumbnail(category);

                model.Categories.Add(category);
            }

            return Ok(model);
        }

        private void GetThumbnail(HomeCategoryVm category)
        {
            switch(category.Uri)
            {
                case "http://dbpedia.org/ontology/Satellite":
                    category.Thumbnail = "http://bpic.588ku.com/element_pic/16/12/07/0544afd0d2c9a4772cfbff5bc3e1f02f.jpg";
                    break;

                case "http://dbpedia.org/ontology/Asteroid":
                    category.Thumbnail = "https://www.macupdate.com/images/icons256/48979.png";
                    break;

                case "http://dbpedia.org/ontology/Constellation":
                    category.Thumbnail = "https://pm1.narvii.com/6433/6e8376431362f95295039d47d914d46467533af7_128.jpg";
                    break;

                case "http://dbpedia.org/ontology/Galaxy":
                    category.Thumbnail = "https://pbs.twimg.com/profile_images/378800000849825178/3a7a6ec6fdb5121c8a3095e14f5e6e71.jpeg";
                    break;

                case "http://dbpedia.org/ontology/Planet":
                    category.Thumbnail = "http://sciencediariescom-wp-media.s3-eu-central-1.amazonaws.com/wp-content/uploads/2017/01/19133731/planets-colours.jpg";
                    break;

                case "http://dbpedia.org/ontology/Star":
                    category.Thumbnail = "http://web.utah.edu/astro/astroinfo/eit001_prev.jpg";
                    break;
            }
        }
    }
}
