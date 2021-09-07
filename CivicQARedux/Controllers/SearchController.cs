using CivicQARedux.Models.FormResponses;
using CivicQARedux.Models.Search;
using CivicQARedux.Services;
using CivicQARedux.Services.Search;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CivicQARedux.Controllers
{
    [Authorize]
    public class SearchController : Controller
    {
        private readonly ISearchEngine _engine;
        private readonly ICurrentUserProvider _userProvider;

        public SearchController(ISearchEngine searchEngine, ICurrentUserProvider currentUserProvider)
        {
            _engine = searchEngine;
            _userProvider = currentUserProvider;
        }


        [HttpPost]
        public async Task<IActionResult> Index([Bind("Subject,Body,Tag")] SearchQuery query)
        {
            var user = await _userProvider.GetCurrentUser();
            if (user is null)
            {
                return Unauthorized();
            }

            List<FormResponse> matches = await _engine.Search(query, user);
            SearchResult result = new() { Query = query, Results = matches };
            return View(result);
        }
    }
}
