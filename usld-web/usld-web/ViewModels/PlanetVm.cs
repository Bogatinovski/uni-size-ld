using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace usld_web.ViewModels
{
    public class PlanetVm : CelestialBodyVm
    {
        public string Name { get; set; }
        public string AverageSpeed { get; set; }
        public string MeanTemperature { get; set; }
        public string Atmosphere { get; set; }
        public string AtmosphereComposition { get; set; }
        public string Satelites { get; set; }
        public string SurfaceArea { get; set; }
    }
}
