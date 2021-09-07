using CivicQARedux.Models.FormResponses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CivicQARedux.Models.Tags
{
    public class TagInput
    {
        [Required]
        [StringLength(20, MinimumLength = 1)]
        [Display(Name = "Tag")]
        public string Text { get; set; }

        [Required]
        public int FormResponseId { get; set; }
    }
}
