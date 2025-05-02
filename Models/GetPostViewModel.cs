using System.Collections.Generic;

namespace Forum.Models
{
    public class GetPostViewModel
    {
        public int Id { get; set; }
        public List<Post> Posts { get; set; }
    }
}