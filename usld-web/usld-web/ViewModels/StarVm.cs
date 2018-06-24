using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace usld_web.ViewModels
{
    public class StarVm : CelestialBodyVm
    {
        public string Luminosity { get; set; }
        public string Radius { get; set; }
        public string Temperature { get; set; }
        public string Mass { get; set; }
        public string Gravity { get; set; }
        public string Epoch { get; set; }
        public string ConstellationUri { get; set; }
        public string ConstellationName { get; set; }
    }
}
