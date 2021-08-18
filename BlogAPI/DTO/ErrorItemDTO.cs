using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
