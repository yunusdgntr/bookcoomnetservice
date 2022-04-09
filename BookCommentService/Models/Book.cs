using System;
using System.Collections.Generic;

namespace BookCommentService.Models
{
    public class Book
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Author { get; set; }
        public string PhotoLink { get; set; }
        public List<Comment> Comments { get; set; }
    }
}

