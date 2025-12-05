using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineLog.Core.Entities
{
    // IdentityUser<int>: ID'si string değil int olsun (1, 2, 3 diye artsın)
    public class AppUser : IdentityUser<int>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        // Bir kullanıcının birden çok filmi olabilir 
        public ICollection<UserMovie> UserMovies { get; set; }
    }
}
