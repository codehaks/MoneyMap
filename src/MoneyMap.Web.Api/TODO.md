# MoneyMap Web API - Implementation TODO List

## Overview
This document outlines the complete implementation plan for the MoneyMap Web API that will serve Android and desktop applications for expense management.

## ✅ Completed Tasks
- [x] Basic API project structure created
- [x] OpenAPI/Swagger integration configured

## 🚧 In Progress
- [ ] Database context and services setup

## 📋 Pending Tasks

### 1. Core Infrastructure Setup
- [ ] **Database Context Configuration**
  - [ ] Add Entity Framework packages to project
  - [ ] Configure MoneyMapDbContext in Program.cs
  - [ ] Set up connection string in appsettings
  - [ ] Register IExpenseService dependency injection

- [ ] **CORS Configuration**
  - [ ] Configure CORS policy for mobile/desktop apps
  - [ ] Allow all origins, methods, and headers for development
  - [ ] Consider production CORS restrictions later

### 2. Data Transfer Objects (DTOs)
- [ ] **Response DTOs**
  - [ ] ExpenseDto - for returning expense data
  - [ ] ExpenseCategoryDto - for returning category data
  - [ ] ApiResponseDto<T> - standardized API response wrapper

- [ ] **Request DTOs**
  - [ ] CreateExpenseDto - for creating new expenses
  - [ ] UpdateExpenseDto - for updating existing expenses
  - [ ] ExpenseFilterDto - for filtering/searching expenses

### 3. ExpensesController Implementation
- [ ] **GET Endpoints**
  - [ ] GET /api/expenses - Get all expenses with filtering
    - [ ] Support userId parameter (required)
    - [ ] Support searchTerm parameter (optional)
    - [ ] Support categoryId parameter (optional)
    - [ ] Support pagination parameters (optional)
  - [ ] GET /api/expenses/{id} - Get specific expense
  - [ ] GET /api/expenses/categories - Get all categories

- [ ] **POST Endpoints**
  - [ ] POST /api/expenses - Create new expense
    - [ ] Validate required fields
    - [ ] Return created expense with generated ID

- [ ] **PUT Endpoints**
  - [ ] PUT /api/expenses/{id} - Update existing expense
    - [ ] Validate expense exists and belongs to user
    - [ ] Return updated expense data

- [ ] **DELETE Endpoints**
  - [ ] DELETE /api/expenses/{id} - Delete expense
    - [ ] Validate expense exists and belongs to user
    - [ ] Return appropriate status code

### 4. Error Handling & Validation
- [ ] **Model Validation**
  - [ ] Add data annotations to DTOs
  - [ ] Implement custom validation attributes if needed
  - [ ] Return proper validation error responses

- [ ] **Exception Handling**
  - [ ] Global exception handling middleware
  - [ ] Structured error responses
  - [ ] Proper HTTP status codes
  - [ ] Logging for all errors

### 5. API Documentation & Testing
- [ ] **OpenAPI/Swagger Enhancement**
  - [ ] Add XML documentation comments
  - [ ] Configure Swagger UI for better testing
  - [ ] Add example request/response bodies

- [ ] **HTTP Test File**
  - [ ] Create comprehensive test scenarios
  - [ ] Test all CRUD operations
  - [ ] Test error scenarios
  - [ ] Test filtering and search functionality

### 6. Performance & Best Practices
- [ ] **Response Optimization**
  - [ ] Implement pagination for large datasets
  - [ ] Add response compression
  - [ ] Optimize database queries

- [ ] **Caching Strategy**
  - [ ] Consider caching for categories endpoint
  - [ ] Implement appropriate cache headers

### 7. Security Preparation (Pre-JWT)
- [ ] **Input Sanitization**
  - [ ] Validate and sanitize all user inputs
  - [ ] Prevent SQL injection through proper EF usage
  - [ ] Implement rate limiting if needed

- [ ] **HTTPS Configuration**
  - [ ] Ensure HTTPS redirection is properly configured
  - [ ] Configure security headers

## 🎥 VIDEO RECORDING PHASE - JWT Authentication Implementation

### 8. JWT Authentication Setup (SAVE FOR LAST)
- [ ] **JWT Configuration**
  - [ ] Install JWT authentication packages
  - [ ] Configure JWT settings in appsettings
  - [ ] Set up JWT middleware in Program.cs

- [ ] **Authentication Controller**
  - [ ] POST /api/auth/login - User login endpoint
  - [ ] POST /api/auth/register - User registration endpoint
  - [ ] POST /api/auth/refresh - Token refresh endpoint

- [ ] **JWT Token Management**
  - [ ] Token generation service
  - [ ] Token validation middleware
  - [ ] Refresh token implementation
  - [ ] Token expiration handling

- [ ] **Secure ExpensesController**
  - [ ] Add [Authorize] attributes
  - [ ] Extract user ID from JWT claims
  - [ ] Remove userId from query parameters (use JWT claims instead)
  - [ ] Update all endpoints to use authenticated user context

- [ ] **User Management Integration**
  - [ ] Integrate with existing Identity system
  - [ ] User registration/login flow
  - [ ] Password hashing and validation

## 📱 Mobile/Desktop App Considerations
- [ ] **API Versioning**
  - [ ] Implement API versioning strategy
  - [ ] Plan for backward compatibility

- [ ] **Offline Support Preparation**
  - [ ] Design sync-friendly endpoints
  - [ ] Consider timestamp-based synchronization
  - [ ] Plan for conflict resolution

## 🚀 Deployment Preparation
- [ ] **Configuration Management**
  - [ ] Environment-specific settings
  - [ ] Production connection strings
  - [ ] Security configurations

- [ ] **Health Checks**
  - [ ] Database connectivity health check
  - [ ] API health endpoint

## 📊 Monitoring & Logging
- [ ] **Structured Logging**
  - [ ] Implement comprehensive logging
  - [ ] Log API requests/responses
  - [ ] Performance monitoring

## Notes
- All endpoints should return JSON responses
- Follow RESTful API conventions
- Implement proper HTTP status codes
- JWT implementation is reserved for video recording session
- Focus on clean, maintainable code structure
- Ensure proper error handling throughout

## API Endpoint Summary (Final Structure)
```
GET    /api/expenses              - Get user's expenses (with filtering)
GET    /api/expenses/{id}         - Get specific expense
POST   /api/expenses              - Create new expense
PUT    /api/expenses/{id}         - Update expense
DELETE /api/expenses/{id}         - Delete expense
GET    /api/expenses/categories   - Get all categories

POST   /api/auth/login           - User login (JWT phase)
POST   /api/auth/register        - User registration (JWT phase)
POST   /api/auth/refresh         - Refresh token (JWT phase)
```
