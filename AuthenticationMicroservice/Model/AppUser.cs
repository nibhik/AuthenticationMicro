using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationMicroservice.Model
{
    public class AppUser : IdentityUser 
    {
        public int FullName { get; set; }
        public int Address { get; set; }
    }
}
