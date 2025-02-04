using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TripManager.Models;

namespace TripManager.Models
{
    public class Tour
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;

        public ICollection<TourLandmark> TourLandmarks { get; set; } = new List<TourLandmark>();
    }
}
