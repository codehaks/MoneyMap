# JWT Authentication Implementation TODO List
## MoneyMap Web API - Simple JWT Integration Guide

### 📋 Overview
This document provides a **simplified** step-by-step guide to implement JWT (JSON Web Token) authentication in the MoneyMap Web API. The implementation leverages the existing ASP.NET Core Identity cookie-based authentication from MoneyMap.Web and provides secure token-based authentication for mobile and desktop applications **without adding new data models**.

### 🎯 Goals
- Secure all API endpoints with JWT Bearer tokens
- **Reuse existing Identity system from MoneyMap.Web** (no new models)
- Provide simple login/register endpoints for client applications
- **No refresh tokens** (keep it simple with longer-lived access tokens)
- Remove userId query parameters (use JWT claims instead)
- Support role-based authorization

---

## 📦 Phase 1: Package Installation & Dependencies

### 1.1 Install Required NuGet Packages
```bash
# Navigate to API project directory
cd src/MoneyMap.Web.Api

# Install JWT Bearer authentication
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer

# Install System.IdentityModel.Tokens.Jwt (if not already included)
dotnet add package System.IdentityModel.Tokens.Jwt
```

### 1.2 Verify Package References
Update `MoneyMap.Web.Api.csproj` to include:
```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.8" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.1.2" />
```

---

## ⚙️ Phase 2: Configuration Setup

### 2.1 Update appsettings.json
Add JWT configuration section:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=MoneyMapDb;Username=postgres;Password=your_password"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "MoneyMapAPI",
    "Audience": "MoneyMapClients",
    "ExpirationInHours": 24
  }
}
```

### 2.2 Update appsettings.Development.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.AspNetCore.Authentication": "Debug"
    }
  },
  "JwtSettings": {
    "SecretKey": "DevelopmentSecretKeyThatIsAtLeast32CharactersLongForTesting!",
    "Issuer": "MoneyMapAPI-Dev",
    "Audience": "MoneyMapClients-Dev",
    "ExpirationInHours": 48
  }
}
```

### 2.3 Create Simple JWT Configuration Model
Create `Models/JwtSettings.cs`:
```csharp
namespace MoneyMap.Web.Api.Models;

public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationInHours { get; set; } = 24;
}
```

---

## 🔧 Phase 3: Simple JWT Service Implementation

### 3.1 Create Simple JWT Token Service Interface
Create `Services/IJwtTokenService.cs`:
```csharp
using MoneyMap.Infrastructure.Data;

namespace MoneyMap.Web.Api.Services;

public interface IJwtTokenService
{
    Task<string> GenerateAccessTokenAsync(ApplicationUser user);
}
```

### 3.2 Create Simple JWT Token Service Implementation
Create `Services/JwtTokenService.cs`:
```csharp
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MoneyMap.Infrastructure.Data;
using MoneyMap.Web.Api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MoneyMap.Web.Api.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<JwtTokenService> _logger;

    public JwtTokenService(
        IOptions<JwtSettings> jwtSettings,
        UserManager<ApplicationUser> userManager,
        ILogger<JwtTokenService> logger)
    {
        _jwtSettings = jwtSettings.Value;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new("sub", user.Id), // Standard JWT subject claim
            new("jti", Guid.NewGuid().ToString()) // JWT ID
        };

        // Add user roles to claims
        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpirationInHours), // Longer-lived tokens
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

---

## 🗄️ Phase 4: Skip Database Changes (Using Existing Identity)

**✅ No database changes needed!** We're using the existing ASP.NET Core Identity system from MoneyMap.Web, so no new models or migrations are required.

---

## 🔐 Phase 5: Simple Authentication Controller

### 5.1 Create Simple Authentication DTOs
Update `Models/DTOs.cs` to include:
```csharp
// Simple Authentication DTOs
public class LoginRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequestDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
    
    [Required]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
    
    [MaxLength(256)]
    public string? UserName { get; set; }
}

public class AuthResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserInfoDto User { get; set; } = new();
}

