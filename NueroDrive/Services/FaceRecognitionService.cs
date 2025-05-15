using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NueroDrive.Services
{
    public class FaceRecognitionService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FaceRecognitionService> _logger;
        private readonly string? _apiUrl;
        private readonly bool _isEnabled;

        public FaceRecognitionService(HttpClient httpClient, IConfiguration configuration, ILogger<FaceRecognitionService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiUrl = configuration["FaceRecognitionAPI:Url"];
            _isEnabled = !string.IsNullOrEmpty(_apiUrl);
            
            if (!_isEnabled)
            {
                _logger.LogWarning("Face Recognition API is disabled. All verification attempts will return false.");
            }
        }

        public async Task<bool> CompareImagesAsync(string image1Base64, string image2Base64)
        {
            if (!_isEnabled)
            {
                _logger.LogInformation("Face Recognition API is disabled. Returning false for verification attempt.");
                return false;
            }
            
            try
            {
                _logger.LogInformation("Calling face verification API");
                
                // Add required prefix if not already present
                if (!image1Base64.StartsWith("data:image"))
                {
                    image1Base64 = "data:image/jpeg;base64," + image1Base64;
                }
                
                if (!image2Base64.StartsWith("data:image"))
                {
                    image2Base64 = "data:image/jpeg;base64," + image2Base64;
                }
                
                var requestData = new
                {
                    img1_path = image1Base64,
                    img2_path = image2Base64
                };

                var jsonRequest = JsonSerializer.Serialize(requestData);
                _logger.LogInformation("Request payload size: {Size} bytes", jsonRequest.Length);
                // Log the first 100 chars of each image to verify prefix is present
                _logger.LogInformation("Image1 prefix: {Prefix}", image1Base64.Substring(0, Math.Min(100, image1Base64.Length)));
                _logger.LogInformation("Image2 prefix: {Prefix}", image2Base64.Substring(0, Math.Min(100, image2Base64.Length)));
                
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                
                _logger.LogInformation("Sending request to {ApiUrl}", _apiUrl);
                var response = await _httpClient.PostAsync(_apiUrl, content);

                // Log the full response for debugging
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("API Response: {Response}", responseBody);

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonSerializer.Deserialize<VerificationResponse>(responseBody);
                    _logger.LogInformation("Verification result: {Result}", result?.verified);
                    return result?.verified ?? false;
                }
                else
                {
                    _logger.LogError("Face recognition API returned {StatusCode}: {Response}", 
                        response.StatusCode, responseBody);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling face recognition API");
                return false;
            }
        }
    }

    // Updated class to match the actual API response
    internal class VerificationResponse
    {
        [JsonPropertyName("verified")]
        public bool verified { get; set; }
        
        [JsonPropertyName("distance")]
        public double distance { get; set; }
        
        [JsonPropertyName("threshold")]
        public double threshold { get; set; }
        
        [JsonPropertyName("model")]
        public string model { get; set; }
        
        [JsonPropertyName("time")]
        public double time { get; set; }
    }
} 