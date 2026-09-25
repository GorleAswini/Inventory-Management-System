# Academic Deliverables: UML Diagrams

## Diagram Inventory
1. **Use Case Diagrams**:
   - Asset Manager Use Cases (User management, roles, categories, organisation profile, full reports, audits).
   - Inventory Executive Use Cases (Dashboard, asset CRUD, search/filter, viewing reports, notifications, profile).
2. **Class Diagrams**:
   - Domain Model & Entity Relationships (`User`, `Role`, `Asset`, `Category`, `AuditLog`, `Notification`).
3. **Sequence Diagrams**:
   - Asset Creation Sequence (Executive -> Controller -> Service -> EF Core -> Audit/Notification -> SMTP).
   - Asset Update & Delete Sequence (Confirmation modal -> Deletion -> Audit Log -> Alert).
4. **Activity Diagrams**:
   - Asset Lifecycle Workflow (`New Asset` -> `Available` -> `Assigned` -> `Under Maintenance` -> `Retired/Lost`).
