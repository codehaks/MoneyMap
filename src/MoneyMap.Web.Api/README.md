# MoneyMap Web API

A RESTful API for expense management that serves Android and desktop applications.

## 🚀 Getting Started

### Prerequisites
- .NET 9.0 SDK
- PostgreSQL database
- Visual Studio 2022 or VS Code

### Setup
1. Update the connection string in `appsettings.json` and `appsettings.Development.json`
2. Ensure the MoneyMap database is created and migrated
3. Run the API: `dotnet run`

The API will be available at:
- HTTP: `http://localhost:5137`
- HTTPS: `https://localhost:7001`
- Swagger UI: `https://localhost:7001/swagger` (Development only)

## 📋 API Endpoints

### Expenses Management

#### Get All Expenses
```http
GET /api/expenses?userId={userId}&searchTerm={term}&categoryId={id}&pageSize={size}&pageNumber={page}&startDate={date}&endDate={date}
```
- **userId** (required): User identifier
- **searchTerm** (optional): Search in expense notes
- **categoryId** (optional): Filter by category
- **pageSize** (optional): Items per page (default: 50, max: 1000)
- **pageNumber** (optional): Page number (default: 1)
- **startDate** (optional): Filter from date (ISO format)
- **endDate** (optional): Filter to date (ISO format)

#### Get Specific Expense
```http
GET /api/expenses/{id}?userId={userId}
```

#### Create Expense
```http
POST /api/expenses
Content-Type: application/json

{
  "amount": 25.50,
  "date": "2024-01-15T10:30:00",
  "note": "Grocery shopping",
  "categoryId": 1,
  "userId": "user-123",
  "userName": "user@example.com"
}
```

#### Update Expense
```http
PUT /api/expenses/{id}
Content-Type: application/json

{
  "amount": 30.75,
  "date": "2024-01-15T10:30:00",
  "note": "Updated grocery shopping",
  "categoryId": 1,
  "userId": "user-123"
}
```

#### Delete Expense
```http
DELETE /api/expenses/{id}?userId={userId}
```

#### Get Categories
```http
GET /api/expenses/categories
```

## 📊 Response Format

All API responses follow this standardized format:

```json
{
  "success": true,
  "message": "Operation completed successfully",
  "data": { /* actual response data */ },
  "errors": []
}
```

### Success Response Example
```json
{
  "success": true,
  "message": "Retrieved 5 expenses",
  "data": {
    "items": [
      {
        "id": 1,
        "amount": 25.50,
        "date": "2024-01-15T10:30:00",
        "note": "Grocery shopping",
        "categoryId": 1,
        "categoryName": "Groceries",
        "userId": "user-123",
        "userName": "user@example.com"
      }
    ],
    "totalCount": 5,
    "pageNumber": 1,
    "pageSize": 50,
    "totalPages": 1,
    "hasPreviousPage": false,
    "hasNextPage": false
  },
  "errors": []
}
```

### Error Response Example
```json
{
  "success": false,
  "message": "Validation failed",
  "data": null,
  "errors": [
    "Amount must be greater than 0",
    "UserId is required"
  ]
}
```

## 🏷️ Available Categories

The API comes with predefined expense categories:

1. **Groceries** - Food and household items
2. **Rent** - Housing costs
3. **Utilities** - Electricity, water, gas, internet
4. **Transportation** - Gas, public transport, car maintenance
5. **Entertainment** - Movies, games, dining out
6. **Health** - Medical expenses, pharmacy
7. **Insurance** - All types of insurance
8. **Other** - Miscellaneous expenses

## 🔧 Testing

Use the included `MoneyMap.Web.Api.http` file to test all endpoints. The file includes:
- Basic CRUD operations
- Filtering and pagination examples
- Error scenario testing
- Sample data for testing

## 🌐 CORS Configuration

The API is configured with permissive CORS settings for development:
- Allows all origins
- Allows all methods
- Allows all headers

**Note**: Restrict CORS settings for production deployment.

## 📝 Data Validation

### Create/Update Expense Validation
- **Amount**: Must be greater than 0
- **Date**: Required, valid DateTime
- **Note**: Optional, max 500 characters
- **CategoryId**: Required, must be valid category (1-8)
- **UserId**: Required, max 450 characters
- **UserName**: Required for creation, max 256 characters

## 🔒 Security Notes

**Current Status**: The API currently uses userId as a query parameter for user identification.

**Upcoming**: JWT authentication will be implemented to:
- Secure all endpoints with bearer tokens
- Extract user identity from JWT claims
- Remove userId from query parameters
- Implement proper authorization

## 🚧 Development Status

### ✅ Completed Features
- [x] Full CRUD operations for expenses
- [x] Category management
- [x] Filtering and search functionality
- [x] Pagination support
- [x] Comprehensive error handling
- [x] Input validation
- [x] CORS configuration
- [x] OpenAPI/Swagger documentation
- [x] Structured logging

### 🎥 Pending (For Video Recording)
- [ ] JWT Authentication implementation
- [ ] Secure endpoint authorization
- [ ] User registration/login endpoints
- [ ] Token refresh mechanism

## 🐛 Error Handling

The API handles various error scenarios:

### HTTP Status Codes
- **200 OK**: Successful GET/PUT operations
- **201 Created**: Successful POST operations
- **204 No Content**: Successful DELETE operations
- **400 Bad Request**: Validation errors, missing parameters
- **404 Not Found**: Resource not found
- **500 Internal Server Error**: Server-side errors

### Common Error Scenarios
1. **Missing UserId**: Returns 400 with validation error
2. **Invalid Expense ID**: Returns 404 with not found error
3. **Validation Failures**: Returns 400 with detailed validation errors
4. **Database Errors**: Returns 500 with generic error message (details logged)

## 📱 Mobile/Desktop App Integration

This API is designed for:
- **Android Apps**: Using Retrofit, OkHttp, or similar HTTP clients
- **Desktop Apps**: Using HttpClient in .NET, Electron with Axios, etc.
- **Cross-platform**: Xamarin, MAUI, Flutter, React Native

### Recommended Client Implementation
1. Store user credentials securely
2. Handle API responses consistently
3. Implement offline caching for better UX
4. Use pagination for large datasets
5. Implement proper error handling

## 🔧 Configuration

### Connection String Format
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=MoneyMapDb;Username=postgres;Password=your_password"
  }
}
```

### Environment-Specific Settings
- **Development**: Uses `appsettings.Development.json`
- **Production**: Uses `appsettings.json`

## 📊 Performance Considerations

- Pagination is implemented to handle large datasets
- Database queries are optimized with Entity Framework
- Responses include only necessary data through DTOs
- Logging is structured for performance monitoring

## 🤝 Contributing

When contributing to the API:
1. Follow the existing code structure
2. Add appropriate validation and error handling
3. Update the HTTP test file with new endpoints
4. Maintain consistent response format
5. Add XML documentation for new endpoints
