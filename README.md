# NueroDrive - Secure Vehicle Authentication System

[![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/apps/aspnet)
[![C#](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=flat-square&logo=microsoft-sql-server&logoColor=white)](https://www.microsoft.com/en-us/sql-server)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5-7952B3?style=flat-square&logo=bootstrap&logoColor=white)](https://getbootstrap.com/)
[![Entity Framework](https://img.shields.io/badge/Entity_Framework-512BD4?style=flat-square&logo=dotnet)](https://docs.microsoft.com/en-us/ef/)

NueroDrive is a facial recognition-based driver authentication system that enhances vehicle security by ensuring only authorized drivers can access vehicles. The system provides real-time notifications for unauthorized access attempts and a comprehensive management interface for vehicle owners.

## 🌐 Live Demo

The application is deployed and accessible at: [https://neurodrive.runasp.net/](https://neurodrive.runasp.net/)

## 🎥 Demo Video

[https://user-images.githubusercontent.com/YOUR_USER_ID/YOUR_REPOSITORY/assets/demo.mp4](https://github.com/user-attachments/assets/27e39ae8-1b92-4534-ad21-c311a613caef)

#### Connecting the GUI application with the Web Application Endpoint that retrieves the Authorized Drivers for the specific car ID and compares them with the image sent from the GUI through an external Face Authentication service.


## Email Preview

Here is a preview of the email that will be sent if unauthorized driver tries to access the vehicle
![Email Preview](email.png)

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

## Project Structure

For detailed documentation, setup instructions, and API documentation, please see the [project README](./NueroDrive/README.md).
