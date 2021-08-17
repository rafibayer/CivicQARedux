using CivicQARedux.Models.FormResponses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CivicQARedux.Models.Tags
{
    public class Tag
    {
        [Key]
        public int Key { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 1)]
        public string Text { get; set; }

        [Required]
        public int FormResponseId { get; set; }

        [Required]
        public FormResponse FormResponse { get; set; }
    }
}
