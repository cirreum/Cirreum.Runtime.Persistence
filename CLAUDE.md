# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build Commands

```bash
# Restore NuGet packages
dotnet restore Cirreum.Runtime.Persistence.slnx

# Build the project
dotnet build Cirreum.Runtime.Persistence.slnx --configuration Release

# Create NuGet package
dotnet pack Cirreum.Runtime.Persistence.slnx --configuration Release

# Clean build artifacts
dotnet clean Cirreum.Runtime.Persistence.slnx
```

## Project Architecture

This is a .NET 10.0 runtime extension library that provides persistence configuration for the Cirreum framework.

### Key Components

1. **Host Extensions** (`src/Cirreum.Runtime.Persistence/Extensions/Hosting/HostExtensions.cs`): 
   - Provides `AddPersistence()` extension method for `IHostApplicationBuilder`
   - Registers Azure Cosmos DB persistence services via service provider pattern
   - Uses marker pattern to prevent duplicate registrations

2. **Dependencies**:
   - `Cirreum.Persistence.Azure`: Azure Cosmos DB persistence implementation
   - `Cirreum.Runtime.ServiceProvider`: Service provider runtime infrastructure

3. **Build System**:
   - Uses modern .slnx solution format
   - MSBuild props files in `build/` directory for modular configuration
   - Automatic versioning from git tags (v1.2.3 → 1.2.3)
   - GitHub Actions workflow for automated NuGet publishing on releases

### Development Patterns

1. **Namespace Convention**: Place extension methods in `Microsoft.AspNetCore.Hosting` namespace for seamless integration
2. **Service Registration**: Use `RegisterServiceProvider<>()` pattern with registrar, settings, and health check types
3. **Duplicate Prevention**: Use marker types with `IsMarkerTypeRegistered<>()` to prevent duplicate service registration
4. **Code Style**: Tabs for indentation, `this.` prefix for instance members (see .editorconfig)

## Testing

No test projects are currently present in this repository. When adding tests, follow the standard .NET test project conventions.

## CI/CD

GitHub Actions workflow (`publish.yml`) automatically:
- Triggers on release creation
- Builds with .NET 10.0
- Publishes to NuGet.org using OIDC authentication
- Versions packages based on git tags (v1.0.0) or generates dev versions