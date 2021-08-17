
using CivicQARedux.Models.FormResponses;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CivicQARedux.Models.Forms
{
    public class Form
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Title { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public List<FormResponse> FormResponses { get; set; } = new List<FormResponse>();

        [Required]
        public int UserId { get; set; }

        [Required]
        public IdentityUser<int> user { get; set; }

        public static Form FromInput(FormInput input, IdentityUser<int> user)
        {
            return new Form
            {
                Title = input.Title,
                CreatedAt = DateTime.Now,
                UserId = user.Id,
            };
        }
    }
}
