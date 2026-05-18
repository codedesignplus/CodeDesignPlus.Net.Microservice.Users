# 👥 Users Microservice

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=.net)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-LGPL%20v3-blue.svg)](LICENSE.md)
[![Tests](https://img.shields.io/badge/tests-passing-success)](tests/)
[![Coverage](https://img.shields.io/badge/coverage-80%25-green)]()
[![Docker](https://img.shields.io/badge/docker-ready-2496ED?logo=docker)](Dockerfile)

A production-ready microservice for managing user accounts, profiles, authentication metadata, and multi-tenant access control built with .NET 9. Implements Clean Architecture, DDD, and CQRS patterns for flexible user and identity management.

---

## What is this microservice?

The Users microservice manages the identity and profile data of every person who interacts with the platform: administrators, accountants, property owners, and residents. It solves the problem of knowing "who is this person, what organizations do they belong to, and what role do they play in each one." Administrators use it to create and manage user accounts, while the system uses it internally to link users with tenants and roles for access control. It works closely with the Roles, RBAC, and MicrosoftGraph microservices to provide a complete identity layer across the platform.

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Key Features](#-key-features)
- [Technology Stack](#️-technology-stack)
- [Prerequisites](#️-prerequisites)
- [Getting Started](#-getting-started)
- [API Endpoints](#-api-endpoints)
- [Domain Model](#-domain-model)
- [Configuration](#️-configuration)
- [Use Cases & Scenarios](#-use-cases--scenarios)
- [Architecture](#️-architecture)
- [Testing](#-testing)
- [Best Practices](#-best-practices)
- [Troubleshooting](#-troubleshooting)
- [Multi-Tenancy](#-multi-tenancy)
- [Profile Management](#-profile-management)
- [Security](#-security)
- [FAQ](#-faq)
- [Contributing](#-contributing)
- [License](#-license)

---

## 🎯 Overview

The Users microservice provides centralized user account and profile management for multi-tenant applications. It handles user identity data, role assignments, tenant associations, and comprehensive profile information.

- **User Management**: Create, update, and manage user accounts with complete lifecycle control
- **Profile System**: Comprehensive user profiles with contact info, job details, and profile pictures
- **Multi-Tenancy**: Associate users with multiple tenants and manage tenant-specific access
- **Role Management**: Assign and revoke roles for authorization and access control
- **Contact Information**: Manage addresses, phone numbers, and multiple email addresses
- **Job Information**: Track employment details, departments, hire dates, and office locations
- **Profile Pictures**: Upload and manage user profile images
- **Domain Events**: Publish user lifecycle events for integration with other microservices
- **CQRS Pattern**: Separate read and write operations for optimal performance

### 🚀 Quick Start

```bash
# 1. Start infrastructure services
git clone https://github.com/codedesignplus/CodeDesignPlus.Environment.Dev
cd CodeDesignPlus.Environment.Dev/resources
docker-compose up -d

# 2. Configure Vault secrets
cd ../../tools/vault
./config-vault.sh

# 3. Run the microservice
dotnet run --project src/entrypoints/CodeDesignPlus.Net.Microservice.Users.Rest

# 4. Access Swagger UI
open http://localhost:5000/swagger
```

### 📊 High-Level Architecture

```
┌─────────────┐
│   Client    │
│ Application │
└──────┬──────┘
       │ HTTPS + JWT
       │
┌──────▼──────────────────────────────────────────────┐
│         Users Microservice (REST/gRPC/Worker)       │
│  ┌──────────────┐  ┌─────────────┐  ┌────────────┐ │
│  │ Controllers  │  │  MediatR    │  │  Handlers  │ │
│  │   (API)      │─▶│   (CQRS)    │─▶│ (Business) │ │
│  └──────────────┘  └─────────────┘  └────┬───────┘ │
│                                           │         │
│  ┌────────────────────────────────────────▼──────┐ │
│  │         UserAggregate (Domain Model)          │ │
│  │  • Basic Info    • Tenants    • Roles         │ │
│  │  • Contact Info  • Job Info   • Picture       │ │
│  └────────────────────────────────────────────────┘ │
└───────┬──────────────────┬──────────────────┬───────┘
        │                  │                  │
   ┌────▼────┐      ┌──────▼──────┐    ┌─────▼─────┐
   │ MongoDB │      │   Redis     │    │ RabbitMQ  │
   │ (Users) │      │  (Cache)    │    │ (Events)  │
   └─────────┘      └─────────────┘    └───────────┘
```

## 🚀 Key Features

### Core Capabilities

- ✅ **User CRUD**: Create, read, update, and delete user accounts
- ✅ **Multi-Tenant Support**: Associate users with multiple tenants/organizations
- ✅ **Role Management**: Add and remove roles for authorization
- ✅ **Profile Management**: Comprehensive user profiles with contact and job information
- ✅ **Contact Information**: Manage addresses, cities, postal codes, phones, and emails
- ✅ **Job Information**: Track job titles, companies, departments, employee IDs, hire dates
- ✅ **Profile Pictures**: Upload and update user profile images
- ✅ **Batch Profile Updates**: Update all profile information in a single operation
- ✅ **Active/Inactive Status**: Control user account activation
- ✅ **Soft Deletion**: Logical deletion with audit trail
- ✅ **Domain Events**: Publish lifecycle events for integration
- ✅ **Pagination & Filtering**: OData-style queries for user lists
- ✅ **Problem Details**: RFC 7807 compliant error responses

### Technical Features

- Clean Architecture with DDD and CQRS
- Domain events for user state changes
- MongoDB for user data persistence
- RabbitMQ for event publishing
- Redis for distributed caching
- OAuth2/OpenID Connect security
- REST API with Swagger documentation
- gRPC API for inter-service communication
- AsyncWorker for event-driven operations
- Multi-tenancy at the data and access level
- Docker containerization
- Comprehensive test coverage (Unit, Integration)

## 🛠️ Technology Stack

### Core
- **.NET 9** - Runtime and framework
- **ASP.NET Core** - Web API framework
- **C# 13** - Programming language

### Storage & Data
- **MongoDB** - User data persistence and queries
- **Redis** - Distributed caching and session storage

### Messaging & Events
- **RabbitMQ** - Event publishing and message broker

### Architecture & Patterns
- **MediatR** - CQRS command/query handling
- **FluentValidation** - Input validation
- **Mapster** - Object mapping
- **NodaTime** - Date/time handling

### Security & Configuration
- **Vault** - Secret management
- **OAuth2/OpenID Connect** - Authentication
- **JWT Bearer** - Token-based security
- **HTTPS** - Encrypted communication

### DevOps & Testing
- **Docker** - Containerization
- **xUnit** - Unit/integration testing
- **Swagger/OpenAPI** - API documentation
- **gRPC** - Inter-service communication

## ⚙️ Prerequisites

### Required
- **.NET 9 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Docker & Docker Compose** - For infrastructure services
- **MongoDB 6.0+** - Document database
- **Redis 7.0+** - Caching layer
- **RabbitMQ 3.12+** - Message broker

### Optional
- **Vault** - Secret management (can use appsettings for local dev)

## 🚀 Getting Started

The following instructions will help you set up the project on your local machine for development and testing purposes.

1. Clone the repository:
```bash
git clone <repository-url>
cd CodeDesignPlus.Net.Microservice.Users
```

2. Run the MongoDB, Redis, and RabbitMQ services using Docker Compose. Clone this repository [CodeDesignPlus.Environment.Dev](https://github.com/codedesignplus/CodeDesignPlus.Environment.Dev) and run the following command:

```bash
cd resources
docker-compose up -d
```

3. Run the script to config the vault:

```bash
cd tools/vault
./config-vault.sh
```

4. Build the solution:
```bash
dotnet build
```

5. Run the desired entry point:
   
   - For REST API:
      ```bash
      dotnet run --project src/entrypoints/CodeDesignPlus.Net.Microservice.Users.Rest
      ```

   - For gRPC:
      ```bash
      dotnet run --project src/entrypoints/CodeDesignPlus.Net.Microservice.Users.gRpc
      ```

   - For Worker:
      ```bash
      dotnet run --project src/entrypoints/CodeDesignPlus.Net.Microservice.Users.AsyncWorker
      ```

## 📡 API Endpoints

### User Operations

#### Create User
```http
POST /api/user
Content-Type: application/json
Authorization: Bearer {token}
X-Tenant: {tenant-id}

{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "firstName": "John",
  "lastName": "Doe",
  "displayName": "John Doe",
  "email": "john.doe@example.com",
  "phone": "+1234567890",
  "isActive": true
}
```

**Response**: `204 No Content`

#### Get User by ID
```http
GET /api/user/{id}
Authorization: Bearer {token}
X-Tenant: {tenant-id}
```

**Response**: `200 OK` with user details
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "firstName": "John",
  "lastName": "Doe",
  "displayName": "John Doe",
  "email": "john.doe@example.com",
  "phone": "+1234567890",
  "picture": {
    "id": "pic-123",
    "name": "profile.jpg",
    "target": "https://storage.example.com/users/john/profile.jpg"
  },
  "tenants": [
    {
      "id": "tenant-a-id",
      "name": "ACME Corporation"
    }
  ],
  "roles": ["Admin", "User"],
  "contact": {
    "address": "123 Main St",
    "city": "New York",
    "state": "NY",
    "country": "USA",
    "postalCode": "10001",
    "phone": "+1234567890",
    "email": ["john.doe@example.com", "john@personal.com"]
  },
  "job": {
    "jobTitle": "Software Engineer",
    "companyName": "ACME Corp",
    "department": "Engineering",
    "employeeId": "EMP-001",
    "employeeType": "Full-Time",
    "employHireDate": "2023-01-15T00:00:00Z",
    "officeLocation": "New York HQ"
  },
  "isActive": true
}
```

#### List Users (Paginated)
```http
GET /api/user?limit=50&skip=0&filter=isActive eq true&orderby=lastName asc
Authorization: Bearer {token}
X-Tenant: {tenant-id}
```

**Query Parameters**:
- `limit` (optional): Number of items per page (default: 100)
- `skip` (optional): Number of items to skip (default: 0)
- `filter` (optional): OData filter expression (e.g., `firstName eq 'John'`, `isActive eq true`)
- `orderby` (optional): OData order expression (e.g., `lastName asc`, `createdAt desc`)

**Response**: `200 OK` with paginated results
```json
{
  "data": [
    {
      "id": "...",
      "firstName": "John",
      "lastName": "Doe",
      "email": "john.doe@example.com",
      "isActive": true
    }
  ],
  "totalCount": 250,
  "limit": 50,
  "skip": 0
}
```

#### Update User
```http
PUT /api/user/{id}
Content-Type: application/json
Authorization: Bearer {token}
X-Tenant: {tenant-id}

{
  "firstName": "John",
  "lastName": "Doe",
  "displayName": "Johnny D",
  "email": "john.doe@example.com",
  "phone": "+1234567890",
  "isActive": true
}
```

**Response**: `204 No Content`

#### Delete User
```http
DELETE /api/user/{id}
Authorization: Bearer {token}
X-Tenant: {tenant-id}
```

**Response**: `204 No Content`

**Note**: This is a soft delete. The user is marked as deleted and deactivated but data is preserved.

### Tenant Management

#### Add Tenant to User
```http
POST /api/user/{id}/tenant
Content-Type: application/json
Authorization: Bearer {token}
X-Tenant: {tenant-id}

{
  "tenantId": "tenant-b-id",
  "name": "Beta Corporation"
}
```

**Response**: `204 No Content`

**Domain Event Published**: `TenantAddedDomainEvent`

#### Remove Tenant from User
```http
DELETE /api/user/{id}/tenant/{tenantId}
Authorization: Bearer {token}
X-Tenant: {tenant-id}
```

**Response**: `204 No Content`

**Domain Event Published**: `TenantRemovedDomainEvent`

### Role Management

#### Add Role to User
```http
POST /api/user/{id}/role
Content-Type: application/json
Authorization: Bearer {token}
X-Tenant: {tenant-id}

{
  "role": "Admin"
}
```

**Response**: `204 No Content`

**Domain Event Published**: `RoleAddedToUserDomainEvent`

#### Remove Role from User
```http
DELETE /api/user/{id}/role/{role}
Authorization: Bearer {token}
X-Tenant: {tenant-id}
```

**Response**: `204 No Content`

**Domain Event Published**: `RoleRemovedToUserDomainEvent`

### Profile Management

#### Update Contact Information
```http
PATCH /api/user/{id}/contact
Content-Type: application/json
Authorization: Bearer {token}
X-Tenant: {tenant-id}

{
  "address": "456 Oak Ave",
  "city": "San Francisco",
  "state": "CA",
  "country": "USA",
  "postalCode": "94102",
  "phone": "+14155551234",
  "email": ["john.doe@example.com", "john@personal.com"]
}
```

**Response**: `204 No Content`

**Domain Event Published**: `ContactInfoUpdatedDomainEvent`

#### Update Job Information
```http
PATCH /api/user/{id}/job
Content-Type: application/json
Authorization: Bearer {token}
X-Tenant: {tenant-id}

{
  "jobTitle": "Senior Software Engineer",
  "companyName": "ACME Corp",
  "department": "Engineering",
  "employeeId": "EMP-001",
  "employeeType": "Full-Time",
  "employHireDate": "2023-01-15T00:00:00Z",
  "officeLocation": "San Francisco Office"
}
```

**Response**: `204 No Content`

**Domain Event Published**: `JobInfoUpdatedDomainEvent`

#### Update Profile (All-in-One)
```http
PUT /api/user/{id}/profile
Content-Type: application/json
Authorization: Bearer {token}
X-Tenant: {tenant-id}

{
  "firstName": "John",
  "lastName": "Doe",
  "displayName": "John Doe",
  "email": "john.doe@example.com",
  "phone": "+1234567890",
  "isActive": true,
  "contact": {
    "address": "456 Oak Ave",
    "city": "San Francisco",
    "state": "CA",
    "country": "USA",
    "postalCode": "94102",
    "phone": "+14155551234",
    "email": ["john.doe@example.com"]
  },
  "job": {
    "jobTitle": "Senior Software Engineer",
    "companyName": "ACME Corp",
    "department": "Engineering",
    "employeeId": "EMP-001",
    "employeeType": "Full-Time",
    "employHireDate": "2023-01-15T00:00:00Z",
    "officeLocation": "San Francisco Office"
  }
}
```

**Response**: `204 No Content`

**Domain Event Published**: `ProfileUpdatedDomainEvent`

**Use Case**: Update all profile information in a single atomic operation.

#### Update Profile Picture
```http
PATCH /api/user/{id}/picture
Content-Type: application/json
Authorization: Bearer {token}
X-Tenant: {tenant-id}

{
  "id": "pic-123",
  "name": "profile.jpg",
  "target": "https://storage.example.com/users/john/profile.jpg"
}
```

**Response**: `204 No Content`

**Domain Event Published**: `UserPictureUpdatedDomainEvent`

**Note**: The `target` URL should point to a file uploaded to the FileStorage microservice or an external storage provider.

### gRPC Endpoints

The Users microservice exposes the following gRPC operations for inter-service communication:

#### AddTenantToUser
```protobuf
rpc AddTenantToUser (AddTenantRequest) returns (google.protobuf.Empty);

message AddTenantRequest {
  string Id = 1;
  Tenant Tenant = 2;
}

message Tenant {
  string Id = 1;
  string Name = 2;
}
```

**Use Case**: Called by other microservices to associate a user with a tenant when a user joins an organization.

#### AddGroupToUser (Add Role)
```protobuf
rpc AddGroupToUser (AddGroupRequest) returns (google.protobuf.Empty);

message AddGroupRequest {
  string Id = 1;
  string Role = 2;
}
```

**Use Case**: Called by RBAC or security microservices to grant roles to users.

### Error Responses

All errors follow RFC 7807 Problem Details format:

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "User not found.",
  "extensions": {
    "layer": "Application",
    "error_code": "USR-404",
    "traceId": "0HMVJ3K7S5Q2K:00000001"
  }
}
```

**Common Status Codes**:
- `200 OK` - Success with response body
- `204 No Content` - Success without response body
- `400 Bad Request` - Invalid input or business rule violation
- `401 Unauthorized` - Missing or invalid token
- `404 Not Found` - User not found
- `409 Conflict` - Tenant or role already exists
- `500 Internal Server Error` - Server error

## 🏗️ Domain Model

### UserAggregate (Root Entity)

#### What is it and what is it for?

The UserAggregate represents a person who uses the platform. It stores their identity (name, email, phone), their profile details (contact address, job information, profile picture), which organizations they belong to, and which roles they hold. It is the single source of truth for "who this person is" across the entire system, enabling multi-tenant access and role-based authorization.

**Properties**:
- `Id` (Guid) - Unique identifier
- `FirstName` (string) - User's first name (required)
- `LastName` (string) - User's last name (required)
- `Email` (string) - Primary email address (required)
- `Phone` (string) - Primary phone number (required)
- `DisplayName` (string?) - Optional display name (defaults to "FirstName LastName")
- `Picture` (UserPicture?) - Profile picture information
- `Tenants` (List<TenantEntity>) - Associated tenants/organizations
- `Roles` (string[]) - Assigned roles for authorization
- `Contact` (ContactInfo) - Contact information value object
- `Job` (JobInfo) - Job information value object
- `IsActive` (bool) - Account activation status
- `CreatedAt` (Instant) - Creation timestamp
- `UpdatedAt` (Instant?) - Last update timestamp
- `DeletedAt` (Instant?) - Soft deletion timestamp
- `IsDeleted` (bool) - Soft deletion flag

**Domain Methods**:
- `Create()` - Factory method to create a new user
- `Update()` - Update basic user information
- `UpdatePicture()` - Update profile picture
- `AddTenant()` - Associate user with a tenant
- `RemoveTenant()` - Dissociate user from a tenant
- `AddRole()` - Grant a role to the user
- `RemoveRole()` - Revoke a role from the user
- `UpdateContactInfo()` - Update contact information
- `UpdateJobInfo()` - Update job information
- `UpdateProfile()` - Update entire profile atomically
- `Delete()` - Soft delete the user

### Value Objects

#### ContactInfo
```csharp
public sealed class ContactInfo
{
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? Country { get; private set; }
    public string? PostalCode { get; private set; }
    public string? Phone { get; private set; }
    public string[] Email { get; private set; }
}
```

**Purpose**: Encapsulates user contact information including physical address and communication details.

#### JobInfo
```csharp
public sealed class JobInfo
{
    public string? JobTitle { get; private set; }
    public string? CompanyName { get; private set; }
    public string? Department { get; private set; }
    public string? EmployeeId { get; private set; }
    public string? EmployeeType { get; private set; }  // Full-Time, Part-Time, Contractor
    public Instant? EmployHireDate { get; private set; }
    public string? OfficeLocation { get; private set; }
}
```

**Purpose**: Encapsulates employment and job-related information.

#### UserPicture
```csharp
public sealed class UserPicture
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Target { get; private set; }  // URL to image
}
```

**Purpose**: Encapsulates profile picture metadata with reference to external storage.

### Entities

#### TenantEntity
```csharp
public class TenantEntity : IEntityBase
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}
```

**Purpose**: Represents a tenant/organization association within the user aggregate.

### Domain Events

The Users microservice publishes the following domain events to RabbitMQ:

- `UserUpdatedDomainEvent` - When basic user info is updated
- `UserPictureUpdatedDomainEvent` - When profile picture changes
- `TenantAddedDomainEvent` - When user is associated with a tenant
- `TenantRemovedDomainEvent` - When user is dissociated from a tenant
- `RoleAddedToUserDomainEvent` - When a role is granted
- `RoleRemovedToUserDomainEvent` - When a role is revoked
- `ContactInfoUpdatedDomainEvent` - When contact info changes
- `JobInfoUpdatedDomainEvent` - When job info changes
- `ProfileUpdatedDomainEvent` - When entire profile is updated
- `UserDeletedDomainEvent` - When user is deleted

**Event Structure Example**:
```json
{
  "aggregateId": "550e8400-e29b-41d4-a716-446655440000",
  "eventId": "event-123",
  "occurredAt": "2026-05-15T10:00:00Z",
  "eventType": "TenantAddedDomainEvent",
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "displayName": "John Doe",
  "tenant": {
    "id": "tenant-a-id",
    "name": "ACME Corporation"
  }
}
```

### Error Codes

The domain layer defines the following error codes:

| Code | Error Message |
|------|---------------|
| 100 | UnknownError |
| 101 | The first name is required. |
| 102 | The user ID is required. |
| 103 | The last name is required. |
| 104 | The email is required. |
| 105 | The phone number is required. |
| 106 | The roles are required. |
| 107 | The updated by ID is invalid. |
| 108 | The tenant already exists. |
| 109 | The tenant was not found. |
| 110 | The role already exists. |
| 111 | The address is required. |
| 112 | The image profile is required. |
| 113 | The role was not found. |

## ⚙️ Configuration

### Application Settings

Configure the microservice in `appsettings.json`:

```json
{
  "Core": {
    "Id": "3caf5d6a-31b7-42bb-8c02-ef54298a9060",
    "PathBase": "/ms-users",
    "AppName": "ms-users",
    "TypeEntryPoint": "rest",
    "Version": "v1",
    "Description": "This microservice management the user",
    "Business": "CodeDesignPlus",
    "Contact": {
      "Name": "CodeDesigPlus",
      "Email": "codedesignplus@outlook.com"
    }
  },
  "Security": {
    "Authority": "https://your-identity-server.com",
    "ValidateAudience": true,
    "ValidateIssuer": true,
    "ValidateLifetime": true,
    "RequireHttpsMetadata": true,
    "ValidIssuers": [],
    "ValidAudiences": ["ms-users"]
  },
  "Mongo": {
    "Enable": true,
    "Database": "db-ms-users",
    "Diagnostic": {
      "Enable": false,
      "EnableCommandText": false
    }
  },
  "Redis": {
    "Instances": {
      "Core": {
        "ConnectionString": "localhost:6379"
      }
    }
  },
  "RedisCache": {
    "Enable": true,
    "Expiration": "00:05:00"
  },
  "RabbitMQ": {
    "Enable": true,
    "Host": "localhost",
    "Port": 5672,
    "UserName": "user",
    "Password": "pass",
    "EnableDiagnostic": false
  },
  "Vault": {
    "Enable": true,
    "Address": "http://localhost:8200",
    "AppName": "ms-users",
    "Solution": "security-codedesignplus",
    "Token": "root",
    "Mongo": {
      "Enable": true,
      "TemplateConnectionString": "mongodb://{0}:{1}@localhost:27017"
    },
    "RabbitMQ": {
      "Enable": true
    }
  },
  "Observability": {
    "Enable": true,
    "ServerOtel": "http://localhost:4317",
    "Trace": {
      "Enable": true,
      "AspNetCore": true,
      "CodeDesignPlusSdk": true,
      "Redis": true,
      "RabbitMQ": true
    },
    "Metrics": {
      "Enable": true,
      "AspNetCore": true
    }
  }
}
```

### Multi-tenancy Configuration

The microservice supports multi-tenancy through the `X-Tenant` header. Each request must include a tenant ID:

```http
X-Tenant: 9588813a-7bc0-4be4-a169-293061881cc3
```

Users can belong to multiple tenants, allowing cross-tenant access scenarios.

### Environment Variables

Key environment variables for Docker deployment:

```bash
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://+:5000
MONGO_CONNECTION_STRING=mongodb://mongo:27017
REDIS_CONNECTION_STRING=redis:6379
RABBITMQ_HOST=rabbitmq
VAULT_ADDRESS=http://vault:8200
VAULT_TOKEN=your-vault-token
```

## 🎯 Use Cases & Scenarios

### 1. User Registration & Onboarding
Create a new user account during application sign-up:

```bash
# Step 1: Create user account
POST /api/user
- Id: generated GUID
- FirstName: "Sarah"
- LastName: "Johnson"
- Email: "sarah.johnson@example.com"
- Phone: "+14155551234"
- IsActive: true

Response: 204 No Content

# Step 2: Add user to primary tenant
POST /api/user/{id}/tenant
- TenantId: organization-id
- Name: "ACME Corporation"

# Step 3: Assign initial roles
POST /api/user/{id}/role
- Role: "User"

# Result: User is ready to access the system
```

### 2. Employee Profile Management
HR system updates employee profile information:

```bash
# Update complete profile including contact and job info
PUT /api/user/{id}/profile
{
  "firstName": "Sarah",
  "lastName": "Johnson",
  "displayName": "Sarah J.",
  "email": "sarah.johnson@acme.com",
  "phone": "+14155551234",
  "isActive": true,
  "contact": {
    "address": "123 Market St",
    "city": "San Francisco",
    "state": "CA",
    "country": "USA",
    "postalCode": "94103",
    "phone": "+14155551234",
    "email": ["sarah.johnson@acme.com", "sjohnson@personal.com"]
  },
  "job": {
    "jobTitle": "Product Manager",
    "companyName": "ACME Corporation",
    "department": "Product",
    "employeeId": "EMP-1001",
    "employeeType": "Full-Time",
    "employHireDate": "2024-03-01T00:00:00Z",
    "officeLocation": "San Francisco HQ"
  }
}

# Domain event published: ProfileUpdatedDomainEvent
# Consumed by: HR system, directory service
```

### 3. Multi-Tenant User Access
User joins multiple organizations:

```bash
# User exists in Tenant A
Tenants: ["tenant-a-id"]

# User joins Tenant B (freelance work)
POST /api/user/{id}/tenant
{
  "tenantId": "tenant-b-id",
  "name": "Beta Startup"
}

# User now has access to both tenants
Tenants: ["tenant-a-id", "tenant-b-id"]

# Frontend switches between tenants via X-Tenant header
GET /api/someresource
Headers: X-Tenant: tenant-a-id  // Access Tenant A resources

GET /api/someresource
Headers: X-Tenant: tenant-b-id  // Access Tenant B resources
```

### 4. Role-Based Access Control (RBAC)
Manage user permissions through roles:

```bash
# Initial user has basic role
Roles: ["User"]

# Promote to administrator
POST /api/user/{id}/role
{
  "role": "Admin"
}

Roles: ["User", "Admin"]

# Revoke admin privileges
DELETE /api/user/{id}/role/Admin

Roles: ["User"]

# RBAC microservice listens to role change events
Event: RoleAddedToUserDomainEvent
Event: RoleRemovedToUserDomainEvent
```

### 5. User Deactivation & Deletion
Temporarily deactivate or permanently delete user:

```bash
# Temporary deactivation
PUT /api/user/{id}
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phone": "+1234567890",
  "isActive": false  // Deactivate
}

# User cannot log in but data is preserved

# Permanent deletion (soft delete)
DELETE /api/user/{id}

# User is marked as deleted
IsDeleted: true
IsActive: false
DeletedAt: "2026-05-15T10:00:00Z"

# Data is preserved for audit and compliance
```

### 6. Profile Picture Management
Upload and update user profile images:

```bash
# Step 1: Upload image to FileStorage microservice
POST https://filestorage.example.com/api/file
Content-Type: multipart/form-data
File: profile.jpg

Response: {
  "fileId": "file-123",
  "url": "https://cdn.example.com/users/john/profile.jpg"
}

# Step 2: Update user profile picture
PATCH /api/user/{id}/picture
{
  "id": "file-123",
  "name": "profile.jpg",
  "target": "https://cdn.example.com/users/john/profile.jpg"
}

# Frontend displays updated picture
```

### 7. Inter-Service Communication (gRPC)
Other microservices interact via gRPC:

```csharp
// Notification microservice calls Users gRPC to add tenant
var client = new Users.UsersClient(channel);

var request = new AddTenantRequest
{
    Id = userId,
    Tenant = new Tenant
    {
        Id = tenantId,
        Name = "New Organization"
    }
};

await client.AddTenantToUserAsync(request);

// Synchronous, type-safe inter-service call
```

### 8. Event-Driven User Creation
AsyncWorker consumes user creation events:

```bash
# External system publishes UserCreatedDomainEvent to RabbitMQ
{
  "aggregateId": "user-123",
  "firstName": "Alice",
  "lastName": "Smith",
  "displayName": "Alice S.",
  "email": "alice.smith@example.com",
  "phone": "+14155551234",
  "isActive": true
}

# AsyncWorker receives event
CreateUserHandler.HandleAsync(event)

# Creates user in Users database
CreateUserCommand → UserAggregate.Create()

# Decoupled user creation from source system
```

## 🏗️ Architecture

### Clean Architecture Layers

```
src/
├── domain/                          # Domain Layer
│   ├── Domain/                      # Aggregates, Entities, Value Objects
│   │   ├── UserAggregate.cs        # Main aggregate root
│   │   ├── Entities/               # TenantEntity
│   │   ├── ValueObjects/           # ContactInfo, JobInfo, UserPicture
│   │   ├── DomainEvents/           # 11 domain events
│   │   ├── Repositories/           # IUserRepository
│   │   └── Errors.cs               # Error code constants
│   ├── Application/                 # Application Layer
│   │   ├── Commands/               # 11 write commands
│   │   │   ├── CreateUser/
│   │   │   ├── UpdateUser/
│   │   │   ├── DeleteUser/
│   │   │   ├── AddTenant/
│   │   │   ├── RemoveTenant/
│   │   │   ├── AddRole/
│   │   │   ├── RemoveRole/
│   │   │   ├── UpdateContact/
│   │   │   ├── UpdateJob/
│   │   │   ├── UpdateProfile/
│   │   │   └── UpdatePicture/
│   │   ├── Queries/                # 2 read queries
│   │   │   ├── GetUsersById/
│   │   │   └── GetAllUsers/
│   │   ├── DTOs/                   # UserDto, TenantDto
│   │   └── Validators/             # FluentValidation rules
│   └── Infrastructure/              # Infrastructure Layer
│       ├── Repositories/           # MongoDB implementation
│       └── Services/               # External service adapters
└── entrypoints/                     # Presentation Layer
    ├── Rest/                        # REST API
    │   ├── Controllers/            # UserController
    │   └── Program.cs              # Startup configuration
    ├── gRpc/                        # gRPC API
    │   ├── Services/               # UserService
    │   └── Protos/                 # users.proto
    └── AsyncWorker/                 # Background jobs
        └── Consumers/              # CreateUserHandler, CompleteOrderHandler
```

### CQRS Pattern

**Commands** (Write operations):
- `CreateUserCommand` - Create new user
- `UpdateUserCommand` - Update basic user info
- `DeleteUserCommand` - Soft delete user
- `AddTenantCommand` - Associate user with tenant
- `RemoveTenantCommand` - Dissociate user from tenant
- `AddRoleCommand` - Grant role to user
- `RemoveRoleCommand` - Revoke role from user
- `UpdateContactCommand` - Update contact information
- `UpdateJobCommand` - Update job information
- `UpdateProfileCommand` - Update entire profile
- `UpdatePictureCommand` - Update profile picture

**Queries** (Read operations):
- `GetUsersByIdQuery` - Get user by ID
- `GetAllUsersQuery` - List users with pagination and filtering

### Domain Events

Published to RabbitMQ after successful operations:
- `UserUpdatedDomainEvent` - User info changed
- `UserPictureUpdatedDomainEvent` - Profile picture updated
- `TenantAddedDomainEvent` - User joined tenant
- `TenantRemovedDomainEvent` - User left tenant
- `RoleAddedToUserDomainEvent` - Role granted
- `RoleRemovedToUserDomainEvent` - Role revoked
- `ContactInfoUpdatedDomainEvent` - Contact info changed
- `JobInfoUpdatedDomainEvent` - Job info changed
- `ProfileUpdatedDomainEvent` - Entire profile updated
- `UserDeletedDomainEvent` - User deleted

**Event Consumers**:
- Identity service (sync user claims)
- RBAC service (update permissions)
- Notification service (send welcome emails)
- Audit service (log user changes)
- Analytics service (track user metrics)

## 🧪 Testing

### Unit & Integration Tests
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/unit/CodeDesignPlus.Net.Microservice.Users.Domain.Test

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverageReportsDirectory=./coverage
```

### Test Structure

```
tests/
├── unit/                                # Unit tests
│   ├── Domain.Test/                     # Aggregate and value object tests
│   ├── Application.Test/                # Command/query handler tests
│   ├── Rest.Test/                       # Controller tests
│   ├── gRpc.Test/                       # gRPC service tests
│   └── AsyncWorker.Test/                # Event handler tests
└── integration/                         # Integration tests
    ├── Rest.Test/                       # Full REST API integration tests
    ├── gRpc.Test/                       # Full gRPC API integration tests
    └── AsyncWorker.Test/                # Event processing integration tests
```

### Manual Testing with Postman

Import the Postman collection from `docs/postman/` for manual testing.

### Testing Multi-Tenancy

```bash
# Create user in Tenant A
POST /api/user
Headers: X-Tenant: tenant-a-id

# Verify isolation: Cannot access from Tenant B
GET /api/user/{id}
Headers: X-Tenant: tenant-b-id
Response: 404 Not Found (tenant isolation enforced)

# Add user to Tenant B
POST /api/user/{id}/tenant
Headers: X-Tenant: tenant-a-id
Body: { "tenantId": "tenant-b-id", "name": "Tenant B" }

# Now accessible from Tenant B
GET /api/user/{id}
Headers: X-Tenant: tenant-b-id
Response: 200 OK
```

## 💡 Best Practices

### User Management

#### ✅ DO: Validate email uniqueness at application level
```csharp
// Check if email is already in use before creating user
var existingUser = await repository.FindByEmailAsync(email);
DomainGuard.IsNotNull(existingUser, Errors.EmailAlreadyExists);
```

#### ✅ DO: Use display names for UI
```csharp
// Allow users to customize display name
var user = UserAggregate.Create(
    id: Guid.NewGuid(),
    firstName: "Jonathan",
    lastName: "Doe",
    displayName: "Johnny",  // User-friendly name for UI
    email: "jonathan.doe@example.com",
    phone: "+1234567890",
    isActive: true
);
```

#### ✅ DO: Handle soft deletion properly
```csharp
// Soft delete preserves data for audit and compliance
user.Delete(deletedBy: adminUserId);

// Query only active users by default
var activeUsers = await repository.FindAsync(
    criteria: new Criteria { Filter = "isDeleted eq false" }
);
```

#### ✅ DO: Track who made changes
```csharp
// All update operations require updatedBy parameter
user.Update(
    firstName: "John",
    lastName: "Doe",
    email: "john.doe@example.com",
    phone: "+1234567890",
    displayName: "John Doe",
    isActive: true,
    updatedBy: currentUserId  // Audit trail
);
```

#### ❌ DON'T: Store sensitive authentication data
```csharp
// Bad: Never store passwords or tokens in Users microservice
public class User
{
    public string PasswordHash { get; set; }  // ❌ Wrong service
    public string RefreshToken { get; set; }   // ❌ Identity service responsibility
}

// Good: Only store identity metadata
public class UserAggregate
{
    public string Email { get; private set; }  // ✅ Identity reference
    public bool IsActive { get; private set; }  // ✅ Account status
}
```

#### ❌ DON'T: Allow duplicate tenants or roles
```csharp
// Bad: Adding same tenant twice
user.AddTenant(tenantId, name, updatedBy);
user.AddTenant(tenantId, name, updatedBy);  // ❌ Should fail

// Good: Domain guards prevent duplicates
public void AddTenant(Guid tenantId, string name, Guid updateBy)
{
    DomainGuard.IsTrue(
        Tenants.Any(t => t.Id == tenantId),
        Errors.TenantAlreadyExists  // ✅ Validation
    );
    
    Tenants.Add(new TenantEntity { Id = tenantId, Name = name });
}
```

### Multi-Tenancy

1. **Always include X-Tenant header** in requests
2. **Validate tenant access** at the application layer
3. **Isolate queries** by tenant at the repository level
4. **Use tenant-aware indexes** in MongoDB for performance
5. **Audit tenant associations** for security compliance

### Profile Management

```csharp
// Use UpdateProfile for atomic profile updates
user.UpdateProfile(
    firstName: "John",
    lastName: "Doe",
    email: "john.doe@example.com",
    phone: "+1234567890",
    displayName: "John Doe",
    isActive: true,
    contact: contactInfo,  // Atomic update
    job: jobInfo,          // Atomic update
    updatedBy: currentUserId
);

// Publishes single ProfileUpdatedDomainEvent
// Better than multiple partial updates
```

## 🐛 Troubleshooting

### Common Issues

#### Issue: User not found after creation
**Cause**: Tenant isolation is enforced. User created in different tenant than query.

**Solution**:
```bash
# Ensure X-Tenant header matches during creation and retrieval
POST /api/user
Headers: X-Tenant: tenant-a-id

GET /api/user/{id}
Headers: X-Tenant: tenant-a-id  # Must match
```

#### Issue: "The tenant already exists" error
**Cause**: Attempting to add a tenant that's already associated with the user.

**Solution**:
```bash
# Check existing tenants before adding
GET /api/user/{id}

# Verify tenant is not in user.tenants array
# Remove tenant first if updating:
DELETE /api/user/{id}/tenant/{tenantId}
POST /api/user/{id}/tenant
```

#### Issue: Domain event not received by consumers
**Cause**: RabbitMQ not running or consumer not registered.

**Solution**:
```bash
# Check RabbitMQ status
docker ps | grep rabbitmq

# Verify queue exists
curl -u guest:guest http://localhost:15672/api/queues

# Check consumer registration in AsyncWorker
[QueueName<UserAggregate>("CreateUserHandler")]
public class CreateUserHandler : IEventHandler<UserCreatedDomainEvent>
```

#### Issue: MongoDB connection timeout
**Cause**: MongoDB not accessible or wrong connection string.

**Solution**:
```bash
# Test MongoDB connectivity
mongosh "mongodb://localhost:27017"

# Check Docker containers
docker ps | grep mongo

# Verify connection string in appsettings
"Mongo": {
  "Enable": true,
  "Database": "db-ms-users"
}
```

#### Issue: Cannot update user - validation errors
**Cause**: Missing required fields or invalid data.

**Solution**:
```csharp
// All required fields must be provided
PUT /api/user/{id}
{
  "firstName": "John",        // Required
  "lastName": "Doe",          // Required
  "email": "john@example.com",// Required
  "phone": "+1234567890",     // Required
  "displayName": "John Doe",  // Optional but recommended
  "isActive": true            // Required
}
```

### Debug Mode

Enable detailed logging in `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "CodeDesignPlus": "Trace",
      "CodeDesignPlus.Net.Microservice.Users": "Trace",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Health Checks

Check service health:
```bash
curl http://localhost:5000/health
```

Expected response:
```json
{
  "status": "Healthy",
  "checks": [
    { "name": "MongoDB", "status": "Healthy" },
    { "name": "Redis", "status": "Healthy" },
    { "name": "RabbitMQ", "status": "Healthy" }
  ]
}
```

## 🔒 Security

### Authentication & Authorization

- **OAuth2/OpenID Connect**: JWT Bearer token authentication
- **RBAC Integration**: Role-based access control via roles array
- **Tenant Isolation**: Multi-tenancy enforced at data and API level
- **Audit Trail**: Track who created/updated/deleted users

### Data Protection

- **Encryption at Rest**: MongoDB encryption for sensitive data
- **Encryption in Transit**: HTTPS/TLS for all API communication
- **PII Handling**: Personal Identifiable Information (email, phone) protected
- **GDPR Compliance**: Soft deletion allows data retention policies

### Best Practices

1. **Never store passwords**: Authentication is the responsibility of Identity/Auth services
2. **Validate tenant access**: Ensure users can only access their tenant's data
3. **Audit all changes**: Log updatedBy for compliance and security investigations
4. **Rotate API credentials**: Regularly update JWT signing keys and database credentials
5. **Monitor for suspicious activity**: Track failed auth attempts and unusual data access
6. **Implement rate limiting**: Prevent brute force and DoS attacks
7. **Use secure headers**: Enable HSTS, CSP, and other security headers

## 🔄 Multi-Tenancy

### Understanding Multi-Tenancy

Users in this microservice support **multi-tenant associations**. A single user can:
- Belong to multiple tenants/organizations
- Have different roles per tenant (handled by RBAC service)
- Access resources from any associated tenant

### Tenant Isolation

```
User: john@example.com
Tenants: ["tenant-a-id", "tenant-b-id"]

Frontend switches context:
- X-Tenant: tenant-a-id → Access Tenant A resources
- X-Tenant: tenant-b-id → Access Tenant B resources

Repository enforces tenant filtering automatically.
```

### Adding Tenants

```bash
# User registers with primary tenant during sign-up
POST /api/user
Headers: X-Tenant: tenant-a-id

# User joins additional tenant later (e.g., freelance work)
POST /api/user/{id}/tenant
Headers: X-Tenant: tenant-a-id
Body: { "tenantId": "tenant-b-id", "name": "Beta Corp" }

# Domain event published: TenantAddedDomainEvent
# Consumed by: RBAC (assign default roles), Analytics, etc.
```

### Removing Tenants

```bash
# User leaves organization
DELETE /api/user/{id}/tenant/{tenantId}

# Domain event published: TenantRemovedDomainEvent
# Consumed by: RBAC (revoke tenant roles), cleanup services
```

## 📱 Profile Management

### Complete Profile Update

Use `PUT /api/user/{id}/profile` for atomic profile updates:

```json
{
  "firstName": "Sarah",
  "lastName": "Johnson",
  "displayName": "Sarah J.",
  "email": "sarah.johnson@acme.com",
  "phone": "+14155551234",
  "isActive": true,
  "contact": {
    "address": "123 Market St",
    "city": "San Francisco",
    "state": "CA",
    "country": "USA",
    "postalCode": "94103",
    "phone": "+14155551234",
    "email": ["sarah.johnson@acme.com"]
  },
  "job": {
    "jobTitle": "Product Manager",
    "companyName": "ACME Corp",
    "department": "Product",
    "employeeId": "EMP-1001",
    "employeeType": "Full-Time",
    "employHireDate": "2024-03-01T00:00:00Z",
    "officeLocation": "San Francisco HQ"
  }
}
```

**Advantages**:
- Single atomic operation
- One domain event published
- Consistent state guaranteed
- Better performance than multiple PATCH requests

### Partial Profile Updates

Use PATCH endpoints for specific sections:

```bash
# Update only contact info
PATCH /api/user/{id}/contact

# Update only job info
PATCH /api/user/{id}/job

# Update only profile picture
PATCH /api/user/{id}/picture
```

**Use Case**: Frontend forms with separate tabs/sections.

## ❓ FAQ

### General Questions

**Q: What's the difference between Users and Identity/Auth microservices?**
A: Users microservice manages user **profiles and metadata** (name, email, phone, tenants, roles). Identity/Auth service handles **authentication** (login, passwords, tokens). Users stores the "who they are", Auth handles "how they prove it".

**Q: Can a user belong to multiple tenants?**
A: Yes! Users support multi-tenant associations. A single user can belong to many organizations and switch context via the `X-Tenant` header.

**Q: How do roles work with RBAC?**
A: Users microservice stores role **assignments** (which roles the user has). RBAC microservice defines role **definitions** (what permissions each role grants). Use `AddRole`/`RemoveRole` to manage assignments.

**Q: What happens when a user is deleted?**
A: Users are **soft deleted**. `IsDeleted` flag is set to `true`, `IsActive` set to `false`, and `DeletedAt` timestamp recorded. Data is preserved for audit and compliance.

**Q: How do I search for users by email?**
A: Use OData filter syntax: `GET /api/user?filter=email eq 'john@example.com'`

### Technical Questions

**Q: Why separate ContactInfo and JobInfo from User entity?**
A: They are **Value Objects** in DDD. They encapsulate related data, are immutable, and can be updated atomically. This provides better domain modeling and consistency.

**Q: How does profile picture storage work?**
A: Profile pictures are stored in a **FileStorage microservice** or external CDN. Users microservice only stores the **reference URL** in `UserPicture` value object.

**Q: What's the UpdateProfile command for?**
A: It updates **all profile information atomically** (basic info + contact + job) in a single operation. Use it for "Save Profile" forms that update everything at once.

**Q: How are domain events consumed?**
A: Published to **RabbitMQ**. AsyncWorker microservices subscribe to queues. For example, Identity service subscribes to `UserUpdatedDomainEvent` to sync user claims.

**Q: Can I customize the UserAggregate?**
A: Yes! The aggregate can be extended with additional properties, value objects, or methods. Follow DDD principles and maintain invariants through domain methods.

### Troubleshooting Questions

**Q: Why do I get 404 when querying a user I just created?**
A: Likely a **tenant mismatch**. Ensure the `X-Tenant` header matches between creation and query. Users are isolated by tenant.

**Q: How do I prevent duplicate emails across tenants?**
A: Implement a **unique email check** at the application layer before creating users. MongoDB unique indexes can also enforce this at the database level.

**Q: Why is my role not taking effect?**
A: Roles in Users microservice are **metadata**. Authorization enforcement happens in the **RBAC microservice** or API Gateway. Ensure role definitions exist in RBAC.

**Q: How do I debug domain event publishing?**
A: Enable RabbitMQ diagnostics: `"RabbitMQ": { "EnableDiagnostic": true }`. Check RabbitMQ management UI at `http://localhost:15672` for queue status.

## 🤝 Contributing

We welcome contributions! Please follow these guidelines:

### Development Workflow

1. **Fork the repository**
2. **Create a feature branch**
   ```bash
   git checkout -b feature/add-user-preferences
   ```

3. **Make your changes**
   - Follow existing code style
   - Add tests for new features
   - Update documentation

4. **Run tests**
   ```bash
   dotnet test
   ```

5. **Commit with conventional commits**
   ```bash
   git commit -m "feat: add user preferences value object"
   git commit -m "fix: resolve tenant isolation issue"
   git commit -m "docs: update profile management section"
   ```

6. **Push and create Pull Request**
   ```bash
   git push origin feature/add-user-preferences
   ```

### Code Standards

- **C# Coding Style**: Follow .editorconfig rules
- **Test Coverage**: Aim for >80% coverage
- **Documentation**: Update README.md for new features
- **Naming Conventions**:
  - Commands: `{Action}Command` (e.g., `UpdateContactCommand`)
  - Queries: `{Action}Query` (e.g., `GetUsersByIdQuery`)
  - Handlers: `{CommandOrQuery}Handler`
  - Tests: `{MethodName}_{Scenario}_{ExpectedResult}`
  - Domain Events: `{Action}DomainEvent` (e.g., `TenantAddedDomainEvent`)

### Pull Request Checklist

- [ ] Code compiles without warnings
- [ ] All tests pass
- [ ] New features have tests
- [ ] Documentation updated
- [ ] CHANGELOG.md updated (if applicable)
- [ ] No breaking changes (or documented with migration guide)
- [ ] Follows SOLID principles and Clean Architecture
- [ ] Domain invariants are protected by domain guards

## 📞 Support & Resources

### Getting Help

- **GitHub Issues**: [Report bugs or request features](https://github.com/codedesignplus/CodeDesignPlus.Net.Microservice.Users/issues)
- **Discussions**: [Ask questions and share ideas](https://github.com/codedesignplus/CodeDesignPlus.Net.Microservice.Users/discussions)
- **Documentation**: [CodeDesignPlus Docs](https://codedesignplus.github.io/)
- **Email**: support@codedesignplus.com

### Related Projects

- **CodeDesignPlus.Net.Sdk**: Core SDK with shared abstractions
- **CodeDesignPlus.Environment.Dev**: Local development environment setup
- **Template Repository**: Microservice scaffolding template

## 📄 License

This project is licensed under the **GNU Lesser General Public License v3.0** - see the [LICENSE.md](LICENSE.md) file for details.

### What This Means

- ✅ **Commercial use**: Use in commercial applications
- ✅ **Modification**: Modify the source code
- ✅ **Distribution**: Distribute the software
- ✅ **Private use**: Use privately
- ⚠️ **Disclose source**: Must disclose source for derivative works
- ⚠️ **License and copyright notice**: Include license and copyright
- ⚠️ **Same license**: Derivative works must use LGPL v3.0

## 🙏 Acknowledgments

Built with:
- **CodeDesignPlus SDK** - Core abstractions and utilities
- **.NET 9** - Microsoft's modern development platform
- **MongoDB** - Flexible document database for user profiles
- **RabbitMQ** - Reliable message broker for domain events
- **Redis** - High-performance caching layer
- **Open Source Community** - For all the amazing tools and libraries

---

**Made with care by CodeDesignPlus**

*For questions, suggestions, or contributions, please open an issue or pull request.*
