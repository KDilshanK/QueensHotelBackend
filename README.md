# Queens Hotel Backend API

ASP.NET Core Web API for Queens Hotel Management System - Complete hotel reservation and billing management solution.

## ?? Project Overview

The Queens Hotel Backend API is a comprehensive REST API built with .NET 8 that manages all aspects of hotel operations including customer management, reservations, billing, room/suite management, and automated services.

**Developer:** dilshan-jolanka  
**Version:** 1.0.0  
**Last Updated:** 2025-05-29

## ? Features

### Core Modules

#### ?? Customer Management
- Customer registration and profile management
- Customer search by NIC, passport, email, phone, or name
- Customer login authentication
- Country of residence management

#### ?? Reservation Management
- Create, update, and cancel reservations
- Search reservations by multiple criteria
- Check-in and check-out processing
- Travel agency integration
- Payment method tracking
- Automated reservation cancellation service (daily at 7 PM)

#### ?? Billing & Payments
- Billing record creation and retrieval
- Service charge management
- No-show charge handling
- Customer invoice generation
- Credit card management with encryption
- Payment status tracking

#### ?? Room & Suite Management
- Room details retrieval with comprehensive information
- Suite management with amenities
- Floor and room type management
- Availability status tracking
- Room rate calculations

#### ?? Additional Services
- Meal plan management
- Travel agency partnerships
- Automated no-show billing
- Health check endpoints
- Comprehensive CORS support

## ??? Technologies

- **.NET 8** - Latest .NET framework
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core** - ORM for database access
- **SQL Server** - Database management system
- **Stored Procedures** - Optimized database operations
- **Swagger/OpenAPI** - API documentation
- **Background Services** - Automated task processing

## ?? Project Structure

```
QueensHotelAPI/
??? Controllers/           # API endpoints
?   ??? BillingController.cs
?   ??? BillingServiceChargeController.cs
?   ??? CheckinController.cs
?   ??? CheckOutController.cs
?   ??? CountryController.cs
?   ??? CreditCardController.cs
?   ??? CustomerController.cs
?   ??? CustomerInvoiceController.cs
?   ??? FloorController.cs
?   ??? MealPlanController.cs
?   ??? ReservationController.cs
?   ??? RoomController.cs
?   ??? RoomTypeController.cs
?   ??? SuiteController.cs
?   ??? TravelAgencyController.cs
?   ??? UserController.cs
?
??? Services/              # Business logic layer
?   ??? ReservationAutoCancelService.cs
?   ??? [Various service implementations]
?
??? Repositories/          # Data access layer
?   ??? [Repository implementations]
?
??? DTOs/                  # Data Transfer Objects
?   ??? [Request/Response DTOs]
?
??? Models/                # Domain models
?   ??? ReservationModels.cs
?
??? Data/                  # Database context
?   ??? QueensHotelDbContext.cs
?
??? Program.cs             # Application entry point
```

## ?? Getting Started

### Prerequisites

- .NET 8 SDK or later
- SQL Server 2019 or later
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/KDilshanK/QueensHotelBackend.git
   cd QueensHotelBackend
   ```

2. **Configure Database Connection**
   
   Update `appsettings.json` with your SQL Server connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=latestQueenHotel;Trusted_Connection=True;TrustServerCertificate=true;"
     }
   }
   ```

3. **Restore NuGet Packages**
   ```bash
   dotnet restore
   ```

4. **Build the Project**
   ```bash
   dotnet build
   ```

5. **Run the Application**
   ```bash
   dotnet run
   ```

The API will be available at:
- HTTP: `http://localhost:5170`
- HTTPS: `https://localhost:7111`
- Swagger UI: `http://localhost:5170/swagger`

## ?? API Documentation

### Key Endpoints

#### Reservations
- `GET /api/reservation` - Get all reservations
- `GET /api/reservation?id={id}` - Get reservation by ID
- `POST /api/reservation` - Create new reservation
- `PUT /api/reservation/{id}` - Update reservation
- `POST /api/reservation/cancel` - Cancel reservation

