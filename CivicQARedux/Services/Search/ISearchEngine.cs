using CivicQARedux.Models.FormResponses;
using CivicQARedux.Models.Search;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CivicQARedux.Services.Search
{
    public interface ISearchEngine
    {
        Task<List<FormResponse>> Search(SearchQuery query, IdentityUser<int> user);
    }
}
