# NueroDrive - Secure Vehicle Authentication System

[![Live Demo](https://img.shields.io/badge/Live_Demo-Visit_Site-blue?style=for-the-badge&logo=microsoft-edge)](https://neurodrive.runasp.net/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/apps/aspnet)
[![C#](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=flat-square&logo=microsoft-sql-server&logoColor=white)](https://www.microsoft.com/en-us/sql-server)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5-7952B3?style=flat-square&logo=bootstrap&logoColor=white)](https://getbootstrap.com/)
[![Entity Framework](https://img.shields.io/badge/Entity_Framework-512BD4?style=flat-square&logo=dotnet)](https://docs.microsoft.com/en-us/ef/)

NueroDrive is a facial recognition-based driver authentication system that enhances vehicle security by ensuring only authorized drivers can access vehicles. The system provides real-time notifications for unauthorized access attempts and a comprehensive management interface for vehicle owners.

## 🌐 Live Demo

The application is deployed and accessible at: [https://neurodrive.runasp.net/](https://neurodrive.runasp.net/)

## Key Features

- **Secure Authentication**: Face recognition technology to verify driver identity
- **Vehicle Management**: Register and manage multiple vehicles under one account
- **Driver Authorization**: Add multiple authorized drivers for each vehicle
- **Real-time Notifications**: Email alerts for unauthorized access attempts
- **Responsive Design**: Mobile-friendly interface for management on any device

## Technology Stack

- **Backend**: ASP.NET Core 8.0 with MVC architecture
- **Database**: Entity Framework Core with SQL Server
- **Frontend**: Bootstrap 5, HTML5, CSS3, JavaScript
- **Authentication**: ASP.NET Core Identity
- **APIs**: RESTful API endpoints for external integration
- **Services**: Email notification service, Face recognition service

## Prerequisites

- .NET 8.0 SDK or later
- SQL Server (LocalDB, Express, or full version)
- Visual Studio 2022 or other compatible IDE
- Git for version control

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/youssef47048/NueroDrive.git
cd NueroDrive
```

### 2. Configure the Application

Create an `appsettings.Development.json` file with the following structure:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=NueroDrive;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "DataProtection": {
    "KeyPath": "DataProtectionKeys"
  },
  "FaceRecognitionAPI": {
    "Url": "https://your-face-recognition-api-url.com/verify"
  },
  "EmailSettings": {
    "SmtpHost": "smtp.example.com",
    "SmtpPort": "587",
    "SmtpUsername": "your-email@example.com",
    "SmtpPassword": "your-email-password",
    "SenderEmail": "your-email@example.com"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

> **IMPORTANT**: Never commit files containing sensitive information like passwords to version control!

### 3. Set Up the Database

```bash
dotnet ef database update
```

### 4. Run the Application

```bash
dotnet run
```

The application will be available at `https://localhost:5001` and `http://localhost:5000`.

## API Documentation

### Face Recognition Authentication

**Endpoint**: `POST /api/FaceRecognition/authenticate`

**Request Body**:
```json
{
  "carId": "ABC123",
  "imageBase64": "base64-encoded-image-data"
}
```

**Success Response** (200 OK):
```json
{
  "message": "Authentication successful",
  "driverName": "John Doe",
  "vehicleName": "Tesla Model 3"
}
```

**Unauthorized Response** (401 Unauthorized):
```json
{
  "message": "Face not recognized",
  "vehicleName": "Tesla Model 3"
}
```

## Email Configuration

### Using Gmail

1. Enable 2-Step Verification on your Google account
2. Generate an App Password:
   - Go to your Google Account → Security
   - Under "Signing in to Google," select App passwords
   - Generate a new app password for "Mail" and "Other (Custom name)"
   - Use this password in your configuration

### Email Troubleshooting

If emails aren't being sent:
1. Check application logs for specific error messages
2. Verify SMTP settings are correct
3. Ensure your email provider allows SMTP access
4. For Gmail, ensure you're using an App Password if 2FA is enabled

## Security Best Practices

1. **Environment Variables**: In production, use environment variables or secret management services instead of configuration files
2. **HTTPS**: Always use HTTPS in production
3. **Regular Updates**: Keep dependencies updated to patch security vulnerabilities
4. **Secure Password Storage**: The system uses ASP.NET Core Identity for secure password hashing

## Deployment

### Preparing for Production

1. Update `appsettings.json` with production settings
2. Configure a production database connection
3. Set up proper logging
4. Enable HTTPS with valid SSL certificates

### Docker Deployment

```bash
docker build -t nuerodrive .
docker run -p 80:80 -p 443:443 nuerodrive
```

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Face recognition technology powered by [Face Recognition API]
- Email notifications using SMTP
- Bootstrap for responsive UI design 