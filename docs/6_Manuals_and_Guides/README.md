# Academic Deliverables: Manuals & Setup Guides

## 1. User Manual
- Guide for Asset Manager (Configuring Organization, Managing Users, Categories, Reports).
- Guide for Inventory Executive (Logging in, Managing Assets, Filtering, Viewing Notifications).

## 2. Installation & Developer Guide
- Prerequisites: .NET SDK (8.0 or 10.0), Visual Studio or VS Code, SQLite.
- Clone and build commands:
  ```bash
  dotnet build
  dotnet run --project src/InventoryManagement.Web
  ```
- Default Demo Credentials (SRS Section 27):
  - **Asset Manager:** `admin@demo.com` / `Admin@1234`
  - **Inventory Executive:** `inventory@demo.com` / `Executive@1234`
