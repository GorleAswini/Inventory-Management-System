using InventoryManagement.Core.Constants;
using InventoryManagement.Core.Entities;
using InventoryManagement.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.Roles.AnyAsync())
        {
            return; // DB has been seeded
        }

        // 1. Seed Roles (SRS Section 5, 27)
        var assetManagerRole = new Role
        {
            Name = Roles.AssetManager,
            Description = "Primary administrative user with full system and user management access."
        };
        var inventoryExecutiveRole = new Role
        {
            Name = Roles.InventoryExecutive,
            Description = "Responsible for day-to-day inventory CRUD, viewing reports and dashboard."
        };

        context.Roles.AddRange(assetManagerRole, inventoryExecutiveRole);
        await context.SaveChangesAsync();

        // 2. Seed Permissions (SRS Section 7.7)
        var permissions = new List<Permission>
        {
            new() { Name = Permissions.ViewAsset, Description = "View inventory assets" },
            new() { Name = Permissions.CreateAsset, Description = "Add new inventory assets" },
            new() { Name = Permissions.EditAsset, Description = "Update existing inventory assets" },
            new() { Name = Permissions.DeleteAsset, Description = "Delete inventory assets" },
            new() { Name = Permissions.ViewReport, Description = "Access and view inventory reports" },
            new() { Name = Permissions.ManageUsers, Description = "Create, edit, activate/deactivate users" },
            new() { Name = Permissions.ManageRoles, Description = "Manage roles and access definitions" },
            new() { Name = Permissions.ManagePermissions, Description = "Assign permissions to roles" },
            new() { Name = Permissions.ManageOrganization, Description = "Manage organization profile & logo" },
            new() { Name = Permissions.ViewNotifications, Description = "View in-app notification alerts" }
        };

        context.Permissions.AddRange(permissions);
        await context.SaveChangesAsync();

        // 3. Seed Role-Permissions (Asset Manager has ALL, Inventory Executive has day-to-day operations)
        foreach (var perm in permissions)
        {
            context.RolePermissions.Add(new RolePermission { RoleId = assetManagerRole.Id, PermissionId = perm.Id });
        }

        var executiveAllowed = new[]
        {
            Permissions.ViewAsset,
            Permissions.CreateAsset,
            Permissions.EditAsset,
            Permissions.DeleteAsset,
            Permissions.ViewReport,
            Permissions.ViewNotifications
        };

        foreach (var perm in permissions.Where(p => executiveAllowed.Contains(p.Name)))
        {
            context.RolePermissions.Add(new RolePermission { RoleId = inventoryExecutiveRole.Id, PermissionId = perm.Id });
        }
        await context.SaveChangesAsync();

        // 4. Seed Default Users (SRS Section 27)
        var adminUser = new User
        {
            EmployeeId = "EMP-001",
            FirstName = "System",
            LastName = "Admin",
            Username = "admin",
            Email = "admin@demo.com",
            PasswordHash = "AQAAAAIAAYagAAAAEG4zX2qV+demoHashExample==", // For demo
            Department = "IT Administration",
            RoleId = assetManagerRole.Id,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        var executiveUser = new User
        {
            EmployeeId = "EMP-002",
            FirstName = "Inventory",
            LastName = "Executive",
            Username = "inventory",
            Email = "inventory@demo.com",
            PasswordHash = "AQAAAAIAAYagAAAAEG4zX2qV+demoHashExample==", // For demo
            Department = "Operations",
            RoleId = inventoryExecutiveRole.Id,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        context.Users.AddRange(adminUser, executiveUser);
        await context.SaveChangesAsync();

        // 5. Seed Organization Profile (SRS Section 7.5)
        var org = new Organisation
        {
            OrganisationName = "Acme Global Technologies",
            OrganisationCode = "ACME-CORP",
            Address = "100 Innovation Boulevard",
            City = "Hyderabad",
            State = "Telangana",
            Country = "India",
            PostalCode = "500081",
            Phone = "+91 40 1234 5678",
            Email = "contact@acmeglobal.com",
            Website = "https://www.acmeglobal.com",
            LogoPath = "/images/default-org-logo.png",
            UpdatedBy = adminUser.Username,
            UpdatedDate = DateTime.UtcNow
        };
        context.Organisations.Add(org);

        // 6. Seed Sample Categories (SRS Section 7.4 & 27)
        var categories = new List<Category>
        {
            new() { Name = "Laptop", Description = "Enterprise laptops and notebooks", IsActive = true },
            new() { Name = "Desktop", Description = "Workstations and desktop towers", IsActive = true },
            new() { Name = "Printer", Description = "Network and office printers", IsActive = true },
            new() { Name = "Monitor", Description = "High-resolution displays", IsActive = true },
            new() { Name = "Networking", Description = "Routers, switches, and access points", IsActive = true },
            new() { Name = "Furniture", Description = "Desks, chairs, and conference setups", IsActive = true },
            new() { Name = "Mobile", Description = "Testing devices and smartphones", IsActive = true },
            new() { Name = "Other", Description = "Miscellaneous office assets", IsActive = true }
        };
        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();

        // 7. Seed Sample Assets (SRS Section 27: 20-50 sample assets)
        var sampleAssets = new List<Asset>
        {
            new() { AssetCode = "LAP-001", AssetName = "Dell Latitude 5420", CategoryId = categories[0].Id, SerialNumber = "DL-5420-991", PurchaseDate = DateTime.UtcNow.AddMonths(-6), PurchaseCost = 1200, Location = "Floor 2, Bay B", Department = "Engineering", AssignedToUserId = adminUser.Id, Status = AssetStatus.Assigned, CreatedBy = "admin" },
            new() { AssetCode = "LAP-002", AssetName = "ThinkPad T14s", CategoryId = categories[0].Id, SerialNumber = "TP-14S-881", PurchaseDate = DateTime.UtcNow.AddMonths(-4), PurchaseCost = 1350, Location = "Floor 2, Bay A", Department = "Engineering", AssignedToUserId = executiveUser.Id, Status = AssetStatus.Assigned, CreatedBy = "admin" },
            new() { AssetCode = "LAP-003", AssetName = "MacBook Pro 16", CategoryId = categories[0].Id, SerialNumber = "MB-PRO-773", PurchaseDate = DateTime.UtcNow.AddMonths(-2), PurchaseCost = 2400, Location = "IT Storage", Department = "Engineering", AssignedToUserId = null, Status = AssetStatus.Available, CreatedBy = "admin" },
            new() { AssetCode = "DSK-001", AssetName = "HP Z4 Workstation", CategoryId = categories[1].Id, SerialNumber = "HP-Z4-1029", PurchaseDate = DateTime.UtcNow.AddYears(-1), PurchaseCost = 1900, Location = "Design Lab", Department = "Design", AssignedToUserId = null, Status = AssetStatus.UnderMaintenance, CreatedBy = "admin" },
            new() { AssetCode = "MON-001", AssetName = "Dell UltraSharp 27", CategoryId = categories[3].Id, SerialNumber = "DEL-U27-331", PurchaseDate = DateTime.UtcNow.AddMonths(-8), PurchaseCost = 350, Location = "Floor 1, Bay C", Department = "Finance", AssignedToUserId = null, Status = AssetStatus.Available, CreatedBy = "admin" },
            new() { AssetCode = "PRN-001", AssetName = "HP LaserJet Pro 400", CategoryId = categories[2].Id, SerialNumber = "HP-LJ-4001", PurchaseDate = DateTime.UtcNow.AddYears(-2), PurchaseCost = 450, Location = "Reception", Department = "Administration", AssignedToUserId = null, Status = AssetStatus.Available, CreatedBy = "admin" },
            new() { AssetCode = "NET-001", AssetName = "Cisco Catalyst 2960", CategoryId = categories[4].Id, SerialNumber = "CS-CAT-992", PurchaseDate = DateTime.UtcNow.AddYears(-1), PurchaseCost = 850, Location = "Server Room A", Department = "IT Infrastructure", AssignedToUserId = null, Status = AssetStatus.Available, CreatedBy = "admin" },
            new() { AssetCode = "MOB-001", AssetName = "Samsung Galaxy S22", CategoryId = categories[6].Id, SerialNumber = "SM-S22-094", PurchaseDate = DateTime.UtcNow.AddMonths(-10), PurchaseCost = 800, Location = "QA Lab", Department = "Quality Assurance", AssignedToUserId = null, Status = AssetStatus.Damaged, CreatedBy = "admin" }
        };
        context.Assets.AddRange(sampleAssets);

        // 8. Seed Initial Audit Log and Notifications
        context.AuditLogs.Add(new AuditLog
        {
            Action = "Created",
            EntityName = "Asset",
            EntityId = 1,
            PerformedBy = "admin",
            Description = "Initial demo asset seeded: LAP-001",
            CreatedDate = DateTime.UtcNow
        });

        context.Notifications.Add(new Notification
        {
            UserId = adminUser.Id,
            Message = "Welcome to the Inventory Management System. The initial setup is complete.",
            NotificationType = NotificationType.SystemAlert,
            IsRead = false,
            CreatedDate = DateTime.UtcNow
        });

        await context.SaveChangesAsync();
    }
}
