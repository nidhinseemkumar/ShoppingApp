# 🛒 ShoppingApp - Modular E-Commerce Platform

![ShoppingApp Banner](shopping_app_banner_1777552836206.png)

## 🌟 Overview

ShoppingApp is a robust, high-performance e-commerce solution built with **ASP.NET Core MVC**. It features a unique **Modular Partial Class Architecture** that separates functional logic into granular, manageable files. This design pattern ensures the codebase remains clean, scalable, and extremely easy to debug as the project grows.

## ✨ Key Features

### 🛍️ Customer Experience
- **Dynamic Product Catalog**: Browse products with advanced filtering and real-time search suggestions.
- **Interactive Shopping Cart**: Seamlessly add, update, and manage cart items with instant price calculations.
- **Secure Checkout**: Streamlined checkout process with multiple payment options, including Cash on Delivery (COD).
- **Personalized Profile**: Manage personal details, view order history, and track order statuses.
- **Product Reviews**: Submit and view ratings/comments for products.

### 🛡️ Administrative Power
- **Comprehensive Dashboard**: Real-time business insights including sales trends, top-selling products, and stock alerts.
- **Advanced Inventory Management**: Full CRUD operations for products and categories with image upload support.
- **User & Order Management**: Monitor users, toggle roles, and manage order fulfillment statuses.
- **Data Portability**: Professional-grade export/import tools for Products, Users, and Orders in **CSV** and **Excel** formats.

## 🏗️ Architectural Excellence: Modular Partial Classes

One of the standout features of this repository is its strict adherence to a **"One File per Function"** pattern using C# Partial Classes.

- **Controllers**: Large controller files are split into granular partials (e.g., `UsersController.Login.Post.cs`, `UsersController.Signup.Get.cs`).
- **Services**: Business logic is decoupled into specialized service partials (e.g., `ProductService.Queries.cs`, `ProductService.Actions.cs`).

This approach drastically reduces file complexity and eliminates the "God Object" anti-pattern common in web applications.

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core 9.0 MVC
- **Data Access**: Entity Framework Core with Repository & Unit of Work Patterns
- **Database**: SQL Server / LocalDB
- **Security**: JWT (JSON Web Tokens) + Cookie-based Authentication
- **Mapping**: AutoMapper for clean DTO transitions
- **UI/UX**: Modern CSS with glassmorphism effects and responsive design
- **Libraries**: ClosedXML (Excel Export/Import), Microsoft.IdentityModel (Security)

## 🚀 Getting Started

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or LocalDB

### Installation
1. **Clone the repository**:
   ```bash
   git clone https://github.com/nidhinseemkumar/ShoppingApp.git
   ```
2. **Update Connection String**:
   Edit `appsettings.json` to point to your SQL Server instance.
3. **Apply Migrations**:
   ```bash
   dotnet ef database update
   ```
4. **Run the application**:
   ```bash
   dotnet run
   ```

## 📁 Project Structure

```text
ShoppingApp/
├── Controllers/         # Granular partial controller actions
├── Services/            # Business logic divided into functional partials
├── Models/              # Database entities and data structures
├── DTOs/                # Data Transfer Objects for clean API contracts
├── Repositories/        # Generic Repository and Unit of Work implementation
├── Views/               # Razor views with modern styling
└── wwwroot/             # Static assets (CSS, JS, Uploads)
```

## 📜 Documentation

For a detailed breakdown of where every function is located, please refer to:
- [FunctionBreakpoints.md](FunctionBreakpoints.md)
- [Project_Documentation.md](Project_Documentation.md)

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

---
*Built with ❤️ by Nidhin Seemkumar*
