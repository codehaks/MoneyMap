# MoneyMap - Personal Expense Tracking Application

## Overview

MoneyMap is a web-based personal expense tracking application built with ASP.NET Core 9.0 and Entity Framework Core. It provides users with a comprehensive solution for managing their personal finances, tracking expenses, categorizing spending, and gaining insights into their financial habits.

## Features

- **User Authentication & Authorization**: Secure user registration and login with role-based access control
- **Expense Management**: Add, edit, and delete personal expenses
- **Category Management**: Organize expenses by custom categories
- **Admin Panel**: Administrative interface for user and category management
- **Responsive Design**: Modern UI built with Bootstrap for desktop and mobile devices
- **Database Integration**: PostgreSQL database with Entity Framework Core

## Technology Stack

- **Backend**: ASP.NET Core 9.0
- **Frontend**: Razor Pages with Bootstrap 5
- **Database**: PostgreSQL with Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **UI Framework**: Bootstrap 5
- **Development**: .NET 9.0 SDK

## Project Structure

```
MoneyMap/
├── src/
│   ├── MoneyMap.Web/          # Web application (Razor Pages)
│   │   ├── Areas/
│   │   │   ├── Admin/         # Admin panel pages
│   │   │   ├── Identity/      # Authentication pages
│   │   │   └── Users/         # User expense management
│   │   ├── Pages/             # Main application pages
│   │   ├── wwwroot/           # Static assets (CSS, JS, images)
│   │   └── Program.cs         # Application entry point
│   └── MoneyMap/              # Core business logic and data access
│       ├── Application/       # Application services and interfaces
│       ├── Core/              # Domain models and business logic
│       ├── Infrastructure/    # Data access and external services
│       └── Migrations/        # Entity Framework migrations
├── MoneyMap.sln               # Visual Studio solution file
└── README.md                  # This documentation
```

## Architecture

### Core Layer (`MoneyMap.Core`)
- **DataModels**: Domain entities (Expense, ExpenseCategory)
- Business logic and domain rules

### Application Layer (`MoneyMap.Application`)
- **Services**: Business logic services (ExpenseService, CalendarService, UserService)
- **Interfaces**: Service contracts and abstractions

### Infrastructure Layer (`MoneyMap.Infrastructure`)
- **Data**: Entity Framework context and user models
- Database access and external service integrations

### Web Layer (`MoneyMap.Web`)
- **Areas**: Feature-based organization (Admin, Users, Identity)
- **Pages**: Razor Pages for user interface
- **wwwroot**: Static assets and client-side resources

## Data Models

### Expense
- `Id`: Primary key
- `Amount`: Decimal amount of the expense
- `Date`: Date when the expense occurred
- `Note`: Optional description or notes
- `CategoryId`: Foreign key to ExpenseCategory
- `Category`: Navigation property to ExpenseCategory

### ExpenseCategory
- `Id`: Primary key
- `Name`: Category name (e.g., "Food", "Transport", "Entertainment")
- `Expenses`: Collection of expenses in this category

### ApplicationUser
- Extends ASP.NET Core Identity's IdentityUser
- Contains user profile information and authentication data

## Setup and Installation

### Prerequisites

- .NET 9.0 SDK
- PostgreSQL database server
- Visual Studio 2022 or VS Code

### Database Setup

1. **Install PostgreSQL** and create a database for the application
2. **Update Connection String** in `src/MoneyMap.Web/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=moneymap;Username=your_username;Password=your_password"
     }
   }
   ```

### Application Setup

1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   cd MoneyMap
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Apply database migrations**:
   ```bash
   cd src/MoneyMap.Web
   dotnet ef database update
   ```

4. **Run the application**:
   ```bash
   dotnet run
   ```

5. **Access the application** at `https://localhost:5001` or `http://localhost:5000`

## Usage Guide

### User Registration and Login

1. Navigate to the application homepage
2. Click "Register" to create a new account
3. Fill in your email and password
4. Log in with your credentials

### Managing Expenses

1. **Add an Expense**:
   - Navigate to the Expenses section
   - Click "Create New"
   - Enter amount, date, category, and optional notes
   - Click "Create"

2. **Edit an Expense**:
   - Find the expense in the list
   - Click "Edit"
   - Modify the details and save

3. **Delete an Expense**:
   - Find the expense in the list
   - Click "Delete" and confirm

### Category Management

- Categories are managed by administrators
- Users can select from available categories when creating expenses
- Default categories are seeded during database initialization

### Admin Features

- **User Management**: View and manage user accounts
- **Category Management**: Add, edit, and delete expense categories
- **System Administration**: Monitor application usage and manage settings

## Development

### Adding New Features

1. **Create Data Models** in `MoneyMap.Core/DataModels/`
2. **Add Services** in `MoneyMap.Application/Services/`
3. **Create Razor Pages** in appropriate Areas
4. **Update Database** with new migrations

### Database Migrations

```bash
# Create a new migration
dotnet ef migrations add MigrationName

# Apply migrations to database
dotnet ef database update

# Remove last migration (if not applied)
dotnet ef migrations remove
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Configuration

### App Settings

Key configuration options in `appsettings.json`:

- **ConnectionStrings**: Database connection string
- **Logging**: Application logging levels
- **Identity**: Authentication and authorization settings

### Environment Variables

- `ASPNETCORE_ENVIRONMENT`: Set to "Development", "Staging", or "Production"
- `ConnectionStrings__DefaultConnection`: Database connection string

## Security Features

- **Password Requirements**: Configurable password complexity rules
- **Account Lockout**: Automatic lockout after failed login attempts
- **Email Confirmation**: Optional email verification for new accounts
- **Role-Based Authorization**: Admin and user role separation
- **CSRF Protection**: Built-in cross-site request forgery protection

## Deployment

### Production Deployment

1. **Build the application**:
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. **Configure production settings**:
   - Update connection strings
   - Set environment variables
   - Configure logging

3. **Deploy to your hosting platform**:
   - Azure App Service
   - AWS Elastic Beanstalk
   - Docker containers
   - IIS

### Docker Deployment

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/MoneyMap.Web/MoneyMap.Web.csproj", "MoneyMap.Web/"]
COPY ["src/MoneyMap/MoneyMap.csproj", "MoneyMap/"]
RUN dotnet restore "MoneyMap.Web/MoneyMap.Web.csproj"
COPY . .
WORKDIR "/src/MoneyMap.Web"
RUN dotnet build "MoneyMap.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MoneyMap.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MoneyMap.Web.dll"]
```

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the terms specified in the LICENSE.txt file.

## Support

For support and questions:
- Create an issue in the repository
- Contact the development team
- Check the documentation for common solutions

## Changelog

### Version 1.0.0
- Initial release
- Basic expense tracking functionality
- User authentication and authorization
- Admin panel for user and category management
- PostgreSQL database integration