using System;
using System.Collections.Generic;
using TripManager.Models;

namespace TripManager.Models
{
    public class Landmark
    {
        public Guid LandmarkId { get; set; } = Guid.NewGuid(); // Primary key

        public string Name { get; set; }
        public string Location { get; set; } // Geographical coordinates as string or separate latitude/longitude fields
        public string Description { get; set; }

        // Optional: If you want to associate landmarks directly with tours, add this:
        public ICollection<Tour> Tours { get; set; } = new List<Tour>();
    }
}
