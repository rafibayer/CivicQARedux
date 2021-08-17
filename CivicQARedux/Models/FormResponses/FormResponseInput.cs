using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CivicQARedux.Models.FormResponses
{
    public class FormResponseInput
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(320)] // RFC Errata 1690
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }

        [Required]
        [MaxLength(200)]
        public string Subject { get; set; }

        [Required]
        [MaxLength(5000)]
        public string Body { get; set; }

        [Required]
        public int FormId { get; set; }
    }
}
