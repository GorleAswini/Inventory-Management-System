# Inventory Management System (IMS) - Project Folder Structure & Architecture

**Project Type:** Fullstack Engineering with .NET (Final Year Engineering Project)  
**Document Version:** 1.0 (Based on SRS Specification)  
**Target Platform:** ASP.NET Core MVC (.NET 8/10), Entity Framework Core, SQLite, Bootstrap 5  

---

## 1. Solution Architecture Overview

The solution adheres to **N-Tier Layered Architecture** with **Separation of Concerns (SoC)**, **SOLID Principles**, and **Dependency Injection** as mandated by the SRS (Sections 13, 14, and NFR-005).

```
InventoryManagementSystem/
├── docs/                                 # Academic Project Deliverables (SRS Section 28)
├── src/
│   ├── InventoryManagement.Core/         # Domain Layer (Entities, Enums, Interfaces, Constants)
│   ├── InventoryManagement.Infrastructure/# Data Layer (EF Core, SQLite DbContext, Repositories, SMTP)
│   ├── InventoryManagement.Services/     # Business Logic Layer (Services, DTOs, Business Rules)
│   └── InventoryManagement.Web/          # UI / Presentation Layer (ASP.NET Core MVC, Views, Controllers)
└── tests/
    └── InventoryManagement.Tests/        # Test Suite (Unit, Validation & Security Tests)
```

---

## 2. Detailed Folder-Wise Structure & Responsibilities

### 2.1 `docs/` — Academic Project Deliverables (SRS Section 28)
Maintains all required academic documentation and diagrams:
- `1_Software_Requirements_Specification/`: Approved SRS document, feature scope, and change logs.
- `2_System_Architecture/`: High-level architecture, layer interactions, and security model.
- `3_Database_Design/`: Relational schema diagrams, dictionary, and SQLite configuration.
- `4_UML_Diagrams/`:
  - Use Case diagrams (Asset Manager vs Inventory Executive - Section 29)
  - Class diagrams (Domain entities & relationships - Section 12)
  - Sequence diagrams (Asset creation, update, delete workflows - Sections 19–21)
  - Activity diagrams (Inventory lifecycle - Section 18)
- `5_Test_Documentation/`: Test plans, test cases (functional, authorization, validation - Section 30), and execution matrix.
- `6_Manuals_and_Guides/`: User manual with screenshots and installation/setup guide.
- `7_Final_Project_Report/`: Project compilation report and presentation deck.

---

### 2.2 `src/InventoryManagement.Core/` — Domain Layer
Pure C# class library containing enterprise business concepts with zero external UI or database dependencies:
- **`Entities/`**: Database models matching Section 11 & 18:
  - `Asset.cs`: Asset Code, Name, Category, Serial Number, Purchase Info, Location, Status, Department, Assigned User, Audit Fields.
  - `Category.cs`: Name, Description, IsActive flag.
  - `User.cs`: Employee ID, Names, Username, Email, PasswordHash, Department, Role, IsActive.
  - `Role.cs`: Predefined system roles (`Asset Manager`, `Inventory Executive`).
  - `Permission.cs`: Granular permissions (e.g., `CREATE_ASSET`, `MANAGE_USERS`).
  - `RolePermission.cs`: Association between Roles and Permissions.
  - `Organisation.cs`: Company profile, logo, address, contact information.
  - `Notification.cs`: In-app notification messages, event types, read/unread status.
  - `AuditLog.cs`: Action, entity name, entity ID, performed by, timestamp, description.
- **`Enums/`**:
  - `AssetStatus.cs`: `Available`, `Assigned`, `UnderMaintenance`, `Damaged`, `Retired`, `Lost`.
  - `NotificationType.cs`: `AssetAdded`, `AssetUpdated`, `AssetDeleted`, `System`.
- **`Constants/`**:
  - `Roles.cs`: Role constants (`AssetManager`, `InventoryExecutive`).
  - `Permissions.cs`: String identifiers for all system permissions.
- **`Interfaces/`**:
  - `Repositories/`: Contract interfaces (`IAssetRepository`, `IUserRepository`, etc.)
  - `Services/`: Service contracts (`IAssetService`, `IAuthService`, `INotificationService`, `IEmailService`).

---

