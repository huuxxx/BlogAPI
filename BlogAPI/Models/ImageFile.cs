using Microsoft.AspNetCore.Http;

namespace BlogAPI.Models
{
    public class ImageFile
    {
        public IFormFile File { get; set; }
    }
}