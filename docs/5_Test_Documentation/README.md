# Academic Deliverables: Test Documentation

## Test Categories (SRS Section 30)
1. **Functional Testing**:
   - Authentication (Login, Logout, Session Expiration).
   - Asset Operations (Create, Update, Delete with confirmation, Search, Filter).
   - User Management & Role Assignment.
   - Profile & Password Change.
2. **Authorisation Testing (RBAC)**:
   - Verify `Inventory Executive` cannot access User Management, Role Management, or Organisation Profile routes.
   - Verify `Asset Manager` has full administrative access.
3. **Validation Testing**:
   - Duplicate Asset Code prevention (BR-001).
   - Mandatory fields check (Asset Name, Code, Category, Status).
   - Password strength validation (Min 8 chars, 1 uppercase, 1 lowercase, 1 number, 1 special character).
4. **UI Responsiveness Testing**:
   - Desktop, Tablet, and Mobile viewport compatibility using Bootstrap 5.
