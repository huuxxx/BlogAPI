using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.DTO
{
    public class BlogItemDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Requests { get; set; }
        public string DateCreated { get; set; }
        public string DateModified { get; set; }

    }
}
