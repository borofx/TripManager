using System;
using System.Collections.Generic;
using TripManager.Models;

namespace TripManager.Models
{
    public class Tour
    {
        public Guid TourId { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        // Collection of UserTour relationships (many-to-many)
        public ICollection<UserTour> UserTours { get; set; } = new List<UserTour>();

        // Collection of landmarks associated with this tour
        public ICollection<Landmark> Landmarks { get; set; } = new List<Landmark>();
    }
}
