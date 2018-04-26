using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace usld_web.ViewModels
{
    public class HomeVm
    {
        public string Abstract { get; set; }
        public string Depiction { get; set; }
        public ICollection<HomeCategoryVm> Categories { get; set; }
    }
}
