# Implementation Plan: Naar-Noor Desktop Application

## Overview

This implementation plan breaks down the Naar-Noor Desktop Application design into actionable coding tasks. The application is a .NET 8+ Windows desktop client (WinForms→WPF) with MVVM architecture, bilingual support (English/Arabic), and comprehensive restaurant management features. Tasks are organized by functional area and ordered to resolve dependencies: project infrastructure first, then authentication, services, UI layers, and integration testing.

**Key Implementation Phases:**
1. Project Setup & Infrastructure (foundation)
2. Authentication & Security (JWT, token management, DPAPI)
3. HTTP Client & API Integration (Refit, Polly resilience)
4. Localization & Bilingual Support (resource files, RTL/LTR)
5. Service Layer Implementation (business logic, caching)
6. UI Implementation (Forms→WPF, ViewModels, data binding)
7. Testing & Validation (unit, property-based, integration tests)

**Implementation Language:** C# (.NET 8+)

---

## Tasks

- [x] 1. Project Setup & Infrastructure
  - [x] 1.1 Create .NET 8 desktop application solution structure
    - Create NaarNoor.Desktop.sln with three projects
    - Add NaarNoor.Desktop.Common (shared models, interfaces, utilities)
    - Add NaarNoor.Desktop.WinForms (UI layer, ViewModels, services)
    - Add NaarNoor.Desktop.Tests (xUnit test project)
    - Create directory structure per design (Models, Services, DTOs, Resources, Configuration)
    - _Requirements: infrastructure foundation_

  - [x] 1.2 Configure project dependencies and NuGet packages
    - Add Microsoft.Toolkit.Mvvm 8.x for MVVM patterns
    - Add Refit 7.x for type-safe HTTP client
    - Add Polly 8.x for resilience policies
    - Add System.Data.SQLite for local caching
    - Add xUnit 2.x and Moq 4.x for testing
    - Configure package versions in .csproj files
    - _Requirements: REQ-141_

  - [x] 1.3 Create core project configuration files
    - Create appsettings.json with default values (API URL, cache TTL, culture)
    - Create appsettings.Development.json for development environment
    - Implement ConfigurationService to load and save app settings
    - Store configuration in %APPDATA%\NaarNoor\app-config.json
    - _Requirements: infrastructure foundation_

  - [x] 1.4 Set up dependency injection container
    - Create ServiceConfiguration class in Configuration folder
    - Register all service interfaces and implementations
    - Register HTTP clients with base URLs
    - Register ViewModels with transient lifetime
    - Register services with singleton lifetime
    - Return IServiceProvider for application startup
    - _Requirements: infrastructure foundation_

- [x] 2. Authentication & Security Services
  - [x] 2.1 Implement IAuthenticationService interface and base class
    - Define IAuthenticationService with methods: AuthenticateAsync, RefreshTokenAsync, LogoutAsync, GetCurrentTokenAsync
    - Add properties: IsAuthenticated, CurrentUserId, CurrentUserRole
    - Create AuthenticationService implementation using async/await pattern
    - Store authentication state in private fields
    - Implement JWT token parsing to extract claims
    - _Requirements: REQ-001, REQ-002_

  - [x] 2.2 Implement secure token storage using DPAPI
    - Create TokenStorageService using Windows.Security.Cryptography
    - Encrypt tokens via DataProtectionScope.CurrentUser before persistence
    - Store encrypted tokens in %APPDATA%\NaarNoor\tokens file
    - Decrypt tokens on retrieval with error handling
    - Clear plaintext tokens from memory using SecureString
    - Implement token file cleanup on logout
    - _Requirements: REQ-003_

  - [ ]* 2.3 Write property test: Authentication Idempotency
    - **Property 1: Authentication Idempotency**
    - **Validates: Requirements REQ-002**
    - Generate random valid credentials via fast-check
    - Verify identical requests return same token
    - Test with multiple concurrent authentication attempts
    - Verify token claims remain consistent

  - [x] 2.4 Implement IAuthApiClient Refit interface
    - Define LoginAsync method: POST /api/auth/login with LoginRequest
    - Define RefreshTokenAsync method: POST /api/auth/refresh with Bearer token
    - Define LogoutAsync method: POST /api/auth/logout with Bearer token
    - Add [Headers("Authorization: Bearer")] attributes for protected endpoints
    - _Requirements: REQ-001, REQ-002_

  - [x] 2.5 Create Refit HTTP client configuration with Polly policies
    - Configure HttpClient with base URL from appsettings
    - Set timeout to 30 seconds
    - Add retry policy: exponential backoff (1s, 2s, 4s)
    - Add circuit breaker: break after 5 failures, 30s duration
    - Register AuthenticationHeaderHandler for token injection
    - _Requirements: REQ-007, REQ-002_

  - [x] 2.6 Implement AuthenticationHeaderHandler for automatic token refresh
    - Extend DelegatingHandler to intercept all HTTP requests
    - Add Authorization header with Bearer token if authenticated
    - Detect 401 Unauthorized responses
    - Trigger automatic token refresh on 401
    - Retry original request with new token
    - Log refresh failures to audit trail
    - _Requirements: REQ-002, REQ-017_

  - [ ]* 2.7 Write unit tests for authentication service
    - Test successful login flow
    - Test token storage and retrieval
    - Test token refresh on expiration
    - Test logout clears all tokens
    - Test invalid credentials error handling
    - Test network timeout handling

  - [x] 2.8 Implement role-based access control (RBAC) enforcement
    - Create AuthorizationService to check user roles against operations
    - Implement methods: CanAccessFeature(feature), CanPerformAction(action)
    - Map roles to permissions: Manager → full access, Chef → menu/orders, Staff → reservations/orders
    - Cache role claims from JWT in memory
    - Raise UnauthorizedAccessException for permission violations
    - Log unauthorized attempts to audit trail (REQ-005)
    - _Requirements: REQ-004, REQ-005_

