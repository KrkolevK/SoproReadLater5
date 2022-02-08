using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReadLater5.Models.Requests
{
    public class BookmarkRequest
    {
        public int? Id { get; set; }
        [Required]
        public string URL { get; set; }
        [Required]
        [StringLength(maximumLength: 500)]
        public string ShortDescription { get; set; }
        public int? CategoryId { get; set; }
    }
}