#### Customers
- `GET /api/customer` - Search customers
- `GET /api/customer/{id}` - Get customer by ID
- `POST /api/customer` - Create new customer
- `PUT /api/customer/{id}` - Update customer
- `POST /api/customer/login` - Customer login

#### Billing
- `POST /api/billing` - Insert billing record
- `GET /api/billing/{id}` - Get billing information
- `POST /api/billingservicecharge` - Add service charge
- `GET /api/billingservicecharge/{billingId}` - Get service charges

#### Invoices
- `GET /api/customerinvoice/{billingId}` - Generate customer invoice

#### Rooms & Suites
- `GET /api/room` - Get room details
- `GET /api/suite` - Get suite details
- `POST /api/suite` - Insert new suite

### Health Checks
- `GET /health` - API health status
- `GET /health/database` - Database connectivity check
- `GET /cors-test` - CORS configuration test

## ?? Security Features

- SQL injection prevention through parameterized queries
- Credit card data encryption
- Secure password handling
- CORS policy configuration
- Connection string protection
- Input validation and sanitization

## ?? Configuration

### App Settings

```json
{
  "QueensHotelSettings": {
    "ApiVersion": "1.0.0",
    "ApiTitle": "Queens Hotel API",
    "Developer": "dilshan-jolanka",
    "MaxRecordsPerRequest": 1000,
    "DefaultTimeZone": "UTC"
  }
}
```

### Auto-Cancel Service

The system includes an automated service that runs daily at 7:00 PM to:
- Identify no-show reservations (unpaid, past check-in date)
- Create no-show billing charges
- Cancel the reservations
- Configure time: `ReservationAutoCancelService.NextRunTimeOfDay`

## ?? Testing

### Using Swagger UI
1. Navigate to `/swagger`
2. Expand endpoint sections
3. Try out endpoints with sample data

### Using cURL
```bash
# Get all customers
curl http://localhost:5170/api/customer

# Get reservation by ID
curl http://localhost:5170/api/reservation?id=1

# Create billing
curl -X POST http://localhost:5170/api/billing \
  -H "Content-Type: application/json" \
  -d '{
    "billingDate": "2025-01-07",
    "totalAmount": 1500.00,
    "isNoShowCharge": false,
    "paymentStatus": "Paid",
    "paymentMethod": 1,
    "createdBy": "admin",
    "reservationId": 1
  }'
```

## ?? Database Schema

The API uses the following main tables:
- `Customer` - Customer information
- `Reservation` - Booking records
- `Billing` - Billing transactions
- `BillingServiceCharge` - Service charges
- `Room` / `Suite` - Accommodation details
- `MealPlan` - Meal plan options
- `TravelAgency` - Partner agencies
- `PaymentCard` - Encrypted payment data

## ?? Troubleshooting

### Common Issues

**Database Connection Failed**
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Ensure database exists
- Test with `/health/database` endpoint

**CORS Errors**
- API has comprehensive CORS enabled
- Test with `/cors-test` endpoint
- Check browser console for specific errors

**Stored Procedure Errors**
- Verify all stored procedures exist in database
- Check parameter names match exactly
- Use 'ALL' for filter parameters in GetCustomerData

## ?? Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## ?? License

This project is proprietary software for Queens Hotel Management System.

## ?? Author

**Dilshan Jolanka**
- GitHub: [@KDilshanK](https://github.com/KDilshanK)
- Email: dilshan-jolanka@queenshotel.com

## ?? Acknowledgments

- Queens Hotel management team
- Development team contributors
- Open source community

## ?? Support

For issues, questions, or contributions, please:
1. Check existing issues on GitHub
2. Create a new issue with detailed information
3. Contact the development team

---

**Note:** Before deploying to production, ensure all connection strings and sensitive data are properly secured using environment variables or Azure Key Vault.
