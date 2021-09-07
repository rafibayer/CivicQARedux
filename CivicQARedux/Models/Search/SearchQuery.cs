using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CivicQARedux.Models.Search
{
    public class SearchQuery
    {
        [Display(Name = "Title")]
        public string Subject { get; set; }

        [Display(Name = "Body")]
        public string Body { get; set; }

        [Display(Name = "Tag")]
        public string Tag { get; set; }
    }
}
