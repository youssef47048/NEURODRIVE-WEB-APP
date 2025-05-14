using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NueroDrive.Data;
using NueroDrive.Models.ViewModels;
using NueroDrive.Services;

namespace NueroDrive.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FaceRecognitionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly FaceRecognitionService _faceRecognitionService;
        private readonly EmailService _emailService;
        private readonly ILogger<FaceRecognitionController> _logger;

        public FaceRecognitionController(
            ApplicationDbContext context,
            FaceRecognitionService faceRecognitionService,
            EmailService emailService,
            ILogger<FaceRecognitionController> logger)
        {
            _context = context;
            _faceRecognitionService = faceRecognitionService;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] FaceRecognitionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Find the vehicle by CarId
                var vehicle = await _context.Vehicles
                    .Include(v => v.User)
                    .Include(v => v.AuthorizedDrivers)
                    .FirstOrDefaultAsync(v => v.CarId == model.CarId);

                if (vehicle == null)
                {
                    _logger.LogWarning($"Authentication attempt for unknown vehicle: {model.CarId}");
                    return NotFound(new { message = "Vehicle not found" });
                }

                var userEmail = vehicle.User.Email;
                var isAuthorized = false;
                string matchedDriverName = null;

                // Compare the provided image with each authorized driver's image
                foreach (var driver in vehicle.AuthorizedDrivers)
                {
                    var result = await _faceRecognitionService.CompareImagesAsync(model.ImageBase64, driver.FaceImageBase64);
                    
                    if (result)
                    {
                        _logger.LogInformation($"Successful authentication for vehicle {model.CarId}, driver: {driver.Name}");
                        isAuthorized = true;
                        matchedDriverName = driver.Name;
                        break;
                    }
                }

                if (!isAuthorized)
                {
                    _logger.LogWarning($"Unauthorized access attempt for vehicle {model.CarId}");
                    
                    // Send email notification for unauthorized access
                    await _emailService.SendUnauthorizedAccessNotificationAsync(
                        userEmail, 
                        vehicle.Name, 
                        DateTime.Now,
                        model.ImageBase64);
                    
                    return Unauthorized(new { 
                        message = "Face not recognized",
                        vehicleName = vehicle.Name
                    });
                }

                return Ok(new { 
                    message = "Authentication successful",
                    driverName = matchedDriverName,
                    vehicleName = vehicle.Name
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during face authentication");
                return StatusCode(500, new { message = "An error occurred during authentication" });
            }
        }
    }
} 