# Academic Deliverables: Database Design

## Schema Specifications
The database consists of 9 core tables:
1. **Users**: User credentials, contact info, role relation, active status.
2. **Roles**: System roles (`Asset Manager`, `Inventory Executive`).
3. **Permissions**: Granular actions (`CREATE_ASSET`, `MANAGE_USERS`, etc.).
4. **RolePermissions**: Join table for many-to-many role permission mappings.
5. **Assets**: Core inventory register with status, category, pricing, assigned user.
6. **Categories**: Asset categorization records.
7. **Notifications**: Notification messages, type, recipient, read flag.
8. **AuditLogs**: Historical trail for created/updated/deleted operations.
9. **Organisation**: Profile metadata, contact, address, and logo storage path.

## Storage Engine
- **Academic Demonstration:** SQLite In-Memory Database (`DataSource=:memory:`).
- **Persistent Local/Production:** SQLite file database (`inventory.db`) or Microsoft SQL Server.