public class UserInfoDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}
```

### 5.2 Create Authentication Controller
Create `Controllers/AuthController.cs`:
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoneyMap.Infrastructure.Data;
using MoneyMap.Web.Api.Models;
using MoneyMap.Web.Api.Services;
using System.ComponentModel.DataAnnotations;

namespace MoneyMap.Web.Api.Controllers;

/// <summary>
/// Controller for authentication operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    /// <summary>
    /// User login endpoint
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>JWT tokens and user information</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 401)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = errors
                });
            }

            _logger.LogInformation("Login attempt for email: {Email}", request.Email);

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid email or password",
                    Errors = new List<string> { "Authentication failed" }
                });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                var message = result.IsLockedOut ? "Account is locked out" : "Invalid email or password";
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = message,
                    Errors = new List<string> { "Authentication failed" }
                });
            }

            // Generate token
            var accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user);

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);

            var response = new AuthResponseDto
            {
                AccessToken = accessToken,
                ExpiresAt = DateTime.UtcNow.AddHours(24), // From settings
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    Roles = roles.ToList()
                }
            };

            _logger.LogInformation("User {UserId} logged in successfully", user.Id);

            return Ok(new ApiResponse<AuthResponseDto>
            {
                Success = true,
                Message = "Login successful",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", request.Email);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred during login",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    /// <summary>
    /// User registration endpoint
    /// </summary>
    /// <param name="request">Registration information</param>
    /// <returns>JWT tokens and user information</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = errors
                });
            }

            _logger.LogInformation("Registration attempt for email: {Email}", request.Email);

            var user = new ApplicationUser
            {
                UserName = request.UserName ?? request.Email,
                Email = request.Email,
                EmailConfirmed = true // For API, we'll skip email confirmation
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Registration failed",
                    Errors = errors
                });
            }

            // Assign default role (optional)
            await _userManager.AddToRoleAsync(user, "User");

            // Generate token
            var accessToken = await _jwtTokenService.GenerateAccessTokenAsync(user);

            var response = new AuthResponseDto
            {
                AccessToken = accessToken,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = new List<string> { "User" }
                }
            };

            _logger.LogInformation("User {UserId} registered successfully", user.Id);

            return CreatedAtAction(nameof(GetUserInfo), new { id = user.Id }, new ApiResponse<AuthResponseDto>
            {
                Success = true,
                Message = "Registration successful",
                Data = response
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for email: {Email}", request.Email);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred during registration",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    /// <summary>
    /// Simple logout endpoint (client-side token removal)
    /// </summary>
    /// <returns>Success response</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    public ActionResult<ApiResponse<object>> Logout()
    {
        // With stateless JWT tokens, logout is handled client-side by removing the token
        // No server-side action needed for simple implementation
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Logged out successfully. Please remove the token from client storage."
        });
    }

    /// <summary>
    /// Get current user information
    /// </summary>
    /// <returns>Current user info</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserInfoDto>), 200)]
    public async Task<ActionResult<ApiResponse<UserInfoDto>>> GetUserInfo()
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User not found"
                });
            }

            var roles = await _userManager.GetRolesAsync(user);

            var userInfo = new UserInfoDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                Roles = roles.ToList()
            };

            return Ok(new ApiResponse<UserInfoDto>
            {
                Success = true,
                Message = "User information retrieved successfully",
                Data = userInfo
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user information");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while retrieving user information",
                Errors = new List<string> { ex.Message }
            });
        }
    }
}
```

---

## ⚙️ Phase 6: Simple Program.cs Configuration

### 6.1 Update Program.cs
Replace the existing `Program.cs` with this **simplified version**:
```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MoneyMap.Application;
using MoneyMap.Application.Services;
using MoneyMap.Infrastructure.Data;
using MoneyMap.Web.Api.Models;
using MoneyMap.Web.Api.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure JWT Settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

// Add database context (reuse existing from MoneyMap.Web)
builder.Services.AddDbContext<MoneyMapDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add Identity (reuse existing configuration from MoneyMap.Web)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Use same settings as MoneyMap.Web for consistency
    options.SignIn.RequireConfirmedAccount = false;
    options.User.RequireUniqueEmail = true;
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
})
.AddEntityFrameworkStores<MoneyMapDbContext>()
.AddDefaultTokenProviders();

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // Set to true in production
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = jwtSettings?.Issuer,
        ValidAudience = jwtSettings?.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.SecretKey ?? throw new InvalidOperationException("JWT Secret Key is not configured")))
    };
});

// Add Authorization
builder.Services.AddAuthorization();

// Register application services
builder.Services.AddTransient<IExpenseService, ExpenseService>();
builder.Services.AddTransient<IJwtTokenService, JwtTokenService>();

// Add Swagger/OpenAPI services with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MoneyMap API",
        Version = "v1",
        Description = "Simple API for managing personal expenses with JWT authentication"
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Include XML comments for better documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Add CORS for mobile/desktop apps
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MoneyMap API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAll");

// Authentication & Authorization (ORDER MATTERS!)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```

---

## 🔧 Phase 7: Update ExpensesController

### 7.1 Remove userId Parameters
Update the `ExpensesController` to remove all `userId` query parameters and use JWT claims instead. The `GetCurrentUserId()` method is already implemented correctly.

### 7.2 Update HTTP Methods
Remove `userId` from all method signatures:

**Before:**
```csharp
public ActionResult<ApiResponse<PagedResult<ExpenseDto>>> GetExpenses(
    [FromQuery] string userId, // REMOVE THIS
    [FromQuery] string? searchTerm = null,
    // ... other parameters
)
```

**After:**
```csharp
public ActionResult<ApiResponse<PagedResult<ExpenseDto>>> GetExpenses(
    [FromQuery] string? searchTerm = null,
    // ... other parameters
)
```

Apply this change to all methods that currently accept `userId` parameter.

---

## 🧪 Phase 8: Simple Testing & Documentation

