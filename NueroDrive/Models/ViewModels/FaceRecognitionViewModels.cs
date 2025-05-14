using System.ComponentModel.DataAnnotations;

namespace NueroDrive.Models.ViewModels
{
    public class FaceRecognitionViewModel
    {
        [Required]
        public string CarId { get; set; }

        [Required]
        public string ImageBase64 { get; set; }
    }
} 