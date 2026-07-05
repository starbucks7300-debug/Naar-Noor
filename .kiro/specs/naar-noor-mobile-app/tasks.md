# Implementation Plan: Naar-Noor Mobile Application

## Overview

This implementation plan breaks down the Naar-Noor Mobile Application design into actionable coding tasks. The application is a React Native cross-platform solution built with Expo, TypeScript, TanStack Query, and Zustand. The app serves customers and restaurant staff for the Naar-Noor restaurant system with comprehensive features including authentication, menu browsing, reservations, orders, reviews, and staff operations. Tasks are organized across 16 phases by functional area and ordered to resolve dependencies: project infrastructure first, then authentication, menu/cart features, orders/reservations, user profiles, staff operations, internationalization, offline support, testing, performance optimization, accessibility, and finally deployment.

**Key Implementation Phases:**
1. Project Initialization (foundation)
2. Foundation Setup (routing, state management, API, testing)
3. Authentication & Security (login, registration, token management)
4. Menu & Browsing (categories, items, search, caching)
5. Cart Management (Zustand store, persistence, calculations)
6. Checkout & Orders (placement, tracking, cancellation)
7. Reservations (booking, management, availability)
8. Reviews & Ratings (submission, display, filtering)
9. User Profile (management, preferences, addresses, payments)
10. Staff App (login, reservations, orders, RBAC)
11. Internationalization (i18n, RTL layout, translations)
12. Offline Support (sync queue, conflict resolution, caching)
13. Testing & Quality (unit, integration, E2E, coverage)
14. Performance & Optimization (bundle, memory, battery, images)
15. Accessibility (screen readers, contrast, keyboard navigation)
16. Deployment (EAS builds, app stores, monitoring)

**Implementation Language:** TypeScript + React Native (Expo)

---

## Overview by Phase

### Phase 1: Project Initialization (Tasks INIT-01 to INIT-06)
Foundation setup: Expo project, TypeScript configuration, dependencies, build tools, and project structure.

### Phase 2: Foundation Setup (Tasks FOUND-01 to FOUND-12)
Core infrastructure: Expo Router navigation, Zustand store, TanStack Query, Axios with interceptors, TypeScript types, testing frameworks, logging, error tracking, and analytics.

### Phase 3: Authentication (Tasks AUTH-01 to AUTH-12)
User authentication: Registration, login, password reset, token refresh, secure storage, logout, password validation, email validation, account lockout, session management, error handling, and tests.

### Phase 4: Menu & Browsing (Tasks MENU-01 to MENU-11)
Menu display: Categories, items, search, filtering, sorting, details, ratings calculation, caching strategy, and tests.

### Phase 5: Cart Management (Tasks CART-01 to CART-08)
Shopping cart: Zustand store, UI components, add to cart, quantity management, persistence, totals calculation, special instructions, and tests.

### Phase 6: Checkout & Orders (Tasks ORDER-01 to ORDER-11)
Orders: Checkout screen, delivery/pickup, address input, payment methods, order placement, tracking, history, cancellation, reorder, and tests.

### Phase 7: Reservations (Tasks RES-01 to RES-10)
Table reservations: Booking screen, date/time picker, availability, table selection, creation, history, modification, cancellation, caching, and tests.

### Phase 8: Reviews & Ratings (Tasks REVIEW-01 to REVIEW-08)
Customer reviews: Submission form, photo uploads, display components, ratings breakdown, experience ratings, filtering, and tests.

### Phase 9: User Profile (Tasks PROFILE-01 to PROFILE-09)
Profile management: View/edit, address management, payment methods, preferences, notifications, language selection, account deletion, and tests.

### Phase 10: Staff App (Tasks STAFF-01 to STAFF-12)
Staff operations: Login, reservations dashboard, check-in, table management, order management, status updates, special instructions, RBAC, notifications, and tests.

### Phase 11: Internationalization (Tasks I18N-01 to I18N-07)
Multi-language support: i18next setup, English/Arabic translations, language switching, RTL layout, rendering tests, and tests.

### Phase 12: Offline Support (Tasks OFFLINE-01 to OFFLINE-09)
Offline functionality: LocalStorage, sync queue, sync service, network monitoring, conflict resolution, offline indicators, cache management, tests, and data persistence strategy.

### Phase 13: Testing & Quality (Tasks TEST-01 to TEST-12)
Comprehensive testing: Unit tests, integration tests, component tests, E2E tests, coverage analysis, performance testing, accessibility testing, security testing, documentation, CI/CD, smoke tests, and regression tests.

### Phase 14: Performance & Optimization (Tasks PERF-01 to PERF-09)
Performance improvements: Image caching, bundle size optimization, code splitting, lazy loading, performance testing, memory optimization, battery efficiency, documentation, and performance monitoring.

### Phase 15: Accessibility (Tasks A11Y-01 to A11Y-08)
Accessibility support: Screen reader support, color contrast verification, keyboard navigation, touch target sizing, accessibility labels, testing, documentation, and issue fixes.

### Phase 16: Deployment (Tasks DEPLOY-01 to DEPLOY-12)
Production release: EAS Build configuration, iOS/Android code signing, environment variables, production APK/IPA builds, app store deployment, analytics/crash reporting, deployment documentation, release notes, and post-launch monitoring.

---

## Task Dependency Graph

Phase 1 → Phase 2 → Phases 3-4 → Phases 5-6 → Phases 7-8 → Phase 9 → Phase 10 → Phase 11 → Phase 12 → Phase 13 → Phase 14 → Phase 15 → Phase 16

Key cross-phase dependencies:
- FOUND-06 (testing setup) → All phase tests (TEST, AUTH, MENU, etc.)
- AUTH-02 (login) → FOUND-02 (store management), FOUND-04 (API)
- CART-01 (store) → ORDER-01 (checkout)
- FOUND-03 (TanStack Query) → All data fetching tasks

---

## Tasks

- [ ] Phase 1: Project Initialization

### Phase 1: Project Initialization (Tasks INIT-01 to INIT-06)

### INIT-01: Set up Expo project with TypeScript
**Task Name:** Initialize Expo Project with TypeScript Configuration  
**User Story:** As a developer, I want to initialize a new Expo project with TypeScript support so we have a solid foundation for React Native development.

**Description:**  
Create a new Expo project using `expo-cli` with TypeScript template. Configure TypeScript compiler options including strict mode, module resolution, and target ES2020. Set up necessary TypeScript configuration files (tsconfig.json) with appropriate paths and strict settings for type safety.

**Acceptance Criteria:**
- [ ] Expo project created with `expo init` or `npx create-expo-app`
- [ ] TypeScript template applied with tsconfig.json configured
- [ ] TypeScript strict mode enabled (strict: true)
- [ ] Project runs without TypeScript errors on first launch
- [ ] .gitignore configured for Expo and Node modules

**Dependencies:** None (first task)  
**Story Points:** 3  
**Priority:** Critical  
**Components/Files:**  
- `tsconfig.json`
- `app.json`
- `babel.config.js`
- `.gitignore`

**Related Requirements:** REQ-5.1.1 (App Launch Time), REQ-5.2 (Security)

---

### INIT-02: Install core dependencies
**Task Name:** Install Core React Native and Build Dependencies  
**User Story:** As a developer, I want to install all core dependencies required for the project so we have necessary libraries available.

**Description:**  
Install essential dependencies: React Native, Expo, React, TanStack Query, Zustand, Expo Router, NativeWind, Axios, and development tools. Use consistent dependency versions and pinned versions in package.json. Set up npm/yarn with proper lock file.

**Acceptance Criteria:**
- [ ] All core dependencies installed successfully
- [ ] package.json contains all required packages with pinned versions
- [ ] package-lock.json or yarn.lock committed to repo
- [ ] No peer dependency warnings during installation
- [ ] TypeScript type definitions available for all packages

**Dependencies:** INIT-01  
**Story Points:** 3  
**Priority:** Critical  
**Components/Files:**  
- `package.json`
- `package-lock.json` or `yarn.lock`
- `.npmrc` (if needed for registry config)

**Related Requirements:** REQ-5.1.1 (App Launch Time)

---

### INIT-03: Configure TypeScript and Babel
**Task Name:** Configure TypeScript Compiler and Babel Transpilation  
**User Story:** As a developer, I want TypeScript and Babel properly configured so code compiles and transpiles correctly for both iOS and Android.

**Description:**  
Configure Babel with React Native preset and TypeScript preset. Set up tsconfig.json with appropriate paths for absolute imports. Configure module resolution for NativeWind and other utility libraries. Ensure Babel handles both TypeScript files and JSX properly.

**Acceptance Criteria:**
- [ ] Babel config includes @babel/preset-typescript
- [ ] Babel config includes @babel/preset-react
- [ ] TypeScript paths configured for absolute imports (e.g., @components, @utils)
- [ ] NativeWind preset configured in Babel
- [ ] Test compilation succeeds without errors
- [ ] Source map generation configured for debugging

**Dependencies:** INIT-01, INIT-02  
**Story Points:** 2  
**Priority:** Critical  
**Components/Files:**  
- `babel.config.js`
- `tsconfig.json` (updated)
- `.babelrc` or babel section in package.json

**Related Requirements:** REQ-5.1.1 (App Launch Time)

---

### INIT-04: Set up dev environment configuration
**Task Name:** Configure Development Environment Variables and Settings  
**User Story:** As a developer, I want environment variables and dev settings configured so I can switch between dev/staging/production environments.

**Description:**  
Create environment configuration files (.env.example, .env.development, .env.staging, .env.production). Set up environment variable loading using react-native-config or similar. Configure API base URLs, feature flags, logging levels, and other environment-specific settings.

**Acceptance Criteria:**
- [ ] .env.example created with all required variables documented
- [ ] Environment configuration loading working in app
- [ ] API_BASE_URL environment variable correctly set
- [ ] Feature flags configurable via environment
- [ ] Secrets not committed to repository
- [ ] Different configs for dev/staging/production

**Dependencies:** INIT-02  
**Story Points:** 2  
**Priority:** High  
**Components/Files:**  
- `.env.example`
- `.env.development`
- `src/config/environment.ts`
- `src/config/index.ts`

**Related Requirements:** REQ-5.2.1 (Authentication Security)

---

### INIT-05: Configure build tools and scripts
**Task Name:** Set Up Build Scripts and Development Commands  
**User Story:** As a developer, I want build scripts configured so I can easily build for iOS, Android, and web targets.

**Description:**  
Set up npm scripts in package.json for common development tasks: `npm run start` (Expo development server), `npm run ios` (build for iOS), `npm run android` (build for Android), `npm run web` (web preview), `npm run build:eas-ios`, `npm run build:eas-android`. Configure EAS configuration if needed for production builds.

**Acceptance Criteria:**
- [ ] `npm run start` launches Expo dev server
- [ ] `npm run ios` builds and runs on iOS simulator
- [ ] `npm run android` builds and runs on Android emulator
- [ ] `npm run web` runs web preview (if applicable)
- [ ] All scripts documented in README
- [ ] Build scripts handle different configurations

**Dependencies:** INIT-01, INIT-02, INIT-03  
**Story Points:** 2  
**Priority:** High  
**Components/Files:**  
- `package.json` (scripts section)
- `eas.json` (for production builds)
- `README.md` (build instructions)

**Related Requirements:** REQ-5.1.1 (App Launch Time)

---

### INIT-06: Initial project structure
**Task Name:** Create Project Directory and File Structure  
**User Story:** As a developer, I want a well-organized project structure so code is easy to navigate and maintain.

**Description:**  
Create comprehensive directory structure following best practices: `/src/components`, `/src/screens`, `/src/services`, `/src/stores`, `/src/hooks`, `/src/utils`, `/src/types`, `/src/navigation`, `/src/assets`, `/src/config`, `/tests`, `/docs`. Create example files in key directories to establish patterns.

**Acceptance Criteria:**
- [ ] All core directories created
- [ ] Directory structure matches organization standards
- [ ] README files in key directories explaining purpose
- [ ] Example/template files demonstrate patterns
- [ ] Git will track directory structure (.gitkeep files)
- [ ] Path aliases configured for all directories

**Dependencies:** INIT-01, INIT-04  
**Story Points:** 2  
**Priority:** High  
**Components/Files:**  
- `/src` directory structure
- `/src/components` with sample component
- `/src/services` with service template
- `/src/stores` with store template
- `/src/hooks`, `/src/utils`, `/src/types`
- `/tests` directory
- `.gitkeep` files for directories

**Related Requirements:** REQ-5.1.1 (App Launch Time)

---

## Phase 2: Foundation Setup (Tasks FOUND-01 to FOUND-12)

### FOUND-01: Set up Expo Router navigation
**Task Name:** Configure Expo Router File-Based Navigation  
**User Story:** As a developer, I want Expo Router configured so I can organize screens using file-based routing similar to Next.js.

**Description:**  
Install and configure Expo Router (expo-router package). Set up routing structure with app directory containing index, auth stack, and main app stack. Configure route parameters, navigation options, and URL deep linking. Set up navigation header configuration and tab navigation structure.

**Acceptance Criteria:**
- [ ] Expo Router installed and configured
- [ ] App directory structure set up for routing
- [ ] Login/Auth stack configured
- [ ] Main app stack configured
- [ ] Route parameters working correctly
- [ ] Deep linking configured
- [ ] Navigation headers displaying correctly

**Dependencies:** INIT-01, INIT-02, INIT-03, INIT-06  
**Story Points:** 5  
**Priority:** Critical  
**Components/Files:**  
- `app/_layout.tsx` (root layout)
- `app/auth/_layout.tsx` (auth stack)
- `app/(app)/_layout.tsx` (main app stack)
- `app/index.tsx` (root screen)
- `src/navigation/linking.ts` (deep linking config)

**Related Requirements:** REQ-3.1 (Authentication), REQ-3.2 (Menu Browsing)

---

### FOUND-02: Configure Zustand store
**Task Name:** Set Up Zustand Global State Management  
**User Story:** As a developer, I want Zustand configured for global state management so I can manage user state, UI state, and app preferences.

**Description:**  
Install and configure Zustand for state management. Create store structure for user state (authentication, profile), UI state (navigation, modals), cart state, and preferences. Implement middleware for persistence (zustand/middleware/immer). Set up store hooks with proper TypeScript typing.

**Acceptance Criteria:**
- [ ] Zustand installed and configured
- [ ] User store created with auth and profile state
- [ ] UI store created for app state
- [ ] Cart store created (prepared for later implementation)
- [ ] Preferences store for language and settings
- [ ] Persist middleware implemented
- [ ] TypeScript types defined for all stores
- [ ] Store actions testable

**Dependencies:** INIT-01, INIT-02  
**Story Points:** 3  
**Priority:** Critical  
**Components/Files:**  
- `src/stores/useUserStore.ts`
- `src/stores/useUIStore.ts`
- `src/stores/useCartStore.ts`
- `src/stores/usePreferencesStore.ts`
- `src/stores/index.ts` (re-exports)
- `src/types/store.types.ts`

**Related Requirements:** REQ-3.1 (Authentication), REQ-3.7 (User Profile)

---

### FOUND-03: Set up TanStack Query
**Task Name:** Configure TanStack Query for Server State Management  
**User Story:** As a developer, I want TanStack Query configured so I can manage server state, caching, and API data fetching efficiently.

**Description:**  
Install TanStack Query (react-query). Create QueryClientProvider in root app layout. Configure default cache times, retry policies, and error handling. Create custom query hooks for API calls (useMenuCategories, useMenuItems, etc.). Set up mutations for POST/PUT/DELETE operations.

**Acceptance Criteria:**
- [ ] TanStack Query installed and configured
- [ ] QueryClientProvider wraps app
- [ ] Default cache times configured (5 minutes for menu, 10 minutes for reservations)
- [ ] Retry policy configured (3 retries with exponential backoff)
- [ ] Custom hooks for API calls created
- [ ] Error handling configured
- [ ] Dev tools console available for debugging
- [ ] TypeScript support verified

**Dependencies:** INIT-02  
**Story Points:** 4  
**Priority:** Critical  
**Components/Files:**  
- `src/services/queryClient.ts`
- `app/_layout.tsx` (updated with QueryClientProvider)
- `src/hooks/useMenuCategories.ts` (example hook)
- `src/config/tanstackQuery.config.ts`

**Related Requirements:** REQ-5.1.1 (API Response Times), REQ-3.2 (Menu Browsing)

---

### FOUND-04: Configure Axios with interceptors
**Task Name:** Set Up Axios HTTP Client with JWT Interceptors  
**User Story:** As a developer, I want Axios configured with interceptors so I can handle authentication, error handling, and request/response transformation globally.

**Description:**  
Create Axios instance with base URL configuration. Implement request interceptor to attach JWT tokens from secure storage. Implement response interceptor to handle 401 errors and trigger token refresh. Set up error handling middleware to normalize error responses. Configure HTTPS certificate pinning for production.

**Acceptance Criteria:**
- [ ] Axios instance created with base URL
- [ ] Request interceptor adds Authorization header with JWT
- [ ] Response interceptor handles 401 errors
- [ ] Token refresh triggered on 401 (before retrying request)
- [ ] Error responses normalized for consistent handling
- [ ] Timeout configured (5 seconds)
- [ ] HTTPS certificate pinning configured for production
- [ ] Interceptors properly typed with TypeScript

**Dependencies:** INIT-02, FOUND-02  
**Story Points:** 4  
**Priority:** Critical  
**Components/Files:**  
- `src/services/api/axiosInstance.ts`
- `src/services/api/interceptors/requestInterceptor.ts`
- `src/services/api/interceptors/responseInterceptor.ts`
- `src/services/api/errorHandler.ts`
- `src/config/axios.config.ts`

**Related Requirements:** REQ-5.2.1 (Authentication Security), REQ-5.2.2 (Data Encryption)

---

### FOUND-05: Create TypeScript types
**Task Name:** Define TypeScript Interfaces and Types for Domain Models  
**User Story:** As a developer, I want comprehensive TypeScript types defined so I have type safety throughout the application.

**Description:**  
Create TypeScript interfaces for all domain models: User, MenuItem, MenuCategory, Order, Reservation, Review, Address, PaymentMethod, etc. Define request/response DTOs for API endpoints. Create enums for status values (OrderStatus, ReservationStatus, etc.). Set up type definitions for API responses and error handling.

**Acceptance Criteria:**
- [ ] User interface defined with fields
- [ ] MenuItem interface with all properties
- [ ] MenuCategory interface created
- [ ] Order interface with status enum
- [ ] Reservation interface defined
- [ ] Review interface with rating
- [ ] Address and PaymentMethod interfaces
- [ ] API response envelope types defined
- [ ] Status enums created (avoid string literals)
- [ ] All types properly exported

