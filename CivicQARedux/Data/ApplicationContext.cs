using CivicQARedux.Models.FormResponses;
using CivicQARedux.Models.Forms;
using CivicQARedux.Models.Tags;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CivicQARedux.Data
{
    public class ApplicationContext : IdentityDbContext<IdentityUser<int>, IdentityRole<int>, int>
    {
        public ApplicationContext(DbContextOptions options)
            : base(options) 
        {
            
        }

        public DbSet<Form> Forms { get; set; }

        public DbSet<FormResponse> Responses { get; set; }

        public DbSet<Tag> Tags { get; set; }
    }
}
