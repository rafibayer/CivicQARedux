using CivicQARedux.Models.FormResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CivicQARedux.Models.Search
{
    public class SearchResult
    {
        public SearchQuery Query { get; set; }
        public List<FormResponse> Results { get; set; }
    }
}
