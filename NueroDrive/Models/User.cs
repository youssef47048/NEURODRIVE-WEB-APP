using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace NueroDrive.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string Name { get; set; }

        // For vehicle management
        public List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }

    public class Vehicle
    {
        public int Id { get; set; }
        
        [Required]
        public string CarId { get; set; } // Unique identifier for the car
        
        [Required]
        public string Name { get; set; } // Car name/model
        
        public string? Description { get; set; }
        
        // UserId is set by the controller, not from form input
        public string UserId { get; set; }
        
        [ForeignKey("UserId")]
        public User User { get; set; }
        
        // For authorized drivers
        public List<AuthorizedDriver> AuthorizedDrivers { get; set; } = new List<AuthorizedDriver>();
    }

    public class AuthorizedDriver
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string FaceImageBase64 { get; set; } // Base64 encoded face image
        
        public DateTime DateAdded { get; set; } = DateTime.Now;
        
        public int VehicleId { get; set; }
        public Vehicle Vehicle { get; set; }
    }

    public enum SubscriptionType
    {
        Monthly,
        Annual
    }

    public class Payment
    {
        public int Id { get; set; }
        
        [Required]
        public decimal Amount { get; set; }
        
        [Required]
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        
        [Required]
        public SubscriptionType SubscriptionType { get; set; }
        
        public string UserId { get; set; }
        
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
} 