### 8.1 Create Simple HTTP Test File
Create `MoneyMap.Web.Api.JWT.http`:
```http
@MoneyMap.Web.Api_HostAddress = http://localhost:5137
@TestEmail = testuser@example.com
@TestPassword = TestPassword123!

### Register a new user
POST {{MoneyMap.Web.Api_HostAddress}}/api/auth/register
Content-Type: application/json

{
  "email": "{{TestEmail}}",
  "password": "{{TestPassword}}",
  "confirmPassword": "{{TestPassword}}",
  "userName": "testuser"
}

### Login with credentials
# @name login
POST {{MoneyMap.Web.Api_HostAddress}}/api/auth/login
Content-Type: application/json

{
  "email": "{{TestEmail}}",
  "password": "{{TestPassword}}"
}

### Extract token from login response
@accessToken = {{login.response.body.data.accessToken}}

### Get current user info
GET {{MoneyMap.Web.Api_HostAddress}}/api/auth/me
Authorization: Bearer {{accessToken}}

### Get all expense categories (no auth required)
GET {{MoneyMap.Web.Api_HostAddress}}/api/expenses/categories
Accept: application/json

### Get all expenses for authenticated user
GET {{MoneyMap.Web.Api_HostAddress}}/api/expenses
Authorization: Bearer {{accessToken}}
Accept: application/json

### Get expenses with search term
GET {{MoneyMap.Web.Api_HostAddress}}/api/expenses?searchTerm=grocery
Authorization: Bearer {{accessToken}}
Accept: application/json

### Create new expense
POST {{MoneyMap.Web.Api_HostAddress}}/api/expenses
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
  "amount": 25.50,
  "date": "2024-01-15T10:30:00",
  "note": "Grocery shopping at Walmart",
  "categoryId": 1,
  "userName": "testuser"
}

### Simple logout (client-side token removal)
POST {{MoneyMap.Web.Api_HostAddress}}/api/auth/logout
Authorization: Bearer {{accessToken}}

### Test unauthorized access (should fail)
GET {{MoneyMap.Web.Api_HostAddress}}/api/expenses
Accept: application/json
```

### 8.2 Update README.md
Add JWT authentication section to the API README.

---

## 🚀 Phase 9: Simple Production Considerations

### 9.1 Basic Production Configuration
- [ ] Use strong, randomly generated JWT secret keys (store in environment variables)
- [ ] Enable HTTPS enforcement (`RequireHttpsMetadata = true`)
- [ ] Configure appropriate CORS policies (restrict origins)
- [ ] Set longer token expiration for production (24-48 hours)

### 9.2 Optional Security Enhancements (Future)
- [ ] Implement rate limiting for authentication endpoints
- [ ] Add email confirmation for registration
- [ ] Implement password reset functionality
- [ ] Add token blacklisting for logout (if needed)

---

## ✅ Simple Testing Checklist

### Authentication Flow Testing
- [ ] User registration with valid data
- [ ] User registration with invalid data (validation errors)
- [ ] User login with valid credentials
- [ ] User login with invalid credentials
- [ ] User login with locked account
- [ ] Simple logout (client-side token removal)

### API Endpoint Testing
- [ ] Access protected endpoints with valid JWT
- [ ] Access protected endpoints without JWT (should return 401)
- [ ] Access protected endpoints with expired JWT (should return 401)
- [ ] Access protected endpoints with invalid JWT (should return 401)
- [ ] Verify user isolation (users can only access their own data)

---

## 📚 Simplified Implementation Notes

### Key Simplifications Made
- **No refresh tokens** - Using longer-lived access tokens (24-48 hours)
- **No new database models** - Reusing existing Identity system
- **Stateless logout** - Client-side token removal only
- **Minimal configuration** - Only essential JWT settings

### JWT Best Practices (Simplified)
- Use strong secret keys (at least 256 bits)
- Store tokens securely on client side
- Always validate tokens server-side
- Use HTTPS in production

### Security Considerations
- Never store JWT secret keys in source code
- Use environment variables for production secrets
- Implement proper CORS policies
- Monitor authentication events

---

## 🎯 Simple Implementation Priority

1. **Phase 1-2**: Package installation and configuration (High Priority)
2. **Phase 3**: JWT service implementation (High Priority)
3. **Phase 4**: Skip - no database changes needed (✅ Done)
4. **Phase 5**: Authentication controller (High Priority)
5. **Phase 6**: Program.cs configuration (High Priority)
6. **Phase 7**: Update existing controllers (Medium Priority)
7. **Phase 8**: Testing and documentation (Medium Priority)
8. **Phase 9**: Production considerations (Low Priority)

---

## 🎉 Benefits of This Simplified Approach

- ✅ **No database migrations** - Uses existing Identity system
- ✅ **No complex refresh token logic** - Simpler to implement and maintain
- ✅ **Reuses existing user management** - Consistent with MoneyMap.Web
- ✅ **Faster implementation** - Fewer moving parts
- ✅ **Easy to understand** - Perfect for getting started with JWT

**Note**: This simplified implementation reuses the existing ASP.NET Core Identity system from MoneyMap.Web without adding any new data models or complex refresh token logic.