**Dependencies:** INIT-02  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/types/models.ts`
- `src/types/api.ts`
- `src/types/index.ts`
- `src/types/enums.ts`

**Related Requirements:** REQ-3.1 (Authentication), REQ-3.2 (Menu Browsing), REQ-3.3 (Cart)

---

### FOUND-06: Set up unit test framework
**Task Name:** Configure Jest and React Native Testing Library  
**User Story:** As a developer, I want unit testing framework set up so I can write tests for functions and hooks.

**Description:**  
Install Jest with React Native preset. Install React Native Testing Library for component testing. Configure Jest configuration with TypeScript support. Create test utilities and setup files. Write example unit tests for utilities and hooks. Configure coverage thresholds.

**Acceptance Criteria:**
- [ ] Jest installed and configured for React Native
- [ ] Testing Library installed
- [ ] jest.config.js configured with TypeScript
- [ ] Test setup file configured
- [ ] Example unit test written and passing
- [ ] Coverage thresholds configured (80% minimum)
- [ ] `npm test` command working
- [ ] Tests run in watch mode available

**Dependencies:** INIT-02, INIT-03  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `jest.config.js`
- `src/tests/setup.ts`
- `src/utils/__tests__/example.test.ts`
- `src/hooks/__tests__/useExample.test.ts`

**Related Requirements:** REQ-13 (Testing & Quality)

---

### FOUND-07: Set up integration test framework
**Task Name:** Configure Integration Testing with React Native Testing Library  
**User Story:** As a developer, I want integration tests configured so I can test screen flows and user interactions.

**Description:**  
Set up integration testing infrastructure using React Native Testing Library and Jest. Create test utilities for rendering screens with necessary providers (QueryClient, Zustand stores). Create factories for test data. Write example integration test for auth flow. Configure test database or mock data.

**Acceptance Criteria:**
- [ ] Testing utilities created for screen rendering
- [ ] Store mocking utilities created
- [ ] Query client mocking configured
- [ ] Test data factories created
- [ ] Example integration test written
- [ ] Tests isolated from actual API
- [ ] `npm run test:integration` command available
- [ ] Coverage reporting configured

**Dependencies:** FOUND-06  
**Story Points:** 4  
**Priority:** High  
**Components/Files:**  
- `src/tests/utils/testUtils.tsx`
- `src/tests/factories/userFactory.ts`
- `src/tests/factories/menuFactory.ts`
- `src/tests/integration/auth.integration.test.tsx`

**Related Requirements:** REQ-13 (Testing & Quality)

---

### FOUND-08: Create mock API server
**Task Name:** Set Up Mock API Server for Development and Testing  
**User Story:** As a developer, I want a mock API server configured so I can develop without depending on the real backend.

**Description:**  
Set up Mock Service Worker (MSW) for API mocking. Create handlers for all API endpoints. Configure mock API server to start during tests and dev mode (optional). Create realistic mock data generators. Document how to use mock API.

**Acceptance Criteria:**
- [ ] Mock Service Worker installed and configured
- [ ] API handlers created for key endpoints (auth, menu, orders, etc.)
- [ ] Realistic mock data generators created
- [ ] Mock server can be enabled/disabled via environment
- [ ] Tests use mock API automatically
- [ ] Documentation for using mock API
- [ ] Response times can be simulated

**Dependencies:** FOUND-04, FOUND-05  
**Story Points:** 4  
**Priority:** High  
**Components/Files:**  
- `src/tests/mocks/server.ts`
- `src/tests/mocks/handlers/auth.ts`
- `src/tests/mocks/handlers/menu.ts`
- `src/tests/mocks/generators/mockData.ts`

**Related Requirements:** REQ-13 (Testing & Quality)

---

### FOUND-09: Configure E2E testing
**Task Name:** Set Up End-to-End Testing with Detox  
**User Story:** As a developer, I want E2E testing configured so I can test complete user workflows on real devices.

**Description:**  
Install Detox E2E testing framework. Create E2E test configuration for iOS and Android. Write example E2E tests for critical flows (login, browse menu, place order). Configure E2E environment setup and teardown.

**Acceptance Criteria:**
- [ ] Detox installed and configured
- [ ] Detox configuration for iOS set up
- [ ] Detox configuration for Android set up
- [ ] Example E2E test written (login flow)
- [ ] `npm run test:e2e:ios` command available
- [ ] `npm run test:e2e:android` command available
- [ ] Build configuration for E2E testing
- [ ] CI/CD integration configured

**Dependencies:** FOUND-06, INIT-05  
**Story Points:** 5  
**Priority:** Medium  
**Components/Files:**  
- `e2e/config.e2e.js`
- `e2e/testSetup.e2e.js`
- `e2e/tests/login.e2e.test.ts`
- `e2e/tests/browseMenu.e2e.test.ts`

**Related Requirements:** REQ-13 (Testing & Quality)

---

### FOUND-10: Set up logging service
**Task Name:** Implement Centralized Logging Service  
**User Story:** As a developer, I want centralized logging configured so I can debug issues and monitor app behavior.

**Description:**  
Create logging service with support for different log levels (debug, info, warn, error). Implement file logging for production. Create log formatters for structured logging. Integrate error reporting for production errors. Configure log output to file system and console based on environment.

**Acceptance Criteria:**
- [ ] Logger service created with log levels
- [ ] Console logging working for development
- [ ] File logging configured (if applicable)
- [ ] Log rotation configured for file logs
- [ ] Structured logging format implemented
- [ ] Sensitive data not logged
- [ ] Log levels configurable by environment
- [ ] Logger can be imported from src/services/logger

**Dependencies:** INIT-04  
**Story Points:** 3  
**Priority:** Medium  
**Components/Files:**  
- `src/services/logger.ts`
- `src/services/logging/consoleLogger.ts`
- `src/services/logging/fileLogger.ts`
- `src/config/logging.config.ts`

**Related Requirements:** REQ-5.1.2 (API Response Times)

---

### FOUND-11: Configure error tracking
**Task Name:** Set Up Error Tracking and Crash Reporting  
**User Story:** As a developer, I want error tracking configured so I can be alerted to production issues and bugs.

**Description:**  
Integrate Sentry or similar error tracking service for production. Configure error reporting to capture unhandled exceptions. Set up breadcrumb tracking for error context. Configure sourcemap uploads for production builds. Create error reporting guidelines documentation.

**Acceptance Criteria:**
- [ ] Error tracking service (Sentry) integrated
- [ ] Unhandled exceptions captured
- [ ] Error context (user, device, network) included
- [ ] Breadcrumbs configured
- [ ] Sourcemaps uploaded for debugging
- [ ] Error reporting disabled in development (optional)
- [ ] Error tracking working in production build
- [ ] Documentation for error handling

**Dependencies:** INIT-04, FOUND-10  
**Story Points:** 3  
**Priority:** Medium  
**Components/Files:**  
- `src/services/errorTracking.ts`
- `app/_layout.tsx` (updated with error tracking)
- `src/config/errorTracking.config.ts`

**Related Requirements:** REQ-16 (Deployment)

---

### FOUND-12: Set up analytics
**Task Name:** Configure Analytics and Event Tracking  
**User Story:** As a product manager, I want analytics configured so I can track user behavior and app performance.

**Description:**  
Integrate analytics service (Firebase Analytics, Mixpanel, or similar). Create analytics events for key user actions (login, view menu, place order, submit review). Implement user identification and session tracking. Configure event parameters for useful data. Create documentation for adding analytics to new features.

**Acceptance Criteria:**
- [ ] Analytics service integrated and configured
- [ ] User identification event sent on login
- [ ] Session tracking configured
- [ ] Example analytics events implemented
- [ ] Event parameters documented
- [ ] Analytics disabled in development (optional)
- [ ] Analytics dashboard accessible
- [ ] Documentation for adding analytics

**Dependencies:** INIT-04  
**Story Points:** 3  
**Priority:** Medium  
**Components/Files:**  
- `src/services/analytics.ts`
- `src/services/analytics/events.ts`
- `src/config/analytics.config.ts`

**Related Requirements:** REQ-16 (Deployment)

---

## Phase 3: Authentication (Tasks AUTH-01 to AUTH-12)

### AUTH-01: Implement registration screen
**Task Name:** Create User Registration Screen UI and Logic  
**User Story:** As a new customer, I want to register a new account using my email and password so I can access the mobile app.

**Description:**  
Create registration screen component with form fields for email, password, confirm password, full name, and phone number. Implement client-side validation for all fields. Display validation error messages inline. Integrate with registration API endpoint. Handle loading state and success/error responses.

**Acceptance Criteria:**
- [ ] Registration form displays all required fields
- [ ] Email format validated with visual feedback
- [ ] Password strength validated (8+ chars, uppercase, lowercase, numbers, special chars)
- [ ] Confirm password matches password
- [ ] Phone number validated for format
- [ ] Submit button disabled until form valid
- [ ] Loading indicator shown during API call
- [ ] Error messages displayed clearly
- [ ] Success redirects to login screen
- [ ] Form values cleared on success

**Dependencies:** FOUND-01, FOUND-02, FOUND-04  
**Story Points:** 5  
**Priority:** Critical  
**Components/Files:**  
- `app/auth/register.tsx`
- `src/components/forms/RegistrationForm.tsx`
- `src/hooks/useRegistration.ts`
- `src/services/api/authApi.ts`

**Related Requirements:** REQ-3.1.1 (User Registration)

---

### AUTH-02: Implement login screen
**Task Name:** Create User Login Screen UI and Logic  
**User Story:** As a registered customer, I want to log in with my email and password so I can access my account.

**Description:**  
Create login screen with email and password input fields. Implement form validation and error handling. Integrate with login API endpoint. Handle JWT token storage securely using platform-specific storage. Auto-populate user profile after login. Implement forgot password link.

**Acceptance Criteria:**
- [ ] Login form displays email and password fields
- [ ] Form validation working (email format, password required)
- [ ] Loading state shown during authentication
- [ ] Successful login stores JWT token securely
- [ ] User profile fetched and loaded
- [ ] Error messages displayed (invalid credentials, network error)
- [ ] Forgot password link present
- [ ] Remember me functionality (optional)
- [ ] Keyboard returns to login screen after dismissal
- [ ] Redirect to main app after successful login

**Dependencies:** FOUND-01, FOUND-02, FOUND-04, FOUND-02  
**Story Points:** 5  
**Priority:** Critical  
**Components/Files:**  
- `app/auth/login.tsx`
- `src/components/forms/LoginForm.tsx`
- `src/hooks/useLogin.ts`
- `src/services/storage/secureTokenStorage.ts`

**Related Requirements:** REQ-3.1.2 (User Login)

---

### AUTH-03: Implement password reset flow
**Task Name:** Create Password Reset Screen and Email Verification Flow  
**User Story:** As a customer who forgot my password, I want to reset it via email link so I can regain access to my account.

**Description:**  
Create password reset request screen accepting email. Implement password reset email sending. Create password reset confirmation screen that opens from email link. Implement token validation. Create password change form with new password and confirmation. Integrate with backend password reset API.

**Acceptance Criteria:**
- [ ] Password reset request screen accepts email
- [ ] Email sent within 5 seconds of request
- [ ] Reset link in email valid for 1 hour
- [ ] Clicking reset link opens app to reset screen
- [ ] Token validated before allowing password change
- [ ] New password meets complexity requirements
- [ ] Password change successful message displayed
- [ ] Success redirects to login screen
- [ ] Invalid/expired token shows error message
- [ ] All existing sessions invalidated after reset

**Dependencies:** AUTH-02, FOUND-04  
**Story Points:** 5  
**Priority:** High  
**Components/Files:**  
- `app/auth/forgotPassword.tsx`
- `app/auth/resetPassword.tsx`
- `src/components/forms/PasswordResetForm.tsx`
- `src/hooks/usePasswordReset.ts`

**Related Requirements:** REQ-3.1.3 (Password Reset)

---

### AUTH-04: Implement token refresh mechanism
**Task Name:** Create Automatic Token Refresh Logic  
**User Story:** As a customer, I want my session to remain valid during active use so I don't get logged out unexpectedly.

**Description:**  
Implement automatic token refresh in Axios response interceptor. Calculate token expiration time and refresh 30 minutes before expiry. Create refresh token endpoint call. Implement retry logic for failed requests after token refresh. Handle refresh token expiration (force re-login).

**Acceptance Criteria:**
- [ ] Token expiration time tracked
- [ ] Refresh triggered 30 minutes before expiration
- [ ] Refresh token endpoint called successfully
- [ ] New JWT issued with extended expiration
- [ ] Failed requests retried after token refresh
- [ ] Refresh token expires after 7 days
- [ ] User prompted to login if refresh fails
- [ ] No user interaction required for auto-refresh
- [ ] Multiple simultaneous requests don't trigger multiple refreshes

**Dependencies:** FOUND-04, AUTH-02  
**Story Points:** 4  
**Priority:** High  
**Components/Files:**  
- `src/services/api/interceptors/tokenRefresh.ts`
- `src/services/api/tokenManager.ts`
- `src/stores/useUserStore.ts` (updated)

**Related Requirements:** REQ-3.1.4 (Token Refresh)

---

### AUTH-05: Implement secure token storage
**Task Name:** Implement Secure Device Storage for Authentication Tokens  
**User Story:** As a system, I want authentication tokens stored securely on device so sensitive credentials are protected.

**Description:**  
Implement secure token storage using platform-specific APIs (Keychain on iOS, Keystore on Android). Create abstraction layer for secure storage. Implement token retrieval with error handling. Test that tokens cannot be accessed by other apps. Document secure storage approach.

**Acceptance Criteria:**
- [ ] Secure storage service created
- [ ] iOS Keychain implementation working
- [ ] Android Keystore implementation working
- [ ] Token encryption handled automatically
- [ ] Token retrieval working on app launch
- [ ] Clear functionality removes tokens on logout
- [ ] Fallback to secure app container storage
- [ ] No sensitive data in app logs
- [ ] Error handling for missing/corrupted tokens

**Dependencies:** FOUND-02  
**Story Points:** 3  
**Priority:** Critical  
**Components/Files:**  
- `src/services/storage/secureTokenStorage.ts`
- `src/services/storage/nativeModules/keychain.ts` (iOS)
- `src/services/storage/nativeModules/keystore.ts` (Android)

**Related Requirements:** REQ-5.2.1 (Authentication Security), REQ-5.2.2 (Data Encryption)

---

### AUTH-06: Implement logout functionality
**Task Name:** Create Logout Feature with Session Cleanup  
**User Story:** As a customer, I want to log out from my account so I can secure my account when not in use.

**Description:**  
Create logout button in account settings/profile screen. Implement logout API call to invalidate token on backend. Clear all stored tokens from device. Clear user data from Zustand store. Clear TanStack Query cache. Redirect to login screen after logout.

**Acceptance Criteria:**
- [ ] Logout button accessible from settings/profile
- [ ] Confirmation dialog before logout
- [ ] Logout API call sent to backend
- [ ] JWT token cleared from secure storage
- [ ] Zustand user store cleared
- [ ] TanStack Query cache cleared
- [ ] Local storage cleaned up
- [ ] User redirected to login screen
- [ ] Logout completed within 2 seconds
- [ ] Cannot access protected screens after logout

**Dependencies:** AUTH-02, FOUND-02, FOUND-03  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/services/api/authApi.ts` (logout endpoint)
- `src/hooks/useLogout.ts`
- `app/(app)/settings.tsx` (logout button)

**Related Requirements:** REQ-3.1.5 (Logout)

---

### AUTH-07: Add password complexity validation
**Task Name:** Implement Client-Side Password Complexity Validation  
**User Story:** As the system, I want to validate password complexity so user accounts are protected with strong passwords.

**Description:**  
Create password validation utility function checking for: minimum 8 characters, at least one uppercase letter, at least one lowercase letter, at least one digit, at least one special character. Display real-time validation feedback with checkmarks/X marks for each requirement. Show password strength meter.

**Acceptance Criteria:**
- [ ] Password length check (8+ characters)
- [ ] Uppercase letter requirement checked
- [ ] Lowercase letter requirement checked
- [ ] Digit requirement checked
- [ ] Special character requirement checked
- [ ] Real-time feedback displayed
- [ ] Visual indicators for met/unmet requirements
- [ ] Password strength meter displayed
- [ ] Validation applied consistently across app
- [ ] Error message if password doesn't meet requirements

**Dependencies:** AUTH-01, AUTH-03  
**Story Points:** 2  
**Priority:** High  
**Components/Files:**  
- `src/utils/passwordValidation.ts`
- `src/components/forms/PasswordStrengthMeter.tsx`
- `src/hooks/usePasswordValidation.ts`

**Related Requirements:** REQ-3.1.1 (User Registration), REQ-5.2.3 (Input Validation)

---

### AUTH-08: Add email validation service
**Task Name:** Implement Email Format and Verification Service  
**User Story:** As the system, I want to validate email addresses so I ensure valid contact information.

**Description:**  
Create email validation utility using RFC 5321 format validation. Implement email uniqueness check during registration. Create email verification flow (optional): send verification email, verify email link. Display email validation error messages clearly.

**Acceptance Criteria:**
- [ ] Email format validated with RFC 5321 standard
- [ ] Real-time email format feedback
- [ ] Email uniqueness checked against API
- [ ] Loading state shown during uniqueness check
- [ ] "Email already registered" error displayed
- [ ] Email verification implemented (optional)
- [ ] Verification email sent with link
- [ ] Verification token validated
- [ ] Account activated after email verification
- [ ] Resend verification email functionality

**Dependencies:** AUTH-01, FOUND-04  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/utils/emailValidation.ts`
- `src/hooks/useEmailValidation.ts`
- `src/services/api/authApi.ts` (updated)

**Related Requirements:** REQ-3.1.1 (User Registration), REQ-5.2.3 (Input Validation)

---

### AUTH-09: Implement account lockout
**Task Name:** Create Account Lockout After Failed Login Attempts  
**User Story:** As a system, I want to lock accounts after failed login attempts so I prevent brute force attacks.

**Requirement:**  
Implement rate limiting for failed login attempts. Lock account after 5 failed attempts for 15 minutes. Display remaining attempts to user. Show lockout message with countdown timer. Backend rate limiting enforcement. Unlocking via email link.

**Acceptance Criteria:**
- [ ] Failed login attempts tracked
- [ ] Account locked after 5 failed attempts
- [ ] Lockout duration: 15 minutes
- [ ] Remaining attempts displayed to user
- [ ] Lockout message with countdown shown
- [ ] Lockout persists across app restarts
- [ ] Email notification sent on lockout
- [ ] Unlock link in email
- [ ] Backend rate limiting enforced
- [ ] API call returns 429 (Too Many Requests)

**Dependencies:** AUTH-02, FOUND-04, FOUND-10  
**Story Points:** 4  
**Priority:** High  
**Components/Files:**  
- `src/services/api/authApi.ts` (updated)
- `src/utils/loginAttempts.ts`
- `app/auth/accountLocked.tsx`

**Related Requirements:** REQ-5.2.1 (Authentication Security)

---

### AUTH-10: Implement session management
**Task Name:** Create Session Management and Activity Tracking  
**User Story:** As a system, I want to manage user sessions so inactive users are automatically logged out for security.

**Description:**  
Implement session timeout tracking user activity. Determine inactivity duration for timeout (e.g., 15 minutes). Clear session and logout user on timeout. Show warning dialog before logout. Allow user to extend session. Track session start/end times.

**Acceptance Criteria:**
- [ ] Session timeout set to 15 minutes inactivity
- [ ] Activity monitored (touches, page navigation, API calls)
- [ ] Warning dialog shown 1 minute before timeout
- [ ] User can extend session from warning
- [ ] Automatic logout on timeout
- [ ] Session data cleared on logout
- [ ] Session tokens invalidated on backend
- [ ] User notified of timeout
- [ ] Multiple sessions per user handled (if applicable)

**Dependencies:** FOUND-02, AUTH-04  
**Story Points:** 4  
**Priority:** Medium  
**Components/Files:**  
- `src/services/sessionManager.ts`
- `src/hooks/useSessionTimeout.ts`
- `src/components/dialogs/SessionTimeoutWarning.tsx`

**Related Requirements:** REQ-5.2.1 (Authentication Security)

---

### AUTH-11: Add authentication error handling
**Task Name:** Create Comprehensive Authentication Error Handling  
**User Story:** As a user, I want clear error messages when authentication fails so I understand what went wrong.

**Description:**  
Handle various authentication error scenarios: invalid credentials, network errors, server errors, token expired, account locked, email not verified. Map backend error codes to user-friendly messages. Implement retry mechanisms for transient errors. Log errors for debugging.

**Acceptance Criteria:**
- [ ] Invalid credentials error message clear
- [ ] Network error handling with retry option
- [ ] Server error messages generic (no stack traces)
- [ ] Token expiration error handled gracefully
- [ ] Account locked error with countdown
- [ ] Email not verified error with resend option
- [ ] Consistent error message styling
- [ ] Error messages in user's language
- [ ] Errors logged for debugging
- [ ] Timeout errors handled with retry

**Dependencies:** AUTH-02, AUTH-03, FOUND-10  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/services/api/errorHandler.ts`
- `src/utils/authErrorMessages.ts`
- `src/components/errors/AuthErrorDisplay.tsx`

**Related Requirements:** REQ-3.1 (Authentication)

---

### AUTH-12: Create auth tests
**Task Name:** Write Comprehensive Authentication Tests  
**User Story:** As a developer, I want authentication flows tested so I ensure login, registration, and token refresh work correctly.

**Description:**  
Write unit tests for password validation, email validation. Write integration tests for registration flow, login flow, password reset flow. Write tests for token refresh mechanism. Test error scenarios and edge cases. Test secure token storage. Achieve 80%+ coverage for auth services.

**Acceptance Criteria:**
- [ ] Unit tests for validation functions
- [ ] Integration tests for registration flow
- [ ] Integration tests for login flow
- [ ] Integration tests for password reset
- [ ] Token refresh tests
- [ ] Error handling tests
- [ ] Secure storage tests
- [ ] Session timeout tests
- [ ] Test coverage 80%+ for auth
- [ ] All tests passing
- [ ] Mocked API responses working

**Dependencies:** FOUND-06, FOUND-07, FOUND-08, AUTH-01 through AUTH-11  
**Story Points:** 5  
**Priority:** High  
**Components/Files:**  
- `src/services/api/__tests__/authApi.test.ts`
- `src/utils/__tests__/passwordValidation.test.ts`
- `src/utils/__tests__/emailValidation.test.ts`
- `src/tests/integration/auth.integration.test.tsx`

**Related Requirements:** REQ-13 (Testing & Quality)

---

## Phase 4: Menu & Browsing (Tasks MENU-01 to MENU-11)

### MENU-01: Create menu category UI components
**Task Name:** Build Reusable Menu Category Display Components  
**User Story:** As a developer, I want reusable menu category components so I can display categories consistently throughout the app.

**Description:**  
Create MenuCategoryCard component displaying category icon, name, and item count. Create MenuCategoryList component showing scrollable list of categories. Implement category filtering and selection. Add loading states and error states. Style components with NativeWind.

**Acceptance Criteria:**
- [ ] MenuCategoryCard component created
- [ ] Category icon displayed (image or icon set)
- [ ] Category name displayed
- [ ] Item count badge shown
- [ ] Touchable/pressable for selection
- [ ] Loading state skeleton displayed
- [ ] Error state with retry option
- [ ] NativeWind styling applied
- [ ] Responsive layout for different screen sizes
- [ ] Dark mode support

