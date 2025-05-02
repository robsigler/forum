using System;

namespace Forum.Models
{
    public class Post
    {
        public int ID { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string AuthorId { get; set; }
        public int ParentId { get; set; }
        public virtual ApplicationUser Author { get; set; }
    }
}