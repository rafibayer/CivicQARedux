
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CivicQARedux.Services
{
    public class CurrentUserProvider : ICurrentUserProvider
    {
        private readonly UserManager<IdentityUser<int>> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserProvider(UserManager<IdentityUser<int>> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IdentityUser<int>> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
        }
    }
}
