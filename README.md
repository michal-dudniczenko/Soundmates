# 🎵 Soundmates API

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-4169E1?logo=postgresql&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker&logoColor=white)

A social matching platform backend API designed for musicians and bands to connect. Find your next bandmate or discover bands looking for artists with similar musical interests.

---

## 📋 Table of Contents

- [Introduction](#-introduction)
- [Tech Stack](#-tech-stack)
- [Architecture](#-architecture)
- [Key Features](#-key-features)
- [Prerequisites](#prerequisites)
- [Configuration](#configuration)
- [Running the App](#running-the-app)
- [API Documentation](#-api-documentation)
- [API Tutorial](#-api-tutorial)

---

## 🎯 Introduction

**Soundmates** is a RESTful API backend for a musician-matching platform. It enables individual artists and bands to create profiles, browse potential matches based on configurable preferences (genre, location, age), and communicate with mutual matches via real-time messaging.

The platform solves the common problem musicians face: finding like-minded collaborators in their area with compatible musical styles and goals.

---

## 🛠 Tech Stack

| Category | Technology |
|----------|------------|
| **Language** | C# |
| **Framework** | ASP.NET Core |
| **Database** | PostgreSQL |
| **ORM** | Entity Framework Core |
| **Authentication** | JWT Bearer Tokens |
| **Real-Time Communication** | SignalR |
| **CQRS Pattern** | MediatR |
| **API Documentation** | Swagger / OpenAPI |
| **Email Service** | MailKit |
| **Containerization** | Docker & Docker Compose |
| **Testing** | xUnit, Moq |

---

## 🏗 Architecture

This project follows **Clean Architecture** principles with a **CQRS (Command Query Responsibility Segregation)** pattern powered by MediatR.

```
Soundmates/
├── src/
│   ├── Soundmates.Api/           # Presentation Layer
│   ├── Soundmates.Application/   # Application Layer (Use Cases)
│   ├── Soundmates.Domain/        # Domain Layer (Core Business Logic)
│   └── Soundmates.Infrastructure/# Infrastructure Layer (Data Access, External Services)
└── tests/
    └── Soundmates.Tests/         # Automated tests
```

### Layer Responsibilities

| Layer | Responsibility |
|-------|----------------|
| **Domain** | Contains domain entities (`User`, `Artist`, `Band`, `Match`, `Message`, etc.), interfaces, and business constants. No external dependencies. |
| **Application** | Implements use cases using CQRS pattern. Contains commands, queries, handlers, validators, and DTOs. Orchestrates business logic. |
| **Infrastructure** | Implements repository interfaces, database context (EF Core), external services (email, authentication), SignalR hub, and data seeding. |
| **API** | Entry point with controllers, middleware, request/response mappings, and API configuration (Swagger, JWT, CORS). |

---

## ✨ Key Features

- **🔐 JWT Authentication** - Secure token-based auth with access tokens (30 min) and refresh tokens (30 days)
- **👥 User Profiles** - Support for both individual artists and bands
- **💘 Smart Matching** - Like/Dislike system with configurable preferences (location, age, genre tags)
- **💬 Real-Time Messaging** - SignalR-powered chat between matched users
- **🎵 Music Samples** - Upload and manage audio samples (MP3/MP4, up to 100MB)
- **📸 Profile Pictures** - Image upload support (JPEG, up to 5MB)
- **🔍 Advanced Filtering** - Filter potential matches by distance, gender, band size, and tags
- **📧 Email Notifications** - SMTP integration via MailKit
- **🚨 User Reporting** - Report inappropriate profiles
- **📖 Data Dictionaries** - Centralized lookups for countries, cities, genders, and tags

---

### Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (recommended)
- [.NET SDK 9.0](https://dotnet.microsoft.com/download/dotnet/9.0) (for local development)

### Configuration

The application requires the following configuration. When running with Docker Compose, environment variables are pre-configured. For local development, update `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=Soundmates;Username=Soundmates;Password=Soundmates"
  },
  "SecretKey": "your_super_secret_key_that_is_at_least_32_characters_long",
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "Port": 587,
    "SenderEmail": "your-email@gmail.com",
    "Password": "your-app-password"
  }
}
```

| Setting | Description |
|---------|-------------|
| `ConnectionStrings:DefaultConnection` | PostgreSQL connection string |
| `SecretKey` | JWT signing key (min 32 characters) |
| `EmailSettings` | SMTP configuration for email notifications |

### Running the App

#### 🐳 Docker (Recommended)

```bash
# Build and start all services
docker compose up --build -d

# Stop the application
docker compose down
```

---

## 📚 API Documentation

Once the application is running, interactive API documentation is available:

| Resource | URL |
|----------|-----|
| **Swagger UI** | [http://localhost:5000/swagger](http://localhost:5000/swagger) |
| **OpenAPI JSON** | [http://localhost:5000/openapi/soundmates.json](http://localhost:5000/openapi/soundmates.json) |

### Database Management

**Adminer** is included for database administration:

| Setting | Value |
|---------|-------|
| **URL** | [http://localhost:8080](http://localhost:8080) |
| **System** | PostgreSQL |
| **Server** | `db` |
| **Username** | `Soundmates` |
| **Password** | `Soundmates` |
| **Database** | `Soundmates` |

---

## 📖 API Tutorial

### Authentication Flow

1. **Register** - `POST /auth/register` with email and password
2. **Login** - `POST /auth/login` to receive access token (30 min) and refresh token (30 days)
3. **Access protected endpoints** - Include `Authorization: Bearer <access_token>` header
4. **Refresh tokens** - `POST /auth/refresh` when access token expires
5. **Logout** - `POST /auth/logout` to invalidate refresh token

### Profile Setup

After registration, the profile is **inactive**. Complete profile setup via `PUT /users/profile` to unlock full functionality.

### Matching Workflow

1. **Configure preferences** - `PUT /matching/match-preference` to set filters
2. **Browse artists** - `GET /matching/artists` returns potential artist matches
3. **Browse bands** - `GET /matching/bands` returns potential band matches
4. **Like/Dislike** - `POST /matching/like` or `POST /matching/dislike`
5. **Mutual match** - When both users like each other, a match is created
6. **Chat** - Send messages via `POST /messages` to matched users

### Validation Rules

All constraints are defined in [`AppConstants.cs`](./api/Soundmates/src/Soundmates.Domain/Constants/AppConstants.cs):

| Rule | Value |
|------|-------|
| Password length | 8-32 characters |
| Max music samples | 5 per user |
| Max sample size | 100 MB |
| Max profile pictures | 5 per user |
| Max image size | 5 MB |
| Message length | Up to 4000 characters |

**Password requirements:**
- Lowercase letter
- Uppercase letter
- Digit
- Special character
- Standard printable ASCII characters only
