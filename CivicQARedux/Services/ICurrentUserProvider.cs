
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CivicQARedux.Services
{
    public interface ICurrentUserProvider
    {
        Task<IdentityUser<int>> GetCurrentUser();
    }
}