**Dependencies:** INIT-06, FOUND-01  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/components/menu/MenuCategoryCard.tsx`
- `src/components/menu/MenuCategoryList.tsx`
- `src/components/common/SkeletonLoader.tsx`

**Related Requirements:** REQ-3.2.1 (Display Menu Categories)

---

### MENU-02: Implement category display with caching
**Task Name:** Fetch and Cache Menu Categories from API  
**User Story:** As a customer, I want menu categories to load quickly so I can start browsing immediately.

**Description:**  
Create custom TanStack Query hook (useMenuCategories) for fetching categories. Implement 5-minute cache stale time. Create menu category API service. Fetch categories on app launch. Handle loading, error, and success states. Implement manual refresh functionality.

**Acceptance Criteria:**
- [ ] useMenuCategories hook created
- [ ] API call to fetch categories working
- [ ] Cache stale time: 5 minutes
- [ ] Manual refresh available
- [ ] Loading state handled
- [ ] Error state handled with retry
- [ ] Categories displayed when loaded
- [ ] Background refetch on app focus
- [ ] Cached data displayed while fetching

**Dependencies:** FOUND-03, FOUND-04, MENU-01  
**Story Points:** 4  
**Priority:** High  
**Components/Files:**  
- `src/hooks/useMenuCategories.ts`
- `src/services/api/menuApi.ts`
- `app/(app)/menu/index.tsx`

**Related Requirements:** REQ-3.2.1 (Display Menu Categories), REQ-5.1.2 (API Response Times)

---

### MENU-03: Create menu item components
**Task Name:** Build Menu Item Display and Detail Components  
**User Story:** As a developer, I want menu item components so I can display items consistently throughout the app.

**Description:**  
Create MenuItem component for list display (image, name, price, dietary tags). Create MenuItemDetail component for full details (description, ingredients, allergens, reviews, ratings). Implement image gallery for menu items. Create variant selector component for items with options.

**Acceptance Criteria:**
- [ ] MenuItem list component created
- [ ] Item image displayed with loading state
- [ ] Item name and price shown
- [ ] Dietary tags displayed
- [ ] Availability status indicated
- [ ] MenuItemDetail component created
- [ ] Full description displayed
- [ ] Ingredients list shown
- [ ] Allergen information prominently displayed
- [ ] Customer rating and review count shown
- [ ] Image gallery working
- [ ] Variant selector functional

**Dependencies:** INIT-06, FOUND-01, MENU-01  
**Story Points:** 5  
**Priority:** High  
**Components/Files:**  
- `src/components/menu/MenuItem.tsx`
- `src/components/menu/MenuItemDetail.tsx`
- `src/components/menu/VariantSelector.tsx`
- `src/components/common/ImageGallery.tsx`

**Related Requirements:** REQ-3.2.2 (Display Menu Items), REQ-3.2.4 (View Item Details)

---

### MENU-04: Implement item filtering system
**Task Name:** Create Menu Item Filtering by Dietary Preferences  
**User Story:** As a customer, I want to filter menu items by dietary preferences so I can easily find suitable dishes.

**Description:**  
Create filtering UI with checkboxes for dietary options (vegetarian, vegan, gluten-free, etc.). Implement filter logic in component state. Apply filters to menu items list in real-time. Show count of filtered items. Allow clearing all filters.

**Acceptance Criteria:**
- [ ] Filter UI displays dietary options
- [ ] Vegetarian filter working
- [ ] Vegan filter working
- [ ] Gluten-free filter working
- [ ] Multiple filters selectable
- [ ] Items filtered in real-time
- [ ] Filtered item count displayed
- [ ] Clear all filters button present
- [ ] Filter state persisted in session
- [ ] Filters work with search results

**Dependencies:** MENU-02, MENU-03  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/components/menu/FilterBar.tsx`
- `src/hooks/useMenuFilters.ts`
- `src/utils/menuFiltering.ts`

**Related Requirements:** REQ-3.2.2 (Display Menu Items)

---

### MENU-05: Implement sorting functionality
**Task Name:** Add Menu Item Sorting Options  
**User Story:** As a customer, I want to sort menu items by price or popularity so I can find items relevant to me.

**Description:**  
Create sorting options (price low-to-high, price high-to-low, most popular, newest). Implement sorting logic. Add sorting UI with radio buttons or segmented control. Apply sorting in real-time. Show active sort selection.

**Acceptance Criteria:**
- [ ] Sort options available (price, popularity)
- [ ] Price low-to-high sorting working
- [ ] Price high-to-low sorting working
- [ ] Popularity sorting working
- [ ] Sorting UI clear and accessible
- [ ] Active sort indicator shown
- [ ] Sorting applied in real-time
- [ ] Sort persists during browsing
- [ ] Sort works with filters
- [ ] Sort works with search

**Dependencies:** MENU-02, MENU-03  
**Story Points:** 2  
**Priority:** Medium  
**Components/Files:**  
- `src/components/menu/SortBar.tsx`
- `src/hooks/useMenuSorting.ts`
- `src/utils/menuSorting.ts`

**Related Requirements:** REQ-3.2.2 (Display Menu Items)

---

### MENU-06: Implement search functionality
**Task Name:** Create Menu Item Search Feature  
**User Story:** As a customer, I want to search for menu items by name so I can quickly find specific dishes.

**Description:**  
Create search bar component with text input. Implement search API call with debouncing. Display search results with category and price. Support partial name matching. Display search suggestions (optional). Clear search with X button. Show empty state when no results.

**Acceptance Criteria:**
- [ ] Search bar displays on menu screen
- [ ] Search accepts text input
- [ ] Debounced search (300-500ms)
- [ ] Partial name matching working
- [ ] Results display within 500ms
- [ ] Results show item name, category, price
- [ ] "No results" message displayed
- [ ] Clear search button works
- [ ] Search results update in real-time
- [ ] Search is case-insensitive

**Dependencies:** FOUND-03, MENU-02  
**Story Points:** 4  
**Priority:** High  
**Components/Files:**  
- `src/components/menu/SearchBar.tsx`
- `src/hooks/useMenuSearch.ts`
- `src/services/api/menuApi.ts` (updated)

**Related Requirements:** REQ-3.2.3 (Search Menu Items)

---

### MENU-07: Create item detail screen
**Task Name:** Build Complete Menu Item Details Screen  
**User Story:** As a customer, I want to view detailed information about a menu item so I can make an informed decision before ordering.

**Description:**  
Create menu item detail screen showing: full description, high-resolution image gallery, price, preparation time, ingredients, allergens, dietary tags, customer rating/review count, related items. Include add-to-cart functionality. Display variants selector.

**Acceptance Criteria:**
- [ ] Item detail screen loads quickly
- [ ] Full description displayed
- [ ] Image gallery functional with zoom
- [ ] Price, preparation time shown
- [ ] Ingredients list displayed
- [ ] Allergen information prominent
- [ ] Dietary tags visible
- [ ] Customer rating displayed
- [ ] Review count shown
- [ ] Add to cart button present
- [ ] Variant selector visible
- [ ] Related items shown
- [ ] Back button navigates away

**Dependencies:** FOUND-01, MENU-03  
**Story Points:** 5  
**Priority:** High  
**Components/Files:**  
- `app/(app)/menu/[itemId].tsx`
- `src/screens/menu/MenuItemDetailScreen.tsx`

**Related Requirements:** REQ-3.2.4 (View Item Details)

---

### MENU-08: Implement ratings display
**Task Name:** Create Rating Display and Breakdown Components  
**User Story:** As a customer, I want to see ratings and reviews for menu items so I can make informed decisions.

**Description:**  
Create rating display component showing average rating with star icons. Create rating breakdown showing distribution (% by star rating). Implement ratings fetching with TanStack Query. Display review count and "See Reviews" link.

**Acceptance Criteria:**
- [ ] Average rating displayed with stars
- [ ] Rating out of 5 clear
- [ ] Rating breakdown percentage shown
- [ ] Visual representation of distribution
- [ ] Review count displayed
- [ ] "See Reviews" link functional
- [ ] Loading state for ratings
- [ ] Error state handled
- [ ] Ratings update when page reloaded

**Dependencies:** FOUND-03, MENU-03  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/components/menu/RatingDisplay.tsx`
- `src/components/menu/RatingBreakdown.tsx`
- `src/hooks/useMenuItemRatings.ts`

**Related Requirements:** REQ-3.2.4 (View Item Details), REQ-3.6.2 (View Item Ratings)

---

### MENU-09: Add ratings calculation logic
**Task Name:** Implement Rating Calculation and Statistics  
**User Story:** As the system, I want ratings calculated accurately so customers see reliable review information.

**Description:**  
Create utility functions for calculating average rating, rating distribution percentages, rating count by star level. Implement in backend API or on client. Ensure accurate rounding and percentages. Cache calculation results.

**Acceptance Criteria:**
- [ ] Average rating calculated correctly
- [ ] Percentage distribution accurate
- [ ] Rounding consistent (1 decimal place)
- [ ] Handles zero reviews gracefully
- [ ] Calculation efficient (not recalculated unnecessarily)
- [ ] Rating calculations validated with tests
- [ ] Results cached appropriately

**Dependencies:** MENU-08  
**Story Points:** 2  
**Priority:** Medium  
**Components/Files:**  
- `src/utils/ratingCalculations.ts`
- `src/utils/__tests__/ratingCalculations.test.ts`

**Related Requirements:** REQ-3.6.2 (View Item Ratings)

---

### MENU-10: Create menu tests
**Task Name:** Write Menu Browsing Feature Tests  
**User Story:** As a developer, I want menu feature tests written so I ensure browsing functionality works correctly.

**Description:**  
Write unit tests for filtering, sorting, search logic. Write integration tests for category display, item display, item details. Write tests for caching behavior. Test loading and error states. Test search debouncing. Achieve 80%+ coverage for menu services.

**Acceptance Criteria:**
- [ ] Filtering logic unit tests
- [ ] Sorting logic unit tests
- [ ] Search logic unit tests
- [ ] Category loading integration tests
- [ ] Item loading integration tests
- [ ] Caching behavior tests
- [ ] Search debouncing tests
- [ ] Error handling tests
- [ ] Test coverage 80%+ for menu
- [ ] All tests passing

**Dependencies:** FOUND-06, FOUND-07, FOUND-08, MENU-01 through MENU-09  
**Story Points:** 4  
**Priority:** High  
**Components/Files:**  
- `src/utils/__tests__/menuFiltering.test.ts`
- `src/utils/__tests__/menuSorting.test.ts`
- `src/hooks/__tests__/useMenuSearch.test.ts`
- `src/tests/integration/menu.integration.test.tsx`

**Related Requirements:** REQ-13 (Testing & Quality)

---

### MENU-11: Implement menu caching strategy
**Task Name:** Optimize Menu Data Caching for Offline Access  
**User Story:** As a customer without internet, I want to view previously loaded menu so I can browse offline.

**Description:**  
Implement TanStack Query persistent caching using localstorage or AsyncStorage. Cache menu categories, items, and ratings. Implement cache invalidation strategy. Display "cached data" indicator when offline. Implement periodic cache refresh. Manage cache size limits.

**Acceptance Criteria:**
- [ ] Menu categories cached locally
- [ ] Menu items cached locally
- [ ] Ratings cached with items
- [ ] Offline access to cached menu working
- [ ] "Cached data - may not be current" indicator shown
- [ ] Cache auto-refreshes periodically
- [ ] Cache size limited (prevent unbounded growth)
- [ ] Stale cache refreshed on app launch
- [ ] Cache cleared on logout
- [ ] Cache persists across app restarts

**Dependencies:** FOUND-03, MENU-02  
**Story Points:** 3  
**Priority:** Medium  
**Components/Files:**  
- `src/services/cache/menuCacheService.ts`
- `src/services/persistence/offlineStorage.ts`
- `src/hooks/useMenuCategories.ts` (updated)

**Related Requirements:** REQ-5.5.1 (Offline Data Access)

---

## Phase 5: Cart Management (Tasks CART-01 to CART-08)

### CART-01: Implement Zustand cart store
**Task Name:** Create Zustand Store for Shopping Cart State  
**User Story:** As a developer, I want a centralized cart store so cart state is consistent across the app.

**Description:**  
Create Zustand store for cart management with actions: addItem, removeItem, updateQuantity, clearCart. Implement cart persistence using AsyncStorage middleware. Store cart items with selected variants and quantities. Handle cart validation (remove unavailable items).

**Acceptance Criteria:**
- [ ] useCartStore hook created
- [ ] addItem action working
- [ ] removeItem action working
- [ ] updateQuantity action working
- [ ] clearCart action working
- [ ] Cart persisted across app sessions
- [ ] Cart data restored on app launch
- [ ] TypeScript types for cart items
- [ ] Immer middleware for immutable updates
- [ ] Cart state accessible from any component

**Dependencies:** FOUND-02  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/stores/useCartStore.ts`
- `src/types/cart.types.ts`

**Related Requirements:** REQ-3.3 (Cart Management)

---

### CART-02: Create cart UI components
**Task Name:** Build Cart Display and Item Management Components  
**User Story:** As a developer, I want reusable cart components so I can display and manage cart consistently.

**Description:**  
Create CartItem component displaying item name, variant, quantity, price. Create quantity selector (+/- buttons). Create CartSummary component showing subtotal, fees, tax, total. Create empty cart message component.

**Acceptance Criteria:**
- [ ] CartItem component displaying all details
- [ ] Quantity selector with +/- buttons
- [ ] Delete button for removing items
- [ ] CartSummary component created
- [ ] Subtotal calculated and displayed
- [ ] Total amount displayed prominently
- [ ] Empty cart message component
- [ ] NativeWind styling applied
- [ ] Responsive layout
- [ ] Dark mode support

**Dependencies:** INIT-06, CART-01  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/components/cart/CartItem.tsx`
- `src/components/cart/CartSummary.tsx`
- `src/components/cart/EmptyCart.tsx`
- `src/components/common/QuantitySelector.tsx`

**Related Requirements:** REQ-3.3.2 (View Shopping Cart)

---

### CART-03: Implement add to cart logic
**Task Name:** Create Add to Cart Functionality  
**User Story:** As a customer, I want to add items to my cart so I can prepare an order.

**Description:**  
Implement add-to-cart button on menu item screens. Show variant/quantity selection bottom sheet. Validate item availability. Add item to Zustand cart store. Show success toast notification. Update cart badge count.

**Acceptance Criteria:**
- [ ] Add to Cart button present on items
- [ ] Variant selection sheet displayed
- [ ] Quantity selector available
- [ ] Item added to cart on confirmation
- [ ] Success notification shown
- [ ] Cart badge count updated
- [ ] Item added within 500ms
- [ ] Unavailable items cannot be added
- [ ] Item persists in cart after app close

**Dependencies:** CART-01, MENU-03  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/components/menu/AddToCartButton.tsx`
- `src/components/cart/VariantAndQuantitySheet.tsx`
- `src/hooks/useAddToCart.ts`

**Related Requirements:** REQ-3.3.1 (Add Items to Cart)

---

### CART-04: Implement quantity management
**Task Name:** Create Item Quantity Adjustment Functionality  
**User Story:** As a customer, I want to adjust item quantities in my cart so I can modify my order.

**Description:**  
Implement increment/decrement buttons in cart screen. Update quantity in Zustand store. Validate quantity range (1-99). Remove item when quantity reaches zero. Update totals on quantity change.

**Acceptance Criteria:**
- [ ] Increment button increases quantity by 1
- [ ] Decrement button decreases quantity by 1
- [ ] Minimum quantity: 1
- [ ] Maximum quantity: 99
- [ ] Quantity 0 removes item from cart
- [ ] Cart totals update on quantity change
- [ ] Changes persist across app sessions
- [ ] Quantity changes reflected immediately

**Dependencies:** CART-01, CART-02  
**Story Points:** 2  
**Priority:** High  
**Components/Files:**  
- `src/hooks/useUpdateCartQuantity.ts`
- `src/components/common/QuantitySelector.tsx` (updated)

**Related Requirements:** REQ-3.3.3 (Manage Quantity)

---

### CART-05: Implement cart persistence
**Task Name:** Ensure Cart Data Persists Across App Sessions  
**User Story:** As a customer, I want my cart to persist when I close and reopen the app so I don't lose my items.

**Description:**  
Use Zustand with AsyncStorage persistence middleware. Save cart to AsyncStorage when modified. Load cart from AsyncStorage on app launch. Handle corrupted cart data gracefully.

**Acceptance Criteria:**
- [ ] Cart saved to AsyncStorage after modifications
- [ ] Cart loaded on app launch
- [ ] Persisted cart restored in Zustand store
- [ ] Cart persists across multiple app sessions
- [ ] Empty cart data handled gracefully
- [ ] Corrupted data doesn't crash app
- [ ] Cart cleared on logout

**Dependencies:** CART-01  
**Story Points:** 2  
**Priority:** High  
**Components/Files:**  
- `src/stores/useCartStore.ts` (updated)
- `src/services/persistence/cartPersistence.ts`

**Related Requirements:** REQ-3.3 (Cart Management)

---

### CART-06: Create cart tests
**Task Name:** Write Cart Management Feature Tests  
**User Story:** As a developer, I want cart tests written so I ensure cart functionality works correctly.

**Description:**  
Write unit tests for cart store actions. Write tests for quantity management. Write tests for cart persistence. Write integration tests for add to cart flow. Write tests for cart calculations.

**Acceptance Criteria:**
- [ ] Store action unit tests
- [ ] Quantity management tests
- [ ] Persistence tests
- [ ] Add to cart integration tests
- [ ] Cart calculation tests
- [ ] Edge case tests (zero quantity, max quantity)
- [ ] Test coverage 80%+ for cart
- [ ] All tests passing
- [ ] Mocked API responses

**Dependencies:** FOUND-06, FOUND-07, CART-01 through CART-05  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/stores/__tests__/useCartStore.test.ts`
- `src/tests/integration/cart.integration.test.tsx`

**Related Requirements:** REQ-13 (Testing & Quality)

---

### CART-07: Implement cart totals calculation
**Task Name:** Create Cart Total Calculations (Subtotal, Tax, Fees)  
**User Story:** As a customer, I want accurate cart totals so I know the final cost before checkout.

**Description:**  
Create utility functions for calculating subtotal, delivery fee, tax, and total. Implement delivery fee logic based on distance/location. Implement tax calculation based on tax rules. Update totals when items added/removed/quantity changed.

**Acceptance Criteria:**
- [ ] Subtotal calculated correctly
- [ ] Delivery fee calculated based on location
- [ ] Tax calculated correctly
- [ ] Total = Subtotal + Delivery + Tax
- [ ] Totals updated on cart changes
- [ ] Tax rates configurable
- [ ] Accurate to 2 decimal places
- [ ] Free delivery threshold (if applicable)
- [ ] Calculations tested and validated

**Dependencies:** CART-01  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/utils/cartCalculations.ts`
- `src/utils/__tests__/cartCalculations.test.ts`

**Related Requirements:** REQ-3.3.2 (View Shopping Cart)

---

### CART-08: Implement special instructions handling
**Task Name:** Add Special Instructions to Cart Items  
**User Story:** As a customer, I want to add special instructions to items so kitchen knows my preferences.

**Description:**  
Add special instructions field to cart item. Create instructions input component. Store instructions with cart items. Display instructions in cart summary. Include instructions in order placement.

**Acceptance Criteria:**
- [ ] Special instructions field in add-to-cart flow
- [ ] Instructions displayed in cart item
- [ ] Instructions persisted with item
- [ ] Instructions editable in cart
- [ ] Max 500 characters for instructions
- [ ] Instructions included in order
- [ ] Instructions displayed to kitchen staff
- [ ] Instructions cleared on item removal

**Dependencies:** CART-01, CART-03  
**Story Points:** 2  
**Priority:** Medium  
**Components/Files:**  
- `src/components/cart/SpecialInstructionsInput.tsx`
- `src/stores/useCartStore.ts` (updated)
- `src/types/cart.types.ts` (updated)

**Related Requirements:** REQ-3.3.4 (Special Instructions)

---

## Phase 6: Checkout & Orders (Tasks ORDER-01 to ORDER-11)

### ORDER-01: Implement checkout screen
**Task Name:** Create Checkout Screen UI and Flow  
**User Story:** As a customer, I want to proceed to checkout so I can place my order.

**Description:**  
Create checkout screen showing: cart summary, delivery method selection, address input/selection, payment method selection, order special instructions, order confirmation button. Implement form validation. Handle API calls for order creation.

**Acceptance Criteria:**
- [ ] Cart summary displayed with items and totals
- [ ] Delivery/pickup method selection visible
- [ ] Address input with validation
- [ ] Payment method selector
- [ ] Special instructions field
- [ ] Order confirmation button
- [ ] Form validation before submission
- [ ] Loading state during order creation
- [ ] Error handling with retry option
- [ ] Success shows order number

**Dependencies:** FOUND-01, CART-01, CART-02  
**Story Points:** 5  
**Priority:** Critical  
**Components/Files:**  
- `app/(app)/checkout/index.tsx`
- `src/screens/checkout/CheckoutScreen.tsx`
- `src/components/checkout/CheckoutSummary.tsx`
- `src/hooks/useCheckout.ts`

**Related Requirements:** REQ-3.5.1 (Place Order)

---

### ORDER-02: Implement delivery/pickup selection
**Task Name:** Create Delivery and Pickup Options Selection  
**User Story:** As a customer, I want to choose between delivery and pickup so I can select my preferred delivery method.

**Description:**  
Create radio button or segmented control for delivery/pickup selection. Show different address inputs based on selection. For pickup, show pickup location selector. Store selection in checkout form state.

**Acceptance Criteria:**
- [ ] Delivery option selectable
- [ ] Pickup option selectable
- [ ] Address fields shown for delivery
- [ ] Pickup location shown for pickup
- [ ] Selection persists in form
- [ ] Visual indication of selected option
- [ ] Different validation for each option
- [ ] Fee calculated based on method

**Dependencies:** ORDER-01  
**Story Points:** 2  
**Priority:** High  
**Components/Files:**  
- `src/components/checkout/DeliveryMethodSelector.tsx`
- `src/components/checkout/PickupLocationSelector.tsx`

**Related Requirements:** REQ-3.5.1 (Place Order)

---

### ORDER-03: Implement address input with validation
**Task Name:** Create Address Input and Validation Component  
**User Story:** As a customer, I want to enter my delivery address with validation so order is delivered correctly.

**Description:**  
Create address input form with fields: street, city, postal code, optional notes. Implement address validation service. Support saved addresses from profile. Show address suggestions/autocomplete. Display address validation errors clearly.

