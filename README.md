# PetStore - Lost & Found Pet Management System

A comprehensive web application for managing lost and found pet reports with user authentication, content moderation, and messaging system.

## 🌟 Features

### For Users
- **Pet Report Management**: Create, edit, and manage lost/found pet reports
- **Real-time Messaging**: Chat with other users about pet reports
- **Search & Filter**: Find pets by location, species, and other criteria
- **User Profiles**: Manage personal information and view report history
- **Status Tracking**: Monitor the status of your reports (Pending, Approved, Rejected, etc.)

### For Administrators
- **Dashboard**: Overview of system statistics and recent activities
- **User Management**: View and manage user accounts
- **Content Moderation**: Approve or reject pet reports with reasons
- **Report Management**: Monitor and manage all pet reports
- **System Statistics**: View detailed analytics and reports
- **System Settings**: Configure application settings

## 🚀 Technology Stack

- **Backend**: ASP.NET Core 8.0 MVC
- **Database**: SQL Server with Entity Framework Core
- **Frontend**: Bootstrap 5, jQuery, Font Awesome
- **Authentication**: ASP.NET Core Identity
- **Architecture**: Repository Pattern with DAO layer

## 📋 Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB or full SQL Server)
- Visual Studio 2022 or VS Code

## 🛠️ Installation

### 1. Clone the Repository
```bash
git clone <repository-url>
cd PetStore
```

### 2. Database Setup
1. Update the connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PetStore;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

2. Run Entity Framework migrations:
```bash
dotnet ef database update
```

### 3. Build and Run
```bash
dotnet restore
dotnet build
dotnet run
```

The application will be available at `https://localhost:5001` or `http://localhost:5000`

## 👥 Default Users

The application comes with pre-configured users:

### Admin Account
- **Email**: admin@petstore.com
- **Password**: Admin@123
- **Role**: Admin

### User Account
- **Email**: user@petstore.com
- **Password**: User@123
- **Role**: User

## 📖 User Guide

### Getting Started

1. **Register/Login**: Create a new account or login with existing credentials
2. **Browse Reports**: View recent pet reports on the homepage
3. **Create Report**: Click "Create Report" to submit a new lost/found pet report

### Creating a Pet Report

1. Navigate to "My Reports" → "Create New Report"
2. Fill in the required information:
   - **Type**: Lost or Found
   - **Species**: Dog, Cat, etc.
   - **Title**: Brief description
   - **Description**: Detailed information
   - **Location**: District and City
   - **Contact Information**: Name, phone, email
   - **Image**: Upload a photo of the pet
3. Submit the report
4. **Note**: New reports require admin approval before being publicly visible

### Messaging System

1. **View Conversations**: Go to "Messages" to see all conversations
2. **Start Chat**: Click on a conversation or use "Contact" button on pet reports
3. **Send Messages**: Type your message and click "Send"
4. **Read Status**: Messages show read/unread status

### For Administrators

#### Dashboard
- View system overview and statistics
- Monitor recent activities
- Quick access to pending reports

#### Content Moderation
1. Go to "Admin" → "Content Moderation"
2. View pending reports
3. Click "Approve" or "Reject" with optional reason
4. Reports are automatically updated with new status

#### User Management
1. Go to "Admin" → "User Management"
2. View all users and their roles
3. Manage user accounts and permissions

## 🔧 Configuration

### App Settings
Key configuration options in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "your-connection-string"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Database Configuration
- The application uses Entity Framework Core with SQL Server
- Migrations are included in the `Migrations/` folder
- Seed data includes default users and sample reports

## 📁 Project Structure

```
PetStore/
├── Controllers/          # MVC Controllers
├── DAO/                 # Data Access Objects
│   ├── Interfaces/      # DAO interfaces
│   └── *.cs            # DAO implementations
├── Data/               # Database context and seed data
├── Models/             # Entity models and DTOs
│   ├── DTOs/          # Data Transfer Objects
│   ├── Entities/      # Database entities
│   └── Enums/         # Enumeration types
├── Views/              # Razor views
├── wwwroot/           # Static files (CSS, JS, images)
└── Program.cs         # Application entry point
```

## 🔐 Security Features

- **Role-based Authorization**: Admin and User roles
- **Content Moderation**: All reports require admin approval
- **Input Validation**: Server-side and client-side validation
- **CSRF Protection**: Anti-forgery tokens on forms
- **Secure Authentication**: ASP.NET Core Identity

## 🐛 Troubleshooting

### Common Issues

1. **Database Connection Error**
   - Verify connection string in `appsettings.json`
   - Ensure SQL Server is running
   - Run `dotnet ef database update`

2. **Build Errors**
   - Run `dotnet restore` to restore packages
   - Check .NET version compatibility
   - Clear obj/ and bin/ folders

3. **Migration Issues**
   - Delete existing database if needed
   - Run `dotnet ef database drop`
   - Run `dotnet ef database update`

### Logs
Check application logs for detailed error information:
- Development: Console output
- Production: Configure logging in `appsettings.json`

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 📞 Support

For support and questions:
- Create an issue in the repository
- Contact the development team
- Check the documentation

## 🔄 Version History

- **v1.0.0**: Initial release with basic pet report functionality
- **v1.1.0**: Added messaging system and content moderation
- **v1.2.0**: Enhanced admin dashboard and user management

---

**Note**: This application is designed for educational purposes and can be extended for production use with additional security measures and features. 