# ShoppingApp - Project Documentation

## 1. Overview
ShoppingApp is a modern E-commerce platform built using **ASP.NET Core MVC**. It features a robust separation of concerns, secure authentication, and a scalable architecture.

---

## 2. Architecture & Design Patterns

### A. Separation of Concerns (Layered Architecture)
The project is divided into logical layers to ensure maintainability:
- **Controllers**: Handle HTTP requests and orchestrate the user experience.
- **Services (The Brain)**: Contain the core business logic (e.g., password hashing, order calculations).
- **Repositories**: Abstract the data access (Entity Framework Core).
- **Models/DTOs**: Define the data structures used across the app.

### B. Partial Class Pattern (Advanced Organization)
To keep the code clean and manageable, all major controllers are implemented as **Partial Classes**. This allows us to split a single class into multiple physical files:
- **`ControllerName.cs`**: Contains the constructor and main landing actions.
- **`ControllerName.Actions.cs`** (or `.Auth.cs`, `.Admin.cs`): Contains specific functional actions (e.g., Login/Signup or Admin management).

**Benefits:**
- Reduces file length and clutter.
- Makes it easier for teams to work on the same controller simultaneously.
- Improves "Findability" for specific features (like Login vs. Profile).

---

## 3. Directory Structure

- `/Controllers`: Web layer entry points (organized via partial classes).
- `/Services`: The business logic layer (IPaymentService, IUserService, etc.).
- `/Repositories`: Database interactions using the Unit of Work pattern.
- `/Models`: Database entities (Product, User, Order).
- `/DTOs`: Data Transfer Objects for passing safe data between layers.
- `/Views`: Razor templates for the user interface.
- `/wwwroot`: Static assets (CSS, JavaScript, Images).

---

## 4. Key Security Features
- **BCrypt.Net**: For secure password hashing and verification.
- **JWT (JSON Web Tokens)**: Used for secure identity handling and API security.
- **Cookie Authentication**: Standard ASP.NET Core middleware for session management.
- **Role-Based Authorization**: Distinct access levels for `Admin` and `Customer`.

---

## 5. Summary
The ShoppingApp is designed with scalability in mind. By combining the **Service Layer** pattern for logic and the **Partial Class** pattern for organization, the codebase remains clean even as the number of features grows.
