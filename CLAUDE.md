# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a .NET 9.0 solution for IMAR Dialogo Operatore, a Blazor Server application for operator dialogue systems in manufacturing environments. The project follows Clean Architecture principles with separate layers for Domain, Application, Infrastructure, and Presentation.

## Architecture

The solution consists of 5 main projects:

- **IMAR_DialogoOperatoreMockup**: Blazor Server web application (main UI project)
- **IMAR_DialogoOperatore.Domain**: Domain entities and models
- **IMAR_DialogoOperatore.Application**: Application services and DTOs  
- **IMAR_DialogoOperatore.Infrastructure**: Data access, external API clients, and infrastructure services
- **GeneratoreTimbratureTeamSystem**: Console application for generating timesheets
- **IMAR_DialogoOperatore.Test**: xUnit test project

### Key Technologies

- **.NET 9.0** with nullable reference types enabled
- **Blazor Server** with DevExpress components for UI
- **Entity Framework Core 9.0** for SQL Server data access
- **Dapper** for optimized database queries
- **RestSharp** for HTTP API calls
- **System.Data.Odbc** for AS/400 connectivity
- **xUnit**, **NSubstitute**, and **Coverlet** for testing

### Data Sources

The application integrates with multiple data sources:
- **SynergyJMES**: Primary SQL Server database
- **AS/400 (IBM i)**: Legacy system via ODBC
- **Imar_Produzione**: Production database
- **Imar_Schedulatore**: Scheduler database  
- **Imar_Connect**: Integration database

## Development Commands

### Building the Solution
```bash
dotnet build IMAR_DialogoOperatore.sln
```

### Running the Web Application
```bash
dotnet run --project IMAR_DialogoOperatoreMockup
```

### Running Tests
```bash
dotnet test IMAR_DialogoOperatore.Test
```

### Running Individual Projects
- Main web app: `dotnet run --project IMAR_DialogoOperatoreMockup`
- Console app: `dotnet run --project GeneratoreTimbratureTeamSystem`

### Package Management
```bash
dotnet restore
dotnet add package [PackageName]
```

## Project Structure

### Domain Layer (IMAR_DialogoOperatore.Domain)
- `Entities/`: Domain entities organized by data source (As400, Imar_Connect, etc.)
- `Models/`: Domain models and value objects

### Application Layer (IMAR_DialogoOperatore.Application)
- `DTOs/`: Data Transfer Objects
- `Interfaces/`: Service contracts and repository interfaces

### Infrastructure Layer (IMAR_DialogoOperatore.Infrastructure)
- `Services/`: Business logic services (AttivitaService, MacchinaService, etc.)
- Data access folders organized by source system:
  - `As400/`: AS/400 legacy system integration
  - `Imar_Connect/`: Integration database access
  - `Imar_Produzione/`: Production database access
  - `Imar_Schedulatore/`: Scheduler database access
  - `JMes/`: JMes API integration
  - `ImarApi/`: Internal API services
- `Mappers/`: Object mapping utilities
- `Utilities/`: Infrastructure utilities

### Presentation Layer (IMAR_DialogoOperatoreMockup)
- `Components/`: Blazor components and pages
- `ViewModels/`: View models for UI binding
- `Commands/`: Command patterns for UI actions
- `Helpers/`: UI helper classes
- `Observers/`: Observer pattern implementations

## Configuration

### Connection Strings
The application uses multiple connection strings defined in `appsettings.json`:
- `SynergyJmesCrescenzi`: Main database
- `As400`: IBM AS/400 system
- `ImarProduzione`: Production database
- `imarSchedulatore`: Scheduler database
- `imarConnect`: Integration database

### Key Configuration Sections
- Database connections for multiple systems
- File paths for TeamSystem integration
- Logging configuration

## Testing

- Test framework: **xUnit**
- Mocking: **NSubstitute** 
- Code coverage: **Coverlet**
- Test project includes utilities and command tests

## Development Notes

### Database Integration
- Uses Entity Framework Core for primary data access
- Dapper for performance-critical queries
- ODBC driver for AS/400 legacy system connectivity
- Multiple database contexts for different data sources

### API Integration
- RestSharp client for external API calls
- Custom error handling utilities for API clients
- Background services for data synchronization

### UI Framework
- Blazor Server with DevExpress components
- Bootstrap v5 for styling
- Interactive server-side rendering
- Circuit options configured for detailed errors in development

### Service Registration
Services are registered in extension methods:
- `AddApplicationServices()`: Application layer services
- `AddInfrastructureServices()`: Infrastructure services  
- `AddDialogoOperatoreServices()`: Domain-specific services