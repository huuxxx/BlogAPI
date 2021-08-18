using System;

namespace BlogAPI.Models
{
    public class ErrorItem
    {
        public Guid Id { get; set; }
        public string DateCreated { get; set; }
        public string StackTrace { get; set; }
        public string Message { get; set; }
    }
}
