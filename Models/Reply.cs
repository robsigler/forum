using System;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Forum.Models
{
    public class Reply
    {
        public int ID { get; set; }
        public string Body { get; set; }
        public DateTime CreatedDate { get; set; }
        public string AuthorId { get; set; }
        public virtual ApplicationUser Author { get; set; }
        public int ThreadId { get; set; }
        public virtual Thread Thread { get; set; }
    }
}