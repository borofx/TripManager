using System;
using TripManager.Models;

namespace TripManager.Models
{
    public class UserTour
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid TourId { get; set; }
        public Tour Tour { get; set; } = null!;
    }
}
