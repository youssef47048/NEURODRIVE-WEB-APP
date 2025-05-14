using System.ComponentModel.DataAnnotations;
using NueroDrive.Models;

namespace NueroDrive.Models.ViewModels
{
    public class SubscriptionViewModel
    {
        public bool HasActiveSubscription { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public SubscriptionType? CurrentSubscriptionType { get; set; }
        public List<Payment> RecentPayments { get; set; } = new List<Payment>();
    }
    
    public class PaymentViewModel
    {
        [Required]
        public SubscriptionType SubscriptionType { get; set; }
        
        // Payment information would typically go here in a real application
        // For this demo, we'll simulate successful payments
        
        // Credit card details (in a real app, you would use a payment processor)
        [Required(ErrorMessage = "Card number is required")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Card number must be 16 digits")]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }
        
        [Required(ErrorMessage = "Expiration date is required")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$", ErrorMessage = "Expiration date must be in MM/YY format")]
        [Display(Name = "Expiration Date (MM/YY)")]
        public string ExpirationDate { get; set; }
        
        [Required(ErrorMessage = "CVV is required")]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV must be 3 or 4 digits")]
        [Display(Name = "CVV")]
        public string CVV { get; set; }
        
        [Required(ErrorMessage = "Name on card is required")]
        [Display(Name = "Name on Card")]
        public string NameOnCard { get; set; }
    }
} 