- [ ] 3. HTTP Client & API Integration
  - [x] 3.1 Define Refit API client interfaces for all feature areas
    - Create IAuthApiClient with auth endpoints
    - Create IReservationApiClient with reservation CRUD methods
    - Create IMenuApiClient with menu item CRUD methods
    - Create IChefApiClient for staff/chef operations
    - Create IReportApiClient for analytics endpoints
    - Add [Headers("Authorization: Bearer")] to protected endpoints
    - _Requirements: REQ-001, REQ-021_

  - [x] 3.2 Implement Result<T> pattern for error handling
    - Create Result<T> generic class with IsSuccess, Value, Error properties
    - Implement Success(value) and Failure(error) factory methods
    - Add Map<U> method for composing Results
    - Create non-generic Result class for void operations
    - Use for all service method return types
    - _Requirements: REQ-009_

  - [x] 3.3 Configure HttpClient with gzip compression and security headers
    - Enable gzip for request/response bodies
    - Add X-Content-Type-Options: nosniff header
    - Add X-Frame-Options: DENY header
    - Add X-XSS-Protection: 1; mode=block header
    - Add Strict-Transport-Security header
    - Add X-Client-Version header with application version
    - Set User-Agent to identify desktop client + .NET version
    - _Requirements: REQ-017_

  - [x] 3.4 Implement certificate pinning for production API endpoint
    - Store SHA-256 hash of expected certificate
    - Create certificate validation handler to check hash before TLS handshake
    - Support fallback certificate for key rotation
    - Allow bypassing in development environment (with warning)
    - Make certificate hash configurable via appsettings
    - _Requirements: REQ-013, REQ-007_

- [x] 4. Localization & Bilingual Support
  - [x] 4.1 Create localization infrastructure and resource files
    - Create ResourceDictionary files: Resources/en.xaml, Resources/ar.xaml
    - Define all UI strings as key-value pairs (200+ strings)
    - Implement ILocalizationService interface
    - Create LocalizationService to load and switch cultures at runtime
    - Support both English (en) and Arabic (ar) cultures
    - Implement CultureChanged observable for UI bindings
    - _Requirements: REQ-121, REQ-122_

  - [x] 4.2 Implement runtime culture switching without application restart
    - Create SetCulture(cultureName) method in LocalizationService
    - Update Thread.CurrentThread.CurrentUICulture
    - Trigger CultureChanged event to notify UI
    - Persist selected culture to app-config.json
    - Reload all UI strings on culture change
    - _Requirements: REQ-121, REQ-122_

  - [x] 4.3 Implement RTL/LTR layout support for Arabic
    - Use FlowLayoutPanel for flexible layouts (WinForms)
    - Configure RightToLeft property based on current culture
    - Adjust text alignment (Arabic: right, English: left)
    - Test with bidirectional text
    - Document RTL migration path for WPF (FlowDirection property)
    - _Requirements: REQ-121, REQ-122_

  - [ ]* 4.4 Write unit tests for localization service
    - Test loading English and Arabic resources
    - Test culture switching functionality
    - Test CultureChanged event firing
    - Test persistence of culture preference

