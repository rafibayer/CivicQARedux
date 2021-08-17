using CivicQARedux.Models.Forms;
using CivicQARedux.Models.Tags;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CivicQARedux.Models.FormResponses
{
    public class FormResponse
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        [MaxLength(200)]
        public string Subject { get; set; }

        [Required]
        [MaxLength(5000)]
        public string Body { get; set; }

        [Required]
        [Display(Name = "Recieved At")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public List<Tag> Tags;

        [Required]
        public Form Form { get; set; }

        [Required]
        public int FormId { get; set; }

        public static FormResponse FromInput(FormResponseInput input, Form form)
        {
            return new FormResponse
            {
                FullName = input.FullName,
                EmailAddress = input.EmailAddress,
                Subject = input.Subject,
                Body = input.Body,
                CreatedAt = DateTime.Now,
                IsActive = true,
                FormId = form.Id
            };
        }


    }
}
