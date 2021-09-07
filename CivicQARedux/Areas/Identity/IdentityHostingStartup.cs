using System;
using CivicQARedux.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(CivicQARedux.Areas.Identity.IdentityHostingStartup))]
namespace CivicQARedux.Areas.Identity
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class IdentityHostingStartup : IHostingStartup
    {
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="builder">Host Builder</param>
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {

                services.AddDefaultIdentity<IdentityUser<int>>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<ApplicationContext>();
            });
        }
    }
}