**Acceptance Criteria:**
- [ ] Address form fields displayed
- [ ] Format validation for all fields
- [ ] City validation (against known cities)
- [ ] Postal code format validated
- [ ] Address suggestions available
- [ ] Saved addresses list available
- [ ] Can select from saved addresses
- [ ] Validation errors displayed
- [ ] Max 10 addresses selectable
- [ ] Notes field for delivery instructions

**Dependencies:** ORDER-01, FOUND-04  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/components/checkout/AddressInput.tsx`
- `src/hooks/useAddressValidation.ts`
- `src/services/api/addressApi.ts`

**Related Requirements:** REQ-3.5.1 (Place Order)

---

### ORDER-04: Implement payment method selection
**Task Name:** Create Payment Method Selection Component  
**User Story:** As a customer, I want to select a payment method so I can pay for my order.

**Description:**  
Create payment method selector showing saved payment methods. Create add new payment method option. Integrate with secure payment gateway (Stripe, PayPal). Show payment method details safely (masked card number). Store only tokenized references.

**Acceptance Criteria:**
- [ ] Saved payment methods displayed
- [ ] Add new payment method option available
- [ ] Payment method selected
- [ ] Card details masked safely
- [ ] Secure payment gateway integration
- [ ] Only tokenized references stored
- [ ] Default payment method highlighted
- [ ] Error messages for payment issues
- [ ] PCI compliance verified

**Dependencies:** ORDER-01, FOUND-04  
**Story Points:** 4  
**Priority:** Critical  
**Components/Files:**  
- `src/components/checkout/PaymentMethodSelector.tsx`
- `src/services/payment/stripeService.ts`
- `src/hooks/usePaymentMethods.ts`

**Related Requirements:** REQ-3.5.1 (Place Order), REQ-5.2.3 (Input Validation)

---

### ORDER-05: Implement order placement API integration
**Task Name:** Integrate Order Creation API Call  
**User Story:** As a system, I want to create orders via API so customer orders are saved to backend.

**Description:**  
Create order API service with createOrder endpoint. Validate all checkout data before sending. Create mutation hook for order placement. Handle API errors and display appropriate messages. Save created order to local storage.

**Acceptance Criteria:**
- [ ] Order creation API call working
- [ ] All checkout data validated before API call
- [ ] Payment processed securely
- [ ] Order created with status "Confirmed"
- [ ] Order response contains order number
- [ ] Order saved locally
- [ ] Error handling for failed orders
- [ ] Timeout handling (5 second timeout)
- [ ] Offline queue support for sync

**Dependencies:** FOUND-04, ORDER-01  
**Story Points:** 3  
**Priority:** Critical  
**Components/Files:**  
- `src/services/api/orderApi.ts`
- `src/hooks/useCreateOrder.ts`

**Related Requirements:** REQ-3.5.1 (Place Order)

---

### ORDER-06: Implement order tracking screen
**Task Name:** Create Order Status Tracking Screen  
**User Story:** As a customer, I want to track my order status so I know when it will arrive.

**Description:**  
Create order detail/tracking screen showing order number, items, current status, preparation time, delivery ETA. Display status timeline (Confirmed → Preparing → Ready → Shipped → Delivered). Show driver location on map for delivery orders. Implement real-time status updates via polling or WebSocket.

**Acceptance Criteria:**
- [ ] Order details displayed clearly
- [ ] Current status shown prominently
- [ ] Status timeline displayed
- [ ] Preparation time shown
- [ ] Delivery/pickup ETA shown
- [ ] Driver location map (if delivery)
- [ ] Real-time status updates received
- [ ] Updates within 1 minute
- [ ] Status history visible
- [ ] Refresh button available

**Dependencies:** FOUND-01, FOUND-03, ORDER-05  
**Story Points:** 5  
**Priority:** High  
**Components/Files:**  
- `app/(app)/orders/[orderId].tsx`
- `src/screens/orders/OrderTrackingScreen.tsx`
- `src/components/orders/OrderStatusTimeline.tsx`
- `src/components/orders/DriverLocationMap.tsx`
- `src/hooks/useOrderTracking.ts`

**Related Requirements:** REQ-3.5.2 (Track Order Status)

---

### ORDER-07: Implement real-time status updates
**Task Name:** Create Real-Time Order Status Updates  
**User Story:** As a customer, I want real-time order updates so I know immediately when my order status changes.

**Description:**  
Implement polling mechanism for order status updates (every 30 seconds). Consider WebSocket for real-time updates if available. Handle connection errors gracefully. Update order state in store. Show notifications on status changes.

**Acceptance Criteria:**
- [ ] Order status polling working (30 second interval)
- [ ] Status updates within 1 minute
- [ ] WebSocket implementation (optional)
- [ ] Connection errors handled
- [ ] Notification shown on status change
- [ ] Battery efficient polling
- [ ] Polling stops when order delivered
- [ ] Polling resumes on app focus

**Dependencies:** ORDER-06, FOUND-02, FOUND-10  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/hooks/useOrderStatusUpdates.ts`
- `src/services/realtime/orderStatusService.ts`
- `src/stores/useOrderStore.ts`

**Related Requirements:** REQ-3.5.2 (Track Order Status), REQ-5.1.4 (Battery Efficiency)

---

### ORDER-08: Implement order history view
**Task Name:** Create Order History Screen  
**User Story:** As a customer, I want to view my past orders so I can see my order history.

**Description:**  
Create orders history screen showing completed orders sorted by date (newest first). Display order number, date, items count, total, status. Implement pagination for orders. Show order details on tap. Allow filtering by status.

**Acceptance Criteria:**
- [ ] Order history displayed (completed orders only)
- [ ] Orders sorted by date (newest first)
- [ ] Each order shows: number, date, total, status
- [ ] Order details expandable
- [ ] Pagination implemented (10 per page)
- [ ] Status filtering available
- [ ] Loading state while fetching
- [ ] Empty state when no orders
- [ ] Caching with TanStack Query

**Dependencies:** FOUND-03, ORDER-01  
**Story Points:** 3  
**Priority:** Medium  
**Components/Files:**  
- `app/(app)/orders/index.tsx`
- `src/screens/orders/OrderHistoryScreen.tsx`
- `src/hooks/useOrderHistory.ts`

**Related Requirements:** REQ-3.5.3 (View Order History)

---

### ORDER-09: Implement order cancellation
**Task Name:** Create Order Cancellation Feature  
**User Story:** As a customer, I want to cancel my order if I change my mind so I can avoid charges.

**Description:**  
Add cancel button to orders in "Confirmed" or "Preparing" status. Show cancellation confirmation dialog. Send cancellation API call. Process refund. Show cancellation message. Update order status to "Cancelled".

**Acceptance Criteria:**
- [ ] Cancel button visible on active orders
- [ ] Cancel not available after delivery started
- [ ] Confirmation dialog before cancellation
- [ ] Cancellation reason optional
- [ ] API call to cancel order sent
- [ ] Refund initiated
- [ ] Order status updated to Cancelled
- [ ] Cancellation email sent
- [ ] Cancellation within 1 second

**Dependencies:** ORDER-01, FOUND-04  
**Story Points:** 3  
**Priority:** Medium  
**Components/Files:**  
- `src/services/api/orderApi.ts` (updated)
- `src/hooks/useCancelOrder.ts`
- `src/components/orders/CancelOrderDialog.tsx`

**Related Requirements:** REQ-3.5.4 (Cancel Order)

---

### ORDER-10: Implement reorder functionality
**Task Name:** Create Reorder Feature for Quick Reordering  
**User Story:** As a customer, I want to quickly reorder items from previous orders so I can save time.

**Description:**  
Add reorder button to order history items. When clicked, populate cart with items from selected order. Allow modification before checkout. Add quick reorder from order detail screen.

**Acceptance Criteria:**
- [ ] Reorder button on order history items
- [ ] Reorder button on order details
- [ ] Cart populated with previous items
- [ ] Items editable before checkout
- [ ] Quantities preserved from previous order
- [ ] Variants preserved
- [ ] Special instructions preserved (optional)
- [ ] User can modify before ordering
- [ ] Availability of items verified

**Dependencies:** CART-01, ORDER-08, ORDER-01  
**Story Points:** 2  
**Priority:** Medium  
**Components/Files:**  
- `src/hooks/useReorder.ts`
- `src/components/orders/ReorderButton.tsx`

**Related Requirements:** REQ-3.5.3 (View Order History)

---

### ORDER-11: Create order tests
**Task Name:** Write Order Management Feature Tests  
**User Story:** As a developer, I want order tests written so I ensure order functionality works correctly.

**Description:**  
Write integration tests for order placement flow. Write tests for order tracking/status updates. Write tests for order cancellation. Write tests for order history. Achieve 80%+ coverage for order services.

**Acceptance Criteria:**
- [ ] Order placement flow tests
- [ ] Order tracking tests
- [ ] Status update tests
- [ ] Order history tests
- [ ] Cancellation tests
- [ ] Reorder tests
- [ ] Error handling tests
- [ ] Test coverage 80%+ for orders
- [ ] All tests passing
- [ ] Mocked API responses

**Dependencies:** FOUND-06, FOUND-07, FOUND-08, ORDER-01 through ORDER-10  
**Story Points:** 4  
**Priority:** High  
**Components/Files:**  
- `src/services/api/__tests__/orderApi.test.ts`
- `src/tests/integration/order.integration.test.tsx`

**Related Requirements:** REQ-13 (Testing & Quality)

---

## Phase 7: Reservations (Tasks RES-01 to RES-10)

### RES-01: Implement reservation screen UI
**Task Name:** Create Reservation Booking Screen UI  
**User Story:** As a customer, I want to book a table reservation so I can secure a seat at my preferred time.

**Description:**  
Create reservation screen with date picker, time selector, party size selector, customer info fields. Display available tables after selection. Show table details (capacity, location, features). Implement form validation.

**Acceptance Criteria:**
- [ ] Date picker displays calendar
- [ ] Time selector shows availability
- [ ] Party size selector (1-12 guests)
- [ ] Customer name field
- [ ] Special requests field
- [ ] Available tables displayed
- [ ] Table details shown (capacity, features)
- [ ] Form validation working
- [ ] Next button disabled until valid
- [ ] Responsive layout

**Dependencies:** FOUND-01  
**Story Points:** 4  
**Priority:** High  
**Components/Files:**  
- `app/(app)/reservations/booking.tsx`
- `src/screens/reservations/ReservationBookingScreen.tsx`
- `src/components/reservations/DateTimeSelector.tsx`

**Related Requirements:** REQ-3.4 (Reservations)

---

### RES-02: Implement date/time picker
**Task Name:** Create Date and Time Selection Components  
**User Story:** As a customer, I want to select my preferred reservation date and time so I can book at my convenience.

**Description:**  
Create calendar date picker component. Create time picker for 30-minute intervals. Show only future dates (no past bookings). Highlight available times. Prevent selecting past times today.

**Acceptance Criteria:**
- [ ] Calendar date picker functional
- [ ] 30-minute time intervals shown
- [ ] Past dates disabled
- [ ] Past times today disabled
- [ ] Minimum date is today
- [ ] Available times highlighted
- [ ] Selected date/time displayed
- [ ] Easy date/time navigation
- [ ] Mobile-friendly UI

**Dependencies:** INIT-06  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/components/common/DatePicker.tsx`
- `src/components/common/TimePicker.tsx`

**Related Requirements:** REQ-3.4.1 (Browse Available Tables)

---

### RES-03: Implement table availability query
**Task Name:** Create Table Availability API Integration  
**User Story:** As the system, I want to query table availability so customers see only available tables.

**Description:**  
Create table availability API hook using TanStack Query. Query database for tables with availability at selected date/time. Return tables with capacity, location, features. Handle loading and error states. Implement debouncing for date/time changes.

**Acceptance Criteria:**
- [ ] useAvailableTables hook created
- [ ] API query returns available tables
- [ ] Debouncing implemented (500ms)
- [ ] Loading state shown
- [ ] Error state handled
- [ ] Empty state when no availability
- [ ] Results cached appropriately
- [ ] Re-query on date/time change
- [ ] Request timeout handled

**Dependencies:** FOUND-03, FOUND-04  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/hooks/useAvailableTables.ts`
- `src/services/api/reservationApi.ts`

**Related Requirements:** REQ-3.4.1 (Browse Available Tables)

---

### RES-04: Implement table selection UI
**Task Name:** Create Table Selection Component  
**User Story:** As a customer, I want to select a specific table so I can choose my preferred seating location.

**Description:**  
Create table grid/list display showing available tables. Show table number, capacity, location (window, corner, center), accessibility features. Implement table selection highlight. Show table details on selection.

**Acceptance Criteria:**
- [ ] Available tables displayed in grid/list
- [ ] Table info shown (number, capacity, location)
- [ ] Accessibility features indicated
- [ ] Selection highlight visible
- [ ] Table details display on tap
- [ ] Visual indication of unavailable tables
- [ ] Selected table persists
- [ ] Can change selection
- [ ] No selection required (optional auto-select)

**Dependencies:** RES-03  
**Story Points:** 2  
**Priority:** High  
**Components/Files:**  
- `src/components/reservations/TableSelector.tsx`
- `src/components/reservations/TableGrid.tsx`

**Related Requirements:** REQ-3.4.1 (Browse Available Tables)

---

### RES-05: Implement reservation creation
**Task Name:** Create Reservation Booking API Integration  
**User Story:** As a system, I want to create reservations via API so bookings are saved to backend.

**Description:**  
Create reservation creation API call with date, time, party size, table, customer info. Validate all data before sending. Generate confirmation code. Handle success/error responses. Show confirmation screen with details.

**Acceptance Criteria:**
- [ ] Reservation creation API integrated
- [ ] Data validated before API call
- [ ] Confirmation code generated
- [ ] Reservation created on backend
- [ ] Confirmation email sent
- [ ] Confirmation code displayed to user
- [ ] Reservation details shown
- [ ] Error handling for conflicts
- [ ] Timeout handling
- [ ] Offline queueing support

**Dependencies:** FOUND-04, RES-01  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/services/api/reservationApi.ts`
- `src/hooks/useCreateReservation.ts`
- `src/screens/reservations/ReservationConfirmation.tsx`

**Related Requirements:** REQ-3.4.2 (Make Reservation)

---

### RES-06: Implement reservation history
**Task Name:** Create Reservation History Screen  
**User Story:** As a customer, I want to view my reservations so I can track my bookings.

**Description:**  
Create reservations history screen showing upcoming and past reservations. Sort by date (upcoming first). Display date, time, party size, table, status. Implement pagination. Allow filtering by status (Confirmed, Cancelled, Completed).

**Acceptance Criteria:**
- [ ] Upcoming reservations listed first
- [ ] Past reservations listed below
- [ ] Each reservation shows key details
- [ ] Status displayed clearly
- [ ] Pagination implemented (10 per page)
- [ ] Filtering by status working
- [ ] Expandable detail view
- [ ] Confirmation code visible
- [ ] Caching with TanStack Query
- [ ] Loading/error states

**Dependencies:** FOUND-03, RES-05  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `app/(app)/reservations/index.tsx`
- `src/screens/reservations/ReservationHistoryScreen.tsx`
- `src/hooks/useReservationHistory.ts`

**Related Requirements:** REQ-3.4.3 (View Reservation History)

---

### RES-07: Implement reservation modification
**Task Name:** Create Reservation Modification Feature  
**User Story:** As a customer, I want to modify my reservation so I can adjust date, time, or party size.

**Description:**  
Add modify button to confirmed reservations. Allow editing date, time, party size, table, special requests. Validate new availability. Show availability conflicts if any. Update reservation on backend. Show confirmation.

**Acceptance Criteria:**
- [ ] Modify button on confirmed reservations only
- [ ] Modification allowed up to 24 hours before
- [ ] Date/time editable with availability check
- [ ] Party size editable
- [ ] Table re-selection available
- [ ] New availability validated
- [ ] Conflicts prevented
- [ ] Modification saved
- [ ] Confirmation email sent
- [ ] Changes reflected immediately

**Dependencies:** RES-06  
**Story Points:** 3  
**Priority:** Medium  
**Components/Files:**  
- `src/hooks/useModifyReservation.ts`
- `src/screens/reservations/ModifyReservationScreen.tsx`

**Related Requirements:** REQ-3.4.4 (Modify Reservation)

---

### RES-08: Implement reservation cancellation
**Task Name:** Create Reservation Cancellation Feature  
**User Story:** As a customer, I want to cancel my reservation if plans change so I free up the table.

**Description:**  
Add cancel button to confirmed reservations. Show cancellation policy. Allow cancellation up to 2 hours before reservation. Show cancellation confirmation. Send cancellation email. Update reservation status.

**Acceptance Criteria:**
- [ ] Cancel button on confirmed reservations
- [ ] Cancellation policy displayed
- [ ] Allowed up to 2 hours before reservation
- [ ] Late cancellation with info message allowed
- [ ] Confirmation dialog before cancellation
- [ ] Cancellation processed
- [ ] Reservation marked Cancelled
- [ ] Cancellation email sent
- [ ] Confirmation code mentioned in cancellation
- [ ] Cancellation within 1 second

**Dependencies:** RES-06  
**Story Points:** 2  
**Priority:** Medium  
**Components/Files:**  
- `src/hooks/useCancelReservation.ts`
- `src/components/reservations/CancelReservationDialog.tsx`

**Related Requirements:** REQ-3.4.5 (Cancel Reservation)

---

### RES-09: Create reservation tests
**Task Name:** Write Reservation Feature Tests  
**User Story:** As a developer, I want reservation tests written so I ensure booking functionality works correctly.

**Description:**  
Write integration tests for reservation booking flow. Write tests for availability queries. Write tests for modification and cancellation. Write tests for date/time validation. Achieve 80%+ coverage.

**Acceptance Criteria:**
- [ ] Booking flow integration tests
- [ ] Availability query tests
- [ ] Modification tests
- [ ] Cancellation tests
- [ ] Validation tests
- [ ] History display tests
- [ ] Error handling tests
- [ ] Test coverage 80%+ for reservations
- [ ] All tests passing
- [ ] Mocked API responses

**Dependencies:** FOUND-06, FOUND-07, FOUND-08, RES-01 through RES-08  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/tests/integration/reservation.integration.test.tsx`
- `src/hooks/__tests__/useAvailableTables.test.ts`

**Related Requirements:** REQ-13 (Testing & Quality)

---

### RES-10: Implement reservation caching
**Task Name:** Create Reservation Data Caching Strategy  
**User Story:** As a customer without internet, I want to view cached reservation data so I can access my bookings offline.

**Description:**  
Implement TanStack Query caching for reservations (10-minute stale time). Cache reservation history locally. Display cached reservation data when offline. Show "cached data" indicator. Update cache when online.

**Acceptance Criteria:**
- [ ] Reservation history cached locally
- [ ] Cache stale time: 10 minutes
- [ ] Offline access to cached data
- [ ] "Cached - may not be current" indicator
- [ ] Cache auto-refreshes when online
- [ ] Cache size limited
- [ ] Cache cleared on logout
- [ ] Stale cache refreshed on app focus

**Dependencies:** FOUND-03, RES-05  
**Story Points:** 2  
**Priority:** Medium  
**Components/Files:**  
- `src/hooks/useReservationHistory.ts` (updated)
- `src/services/cache/reservationCacheService.ts`

**Related Requirements:** REQ-5.5.1 (Offline Data Access)

---

## Phase 8: Reviews & Ratings (Tasks REVIEW-01 to REVIEW-08)

### REVIEW-01: Implement review submission form
**Task Name:** Create Review Submission Form Component  
**User Story:** As a customer, I want to submit reviews for items I've ordered so I can share my dining experience.

**Description:**  
Create review form with star rating, review text (optional, 500 char max), photo uploads (up to 3 images). Implement client-side validation. Show character counter for text. Allow multiple photo selections with preview.

**Acceptance Criteria:**
- [ ] Star rating selector (1-5 stars)
- [ ] Review text field (max 500 chars)
- [ ] Character counter displayed
- [ ] Photo upload (up to 3)
- [ ] Photo preview shown
- [ ] Photo size validation (5MB max each)
- [ ] Remove photo option
- [ ] Submit button on form
- [ ] Form validation working
- [ ] Loading state during submission

**Dependencies:** FOUND-01  
**Story Points:** 4  
**Priority:** High  
**Components/Files:**  
- `src/components/reviews/ReviewForm.tsx`
- `src/components/reviews/PhotoUploadSection.tsx`
- `src/hooks/useReviewForm.ts`

**Related Requirements:** REQ-3.6.1 (Submit Item Review)

---

### REVIEW-02: Implement photo upload for reviews
**Task Name:** Create Photo Upload and Compression Service  
**User Story:** As a customer, I want to add photos to reviews so I can show the actual dish appearance.

**Description:**  
Create photo selection from camera roll or take photo. Implement image compression/optimization. Validate image size and format. Upload photos to backend with review. Show upload progress. Handle upload errors.

**Acceptance Criteria:**
- [ ] Photo selection from gallery
- [ ] Camera photo capture
- [ ] Image compression working
- [ ] Size validation (5MB max)
- [ ] Format validation (JPG, PNG, WebP)
- [ ] Upload progress indicator
- [ ] Multiple photos queued
- [ ] Error handling for failed uploads
- [ ] Photos included in review submission
- [ ] Photos displayed after submission

**Dependencies:** REVIEW-01, FOUND-04  
**Story Points:** 4  
**Priority:** High  
**Components/Files:**  
- `src/services/uploads/photoUploadService.ts`
- `src/hooks/usePhotoUpload.ts`
- `src/utils/imageCompression.ts`

**Related Requirements:** REQ-3.6.1 (Submit Item Review)

---

### REVIEW-03: Implement review display components
**Task Name:** Create Review Display and List Components  
**User Story:** As a developer, I want review display components so I can show reviews consistently.

