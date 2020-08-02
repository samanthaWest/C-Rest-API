using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Models;

namespace TodoApi.Models
{
    public class Book
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }

        public long LibraryId { get; set; }
    }
}
