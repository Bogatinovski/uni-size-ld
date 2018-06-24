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

        // Planets
        public static string GetSinglePlanet()
        {
            return @"
                    SELECT DISTINCT ?subject ?label ?name ?comment ?abstract ?averageSpeed (MAX(?meanTemperature) AS ?meanTemperatureAggr) ?thumbnail ?atmosphere (MIN(?atmosphereComposition) AS ?atmosphereCompositionAggr) (MIN(?satellites) AS ?satellitesAggr) ?satelliteLabel (MAX(?surfaceArea) AS ?surfaceAreaAggr) (MAX(?surfaceArea2) AS ?surfaceAreaAggr2)
                    WHERE {
	                    ?subject ?predicate 				dbo:Planet ;
			                     ?predicate 				dbo:CelestialBody;
			                     rdfs:label 				?label ;
			                     dbo:abstract 				?abstract ;
			                     rdfs:comment 				?comment ;
			                     ?predicate2 				dbr:Planet ;
			                     dbo:thumbnail				?thumbnail ;
			                     foaf:name 					?name .
		                    FILTER (str(?subject) = @subjectUri && langMatches(lang(?abstract), 'EN') && langMatches(lang(?label), 'EN') && langMatches(lang(?name), 'EN') && langMatches(lang(?comment), 'EN')) .
		                    OPTIONAL { ?subject	dbo:averageSpeed 		?averageSpeed . }

		                    OPTIONAL { ?subject	dbp:atmosphereComposition		?atmosphereComposition . }
		                    OPTIONAL { ?subject	dbp:surfaceGrav					?surfaceGrav . }
		                    OPTIONAL { ?subject	dbp:atmosphere					?atmosphere . }
		                    OPTIONAL { ?subject	dbp:satellites					?satellites . 
                                OPTIONAL {  ?satellites rdfs:label ?satelliteLabel   
                                            FILTER (langMatches(lang(?satelliteLabel), 'EN')) } 
                            }
		                    OPTIONAL { ?subject	dbp:surfaceArea					?surfaceArea . }
		                    OPTIONAL { ?subject	dbo:surfaceArea					?surfaceArea2 . }
		                    OPTIONAL { ?subject	dbo:meanTemperature				?meanTemperature . }
                            
                    } 
                    GROUP BY ?subject ?label ?name ?comment ?abstract ?thumbnail ?atmosphere ?averageSpeed ?satelliteLabel
                    ORDER BY ?label ?name
                    LIMIT 1";
        }

        public static string GetListOfPlanets()
        {
            return @"SELECT DISTINCT ?subject ?label ?name ?comment ?abstract ?averageSpeed ?escapeVelocity (MAX(?meanTemperature) AS ?meanTemperatureAggr) ?thumbnail (MIN(?caption) AS ?captionAggr) ?angularSize ?argPeri ?ascNode ?atmosphere (MIN(?atmosphereComposition) AS ?atmosphereCompositionAggr) ?satelites ?siderealDay (MAX(?surfaceArea) AS ?surfaceAreaAggr) (MAX(?surfaceGrav) AS ?surfaceGravAggr)
                        WHERE {
	                        ?subject ?predicate dbo:Planet ;
			                         ?predicate dbo:CelestialBody;
				                         rdfs:label ?label ;
			                         dbo:abstract ?abstract ;
			                         rdfs:comment ?comment ;
			                         ?predicate2 dbr:Planet ;
			                         dbo:thumbnail		?thumbnail ;
			                         foaf:name ?name .
		                        FILTER (langMatches(lang(?abstract), 'EN') && langMatches(lang(?label), 'EN') && langMatches(lang(?name), 'EN') && langMatches(lang(?comment), 'EN')) .
		                        OPTIONAL {
			                        ?subject	dbo:averageSpeed 	?averageSpeed ;
						                        dbo:escapeVelocity	?escapeVelocity ;
						                        dbo:meanTemperature	?meanTemperature ;
						
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
                        GROUP BY ?subject ?label ?name ?comment ?abstract ?averageSpeed ?escapeVelocity ?thumbnail ?angularSize ?argPeri ?ascNode ?atmosphere ?satelites ?siderealDay 
                        ORDER BY ?label ?name
                        LIMIT 100";
        }

        // Satellites
        public static string GetListOfSatellites()
        {
            return @"SELECT DISTINCT ?subject ?label ?thumbnail ?comment ?axialTilt ?abstract (MAX(?inclination) AS ?inclinationAggr)
                    WHERE {
	                    ?subject    ?predicate      dbo:CelestialBody ;
			                        ?predicate      dbo:Place;
                                    dbp:satelliteOf ?satelliteOf ;
                                    rdfs:label      ?label ;
                                    dbo:thumbnail   ?thumbnail ;
                                    dbp:axialTilt   ?axialTilt ;
                                    dbp:inclination ?inclination ;
			                        dbo:abstract 	?abstract ;
                                    rdfs:comment    ?comment .
                             FILTER(langMatches(lang(?label), 'EN') && langMatches(lang(?comment), 'EN') && langMatches(lang(?abstract), 'EN'))
                    } 
                    GROUP BY ?subject ?label ?thumbnail ?comment ?axialTilt ?abstract
                    ORDER BY ?label
                    LIMIT 100";
        }

        public static string GetSingleSatellite()
        {
            return @"SELECT DISTINCT ?subject ?label ?thumbnail ?comment ?axialTilt ?abstract (MAX(?inclination) AS ?inclinationAggr)
                    WHERE {
	                    ?subject    ?predicate      dbo:CelestialBody ;
			                        ?predicate      dbo:Place;
                                    dbp:satelliteOf ?satelliteOf ;
                                    rdfs:label      ?label ;
                                    dbo:thumbnail   ?thumbnail ;
                                    dbp:axialTilt   ?axialTilt ;
                                    dbp:inclination ?inclination ;
			                        dbo:abstract 	?abstract ;
                                    rdfs:comment    ?comment .
                             FILTER(str(?subject) = @subjectUri && langMatches(lang(?label), 'EN') && langMatches(lang(?comment), 'EN') && langMatches(lang(?abstract), 'EN'))
                    } 
                    GROUP BY ?subject ?label ?thumbnail ?comment ?axialTilt ?abstract
                    ORDER BY ?label
                    LIMIT 1";
        }

        // Constelations
        public static string GetListOfConstellations()
        {
            return @"SELECT DISTINCT ?subject ?numberbfstars ?numbermainstars ?symbolism (MAX(?meteors) AS ?meteorsAggr) ?meteorsLabel ?label ?thumbnail ?comment 
                    WHERE {
	                    ?subject    ?predicate          dbo:CelestialBody ;
				                    ?predicate          dbo:Constellation ;
				                    rdfs:label          ?label ;
				                    dbo:thumbnail       ?thumbnail ;
                                    dbp:numberbfstars   ?numberbfstars ;
                                    dbp:numbermainstars ?numbermainstars ;
                                    dbp:meteorshowers   ?meteors ;
                                    dbp:symbolism       ?symbolism ;
				                    rdfs:comment        ?comment .
                                    OPTIONAL { ?meteors rdfs:label ?meteorsLabel . 
                                        FILTER(langMatches(lang(?meteorsLabel), 'EN')) 
                                    }
                                    OPTIONAL { ?subject dbp:quadrant ?quadrant . }
		                     FILTER(langMatches(lang(?label), 'EN') && langMatches(lang(?comment), 'EN'))
                    }
                    GROUP BY ?subject ?meteorsLabel ?numberbfstars ?numbermainstars ?symbolism ?label ?thumbnail ?comment
                    ORDER BY ?label
                    LIMIT 100";
        }

        public static string GetSingleConstellation()
        {
            return @"SELECT DISTINCT ?subject ?numberbfstars ?numbermainstars ?symbolism (MAX(?meteors) AS ?meteorsAggr) ?meteorsLabel ?label ?thumbnail ?abstract 
                    WHERE {
	                    ?subject    ?predicate          dbo:CelestialBody ;
				                    ?predicate          dbo:Constellation ;
				                    rdfs:label          ?label ;
				                    dbo:thumbnail       ?thumbnail ;
                                    dbp:numberbfstars   ?numberbfstars ;
                                    dbp:numbermainstars ?numbermainstars ;
                                    dbp:meteorshowers   ?meteors ;
                                    dbp:symbolism       ?symbolism ;
				                    dbo:abstract        ?abstract .
                                    OPTIONAL { ?meteors rdfs:label ?meteorsLabel . 
                                        FILTER(langMatches(lang(?meteorsLabel), 'EN')) 
                                    }
                                    OPTIONAL { ?subject dbp:quadrant ?quadrant . }
		                     FILTER(str(?subject) = @subjectUri && langMatches(lang(?label), 'EN') && langMatches(lang(?abstract), 'EN'))
                    }
                    GROUP BY ?subject ?meteorsLabel ?numberbfstars ?numbermainstars ?symbolism ?label ?thumbnail ?abstract
                    ORDER BY ?label
                    LIMIT 1";
        }

        // Asteroids
        public static string GetListOfAsteroids()
        {
            return @"SELECT DISTINCT ?subject ?label ?thumbnail ?comment
                WHERE {
	                ?subject    ?predicate      dbo:Asteroid ;
				                rdfs:label      ?label ;
				                rdfs:comment    ?comment .
		                FILTER(langMatches(lang(?label), 'EN') && langMatches(lang(?comment), 'EN'))
                        OPTIONAL { ?subject dbo:thumbnail ?thumbnail . }
                } 
                ORDER BY ?label
                LIMIT 100";
        }

        public static string GetSingleAsteroid()
        {
            return @"SELECT DISTINCT ?subject ?label ?thumbnail ?comment ?abstract
                    WHERE {
	                    ?subject    ?predicate      dbo:Asteroid ;
				                    rdfs:label      ?label ;
                                    dbo:abstract    ?abstract ;
				                    rdfs:comment    ?comment .
		                    FILTER(str(?subject) = @subjectUri && langMatches(lang(?label), 'EN') && langMatches(lang(?comment), 'EN'))
                            OPTIONAL { ?subject dbo:thumbnail ?thumbnail . }
                    } 
                    ORDER BY ?label
                    LIMIT 1";
        }


        // Galaxies
        public static string GetListOfGalaxies()
        {
            return @"SELECT DISTINCT ?subject ?label ?thumbnail ?comment
                WHERE {
	                ?subject    ?predicate      dbo:Galaxy ;
				                rdfs:label      ?label ;
                                dbo:thumbnail   ?thumbnail ;
                                dbp:size        ?size ;
				                rdfs:comment    ?comment .
		                FILTER(langMatches(lang(?label), 'EN') && langMatches(lang(?comment), 'EN'))
                } 
                ORDER BY ?label
                LIMIT 100";
        }

        public static string GetSingleGalaxy()
        {
            return @"SELECT DISTINCT ?subject ?label ?thumbnail ?abstract ?size
                WHERE {
	                ?subject    ?predicate      dbo:Galaxy ;
				                rdfs:label      ?label ;
                                dbo:thumbnail   ?thumbnail ;
                                dbp:size        ?size ;
				                dbo:abstract    ?abstract .
		                FILTER(str(?subject) = @subjectUri && langMatches(lang(?label), 'EN') && langMatches(lang(?abstract), 'EN'))
                } 
                ORDER BY ?label
                LIMIT 1";
        }

        // Stars
        public static string GetListOfStars()
        {
            return @"SELECT DISTINCT ?subject (MIN(?luminosity) AS ?luminosityAggr) (MIN(?radius) AS ?radiusAggr) (MIN(?temperature) as ?temperatureAggr) (MIN(?mass) AS ?massAggr) ?constel ?constelLabel (MAX(?gravity) AS ?gravityAggr) (MAX(?epoch) AS ?epochAggr) ?label ?thumbnail ?comment
                WHERE {
	                ?subject    ?predicate      dbo:Star ;
                                ?predicate      dbo:CelestialBody ;
				                rdfs:label      ?label ;
                                dbo:thumbnail   ?thumbnail ;
                                dbp:epoch       ?epoch ;
                                dbp:gravity     ?gravity ;
                                dbp:mass        ?mass ;
                                dbp:temperature ?temperature ;
                                dbp:radius      ?radius ;
                                dbp:luminosity  ?luminosity ;
                                dbp:constell    ?constel .
                                    ?constel    rdfs:label      ?constelLabel ;
				                rdfs:comment    ?comment .
		                FILTER(langMatches(lang(?label), 'EN') && langMatches(lang(?comment), 'EN') && langMatches(lang(?constelLabel), 'EN'))
                } 
                GROUP BY ?subject ?constelLabel ?label ?thumbnail ?comment ?constel
                ORDER BY ?label
                LIMIT 100";
        }

        public static string GetSingleStar()
        {
            return @"SELECT DISTINCT ?subject (MIN(?luminosity) AS ?luminosityAggr) (MIN(?radius) AS ?radiusAggr) (MIN(?temperature) as ?temperatureAggr) (MIN(?mass) AS ?massAggr) ?constel ?constelLabel (MAX(?gravity) AS ?gravityAggr) (MAX(?epoch) AS ?epochAggr) ?label ?thumbnail ?abstract
                WHERE {
	                ?subject    ?predicate      dbo:Star ;
                                ?predicate      dbo:CelestialBody ;
				                rdfs:label      ?label ;
                                dbo:thumbnail   ?thumbnail ;
                                dbp:epoch       ?epoch ;
                                dbp:gravity     ?gravity ;
                                dbp:mass        ?mass ;
                                dbp:temperature ?temperature ;
                                dbp:radius      ?radius ;
                                dbp:luminosity  ?luminosity ;
                                dbp:constell    ?constel .
                                    ?constel    rdfs:label      ?constelLabel ;
				                dbo:abstract    ?abstract .
		                FILTER(str(?subject) = @subjectUri && langMatches(lang(?label), 'EN') && langMatches(lang(?abstract), 'EN') && langMatches(lang(?constelLabel), 'EN'))
                } 
                GROUP BY ?subject ?constelLabel ?label ?thumbnail ?abstract ?constel
                ORDER BY ?label
                LIMIT 1";
        }


        // Swarms
        public static string GetListOfSwarms()
        {
            return @"SELECT DISTINCT ?subject ?label ?thumbnail ?comment
                WHERE {
	                ?subject    ?predicate      dbo:Swarm ;
                                ?predicate      dbo:CelestialBody ;
				                rdfs:label      ?label ;
                                dbo:thumbnail   ?thumbnail ;
				                rdfs:comment    ?comment .
		                FILTER(langMatches(lang(?label), 'EN') && langMatches(lang(?comment), 'EN'))
                } 
                ORDER BY ?label
                LIMIT 100";
        }

        [Obsolete]
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