**Description:**  
Create ReviewItem component showing reviewer name, rating, review text, photos, date. Create ReviewList component with pagination. Implement review filtering and sorting. Show "helpful" count (if applicable).

**Acceptance Criteria:**
- [ ] Review item displays all information
- [ ] Reviewer name shown (anonymized: First + last initial)
- [ ] Star rating displayed
- [ ] Review text displayed
- [ ] Photos displayed as gallery
- [ ] Review date shown (relative format)
- [ ] Multiple reviews in list
- [ ] Pagination working (5 per page)
- [ ] Sorting options available
- [ ] Responsive layout

**Dependencies:** INIT-06, FOUND-01  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/components/reviews/ReviewItem.tsx`
- `src/components/reviews/ReviewList.tsx`
- `src/components/common/ReviewPhotoGallery.tsx`

**Related Requirements:** REQ-3.6.2 (View Item Ratings)

---

### REVIEW-04: Implement ratings breakdown
**Task Name:** Create Rating Distribution Display Component  
**User Story:** As a customer, I want to see the breakdown of ratings so I understand the distribution of opinions.

**Description:**  
Create ratings breakdown component showing percentage distribution across 5-star range. Display as horizontal bars with percentages. Show total review count. Show average rating prominently.

**Acceptance Criteria:**
- [ ] Breakdown shows all 5 rating levels
- [ ] Percentage for each level displayed
- [ ] Visual bar representation
- [ ] Total review count shown
- [ ] Average rating displayed
- [ ] Responsive layout
- [ ] Accurate percentages
- [ ] Color-coded ratings (optional)

**Dependencies:** REVIEW-03, MENU-09  
**Story Points:** 2  
**Priority:** Medium  
**Components/Files:**  
- `src/components/reviews/RatingsBreakdown.tsx`

**Related Requirements:** REQ-3.6.2 (View Item Ratings)

---

### REVIEW-05: Implement experience rating
**Task Name:** Create Overall Dining Experience Rating  
**User Story:** As a customer, I want to rate my overall experience so the restaurant gets valuable feedback.

**Description:**  
Create experience rating screen after order delivery. Display star rating selector for overall experience. Optional service quality rating. Optional food quality rating. Optional feedback text (300 chars max). Submit to backend.

**Acceptance Criteria:**
- [ ] Experience rating selector (1-5 stars) required
- [ ] Service quality rating optional
- [ ] Food quality rating optional
- [ ] Feedback text field (max 300 chars)
- [ ] Character counter displayed
- [ ] Submit button
- [ ] Form validation
- [ ] Success message
- [ ] Ratings stored on backend
- [ ] Rating prompt dismissible

**Dependencies:** ORDER-06, FOUND-04  
**Story Points:** 3  
**Priority:** Medium  
**Components/Files:**  
- `src/components/orders/ExperienceRatingPrompt.tsx`
- `src/hooks/useSubmitExperienceRating.ts`

**Related Requirements:** REQ-3.6.3 (Rate Dining Experience)

---

### REVIEW-06: Implement service quality rating
**Task Name:** Create Service Quality Rating Component  
**User Story:** As a customer, I want to rate service quality so restaurant can improve customer service.

**Description:**  
Add service quality rating component with 1-5 star selector. Show description of each rating level. Optional comment field for service feedback. Include in experience rating or as separate flow.

**Acceptance Criteria:**
- [ ] Service quality rating selector
- [ ] 1-5 star options shown
- [ ] Rating descriptions displayed
- [ ] Comment field for feedback
- [ ] Optional rating
- [ ] Rating stored with submission
- [ ] Accessible from experience rating

**Dependencies:** REVIEW-05  
**Story Points:** 2  
**Priority:** Medium  
**Components/Files:**  
- `src/components/reviews/ServiceQualityRating.tsx`

**Related Requirements:** REQ-3.6.3 (Rate Dining Experience)

---

### REVIEW-07: Create review tests
**Task Name:** Write Review Feature Tests  
**User Story:** As a developer, I want review tests written so I ensure review functionality works correctly.

**Description:**  
Write tests for review form validation. Write tests for photo upload. Write tests for review display. Write tests for ratings calculations. Write integration tests for review submission flow.

**Acceptance Criteria:**
- [ ] Review form validation tests
- [ ] Photo upload tests
- [ ] Review display tests
- [ ] Ratings calculation tests
- [ ] Submission flow tests
- [ ] Error handling tests
- [ ] Test coverage 80%+ for reviews
- [ ] All tests passing
- [ ] Mocked API responses

**Dependencies:** FOUND-06, FOUND-07, FOUND-08, REVIEW-01 through REVIEW-06  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/components/reviews/__tests__/ReviewForm.test.tsx`
- `src/tests/integration/review.integration.test.tsx`

**Related Requirements:** REQ-13 (Testing & Quality)

---

### REVIEW-08: Implement review filtering/sorting
**Task Name:** Create Review Filtering and Sorting Features  
**User Story:** As a customer, I want to filter and sort reviews so I can find most relevant feedback.

**Description:**  
Implement filtering by rating (show only 5-star, or 4+ star reviews, etc.). Implement sorting (newest first, highest rating, lowest rating). Display active filters. Allow clearing filters. Real-time filter application.

**Acceptance Criteria:**
- [ ] Filter by rating working (1-5 stars)
- [ ] Multiple rating selections available
- [ ] Sorting options: newest, highest rating, lowest rating
- [ ] Active filters displayed
- [ ] Clear filters button works
- [ ] Real-time application of filters
- [ ] Review count updates on filter
- [ ] Filters persist during session
- [ ] Mobile-friendly filter UI

**Dependencies:** REVIEW-03, REVIEW-04  
**Story Points:** 2  
**Priority:** Medium  
**Components/Files:**  
- `src/components/reviews/ReviewFilterBar.tsx`
- `src/hooks/useReviewFilters.ts`

**Related Requirements:** REQ-3.6.2 (View Item Ratings)

---

## Phase 9: User Profile (Tasks PROFILE-01 to PROFILE-09)

### PROFILE-01: Implement profile view/edit screen
**Task Name:** Create User Profile Display and Edit Screen  
**User Story:** As a customer, I want to view and edit my profile information so I keep my account details current.

**Description:**  
Create profile screen showing name, email, phone, profile picture, language preference. Implement edit mode with input fields. Save changes to backend. Display loading state during save. Show success message. Allow uploading profile picture.

**Acceptance Criteria:**
- [ ] Profile displays user information
- [ ] Edit button available
- [ ] Edit mode shows input fields
- [ ] Name editable with validation
- [ ] Phone editable with validation
- [ ] Profile picture uploadable
- [ ] Picture preview shown
- [ ] Changes saved on submit
- [ ] Loading state during save
- [ ] Success notification displayed
- [ ] Cancel edit option available

**Dependencies:** FOUND-01, FOUND-04  
**Story Points:** 4  
**Priority:** High  
**Components/Files:**  
- `app/(app)/profile/index.tsx`
- `src/screens/profile/ProfileScreen.tsx`
- `src/components/profile/ProfileForm.tsx`
- `src/hooks/useUpdateProfile.ts`

**Related Requirements:** REQ-3.7.1 (View and Edit Profile)

---

### PROFILE-02: Implement address management
**Task Name:** Create Address Management Screen  
**User Story:** As a customer, I want to save multiple delivery addresses so I can quickly select during checkout.

**Description:**  
Create addresses screen showing saved addresses with labels. Implement add new address flow. Implement edit address functionality. Implement delete address with confirmation. Allow setting default address. Show address on map.

**Acceptance Criteria:**
- [ ] List of saved addresses displayed
- [ ] Address labels shown (Home, Work, Other)
- [ ] Add new address button
- [ ] Edit address option
- [ ] Delete address with confirmation
- [ ] Set default address option
- [ ] Max 10 addresses
- [ ] Default address indicated
- [ ] Address details complete
- [ ] Changes saved to backend

**Dependencies:** FOUND-01, FOUND-04  
**Story Points:** 4  
**Priority:** High  
**Components/Files:**  
- `app/(app)/profile/addresses.tsx`
- `src/screens/profile/AddressManagementScreen.tsx`
- `src/components/profile/AddressItem.tsx`
- `src/hooks/useAddressManagement.ts`

**Related Requirements:** REQ-3.7.2 (Manage Delivery Addresses)

---

### PROFILE-03: Implement address validation
**Task Name:** Create Address Format and Geographic Validation  
**User Story:** As the system, I want to validate addresses so delivery locations are accurate.

**Description:**  
Implement address format validation (street, city, postal code). Validate geographic validity using address validation service. Show validation errors. Support address suggestions/autocomplete. Store validated addresses.

**Acceptance Criteria:**
- [ ] Street address format validated
- [ ] City validated against known cities
- [ ] Postal code format validated
- [ ] Geographic validity checked
- [ ] Validation errors displayed
- [ ] Address suggestions available
- [ ] Autocomplete working
- [ ] Invalid addresses prevented
- [ ] Validation in real-time (optional)

**Dependencies:** PROFILE-02, FOUND-04  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/utils/addressValidation.ts`
- `src/services/api/addressApi.ts`
- `src/hooks/useAddressValidation.ts`

**Related Requirements:** REQ-3.7.2 (Manage Delivery Addresses), REQ-5.2.3 (Input Validation)

---

### PROFILE-04: Implement payment methods management
**Task Name:** Create Payment Method Management Screen  
**User Story:** As a customer, I want to manage my payment methods so I can use different payment options.

**Description:**  
Create payment methods screen showing saved payment methods (masked card numbers). Implement add new payment method with secure gateway. Implement delete payment method. Allow setting default payment method. Max 5 payment methods per account.

**Acceptance Criteria:**
- [ ] Saved payment methods displayed
- [ ] Card numbers masked safely
- [ ] Add payment method button
- [ ] Secure payment gateway integrated
- [ ] Delete payment method option
- [ ] Set default payment method
- [ ] Max 5 methods per account
- [ ] Only tokenized references stored
- [ ] Default method indicated
- [ ] Changes saved

**Dependencies:** FOUND-01, FOUND-04  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `app/(app)/profile/paymentMethods.tsx`
- `src/screens/profile/PaymentMethodsScreen.tsx`
- `src/hooks/usePaymentMethods.ts`

**Related Requirements:** REQ-3.7.3 (Manage Payment Methods)

---

### PROFILE-05: Implement preferences management
**Task Name:** Create User Preferences Settings Screen  
**User Story:** As a customer, I want to manage my preferences so I control my app experience.

**Description:**  
Create preferences screen with toggles for various settings (notifications, promotional emails, special offers). Store preferences in backend. Persist preferences locally in Zustand store. Apply preferences throughout app.

**Acceptance Criteria:**
- [ ] Preferences screen displays all options
- [ ] Toggle switches for each setting
- [ ] Changes saved immediately
- [ ] Preferences persisted to backend
- [ ] Preferences loaded on login
- [ ] Preferences cached locally
- [ ] Privacy policy link
- [ ] Account deletion link

**Dependencies:** FOUND-02, FOUND-04  
**Story Points:** 2  
**Priority:** Medium  
**Components/Files:**  
- `app/(app)/settings/preferences.tsx`
- `src/screens/settings/PreferencesScreen.tsx`
- `src/stores/usePreferencesStore.ts`
- `src/hooks/useUpdatePreferences.ts`

**Related Requirements:** REQ-3.7.4 (Preferences and Notifications)

---

### PROFILE-06: Implement notification settings
**Task Name:** Create Notification Preference Configuration  
**User Story:** As a customer, I want to control notification preferences so I receive only desired alerts.

**Description:**  
Create notification settings screen with toggles: order updates, promotions, special offers, operational notifications. Implement push notification permission requests. Respect system notification settings. Send test notification.

**Acceptance Criteria:**
- [ ] Notification options toggleable
- [ ] Order status update notifications
- [ ] Promotional notifications
- [ ] Special offer notifications
- [ ] Operational notifications
- [ ] System permission handling
- [ ] Test notification button
- [ ] Settings saved and applied
- [ ] Graceful handling when disabled

**Dependencies:** PROFILE-05, FOUND-10  
**Story Points:** 3  
**Priority:** Medium  
**Components/Files:**  
- `src/components/settings/NotificationSettings.tsx`
- `src/hooks/useNotificationSettings.ts`
- `src/services/notifications/pushNotificationService.ts`

**Related Requirements:** REQ-3.7.4 (Preferences and Notifications)

---

### PROFILE-07: Implement language preference
**Task Name:** Create Language Selection Interface  
**User Story:** As a user, I want to select my preferred language so I use the app in English or Arabic.

**Description:**  
Create language selector with English and Arabic options. Update app language immediately on selection. Persist language preference to backend and local storage. Apply RTL layout for Arabic.

**Acceptance Criteria:**
- [ ] Language selector in settings
- [ ] English and Arabic options
- [ ] Language change immediate
- [ ] No app restart required
- [ ] RTL layout applied for Arabic
- [ ] Language preference saved
- [ ] Preference persisted across sessions
- [ ] Preference synced to backend
- [ ] All content translated

**Dependencies:** FOUND-02, PROFILE-05  
**Story Points:** 2  
**Priority:** High  
**Components/Files:**  
- `src/components/settings/LanguageSelector.tsx`
- `src/hooks/useLanguagePreference.ts`
- `src/stores/usePreferencesStore.ts`

**Related Requirements:** REQ-5.3.1 (Multi-Language Support)

---

### PROFILE-08: Implement account deletion
**Task Name:** Create Account Deletion Feature  
**User Story:** As a customer, I want to delete my account if I no longer want to use the service.

**Description:**  
Create account deletion option in settings. Display warning dialog with consequences. Require password confirmation. Send confirmation email. Implement 30-day grace period for account recovery. Delete all customer data after grace period.

**Acceptance Criteria:**
- [ ] Delete account option in settings
- [ ] Warning dialog with consequences
- [ ] Password confirmation required
- [ ] Confirmation email sent
- [ ] 30-day grace period for recovery
- [ ] Account marked as deleted immediately
- [ ] Data deleted after grace period
- [ ] Account recovery link in email
- [ ] Cancellation of grace period possible

**Dependencies:** FOUND-04  
**Story Points:** 4  
**Priority:** Medium  
**Components/Files:**  
- `src/components/settings/DeleteAccountDialog.tsx`
- `src/services/api/userApi.ts`
- `src/hooks/useDeleteAccount.ts`

**Related Requirements:** REQ-3.7.5 (Delete Account)

---

### PROFILE-09: Create profile tests
**Task Name:** Write User Profile Feature Tests  
**User Story:** As a developer, I want profile tests written so I ensure profile functionality works correctly.

**Description:**  
Write tests for profile display and editing. Write tests for address management and validation. Write tests for payment methods. Write tests for preferences and settings. Write integration tests for profile update flow.

**Acceptance Criteria:**
- [ ] Profile display tests
- [ ] Profile edit tests
- [ ] Address management tests
- [ ] Payment methods tests
- [ ] Preferences tests
- [ ] Language preference tests
- [ ] Account deletion tests
- [ ] Test coverage 80%+ for profile
- [ ] All tests passing
- [ ] Mocked API responses

**Dependencies:** FOUND-06, FOUND-07, FOUND-08, PROFILE-01 through PROFILE-08  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/screens/profile/__tests__/ProfileScreen.test.tsx`
- `src/tests/integration/profile.integration.test.tsx`

**Related Requirements:** REQ-13 (Testing & Quality)

---

## Phase 10: Staff App (Tasks STAFF-01 to STAFF-12)

### STAFF-01: Implement staff login
**Task Name:** Create Staff Authentication Screen  
**User Story:** As a restaurant staff member, I want to log in to the staff app so I can access operational features.

**Description:**  
Create staff login screen with username/employee ID and password fields. Validate staff credentials and verify role assignment. Issue JWT token with role information. Redirect to role-appropriate dashboard. Display role on home screen.

**Acceptance Criteria:**
- [ ] Staff login form with username/ID and password
- [ ] Role verification on successful login
- [ ] JWT token includes staff role
- [ ] Role-appropriate dashboard displayed
- [ ] Account lockout after 5 failed attempts
- [ ] Loading state during authentication
- [ ] Error messages for invalid credentials
- [ ] Logout clears staff session
- [ ] Token stored securely

**Dependencies:** FOUND-01, FOUND-04, AUTH-02  
**Story Points:** 3  
**Priority:** Critical  
**Components/Files:**  
- `app/staff/login.tsx`
- `src/components/staff/StaffLoginForm.tsx`
- `src/services/api/staffAuthApi.ts`

**Related Requirements:** REQ-4.1.1 (Staff Login)

---

### STAFF-02: Create staff reservations dashboard
**Task Name:** Build Staff Reservations Management Dashboard  
**User Story:** As a waiter, I want to view today's reservations so I can prepare tables and manage seating.

**Description:**  
Create staff dashboard showing today's reservations sorted by time. Display party size, reservation time, customer name, table assignment, status. Implement filtering by status and time range. Color code status visually. Auto-refresh every 2 minutes.

**Acceptance Criteria:**
- [ ] Current day reservations displayed
- [ ] Sorted by reservation time
- [ ] Each shows: party size, time, customer, table, status
- [ ] Filtering by status available
- [ ] Manual refresh available
- [ ] Auto-refresh every 2 minutes
- [ ] Color coding for status
- [ ] Scrollable for many reservations
- [ ] Touch targets appropriately sized

**Dependencies:** FOUND-01, STAFF-01  
**Story Points:** 4  
**Priority:** High  
**Components/Files:**  
- `app/(staff)/dashboard.tsx`
- `src/screens/staff/ReservationsDashboard.tsx`
- `src/components/staff/ReservationCard.tsx`

**Related Requirements:** REQ-4.2.1 (View Reservations Overview)

---

### STAFF-03: Implement check-in functionality
**Task Name:** Create Customer Check-In Feature  
**User Story:** As a waiter, I want to check in customers when they arrive so I can mark table as occupied.

**Description:**  
Create check-in screen accessible from reservation. Verify customer name/presence. Allow table assignment if not pre-assigned. Update reservation status to "Checked In". Notify kitchen. Update table status in real-time.

**Acceptance Criteria:**
- [ ] Check-in button on reservations
- [ ] Customer name verification
- [ ] Table assignment/confirmation
- [ ] Reservation status updated to Checked In
- [ ] Table status updated in real-time
- [ ] Kitchen notified of new table
- [ ] Check-in processed within 1 second
- [ ] Confirmation displayed to staff
- [ ] Back to dashboard after check-in

**Dependencies:** STAFF-02, FOUND-04  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/screens/staff/CheckInScreen.tsx`
- `src/services/api/staffApi.ts`
- `src/hooks/useCheckInCustomer.ts`

**Related Requirements:** REQ-4.2.2 (Check In Customers)

---

### STAFF-04: Implement table management
**Task Name:** Create Table Status Management Interface  
**User Story:** As a waiter, I want to manage table occupancy so I can track available tables.

**Description:**  
Create table management screen showing all tables with status (available, occupied, reserved, being cleaned). Implement table status updates. Show occupancy duration. Allow marking tables as cleaned/available.

**Acceptance Criteria:**
- [ ] All tables displayed with status
- [ ] Status color coded
- [ ] Occupancy duration shown
- [ ] Mark as cleaned button
- [ ] Status transitions working
- [ ] Real-time status updates
- [ ] Table details (capacity, location)
- [ ] Responsive table layout
- [ ] Manual refresh available

**Dependencies:** STAFF-02  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/screens/staff/TableManagementScreen.tsx`
- `src/components/staff/TableStatus.tsx`
- `src/hooks/useTableManagement.ts`

**Related Requirements:** REQ-4.2 (Reservation Management)

---

### STAFF-05: Create order management interface
**Task Name:** Build Chef/Staff Order Management Screen  
**User Story:** As a chef, I want to view active orders so I can see what needs to be prepared.

**Description:**  
Create active orders screen showing orders with status "Confirmed" or "Preparing". Display order number, items, special instructions, preparation time remaining, table/delivery info. Sort by urgency. Show new order notifications.

**Acceptance Criteria:**
- [ ] Active orders displayed
- [ ] Order number, items, instructions shown
- [ ] Preparation time displayed
- [ ] Sorted by urgency
- [ ] New order notifications triggered
- [ ] Real-time status updates
- [ ] Special instructions highlighted
- [ ] Completed orders hidden
- [ ] Manual refresh available

**Dependencies:** FOUND-01, STAFF-01  
**Story Points:** 4  
**Priority:** High  
**Components/Files:**  
- `app/(staff)/orders.tsx`
- `src/screens/staff/OrderManagementScreen.tsx`
- `src/components/staff/OrderCard.tsx`

**Related Requirements:** REQ-4.3.1 (View Active Orders)

---

### STAFF-06: Implement order status updates
**Task Name:** Create Order Status Transition Functionality  
**User Story:** As a chef, I want to update order status as I prepare items so staff knows when order is ready.

**Description:**  
Create order detail screen with status update buttons (Confirmed → Preparing → Ready). Validate status transitions. Timestamp all transitions. Notify waiter/delivery when ready. Update customer app immediately.

**Acceptance Criteria:**
- [ ] Order detail screen shows current status
- [ ] Status transition buttons available
- [ ] Valid transitions enforced
- [ ] Timestamps recorded
- [ ] Waiter notified when ready
- [ ] Customer notified when ready
- [ ] Status changes reflected immediately
- [ ] Cannot skip status levels
- [ ] Confirmation before status change

