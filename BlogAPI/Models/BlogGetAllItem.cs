using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Models
{
    public class BlogGetAllItem
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public string DateCreated { get; set; }
    }
}
