using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.Data
{
    public class BookmarkData
    {
        public int Id { get; set; }
        public string URL { get; set; }
        public string ShortDescription { get; set; }
        public int? CategoryId { get; set; }
        [DisplayFormat(NullDisplayText = "No Category")]
        public string CategoryName { get; set; }
    }
}
