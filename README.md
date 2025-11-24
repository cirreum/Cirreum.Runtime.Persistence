# Cirreum.Runtime.Persistence

[![NuGet Version](https://img.shields.io/nuget/v/Cirreum.Runtime.Persistence.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.Runtime.Persistence/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Cirreum.Runtime.Persistence.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.Runtime.Persistence/)
[![GitHub Release](https://img.shields.io/github/v/release/cirreum/Cirreum.Runtime.Persistence?style=flat-square&labelColor=1F1F1F&color=FF3B2E)](https://github.com/cirreum/Cirreum.Runtime.Persistence/releases)
[![License](https://img.shields.io/github/license/cirreum/Cirreum.Runtime.Persistence?style=flat-square&labelColor=1F1F1F&color=F2F2F2)](https://github.com/cirreum/Cirreum.Runtime.Persistence/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-003D8F?style=flat-square&labelColor=1F1F1F)](https://dotnet.microsoft.com/)

**Simplified persistence configuration for Cirreum runtime applications**

## Overview

**Cirreum.Runtime.Persistence** provides a unified persistence layer configuration for the Cirreum framework. It simplifies database integration by offering a single extension method that automatically registers and configures persistence providers with built-in health checks.

## Installation

```bash
dotnet add package Cirreum.Runtime.Persistence
```

## Usage

Add persistence to your application with a single line:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add persistence support
builder.AddPersistence();

var app = builder.Build();
```

This automatically:
- Registers Azure Cosmos DB persistence services
- Configures health checks for database connectivity
- Prevents duplicate service registration
- Integrates with the Cirreum service provider infrastructure

## Features

- **Simple Integration**: One method to configure all persistence needs
- **Azure Cosmos DB Support**: Built-in support for Azure Cosmos DB through `Cirreum.Persistence.Azure`
- **Health Checks**: Automatic health check registration for monitoring
- **Duplicate Prevention**: Smart registration prevents service conflicts
- **Extensible Design**: Built on the Cirreum service provider pattern for easy extension

## Contribution Guidelines

1. **Be conservative with new abstractions**  
   The API surface must remain stable and meaningful.

2. **Limit dependency expansion**  
   Only add foundational, version-stable dependencies.

3. **Favor additive, non-breaking changes**  
   Breaking changes ripple through the entire ecosystem.

4. **Include thorough unit tests**  
   All primitives and patterns should be independently testable.

5. **Document architectural decisions**  
   Context and reasoning should be clear for future maintainers.

6. **Follow .NET conventions**  
   Use established patterns from Microsoft.Extensions.* libraries.

## Versioning

Cirreum.Runtime.Persistence follows [Semantic Versioning](https://semver.org/):

- **Major** - Breaking API changes
- **Minor** - New features, backward compatible
- **Patch** - Bug fixes, backward compatible

Given its foundational role, major version bumps are rare and carefully considered.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**Cirreum Foundation Framework**  
*Layered simplicity for modern .NET*