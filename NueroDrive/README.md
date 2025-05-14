# NueroDrive - Face Recognition Authentication System

NueroDrive is a face recognition-based driver authentication system developed for advanced driver assistance services. The system allows vehicle owners to register authorized drivers through face recognition, ensuring only authorized individuals can access their vehicles.

## Features

- User registration and authentication
- Vehicle management
- Authorized driver management with face recognition
- API endpoint for facial recognition authentication
- Email notifications for unauthorized access attempts

## Technology Stack

- ASP.NET Core MVC 8.0
- Entity Framework Core with SQL Server
- Bootstrap 5 for UI
- Face Recognition API integration

## Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB is sufficient for development)
- Visual Studio 2022 or later (recommended)

## Setup Instructions

1. Clone the repository
2. Update the connection string in `appsettings.json` if needed
3. Set up the database:
    ```
    dotnet ef database update
    ```
4. Update the Face Recognition API URL in `appsettings.json`
5. Update the email settings in `appsettings.json` for notifications
6. Run the application:
    ```
    dotnet run
    ```

## API Endpoints

### Face Recognition Authentication

- **URL**: `/api/FaceRecognition/authenticate`
- **Method**: `POST`
- **Request Body**:
  ```json
  {
    "carId": "CAR123",
    "imageBase64": "base64-encoded-image-content"
  }
  ```
- **Success Response**:
  ```json
  {
    "message": "Authentication successful"
  }
  ```
- **Error Response**:
  ```json
  {
    "message": "Face not recognized"
  }
  ```

## Configuring Email Notifications


## How Email Notifications Work

The system sends an email notification to the vehicle owner whenever an unauthorized person attempts to access a vehicle. This provides:

1. Real-time security alerts
2. Records of access attempts
3. Immediate awareness of potential security breaches

## Setup Instructions

### 1. SMTP Configuration

Update the `appsettings.json` file with your SMTP server details:

```json
"EmailSettings": {
  "SmtpHost": "smtp.gmail.com",  // Your SMTP server address
  "SmtpPort": "587",             // SMTP port (typically 587 for TLS)
  "SmtpUsername": "your-email@gmail.com",  // Your email address
  "SmtpPassword": "your-app-password",     // Your password or app password
  "SenderEmail": "your-email@gmail.com"    // The "From" email address
}
```

### 2. Using Gmail

If you're using Gmail as your SMTP provider:

1. Go to your Google Account → Security
2. Enable 2-Step Verification if not already enabled
3. Go to "App passwords" under 2-Step Verification
4. Select "Mail" and "Other" (Custom name: "NueroDrive")
5. Copy the generated 16-character app password
6. Paste this password in the `SmtpPassword` field in `appsettings.json`

### 3. Other Email Providers

For other email providers:
- Find their SMTP server details and port
- Use your email address and password (or app password if required)
- Update the configuration accordingly

## Testing the Email Notifications

You can test email notifications using the following methods:

### 1. API Testing with Postman

#### Test unauthorized access with `/api/verify-driver`:

```json
POST https://your-domain.com/api/verify-driver
Content-Type: application/json

{
  "carId": "YOUR_CAR_ID",
  "imageBase64": "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD..."
}
```

Use a face image that doesn't match any authorized driver.

#### Test unauthorized access with `/api/vehicle/verifyface`:

```json
POST https://your-domain.com/api/vehicle/verifyface
Content-Type: application/json

{
  "driverId": 1,
  "imageBase64": "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQAAAQABAAD..."
}
```

Use a face image that doesn't match the specified driver.

### 2. Troubleshooting

If emails aren't being sent:

1. Check application logs for specific error messages
2. Verify your SMTP settings are correct
3. Ensure your email provider allows SMTP access
4. Check if your app password is valid and correctly entered
5. Try different port settings if connection issues persist

## Security Considerations

- **Never commit `appsettings.json` with real credentials to source control**
- Use environment variables or secure configuration storage in production
- Rotate app passwords periodically for better security
- Use TLS/SSL for secure SMTP connections

## Additional Customization

You can customize the email template by modifying the `SendUnauthorizedAccessNotificationAsync` method in `EmailService.cs`.

## Support

If you encounter issues with the email notification system, check the application logs for detailed error information. 

## Docker Support

The project includes Docker support. To build and run the Docker container:

```
docker build -t nuerodrive .
docker run -p 8080:80 nuerodrive
```

## Integration with QT GUI Application

The QT GUI application should send a POST request to the `/api/FaceRecognition/authenticate` endpoint with:

1. Car ID - A unique identifier for the vehicle
2. Image - A base64-encoded image of the driver's face

The API will respond with a success message if the driver is authorized, or an error message if not authorized. 