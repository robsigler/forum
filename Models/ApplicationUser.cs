using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;


namespace Forum.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Thread> AuthoredThreads { get; set; }
    }
}