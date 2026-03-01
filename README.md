# HospitalCare API

A modern healthcare management system built with Clean Architecture, .NET 10, and MongoDB.

## Features

- **Clean Architecture** - Domain-driven design with clear separation of concerns
- **MongoDB Integration** - Code-first approach with MongoDB as the database
- **JWT Authentication** - Role-based authorization (Doctor, HospitalEmployee, Admin)
- **Scalar API Documentation** - Modern, interactive API documentation
- **Serilog Logging** - Structured logging to MongoDB with request tracking

## Project Structure

```
HospitalCare/
├── HospitalCare.Domain/           # Core domain entities and interfaces
│   ├── Entities/                  # Patient, Doctor, Appointment, User
│   ├── Interfaces/                # Repository interfaces
│   └── Events/                    # Domain events
├── HospitalCare.Application/      # Business logic and DTOs
│   ├── DTOs/                      # Data transfer objects
│   ├── Interfaces/                # Service interfaces
│   └── Services/                  # Application services
├── HospitalCare.Infrastructure/   # Data access and external services
│   ├── Data/                      # MongoDB context, migrations, seeding
│   ├── Repositories/              # MongoDB repositories
│   └── Services/                  # JWT service
├── HospitalCare.Api/              # REST API with controllers
│   └── Controllers/               # API endpoints
└── HospitalCare.Migrations/       # Database migration tool
```

## User Roles & Permissions

| Role | Permissions |
|------|-------------|
| **Admin** | Full access: manage doctors, patients, appointments, users |
| **HospitalEmployee** | Manage patients, create/reschedule appointments, register users |
| **Doctor** | View patients, appointments, doctors; complete appointments |

## Getting Started

### Prerequisites

- .NET 10 SDK
- MongoDB (local or Atlas)

### Configuration

Update `appsettings.json`:

```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "HospitalCareDb"
  },
  "Jwt": {
    "SecretKey": "Your-Secret-Key-Here",
    "Issuer": "HospitalCare",
    "Audience": "HospitalCareApi",
    "ExpirationMinutes": 60
  }
}
```

### Run Database Migrations

```bash
cd HospitalCare.Migrations
dotnet run
```

### Run the API

```bash
cd HospitalCare
dotnet run --project HospitalCare.Api
```

### Access Points

- **API**: http://localhost:5239
- **Scalar UI**: http://localhost:5239/scalar/v1
- **OpenAPI Spec**: http://localhost:5239/openapi/v1.json

## API Endpoints

### Authentication

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/login` | Login and get JWT token |
| POST | `/api/auth/register` | Register new user (auth required) |
| GET | `/api/auth/me` | Get current user info |
| POST | `/api/auth/change-password` | Change password |

### Patients

| Method | Endpoint | Description | Roles |
|--------|----------|-------------|-------|
| GET | `/api/patients` | Get all patients | Doctor, HospitalEmployee, Admin |
| GET | `/api/patients/{id}` | Get patient by ID | Doctor, HospitalEmployee, Admin |
| GET | `/api/patients/search?name=` | Search patients by name | Doctor, HospitalEmployee, Admin |
| POST | `/api/patients` | Create patient | HospitalEmployee, Admin |
| PUT | `/api/patients/{id}` | Update patient | HospitalEmployee, Admin |
| DELETE | `/api/patients/{id}` | Delete patient | Admin |

### Doctors

| Method | Endpoint | Description | Roles |
|--------|----------|-------------|-------|
| GET | `/api/doctors` | Get all doctors | Doctor, HospitalEmployee, Admin |
| GET | `/api/doctors/{id}` | Get doctor by ID | Doctor, HospitalEmployee, Admin |
| GET | `/api/doctors/specialization/{spec}` | Get doctors by specialization | Doctor, HospitalEmployee, Admin |
| POST | `/api/doctors` | Create doctor | Admin |
| DELETE | `/api/doctors/{id}` | Delete doctor | Admin |

### Appointments

| Method | Endpoint | Description | Roles |
|--------|----------|-------------|-------|
| GET | `/api/appointments` | Get all appointments | Doctor, HospitalEmployee, Admin |
| GET | `/api/appointments/{id}` | Get appointment by ID | Doctor, HospitalEmployee, Admin |
| GET | `/api/appointments/patient/{id}` | Get patient appointments | Doctor, HospitalEmployee, Admin |
| GET | `/api/appointments/doctor/{id}` | Get doctor appointments | Doctor, HospitalEmployee, Admin |
| POST | `/api/appointments` | Create appointment | HospitalEmployee, Admin |
| PUT | `/api/appointments/{id}/cancel` | Cancel appointment | Doctor, HospitalEmployee, Admin |
| PUT | `/api/appointments/{id}/complete` | Complete appointment | Doctor, Admin |
| PUT | `/api/appointments/{id}/reschedule` | Reschedule appointment | HospitalEmployee, Admin |

## Default Credentials

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@hospitalcare.com | Admin@123 |
| HospitalEmployee | reception@hospitalcare.com | Employee@123 |
| HospitalEmployee | nurse@hospitalcare.com | Employee@123 |

## Logging

All API requests are logged to MongoDB with:
- Timestamp
- HTTP method and path
- Response status code
- Response time
- User ID and email
- Environment and machine name

Query logs:
```javascript
db.logs.find().sort({ _id: -1 }).limit(10)
```

## Technologies

- **.NET 10** - Web API framework
- **MongoDB** - NoSQL database
- **Serilog** - Structured logging
- **JWT Bearer** - Authentication
- **Scalar** - API documentation
- **Clean Architecture** - Architectural pattern

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