### 2.3 `src/InventoryManagement.Infrastructure/` — Data & External Services Layer
Implements persistence and third-party integrations:
- **`Data/`**:
  - `ApplicationDbContext.cs`: EF Core DbContext with DbSets for all 9 entities.
  - `Configurations/`: Entity type configurations (keys, unique constraints, foreign keys).
  - `DbInitializer.cs`: Seeds default users (`admin@demo.com`, `inventory@demo.com`), roles, categories, and 20–50 sample assets (Section 27).
- **`Repositories/`**:
  - Concrete EF Core implementations of repository interfaces for data access.
- **`Email/`**:
  - `SmtpEmailSender.cs`: SMTP client for sending notifications on asset events (Section 7.12).
  - `EmailSettings.cs`: SMTP host, port, credentials configuration model.
- **`Logging/`**:
  - Application logging hooks for startup, security events, errors, and audit triggers (Section 17).

---

### 2.4 `src/InventoryManagement.Services/` — Business Logic Layer
Orchestrates business workflows, validation rules, and operations:
- **`Services/`**:
  - `AssetService.cs`: Enforces BR-001 (unique asset code), BR-008 (no assignment to inactive user), triggers audit log (BR-012) and notifications (BR-011).
  - `AuthService.cs`: Validates credentials, securely hashes passwords, checks inactive status (BR-007).
  - `UserService.cs`: User lifecycle management, activation/deactivation (Asset Manager only).
  - `DashboardService.cs`: Aggregates KPIs (Total, Active, Available, Assigned, Maintenance) and chart distributions.
  - `ReportService.cs`: Generates the 6 required reports (Inventory Register, Status, Category, Location, Department, Activity).
  - `NotificationService.cs`: In-app notification creation and triggers SMTP email alerts.
  - `AuditService.cs`: Records audit trail entries for changes across entities.
  - `OrganisationService.cs`: Profile and branding management.
- **`DTOs/`**:
  - Data transfer objects for separating domain models from view presentation.

---

### 2.5 `src/InventoryManagement.Web/` — Presentation Layer (ASP.NET Core MVC)
User-facing responsive application built with Razor Views and Bootstrap 5:
- **`Controllers/`**:
  - `AccountController.cs`: Login, logout, access denied handler.
  - `DashboardController.cs`: Displays KPI cards, charts, and activity feeds.
  - `AssetsController.cs`: Complete asset register, search, filter, CRUD, detail view.
  - `CategoriesController.cs`: Restricted to Asset Manager.
  - `UsersController.cs`: Restricted to Asset Manager (BR-003).
  - `RolesController.cs`: Role and permission matrix (BR-004).
  - `OrganisationController.cs`: Organization profile & logo upload (BR-005).
  - `ReportsController.cs`: Filterable reports with HTML tables (Section 7.13, 22).
  - `NotificationsController.cs`: In-app notification center and mark-as-read actions.
  - `ProfileController.cs`: Current user profile and password change (Section 7.8, 7.9).
- **`ViewModels/`**: Strongly-typed models with validation annotations (Data Annotations) for server-side validation.
- **`Views/`**:
  - Razor views grouped per controller.
  - `Shared/_Layout.cshtml`: Sidebar navigation, top bar, notification badge dropdown, user profile dropdown (Section 23).
- **`Filters/`**:
  - `HasPermissionAttribute.cs`: Custom authorization filter for permission checking.
- **`wwwroot/`**:
  - `css/`: Theme styling for administrative dashboard layout.
  - `js/`: Chart scripts (Chart.js / Dashboard KPIs), interactive filters, modal confirmations.
  - `uploads/`: Uploaded organization logos.

---

### 2.6 `tests/InventoryManagement.Tests/` — Quality Assurance Suite
Ensures test coverage as specified in Section 30:
- `Services/`: Business logic test cases for asset CRUD, unique code constraint, dashboard metrics.
- `Validation/`: Model validation tests (email format, mandatory fields, password complexity).
- `Security/`: RBAC tests ensuring `Inventory Executive` cannot access restricted administrative routes.

---

## 3. Technology Stack Reference

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 8/10) |
| Language | C# |
| ORM | Entity Framework Core |
| Database | SQLite In-Memory / SQLite Persistent |
| Frontend | Razor Views, Bootstrap 5, HTML5, CSS3, JavaScript |
| Auth & RBAC | Cookie Authentication, Role & Permission Authorization |
| Email | System.Net.Mail / MailKit (SMTP) |
| Testing | xUnit, Moq, FluentAssertions |
