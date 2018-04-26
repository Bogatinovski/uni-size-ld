using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace usld_web
{
    public static class SparqlCommands
    {
        public static string GetHomePage()
        {
            return @"SELECT ?subject ?label ?comment
                        WHERE {
	                        ?subject rdfs:subClassOf dbo:CelestialBody ;
			                            rdfs:label ?label .
		                        FILTER (lang(?label) = 'en') .
		                        OPTIONAL {
			                        ?subject rdfs:comment ?comment .
                                    FILTER (lang(?comment) = 'en')
		                        }
                        }";
        }

        public static string GetPlanet()
        {
            return @"SELECT DISTINCT ?subject ?label ?name ?abstract ?averageSpeed ?escapeVelocity (MAX(?meanTemperature) AS ?meanTemperatureAggr) ?thumbnail (MIN(?caption) AS ?captionAggr) ?angularSize ?argPeri ?ascNode ?atmosphere (MIN(?atmosphereComposition) AS ?atmosphereCompositionAggr) ?satelites ?siderealDay (MAX(?surfaceArea) AS ?surfaceAreaAggr) (MAX(?surfaceGrav) AS ?surfaceGravAggr)
                        WHERE {
	                        ?subject ?predicate dbo:Planet ;
	                                 ?predicate dbo:CelestialBody;
                                         rdfs:label ?label ;
			                         dbo:abstract ?abstract ;
			                         ?predicate2 dbr:Planet ;
						             foaf:name ?name .
		                        FILTER (langMatches(lang(?abstract), 'EN') && langMatches(lang(?label), 'EN') && langMatches(lang(?name), 'EN')) .
		                        OPTIONAL {
			                        ?subject	dbo:averageSpeed 	?averageSpeed ;
						                        dbo:escapeVelocity	?escapeVelocity ;
						                        dbo:meanTemperature	?meanTemperature ;
						                        dbo:thumbnail		?thumbnail ;
						                        dbp:angularSize		?angularSize ;
						                        dbp:argPeri			?argPeri ;
						                        dbp:ascNode			?ascNode ;
						                        dbp:atmosphere		?atmosphere ;
						                        dbp:atmosphereComposition ?atmosphereComposition ;
						                        dbp:caption			?caption ;
						                        dbp:satellites		?satelites ;
						                        dbp:siderealDay		?siderealDay ;
						                        dbp:surfaceArea		?surfaceArea ;
						                        dbp:surfaceGrav		?surfaceGrav.
		                        }
		
                        } 
                        GROUP BY ?subject ?label ?name ?abstract ?averageSpeed ?escapeVelocity ?thumbnail ?angularSize ?argPeri ?ascNode ?atmosphere ?satelites ?siderealDay 
                        ORDER BY ?label ?name
                        LIMIT 100";
        }

        public static string GetListOfObjects()
        {
            return @"SELECT DISTINCT ?subject ?label ?thumbnail ?comment
                        WHERE {
	                        ?subject ?predicate @bodyType ;
	                                 ?predicate dbo:CelestialBody;
                                     rdfs:label     ?label ;
                                     dbo:thumbnail		?thumbnail ;
                                     rdfs:comment       ?comment .
		                        FILTER (langMatches(lang(?label), 'EN') && langMatches(lang(?comment),'EN')) .
		                        OPTIONAL {
			                        ?subject	foaf:name ?name .
						            FILTER (langMatches(lang(?name), 'EN')) .
		                        }
                        } 
                        ORDER BY ?label 
                        LIMIT 100";
        }
    }
}
