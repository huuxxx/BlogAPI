﻿namespace BlogAPI.DTO
{
    public class BlogDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int Requests { get; set; }
        public string DateCreated { get; set; }
        public string DateModified { get; set; }

    }
}
