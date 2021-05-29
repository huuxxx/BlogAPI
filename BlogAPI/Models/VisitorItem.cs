﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.DTO
{
    public class VisitorItem
    {
        [Key]
        public int Id { get; set; }
        public string VisitorIP { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
    }
}
