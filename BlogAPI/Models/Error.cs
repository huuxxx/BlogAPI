using System;
using System.ComponentModel.DataAnnotations;

namespace BlogAPI.Models
{
    public class Error
    {
        [Key]
        public Guid Id { get; set; }
        public string DateCreated { get; set; }
        public string StackTrace { get; set; }
        public string Message { get; set; }
    }
}