**Dependencies:** STAFF-05, FOUND-04  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/screens/staff/OrderDetailScreen.tsx`
- `src/components/staff/OrderStatusUpdater.tsx`
- `src/hooks/useUpdateOrderStatus.ts`

**Related Requirements:** REQ-4.3.2 (Update Order Status)

---

### STAFF-07: Implement special instructions display
**Task Name:** Create Special Instructions Display for Orders  
**User Story:** As a chef, I want to see special instructions for orders so I can prepare items exactly as requested.

**Description:**  
Display special instructions prominently on order detail screen. Highlight allergen-related instructions in red. Allow chef to acknowledge reading instructions. Track acknowledgment.

**Acceptance Criteria:**
- [ ] Instructions displayed prominently
- [ ] Allergen warnings highlighted in red
- [ ] Instructions clearly formatted
- [ ] Acknowledgment option available
- [ ] Acknowledgment tracked and timestamped
- [ ] Instructions persist through order lifecycle
- [ ] Print instructions button (optional)

**Dependencies:** STAFF-05  
**Story Points:** 2  
**Priority:** High  
**Components/Files:**  
- `src/components/staff/SpecialInstructionsDisplay.tsx`
- `src/hooks/useAcknowledgeInstructions.ts`

**Related Requirements:** REQ-4.3.3 (Special Order Instructions)

---

### STAFF-08: Implement role-based access control
**Task Name:** Create Role-Based Feature Access Control  
**User Story:** As the system, I want to enforce role-based access so only authorized staff can access certain features.

**Description:**  
Implement RBAC with roles: Waiter, Chef, Manager, Admin. Configure permissions for each role. Validate JWT token claims on each screen. Hide unauthorized features. Redirect to access denied on unauthorized access.

**Acceptance Criteria:**
- [ ] Waiter role can access reservations and orders
- [ ] Chef role can access order preparation
- [ ] Manager role can access all staff features
- [ ] Admin role can access all features
- [ ] JWT token includes role claim
- [ ] Features hidden from unauthorized roles
- [ ] Unauthorized access redirected
- [ ] Role changes require re-authentication
- [ ] Consistent permission enforcement

**Dependencies:** STAFF-01, FOUND-04  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/services/auth/roleBasedAccess.ts`
- `src/hooks/useRoleBasedAccess.ts`
- `src/utils/permissions.ts`

**Related Requirements:** REQ-4.4.1 (Role-Based Feature Access)

---

### STAFF-09: Implement permission validation
**Task Name:** Create Permission Validation on Sensitive Actions  
**User Story:** As the system, I want to validate permissions on sensitive actions so unauthorized operations are prevented.

**Description:**  
Validate user role and permissions before sensitive operations. Implement both client-side and server-side validation. Log unauthorized attempts as security events. Display clear error messages.

**Acceptance Criteria:**
- [ ] Permissions validated before sensitive actions
- [ ] Client-side validation for UX
- [ ] Server-side validation for security
- [ ] Unauthorized attempts logged
- [ ] Consistent enforcement
- [ ] Clear error messages
- [ ] No sensitive info in errors
- [ ] Security event logging

**Dependencies:** STAFF-08  
**Story Points:** 2  
**Priority:** High  
**Components/Files:**  
- `src/services/auth/permissionValidator.ts`
- `src/hooks/usePermissionValidation.ts`

**Related Requirements:** REQ-4.4.2 (Permission Validation)

---

### STAFF-10: Create staff tests
**Task Name:** Write Staff App Feature Tests  
**User Story:** As a developer, I want staff app tests written so I ensure staff functionality works correctly.

**Description:**  
Write tests for staff login. Write tests for reservations dashboard. Write tests for order management. Write tests for role-based access. Write integration tests for staff workflows.

**Acceptance Criteria:**
- [ ] Staff login tests
- [ ] Reservations dashboard tests
- [ ] Order management tests
- [ ] Check-in functionality tests
- [ ] RBAC tests
- [ ] Permission validation tests
- [ ] Error handling tests
- [ ] Test coverage 80%+ for staff
- [ ] All tests passing

**Dependencies:** FOUND-06, FOUND-07, FOUND-08, STAFF-01 through STAFF-09  
**Story Points:** 4  
**Priority:** High  
**Components/Files:**  
- `src/tests/integration/staff.integration.test.tsx`
- `src/services/auth/__tests__/roleBasedAccess.test.ts`

**Related Requirements:** REQ-13 (Testing & Quality)

---

### STAFF-11: Implement staff notifications
**Task Name:** Create Staff Notification System  
**User Story:** As a staff member, I want to receive notifications so I'm alerted to important events.

**Description:**  
Implement push notifications for: new orders, reservation check-ins, order ready alerts, system messages. Show visual and/or audio notifications. Display notification center showing recent notifications.

**Acceptance Criteria:**
- [ ] Push notifications implemented
- [ ] New order notifications
- [ ] Check-in notifications
- [ ] Ready for pickup notifications
- [ ] System message notifications
- [ ] Visual indicators (badges)
- [ ] Sound notifications (optional)
- [ ] Notification center showing history
- [ ] Mark as read functionality

**Dependencies:** FOUND-10, STAFF-01  
**Story Points:** 3  
**Priority:** Medium  
**Components/Files:**  
- `src/services/notifications/staffNotifications.ts`
- `src/screens/staff/NotificationCenter.tsx`

**Related Requirements:** REQ-4.2.1, REQ-4.3.1

---

### STAFF-12: Implement role-based UI rendering
**Task Name:** Create Conditional UI Rendering Based on Staff Role  
**User Story:** As a staff member, I want to see only relevant features for my role so I can work efficiently.

**Description:**  
Implement conditional rendering of UI components based on user role. Hide irrelevant features. Show role-specific dashboards. Optimize UI for each role's workflow.

**Acceptance Criteria:**
- [ ] Role-based component rendering
- [ ] Waiter-specific UI rendered
- [ ] Chef-specific UI rendered
- [ ] Manager-specific UI rendered
- [ ] Admin-specific UI rendered
- [ ] Irrelevant features hidden
- [ ] Navigation reflects role permissions
- [ ] Dashboard customized per role
- [ ] Context switching between roles

**Dependencies:** STAFF-08  
**Story Points:** 2  
**Priority:** Medium  
**Components/Files:**  
- `src/components/staff/RoleBasedRenderer.tsx`
- `src/hooks/useStaffRole.ts`

**Related Requirements:** REQ-4.4.1 (Role-Based Feature Access)

---

## Phase 11: Internationalization (Tasks I18N-01 to I18N-07)

### I18N-01: Set up i18next configuration
**Task Name:** Configure i18next Localization Framework  
**User Story:** As a developer, I want i18next configured so I can manage multilingual content efficiently.

**Description:**  
Install and configure i18next library. Set up language detection (device language). Create i18next instance with English and Arabic languages. Configure language switching. Set up fallback language to English.

**Acceptance Criteria:**
- [ ] i18next installed and configured
- [ ] Language detection working
- [ ] English set as fallback
- [ ] Language switching functional
- [ ] Configuration file structured properly
- [ ] Namespaces configured (common, auth, menu, etc.)
- [ ] Custom language detection logic

**Dependencies:** INIT-02  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/services/i18n/i18n.ts`
- `src/config/i18n.config.ts`
- `src/services/i18n/languageDetector.ts`

**Related Requirements:** REQ-5.3 (Internationalization)

---

### I18N-02: Create English translation files
**Task Name:** Build English Translation Resource Files  
**User Story:** As a developer, I want all English content externalized so I can manage text centrally.

**Description:**  
Create JSON translation files for English language. Organize translations by namespace (common, auth, menu, orders, reservations, profile, settings, staff). Include all UI text, labels, messages, errors. Use descriptive keys.

**Acceptance Criteria:**
- [ ] Translation files created for all namespaces
- [ ] Common namespace: basic UI labels
- [ ] Auth namespace: authentication messages
- [ ] Menu namespace: menu-related text
- [ ] Orders namespace: order-related text
- [ ] Reservations namespace: reservation text
- [ ] Profile namespace: profile text
- [ ] Settings namespace: settings text
- [ ] Staff namespace: staff app text
- [ ] Keys are consistent and descriptive
- [ ] All UI text included

**Dependencies:** I18N-01  
**Story Points:** 5  
**Priority:** High  
**Components/Files:**  
- `src/locales/en/common.json`
- `src/locales/en/auth.json`
- `src/locales/en/menu.json`
- `src/locales/en/orders.json`
- `src/locales/en/reservations.json`
- `src/locales/en/profile.json`
- `src/locales/en/settings.json`
- `src/locales/en/staff.json`

**Related Requirements:** REQ-5.3.1 (Multi-Language Support)

---

### I18N-03: Create Arabic translation files
**Task Name:** Build Arabic Translation Resource Files  
**User Story:** As an Arabic-speaking user, I want the app in Arabic so I understand all content.

**Description:**  
Create JSON translation files for Arabic language with same structure as English. Translate all content accurately. Ensure RTL-compatible formatting. Handle Arabic-specific formatting (numbers, dates, pluralization).

**Acceptance Criteria:**
- [ ] Arabic translation files created
- [ ] All namespaces translated
- [ ] RTL formatting compatible
- [ ] Arabic numbers handled
- [ ] Date/time formatting appropriate
- [ ] Pluralization rules correct
- [ ] Quality reviewed by native speaker
- [ ] Accuracy verified
- [ ] All UI text translated

**Dependencies:** I18N-02  
**Story Points:** 5  
**Priority:** High  
**Components/Files:**  
- `src/locales/ar/common.json`
- `src/locales/ar/auth.json`
- `src/locales/ar/menu.json`
- `src/locales/ar/orders.json`
- `src/locales/ar/reservations.json`
- `src/locales/ar/profile.json`
- `src/locales/ar/settings.json`
- `src/locales/ar/staff.json`

**Related Requirements:** REQ-5.3.1 (Multi-Language Support)

---

### I18N-04: Implement language switching
**Task Name:** Create Language Switching Functionality  
**User Story:** As a user, I want to switch languages easily so I can use the app in my preferred language.

**Description:**  
Implement language selector in settings. Update app language immediately when selected. Persist language preference to AsyncStorage and backend. Reload all displayed text. Handle RTL layout toggle.

**Acceptance Criteria:**
- [ ] Language selector in settings
- [ ] English and Arabic options
- [ ] Language change immediate (no restart)
- [ ] RTL toggle when switching to Arabic
- [ ] All text updates in new language
- [ ] Preference persisted
- [ ] Preference synced to backend
- [ ] Language restored on app launch

**Dependencies:** I18N-01, I18N-02, I18N-03  
**Story Points:** 2  
**Priority:** High  
**Components/Files:**  
- `src/components/settings/LanguageSelector.tsx`
- `src/hooks/useLanguageSwitch.ts`
- `src/stores/usePreferencesStore.ts`

**Related Requirements:** REQ-5.3.1 (Multi-Language Support)

---

### I18N-05: Implement RTL layout support
**Task Name:** Configure Right-to-Left Layout for Arabic  
**User Story:** As an Arabic user, I want the layout mirrored for Arabic so the interface feels natural.

**Description:**  
Implement automatic layout mirroring for Arabic. Use Expo RTL configuration. Mirror all directional elements (buttons, icons, navigation). Flip text alignment. Update NativeWind styling for RTL. Test all screens in RTL mode.

**Acceptance Criteria:**
- [ ] Layout automatically mirrored for Arabic
- [ ] Text right-aligned for Arabic
- [ ] Buttons and controls repositioned
- [ ] Navigation drawer mirrored
- [ ] Icons flipped appropriately
- [ ] Images mirrored where needed
- [ ] No layout overflow
- [ ] All screens support RTL
- [ ] NativeWind styling RTL-compatible

**Dependencies:** I18N-04  
**Story Points:** 4  
**Priority:** High  
**Components/Files:**  
- `src/utils/rtlSupport.ts`
- `src/config/rtlConfig.ts`
- `app/_layout.tsx` (updated)

**Related Requirements:** REQ-5.3.2 (Right-to-Left Layout)

---

### I18N-06: Test RTL rendering
**Task Name:** Verify RTL Layout Rendering Across App  
**User Story:** As a developer, I want to verify RTL rendering works correctly so Arabic users have good experience.

**Description:**  
Test all screens in RTL mode (Arabic). Check text alignment, button positioning, icon orientation. Verify no layout overflow or misalignment. Test with long text strings. Check date/time formatting for Arabic.

**Acceptance Criteria:**
- [ ] All screens tested in RTL mode
- [ ] Text properly right-aligned
- [ ] Components properly mirrored
- [ ] No layout overflow
- [ ] Icons appropriately flipped
- [ ] Date/time formats correct
- [ ] Numbers formatted for Arabic
- [ ] Long text strings handled
- [ ] Images display correctly

**Dependencies:** I18N-05  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- Test reports and documentation

**Related Requirements:** REQ-5.3.2 (Right-to-Left Layout)

---

### I18N-07: Create i18n tests
**Task Name:** Write Internationalization Feature Tests  
**User Story:** As a developer, I want i18n tests written so I ensure translations and RTL work correctly.

**Description:**  
Write tests for language switching. Write tests for translation key resolution. Write tests for RTL layout. Write tests for Arabic date/time formatting. Test that all translation keys exist.

**Acceptance Criteria:**
- [ ] Language switching tests
- [ ] Translation loading tests
- [ ] Missing translation detection
- [ ] RTL layout tests
- [ ] Date/time formatting tests
- [ ] Pluralization tests
- [ ] Test coverage for i18n
- [ ] All tests passing

**Dependencies:** FOUND-06, FOUND-07, I18N-01 through I18N-06  
**Story Points:** 3  
**Priority:** Medium  
**Components/Files:**  
- `src/tests/i18n/i18n.test.ts`
- `src/tests/i18n/rtl.test.ts`

**Related Requirements:** REQ-13 (Testing & Quality)

---

## Phase 12: Offline Support (Tasks OFFLINE-01 to OFFLINE-09)

### OFFLINE-01: Implement local storage service
**Task Name:** Create AsyncStorage Wrapper Service  
**User Story:** As a developer, I want a local storage service so I can persist and retrieve data from device storage.

**Description:**  
Create LocalStorageService wrapper around AsyncStorage. Implement methods: setItem, getItem, removeItem, clear. Implement JSON serialization/deserialization. Add error handling and logging. Create TypeScript types for storage keys.

**Acceptance Criteria:**
- [ ] LocalStorageService created
- [ ] setItem, getItem, removeItem, clear methods
- [ ] JSON serialization working
- [ ] Error handling implemented
- [ ] TypeScript types for keys
- [ ] Storage capacity managed
- [ ] Fallback handling for full storage
- [ ] Logging for debugging

**Dependencies:** INIT-02, FOUND-10  
**Story Points:** 2  
**Priority:** High  
**Components/Files:**  
- `src/services/storage/localStorage.ts`
- `src/types/storage.types.ts`

**Related Requirements:** REQ-5.5 (Offline Support)

---

### OFFLINE-02: Implement sync queue
**Task Name:** Create Offline Action Queue Management  
**User Story:** As a developer, I want a sync queue so offline actions are queued and synced when connection returns.

**Description:**  
Create SyncQueue class to manage offline actions. Store queued actions in localStorage. Implement queue operations: enqueue, dequeue, peek. Assign unique IDs to actions. Track action status (pending, failed, synced).

**Acceptance Criteria:**
- [ ] SyncQueue class created
- [ ] Actions queued with unique IDs
- [ ] Queue persisted to localStorage
- [ ] Enqueue/dequeue operations
- [ ] Status tracking for actions
- [ ] Failed action handling
- [ ] Queue size limited
- [ ] TypeScript types for actions
- [ ] Logging for debugging

**Dependencies:** OFFLINE-01  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/services/sync/syncQueue.ts`
- `src/types/sync.types.ts`

**Related Requirements:** REQ-5.5.2 (Sync Queue Management)

---

### OFFLINE-03: Implement sync service
**Task Name:** Create Synchronization Service  
**User Story:** As a developer, I want a sync service so queued offline actions are synced to the backend when online.

**Description:**  
Create SyncService to process sync queue. Implement periodic sync attempts when online. Process actions in order. Retry failed actions with exponential backoff. Remove successfully synced actions from queue. Handle conflicts and errors.

**Acceptance Criteria:**
- [ ] SyncService processes queue
- [ ] Actions synced in order
- [ ] Retry with exponential backoff
- [ ] Failed action notification
- [ ] Queue validation before sync
- [ ] Partial sync handling
- [ ] Conflict resolution
- [ ] Error logging
- [ ] Successful sync removes from queue

**Dependencies:** OFFLINE-02, FOUND-04  
**Story Points:** 4  
**Priority:** High  
**Components/Files:**  
- `src/services/sync/syncService.ts`
- `src/services/sync/retryStrategy.ts`

**Related Requirements:** REQ-5.5.2 (Sync Queue Management)

---

### OFFLINE-04: Implement network status monitoring
**Task Name:** Create Network Status Detection Service  
**User Story:** As a developer, I want network status monitoring so I know when connection changes.

**Description:**  
Use NetInfo library to monitor network connectivity. Create hook useNetworkStatus for component access. Trigger sync service when connection returns. Update app state with network status. Show offline indicator when disconnected.

**Acceptance Criteria:**
- [ ] NetInfo integrated
- [ ] useNetworkStatus hook created
- [ ] Network state tracked
- [ ] Online/offline detection working
- [ ] Connection type detected (WiFi, cellular, etc.)
- [ ] Sync triggered when online
- [ ] App state updated
- [ ] Persistent network status tracking
- [ ] Error handling for detection

**Dependencies:** FOUND-02, OFFLINE-03  
**Story Points:** 2  
**Priority:** High  
**Components/Files:**  
- `src/services/network/networkStatus.ts`
- `src/hooks/useNetworkStatus.ts`
- `src/stores/useNetworkStore.ts`

**Related Requirements:** REQ-5.5.3 (Network Status Indication)

---

### OFFLINE-05: Implement conflict resolution
**Task Name:** Create Data Conflict Resolution Strategy  
**User Story:** As the system, I want conflict resolution so data inconsistencies are handled gracefully.

**Description:**  
Implement conflict detection for offline changes synced to backend. Create resolution strategies: last-write-wins, server-wins, merge. Handle conflicts when syncing. Notify user of conflicts (if applicable).

**Acceptance Criteria:**
- [ ] Conflict detection implemented
- [ ] Last-write-wins strategy
- [ ] Server-wins strategy
- [ ] Conflict resolution applied
- [ ] User notification for conflicts
- [ ] Conflict logging
- [ ] Data integrity maintained
- [ ] Retry after resolution

**Dependencies:** OFFLINE-03  
**Story Points:** 3  
**Priority:** Medium  
**Components/Files:**  
- `src/services/sync/conflictResolver.ts`
- `src/utils/conflictResolution.ts`

**Related Requirements:** REQ-5.5 (Offline Support)

---

### OFFLINE-06: Implement offline indicators
**Task Name:** Create Offline Status UI Indicators  
**User Story:** As a user, I want to know my network connection status so I understand data limitations.

**Description:**  
Create offline banner/indicator showing "No Internet Connection". Display in persistent location (top of screen). Hide when online. Show sync status (syncing, synced, sync failed). Include retry button for failed syncs.

**Acceptance Criteria:**
- [ ] Offline banner displays when disconnected
- [ ] Banner persists at top of screen
- [ ] "No Internet Connection" message
- [ ] Sync status displayed
- [ ] Last updated timestamp shown
- [ ] Retry button available
- [ ] Graceful animations on show/hide
- [ ] Accessible text for screen readers

**Dependencies:** OFFLINE-04  
**Story Points:** 2  
**Priority:** High  
**Components/Files:**  
- `src/components/network/OfflineBanner.tsx`
- `src/components/network/SyncStatus.tsx`

**Related Requirements:** REQ-5.5.3 (Network Status Indication)

---

### OFFLINE-07: Implement cache management
**Task Name:** Create Cache Size and Expiration Management  
**User Story:** As a developer, I want cache management so offline data doesn't consume excessive storage.

**Description:**  
Implement cache size limits (prevent unbounded growth). Create cache expiration strategy (TTL for different data types). Implement cache cleanup on app launch. Remove old cached data when storage quota exceeded. Log cache statistics.

**Acceptance Criteria:**
- [ ] Cache size limits enforced
- [ ] TTL implemented for data types
- [ ] Expired data removed periodically
- [ ] Storage quota checked
- [ ] Automatic cleanup on quota exceeded
- [ ] Cache statistics logged
- [ ] User configurable cache size (optional)
- [ ] Cache efficiency monitored

**Dependencies:** OFFLINE-01  
**Story Points:** 3  
**Priority:** Medium  
**Components/Files:**  
- `src/services/cache/cacheManager.ts`
- `src/services/cache/cacheConfig.ts`
- `src/utils/cacheCleanup.ts`

**Related Requirements:** REQ-5.1.3 (Memory Usage)

---

### OFFLINE-08: Create offline tests
**Task Name:** Write Offline Functionality Tests  
**User Story:** As a developer, I want offline tests written so I ensure offline features work correctly.

**Description:**  
Write tests for offline detection. Write tests for queue management. Write tests for sync service. Write tests for conflict resolution. Write integration tests for offline workflows.

**Acceptance Criteria:**
- [ ] Network status tests
- [ ] Queue management tests
- [ ] Sync service tests
- [ ] Conflict resolution tests
- [ ] Cache management tests
- [ ] Integration tests for offline flows
- [ ] Error handling tests
- [ ] Test coverage 80%+ for offline
- [ ] All tests passing

**Dependencies:** FOUND-06, FOUND-07, FOUND-08, OFFLINE-01 through OFFLINE-07  
**Story Points:** 4  
**Priority:** High  
**Components/Files:**  
- `src/services/sync/__tests__/syncService.test.ts`
- `src/tests/integration/offline.integration.test.tsx`

