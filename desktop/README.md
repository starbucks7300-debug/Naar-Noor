# Naar-Noor Desktop Application

A native Windows desktop client built with .NET 8+ and Windows Forms, providing restaurant staff and managers with comprehensive management capabilities for the Naar-Noor restaurant system.

## Project Structure

```
desktop/
├── src/
│   ├── NaarNoor.Desktop.Common/
│   │   ├── Models/              - Domain models and entities
│   │   ├── Services/            - Service interfaces and implementations
│   │   ├── DTOs/                - Data transfer objects
│   │   ├── Constants/           - Application constants and configuration
│   │   └── Utilities/           - Utility classes and helpers
│   │
│   ├── NaarNoor.Desktop.WinForms/
│   │   ├── Forms/               - Windows Forms UI components
│   │   ├── ViewModels/          - MVVM ViewModels (CommunityToolkit.Mvvm)
│   │   ├── Services/            - UI-specific service implementations
│   │   ├── Resources/           - Localization resources (en.xaml, ar.xaml)
│   │   ├── Configuration/       - Dependency injection and app configuration
│   │   └── Program.cs           - Application entry point
│   │
│   └── NaarNoor.Desktop.Tests/
│       ├── Services/            - Unit tests for services
│       ├── ViewModels/          - Unit tests for ViewModels
│       ├── Mocks/               - Mock implementations and test helpers
│       └── UnitTest1.cs         - Test fixture template
│
├── NaarNoor.Desktop.sln         - Solution file
├── appsettings.json             - Default configuration
├── appsettings.Development.json - Development-specific configuration
├── .gitignore                   - Git ignore rules
└── README.md                    - This file
```

## Prerequisites

- .NET 8 SDK or later
- Windows 10/11
- Visual Studio 2022 or Visual Studio Code

## Installation

### 1. Clone the Repository

```bash
cd desktop
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Build the Solution

```bash
dotnet build
```

### 4. Run Tests

```bash
dotnet test
```

### 5. Run the Application

```bash
cd src/NaarNoor.Desktop.WinForms
dotnet run
```

## Configuration

### API Configuration

Update `appsettings.json` to configure the backend API endpoint:

```json
{
  "Api": {
    "BaseUrl": "http://localhost:5000",
    "TimeoutSeconds": 30
  }
}
```

### Development Configuration

For development, use `appsettings.Development.json`:

```json
{
  "Api": {
    "BaseUrl": "http://localhost:5000",
    "TimeoutSeconds": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

### Localization

The application supports English (en) and Arabic (ar) localization. Set the culture in `appsettings.json`:

```json
{
  "Culture": "en"
}
```

## Architecture

The application follows MVVM (Model-View-ViewModel) architecture with the following layers:

1. **Presentation Layer (WinForms)**: Forms-based UI with Windows Forms components
2. **MVVM Layer**: ViewModels using Microsoft.Toolkit.Mvvm for data binding and commands
3. **Service Layer**: Business logic and API orchestration
4. **API Client Layer (Refit)**: Type-safe HTTP client for backend integration
5. **Data Access & Caching**: SQLite-based caching with offline capability

## Key Technologies

- **.NET 8**: Latest .NET framework
- **Windows Forms**: Initial UI implementation (WPF evolution planned)
- **CommunityToolkit.Mvvm**: MVVM patterns and utilities
- **Refit**: Type-safe HTTP client library
- **Polly**: Resilience policies (retry, circuit breaker)
- **System.Data.SQLite**: Local data persistence and caching
- **xUnit**: Unit testing framework
- **Moq**: Mocking library for testing

## Features

### Authentication & Security
- JWT-based authentication with refresh token support
- Secure token storage using Windows DPAPI
- Role-based access control (RBAC)
- Audit logging of security events
- TLS 1.3 encryption for all API communication

### Dashboard
- Real-time dashboard with key metrics
- Reservation overview
- Current orders display
- Staff status indicators
- Revenue summary (manager only)
- Role-based widget customization

### Reservation Management
- View, create, update, and delete reservations
- Date-based filtering and search
- Reservation conflict prevention
- Offline reservation queuing

### Menu Management
- View, create, update, and delete menu items
- Bilingual menu support (English/Arabic)
- Category filtering
- Price and availability management

### Staff Management
- View staff members and roles
- Status management (available, busy, break)
- Role-based filtering
- Scheduled hours tracking

### Reports & Analytics
- Revenue reports
- Order statistics
- Reservation trends
- Date range filtering
- CSV export capability

### Offline Support
- Automatic offline detection
- Pending operations queuing
- Automatic sync on reconnection
- Last-write-wins conflict resolution

### Localization
- Full bilingual support (English/Arabic)
- Runtime culture switching
- RTL layout support for Arabic
- Persistent language preference

## Testing

The project uses xUnit for unit testing and property-based testing with fast-check for correctness verification.

### Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true

# Run specific test project
dotnet test src/NaarNoor.Desktop.Tests
```

### Test Structure

- **Unit Tests**: Test individual components in isolation
- **Integration Tests**: Test service integration with API clients
- **Property-Based Tests**: Verify correctness properties across random inputs

## Security

The application implements enterprise-grade security:

- **Input Validation**: All user inputs validated per OWASP guidelines
- **Injection Prevention**: Parameterized queries, no SQL/XSS injection
- **Secure Token Storage**: DPAPI encryption of authentication tokens
- **TLS 1.3 Enforcement**: All network communication encrypted
- **Certificate Pinning**: Production API endpoint certificate validation
- **Audit Logging**: Security events logged to SQLite database
- **Request Signing**: State-changing operations signed with HMAC

## Development Workflow

### Build

```bash
dotnet build
```

### Format Code

```bash
dotnet format
```

### Run Tests

```bash
dotnet test
```

### Run Application

```bash
cd src/NaarNoor.Desktop.WinForms
dotnet run
```

## Troubleshooting

### Connection Issues

If the application cannot connect to the API:

1. Verify the API base URL in `appsettings.json`
2. Ensure the backend API is running on the configured port
3. Check network connectivity
4. Verify firewall settings allow the connection

### Missing Dependencies

```bash
# Clean and restore
dotnet clean
dotnet restore
```

### Build Errors

```bash
# Rebuild solution
dotnet clean
dotnet build
```

## Contributing

Please refer to the main repository contribution guidelines.

## License

This project is part of the Naar-Noor system. Please refer to the repository license.

## Support

For issues and questions, please refer to the project's issue tracker or documentation.