- [ ] 5. Cache & Offline Support Infrastructure
  - [x] 5.1 Implement ICacheService with multi-layer caching strategy
    - Create CacheService with L1 (in-memory), L2 (SQLite), L3 (JSON file) layers
    - Implement GetAsync<T>(key) and SetAsync<T>(key, value, expiration)
    - Support RemoveAsync(key) and ClearAsync() operations
    - Implement InvalidatePattern(pattern) for cascading invalidation
    - Store cache metadata: key, value, expiration time, created timestamp
    - Use LRU eviction when cache exceeds size limit (100MB)
    - _Requirements: REQ-026, REQ-040_

  - [x] 5.2 Set up SQLite database schema for caching and offline operations
    - Create cache_entries table: key, value, expiration, created_at, updated_at
    - Create pending_operations table: id, user_id, operation_type, payload, created_at, synced_at
    - Create audit_logs table: timestamp, user_id, action, resource_type, status, details
    - Create indexes on frequently queried columns (user_id, timestamp, key)
    - Initialize database on first application run
    - _Requirements: REQ-040, REQ-005_

  - [~] 5.3 Implement offline mode detection and queuing
    - Create NetworkConnectivityService to detect network availability
    - Queue write operations when offline in pending_operations table
    - Show "Working Offline" indicator in UI
    - Prevent operations requiring network (show error message)
    - Store operation metadata: timestamp, user_id, operation details
    - _Requirements: REQ-016_

  - [~] 5.4 Implement sync queue processing on reconnection
    - Create SyncService to process pending operations queue
    - Sort by created_at and execute in order
    - Implement conflict resolution: last-write-wins
    - Handle partial sync failures (rollback vs commit)
    - Update pending_operations.synced_at on success
    - Remove successfully synced operations from queue
    - Notify user of sync completion
    - _Requirements: REQ-016_

  - [ ]* 5.5 Write property test: Cache Coherency
    - **Property 2: Cache Coherency**
    - **Validates: Requirements REQ-026, REQ-040**
    - Generate random cache operations (set, get, invalidate)
    - Verify set(k, v) ∧ get(k) ⟹ get(k) = v
    - Test with multiple rapid updates to same key
    - Verify reads reflect most recent write

- [ ] 6. Service Layer Implementation
  - [~] 6.1 Implement IReservationService with full CRUD operations
    - Create ReservationService with methods: GetReservationsAsync, GetReservationByIdAsync, CreateReservationAsync, UpdateReservationAsync, DeleteReservationAsync
    - Add IObservable<ReservationNotification> for updates
    - Cache reservations with 30s TTL
    - Invalidate cache after mutations
    - Call IReservationApiClient for API operations
    - Handle errors via Result<T> pattern
    - _Requirements: REQ-061, REQ-065, REQ-066_

  - [~] 6.2 Implement IMenuService with menu item management
    - Create MenuService with CRUD methods for menu items
    - Implement caching for full menu list (2-hour TTL)
    - Add filtering by category and availability
    - Support bilingual names (EN/AR)
    - Validate menu item data before submission
    - Invalidate cache on creation, update, or deletion
    - _Requirements: REQ-041, REQ-042, REQ-043, REQ-044_

  - [~] 6.3 Implement IChefService for staff management
    - Create ChefService for staff/chef operations
    - Implement GetStaffAsync(), UpdateStaffStatusAsync()
    - Support role-based filtering (Chef, Waiter, Manager)
    - Cache staff list with 15-minute TTL
    - Publish staff status changes to observable stream
    - _Requirements: REQ-081, REQ-024_

  - [~] 6.4 Implement IReportService for analytics
    - Create ReportService with methods: GetRevenueAsync(), GetOrderStatsAsync(), GetReservationTrendsAsync()
    - Cache report data with 1-hour TTL (longer for stability)
    - Support date range filtering
    - Return aggregated data structures (no raw queries)
    - _Requirements: REQ-101, REQ-029_

  - [~] 6.5 Create LocalizationService implementation
    - Load resource dictionary based on current culture
    - Implement GetString(key) and GetString(key, args) for i18n
    - Cache loaded resources in memory
    - Expose CultureChanged observable for UI updates
    - Persist culture preference to app-config.json
    - _Requirements: REQ-121, REQ-122_

  - [ ]* 6.6 Write unit tests for all service implementations
    - Test successful CRUD operations
    - Test cache hit/miss scenarios
    - Test error handling and recovery
    - Mock API clients with Moq
    - Test Result<T> return types
    - Verify cache invalidation after mutations