**Related Requirements:** REQ-13 (Testing & Quality)

---

### OFFLINE-09: Implement data persistence strategy
**Task Name:** Create Overall Data Persistence Strategy  
**User Story:** As a system, I want a comprehensive data persistence strategy so user data is reliable.

**Description:**  
Document offline-first architecture. Define what data is cached and for how long. Specify sync frequency and retry policies. Define conflict resolution approach. Create recovery procedures for data loss.

**Acceptance Criteria:**
- [ ] Architecture documented
- [ ] Data cache strategy defined
- [ ] Sync frequency specified
- [ ] Retry policy documented
- [ ] Conflict resolution approach
- [ ] Recovery procedures
- [ ] Data consistency guarantees
- [ ] Testing strategy for persistence
- [ ] Performance benchmarks

**Dependencies:** OFFLINE-01 through OFFLINE-08  
**Story Points:** 2  
**Priority:** Medium  
**Components/Files:**  
- `docs/OFFLINE_FIRST_STRATEGY.md`
- `docs/DATA_PERSISTENCE.md`

**Related Requirements:** REQ-5.5 (Offline Support)

---

## Phase 13: Testing & Quality (Tasks TEST-01 to TEST-12)

### TEST-01: Write comprehensive unit tests for services
**Task Name:** Create Service Layer Unit Tests  
**User Story:** As a developer, I want service unit tests written so I ensure service logic works correctly.

**Description:**  
Write unit tests for all service modules: authApi, menuApi, orderApi, reservationApi, etc. Test API call construction, error handling, response transformation. Mock HTTP requests with MSW. Achieve 85%+ coverage for services.

**Acceptance Criteria:**
- [ ] Unit tests for all API services
- [ ] API call construction tested
- [ ] Error scenarios tested
- [ ] Response transformation verified
- [ ] Mock API responses used
- [ ] Edge cases tested
- [ ] Test coverage 85%+ for services
- [ ] All tests passing
- [ ] CI integration working

**Dependencies:** FOUND-06, FOUND-08  
**Story Points:** 5  
**Priority:** High  
**Components/Files:**  
- `src/services/api/__tests__/*.test.ts`
- Coverage reports

**Related Requirements:** REQ-13 (Testing & Quality)

---

### TEST-02: Write integration tests for API flows
**Task Name:** Create API Integration Tests  
**User Story:** As a developer, I want API flow tests written so I ensure end-to-end API interactions work.

**Description:**  
Write integration tests for complete flows: login flow with token refresh, order placement flow, reservation booking flow. Test multiple API calls in sequence. Test error scenarios and recovery.

**Acceptance Criteria:**
- [ ] Auth flow integration tests
- [ ] Menu browsing flow tests
- [ ] Checkout flow tests
- [ ] Order tracking flow tests
- [ ] Reservation flow tests
- [ ] Multiple API calls tested
- [ ] Error recovery tested
- [ ] Test coverage 80%+ for flows
- [ ] All tests passing

**Dependencies:** FOUND-07, FOUND-08  
**Story Points:** 4  
**Priority:** High  
**Components/Files:**  
- `src/tests/integration/*.integration.test.tsx`

**Related Requirements:** REQ-13 (Testing & Quality)

---

### TEST-03: Write component tests
**Task Name:** Create React Component Unit Tests  
**User Story:** As a developer, I want component tests written so I ensure components render and interact correctly.

**Description:**  
Write tests for key components: forms, buttons, cards, modals. Test rendering, user interactions, prop handling. Test accessibility attributes. Mock store and API calls. Achieve 80%+ component coverage.

**Acceptance Criteria:**
- [ ] Component render tests
- [ ] User interaction tests
- [ ] Prop variation tests
- [ ] Error state tests
- [ ] Loading state tests
- [ ] Accessibility tests
- [ ] Test coverage 80%+ for components
- [ ] All tests passing
- [ ] Snapshot tests (where appropriate)

**Dependencies:** FOUND-06  
**Story Points:** 4  
**Priority:** High  
**Components/Files:**  
- `src/components/__tests__/*.test.tsx`

**Related Requirements:** REQ-13 (Testing & Quality)

---

### TEST-04: Write E2E tests for critical flows
**Task Name:** Create End-to-End Tests for Critical User Flows  
**User Story:** As a developer, I want E2E tests written so I ensure critical user workflows work on real devices.

**Description:**  
Write E2E tests using Detox: login flow, menu browsing, add to cart, checkout, order tracking, reservation booking. Test on iOS and Android. Test common error scenarios.

**Acceptance Criteria:**
- [ ] Login E2E test
- [ ] Menu browsing E2E test
- [ ] Checkout E2E test
- [ ] Order tracking E2E test
- [ ] Reservation E2E test
- [ ] iOS E2E tests passing
- [ ] Android E2E tests passing
- [ ] Error scenarios tested
- [ ] CI integration for E2E

**Dependencies:** FOUND-09  
**Story Points:** 5  
**Priority:** High  
**Components/Files:**  
- `e2e/tests/*.e2e.test.ts`

**Related Requirements:** REQ-13 (Testing & Quality)

---

### TEST-05: Implement coverage analysis
**Task Name:** Set Up Code Coverage Reporting  
**User Story:** As a developer, I want coverage analysis so I can track test coverage metrics.

**Description:**  
Set up coverage reporting with Jest. Configure coverage thresholds (80% minimum). Generate coverage reports. Integrate with CI/CD. Track coverage trends.

**Acceptance Criteria:**
- [ ] Coverage reporting configured
- [ ] Coverage reports generated
- [ ] Thresholds enforced (80% minimum)
- [ ] Coverage displayed in CI
- [ ] Coverage trends tracked
- [ ] Uncovered code identified
- [ ] Reports accessible

**Dependencies:** FOUND-06, TEST-01 through TEST-04  
**Story Points:** 2  
**Priority:** High  
**Components/Files:**  
- `jest.config.js` (coverage config)
- Coverage reports

**Related Requirements:** REQ-13 (Testing & Quality)

---

### TEST-06: Add performance testing
**Task Name:** Implement Performance Benchmarking and Testing  
**User Story:** As a developer, I want performance tests so I can ensure app performance meets requirements.

**Description:**  
Create performance tests measuring: app startup time, screen load times, API response times, rendering performance. Set performance baselines. Monitor regressions.

**Acceptance Criteria:**
- [ ] Startup time baseline established
- [ ] Screen load time targets set
- [ ] API performance monitored
- [ ] Rendering performance tested
- [ ] Baselines documented
- [ ] Regression detection enabled
- [ ] Performance reports generated

**Dependencies:** FOUND-06  
**Story Points:** 3  
**Priority:** Medium  
**Components/Files:**  
- `src/tests/performance/*.perf.test.ts`

**Related Requirements:** REQ-5.1 (Performance)

---

### TEST-07: Add accessibility testing
**Task Name:** Implement Accessibility Testing  
**User Story:** As a developer, I want accessibility tests written so I ensure WCAG compliance.

**Description:**  
Write tests for accessibility: screen reader support, color contrast, keyboard navigation, touch targets. Use testing library accessibility queries. Test with screen readers (optional).

**Acceptance Criteria:**
- [ ] Accessibility queries used in tests
- [ ] Screen reader attributes tested
- [ ] Color contrast verified
- [ ] Keyboard navigation tested
- [ ] Touch target sizes verified
- [ ] ARIA labels tested
- [ ] Accessibility tests passing
- [ ] Compliance documentation

**Dependencies:** FOUND-06  
**Story Points:** 3  
**Priority:** Medium  
**Components/Files:**  
- `src/tests/accessibility/*.a11y.test.tsx`

**Related Requirements:** REQ-5.4 (Accessibility)

---

### TEST-08: Add security testing
**Task Name:** Implement Security Testing  
**User Story:** As a developer, I want security tests so I ensure the app follows security best practices.

**Description:**  
Write tests for: token storage security, data encryption, input validation, API security headers, HTTPS enforcement. Test for common vulnerabilities.

**Acceptance Criteria:**
- [ ] Token storage tests
- [ ] Encryption verification
- [ ] Input validation tests
- [ ] SQL injection prevention tested
- [ ] XSS prevention tested
- [ ] HTTPS enforcement verified
- [ ] Security headers tested
- [ ] OWASP compliance checked

