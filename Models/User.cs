using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using TripManager.Models;

namespace TripManager.Models
{
    public class User : IdentityUser<Guid>
    {
        public string FullName { get; set; } = null!;
        public ICollection<UserTour> UserTours { get; set; } = new List<UserTour>();
    }
}
