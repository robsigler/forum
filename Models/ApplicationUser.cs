using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;


namespace Forum.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime JoinDate { get; set; }
        public virtual ICollection<Thread> AuthoredThreads { get; set; }
    }
}