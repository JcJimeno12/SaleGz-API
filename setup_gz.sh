#!/bin/bash

# =============================================
# SaleGz API - Setup Script
# .NET 8 + Clean Architecture
# =============================================

echo "🚀 Creando solución SaleGz..."

# Crear solución
dotnet new sln -n SaleGz

# ── Crear proyectos ──────────────────────────
dotnet new classlib -n SaleGz.Domain       -o src/SaleGz.Domain       --framework net8.0
dotnet new classlib -n SaleGz.Application  -o src/SaleGz.Application  --framework net8.0
dotnet new classlib -n SaleGz.Infrastructure -o src/SaleGz.Infrastructure --framework net8.0
dotnet new webapi   -n SaleGz.API          -o src/SaleGz.API          --framework net8.0

# ── Agregar a la solución ───────────────────
dotnet sln add src/SaleGz.Domain/SaleGz.Domain.csproj
dotnet sln add src/SaleGz.Application/SaleGz.Application.csproj
dotnet sln add src/SaleGz.Infrastructure/SaleGz.Infrastructure.csproj
dotnet sln add src/SaleGz.API/SaleGz.API.csproj

# ── Referencias entre proyectos ─────────────
dotnet add src/SaleGz.Application/SaleGz.Application.csproj   reference src/SaleGz.Domain/SaleGz.Domain.csproj
dotnet add src/SaleGz.Infrastructure/SaleGz.Infrastructure.csproj reference src/SaleGz.Domain/SaleGz.Domain.csproj
dotnet add src/SaleGz.Infrastructure/SaleGz.Infrastructure.csproj reference src/SaleGz.Application/SaleGz.Application.csproj
dotnet add src/SaleGz.API/SaleGz.API.csproj                   reference src/SaleGz.Application/SaleGz.Application.csproj
dotnet add src/SaleGz.API/SaleGz.API.csproj                   reference src/SaleGz.Infrastructure/SaleGz.Infrastructure.csproj

# ── NuGet Packages ──────────────────────────

# Application
dotnet add src/SaleGz.Application/SaleGz.Application.csproj package MediatR --version 12.4.1
dotnet add src/SaleGz.Application/SaleGz.Application.csproj package FluentValidation.DependencyInjectionExtensions --version 11.9.2
dotnet add src/SaleGz.Application/SaleGz.Application.csproj package AutoMapper --version 13.0.1

# Infrastructure
dotnet add src/SaleGz.Infrastructure/SaleGz.Infrastructure.csproj package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.11
dotnet add src/SaleGz.Infrastructure/SaleGz.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Tools --version 8.0.11
dotnet add src/SaleGz.Infrastructure/SaleGz.Infrastructure.csproj package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.11
dotnet add src/SaleGz.Infrastructure/SaleGz.Infrastructure.csproj package BCrypt.Net-Next --version 4.0.3
dotnet add src/SaleGz.Infrastructure/SaleGz.Infrastructure.csproj package AutoMapper --version 13.0.1

# API
dotnet add src/SaleGz.API/SaleGz.API.csproj package Swashbuckle.AspNetCore --version 6.9.0
dotnet add src/SaleGz.API/SaleGz.API.csproj package Serilog.AspNetCore --version 8.0.3
dotnet add src/SaleGz.API/SaleGz.API.csproj package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.11

echo "✅ Solución creada exitosamente!"
echo ""
echo "📁 Estructura:"
echo "   SaleGz/"
echo "   ├── src/"
echo "   │   ├── SaleGz.Domain/"
echo "   │   ├── SaleGz.Application/"
echo "   │   ├── SaleGz.Infrastructure/"
echo "   │   └── SaleGz.API/"
echo ""
echo "▶️  Siguiente paso: copiar los archivos generados a cada proyecto."
