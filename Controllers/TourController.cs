using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TripManager.Data;
using TripManager.Models;
using Microsoft.EntityFrameworkCore;

namespace TripManager.Controllers
{
    [Authorize] // Ensure user is logged in to access the controller
    public class TourController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<TourController> _logger;

        public TourController(ApplicationDbContext context, UserManager<User> userManager, ILogger<TourController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        // **Admin-only**: View all landmarks for adding to tours (Create/Manage)
        [Authorize(Roles = "Admin")]
        public IActionResult ManageLandmarks()
        {
            var landmarks = _context.Landmarks.ToList();
            return View(landmarks); // Return a view with all landmarks
        }

        // **Admin-only**: Add a new landmark (Create)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AddLandmark()
        {
            return View(); // Return a view to create a new landmark
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddLandmark(Landmark landmark)
        {
            if (ModelState.IsValid)
            {
                _context.Landmarks.Add(landmark);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageLandmarks));
            }
            return View(landmark);
        }

        // **Admin-only**: Delete a landmark (Delete)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteLandmark(int id)
        {
            var landmark = await _context.Landmarks.FindAsync(id);
            if (landmark != null)
            {
                _context.Landmarks.Remove(landmark);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageLandmarks));
        }

        // **Admin-only**: Edit a landmark (Edit)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> EditLandmark(int id)
        {
            var landmark = await _context.Landmarks.FindAsync(id);
            if (landmark == null) return NotFound();
            return View(landmark);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> EditLandmark(Landmark landmark)
        {
            if (ModelState.IsValid)
            {
                _context.Landmarks.Update(landmark);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageLandmarks));
            }
            return View(landmark);
        }

        // **User-only**: View user's tours
        public async Task<IActionResult> MyTours()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var tours = await _context.Tours
                .Where(t => t.UserId == userId)
                .Include(t => t.TourLandmarks)
                    .ThenInclude(tl => tl.Landmark)
                .ToListAsync();

            return View(tours);
        }



        // **User-only**: Create a new tour (Plan a trip)
        [HttpGet]
        public async Task<IActionResult> CreateTour()
        {
            var landmarks = await _context.Landmarks.ToListAsync(); // Fetch all landmarks
            ViewBag.Landmarks = landmarks; // Pass landmarks to the view
            return View(); // Return the CreateTour view
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTour(Tour tour, int[] selectedLandmarkIds)
        {
            // Remove the ModelState check for UserId and User
            ModelState.Remove("UserId");
            ModelState.Remove("User");

            if (!ModelState.IsValid)
            {
                var landmarks = await _context.Landmarks.ToListAsync();
                ViewBag.Landmarks = landmarks;
                return View(tour);
            }

            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Create new tour instance
            var newTour = new Tour
            {
                Name = tour.Name,
                UserId = userId,
                TourLandmarks = new List<TourLandmark>()
            };

            if (selectedLandmarkIds != null && selectedLandmarkIds.Length > 0)
            {
                foreach (var landmarkId in selectedLandmarkIds)
                {
                    var landmark = await _context.Landmarks.FindAsync(landmarkId);
                    if (landmark != null)
                    {
                        newTour.TourLandmarks.Add(new TourLandmark
                        {
                            LandmarkId = landmarkId
                        });
                    }
                }
            }

            _context.Tours.Add(newTour);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyTours));
        }

        // **User-only**: Add landmarks to the user's tour
        [HttpGet]
        public async Task<IActionResult> AddLandmarkToTour(int tourId)
        {
            var landmarks = await _context.Landmarks.ToListAsync();
            ViewBag.TourId = tourId;
            return View(landmarks); // Return a view to select landmarks for the tour
        }

        [HttpPost]
        public async Task<IActionResult> AddLandmarkToTour(int tourId, int landmarkId)
        {
            var tour = await _context.Tours
                .Include(t => t.TourLandmarks)
                .FirstOrDefaultAsync(t => t.Id == tourId);

            var landmark = await _context.Landmarks.FindAsync(landmarkId);
            if (tour == null || landmark == null)
            {
                return NotFound();
            }

            // Add the landmark to the tour
            if (!tour.TourLandmarks.Any(tl => tl.LandmarkId == landmarkId))
            {
                tour.TourLandmarks.Add(new TourLandmark { TourId = tourId, LandmarkId = landmarkId });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(MyTours));
        }

        // **User-only**: Remove landmark from the tour
        [HttpPost]
        public async Task<IActionResult> RemoveLandmarkFromTour(int tourId, int landmarkId)
        {
            var tourLandmark = await _context.TourLandmarks
                .FirstOrDefaultAsync(tl => tl.TourId == tourId && tl.LandmarkId == landmarkId);

            if (tourLandmark != null)
            {
                _context.TourLandmarks.Remove(tourLandmark);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(MyTours));
        }
    }
}
