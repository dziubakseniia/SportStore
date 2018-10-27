using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportsStore.WebUI.Models
{
    public class CategorySortingViewModel
    {
        public IEnumerable<string> Categories { get; set; }
        public List<string> Sorting { get; set; }
    }
}