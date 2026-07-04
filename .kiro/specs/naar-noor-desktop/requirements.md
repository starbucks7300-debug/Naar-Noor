# Requirements Document: Naar-Noor Desktop Application

**Version:** 1.0  
**Date:** 2024  
**Status:** Draft  
**Scope:** Windows desktop client for Naar-Noor restaurant management system  
**Target Platform:** .NET 8+, Windows 10/11, WinForms→WPF evolution path

---

## Table of Contents

1. [Overview](#overview)
2. [Core Authentication & Security (REQ-001 to REQ-020)](#core-authentication--security)
3. [Dashboard & Monitoring (REQ-021 to REQ-040)](#dashboard--monitoring)
4. [Menu Management (REQ-041 to REQ-060)](#menu-management)
5. [Reservation System (REQ-061 to REQ-080)](#reservation-system)
6. [Staff Management (REQ-081 to REQ-100)](#staff-management)
7. [Reports & Analytics (REQ-101 to REQ-120)](#reports--analytics)
8. [Localization & UI (REQ-121 to REQ-140)](#localization--ui)
9. [Infrastructure & Deployment (REQ-141 to REQ-160)](#infrastructure--deployment)

---

## Overview

The Naar-Noor Desktop Application is a comprehensive Windows desktop client that enables restaurant staff and managers to interact with the restaurant management system. It integrates seamlessly with the ASP.NET Core 8 backend through REST APIs, providing real-time dashboards, menu management, reservation handling, staff coordination, and business analytics—all with bilingual support (English/Arabic) and MVVM architecture for maintainability and testability.

**Key Characteristics:**
- Native Windows experience (WinForms→WPF evolution path)
- Offline-capable with intelligent caching and sync
- Role-based access control (Manager, Chef, Staff, Admin)
- Bilingual UI with runtime language switching
- High performance (<500ms UI response time, <2s data load)
- Enterprise-grade security (TLS 1.3, DPAPI token storage, audit logging)

---

## Core Authentication & Security

### REQ-001: User Login with Credentials

**Description:**  
The desktop client must authenticate users using username/password credentials against the backend API. Upon successful authentication, the system issues a JWT token stored securely for subsequent API requests.

**Acceptance Criteria:**
- [ ] User can enter username and password on login form
- [ ] Credentials submitted via secure HTTP POST to `/api/auth/login` endpoint
- [ ] Valid credentials return JWT access token and refresh token
- [ ] Invalid credentials display user-friendly error message (e.g., "Invalid username or password")
- [ ] Login form clears sensitive fields after failed attempt
- [ ] Network timeouts display retry option or offline mode option
- [ ] User redirected to dashboard after successful login
- [ ] Authentication tokens stored per REQ-003

**Priority:** Must Have  
**Dependencies:** None  
**Validation Method:** Integration test + manual testing

---

### REQ-002: Session Management & Token Lifecycle

**Description:**  
The system must manage user sessions and JWT token lifecycle, including token expiration, refresh token handling, and automatic re-authentication when tokens expire.

**Acceptance Criteria:**
- [ ] Access tokens expire after 30 minutes of issuance
- [ ] Refresh tokens automatically obtained on token expiration
- [ ] Expired access token detected via 401 Unauthorized response from API
- [ ] System automatically calls `/api/auth/refresh` with refresh token
- [ ] Successful token refresh allows operation to continue transparently
- [ ] Refresh token rotation implemented (old refresh token invalidated after use)
- [ ] Max token refresh attempts limited to 3 retries before forcing re-login
- [ ] User notified if refresh fails and must re-authenticate
- [ ] Session persists across application restart (tokens retrieved from secure storage)
- [ ] Logout clears all tokens and session state

**Priority:** Must Have  
**Dependencies:** REQ-001, REQ-003  
**Validation Method:** Integration test + property-based testing (idempotency)

---

### REQ-003: Secure Token Storage with DPAPI

**Description:**  
Authentication tokens (access token, refresh token) must be stored securely on the local machine using Windows Data Protection API (DPAPI) to prevent unauthorized access even if the machine is compromised.

**Acceptance Criteria:**
- [ ] Tokens encrypted using DPAPI before persisted to disk
- [ ] Encryption scoped to current user (CurrentUser scope, not LocalMachine)
- [ ] Tokens stored in application data directory: `%APPDATA%\NaarNoor\tokens`
- [ ] Token file permissions restricted to current user only
- [ ] Tokens decrypted from storage only when needed for API requests
- [ ] Decryption failure gracefully handled (prompt user to re-login)
- [ ] Tokens cleared from memory immediately after use
- [ ] Application uses secure string/byte array handling (no plaintext in memory)
- [ ] No tokens logged to application logs or event viewers
- [ ] Token file deleted on logout

**Priority:** Must Have  
**Dependencies:** None  
**Validation Method:** Unit test + security review

---

### REQ-004: Role-Based Access Control (RBAC)

**Description:**  
The system must enforce role-based access control, restricting feature access based on authenticated user's role. Four roles exist: Manager, Chef, Staff, and Admin.

**Acceptance Criteria:**
- [ ] User role retrieved from JWT token claims during authentication
- [ ] Role value persisted in application context for feature checks
- [ ] Menu management features restricted to Manager/Admin roles (REQ-041+)
- [ ] Reservation override restricted to Manager/Admin roles (REQ-061+)
- [ ] Chef features (menu editing, order status) restricted to Chef role
- [ ] Staff features (reservation booking, basic reporting) available to Staff+
- [ ] Admin features (system configuration, audit logs) restricted to Admin role
- [ ] Unauthorized access attempts logged to audit trail (REQ-005)
- [ ] UI controls disabled or hidden for unauthorized roles
- [ ] API calls validate authorization headers and role claims

**Priority:** Must Have  
**Dependencies:** REQ-001  
**Validation Method:** Unit test + integration test

---

### REQ-005: Audit Logging for Security Events

**Description:**  
The system must maintain comprehensive audit logs of security-relevant events (authentication, authorization failures, sensitive operations) for compliance and forensics.

**Acceptance Criteria:**
- [ ] Login attempts (success/failure) logged with username and timestamp
- [ ] Logout events logged with user ID and timestamp
- [ ] Unauthorized access attempts logged with attempted action and user
- [ ] Sensitive operations (data creation, modification, deletion) logged
- [ ] Audit logs stored in SQLite database: `audit_logs` table
- [ ] Audit log fields: `timestamp`, `user_id`, `action`, `resource_type`, `status`, `details`
- [ ] Audit logs retained for minimum 90 days
- [ ] Audit logs not modifiable via UI (append-only)
- [ ] Admin users can view filtered audit logs (REQ-005 + REQ-100)
- [ ] Log queries optimized via indexing on user_id and timestamp

**Priority:** Should Have  
**Dependencies:** REQ-001, REQ-004  
**Validation Method:** Integration test + manual audit review

---

### REQ-006: Password Policy & Account Security

**Description:**  
The system must enforce password policies at login and support account lockout to prevent brute-force attacks.

**Acceptance Criteria:**
- [ ] Failed login attempts tracked per username
- [ ] Account locked after 5 consecutive failed attempts
- [ ] Lockout duration: 15 minutes or until Admin override
- [ ] User notified of account lockout with unlock instructions
- [ ] Failed attempt counter reset on successful login
- [ ] Lockout state stored in backend (not client-side)
- [ ] Client respects server's account lockout status
- [ ] Rate limiting applied: max 10 login requests per minute per IP

**Priority:** Should Have  
**Dependencies:** REQ-001  
**Validation Method:** Integration test + manual testing

---

### REQ-007: TLS 1.3 Encryption for All Network Communication

**Description:**  
All network communication between desktop client and backend API must use TLS 1.3 encryption to prevent eavesdropping and man-in-the-middle attacks.

**Acceptance Criteria:**
- [ ] All HTTP requests upgraded to HTTPS
- [ ] HttpClient configured to require TLS 1.3 minimum
- [ ] TLS 1.2 and earlier explicitly disabled
- [ ] Certificate validation enabled (invalid certificates rejected)
- [ ] Certificate pinning implemented for production API endpoint
- [ ] Self-signed certificates handled gracefully in development (with opt-in warning)
- [ ] Network traffic not loggable in plaintext
- [ ] Failed TLS connections display appropriate error message

**Priority:** Must Have  
**Dependencies:** None  
**Validation Method:** Network inspection + security testing

---

### REQ-008: Input Validation & Injection Prevention

**Description:**  
All user inputs must be validated and sanitized to prevent injection attacks (SQL, XSS, command injection).

**Acceptance Criteria:**
- [ ] All textbox inputs validated before submission
- [ ] Username: alphanumeric + underscore, max 50 chars, no spaces
- [ ] Passwords: min 8 chars, min 1 uppercase, 1 lowercase, 1 digit, 1 special char
- [ ] API requests built using parameterized queries (no string concatenation)
- [ ] Menu item names: max 200 chars, no HTML/script tags
- [ ] Price fields accept only positive decimal numbers (max 999.99)
- [ ] Date fields use date picker (no free text entry)
- [ ] Validation errors displayed to user with clear guidance
- [ ] Injection attempts logged to audit trail

**Priority:** Must Have  
**Dependencies:** None  
**Validation Method:** Unit test + security testing

---

### REQ-009: Error Handling Without Information Disclosure

**Description:**  
Error messages must be user-friendly and never disclose sensitive system information that could aid attackers.

**Acceptance Criteria:**
- [ ] Generic error message shown to users: "Operation failed. Please try again."
- [ ] Detailed error information logged internally only
- [ ] No stack traces exposed in UI
- [ ] No database error messages shown
- [ ] No API endpoint paths revealed in errors
- [ ] No internal IP addresses or hostnames disclosed
- [ ] Authentication failures show generic message (not "user not found" vs "wrong password")
- [ ] Network errors handled with retry suggestion, not raw HTTP status codes

**Priority:** Should Have  
**Dependencies:** None  
**Validation Method:** Manual testing + security review

---

### REQ-010: Multi-Factor Authentication Support (Future)

**Description:**  
The system architecture must support future integration of multi-factor authentication (MFA) without requiring major refactoring.

**Acceptance Criteria:**
- [ ] Authentication service designed to handle additional challenge factors
- [ ] JWT token structure supports MFA indicators
- [ ] UI framework supports modal/form injection for MFA challenges
- [ ] Backend API contract includes MFA endpoints (already implemented)
- [ ] Client-side hooks prepared for OTP verification flow
- [ ] No breaking changes required to existing auth flow when MFA added

**Priority:** Nice to Have  
**Dependencies:** REQ-001  
**Validation Method:** Architectural review

---

### REQ-011 through REQ-020: Additional Security Requirements

### REQ-011: Claims-Based Authorization

**Description:**  
The system must support granular authorization using JWT claims beyond just roles, enabling fine-grained access control.

**Acceptance Criteria:**
- [ ] JWT token includes custom claims: `restaurant_id`, `department`, `permissions`
- [ ] Authorization service checks specific claims for sensitive operations
- [ ] Different manager types (head chef manager, floor manager) distinguished via claims
- [ ] API integration validates claims match user's intended scope
- [ ] Claims immutable after token issuance (validated by backend)

**Priority:** Should Have  
**Dependencies:** REQ-001, REQ-004  
**Validation Method:** Integration test

---

### REQ-012: Secure Logout & Session Invalidation

**Description:**  
Logout must completely clear session data and invalidate tokens to prevent session hijacking.

**Acceptance Criteria:**
- [ ] Logout call sent to backend to invalidate refresh token
- [ ] All local tokens removed from DPAPI storage
- [ ] Application state cleared: user context, cached data, ViewModels
- [ ] In-memory caches flushed
- [ ] Temporary files cleaned up
- [ ] User redirected to login screen
- [ ] Application state reset to pre-login condition
- [ ] Subsequent API calls fail if attempted before re-login

**Priority:** Must Have  
**Dependencies:** REQ-001, REQ-003  
**Validation Method:** Integration test

---

### REQ-013: Certificate Pinning for API Endpoint

**Description:**  
Certificate pinning ensures the application only accepts the legitimate backend API certificate, preventing MITM attacks via fraudulent certificates.

**Acceptance Criteria:**
- [ ] SHA-256 hash of expected certificate stored in application
- [ ] Certificate validation performed before TLS handshake
- [ ] Mismatched certificate causes connection to fail
- [ ] Fallback certificate (backup/future cert) configurable
- [ ] Development environment allows bypassing pinning with warning
- [ ] Certificate hash configurable via secure configuration (not hardcoded)

**Priority:** Should Have  
**Dependencies:** REQ-007  
**Validation Method:** Security testing

---

### REQ-014: Secure Credential Input UI

**Description:**  
Login form must prevent credential capture and secure input handling.

**Acceptance Criteria:**
- [ ] Password field masks input with bullets/asterisks
- [ ] Clipboard access disabled for password field (prevent copy)
- [ ] Auto-complete/auto-suggest disabled for credentials
- [ ] No credential data appears in logs or crash reports
- [ ] Browser/Windows credential manager integration optional (not required)
- [ ] Password field memory cleared after login attempt
- [ ] Focus management prevents unexpected tab-order exposure

**Priority:** Should Have  
**Dependencies:** REQ-001  
**Validation Method:** Manual testing + code review

---

### REQ-015: Network Request Signing for Sensitive Operations

**Description:**  
Sensitive state-changing operations (menu modification, staff changes) must be request-signed to prevent tampering.

**Acceptance Criteria:**
- [ ] DELETE/PUT/POST operations to sensitive endpoints include HMAC signature
- [ ] Signature computed using client-side secret (derived from user token)
- [ ] Signature header: `X-Request-Signature`
- [ ] Backend validates signature before processing
- [ ] Signature includes request body hash and timestamp
- [ ] Timestamp validation prevents replay attacks (5-minute window)

**Priority:** Should Have  
**Dependencies:** REQ-001  
**Validation Method:** Integration test + security testing

---

### REQ-016: Offline Mode with Data Integrity

**Description:**  
When network connectivity is unavailable, the application must continue functioning with cached data while queuing mutations for later sync.

**Acceptance Criteria:**
- [ ] Offline status detected automatically (network connectivity check)
- [ ] User notified of offline mode: "Working Offline"
- [ ] Read operations served from local SQLite cache
- [ ] Write operations queued in `pending_operations` table
- [ ] Queued operations include: timestamp, user_id, operation_type, payload
- [ ] UI prevents operations known to require network
- [ ] Visual indication of queued operations (badge/notification)
- [ ] On reconnection, queued operations synced in order
- [ ] Sync failures rolled back with user notification
- [ ] Conflict resolution applied: last-write-wins for conflicting updates

**Priority:** Should Have  
**Dependencies:** REQ-002, REQ-040  
**Validation Method:** Integration test + manual testing

---

### REQ-017: Security Headers in HTTP Requests

**Description:**  
All HTTP requests must include security headers to enforce additional protections.

**Acceptance Criteria:**
- [ ] `X-Content-Type-Options: nosniff` prevents MIME-sniffing
- [ ] `X-Frame-Options: DENY` prevents clickjacking
- [ ] `X-XSS-Protection: 1; mode=block` enables XSS filter
- [ ] `Strict-Transport-Security: max-age=31536000` enforces HTTPS
- [ ] Custom header `X-Client-Version` identifies client version
- [ ] User-Agent set to identify desktop client + .NET version

**Priority:** Should Have  
**Dependencies:** REQ-007  
**Validation Method:** Network inspection

---

### REQ-018: Sensitive Data Handling in Logs

**Description:**  
Logging must never expose sensitive information (passwords, tokens, PII).

**Acceptance Criteria:**
- [ ] Password fields excluded from all logs
- [ ] JWT tokens masked in logs: show only first 20 chars + ellipsis
- [ ] Personal information (emails, phone numbers) not logged in normal operation
- [ ] PII logged only for audit purposes with explicit flag
- [ ] Log files stored in encrypted location or access-restricted
- [ ] Log retention policy enforced: max 30 days for detail logs
- [ ] Sensitive API responses filtered before logging

**Priority:** Should Have  
**Dependencies:** None  
**Validation Method:** Code review + log inspection

---

### REQ-019: Dependency Vulnerability Management

**Description:**  
The application must track and update dependencies to address security vulnerabilities.

**Acceptance Criteria:**
- [ ] NuGet dependencies scanned for vulnerabilities at build time
- [ ] Build fails if critical vulnerabilities detected
- [ ] Dependency update process documented
- [ ] Security advisories monitored via GitHub Dependabot
- [ ] Automated PRs created for security updates
- [ ] Monthly security audit of dependencies scheduled

**Priority:** Should Have  
**Dependencies:** None  
**Validation Method:** Automated scanning + CI/CD integration

---

### REQ-020: Compliance with OWASP Desktop Security Guidelines

**Description:**  
The application architecture and implementation must align with OWASP Desktop Security guidelines.

**Acceptance Criteria:**
- [ ] Follows OWASP Top 10 security principles
- [ ] Implements defense-in-depth: no single point of failure
- [ ] Input validation at all boundaries
- [ ] Secure defaults: fail-secure not fail-open
- [ ] Least privilege principle: users/processes have minimal necessary permissions
- [ ] Principle of least surprise: expected security behavior
- [ ] Security testing part of development lifecycle

**Priority:** Should Have  
**Dependencies:** All security requirements  
**Validation Method:** Security audit + penetration testing

---

## Dashboard & Monitoring

### REQ-021: Dashboard Overview Display

**Description:**  
The dashboard must display a comprehensive overview of restaurant operations with key metrics and status indicators.

**Acceptance Criteria:**
- [ ] Dashboard accessible immediately after login
- [ ] Display layout responsive to 1920x1080 minimum resolution
- [ ] Key widgets: Reservations Today, Current Orders, Staff Status, Revenue (if permitted)
- [ ] All widgets load within 2 seconds
- [ ] Data refreshes automatically every 30 seconds (configurable)
- [ ] Manual refresh button available
- [ ] Network errors display gracefully without freezing UI
- [ ] Offline mode shows last-known-good data with "Offline" badge

**Priority:** Must Have  
**Dependencies:** REQ-001  
**Validation Method:** Integration test + manual testing + performance testing

---

### REQ-022: Reservation Overview Widget

**Description:**  
Display today's reservations with booking times, party sizes, and status.

**Acceptance Criteria:**
- [ ] List all reservations for current date sorted by booking time
- [ ] Show: reservation ID, customer name, party size, booking time, table number, status
- [ ] Color-coded status indicators: confirmed (green), pending (yellow), cancelled (red)
- [ ] Click reservation to open detail view (see REQ-065)
- [ ] Filter by status (all/confirmed/pending/arrived/completed)
- [ ] Search by customer name or reservation ID
- [ ] Show count: "12 reservations today"
- [ ] Pagination if >50 reservations: 10 items per page

**Priority:** Must Have  
**Dependencies:** REQ-021, REQ-061  
**Validation Method:** Integration test + UI test

---

### REQ-023: Current Orders Display

**Description:**  
Show active food orders with status and kitchen information.

**Acceptance Criteria:**
- [ ] Display all orders placed today with status
- [ ] Show: order ID, table number, items, order time, status
- [ ] Status workflow: pending → in_progress → ready → delivered
- [ ] Color-coded badges for each status
- [ ] Filter by status or table number
- [ ] Order summary: "5 pending, 8 in progress, 3 ready"
- [ ] Click order to view full order details
- [ ] Update via real-time notification if available, else 30-second refresh

**Priority:** Must Have  
**Dependencies:** REQ-021  
**Validation Method:** Integration test + manual testing

---

### REQ-024: Staff Status Indicator

**Description:**  
Show availability and role of current staff members working today.

**Acceptance Criteria:**
- [ ] List all staff scheduled for today
- [ ] Display: name, role (chef/waiter/manager), status (available/busy/break)
- [ ] Color indicators: green (available), yellow (busy), red (break)
- [ ] Staff manager can update staff status
- [ ] Show count: "3 managers, 5 waiters, 2 chefs available"
- [ ] Allow quick status change from dashboard (drag/click)
- [ ] Staff notified of status changes

**Priority:** Should Have  
**Dependencies:** REQ-021, REQ-081  
**Validation Method:** Integration test + manual testing

---

### REQ-025: Real-Time Notifications (Future)

**Description:**  
Display real-time notifications for critical events (new reservation, order ready, staff assignment).

**Acceptance Criteria:**
- [ ] Notification center icon in top-right corner
- [ ] Unread notification count badge
- [ ] Toast notifications for critical events (dismissible)
- [ ] Notification history viewable in notification center
- [ ] Filter notifications by type
- [ ] Notification preferences configurable (sound, desktop alert, etc.)
- [ ] Backend push integration prepared (WebSocket support)

**Priority:** Nice to Have  
**Dependencies:** REQ-021  
**Validation Method:** Manual testing

---

### REQ-026: Dashboard Data Refresh & Caching

**Description:**  
Dashboard data must be cached efficiently with intelligent refresh strategies.

**Acceptance Criteria:**
- [ ] Initial dashboard load checks cache before API call
- [ ] Cache hit: data served immediately (<100ms)
- [ ] Cache miss: API call with loading indicator
- [ ] API call completed: data refreshed, cache updated
- [ ] Cache TTL: 30 seconds for dashboard data (configurable)
- [ ] Manual refresh ignores cache, forces API call
- [ ] Failed API calls fall back to cache without error interruption
- [ ] Cache invalidation on mutations (new reservation, etc.)

**Priority:** Must Have  
**Dependencies:** REQ-021, REQ-040  
**Validation Method:** Performance test + integration test

---

### REQ-027: Role-Based Dashboard Customization

**Description:**  
Different roles see customized dashboard layouts with relevant widgets.

**Acceptance Criteria:**
- [ ] Manager: sees full dashboard (all 8 widgets)
- [ ] Chef: sees Orders, Menu Items, Staff Status (3 widgets)
- [ ] Staff: sees Reservations, Orders, simplified Staff Status (2 widgets)
- [ ] Admin: sees Dashboard + System Health, User Activity (2 additional)
- [ ] Widget layout preserved on application restart
- [ ] Widget visibility configurable per role

**Priority:** Should Have  
**Dependencies:** REQ-001, REQ-021  
**Validation Method:** Unit test + manual testing

---

### REQ-028: System Health Status Widget

**Description:**  
Display system health indicators (API connectivity, database, cache).

**Acceptance Criteria:**
- [ ] Health check endpoint: `/api/health`
- [ ] Show API status: connected (green), degraded (yellow), offline (red)
- [ ] Show database status indicator
- [ ] Show cache status and hit rate percentage
- [ ] Last health check timestamp displayed
- [ ] Manual health check button
- [ ] Admin users only (REQ-004)
- [ ] Health check runs every 60 seconds

**Priority:** Should Have  
**Dependencies:** REQ-021, REQ-028  
**Validation Method:** Integration test

---

### REQ-029: Revenue Summary Widget (Manager Only)

**Description:**  
Display daily/weekly revenue metrics (if authorization permits).

**Acceptance Criteria:**
- [ ] Show today's revenue in currency
- [ ] Show weekly revenue trend
- [ ] Click for detailed breakdown by category
- [ ] Manager/Admin roles only
- [ ] Data from reports API endpoint
- [ ] Update frequency: 5 minutes
- [ ] Offline mode shows cached data with timestamp

**Priority:** Should Have  
**Dependencies:** REQ-021, REQ-101  
**Validation Method:** Integration test

---

### REQ-030: Performance Monitoring (Admin Only)

**Description:**  
Admin dashboard includes performance metrics for system optimization.

**Acceptance Criteria:**
- [ ] Show API response time: average/min/max
- [ ] Database query performance indicators
- [ ] Cache hit rate percentage
- [ ] Memory usage of desktop application
- [ ] Active connection count to API
- [ ] Data collected every 10 seconds
- [ ] Chart visualization of trends (last 1 hour)
- [ ] Alert if any metric exceeds threshold

**Priority:** Nice to Have  
**Dependencies:** REQ-021  
**Validation Method:** Manual testing

---

## Menu Management

### REQ-041: View Menu Items

**Description:**  
Users must be able to view all menu items with filtering and search capabilities.

**Acceptance Criteria:**
- [ ] Menu view accessible from main navigation
- [ ] Display list of all menu items with: ID, name, description, category, price
- [ ] Display bilingual names (English and Arabic)
- [ ] Search menu items by name or category
- [ ] Filter by category dropdown
- [ ] Filter by availability status (available/unavailable)
- [ ] Sort by name, price, or category
- [ ] Pagination: 20 items per page
- [ ] Item count displayed: "Total: 147 items"
- [ ] Load time <2 seconds for full menu
- [ ] Offline mode shows cached menu

**Priority:** Must Have  
**Dependencies:** REQ-001  
**Validation Method:** Integration test + UI test

---

### REQ-042: Create New Menu Item

**Description:**  
Authorized users (Chef, Manager, Admin) can create new menu items.

**Acceptance Criteria:**
- [ ] "Add Menu Item" button visible for authorized roles
- [ ] Form fields: Name (EN), Name (AR), Description (EN), Description (AR), Category, Price, Availability
- [ ] Category dropdown populated from backend
- [ ] Price field accepts decimal (0.00-9999.99)
- [ ] Form validation per REQ-008
- [ ] Name required, max 100 chars
- [ ] Description optional, max 500 chars
- [ ] Category required, from predefined list
- [ ] Submit button disabled while processing
- [ ] Success message: "Menu item created successfully"
- [ ] New item appears in list immediately
- [ ] Cache invalidated for menu items
- [ ] Operation logged to audit trail (REQ-005)

**Priority:** Must Have  
**Dependencies:** REQ-001, REQ-004  
**Validation Method:** Integration test + unit test

---

### REQ-043: Edit Menu Item

**Description:**  
Authorized users can modify existing menu items.

**Acceptance Criteria:**
- [ ] Edit button visible for authorized roles
- [ ] Form pre-populated with current item data
- [ ] All fields editable except ID
- [ ] Validation same as creation (REQ-042)
- [ ] Submit button triggers PUT request to `/api/menu/{id}`
- [ ] Optimistic locking: version number checked for conflicts
- [ ] Conflict message if another user edited item simultaneously
- [ ] On conflict, offer options: view changes, reload, or overwrite
- [ ] Success message displays with updated data
- [ ] Item updated in list view immediately
- [ ] Cache invalidated for affected item
- [ ] Operation logged to audit trail with change details

**Priority:** Must Have  
**Dependencies:** REQ-001, REQ-004  
**Validation Method:** Integration test + conflict testing

---

### REQ-044: Delete Menu Item

**Description:**  
Authorized users can remove menu items from the system.

**Acceptance Criteria:**
- [ ] Delete button visible for authorized roles
- [ ] Confirmation dialog: "Are you sure? This action cannot be undone."
- [ ] Confirmation requires checkbox acknowledgment
- [ ] Delete sends request to `/api/menu/{id}` (DELETE)
- [ ] Item removed from list immediately
- [ ] Soft delete on backend (marked inactive, not physically removed)
- [ ] References to item preserved for historical reports
- [ ] Success message: "Menu item deleted"
- [ ] Cache invalidated
- [ ] Operation logged to audit trail with item snapshot
- [ ] Undo option available for 5 minutes (requires admin permission)

**Priority:** Must Have  
**Dependencies:** REQ-001, REQ-004  
**Validation Method:** Integration test + soft delete verification

---

### REQ-045: Bilingual Menu Item Support

**Description:**  
Menu items support both English and Arabic text with proper RTL/LTR rendering.

**Acceptance Criteria:**
- [ ] Each menu item has separate English and Arabic name fields
- [ ] Each menu item has separate English and Arabic description fields
- [ ] Arabic text stored in proper Unicode (UTF-8)
- [ ] Form shows both language fields side-by-side
- [ ] Arabic field displays RTL (right-to-left) text flow
- [ ] English field displays LTR (left-to-right) text flow
- [ ] Validation applied to both language fields
- [ ] Menu display respects application language setting (REQ-121)
- [ ] Search/filter works on both language versions
- [ ] Arabic names properly indexed for search performance

**Priority:** Must Have  
**Dependencies:** REQ-041, REQ-121  
**Validation Method:** Manual testing + unit test

---

### REQ-046: Menu Item Categories

**Description:**  
Menu items are organized into categories for better navigation.

**Acceptance Criteria:**
- [ ] Predefined categories: Appetizers, Mains, Desserts, Beverages, Special Offers
- [ ] Each menu item assigned to exactly one category
- [ ] Category list maintained by Admin users
- [ ] Menu view includes category filtering dropdown
- [ ] Category filter persists across navigation
- [ ] Each category name available in English and Arabic
- [ ] Category count displayed: "12 Appetizers"
- [ ] Drag-and-drop category reordering (optional UI enhancement)

**Priority:** Must Have  
**Dependencies:** REQ-041  
**Validation Method:** Unit test + UI test

---

### REQ-047: Menu Item Pricing

**Description:**  
Menu item prices tracked and validated for business rules.

**Acceptance Criteria:**
- [ ] Price field required, decimal format (0.00-9999.99)
- [ ] Minimum price: 0.01
- [ ] Maximum price: 9999.99
- [ ] Currency symbol displayed: $ or regional equivalent
- [ ] Price change history tracked (optional audit trail)
- [ ] Bulk price adjustment available for manager (adjust by percentage)
- [ ] Price validation prevents negative or zero-value entries
- [ ] Display decimal to 2 places minimum (e.g., "12.50" not "12.5")

**Priority:** Must Have  
**Dependencies:** REQ-041  
**Validation Method:** Unit test + input validation test

---

### REQ-048: Menu Availability Status

**Description:**  
Menu items can be marked as available or unavailable.

**Acceptance Criteria:**
- [ ] Availability toggle: available/unavailable
- [ ] Unavailable items grayed out in menu displays
- [ ] Unavailable items excluded from reservation suggestions
- [ ] Quick availability toggle from list view (checkbox)
- [ ] Availability change effective immediately across all views
- [ ] Historical availability changes logged
- [ ] Bulk availability change available (make all items available/unavailable)
- [ ] Reason for unavailability optional (e.g., "Out of stock")

**Priority:** Must Have  
**Dependencies:** REQ-041  
**Validation Method:** Integration test + UI test

---

### REQ-049: Menu Import/Export (Future)

**Description:**  
Support bulk menu operations via import/export.

**Acceptance Criteria:**
- [ ] Export menu to CSV format (with bilingual support)
- [ ] Export includes: ID, Name_EN, Name_AR, Category, Price, Description_EN, Description_AR
- [ ] Import menu from CSV (create bulk items)
- [ ] Validation warnings before import
- [ ] Dry-run option to preview import results
- [ ] Successful imports logged
- [ ] Manager/Admin roles only

**Priority:** Nice to Have  
**Dependencies:** REQ-041  
**Validation Method:** Manual testing

---

### REQ-050: Menu Change History Audit

**Description:**  
Track all menu item modifications for compliance and analysis.

**Acceptance Criteria:**
- [ ] All menu changes logged: creation, modification, deletion
- [ ] Log includes: timestamp, user_id, action, old_value, new_value
- [ ] History accessible via menu item detail view (admin only)
- [ ] Timeline view of all changes to an item
- [ ] Ability to export change history report
- [ ] Retention: minimum 1 year
- [ ] Search/filter change history by date range, user, or action type

**Priority:** Should Have  
**Dependencies:** REQ-041, REQ-005  
**Validation Method:** Audit trail review

---

## Reservation System

### REQ-061: View Reservations

**Description:**  
Users can view existing reservations with flexible filtering and search.

**Acceptance Criteria:**
- [ ] Reservations accessible from main navigation
- [ ] Calendar view: month, week, day options
- [ ] List view: table with columns (ID, Customer, Party Size, Time, Table, Status)
- [ ] Default filter: today's reservations
- [ ] Date range picker for filtering (from/to dates)
- [ ] Filter by status: all, confirmed, pending, arrived, completed, cancelled
- [ ] Search by customer name, phone, or reservation ID
- [ ] Display pagination: 25 items per page
- [ ] Sort by time, customer name, or party size
- [ ] Color indicators for status (confirmed=green, pending=yellow, etc.)
- [ ] Load time <2 seconds for typical date range
- [ ] Offline mode shows cached reservations with sync indicator

**Priority:** Must Have  
**Dependencies:** REQ-001  
**Validation Method:** Integration test + UI test

---

### REQ-062: Create Reservation

**Description:**  
Authorized users can create new reservations.

**Acceptance Criteria:**
- [ ] "New Reservation" button visible for authorized roles
- [ ] Form fields: Customer Name, Phone, Party Size, Date, Time, Table, Notes
- [ ] Customer name required, max 100 chars
- [ ] Phone: phone number format validated, optional
- [ ] Party size: integer 1-50 (with validation)
- [ ] Date picker: future dates only, max 90 days ahead
- [ ] Time picker: restaurant operating hours only (e.g., 11:00-23:00)
- [ ] Table selection: dropdown of available tables for selected date/time
- [ ] Notes field: optional, max 200 chars
- [ ] Form validation per REQ-008
- [ ] Submit button triggers POST to `/api/reservations`
- [ ] Double-booking prevention: verify table availability (REQ-063)
- [ ] Success message with confirmation number
- [ ] New reservation appears in calendar immediately
- [ ] Cache invalidated
- [ ] Operation logged to audit trail

**Priority:** Must Have  
**Dependencies:** REQ-001, REQ-061  
**Validation Method:** Integration test + conflict testing

---

### REQ-063: Double-Booking Prevention

**Description:**  
System must prevent two reservations for the same table in overlapping time slots.

**Acceptance Criteria:**
- [ ] Before reservation creation, check table availability
- [ ] Query backend: GET `/api/reservations/check-availability?table_id=X&date=Y&time=Z`
- [ ] Table unavailable if conflicting reservation exists
- [ ] Conflict resolution: show available alternative times/tables
- [ ] Optimistic locking: use version numbers for concurrent writes
- [ ] If conflict detected during submission, retry with fresh table list
- [ ] Failed submission shows: "Table not available. Suggested times: [list]"
- [ ] Maximum reservation duration: 2 hours (configurable)
- [ ] Concurrent booking attempts handled by backend (last-write-wins)

**Priority:** Must Have  
**Dependencies:** REQ-061, REQ-062  
**Validation Method:** Integration test + concurrency testing

---

### REQ-064: Edit Reservation

**Description:**  
Authorized users can modify existing reservations.

**Acceptance Criteria:**
- [ ] Edit button visible for authorized roles
- [ ] Form pre-populated with current reservation data
- [ ] Editable fields: Customer Name, Phone, Party Size, Time, Table, Notes
- [ ] Date not editable (cancel + recreate if date change needed)
- [ ] Double-booking check applies to edits (REQ-063)
- [ ] Validation same as creation (REQ-062)
- [ ] Submit triggers PUT to `/api/reservations/{id}`
- [ ] Optimistic locking: version conflict handling per REQ-063
- [ ] Success message with updated details
- [ ] Reservation updated in calendar immediately
- [ ] Cache invalidated
- [ ] Change notification sent to customer (if email available)
- [ ] Operation logged to audit trail with change details

**Priority:** Must Have  
**Dependencies:** REQ-061, REQ-062  
**Validation Method:** Integration test

---

### REQ-065: Cancel Reservation

**Description:**  
Users can cancel reservations with appropriate notifications.

**Acceptance Criteria:**
- [ ] Cancel button visible for authorized roles
- [ ] Confirmation dialog: "Cancel reservation for [Customer]? This action sends cancellation notice."
- [ ] Cancellation reason dropdown: optional (no-show, customer request, venue issue)
- [ ] DELETE request to `/api/reservations/{id}`
- [ ] Reservation status marked as "cancelled" (soft delete)
- [ ] Reservation remains in history for reporting
- [ ] Cancellation notification sent to customer email (async)
- [ ] Table becomes available for future bookings
- [ ] Success message: "Reservation cancelled"
- [ ] Removed from active calendar view (but visible in filters)
- [ ] Undo option available for 24 hours (admin restore)
- [ ] Operation logged to audit trail with reason

**Priority:** Must Have  
**Dependencies:** REQ-061, REQ-062  
**Validation Method:** Integration test

---

### REQ-066: Reservation Check-In

**Description:**  
Staff can mark reservations as checked-in when customers arrive.

**Acceptance Criteria:**
- [ ] Check-in button visible for staff/manager roles
- [ ] Only available for "confirmed" reservations
- [ ] Clicking check-in updates status to "arrived"
- [ ] System records check-in time (current timestamp)
- [ ] Check-in triggers PATCH to `/api/reservations/{id}/check-in`
- [ ] Success message: "Customer [name] checked in"
- [ ] Reservation moved to "active" section of dashboard
- [ ] Visual indicator: table now occupied
- [ ] Late arrival handling: timestamp shows delay
- [ ] Check-in within 30 minutes of scheduled time (warning if later)

**Priority:** Should Have  
**Dependencies:** REQ-061  
**Validation Method:** Integration test

---

### REQ-067: Reservation Notes & Communication

**Description:**  
Users can add and view notes, with special dietary/accessibility needs tracking.

**Acceptance Criteria:**
- [ ] Notes field in reservation form (max 200 chars)
- [ ] Special requests section: dietary restrictions, accessibility needs, preferences
- [ ] Common templates: "Vegetarian", "Gluten-free", "Wheelchair accessible", "High chair needed"
- [ ] Notes visible in calendar/list views with indicator icon
- [ ] Notes copied to kitchen/staff when order placed
- [ ] History of all notes modifications tracked
- [ ] Alert icon if special requests present (draws attention)
- [ ] Staff can update notes at check-in

**Priority:** Should Have  
**Dependencies:** REQ-061  
**Validation Method:** Integration test + UI test

---

### REQ-068: Walk-In Reservations

**Description:**  
Staff can create walk-in reservations on-the-fly.

**Acceptance Criteria:**
- [ ] "Walk-In" button on reservations screen
- [ ] Quick form: Customer Name (optional), Party Size, Table
- [ ] Auto-populate current date and time
- [ ] Minimal validation (party size required)
- [ ] Creates reservation with status "arrived" (pre-checked-in)
- [ ] No advance notice sent to customer
- [ ] Logged as "Walk-in" type for reporting
- [ ] Time stamp recorded for accurate occupancy tracking

**Priority:** Should Have  
**Dependencies:** REQ-061, REQ-062  
**Validation Method:** Integration test

---

### REQ-069: Reservation Reminders (Future)

**Description:**  
System sends automated reminders to customers before reservations.

**Acceptance Criteria:**
- [ ] Reminder sent 24 hours before reservation (configurable)
- [ ] Reminder sent 2 hours before reservation (configurable)
- [ ] Sent via email and/or SMS (if configured)
- [ ] Reminder includes: reservation details, cancellation link, restaurant contact
- [ ] Reminder status tracked: sent, bounced, clicked
- [ ] Customer can confirm/cancel from reminder
- [ ] Opt-out available in customer profile (backend)
- [ ] Admin can trigger manual reminder

**Priority:** Nice to Have  
**Dependencies:** REQ-061  
**Validation Method:** Integration test

---

### REQ-070: Reservation Reporting

**Description:**  
Managers and admin can generate reservation reports for analysis.

**Acceptance Criteria:**
- [ ] Reports accessible from Reports menu
- [ ] Report types: Daily Summary, Weekly Trend, Customer Analysis, No-Show Report
- [ ] Daily Summary: reservations, walk-ins, cancellations, no-shows
- [ ] Date range filters with presets (Today, This Week, This Month, Custom)
- [ ] Export to CSV or PDF
- [ ] Performance metrics: average party size, occupancy rate, cancellation rate
- [ ] Customer repeat visit analysis: first-time vs. repeat guests
- [ ] No-show tracking and trends
- [ ] Report caching (same report within 1 hour reused)
- [ ] Manager/Admin roles only

**Priority:** Should Have  
**Dependencies:** REQ-061, REQ-101  
**Validation Method:** Integration test

---

## Staff Management

### REQ-081: View Staff Directory

**Description:**  
Managers and admin can view all staff members with their roles and contact information.

**Acceptance Criteria:**
- [ ] Staff directory accessible from main navigation
- [ ] Display table: ID, Name, Role, Email, Phone, Department, Status
- [ ] Filter by role: Manager, Chef, Waiter, Admin
- [ ] Filter by department: Front-of-House, Back-of-House, Management
- [ ] Filter by status: active, inactive, on-leave
- [ ] Search by name, email, or phone
- [ ] Sort by name, role, or hire date
- [ ] Pagination: 20 staff per page
- [ ] Inactive staff shown with grayed-out indicator
- [ ] Total staff count displayed
- [ ] Manager/Admin roles only

**Priority:** Must Have  
**Dependencies:** REQ-001, REQ-004  
**Validation Method:** Integration test + UI test

---

### REQ-082: Create Staff Member

**Description:**  
Admin users can add new staff members to the system.

**Acceptance Criteria:**
- [ ] "Add Staff Member" button visible for admin
- [ ] Form fields: First Name, Last Name, Email, Phone, Role, Department, Hire Date, Notes
- [ ] First/Last name required, max 50 chars each
- [ ] Email required, unique, valid email format
- [ ] Phone optional, valid phone format
- [ ] Role required: dropdown (Manager, Chef, Waiter, Admin)
- [ ] Department required: dropdown (Front-of-House, Back-of-House, Management)
- [ ] Hire date: date picker
- [ ] Notes optional, max 200 chars
- [ ] Form validation per REQ-008
- [ ] Submit creates user account with temporary password (sent via email)
- [ ] New staff member appears in directory immediately
- [ ] Operation logged to audit trail
- [ ] Email sent to new staff with login credentials

**Priority:** Must Have  
**Dependencies:** REQ-001, REQ-004  
**Validation Method:** Integration test

---

### REQ-083: Edit Staff Member

**Description:**  
Admin users can modify staff member information.

**Acceptance Criteria:**
- [ ] Edit button visible for admin
- [ ] Form pre-populated with current information
- [ ] Editable fields: All except ID and email (email change via separate flow)
- [ ] Validation same as creation (REQ-082)
- [ ] Submit triggers PUT to `/api/staff/{id}`
- [ ] Successful update message
- [ ] Staff member updated in directory immediately
- [ ] Role/department changes take effect immediately
- [ ] Operation logged to audit trail with change details
- [ ] Staff member notified of role changes (optional)

**Priority:** Must Have  
**Dependencies:** REQ-081  
**Validation Method:** Integration test

---

### REQ-084: Deactivate Staff Member

**Description:**  
Admin can deactivate staff members without deleting records.

**Acceptance Criteria:**
- [ ] "Deactivate" button visible for admin
- [ ] Confirmation: "Deactivate [Name]? They will lose system access."
- [ ] Deactivation sets status to "inactive"
- [ ] Staff member login disabled immediately
- [ ] Inactive staff appear in directory with visual indicator
- [ ] Historical records preserved for audit/payroll
- [ ] Reactivation available (reverse deactivation)
- [ ] Operation logged to audit trail
- [ ] Deactivated staff cannot be assigned to future shifts
- [ ] PATCH request to `/api/staff/{id}/deactivate`

**Priority:** Should Have  
**Dependencies:** REQ-081  
**Validation Method:** Integration test

---

### REQ-085: Staff Role & Permissions Management

**Description:**  
System manages staff roles and associated permissions.

**Acceptance Criteria:**
- [ ] Four roles defined: Manager, Chef, Waiter, Admin
- [ ] Each role has predefined permission set
- [ ] Manager: menu/reservation/staff management, limited reports
- [ ] Chef: menu editing, order management, kitchen operations
- [ ] Waiter: reservations, customer communication, basic features
- [ ] Admin: full system access, configuration, user management
- [ ] Role-based menu hiding (unavailable features greyed out)
- [ ] Permissions checked at API level (defense-in-depth)
- [ ] Custom permissions possible for future extensions (architecture ready)

**Priority:** Must Have  
**Dependencies:** REQ-001, REQ-004  
**Validation Method:** Unit test + integration test

---

### REQ-086: Staff Schedule Management

**Description:**  
Track and manage staff working schedules.

**Acceptance Criteria:**
- [ ] Schedule view accessible from staff directory
- [ ] Calendar view: week/month display
- [ ] Drag-and-drop shift assignment (optional UI enhancement)
- [ ] Shift details: date, time, role assigned, notes
- [ ] Staff member shift list shows scheduled shifts
- [ ] Manager can create/edit shifts for staff
- [ ] Shift conflicts detected (double-booking same staff)
- [ ] Manager/Admin roles only
- [ ] Shift history preserved for audit
- [ ] Availability calendar shows who's working each day

**Priority:** Should Have  
**Dependencies:** REQ-081  
**Validation Method:** Integration test

---

### REQ-087: Staff Performance Tracking

**Description:**  
Managers can track basic staff performance metrics.

**Acceptance Criteria:**
- [ ] Performance view in staff detail page
- [ ] Metrics tracked: orders processed, reservations handled, customer feedback ratings
- [ ] Period selectors: Today, This Week, This Month
- [ ] Performance displayed as trend or comparison
- [ ] Manager/Admin roles only
- [ ] Data aggregated from backend analytics
- [ ] Notes on performance available for manager documentation
- [ ] Historical performance data retained for annual reviews

**Priority:** Should Have  
**Dependencies:** REQ-081, REQ-101  
**Validation Method:** Integration test

---

### REQ-088: Staff Communication Features

**Description:**  
Enable internal messaging between staff members and managers.

**Acceptance Criteria:**
- [ ] Messaging system accessible from dashboard
- [ ] Send message to individual staff member or group
- [ ] Message history searchable
- [ ] Unread message badge
- [ ] Manager can broadcast announcements to all staff
- [ ] Messages timestamped and attributed to sender
- [ ] 30-day message retention (older messages auto-deleted)
- [ ] Notification sound optional (configurable)
- [ ] Offline: queue messages for sending on reconnect

**Priority:** Nice to Have  
**Dependencies:** REQ-081  
**Validation Method:** Integration test

---

### REQ-089: Staff Performance Reports

**Description:**  
Generate reports on individual and team performance metrics.

**Acceptance Criteria:**
- [ ] Reports accessible from Reports menu
- [ ] Report types: Individual Performance, Team Summary, Top Performers
- [ ] Period filters: daily, weekly, monthly, custom range
- [ ] Metrics: orders processed, reservation handling, customer ratings
- [ ] Export to PDF or CSV
- [ ] Manager/Admin roles only
- [ ] Data aggregated from backend analytics

**Priority:** Should Have  
**Dependencies:** REQ-081, REQ-101  
**Validation Method:** Integration test

---

### REQ-090: Staff Directory Export

**Description:**  
Admin can export staff directory for external use.

**Acceptance Criteria:**
- [ ] Export button in staff directory
- [ ] Export format: CSV (includes name, role, email, phone, hire date)
- [ ] PII exported only to authorized users (audit logged)
- [ ] Exported file marked confidential
- [ ] Admin roles only

**Priority:** Nice to Have  
**Dependencies:** REQ-081  
**Validation Method:** Manual testing

---

## Reports & Analytics

### REQ-101: Revenue Report

**Description:**  
Managers can view and export revenue reports with various time periods.

**Acceptance Criteria:**
- [ ] Revenue report accessible from Reports menu
- [ ] Display total revenue for selected period
- [ ] Period selection: Today, This Week, This Month, Custom Range
- [ ] Breakdown by: category, payment method, reservation vs walk-in
- [ ] Charts: line chart (daily trend), pie chart (breakdown)
- [ ] Data refresh: 5-minute cache (manager can force refresh)
- [ ] Export to PDF or CSV
- [ ] Offline mode shows last known data with timestamp
- [ ] Manager/Admin roles only
- [ ] Drill-down: click category to see itemized revenue

**Priority:** Should Have  
**Dependencies:** REQ-001, REQ-004  
**Validation Method:** Integration test + report generation test

---

### REQ-102: Occupancy Report

**Description:**  
Analyze table occupancy rates and capacity utilization.

**Acceptance Criteria:**
- [ ] Report accessible from Reports menu
- [ ] Display occupancy percentage by time slot
- [ ] Occupancy timeline: hourly breakdown for selected day/week/month
- [ ] Show peak vs off-peak hours
- [ ] Compare occupancy year-over-year (if historical data available)
- [ ] Identify underutilized time slots (recommendations)
- [ ] Export to PDF or CSV
- [ ] Manager/Admin roles only
- [ ] Data aggregated from reservation and check-in logs

**Priority:** Should Have  
**Dependencies:** REQ-061, REQ-066  
**Validation Method:** Integration test

---

### REQ-103: Menu Performance Report

**Description:**  
Analyze which menu items are most/least popular.

**Acceptance Criteria:**
- [ ] Report accessible from Reports menu
- [ ] Show item frequency (times ordered) by period
- [ ] Show revenue generated per item
- [ ] Rank items by popularity
- [ ] Show items with low sales (consider discontinuing)
- [ ] Identify trending items
- [ ] Period selector: daily, weekly, monthly, custom
- [ ] Export to PDF or CSV
- [ ] Manager/Chef/Admin roles
- [ ] Data sourced from orders API

**Priority:** Should Have  
**Dependencies:** REQ-041, REQ-101  
**Validation Method:** Integration test

---

### REQ-104: Customer Insights Report

**Description:**  
Analyze customer patterns and behavior.

**Acceptance Criteria:**
- [ ] Report accessible from Reports menu
- [ ] Metrics: total unique customers, repeat visitors percentage
- [ ] Customer segmentation: high-value, regular, occasional, one-time
- [ ] Average party size by customer type
- [ ] Average spend per visit
- [ ] Peak customer acquisition times
- [ ] Top customers by spend
- [ ] Export to PDF or CSV
- [ ] Manager/Admin roles only
- [ ] Privacy compliance: no PII in exports by default

**Priority:** Should Have  
**Dependencies:** REQ-061, REQ-101  
**Validation Method:** Integration test

---

### REQ-105: No-Show Analysis Report

**Description:**  
Track and analyze reservation no-shows for business insights.

**Acceptance Criteria:**
- [ ] Report accessible from Reports menu
- [ ] No-show rate by period (daily/weekly/monthly)
- [ ] Identify patterns: day of week, time of day, party size
- [ ] Customer no-show history (repeat offenders)
- [ ] Impact analysis: tables wasted, revenue lost
- [ ] Trend visualization
- [ ] Manager/Admin roles only
- [ ] Export to PDF or CSV

**Priority:** Should Have  
**Dependencies:** REQ-061, REQ-070  
**Validation Method:** Integration test

---

### REQ-106: Performance Metrics Dashboard

**Description:**  
Real-time performance metrics for operational oversight.

**Acceptance Criteria:**
- [ ] Metrics display: KPIs for current day/week/month
- [ ] Key metrics: Total Reservations, Walk-ins, Occupancy %, Avg Check Size, Revenue
- [ ] Comparison to previous period (week-over-week, month-over-month)
- [ ] Color indicators: green (good), yellow (warning), red (critical)
- [ ] Auto-refresh every 5 minutes
- [ ] Manager/Admin roles only
- [ ] Mobile-friendly layout
- [ ] Export summary to email (schedule reports)

**Priority:** Should Have  
**Dependencies:** REQ-021, REQ-101  
**Validation Method:** Integration test + UI test

---

### REQ-107: Staff Utilization Report

**Description:**  
Analyze staff productivity and utilization rates.

**Acceptance Criteria:**
- [ ] Report accessible from Reports menu
- [ ] Metrics: hours worked, orders processed, reservations handled
- [ ] Staff comparison: top performers, underutilized staff
- [ ] Utilization by role: managers, chefs, waiters
- [ ] Period selector: daily, weekly, monthly, custom
- [ ] Drill-down: click staff member to see detailed activity
- [ ] Export to PDF or CSV
- [ ] Manager/Admin roles only
- [ ] Data aggregated from shift logs and order tracking

**Priority:** Should Have  
**Dependencies:** REQ-081, REQ-087  
**Validation Method:** Integration test

---

### REQ-108: Report Scheduling

**Description:**  
Admin can schedule automatic report generation and delivery.

**Acceptance Criteria:**
- [ ] Scheduling interface in Reports menu
- [ ] Supported schedules: daily, weekly, monthly
- [ ] Email delivery of generated reports
- [ ] Report format: PDF or CSV (selectable)
- [ ] Multiple recipients configurable
- [ ] Scheduled report list with enable/disable toggle
- [ ] Edit/delete scheduled reports
- [ ] Test send option to verify delivery
- [ ] Admin roles only

**Priority:** Nice to Have  
**Dependencies:** REQ-101+  
**Validation Method:** Integration test

---

### REQ-109: Report Caching & Performance

**Description:**  
Report generation optimized for performance with intelligent caching.

**Acceptance Criteria:**
- [ ] Reports cached for 1 hour (if parameters identical)
- [ ] Cache invalidated on data mutations
- [ ] Large reports processed asynchronously (show progress)
- [ ] Estimated generation time displayed before generating
- [ ] Maximum report time limit: 30 seconds
- [ ] Reports >100MB offloaded to background job queue
- [ ] Download link provided when ready
- [ ] Cached reports available offline (last-known-good)

**Priority:** Should Have  
**Dependencies:** REQ-101+  
**Validation Method:** Performance test

---

### REQ-110: Advanced Analytics (Future)

**Description:**  
Advanced analytics and predictive features for business optimization.

**Acceptance Criteria:**
- [ ] Predictive analytics: forecast future occupancy/revenue
- [ ] Trend analysis: identify patterns and anomalies
- [ ] Recommendation engine: suggest menu adjustments, pricing strategies
- [ ] Cohort analysis: customer segmentation and lifetime value
- [ ] Churn prediction: identify at-risk customers
- [ ] Machine learning model integration (future)
- [ ] Admin/Analytics roles only

**Priority:** Nice to Have  
**Dependencies:** REQ-101+  
**Validation Method:** Manual testing (feature preview)

---

## Localization & UI

### REQ-121: English/Arabic Bilingual UI

**Description:**  
The application must support both English and Arabic user interfaces with seamless language switching.

**Acceptance Criteria:**
- [ ] All UI text available in English and Arabic
- [ ] Language selection on login screen
- [ ] Language preference persisted across sessions
- [ ] Runtime language switching without application restart
- [ ] Both languages complete (no mixed-language screens)
- [ ] Proper translation for all UI elements, buttons, labels, messages
- [ ] Technical terms appropriately translated or transliterated

**Priority:** Must Have  
**Dependencies:** None  
**Validation Method:** Manual testing + translation review

---

### REQ-122: Right-to-Left (RTL) Support for Arabic

**Description:**  
Arabic interface displays with proper right-to-left text flow and layout.

**Acceptance Criteria:**
- [ ] Arabic UI mirrors English layout (RTL flow)
- [ ] Text alignment right-aligned for Arabic content
- [ ] Buttons and controls repositioned for RTL (e.g., OK button moves to left)
- [ ] Icons and arrows mirrored appropriately (left→right arrows become right→left)
- [ ] Form labels right-aligned in Arabic mode
- [ ] Table columns reordered (rightmost = first column in Arabic)
- [ ] Menu items hierarchy preserved with proper indentation
- [ ] Data grids header text right-aligned in Arabic
- [ ] Scrollbars positioned on left in RTL mode
- [ ] Responsive layout adapts to RTL without truncation

**Priority:** Must Have  
**Dependencies:** REQ-121  
**Validation Method:** Manual UI testing + RTL-specific testing

---

### REQ-123: Number & Date Localization

**Description:**  
Numbers, dates, and currencies display in locale-appropriate format.

**Acceptance Criteria:**
- [ ] Numbers formatted per locale (e.g., 1,000.50 vs 1.000,50)
- [ ] Dates formatted per locale: MM/DD/YYYY (en) vs DD/MM/YYYY (ar)
- [ ] Time format: 12-hour (en, optional) vs 24-hour (ar, typical)
- [ ] Currency symbol and format: $ (en) vs other (ar locale)
- [ ] Decimal separator: . (English) vs , (some locales)
- [ ] Thousand separator respected
- [ ] Percent format: 50% vs 50٪ (Arabic)

**Priority:** Should Have  
**Dependencies:** REQ-121  
**Validation Method:** Manual testing + locale testing

---

### REQ-124: Date/Time Pickers for Both Languages

**Description:**  
Date and time input controls support both language modes.

**Acceptance Criteria:**
- [ ] Date picker displays calendar in selected language
- [ ] Month/day names translated
- [ ] Time picker shows time in 24-hour format
- [ ] Keyboard input accepted in both languages (numeric entry same)
- [ ] Validation messages in selected language
- [ ] Keyboard shortcuts work in both layouts (Alt+D for date, etc.)

**Priority:** Should Have  
**Dependencies:** REQ-121, REQ-122  
**Validation Method:** Integration test + manual testing

---

### REQ-125: Localized Error & Help Messages

**Description:**  
All user-facing messages (errors, confirmations, help) translated to both languages.

**Acceptance Criteria:**
- [ ] Error messages translated
- [ ] Confirmation dialogs translated ("OK", "Cancel", "Yes", "No")
- [ ] Help text/tooltips translated
- [ ] Input validation messages translated
- [ ] System messages (network errors, etc.) translated
- [ ] Message placeholders (names, numbers) work in both languages
- [ ] Plural handling correct in both languages
- [ ] Special characters/diacritics rendered correctly

**Priority:** Must Have  
**Dependencies:** REQ-121  
**Validation Method:** Manual testing + translation review

---

### REQ-126: Context Menu Localization

**Description:**  
Right-click context menus fully localized.

**Acceptance Criteria:**
- [ ] Context menu items translated (Copy, Paste, Delete, etc.)
- [ ] Context menu layout adapts to RTL
- [ ] Keyboard shortcuts displayed in context (e.g., "Ctrl+C")
- [ ] Submenu indicators properly positioned (arrows mirrored for RTL)

**Priority:** Should Have  
**Dependencies:** REQ-121  
**Validation Method:** Manual testing

---

### REQ-127: Responsive UI Layout

**Description:**  
UI adapts responsively to different screen sizes and DPI settings.

**Acceptance Criteria:**
- [ ] Minimum supported resolution: 1920x1080
- [ ] UI scales appropriately for higher DPI (125%, 150%, 200%)
- [ ] Text remains readable at all DPI levels
- [ ] Controls remain clickable (adequate sizing)
- [ ] No content truncation or overflow
- [ ] Window can be resized; controls reflow appropriately
- [ ] Horizontal scrolling minimized
- [ ] Touch-friendly on high-DPI displays (if touch-enabled)

**Priority:** Should Have  
**Dependencies:** None  
**Validation Method:** Manual testing on various DPI/resolutions

---

### REQ-128: Accessibility - WCAG 2.1 AA Compliance

**Description:**  
Application meets WCAG 2.1 Level AA accessibility standards.

**Acceptance Criteria:**
- [ ] Keyboard navigation: all features accessible via keyboard
- [ ] Tab order logical and predictable
- [ ] Focus indicators visible
- [ ] Color contrast ratio: 4.5:1 for normal text, 3:1 for large text
- [ ] No color alone used to convey information
- [ ] Sufficient color contrast in charts/graphs
- [ ] Screen reader compatible (NVDA/JAWS testing recommended)
- [ ] Alt text for images/icons
- [ ] Labels associated with form fields
- [ ] Error messages identify field and describe issue
- [ ] Form instructions clear and accessible
- [ ] Sufficient touch target size (48x48px minimum)
- [ ] No time-based content change without user control

**Priority:** Should Have  
**Dependencies:** All UI requirements  
**Validation Method:** Automated WCAG scanner + manual testing + assistive tech testing

---

### REQ-129: Keyboard Navigation

**Description:**  
All application features accessible and efficient via keyboard.

**Acceptance Criteria:**
- [ ] Tab key navigates all interactive elements
- [ ] Tab order follows logical flow (top to bottom, left to right)
- [ ] Shift+Tab navigates backward
- [ ] Enter/Space activates buttons and toggles
- [ ] Arrow keys navigate lists/grids/menus
- [ ] Escape closes dialogs/menus
- [ ] Alt+letter keyboard shortcuts for main menu items
- [ ] Ctrl+N = New item (common convention)
- [ ] Ctrl+S = Save
- [ ] Ctrl+Z = Undo (if applicable)
- [ ] F1 = Help
- [ ] No keyboard traps (focus cannot escape an element)

**Priority:** Should Have  
**Dependencies:** None  
**Validation Method:** Manual keyboard testing

---

### REQ-130: Screen Reader Support

**Description:**  
Application compatible with screen readers (NVDA, JAWS).

**Acceptance Criteria:**
- [ ] All text content readable by screen reader
- [ ] Buttons/controls identified by screen reader
- [ ] Form labels associated with inputs
- [ ] Menu structure announced correctly
- [ ] Dialog windows announced as dialogs
- [ ] Alt text for meaningful images
- [ ] No audio-only content
- [ ] Dynamic content changes announced
- [ ] List/table structure preserved for screen reader

**Priority:** Should Have  
**Dependencies:** REQ-128  
**Validation Method:** Manual testing with NVDA/JAWS

---

### REQ-131 through REQ-140: Additional Localization & UI Requirements

### REQ-131: Theme & Color Accessibility

**Description:**  
UI themes and color schemes meet accessibility standards.

**Acceptance Criteria:**
- [ ] Light theme: high contrast, readable text
- [ ] Dark theme option available (optional)
- [ ] Color pairs tested for color-blind accessibility (deuteranopia/protanopia)
- [ ] Status indicators not color-only (use icons + color)
- [ ] Chart colors distinguishable for colorblind users
- [ ] Theme preference persisted across sessions

**Priority:** Should Have  
**Dependencies:** REQ-128  
**Validation Method:** Color contrast testing + manual testing

---

### REQ-132: Help System

**Description:**  
Comprehensive in-app help system for user guidance.

**Acceptance Criteria:**
- [ ] Help menu accessible from main menu or Help icon
- [ ] Help topics covering all major features
- [ ] Search functionality in help
- [ ] Tooltips for complex controls
- [ ] Tooltips appear on hover (800ms delay)
- [ ] Tooltips closable with Escape
- [ ] Context-sensitive help (F1 in forms)
- [ ] Video tutorials (optional future feature)
- [ ] Help in both English and Arabic

**Priority:** Nice to Have  
**Dependencies:** REQ-121  
**Validation Method:** Manual testing

---

### REQ-133: User Preferences & Settings

**Description:**  
Users can customize application behavior via preferences.

**Acceptance Criteria:**
- [ ] Settings accessible via Tools → Options or similar
- [ ] Preferences saved: language, theme, auto-refresh interval
- [ ] Optional: font size adjustment
- [ ] Optional: color scheme customization
- [ ] Optional: keyboard shortcut customization (future)
- [ ] Settings persisted across application sessions
- [ ] Reset to defaults option available
- [ ] Preferences stored locally (not synced to backend)

**Priority:** Should Have  
**Dependencies:** REQ-121  
**Validation Method:** Integration test

---

### REQ-134: Print-Friendly Views

**Description:**  
Key reports and data views printable in user-friendly format.

**Acceptance Criteria:**
- [ ] Print button available on dashboard, reports, reservation details
- [ ] Print preview shown before printing
- [ ] Unnecessary UI elements (toolbars, headers) hidden in print
- [ ] Colors adjusted for black/white printing (good contrast)
- [ ] Page breaks inserted logically (not mid-row)
- [ ] Header/footer with document title and date
- [ ] Bilingual support: print in selected language
- [ ] PDF export alternative to printing
- [ ] Print layout responsive to page size

**Priority:** Nice to Have  
**Dependencies:** REQ-021+  
**Validation Method:** Manual testing

---

### REQ-135: Undo/Redo Functionality

**Description:**  
Critical operations support undo/redo for error recovery.

**Acceptance Criteria:**
- [ ] Undo stack size: last 50 operations
- [ ] Ctrl+Z triggers undo (previous state restored)
- [ ] Ctrl+Y or Ctrl+Shift+Z triggers redo
- [ ] Undo menu shows last operation
- [ ] Undo/Redo buttons in toolbar (if applicable)
- [ ] Undo disabled when stack empty
- [ ] Redo disabled when at latest state
- [ ] Transactional: undo entire multi-step operation atomically
- [ ] Limited scope: undo affects current form/view only

**Priority:** Should Have  
**Dependencies:** None  
**Validation Method:** Manual testing

---

### REQ-136: Search & Find Functionality

**Description:**  
Global search and find-in-page capabilities.

**Acceptance Criteria:**
- [ ] Ctrl+F opens find dialog (in data grids)
- [ ] Find highlights matching text in yellow
- [ ] Find next/previous buttons
- [ ] Case-sensitive search option
- [ ] Match whole word option
- [ ] Result count displayed
- [ ] Escape closes find dialog
- [ ] Search in bilingual content (English or Arabic)
- [ ] Global search across all views (future enhancement)

**Priority:** Nice to Have  
**Dependencies:** REQ-121  
**Validation Method:** Manual testing

---

### REQ-137: Status Bar Information

**Description:**  
Status bar displays contextual information and status.

**Acceptance Criteria:**
- [ ] Status bar at application bottom
- [ ] Connection status indicator (online/offline)
- [ ] Current user name displayed
- [ ] Last sync timestamp
- [ ] Operation status (processing, idle, error)
- [ ] Row/item count in current view
- [ ] Notifications/alerts displayed briefly in status bar
- [ ] Clickable status elements (e.g., click connection status for details)

**Priority:** Should Have  
**Dependencies:** None  
**Validation Method:** Manual testing

---

### REQ-138: Toolbar Customization (Optional)

**Description:**  
Advanced users can customize toolbar buttons.

**Acceptance Criteria:**
- [ ] Right-click toolbar → "Customize Toolbar"
- [ ] Add/remove buttons from toolbar
- [ ] Reorder buttons via drag-and-drop
- [ ] Reset toolbar to defaults option
- [ ] Customization persisted across sessions
- [ ] Hide/show text labels on buttons

**Priority:** Nice to Have  
**Dependencies:** None  
**Validation Method:** Manual testing

---

### REQ-139: Form Autosave

**Description:**  
Long forms auto-save drafts to prevent data loss.

**Acceptance Criteria:**
- [ ] Forms with >5 fields auto-save to local SQLite every 30 seconds
- [ ] User notified of auto-save: small "Saved" message
- [ ] Unsaved changes indicator on form title
- [ ] On close with unsaved changes: prompt to save
- [ ] Autosave persisted across application crashes/restarts
- [ ] Recovery option: "Restore unsaved changes" on app restart

**Priority:** Should Have  
**Dependencies:** None  
**Validation Method:** Integration test

---

### REQ-140: Export/Import Confirmation Dialogs

**Description:**  
Data export/import operations show clear confirmation and status.

**Acceptance Criteria:**
- [ ] Export dialog shows: file location, format, number of records
- [ ] Import dialog shows: preview of data, conflict resolution options
- [ ] Progress bar for large exports (>10MB)
- [ ] Completion dialog with success count and any errors
- [ ] Success message: "150 records exported to file.csv"
- [ ] Error message with specific details if export fails
- [ ] Export file path copyable to clipboard

**Priority:** Nice to Have  
**Dependencies:** REQ-041+  
**Validation Method:** Manual testing

---

## Infrastructure & Deployment

### REQ-141: .NET 8+ Framework

**Description:**  
Application built on .NET 8 or higher framework.

**Acceptance Criteria:**
- [ ] Target framework: .NET 8.0 or higher
- [ ] Project file (.csproj) specifies TargetFramework >= net8.0
- [ ] Leverages latest .NET language features
- [ ] All dependencies compatible with .NET 8+
- [ ] Build/run on Windows with .NET 8 Runtime installed
- [ ] Can target .NET 9+ in future without major refactoring

**Priority:** Must Have  
**Dependencies:** None  
**Validation Method:** Build verification

---

### REQ-142: Windows 10/11 Compatibility

**Description:**  
Application runs on Windows 10 and Windows 11 operating systems.

**Acceptance Criteria:**
- [ ] Tested on Windows 10 (Build 19041+)
- [ ] Tested on Windows 11 (all versions)
- [ ] Minimum OS requirement documented
- [ ] No Windows 7/8 compatibility required
- [ ] Modern Windows APIs used appropriately
- [ ] Graceful handling of Windows version differences
- [ ] Works with Windows Defender and security features

**Priority:** Must Have  
**Dependencies:** None  
**Validation Method:** Testing on target OS versions

---

### REQ-143: WinForms to WPF Migration Path

**Description:**  
Architecture supports gradual migration from WinForms to WPF without breaking changes.

**Acceptance Criteria:**
- [ ] ViewModels shared between WinForms and WPF implementations
- [ ] Service layer identical for both UI frameworks
- [ ] Data binding patterns compatible with both frameworks
- [ ] One module migrated as proof-of-concept
- [ ] Unit tests verify ViewModel behavior independent of UI framework
- [ ] Documentation on migration process
- [ ] No architectural refactoring required for migration

**Priority:** Should Have  
**Dependencies:** None  
**Validation Method:** Architectural review + PoC implementation

---

### REQ-144: Dependency Injection Framework

**Description:**  
Application uses dependency injection for loose coupling.

**Acceptance Criteria:**
- [ ] Microsoft.Extensions.DependencyInjection used
- [ ] All services registered in startup configuration
- [ ] ViewModels created via DI container
- [ ] Services injected via constructor
- [ ] Singletons: configuration, authentication, API clients
- [ ] Transients: ViewModels, transient services
- [ ] Scoped (if applicable): request-scoped services
- [ ] Testing enables mock injection

**Priority:** Must Have  
**Dependencies:** None  
**Validation Method:** Code review + unit test

---

### REQ-145: MSIX Package Format

**Description:**  
Application packaged and distributed via MSIX format for modern deployment.

**Acceptance Criteria:**
- [ ] MSIX package created during build process
- [ ] Package includes: application binaries, dependencies, resources
- [ ] Self-contained .NET 8 runtime included
- [ ] Digital signature on MSIX package
- [ ] Package uploaded to Windows App Store (future)
- [ ] Installation via Store or direct MSIX file
- [ ] Automatic updates supported via Store or update service
- [ ] Uninstall via Windows Add/Remove Programs

**Priority:** Should Have  
**Dependencies:** REQ-141, REQ-142  
**Validation Method:** Package creation and testing

---

### REQ-146: Application Configuration Management

**Description:**  
Externalized configuration for deployment flexibility.

**Acceptance Criteria:**
- [ ] Configuration file: `appsettings.json` in application directory
- [ ] Settings: API base URL, cache TTL, language, theme, offline mode
- [ ] Environment-specific configs: Development, Staging, Production
- [ ] Encrypted sensitive settings (ConnectionStrings, API keys)
- [ ] Settings manageable via UI (Preferences) with persistence
- [ ] Configuration reloaded on application start
- [ ] No hardcoded secrets in source code

**Priority:** Must Have  
**Dependencies:** None  
**Validation Method:** Code review + configuration test

---

### REQ-147: Logging & Diagnostics

**Description:**  
Comprehensive logging for troubleshooting and monitoring.

**Acceptance Criteria:**
- [ ] Structured logging (Serilog or similar)
- [ ] Log levels: Debug, Information, Warning, Error, Critical
- [ ] Logs written to file: `%APPDATA%\NaarNoor\logs\app.log`
- [ ] Log rotation: new file daily, old files retained 30 days
- [ ] Log output: JSON format (searchable)
- [ ] Exception stack traces logged with full context
- [ ] Performance metrics logged (API response times)
- [ ] Sensitive data (passwords, tokens) never logged
- [ ] Log verbosity configurable per environment
- [ ] Support for log aggregation (future: ELK, Splunk)

**Priority:** Should Have  
**Dependencies:** None  
**Validation Method:** Log inspection + integration test

---

### REQ-148: Error Reporting & Crash Dumps

**Description:**  
Application collects and reports crashes for support.

**Acceptance Criteria:**
- [ ] Unhandled exceptions caught globally
- [ ] Crash dump with exception details logged
- [ ] User offered option to submit crash report to support (optional)
- [ ] Crash reports include: OS info, .NET version, app version, error details
- [ ] No PII or sensitive data included in crash reports
- [ ] User prompted to provide context (what were you doing?)
- [ ] Report sent to backend or support system
- [ ] Support can access crash reports for investigation

**Priority:** Should Have  
**Dependencies:** REQ-147  
**Validation Method:** Manual testing (crash scenario)

---

### REQ-149: Self-Contained Runtime Deployment

**Description:**  
Application can be deployed as self-contained with bundled .NET runtime.

**Acceptance Criteria:**
- [ ] Build option: self-contained single-file executable
- [ ] Executable includes all dependencies and runtime
- [ ] File size: ~150-200MB (typical .NET self-contained size)
- [ ] Executable runs without .NET installation
- [ ] Single executable can be distributed to end users
- [ ] Update strategy: download new executable or use app update service
- [ ] Pros: no .NET dependency; Cons: larger file, update burden

**Priority:** Should Have  
**Dependencies:** REQ-141  
**Validation Method:** Build and deployment test

---

### REQ-150: Application Versioning & Updates

**Description:**  
Version tracking and update mechanism for distributing fixes and features.

**Acceptance Criteria:**
- [ ] Application version stored in AssemblyVersion
- [ ] Version displayed in About dialog
- [ ] Update check performed at startup
- [ ] Backend endpoint: `/api/updates/latest-version`
- [ ] If update available, notify user with option to update
- [ ] Download update in background or via prompt
- [ ] Update applied on next application restart
- [ ] Rollback mechanism (keep previous version executable)
- [ ] Update logs recorded
- [ ] Automatic update optional (configurable)

**Priority:** Should Have  
**Dependencies:** REQ-145, REQ-146  
**Validation Method:** Integration test

---

### REQ-151 through REQ-160: Final Infrastructure & Deployment Requirements

### REQ-151: API Integration Contract

**Description:**  
Standardized contracts between desktop client and backend API.

**Acceptance Criteria:**
- [ ] Refit interfaces define all API endpoints
- [ ] Request/response DTOs match backend contracts
- [ ] API versioning strategy: versioned endpoints or headers
- [ ] Error responses follow standard format: `{ error: string, details?: string }`
- [ ] Pagination standardized: `{ data: [...], page: int, pageSize: int, total: int }`
- [ ] Timestamp fields in ISO 8601 format
- [ ] No circular references in DTOs
- [ ] Optional fields nullable (C# nullable reference types)
- [ ] API documentation generated from Swagger/OpenAPI

**Priority:** Must Have  
**Dependencies:** None  
**Validation Method:** Integration test + contract testing

---

### REQ-152: HTTP Client Resilience Policies

**Description:**  
HttpClient configured with resilience policies for fault tolerance.

**Acceptance Criteria:**
- [ ] Retry policy: exponential backoff (1s, 2s, 4s), max 3 retries
- [ ] Retry applied only to transient failures (500, 503, 408, timeout)
- [ ] Circuit breaker: open after 5 failures, duration 30s
- [ ] Timeout policy: 30-second request timeout
- [ ] Bulkhead policy: limits concurrent requests to 10
- [ ] Fallback: return cached data on circuit breaker open
- [ ] Policies composed with Polly.Bulkhead + Polly.CircuitBreaker
- [ ] Policy timing logged for diagnostics

**Priority:** Must Have  
**Dependencies:** None  
**Validation Method:** Resilience testing + integration test

---

### REQ-153: Offline-First Data Synchronization

**Description:**  
When offline, queued operations sync when reconnected.

**Acceptance Criteria:**
- [ ] Pending operations stored in `pending_operations` SQLite table
- [ ] Table fields: id, timestamp, operation_type, resource_type, payload, status
- [ ] Operations queued for: POST (create), PUT (update), DELETE
- [ ] On reconnection detected, sync pending operations
- [ ] Sync in original order (FIFO)
- [ ] Conflict detection: compare resource version before applying
- [ ] Conflict resolution: skip operation and notify user
- [ ] Retry transient failures (exponential backoff)
- [ ] Mark completed operations as synced, then delete from queue
- [ ] Failed operations remain in queue for manual retry

**Priority:** Should Have  
**Dependencies:** REQ-016, REQ-040  
**Validation Method:** Integration test + offline scenario testing

---

### REQ-154: SQLite Database Schema

**Description:**  
Local SQLite database for caching and offline support.

**Acceptance Criteria:**
- [ ] Database file: `%APPDATA%\NaarNoor\app.db`
- [ ] Core tables: cache_entries, pending_operations, audit_logs
- [ ] Schema versioning: `schema_version` table for migrations
- [ ] Indexes on frequently queried columns (user_id, timestamp, resource_id)
- [ ] Foreign key constraints enabled
- [ ] Triggers for audit logging (optional)
- [ ] Encryption at rest (optional, via SQLCipher)
- [ ] Database integrity check on startup
- [ ] Backup functionality (REQ-155)
- [ ] Entity Framework Core or Dapper for data access

**Priority:** Must Have  
**Dependencies:** REQ-016  
**Validation Method:** Database inspection + integration test

---

### REQ-155: Data Backup & Recovery

**Description:**  
Automatic backups of local data for disaster recovery.

**Acceptance Criteria:**
- [ ] Daily backup of SQLite database
- [ ] Backup location: `%APPDATA%\NaarNoor\backups\`
- [ ] Backups retained: last 7 days
- [ ] Backup file encrypted with DPAPI
- [ ] Manual backup option via Tools menu
- [ ] Restore functionality: select backup date, restore
- [ ] Restore creates new backup of current data first
- [ ] Success message after restore
- [ ] Automatic cleanup of old backups (>7 days)

**Priority:** Should Have  
**Dependencies:** REQ-154  
**Validation Method:** Manual testing + backup verification

---

### REQ-156: Performance & Load Testing

**Description:**  
Application tested to meet performance requirements.

**Acceptance Criteria:**
- [ ] UI response time: <500ms for user actions
- [ ] Data load: <2 seconds for typical queries
- [ ] Dashboard load: <2 seconds after login
- [ ] Menu load: <1 second for 1000+ items
- [ ] Concurrent users: support 100+ simultaneous desktop clients
- [ ] Large data sets: tested with 10,000+ reservations
- [ ] Memory usage: <200MB steady-state, <500MB peak
- [ ] CPU usage: <20% idle, <80% peak
- [ ] Cache efficiency: >80% hit rate for dashboard data
- [ ] Load testing: simulated with automated tools

**Priority:** Should Have  
**Dependencies:** None  
**Validation Method:** Performance & load testing

---

### REQ-157: Security Scanning & Vulnerability Management

**Description:**  
Proactive security scanning for vulnerabilities.

**Acceptance Criteria:**
- [ ] SAST (Static Application Security Testing) in CI/CD
- [ ] Dependency scanning (NuGet packages) for vulnerabilities
- [ ] Build fails on critical/high-severity vulnerabilities
- [ ] SCA (Software Composition Analysis) report generated
- [ ] Regular penetration testing (quarterly recommended)
- [ ] Secure coding practices enforced in code review
- [ ] Security headers and controls verified in testing
- [ ] Dependencies updated monthly or on security advisory

**Priority:** Should Have  
**Dependencies:** None  
**Validation Method:** Automated scanning + manual testing

---

### REQ-158: Unit Test Coverage

**Description:**  
Comprehensive unit test suite for code quality.

**Acceptance Criteria:**
- [ ] Test framework: xUnit
- [ ] Mock framework: Moq
- [ ] Service layer coverage: >80%
- [ ] ViewModel coverage: >70%
- [ ] Tests for happy path and error scenarios
- [ ] Tests for edge cases (null, empty, boundary values)
- [ ] Tests runnable in CI/CD pipeline
- [ ] Code coverage report generated
- [ ] Coverage trend tracked over time
- [ ] Property-based tests for critical algorithms (fast-check)

**Priority:** Must Have  
**Dependencies:** None  
**Validation Method:** Coverage report + CI/CD integration

---

### REQ-159: Integration Testing

**Description:**  
Integration tests verify API communication and workflows.

**Acceptance Criteria:**
- [ ] Test framework: xUnit with WebApplicationFactory
- [ ] Mock API server for testing
- [ ] Test scenarios: full login flow, data CRUD, error handling
- [ ] Tests cover role-based access control
- [ ] Tests for retry/resilience policies
- [ ] Tests for offline mode and sync
- [ ] Tests run against real SQLite in-memory database
- [ ] Test data setup/teardown automated
- [ ] Tests runnable in CI/CD pipeline
- [ ] Concurrent request testing

**Priority:** Must Have  
**Dependencies:** None  
**Validation Method:** CI/CD integration

---

### REQ-160: Continuous Integration & Deployment Pipeline

**Description:**  
Automated CI/CD pipeline for build, test, and deployment.

**Acceptance Criteria:**
- [ ] CI trigger: every commit to main/develop
- [ ] Build step: dotnet build with no warnings
- [ ] Test step: run unit and integration tests
- [ ] Coverage check: fail if coverage drops below threshold
- [ ] Security scan: SAST and dependency scanning
- [ ] Build artifacts: MSIX package, changelog
- [ ] Deployment to staging: automatic on successful test
- [ ] Deployment to production: manual approval gate
- [ ] Version bump: automatic based on commit message (semver)
- [ ] GitHub Actions / Azure Pipelines / similar
- [ ] Build history and artifacts retained for 90 days

**Priority:** Should Have  
**Dependencies:** All prior requirements  
**Validation Method:** CI/CD configuration review + test run

---

## Summary & Next Steps

This comprehensive requirements document covers 160 distinct requirements organized into 8 feature areas:

- **Core Authentication & Security (REQ-001 to REQ-020)**: Foundation for secure access and data protection
- **Dashboard & Monitoring (REQ-021 to REQ-040)**: Real-time operational visibility
- **Menu Management (REQ-041 to REQ-060)**: CRUD operations with bilingual support
- **Reservation System (REQ-061 to REQ-080)**: Booking, conflict prevention, tracking
- **Staff Management (REQ-081 to REQ-100)**: Employee records, roles, performance
- **Reports & Analytics (REQ-101 to REQ-120)**: Business intelligence and insights
- **Localization & UI (REQ-121 to REQ-140)**: Bilingual support, accessibility, user experience
- **Infrastructure & Deployment (REQ-141 to REQ-160)**: Technical platform, CI/CD, testing

### Priority Distribution

- **Must Have**: 45 requirements (critical for MVP)
- **Should Have**: 85 requirements (important for full functionality)
- **Nice to Have**: 30 requirements (enhancements and future features)

### Implementation Phases

**Phase 1 (MVP - Weeks 1-8):**
- REQ-001 to REQ-020 (Authentication)
- REQ-021 to REQ-030 (Dashboard basics)
- REQ-041 to REQ-050 (Menu management)
- REQ-061 to REQ-070 (Reservations)
- REQ-141 to REQ-145 (Infrastructure)

**Phase 2 (Enhanced - Weeks 9-16):**
- REQ-031 to REQ-040 (Advanced dashboard)
- REQ-051 to REQ-060 (Menu advanced)
- REQ-071 to REQ-080 (Reservations advanced)
- REQ-081 to REQ-090 (Staff management)
- REQ-121 to REQ-135 (Localization & UI)

**Phase 3 (Analytics & Operations - Weeks 17-24):**
- REQ-091 to REQ-100 (Staff advanced)
- REQ-101 to REQ-110 (Reports & analytics)
- REQ-136 to REQ-140 (UI polish)
- REQ-146 to REQ-160 (Deployment & CI/CD)

### Traceability

Each requirement includes:
- ✅ Unique ID for tracking
- ✅ Clear description and acceptance criteria
- ✅ Priority level for scheduling
- ✅ Dependencies for sequencing
- ✅ Validation method for testing

### Quality Gates

- All requirements testable (automated or manual)
- Dependencies minimized and explicit
- No ambiguous or conflicting requirements
- Clear acceptance criteria for verification
- Aligned with technical design document

---

**Document Version**: 1.0  
**Last Updated**: 2024  
**Status**: Ready for Implementation Planning

