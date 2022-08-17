using System.ComponentModel.DataAnnotations;

namespace BlogAPI.DTO
{
    public class VisitorItemDto
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
