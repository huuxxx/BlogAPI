using System;
using System.ComponentModel.DataAnnotations;

namespace BlogAPI.DTO
{
    public class ErrorItemDTO
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
        public string StackTrace { get; set; }
        public string Message { get; set; }
    }
}
