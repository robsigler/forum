using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Models
{
    public class Post
    {
        public int ID { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AuthorId { get; set; }
        public string ParentId { get; set; }
        public virtual ApplicationUser Author { get; set; }
    }
}