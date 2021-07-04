using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.DTO
{
    public class VisitorItemConverted
    {
        [Key]
        public string Id { get; set; }
        public string VisitorIP { get; set; }
        public string ScreenWidth { get; set; }
        public string ScreenHeight { get; set; }
        public string DateVisited { get; set; }
        public string ViewedBlogs { get; set; }
        public string ViewedProjects { get; set; }
        public string ViewedAbout { get; set; }
    }
}
