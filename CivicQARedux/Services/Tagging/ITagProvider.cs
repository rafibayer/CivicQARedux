using CivicQARedux.Models.FormResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CivicQARedux.Services.Tagging
{
    public interface ITagProvider
    {
        Task<List<string>> GenerateTags(FormResponse response);
    }
}
