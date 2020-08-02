using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class BookDTO
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public long LibraryId { get; set; }
    }
}