- [~] 7. Checkpoint 1 - Service Layer Complete
  - Ensure all service layer tests pass
  - Verify cache strategy works correctly
  - Confirm offline queue functionality
  - Test API client integration
  - Ask the user if questions arise.

- [ ] 8. MVVM ViewModel Implementation
  - [~] 8.1 Create ViewModelBase class with common functionality
    - Extend ObservableObject from CommunityToolkit.Mvvm
    - Add error handling: ErrorMessage, IsLoading properties
    - Implement ExecuteAsync<T> helper for service calls
    - Add SetError and ClearError convenience methods
    - Provide access to IServiceProvider for service resolution
    - Support INotifyPropertyChanged via [ObservableProperty] attributes
    - _Requirements: infrastructure foundation_

  - [~] 8.2 Implement LoginViewModel with authentication logic
    - Add UsernameInput, PasswordInput observable properties
    - Implement LoginCommand with async logic
    - Validate inputs before submission
    - Call IAuthenticationService.AuthenticateAsync()
    - Handle authentication errors with user-friendly messages
    - Navigate to dashboard on success
    - Clear sensitive fields on error
    - _Requirements: REQ-001, REQ-002_

  - [~] 8.3 Implement DashboardViewModel as container/aggregator
    - Add observable properties for dashboard state
    - Load reservation, order, and staff data on initialization
    - Implement auto-refresh every 30 seconds
    - Handle errors gracefully (show fallback cached data)
    - Subscribe to all service observables for updates
    - Implement role-based widget visibility (REQ-027)
    - _Requirements: REQ-021, REQ-026, REQ-027_

  - [~] 8.4 Implement ReservationViewModel for reservation management
    - Add SelectedDate, SelectedReservation observable properties
    - Implement LoadReservationsCommand for date-filtered retrieval
    - Implement CreateReservationCommand with form validation
    - Implement UpdateReservationCommand
    - Implement DeleteReservationCommand with confirmation
    - Show/hide new reservation form modal
    - Bind to paginated data grid (50 items per page)
    - _Requirements: REQ-061, REQ-065_

  - [~] 8.5 Implement MenuViewModel for menu item management
    - Add menu items ObservableCollection property
    - Implement search/filter by category
    - Implement CreateMenuItemCommand with form validation
    - Implement UpdateMenuItemCommand with conflict handling
    - Implement DeleteMenuItemCommand with confirmation
    - Support bilingual menu display
    - Show creation/modification timestamps
    - _Requirements: REQ-041, REQ-042, REQ-043, REQ-044_

  - [~] 8.6 Implement StaffViewModel for staff management
    - Add staff list ObservableCollection property
    - Implement UpdateStaffStatusCommand
    - Show role, availability status, scheduled hours
    - Enable quick status change from UI
    - Auto-refresh staff list every 2 minutes
    - _Requirements: REQ-081, REQ-024_

  - [~] 8.7 Implement ReportViewModel for analytics display
    - Add revenue summary, order stats, reservation trends properties
    - Implement LoadReportCommand with date range filtering
    - Show year-to-date metrics
    - Display charts/graphs (bar chart library optional)
    - Implement export to CSV functionality
    - _Requirements: REQ-101, REQ-029_

  - [ ]* 8.8 Write unit tests for all ViewModels
    - Test command execution and state updates
    - Test error handling and user messaging
    - Test service integration with mocked dependencies
    - Test observable collection updates
    - Mock RelayCommand with Moq

