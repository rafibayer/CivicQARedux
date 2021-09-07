using CivicQARedux.Data;
using CivicQARedux.Models.FormResponses;
using CivicQARedux.Models.Search;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CivicQARedux.Services.Search
{
    public class SearchEngine : ISearchEngine
    {
        private readonly ApplicationContext _context;

        public SearchEngine(ApplicationContext applicationContext)
        {
            _context = applicationContext;
        }

        public Task<List<FormResponse>> Search(SearchQuery query, IdentityUser<int> user)
        {
            var results = _context.Responses
                .Where(r => r.Form.UserId == user.Id);

            if (query.Subject is not null)
            {
                string subject = query.Subject.ToUpper();
                results = results.Where(r => r.Subject.ToUpper().Contains(subject));
            }

            if (query.Body is not null)
            {
                string body = query.Body.ToUpper();
                results = results.Where(r => r.Body.ToUpper().Contains(body));
            }

            // Note: Tags are always normalized to upper-case,
            // so we can skip making the column uppercase in the Where
            if (query.Tag is not null)
            {
                string tag = query.Tag.ToUpper();
                results = results.Where(r => r.Tags.Any(t => t.Text.Contains(tag)));
            }

            return results.ToListAsync();
        }
    }
}
