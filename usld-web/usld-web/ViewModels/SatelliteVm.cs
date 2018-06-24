using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace usld_web.ViewModels
{
    public class SatelliteVm : CelestialBodyVm
    {
        public string AxialTilt { get; set; }
        public string Inclination { get; set; }
    }
}
