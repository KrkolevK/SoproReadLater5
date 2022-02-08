using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Models.Args
{
    public class BookmarkArgs
    {
        public int? Id { get; set; }
        public string URL { get; set; }
        public string ShortDescription { get; set; }
        public int? CategoryId { get; set; }
    }
}
