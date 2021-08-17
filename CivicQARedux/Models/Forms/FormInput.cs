using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CivicQARedux.Models.Forms
{
    public class FormInput
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Title { get; set; }
    }
}
