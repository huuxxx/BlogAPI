using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Models
{
    public class GetBlog
    {
        public int Id { get; set; }
        public bool PreventIncrement { get; set; }
    }
}
