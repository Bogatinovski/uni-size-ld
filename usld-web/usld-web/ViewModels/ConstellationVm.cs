using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace usld_web.ViewModels
{
    public class ConstellationVm : CelestialBodyVm
    {
        public string NumberOfStars { get; set; }
        public string NumberOfMainStars { get; set; }
        public string Symbolism { get; set; }
        public string Meteors { get; set; }
    }
}
