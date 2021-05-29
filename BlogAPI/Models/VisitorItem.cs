using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.DTO
{
    public class VisitorItem
    {
        public string VisitorIP { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
    }
}
