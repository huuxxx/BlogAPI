using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Models
{
    public class PageViewed
    {
        public int SessionId { get; set; }
        public string PageType { get; set; }
    }
}
