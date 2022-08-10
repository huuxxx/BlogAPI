using System;
using System.ComponentModel.DataAnnotations;

namespace BlogAPI.Models
{
    public class Visitor
    {
        [Key]
        public int Id { get; set; }
        public string VisitorIP { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public DateTime DateVisited { get; set; }
        public bool ViewedBlogs { get; set; }
        public bool ViewedProjects { get; set; }
        public bool ViewedAbout { get; set; }
    }
}
