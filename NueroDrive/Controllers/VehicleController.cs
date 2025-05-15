using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NueroDrive.Data;
using NueroDrive.Models;
using NueroDrive.Models.ViewModels;
using NueroDrive.Services;
using System.Security.Claims;

namespace NueroDrive.Controllers
{
    [Authorize]
    public class VehicleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VehicleController> _logger;
        private readonly FaceRecognitionService _faceRecognitionService;
        private readonly EmailService _emailService;

        public VehicleController(
            ApplicationDbContext context, 
            ILogger<VehicleController> logger,
            FaceRecognitionService faceRecognitionService,
            EmailService emailService)
        {
            _context = context;
            _logger = logger;
            _faceRecognitionService = faceRecognitionService;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vehicles = await _context.Vehicles
                .Where(v => v.UserId == userId)
                .ToListAsync();
            
            return View(vehicles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Vehicle vehicle)
        {
            try
            {
                _logger.LogInformation("Create vehicle request received for CarId: {CarId}", vehicle.CarId);
                
                // Remove any errors related to UserId since we set it ourselves
                if (ModelState.ContainsKey("UserId"))
                {
                    ModelState.Remove("UserId");
                }
                
                if (ModelState.ContainsKey("User"))
                {
                    ModelState.Remove("User");
                }

                if (ModelState.IsValid)
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    _logger.LogInformation("User ID: {UserId}", userId);
                    
                    // Check if CarId is already used
                    var existingVehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.CarId == vehicle.CarId);
                    if (existingVehicle != null)
                    {
                        _logger.LogWarning("Car ID {CarId} is already registered", vehicle.CarId);
                        ModelState.AddModelError("CarId", "This Car ID is already registered.");
                        return View(vehicle);
                    }
                    
                    vehicle.UserId = userId;
                    
                    _context.Add(vehicle);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Vehicle created successfully with ID: {Id}", vehicle.Id);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogWarning("Model state is invalid: {Errors}", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                    return View(vehicle);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating vehicle: {Message}", ex.Message);
                ModelState.AddModelError(string.Empty, "An error occurred while creating the vehicle. Please try again.");
                return View(vehicle);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vehicle = await _context.Vehicles
                .Include(v => v.AuthorizedDrivers)
                .FirstOrDefaultAsync(v => v.Id == id && v.UserId == userId);
                
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        [HttpGet]
        public async Task<IActionResult> AddDriver(int vehicleId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == vehicleId && v.UserId == userId);
                
            if (vehicle == null)
            {
                return NotFound();
            }

            var viewModel = new AddDriverViewModel
            {
                VehicleId = vehicleId
            };
            
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddDriver(AddDriverViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var vehicle = await _context.Vehicles
                    .FirstOrDefaultAsync(v => v.Id == model.VehicleId && v.UserId == userId);
                    
                if (vehicle == null)
                {
                    return NotFound();
                }

                var driver = new AuthorizedDriver
                {
                    Name = model.DriverName,
                    FaceImageBase64 = model.FaceImageBase64,
                    VehicleId = model.VehicleId
                };

                _context.AuthorizedDrivers.Add(driver);
                await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(Details), new { id = model.VehicleId });
            }
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveDriver(int driverId, int vehicleId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == vehicleId && v.UserId == userId);
                
            if (vehicle == null)
            {
                return NotFound();
            }

            var driver = await _context.AuthorizedDrivers
                .FirstOrDefaultAsync(d => d.Id == driverId && d.VehicleId == vehicleId);
                
            if (driver == null)
            {
                return NotFound();
            }

            _context.AuthorizedDrivers.Remove(driver);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Details), new { id = vehicleId });
        }
        
        // GET: Vehicle/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == id && v.UserId == userId);
                
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }
        
        // POST: Vehicle/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vehicle = await _context.Vehicles
                .Include(v => v.AuthorizedDrivers)
                .FirstOrDefaultAsync(v => v.Id == id && v.UserId == userId);
                
            if (vehicle == null)
            {
                return NotFound();
            }

            try
            {
                _logger.LogInformation("Deleting vehicle {Id} ({Name}) and all associated drivers", vehicle.Id, vehicle.Name);
                
                // Entity Framework will automatically delete related AuthorizedDrivers due to cascade delete
                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Vehicle {Id} deleted successfully", id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting vehicle {Id}", id);
                ModelState.AddModelError(string.Empty, "An error occurred while deleting the vehicle.");
                return View(vehicle);
            }
        }

        [HttpGet]
        [Route("api/vehicle/{carId}/drivers")]
        public async Task<IActionResult> GetVehicleDrivers(string carId)
        {
            try
            {
                _logger.LogInformation("Getting authorized drivers for Car ID: {CarId}", carId);
                
                var vehicle = await _context.Vehicles
                    .Include(v => v.AuthorizedDrivers)
                    .FirstOrDefaultAsync(v => v.CarId == carId);
                    
                if (vehicle == null)
                {
                    _logger.LogWarning("Vehicle with Car ID {CarId} not found", carId);
                    return NotFound(new { success = false, message = "Vehicle not found" });
                }
                
                var drivers = vehicle.AuthorizedDrivers.Select(d => new {
                    id = d.Id,
                    name = d.Name,
                    dateAdded = d.DateAdded
                }).ToList();
                
                return Ok(new { 
                    success = true, 
                    vehicleId = vehicle.Id,
                    vehicleName = vehicle.Name,
                    drivers = drivers 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving drivers for Car ID {CarId}", carId);
                return StatusCode(500, new { success = false, message = "Failed to retrieve drivers" });
            }
        }

        [HttpPost]
        [Route("api/vehicle/verifyface")]
        public async Task<IActionResult> VerifyFaceApi([FromBody] FaceVerificationRequest request)
        {
            try
            {
                _logger.LogInformation("Face verification request received for driver ID: {DriverId}", request.DriverId);
                
                var driver = await _context.AuthorizedDrivers.FindAsync(request.DriverId);
                if (driver == null)
                {
                    _logger.LogWarning("Driver with ID {DriverId} not found", request.DriverId);
                    return NotFound(new { success = false, message = "Driver not found" });
                }

                // Use the FaceRecognitionService to compare images
                bool isMatch = await _faceRecognitionService.CompareImagesAsync(
                    driver.FaceImageBase64, 
                    request.ImageBase64);
                
                // If no match found, send email notification
                if (!isMatch)
                {
                    try
                    {
                        // Get the vehicle and its owner
                        var vehicle = await _context.Vehicles
                            .Include(v => v.User)
                            .FirstOrDefaultAsync(v => v.Id == driver.VehicleId);
                            
                        if (vehicle?.User?.Email != null)
                        {
                            await _emailService.SendUnauthorizedAccessNotificationAsync(
                                vehicle.User.Email,
                                vehicle.Name,
                                DateTime.Now,
                                request.ImageBase64);
                                
                            _logger.LogInformation("Unauthorized access email sent to {Email} for driver ID {DriverId}", 
                                vehicle.User.Email, request.DriverId);
                        }
                        else
                        {
                            _logger.LogWarning("Could not send unauthorized access email, vehicle or owner email not found");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send unauthorized access email notification");
                    }
                }

                return Ok(new { 
                    isMatch = isMatch,
                    driverName = driver.Name,
                    success = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying face");
                return StatusCode(500, new { success = false, message = "Face verification failed" });
            }
        }
    }

    public class FaceVerificationRequest
    {
        public int DriverId { get; set; }
        public string ImageBase64 { get; set; }
    }
} 