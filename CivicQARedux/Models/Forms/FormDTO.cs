using CivicQARedux.Models.FormResponses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CivicQARedux.Models.Forms
{
    public class FormDTO
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Responses")]
        public List<FormResponse> FormResponses { get; set; }

        public static FormDTO FromForm(Form form)
        {
            return new FormDTO
            {
                Id = form.Id,
                Title = form.Title,
                CreatedAt = form.CreatedAt,
                FormResponses = form.FormResponses,
            };
        }
    }
}
