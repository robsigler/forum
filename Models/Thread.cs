using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Models
{
    public class Thread
    {
        public int ID { get; set; }
        public string Subject { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AuthorId { get; set; }
        public virtual ApplicationUser Author { get; set; }
        public virtual ICollection<Post> Posts { get; set; }
    }
}