- [ ] 9. UI Layer - Forms Implementation (WinForms)
  - [~] 9.1 Implement LoginForm with credential input and validation
    - Create Form with TextBox for username
    - Create MaskedTextBox for password input
    - Implement Login button with enabled/disabled state
    - Show error messages below inputs (red text)
    - Clear sensitive fields on error
    - Bind to LoginViewModel
    - Show loading spinner during authentication
    - _Requirements: REQ-001, REQ-014_

  - [~] 9.2 Implement DashboardForm as main application window
    - Create main container with menu bar and navigation
    - Implement tab control for feature navigation (Reservations, Menu, Staff, Reports)
    - Add dashboard panel with key metrics widgets
    - Show user info and logout button in top bar
    - Implement theme switching (light/dark mode optional)
    - Add culture selector dropdown (EN/AR)
    - _Requirements: REQ-021, REQ-121_

  - [~] 9.3 Implement ReservationForm with CRUD interface
    - Create data grid to display reservations (sortable, filterable)
    - Add filter controls: date range picker, status dropdown
    - Implement search textbox for customer name
    - Add "New Reservation" button opening modal form
    - Add Edit/Delete buttons per row with confirmation
    - Show reservation details panel on row selection
    - Implement pagination (10 items per page)
    - _Requirements: REQ-061, REQ-062_

  - [~] 9.4 Implement MenuForm with bilingual display
    - Create data grid for menu items with sorting/filtering
    - Add columns: Name (EN), Name (AR), Category, Price, Availability
    - Implement category filter dropdown
    - Add search by name (case-insensitive)
    - Add "New Item" button opening form dialog
    - Add Edit/Delete buttons with confirmations
    - Show item image/description panel (optional)
    - _Requirements: REQ-041, REQ-045_

  - [~] 9.5 Implement StaffForm for staff status management
    - Create list view showing staff members
    - Display: Name, Role, Status (color-coded badge)
    - Implement dropdown to change status (available/busy/break)
    - Show scheduled hours and current shift info
    - Add search by staff name
    - Color coding: Green (available), Yellow (busy), Red (break)
    - _Requirements: REQ-081, REQ-024_

  - [~] 9.6 Implement ReportForm for analytics display
    - Create tabbed interface: Revenue, Orders, Reservations
    - Show key metrics cards (large numeric displays)
    - Add date range picker for filtering
    - Implement data grid with detailed breakdown
    - Add export to CSV button
    - Show trend sparklines (if chart library available)
    - _Requirements: REQ-101, REQ-103_

  - [ ]* 9.7 Write UI integration tests for Forms
    - Test form navigation flow
    - Test data grid sorting and filtering
    - Test modal dialog open/close
    - Test button enabled/disabled states
    - Test error message display
    - Test culture switching updates UI text

- [ ] 10. Error Handling & Resilience
  - [~] 10.1 Implement comprehensive exception handling
    - Create ExceptionHandlingMiddleware for top-level error catching
    - Implement try-catch in all service methods
    - Log detailed errors to application log (not user-visible)
    - Show generic error messages to users
    - Never expose stack traces, stack frames, or internal details
    - Handle specific exception types (network timeout, API error, validation)
    - _Requirements: REQ-009_

  - [~] 10.2 Implement retry logic with exponential backoff
    - Configure Polly retry policy: 3 attempts, 1s-4s backoff
    - Apply to all transient failures (timeouts, 5xx errors)
    - Skip retries for permanent failures (4xx client errors)
    - Log retry attempts for debugging
    - Max retry timeout: 30 seconds total
    - _Requirements: REQ-002, REQ-019_

  - [~] 10.3 Implement circuit breaker for cascading failure prevention
    - Configure Polly circuit breaker: 5 failures, 30s break duration
    - Detect persistent API unavailability
    - Prevent excessive retry loops
    - Fall back to cached data when circuit open
    - Notify user of API connectivity issues
    - Log circuit state changes to audit trail
    - _Requirements: REQ-002_

  - [ ]* 10.4 Write property test: Error Recovery
    - **Property 5: Error Recovery**
    - **Validates: Requirements REQ-002**
    - Generate failed operations with transient errors
    - Verify retry_count(op) > 0 for transient failures
    - Test with network timeout scenarios
    - Test with 503 Service Unavailable responses
    - Verify permanent failures are not retried

