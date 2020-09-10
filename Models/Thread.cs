using System;

namespace Forum.Models
{
    public class Thread 
    {
        public int ID { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