**Dependencies:** FOUND-06  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/tests/security/*.security.test.ts`

**Related Requirements:** REQ-5.2 (Security)

---

### TEST-09: Create test documentation
**Task Name:** Write Testing Guidelines and Documentation  
**User Story:** As a developer, I want test documentation so I understand how to write and run tests.

**Description:**  
Document testing strategy and guidelines. Create test writing guides. Document testing commands and CI processes. Create troubleshooting guides for common test issues.

**Acceptance Criteria:**
- [ ] Testing strategy documented
- [ ] Test writing guidelines
- [ ] Commands documented (test, coverage, e2e)
- [ ] CI process documented
- [ ] Troubleshooting guide
- [ ] Examples provided
- [ ] Best practices documented

**Dependencies:** TEST-01 through TEST-08  
**Story Points:** 2  
**Priority:** Medium  
**Components/Files:**  
- `docs/TESTING.md`
- `docs/TEST_WRITING_GUIDE.md`

**Related Requirements:** REQ-13 (Testing & Quality)

---

### TEST-10: Set up CI/CD testing
**Task Name:** Configure Automated Testing in CI/CD Pipeline  
**User Story:** As a developer, I want CI/CD to run tests automatically so I catch issues early.

**Description:**  
Configure GitHub Actions or similar to run unit tests on every pull request. Run integration tests. Run linting. Generate coverage reports. Fail build on test failures. Post results as PR comments.

**Acceptance Criteria:**
- [ ] Unit tests run on PR
- [ ] Integration tests run on PR
- [ ] E2E tests run on PR (iOS and Android)
- [ ] Linting checks enforced
- [ ] Coverage reports generated
- [ ] Build fails on test failure
- [ ] Results posted as PR comment
- [ ] Notifications on failures

**Dependencies:** FOUND-06, TEST-01 through TEST-09  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `.github/workflows/01-unit-tests.yml` (updated)
- `.github/workflows/02-integration-tests.yml` (updated)
- `.github/workflows/03-coverage-analysis.yml` (updated)

**Related Requirements:** REQ-13 (Testing & Quality)

---

### TEST-11: Add smoke tests
**Task Name:** Create Smoke Tests for Critical Paths  
**User Story:** As a developer, I want smoke tests so I can quickly verify basic app functionality.

**Description:**  
Create lightweight smoke tests covering essential functionality: app launches, login works, menu loads, basic order creation. Run smoke tests in CI before full test suite. Fast execution.

**Acceptance Criteria:**
- [ ] App launch smoke test
- [ ] Login smoke test
- [ ] Menu loading smoke test
- [ ] Cart functionality smoke test
- [ ] Checkout smoke test
- [ ] All tests execute in < 5 minutes
- [ ] Quick feedback on basic issues
- [ ] CI runs smoke tests first

**Dependencies:** FOUND-09  
**Story Points:** 2  
**Priority:** Medium  
**Components/Files:**  
- `e2e/tests/smoke.e2e.test.ts`

**Related Requirements:** REQ-13 (Testing & Quality)

---

### TEST-12: Add regression tests
**Task Name:** Create Regression Test Suite  
**User Story:** As a developer, I want regression tests so I catch previously fixed bugs from re-occurring.

**Description:**  
Create and document regression tests for bugs that have been fixed. Add new regression tests as bugs are discovered and fixed. Maintain regression test suite.

**Acceptance Criteria:**
- [ ] Regression test suite created
- [ ] Tests for known issues
- [ ] Tests prevent re-occurrence
- [ ] Suite maintained as bugs fixed
- [ ] CI runs regression tests
- [ ] Regression tests passing
- [ ] Documentation of regressions

**Dependencies:** FOUND-06, FOUND-07  
**Story Points:** 2  
**Priority:** Medium  
**Components/Files:**  
- `src/tests/regression/*.regression.test.tsx`

**Related Requirements:** REQ-13 (Testing & Quality)

---

## Phase 14: Performance & Optimization (Tasks PERF-01 to PERF-09)

### PERF-01: Implement image caching strategy
**Task Name:** Create Image Caching and Optimization  
**User Story:** As a user, I want images to load quickly so I have smooth browsing experience.

**Description:**  
Implement image caching using react-native-cached-image or similar. Cache images locally after first download. Serve cached images on subsequent views. Implement cache expiration. Optimize image formats (WebP for Android, JPEG for iOS).

**Acceptance Criteria:**
- [ ] Image caching implemented
- [ ] First-time download and cache
- [ ] Cached images served immediately
- [ ] Cache expiration after 7 days
- [ ] Image format optimization (WebP, JPEG)
- [ ] Placeholder images while loading
- [ ] Cache size management
- [ ] Loading errors handled gracefully

**Dependencies:** FOUND-03  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/services/cache/imageCacheService.ts`
- `src/components/common/CachedImage.tsx`

**Related Requirements:** REQ-5.1.3 (Memory Usage)

---

### PERF-02: Optimize bundle size
**Task Name:** Reduce Application Bundle Size  
**User Story:** As a user, I want fast app download so I can install quickly.

**Description:**  
Analyze bundle size using bundle analysis tools. Identify large dependencies. Remove unused code. Enable code minification and compression. Measure bundle size impact of new features.

**Acceptance Criteria:**
- [ ] Bundle size analyzed
- [ ] Unused code removed
- [ ] Minification enabled
- [ ] Compression applied
- [ ] Bundle size target < 50MB (iOS), < 60MB (Android)
- [ ] Download time optimized
- [ ] Bundle analysis integrated in CI

**Dependencies:** INIT-05  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- Bundle analysis scripts
- Documentation

**Related Requirements:** REQ-5.1.1 (App Launch Time)

---

### PERF-03: Implement code splitting
**Task Name:** Create Code Splitting for Lazy Loading  
**User Story:** As a user, I want the app to load faster so I start using it sooner.

**Description:**  
Implement code splitting for feature modules. Load only critical code initially. Lazy load screens and features. Implement dynamic imports for heavy components. Measure load time improvements.

**Acceptance Criteria:**
- [ ] Code splitting implemented
- [ ] Initial bundle optimized
- [ ] Lazy loading for screens
- [ ] Dynamic imports for heavy components
- [ ] Load time improvements measured
- [ ] Bundle analysis shows splitting working
- [ ] No UX degradation

**Dependencies:** PERF-02  
**Story Points:** 3  
**Priority:** Medium  
**Components/Files:**  
- Code splitting configuration

**Related Requirements:** REQ-5.1.1 (App Launch Time)

---

### PERF-04: Implement lazy loading
**Task Name:** Create Lazy Loading for Images and Components  
**User Story:** As a user, I want scrolling to be smooth so I experience responsive app.

**Description:**  
Implement lazy loading for images below the fold. Implement virtualization for long lists. Lazy load components when scrolled into view. Use FlatList optimizations.

**Acceptance Criteria:**
- [ ] Images lazy loaded below fold
- [ ] List virtualization implemented
- [ ] Scrolling smooth (60 FPS target)
- [ ] Memory usage optimized
- [ ] Placeholder images shown
- [ ] Image loading doesn't block UI
- [ ] Component lazy loading working

**Dependencies:** PERF-01  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/components/common/LazyImage.tsx`
- `src/components/common/VirtualizedList.tsx`

**Related Requirements:** REQ-5.1.3 (Memory Usage), REQ-5.1.1 (App Launch Time)

---

### PERF-05: Perform performance testing
**Task Name:** Execute Comprehensive Performance Testing  
**User Story:** As a developer, I want performance tested so I ensure app meets performance targets.

**Description:**  
Test app startup time. Test screen load times. Test memory usage during normal usage. Test battery drain. Test on various devices (old and new). Document performance metrics.

**Acceptance Criteria:**
- [ ] Startup time < 2 seconds
- [ ] Screen load time < 1.5 seconds
- [ ] Memory usage < 100MB
- [ ] Smooth scrolling (60 FPS)
- [ ] Battery drain minimized
- [ ] Performance tested on multiple devices
- [ ] Results documented

**Dependencies:** TEST-06, PERF-01 through PERF-04  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- Performance test reports
- Benchmarks

**Related Requirements:** REQ-5.1 (Performance)

---

### PERF-06: Optimize memory usage
**Task Name:** Reduce Memory Consumption  
**User Story:** As a user, I want the app to run smoothly on older devices so I have good experience.

**Description:**  
Profile app memory usage. Identify memory leaks. Optimize object creation and garbage collection. Clean up subscriptions and listeners. Implement memory management best practices.

**Acceptance Criteria:**
- [ ] Memory profiling completed
- [ ] Memory leaks identified and fixed
- [ ] Memory usage < 100MB
- [ ] GC optimization implemented
- [ ] Subscriptions properly cleaned up
- [ ] No crashes on low memory devices
- [ ] Performance stable over time

**Dependencies:** PERF-04  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- Memory profiling reports

**Related Requirements:** REQ-5.1.3 (Memory Usage)

---

### PERF-07: Optimize battery efficiency
**Task Name:** Minimize Battery Drain  
**User Story:** As a user, I want minimal battery drain so I can use the app longer.

**Description:**  
Analyze battery usage. Optimize background activity. Minimize polling frequency. Disable unnecessary sensors. Implement low power mode support. Test battery drain.

**Acceptance Criteria:**
- [ ] Battery usage profiled
- [ ] Background activity minimized
- [ ] Polling intervals optimized (30+ seconds)
- [ ] Location tracking minimal
- [ ] Screen brightness not affected
- [ ] Low power mode supported
- [ ] Battery drain acceptable

**Dependencies:** PERF-04  
**Story Points:** 2  
**Priority:** Medium  
**Components/Files:**  
- Battery optimization documentation

**Related Requirements:** REQ-5.1.4 (Battery Efficiency)

---

### PERF-08: Create performance documentation
**Task Name:** Document Performance Guidelines and Metrics  
**User Story:** As a developer, I want performance documentation so I understand targets and guidelines.

**Description:**  
Document performance targets for app launch, screen load, API calls, memory usage. Create guidelines for writing performant code. Document best practices and anti-patterns.

**Acceptance Criteria:**
- [ ] Performance targets documented
- [ ] Load time targets specified
- [ ] Memory targets specified
- [ ] Best practices documented
- [ ] Anti-patterns documented
- [ ] Optimization guide created
- [ ] Code examples provided

**Dependencies:** PERF-01 through PERF-07  
**Story Points:** 2  
**Priority:** Medium  
**Components/Files:**  
- `docs/PERFORMANCE.md`
- `docs/OPTIMIZATION_GUIDE.md`

**Related Requirements:** REQ-5.1 (Performance)

---

### PERF-09: Implement performance monitoring
**Task Name:** Create Real-Time Performance Monitoring  
**User Story:** As a developer, I want to monitor performance so I can identify regressions in production.

**Description:**  
Integrate performance monitoring service (Firebase Performance, New Relic, Sentry). Monitor app startup time. Monitor screen load times. Monitor API response times. Set performance budgets. Alert on performance degradation.

**Acceptance Criteria:**
- [ ] Performance monitoring integrated
- [ ] App startup time monitored
- [ ] Screen load times monitored
- [ ] API response times tracked
- [ ] Performance dashboard accessible
- [ ] Alerts configured for regressions
- [ ] Production data collected
- [ ] Performance trends analyzed

**Dependencies:** FOUND-11  
**Story Points:** 3  
**Priority:** Medium  
**Components/Files:**  
- `src/services/monitoring/performanceMonitoring.ts`

**Related Requirements:** REQ-5.1 (Performance)

---

## Phase 15: Accessibility (Tasks A11Y-01 to A11Y-08)

### A11Y-01: Implement screen reader support
**Task Name:** Add VoiceOver and TalkBack Support  
**User Story:** As a visually impaired user, I want to use screen readers so I can access the app.

**Description:**  
Add accessibility labels to all interactive elements. Use accessible component hierarchy. Implement proper heading structure. Add test IDs for testing. Test with VoiceOver (iOS) and TalkBack (Android). Fix any issues found.

**Acceptance Criteria:**
- [ ] All interactive elements have labels
- [ ] Buttons describe their action
- [ ] Form fields have associated labels
- [ ] Images have alt text
- [ ] Heading hierarchy logical
- [ ] Screen reader navigation smooth
- [ ] VoiceOver tested on iOS
- [ ] TalkBack tested on Android
- [ ] All critical features accessible

**Dependencies:** FOUND-01  
**Story Points:** 4  
**Priority:** High  
**Components/Files:**  
- Accessibility improvements across all components

**Related Requirements:** REQ-5.4.1 (Screen Reader Support)

---

### A11Y-02: Verify color contrast (WCAG AA)
**Task Name:** Ensure Color Contrast Compliance  
**User Story:** As a user with low vision, I want sufficient color contrast so I can read text easily.

**Description:**  
Verify all text has minimum 4.5:1 contrast ratio for normal text (WCAG AA). Verify 3:1 ratio for large text. Update colors that don't meet standards. Use contrast checking tools. Test with contrast checking apps.

**Acceptance Criteria:**
- [ ] Text contrast ratio ≥ 4.5:1 (normal)
- [ ] Text contrast ratio ≥ 3:1 (large)
- [ ] All screens verified
- [ ] Color contrast tool reports passing
- [ ] No critical contrast failures
- [ ] Dark mode contrast verified
- [ ] Status indicators not color-only

**Dependencies:** FOUND-01  
**Story Points:** 2  
**Priority:** High  
**Components/Files:**  
- Color palette updates
- Accessibility test report

**Related Requirements:** REQ-5.4.2 (Color Contrast)

---

### A11Y-03: Implement keyboard navigation
**Task Name:** Enable Full Keyboard Navigation  
**User Story:** As a user unable to use touch, I want to navigate using keyboard so I can use all features.

**Description:**  
Enable keyboard navigation on iPad and other devices with external keyboards. Implement logical tab order. Add keyboard shortcuts for common actions (Enter to submit, Escape to close). Test keyboard navigation thoroughly.

**Acceptance Criteria:**
- [ ] Tab navigation works logically
- [ ] Focus indicators visible and clear
- [ ] All interactive elements reachable
- [ ] Keyboard shortcuts follow conventions
- [ ] Escape closes dialogs/modals
- [ ] Enter submits forms
- [ ] Tab order tested
- [ ] No keyboard traps

**Dependencies:** FOUND-01  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- Keyboard navigation implementation

**Related Requirements:** REQ-5.4.4 (Keyboard Navigation)

---

### A11Y-04: Implement touch target sizing
**Task Name:** Ensure Adequate Touch Target Sizes  
**User Story:** As a user with motor challenges, I want large touch targets so I can interact easily.

**Description:**  
Verify all interactive elements are at least 48x48 points. Ensure adequate spacing between touch targets to prevent accidental taps. Update components that are too small. Test with accessibility tools.

**Acceptance Criteria:**
- [ ] All buttons 48x48 points minimum
- [ ] All links 48x48 points minimum
- [ ] Form fields adequately sized
- [ ] Spacing between targets sufficient
- [ ] No cramped interfaces
- [ ] Accessibility checker passing
- [ ] Tested on various device sizes

**Dependencies:** FOUND-01  
**Story Points:** 2  
**Priority:** High  
**Components/Files:**  
- Component size updates

**Related Requirements:** REQ-5.4.3 (Touch Target Size)

---

### A11Y-05: Implement accessibility labels
**Task Name:** Add Comprehensive Accessibility Attributes  
**User Story:** As an assistive technology user, I want clear accessibility labels so I understand UI elements.

**Description:**  
Add accessible labels to all components. Add accessibility hints for complex interactions. Add role information for custom components. Test with accessibility inspector tools. Fix any missing or unclear labels.

**Acceptance Criteria:**
- [ ] All elements have accessibility labels
- [ ] Labels are descriptive and clear
- [ ] Hints provided for complex elements
- [ ] Custom components have roles
- [ ] Accessibility hints helpful
- [ ] No generic "Button" labels
- [ ] Inspector tools show no issues

**Dependencies:** A11Y-01  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- Accessibility label improvements

**Related Requirements:** REQ-5.4.1 (Screen Reader Support)

---

### A11Y-06: Add accessibility testing
**Task Name:** Create Accessibility Testing Suite  
**User Story:** As a developer, I want accessibility testing so I catch issues automatically.

**Description:**  
Create automated accessibility tests using accessibility testing library. Test for missing labels, poor contrast, small touch targets. Integrate tests in CI. Run manual accessibility testing with assistive technologies.

**Acceptance Criteria:**
- [ ] Automated a11y tests created
- [ ] Tests for labels, contrast, sizing
- [ ] CI integration working
- [ ] Manual testing with VoiceOver
- [ ] Manual testing with TalkBack
- [ ] Issues fixed based on test results
- [ ] Ongoing testing enabled

**Dependencies:** FOUND-06, A11Y-01 through A11Y-05  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- `src/tests/accessibility/*.a11y.test.tsx`

**Related Requirements:** TEST-07, REQ-5.4

---

### A11Y-07: Create accessibility documentation
**Task Name:** Write Accessibility Guidelines and Documentation  
**User Story:** As a developer, I want accessibility guidelines so I can build accessible features.

**Description:**  
Document accessibility requirements. Create guidelines for component development. Document testing procedures. Create troubleshooting guide. Document WCAG 2.1 AA compliance approach.

**Acceptance Criteria:**
- [ ] Accessibility requirements documented
- [ ] Component development guidelines
- [ ] Testing procedures documented
- [ ] Troubleshooting guide
- [ ] WCAG compliance documented
- [ ] Code examples provided
- [ ] Best practices documented

**Dependencies:** A11Y-01 through A11Y-06  
**Story Points:** 2  
**Priority:** Medium  
**Components/Files:**  
- `docs/ACCESSIBILITY.md`
- `docs/WCAG_COMPLIANCE.md`

**Related Requirements:** REQ-5.4

---

### A11Y-08: Fix accessibility issues
**Task Name:** Address Accessibility Issues Found During Testing  
**User Story:** As an accessible app user, I want all accessibility issues fixed so I can use all features.

**Description:**  
Run comprehensive accessibility audit. Document all issues found. Prioritize and fix issues. Re-test after fixes. Verify WCAG AA compliance. Document resolution approach.

**Acceptance Criteria:**
- [ ] Accessibility audit completed
- [ ] Issues documented and prioritized
- [ ] Critical issues fixed
- [ ] Major issues fixed
- [ ] Re-testing completed
- [ ] WCAG AA compliance verified
- [ ] Zero accessibility errors in audit

**Dependencies:** A11Y-01 through A11Y-07  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- Accessibility audit report
- Issue resolution log

**Related Requirements:** REQ-5.4

---

## Phase 16: Deployment (Tasks DEPLOY-01 to DEPLOY-12)

### DEPLOY-01: Configure EAS Build
**Task Name:** Set Up Expo Application Services Build System  
**User Story:** As a developer, I want EAS Build configured so I can build production releases.

**Description:**  
Create EAS account and configure EAS CLI. Set up EAS Build configuration (eas.json). Configure build profiles for development, preview, production. Test build process.

**Acceptance Criteria:**
- [ ] EAS account created
- [ ] EAS CLI installed and configured
- [ ] eas.json configured
- [ ] Development build working
- [ ] Preview build working
- [ ] Production build profile set up
- [ ] Build process tested

**Dependencies:** INIT-05  
**Story Points:** 2  
**Priority:** Critical  
**Components/Files:**  
- `eas.json`

**Related Requirements:** REQ-16 (Deployment)

---

### DEPLOY-02: Set up iOS code signing
**Task Name:** Configure iOS Certificate and Provisioning Profile Setup  
**User Story:** As a developer, I want iOS code signing configured so I can build for iOS App Store.

**Description:**  
Create Apple Developer account. Generate iOS certificates (distribution and development). Create provisioning profiles. Configure code signing in Xcode. Set up in EAS Build.

**Acceptance Criteria:**
- [ ] Apple Developer account set up
- [ ] Certificates generated
- [ ] Provisioning profiles created
- [ ] Code signing configured
- [ ] EAS Build can sign iOS builds
- [ ] Test build successful
- [ ] TestFlight upload working

**Dependencies:** DEPLOY-01  
**Story Points:** 3  
**Priority:** Critical  
**Components/Files:**  
- iOS code signing configuration

**Related Requirements:** REQ-16 (Deployment)

---

### DEPLOY-03: Set up Android signing
**Task Name:** Configure Android Keystore and Signing Configuration  
**User Story:** As a developer, I want Android signing configured so I can build for Google Play Store.

**Description:**  
Create keystore for Android app signing. Generate signing key. Configure gradle signing. Set up in EAS Build. Test signing process.

**Acceptance Criteria:**
- [ ] Android keystore created
- [ ] Signing key generated
- [ ] Gradle signing configured
- [ ] EAS Build can sign Android builds
- [ ] Test build successful
- [ ] Play Store upload ready
- [ ] Keystore backed up securely

**Dependencies:** DEPLOY-01  
**Story Points:** 3  
**Priority:** Critical  
**Components/Files:**  
- Android keystore configuration

**Related Requirements:** REQ-16 (Deployment)

---

### DEPLOY-04: Configure environment variables
**Task Name:** Set Up Production Environment Configuration  
**User Story:** As a developer, I want environment variables configured so production build uses correct settings.

**Description:**  
Configure production environment variables (API URL, analytics keys, error tracking keys). Set up environment-specific configurations. Ensure secrets are not committed. Document environment setup.

**Acceptance Criteria:**
- [ ] Production API URL configured
- [ ] Analytics production keys set
- [ ] Error tracking production keys set
- [ ] Feature flags configured
- [ ] Environment variables documented
- [ ] Secrets properly managed
- [ ] No secrets in repo

**Dependencies:** INIT-04, DEPLOY-01  
**Story Points:** 2  
**Priority:** Critical  
**Components/Files:**  
- `.env.production`
- Environment configuration documentation

**Related Requirements:** REQ-5.2 (Security)

---

### DEPLOY-05: Build production APK
**Task Name:** Create Production Android APK Build  
**User Story:** As a developer, I want to build production Android APK so I can submit to Google Play Store.

**Description:**  
Create production Android build using EAS. Generate signed APK. Test APK on Android devices. Verify build integrity. Document build process.

**Acceptance Criteria:**
- [ ] Production APK built successfully
- [ ] APK properly signed
- [ ] APK tested on Android devices
- [ ] No test data in production
- [ ] Performance acceptable
- [ ] Size within Google Play limits
- [ ] Build reproducible

**Dependencies:** DEPLOY-01, DEPLOY-03  
**Story Points:** 2  
**Priority:** Critical  
**Components/Files:**  
- Production APK

**Related Requirements:** REQ-16 (Deployment)

---

### DEPLOY-06: Build production IPA
**Task Name:** Create Production iOS IPA Build  
**User Story:** As a developer, I want to build production iOS IPA so I can submit to Apple App Store.

**Description:**  
Create production iOS build using EAS. Generate signed IPA. Test IPA on iOS devices/simulators. Verify build integrity. Document build process.

**Acceptance Criteria:**
- [ ] Production IPA built successfully
- [ ] IPA properly signed
- [ ] IPA tested on iOS devices
- [ ] No test data in production
- [ ] Performance acceptable
- [ ] Size within App Store limits
- [ ] Build reproducible

**Dependencies:** DEPLOY-01, DEPLOY-02  
**Story Points:** 2  
**Priority:** Critical  
**Components/Files:**  
- Production IPA

**Related Requirements:** REQ-16 (Deployment)

---

### DEPLOY-07: Set up app store deployment
**Task Name:** Configure App Store and Google Play Store Submission  
**User Story:** As a product manager, I want app stores configured so we can release the app.

**Description:**  
Create app listings on App Store and Google Play Store. Configure store metadata (description, screenshots, pricing). Set up app store credentials in EAS. Create release process documentation.

**Acceptance Criteria:**
- [ ] App Store listing created
- [ ] Google Play listing created
- [ ] Metadata complete (description, screenshots)
- [ ] Pricing configured
- [ ] Privacy policy linked
- [ ] Terms of service linked
- [ ] Store credentials secure
- [ ] Release process documented

**Dependencies:** DEPLOY-05, DEPLOY-06  
**Story Points:** 3  
**Priority:** High  
**Components/Files:**  
- Store listings
- Release process documentation

**Related Requirements:** REQ-16 (Deployment)

---

### DEPLOY-08: Configure analytics dashboard
**Task Name:** Set Up Analytics Monitoring Dashboard  
**User Story:** As a product manager, I want analytics dashboard so I can track user behavior and app usage.

**Description:**  
Set up analytics dashboard (Firebase, Mixpanel, etc.). Configure key metrics tracking. Create dashboards for important metrics. Set up data retention policies. Test analytics in production.

**Acceptance Criteria:**
- [ ] Analytics dashboard accessible
- [ ] Key metrics tracked
- [ ] User identification working
- [ ] Custom events working
- [ ] Retention policies configured
- [ ] Real-time data viewing
- [ ] Export capabilities

**Dependencies:** FOUND-12, DEPLOY-01  
**Story Points:** 2  
**Priority:** Medium  
**Components/Files:**  
- Analytics configuration

**Related Requirements:** REQ-16 (Deployment)

---

### DEPLOY-09: Set up crash reporting
**Task Name:** Configure Crash Reporting and Error Tracking  
**User Story:** As a developer, I want crash reporting configured so I'm notified of production issues.

**Description:**  
Configure error tracking service (Sentry, Firebase Crashlytics). Set up crash alerts. Configure sourcemap uploads. Test crash reporting. Document troubleshooting procedures.

**Acceptance Criteria:**
- [ ] Crash reporting configured
- [ ] Errors reported to dashboard
- [ ] Alerts configured for critical errors
- [ ] Sourcemaps uploaded
- [ ] Stack traces readable
- [ ] Test crash reports working
- [ ] User identification in errors

**Dependencies:** FOUND-11, DEPLOY-01  
**Story Points:** 2  
**Priority:** High  
**Components/Files:**  
- Crash reporting configuration

**Related Requirements:** REQ-16 (Deployment)

---

### DEPLOY-10: Document deployment process
**Task Name:** Create Comprehensive Deployment Documentation  
**User Story:** As a developer, I want deployment documentation so I can deploy releases consistently.

**Description:**  
Document deployment prerequisites. Document build and release process. Document app store submission process. Create rollback procedures. Create troubleshooting guide.

**Acceptance Criteria:**
- [ ] Deployment prerequisites documented
- [ ] Build process step-by-step
- [ ] Release process documented
- [ ] App store submission process
- [ ] Rollback procedures documented
- [ ] Troubleshooting guide
- [ ] Emergency procedures

**Dependencies:** DEPLOY-01 through DEPLOY-09  
**Story Points:** 2  
**Priority:** High  
**Components/Files:**  
- `docs/DEPLOYMENT.md`
- `docs/RELEASE_PROCESS.md`

**Related Requirements:** REQ-16 (Deployment)

---

### DEPLOY-11: Create release notes
**Task Name:** Document Release Notes and Changelog  
**User Story:** As a user, I want to know what's new so I understand app updates.

**Description:**  
Create release notes for production releases. Document features, bug fixes, improvements. Include version numbers and dates. Create changelog accessible in app. Update app store release notes.

**Acceptance Criteria:**
- [ ] Release notes written for v1.0
- [ ] Features documented
- [ ] Bug fixes documented
- [ ] Known issues listed
- [ ] Changelog in app accessible
- [ ] App store release notes
- [ ] Version history documented

**Dependencies:** DEPLOY-01  
**Story Points:** 2  
**Priority:** Medium  
**Components/Files:**  
- `CHANGELOG.md`
- `docs/RELEASE_NOTES.md`

**Related Requirements:** REQ-16 (Deployment)

---

### DEPLOY-12: Plan post-launch monitoring
**Task Name:** Create Post-Launch Monitoring and Support Plan  
**User Story:** As a development team, I want post-launch monitoring so we can quickly address production issues.

**Description:**  
Create monitoring plan for post-launch period. Define metrics to watch. Create alert thresholds. Plan support response procedures. Schedule monitoring shifts. Create incident response procedures.

**Acceptance Criteria:**
- [ ] Monitoring plan created
- [ ] Key metrics defined
- [ ] Alert thresholds set
- [ ] Support response SLAs defined
- [ ] Incident procedures documented
- [ ] Monitoring schedule created
- [ ] Escalation procedures

**Dependencies:** DEPLOY-08, DEPLOY-09, DEPLOY-10  
**Story Points:** 2  
**Priority:** High  
**Components/Files:**  
- `docs/MONITORING_PLAN.md`
- `docs/INCIDENT_RESPONSE.md`

**Related Requirements:** REQ-16 (Deployment)

---

## Summary

This comprehensive tasks.md file contains **90 detailed implementation tasks** across **16 phases** of the Naar-Noor Mobile Application React Native project:

- **Phase 1: Project Initialization** (6 tasks)
- **Phase 2: Foundation Setup** (12 tasks)
- **Phase 3: Authentication** (12 tasks)
- **Phase 4: Menu & Browsing** (11 tasks)
- **Phase 5: Cart Management** (8 tasks)
- **Phase 6: Checkout & Orders** (11 tasks)
- **Phase 7: Reservations** (10 tasks)
- **Phase 8: Reviews & Ratings** (8 tasks)
- **Phase 9: User Profile** (9 tasks)
- **Phase 10: Staff App** (12 tasks)
- **Phase 11: Internationalization** (7 tasks)
- **Phase 12: Offline Support** (9 tasks)
- **Phase 13: Testing & Quality** (12 tasks)
- **Phase 14: Performance & Optimization** (9 tasks)
- **Phase 15: Accessibility** (8 tasks)
- **Phase 16: Deployment** (12 tasks)

Each task includes:
- Clear task name and ID
- User story context
- Detailed description
- 3-4 specific acceptance criteria
- Dependencies on other tasks
- Story point estimates (1-13 scale)
- Priority level (Critical/High/Medium/Low)
- Component/file names
- Related requirements references

The tasks are properly sequenced with realistic dependencies and story point estimates based on complexity and effort.

---

---

## Notes

### Implementation Guidelines

1. **Phasing Strategy:** Phases should be executed sequentially, though tasks within a phase can be parallelized where dependencies allow.

2. **Story Point Estimation:** Story points use Fibonacci scale (1, 2, 3, 5, 8, 13). Estimates account for:
   - Complexity (new patterns vs. repeated patterns)
   - Uncertainty (research, unknown third-party behavior)
   - Risk (potential roadblocks, integration challenges)

3. **Dependency Management:**
   - Each task clearly lists dependencies
   - Tasks should not begin until dependencies are complete
   - Some "soft" dependencies allow parallel work with careful coordination

4. **Component Organization:**
   - Components follow atomic design: atoms, molecules, organisms
   - Shared components in `src/components/common`
   - Feature-specific components in `src/components/{feature}`
   - Custom hooks co-located with components or in `src/hooks`

5. **Testing Strategy:**
   - Unit tests for utilities and logic (80%+ coverage)
   - Integration tests for user flows (with mock API)
   - E2E tests for critical paths (on real devices)
   - All tests use React Testing Library best practices

6. **Accessibility First:**
   - Implement accessibility from start, not as afterthought
   - Use semantic HTML, ARIA labels, proper heading hierarchy
   - Test with VoiceOver (iOS) and TalkBack (Android)
   - Target WCAG 2.1 AA compliance

7. **Performance Targets:**
   - App launch: < 2 seconds (initial), < 1.5 seconds (subsequent)
   - Screen load: < 1.5 seconds
   - API response: < 3 seconds (expected), < 5 seconds (timeout)
   - Memory: < 100MB during normal usage
   - Bundle size: < 50MB (iOS), < 60MB (Android)

8. **Security Best Practices:**
   - All sensitive data (tokens, addresses, payment info) encrypted
   - HTTPS for all API communication (TLS 1.2+)
   - JWT tokens with appropriate expiration (24 hours access, 7 days refresh)
   - Rate limiting on authentication endpoints
   - Input validation on both client and server

9. **Offline-First Approach:**
   - Design features to work offline first
   - Cache essential data locally
   - Queue operations when offline
   - Sync automatically when connection returns
   - Detect and resolve conflicts gracefully

10. **TypeScript Usage:**
    - Strict mode enabled throughout
    - No `any` types without justification
    - Proper typing for API responses and store state
    - Enum usage for status values (no string literals)
    - Generic types for reusable components and hooks

### Risk Mitigation

1. **Third-party Dependencies:** Vet dependencies for maintenance, security, and platform compatibility before integration.

2. **API Changes:** Maintain version control for API contracts. Create versioned hooks for API integration.

3. **Device Fragmentation:** Test regularly on diverse devices (old/new, various screen sizes).

4. **Performance Regressions:** Monitor bundle size and startup time in CI. Alert on regressions.

5. **Data Loss:** Implement robust error handling and data validation. Test offline sync scenarios thoroughly.

### Continuous Integration Recommendations

- Run unit tests on every commit (< 5 min)
- Run integration tests on PR (< 15 min)
- Run E2E tests on master (< 30 min for iOS + Android)
- Generate coverage reports (target 80%+)
- Run linting and type checks on every commit
- Build artifacts for testing
- Notify team of failures immediately

### Success Criteria for Phase Completion

1. **Phase 1:** Project compiles, runs on both iOS and Android with no TypeScript errors
2. **Phase 2:** Navigation works, state management functional, API calls intercepted and logged
3. **Phase 3:** Users can register, login, logout; tokens stored securely; session management working
4. **Phase 4:** Menu displays all categories and items; search works; caching functional
5. **Phase 5:** Cart works end-to-end; items persist; calculations accurate
6. **Phase 6:** Orders can be placed and tracked; real-time updates working
7. **Phase 7:** Reservations can be booked and modified; availability checked
8. **Phase 8:** Reviews can be submitted; photos upload; ratings display correctly
9. **Phase 9:** User profile editable; addresses and payments manageable
10. **Phase 10:** Staff login works; reservations and orders manageable by staff; RBAC enforced
11. **Phase 11:** App works fully in English and Arabic; RTL layout correct
12. **Phase 12:** Offline features functional; sync queue processes; conflicts resolved
13. **Phase 13:** 80%+ test coverage; critical flows E2E tested; CI/CD working
14. **Phase 14:** Performance targets met; bundle size within limits; no memory leaks
15. **Phase 15:** WCAG AA compliance achieved; VoiceOver/TalkBack functional
16. **Phase 16:** Apps built and submitted to stores; monitoring active; release documented

### Known Constraints

- Expo Go limitations for native modules (address with EAS Build if needed)
- Expo Router still evolving (monitor for breaking changes)
- RTL support may require additional testing for custom components
- Some Expo APIs may not support all iOS/Android capabilities
- Performance on older devices (< 2GB RAM) may be challenging

### Future Enhancements (Out of Scope)

- Offline map features for delivery tracking
- AR menu preview for dishes
- Advanced payment methods (cryptocurrency, Buy Now Pay Later)
- Social features (sharing, ratings, leaderboards)
- Advanced analytics (cohort analysis, funnel tracking)
- AI-powered recommendations
- Voice ordering
- IoT integrations (kitchen display, table management)