- [ ] 11. Testing & Validation
  - [~] 11.1 Write comprehensive unit tests for authentication flow
    - Test successful login with valid credentials
    - Test failed login with invalid credentials
    - Test token refresh on 401 response
    - Test token storage and retrieval via DPAPI
    - Test logout clears all tokens
    - Test concurrent authentication requests (idempotency)
    - Coverage target: >90% for auth service
    - _Requirements: REQ-001, REQ-002, REQ-003_

  - [~] 11.2 Write unit tests for RBAC and authorization
    - Test role checking: Manager, Chef, Staff, Admin
    - Test permission enforcement: unauthorized access blocked
    - Test claim-based authorization
    - Test unauthorized operation logging to audit trail
    - Coverage target: >85%
    - _Requirements: REQ-004, REQ-005_

  - [ ]* 11.3 Write property test: Permission Enforcement
    - **Property 4: Permission Enforcement**
    - **Validates: Requirements REQ-004**
    - Generate random users with different roles
    - Generate random resources requiring specific roles
    - Verify authorize(u, x) ⟹ (r ⊇ r')
    - Test all role combinations
    - Verify unauthorized attempts are logged

  - [~] 11.4 Write integration tests for API client and services
    - Create mock API server (in-process WebApplicationFactory)
    - Test full auth → service → API → response flow
    - Test error scenarios: timeouts, 400/401/500 responses
    - Test cache hit and miss paths
    - Test offline mode operations
    - Test pending operation sync
    - _Requirements: REQ-001, REQ-026, REQ-016_

  - [ ]* 11.5 Write property test: Reservation Conflict Prevention
    - **Property 3: Reservation Conflict Prevention**
    - **Validates: Requirements REQ-061, REQ-069**
    - Generate concurrent reservation requests on same table
    - Verify not both succeed for overlapping time slots
    - Test table availability checking logic
    - Test optimistic locking version checking
    - Generate edge cases: exact slot boundaries

  - [~] 11.6 Write audit trail verification tests
    - Test all security events logged: login, logout, auth failures
    - Test unauthorized access attempts logged
    - Test sensitive operations logged with payload
    - Verify audit log retention: 90 days minimum
    - Verify audit logs are append-only (immutable)
    - _Requirements: REQ-005_

  - [~] 11.7 Set up continuous test execution pipeline
    - Configure xUnit test runner for CI
    - Set coverage threshold: >80% overall
    - Enable parallel test execution
    - Generate coverage report (OpenCover)
    - Fail build if coverage drops below threshold
    - _Requirements: infrastructure foundation_

- [~] 12. Checkpoint 2 - Core Implementation Complete
  - Ensure all unit tests pass (>80% coverage)
  - Run integration tests with mock API
  - Verify offline mode functionality
  - Test cache invalidation strategies
  - Verify audit logging
  - Ask the user if questions arise.

- [ ] 13. Security Hardening & Validation
  - [~] 13.1 Implement input validation on all user inputs
    - Validate username: alphanumeric + underscore, max 50 chars
    - Validate password: min 8 chars, 1 uppercase, 1 lowercase, 1 digit, 1 special
    - Validate menu item names: max 200 chars, no HTML/script tags
    - Validate prices: positive decimals only, max 999.99
    - Show validation errors inline (red text)
    - Disable submit button while validation errors exist
    - _Requirements: REQ-008_

  - [~] 13.2 Prevent injection attacks in data access
    - Use parameterized queries exclusively (Refit handles this)
    - Never concatenate user input into SQL or API requests
    - Validate all API payloads before submission
    - Escape HTML in display labels
    - _Requirements: REQ-008_

  - [~] 13.3 Implement request signing for state-changing operations
    - Compute HMAC-SHA256 signature of request payload
    - Include timestamp in signature
    - Add X-Request-Signature header to PUT/POST/DELETE
    - Verify signature includes payload + timestamp
    - Prevent replay attacks: reject if timestamp >5 minutes old
    - _Requirements: REQ-015_

  - [~] 13.4 Configure TLS 1.3 enforcement and certificate validation
    - Set HttpClient to require TLS 1.3 minimum
    - Disable TLS 1.2, 1.1, 1.0
    - Enable certificate validation (check root, chain, expiration)
    - Implement certificate pinning (SHA-256 hash)
    - Allow development override with warning
    - Log certificate validation failures
    - _Requirements: REQ-007, REQ-013_

  - [~] 13.5 Implement secure logging (no sensitive data)
    - Exclude passwords from all logs
    - Mask JWT tokens: show only first 20 chars
    - Exclude PII from normal logs
    - Log only first 100 chars of long data
    - Implement log file encryption or access control
    - Implement log retention: max 30 days detail logs
    - _Requirements: REQ-018_

  - [~] 13.6 Run security-focused testing and code review
    - Manual code review for injection vulnerabilities
    - Test with common attack payloads
    - Verify no hardcoded credentials or secrets
    - Scan for insecure patterns (plaintext passwords, etc.)
    - Test with invalid/malicious certificates
    - Verify DPAPI token encryption works
    - _Requirements: REQ-020_

- [ ] 14. Documentation & Deployment Setup
  - [~] 14.1 Create application README and developer guide
    - Document system requirements: .NET 8+, Windows 10/11
    - Document installation instructions
    - Document how to configure API endpoint (appsettings.json)
    - Document offline mode usage
    - Document troubleshooting common issues
    - _Requirements: infrastructure foundation_

  - [~] 14.2 Set up MSIX packaging for modern distribution
    - Create MSIX manifest (Package.appxmanifest)
    - Configure application identity and version
    - Set certificate for code signing
    - Create appx package for distribution
    - Document installation via Microsoft Store or direct deployment
    - _Requirements: REQ-141_

  - [~] 14.3 Configure CI/CD pipeline for builds and tests
    - Set up GitHub Actions workflow
    - Trigger on PR and push to main
    - Run linting and code analysis
    - Run xUnit tests with coverage
    - Build Release configuration
    - Generate test report artifacts
    - _Requirements: infrastructure foundation_

- [ ] 15. Final Integration & Checkpoint
  - [~] 15.1 Perform end-to-end feature validation
    - Test complete login flow
    - Test dashboard data loading and refresh
    - Test all CRUD operations (reservations, menu, staff)
    - Test offline mode: disconnect, make changes, reconnect
    - Test error scenarios: network timeouts, 401 responses
    - Test bilingual UI switching (EN ↔ AR)
    - _Requirements: all feature requirements_

  - [~] 15.2 Run final security audit
    - Verify TLS 1.3 enforcement
    - Verify DPAPI token storage
    - Verify audit logging of all events
    - Verify no sensitive data in logs
    - Verify role-based access control enforced
    - Verify input validation prevents injection
    - _Requirements: REQ-001 through REQ-020_

  - [~] 15.3 Verify correctness properties with property-based tests
    - Run Property 1: Authentication Idempotency (REQ-002)
    - Run Property 2: Cache Coherency (REQ-026, REQ-040)
    - Run Property 3: Reservation Conflict Prevention (REQ-061)
    - Run Property 4: Permission Enforcement (REQ-004)
    - Run Property 5: Error Recovery (REQ-002)
    - All properties must pass 1000+ test cases each
    - _Requirements: design correctness properties_

  - [~] 15.4 Final checkpoint - Ensure all tests pass
    - Run full test suite: xUnit + property tests
    - Coverage must be >80%
    - All integration tests pass
    - Manual QA sign-off
    - Ask the user if questions arise.

---

## Notes

- **Property-Based Tests:** Tasks marked with * include property-based testing using fast-check. These validate universal correctness properties defined in the design document.
- **Test Coverage:** Core service and ViewModel layers require >80% code coverage via xUnit tests.
- **Dependency Order:** Tasks are sequenced to resolve dependencies (setup → auth → services → UI).
- **Offline-First:** All features designed with offline capability via SQLite caching and sync queues.
- **Security-First:** DPAPI token storage, TLS 1.3, input validation, audit logging throughout.
- **Bilingual Support:** All UI strings localized; RTL layout support for Arabic via FlowLayoutPanel (WinForms) with WPF migration path.
- **Error Handling:** All failures caught via Result<T> pattern with retry/circuit-breaker via Polly.
- **Performance:** Dashboard widgets load <2s; cache TTL 30s for most data; virtual scrolling for large lists.
- **Accessibility:** Form labels properly associated; keyboard navigation supported; high contrast mode compatible.

---

## Task Dependency Graph

```json
{
  "waves": [
    {
      "id": 0,
      "tasks": ["1.1", "1.2", "1.3"],
      "description": "Project infrastructure and configuration setup"
    },
    {
      "id": 1,
      "tasks": ["1.4", "2.1", "3.1"],
      "description": "Core DI setup, auth interface, and API client definitions"
    },
    {
      "id": 2,
      "tasks": ["2.2", "2.4", "3.2", "3.3", "5.1", "5.2"],
      "description": "Token storage, HTTP client configuration, caching infrastructure"
    },
    {
      "id": 3,
      "tasks": ["2.5", "2.6", "2.8", "3.4"],
      "description": "HTTP resilience policies, auth header handler, RBAC, certificate pinning"
    },
    {
      "id": 4,
      "tasks": ["2.3", "2.7", "4.1", "4.2", "4.3"],
      "description": "Auth property test, unit tests, localization setup"
    },
    {
      "id": 5,
      "tasks": ["5.3", "5.4", "6.1", "6.2", "6.3", "6.4", "6.5"],
      "description": "Offline mode, sync queue, all service implementations"
    },
    {
      "id": 6,
      "tasks": ["5.5", "6.6", "8.1", "8.2"],
      "description": "Cache coherency property test, service unit tests, ViewModelBase"
    },
    {
      "id": 7,
      "tasks": ["8.3", "8.4", "8.5", "8.6", "8.7"],
      "description": "Core ViewModel implementations"
    },
    {
      "id": 8,
      "tasks": ["8.8", "9.1", "9.2"],
      "description": "ViewModel tests, LoginForm, DashboardForm"
    },
    {
      "id": 9,
      "tasks": ["9.3", "9.4", "9.5", "9.6"],
      "description": "Feature forms: Reservations, Menu, Staff, Reports"
    },
    {
      "id": 10,
      "tasks": ["9.7", "10.1", "10.2", "10.3"],
      "description": "UI integration tests, error handling, resilience patterns"
    },
    {
      "id": 11,
      "tasks": ["10.4", "11.1", "11.2"],
      "description": "Error recovery property test, auth/RBAC unit tests"
    },
    {
      "id": 12,
      "tasks": ["11.3", "11.4", "11.5", "11.6"],
      "description": "Permission enforcement, integration tests, conflict prevention, audit tests"
    },
    {
      "id": 13,
      "tasks": ["11.7", "13.1", "13.2", "13.3"],
      "description": "Test pipeline setup, input validation, injection prevention, request signing"
    },
    {
      "id": 14,
      "tasks": ["13.4", "13.5", "13.6"],
      "description": "TLS enforcement, secure logging, security review"
    },
    {
      "id": 15,
      "tasks": ["14.1", "14.2", "14.3"],
      "description": "Documentation, MSIX packaging, CI/CD setup"
    },
    {
      "id": 16,
      "tasks": ["15.1", "15.2", "15.3", "15.4"],
      "description": "End-to-end validation, security audit, property test verification, final checkpoint"
    }
  ]
}
```

---

## Implementation Notes by Phase

### Phase 1-2: Project Setup (Waves 0-1)
- Create solution structure with three projects
- Configure NuGet dependencies (Refit, Polly, MVVM Toolkit, xUnit, Moq)
- Set up DI container with service registrations
- Create configuration service for appsettings management

### Phase 3-4: Authentication Core (Waves 2-3)
- Implement secure token storage via DPAPI
- Configure HTTP clients with Refit interfaces
- Add resilience policies (retry, circuit breaker)
- Implement authentication header injection and auto-refresh
- Implement RBAC and audit logging

### Phase 5: Service Layer (Wave 5-6)
- Implement all business services (Reservation, Menu, Chef, Report)
- Set up multi-layer caching (in-memory, SQLite, JSON)
- Implement offline mode and sync queue
- All services use Result<T> for error handling

### Phase 6-7: MVVM & UI (Waves 7-10)
- Create ViewModelBase with common patterns
- Implement ViewModels for each feature
- Create WinForms UI with data binding
- Implement navigation and modal dialogs

### Phase 8: Resilience & Error Handling (Waves 10-14)
- Implement comprehensive exception handling
- Add retry logic with exponential backoff
- Configure circuit breaker for cascading failures
- Implement request signing for mutation security
- Add input validation and injection prevention

### Phase 9: Testing & Security (Waves 11-15)
- Write comprehensive unit tests (>80% coverage)
- Implement property-based tests for correctness properties
- Run integration tests with mock API
- Perform security hardening and audit

### Phase 10: Documentation & Release (Wave 15-16)
- Create developer documentation
- Set up MSIX packaging
- Configure CI/CD pipeline
- Final validation and sign-off

