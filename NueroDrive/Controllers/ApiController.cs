using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NueroDrive.Data;
using NueroDrive.Models;
using NueroDrive.Services;

namespace NueroDrive.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ApiController> _logger;
        private readonly FaceRecognitionService _faceRecognitionService;
        private readonly EmailService _emailService;

        public ApiController(
            ApplicationDbContext context, 
            ILogger<ApiController> logger,
            FaceRecognitionService faceRecognitionService,
            EmailService emailService)
        {
            _context = context;
            _logger = logger;
            _faceRecognitionService = faceRecognitionService;
            _emailService = emailService;
        }
        
        [HttpPost]
        [Route("verify-driver")]
        public async Task<IActionResult> VerifyDriver([FromBody] DriverVerificationRequest request)
        {
            try
            {
                _logger.LogInformation("External driver verification request received for Car ID: {CarId}", request.CarId);
                
                // Validate inputs
                if (string.IsNullOrEmpty(request.CarId) || string.IsNullOrEmpty(request.ImageBase64))
                {
                    return BadRequest(new { 
                        success = false, 
                        message = "Car ID and image are required" 
                    });
                }
                
                // Get the vehicle using the car ID
                var vehicle = await _context.Vehicles
                    .Include(v => v.AuthorizedDrivers)
                    .FirstOrDefaultAsync(v => v.CarId == request.CarId);
                    
                if (vehicle == null)
                {
                    _logger.LogWarning("Vehicle with Car ID {CarId} not found during verification", request.CarId);
                    return NotFound(new { 
                        success = false, 
                        message = "Vehicle not found" 
                    });
                }
                
                // If no drivers are registered
                if (!vehicle.AuthorizedDrivers.Any())
                {
                    _logger.LogWarning("No authorized drivers found for Car ID {CarId}", request.CarId);
                    return Ok(new { 
                        success = true,
                        isAuthorized = false,
                        message = "No authorized drivers found for this vehicle" 
                    });
                }
                
                // Check against each driver
                foreach (var driver in vehicle.AuthorizedDrivers)
                {
                    _logger.LogInformation($"Comparing with driver {driver.Name}");
                    _logger.LogInformation($"Driver image size: {driver.FaceImageBase64.Length}");
                    _logger.LogInformation($"Request image size: {request.ImageBase64.Length}");

                    var isMatch = await _faceRecognitionService.CompareImagesAsync(
                        driver.FaceImageBase64, 
                        request.ImageBase64);

                    _logger.LogInformation($"Match result: {isMatch}");
                    
                    // If a match is found
                    if (isMatch)
                    {
                        _logger.LogInformation("Driver match found for {DriverName}", driver.Name);
                            
                        return Ok(new { 
                            success = true,
                            isAuthorized = true,
                            driverName = driver.Name
                        });
                    }
                }
                
                // No match found among any authorized drivers
                _logger.LogInformation("No matching driver found for Car ID {CarId}", request.CarId);
                
                // Send email notification for unauthorized access
                try
                {
                    // Include vehicle owner in query to get their email
                    var vehicleWithOwner = await _context.Vehicles
                        .Include(v => v.User)
                        .FirstOrDefaultAsync(v => v.Id == vehicle.Id);
                        
                    if (vehicleWithOwner?.User?.Email != null)
                    {
                        await _emailService.SendUnauthorizedAccessNotificationAsync(
                            vehicleWithOwner.User.Email,
                            vehicleWithOwner.Name,
                            DateTime.Now,
                            request.ImageBase64);
                        
                        _logger.LogInformation("Unauthorized access email sent to {Email}", vehicleWithOwner.User.Email);
                    }
                    else
                    {
                        _logger.LogWarning("Could not send unauthorized access email, owner email not found");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send unauthorized access email notification");
                }
                
                return Ok(new { 
                    success = true,
                    isAuthorized = false,
                    message = "No authorized driver match found" 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during external driver verification for Car ID {CarId}", request.CarId);
                return StatusCode(500, new { 
                    success = false, 
                    message = "Face verification failed" 
                });
            }
        }
    }
    
    public class DriverVerificationRequest
    {
        public string CarId { get; set; }
        public string ImageBase64 { get; set; }
    }
} 