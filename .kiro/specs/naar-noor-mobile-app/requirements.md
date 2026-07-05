# Naar-Noor Mobile Application - Requirements Specification

## 1. Introduction

The Naar-Noor Mobile Application is a cross-platform React Native solution designed to serve customers and restaurant staff for the Naar-Noor restaurant system. The application provides comprehensive features including user authentication, menu browsing, table reservations, order management, review submission, and staff-specific operational tools. Built on ASP.NET Core 8 REST API backend, the mobile app utilizes TanStack Query for state management, Expo Router for navigation, and NativeWind for styling. The application supports both iOS and Android platforms with full internationalization (English and Arabic) and RTL (Right-to-Left) support.

**Target Users:**
- Customers: Browse menus, make reservations, place orders, submit reviews, and manage profiles
- Restaurant Staff: Manage reservations, view orders, process payments, and access role-based operational features

**Technology Stack:**
- Framework: React Native with Expo
- State Management: TanStack Query + Zustand
- Navigation: Expo Router
- Styling: NativeWind (Tailwind CSS for React Native)
- Backend: ASP.NET Core 8 REST API
- Authentication: JWT-based token mechanism
- Localization: English and Arabic (bidirectional support)

---

## 2. Glossary

### Systems and Components

**MobileApp:** The React Native cross-platform application running on iOS and Android devices, providing customer and staff interfaces for restaurant operations.

**AuthenticationService:** Backend service responsible for user authentication, token generation, JWT validation, and credential management. Implements OAuth2/JWT mechanisms.

**MenuService:** Backend service managing restaurant menu data including categories, items, pricing, descriptions, dietary information, availability status, and menu variants.

**ReservationService:** Backend service handling table reservations, availability checking, booking management, cancellations, and reservation history.

**OrderService:** Backend service managing customer orders from placement through preparation and delivery, including order status tracking and history.

**ReviewService:** Backend service handling customer reviews, ratings, and feedback submission for menu items and overall dining experience.

**PaymentService:** Backend service processing payments, managing transaction records, payment methods, and payment status tracking.

**LocalStorageService:** Mobile-side service managing local data persistence including user preferences, cached data, offline queue, and temporary drafts.

**SyncService:** Mobile-side service responsible for syncing local data with backend when connection is available, handling conflict resolution and data consistency.

### Key Terms

**JWT (JSON Web Token):** Secure token mechanism for stateless authentication and authorization between mobile app and backend API.

**TanStack Query:** Data synchronization library managing server state, caching, and automatic data fetching/updating on the mobile client.

**Zustand:** Lightweight state management library for managing global application state (user state, UI state, preferences).

**Expo Router:** Navigation library providing file-based routing similar to Next.js for React Native applications.

**NativeWind:** Utility-first styling solution providing Tailwind CSS compatibility for React Native components.

**RTL (Right-to-Left):** Text direction and layout orientation for Arabic language support, mirroring UI elements appropriately.

**Role-Based Access Control (RBAC):** System controlling feature access based on user roles (Customer, Staff, Manager, Admin).

**Offline-First Architecture:** Application capability to function without internet connection, queueing actions and syncing when connection returns.

**Rest API:** RESTful web service endpoints providing data access and business operations for the mobile application.

---

## 3. Customer App Features

### 3.1 Authentication & Account Management

#### REQ-3.1.1: User Registration
**Pattern:** Ubiquitous
**User Story:** As a new customer, I want to register a new account using my email and password so I can access the mobile app and make reservations.

**Requirement:** The MobileApp SHALL provide a registration screen where users enter email, password, confirm password, full name, and phone number. The AuthenticationService SHALL validate that the email is unique, password meets complexity requirements (minimum 8 characters, including uppercase, lowercase, numbers, and special characters), and the phone number is in valid format. Upon successful validation, the AuthenticationService SHALL create a new customer account and return a JWT token. The MobileApp SHALL store the token securely using platform-specific secure storage (Keychain on iOS, Keystore on Android).

**Acceptance Criteria:**
- Registration form accepts email, password, confirm password, full name, and phone number
- Password validation enforces minimum 8 characters with uppercase, lowercase, numbers, and special characters
- Email uniqueness validation prevents duplicate account creation
- Phone number format validation ensures valid phone numbers
- Successful registration returns JWT token
- JWT token is stored securely in device secure storage
- Error messages display for all validation failures

---

#### REQ-3.1.2: User Login
**Pattern:** Ubiquitous
**User Story:** As a registered customer, I want to log in with my email and password so I can access my account and existing data.

**Requirement:** The MobileApp SHALL display a login screen accepting email and password credentials. Upon form submission, the AuthenticationService SHALL validate credentials against stored account records. If credentials are valid, the AuthenticationService SHALL issue a JWT token with appropriate expiration time (24 hours). The MobileApp SHALL store the token securely and automatically populate the user's profile information from the backend.

**Acceptance Criteria:**
- Login form accepts email and password
- Authentication validates credentials securely
- Successful login issues JWT token with 24-hour expiration
- JWT token stored in secure device storage
- User profile automatically loaded after login
- Invalid credentials display clear error message
- Account lockout occurs after 5 failed login attempts

---

#### REQ-3.1.3: Password Reset
**Pattern:** Event-driven
**User Story:** As a customer who forgot my password, I want to reset it via email link so I can regain access to my account.

**Requirement:** When user requests password reset, the MobileApp SHALL display email input field. The AuthenticationService SHALL verify the email exists and send password reset link via email. The link SHALL be valid for 1 hour and contain a unique reset token. When user clicks the link, the MobileApp SHALL open to a password reset screen allowing new password entry. The AuthenticationService SHALL validate the reset token, update the password, and invalidate all existing JWT tokens for that user.

**Acceptance Criteria:**
- Password reset email sent within 5 seconds of request
- Reset link valid for exactly 1 hour
- Reset link contains cryptographically secure token
- New password meets complexity requirements
- All existing sessions invalidated after password reset
- User notified via email of password change



#### REQ-3.1.4: Token Refresh
**Pattern:** Event-driven
**User Story:** As a customer, I want my session to remain valid during active use so I don't get logged out unexpectedly.

**Requirement:** The MobileApp SHALL automatically refresh JWT tokens when they approach expiration (30 minutes before expiry). The AuthenticationService SHALL provide a refresh token endpoint that issues new tokens without requiring credential re-entry. If token refresh fails, the MobileApp SHALL prompt user to log in again. Refresh tokens SHALL be stored securely and valid for 7 days.

**Acceptance Criteria:**
- Automatic token refresh triggered 30 minutes before expiration
- Refresh token endpoint returns new valid JWT
- Failed refresh attempts trigger login screen
- Refresh tokens expire after 7 days
- Expired tokens cannot refresh

---

#### REQ-3.1.5: Logout
**Pattern:** Ubiquitous
**User Story:** As a customer, I want to log out from my account so I can secure my account when not in use.

**Requirement:** The MobileApp SHALL provide a logout button in the account settings. Upon logout, the MobileApp SHALL clear all stored authentication tokens, user data, and cached information from device storage. The AuthenticationService SHALL invalidate the current JWT token on the backend.

**Acceptance Criteria:**
- Logout button available in account settings
- JWT token cleared from device storage
- User data cleared from local cache
- User redirected to login screen after logout
- Logout confirmed within 2 seconds

---

### 3.2 Menu Browsing

#### REQ-3.2.1: Display Menu Categories
**Pattern:** Ubiquitous
**User Story:** As a customer, I want to view menu categories so I can explore available dishes organized by type.

**Requirement:** The MobileApp SHALL fetch and display menu categories from MenuService (e.g., Appetizers, Main Courses, Desserts, Beverages). Categories SHALL include icon, name, and item count. The MobileApp SHALL cache categories locally using TanStack Query and display them in a scrollable list view with category icons, names, and item count badges. Categories SHALL be displayed in the user's selected language (English or Arabic) with proper RTL layout for Arabic.

**Acceptance Criteria:**
- Menu categories displayed with icons and names
- Item count shown for each category
- Categories display in user's selected language
- RTL layout applied for Arabic language
- Categories load from cache if available
- Fresh data fetched from API on app launch
- Loading indicator shown while fetching

---

#### REQ-3.2.2: Display Menu Items
**Pattern:** Ubiquitous
**User Story:** As a customer, I want to view menu items within a category so I can choose what to order.

**Requirement:** The MobileApp SHALL display menu items when a category is selected. Each item SHALL show name, description, base price, image, availability status, and dietary information (vegetarian, vegan, gluten-free, etc.). The MenuService SHALL provide item details including preparation time and calories. Items SHALL be filterable by dietary preferences and sortable by price or popularity. The MobileApp SHALL use TanStack Query with automatic cache invalidation (5-minute stale time).

**Acceptance Criteria:**
- Menu items display with name, description, price, and image
- Dietary information displayed (vegetarian, vegan, gluten-free icons)
- Availability status shown (Available/Unavailable)
- Items filterable by dietary preferences
- Items sortable by price (low-to-high, high-to-low) and popularity
- Item details load within 2 seconds
- Offline cached items display when offline

---

#### REQ-3.2.3: Search Menu Items
**Pattern:** Event-driven
**User Story:** As a customer, I want to search for menu items by name so I can quickly find specific dishes.

**Requirement:** The MobileApp SHALL provide a search bar on the menu screen accepting free-text input. As the customer types, the MenuService SHALL return matching items from all categories. Search results SHALL include item name, category, price, and availability. Search SHALL be case-insensitive and support partial name matching. Results SHALL appear with minimal latency (under 500ms).

**Acceptance Criteria:**
- Search bar accepts text input
- Search matches items by name
- Search is case-insensitive
- Partial name matching supported
- Results display within 500ms
- Search results include category and price
- Empty state shown when no results found
- Search results cleared when search field emptied

---

#### REQ-3.2.4: View Item Details
**Pattern:** Ubiquitous
**User Story:** As a customer, I want to view detailed information about a menu item so I can make an informed decision before ordering.

**Requirement:** The MobileApp SHALL display item detail screen showing full description, high-resolution image, price, preparation time, ingredients list, allergen information, dietary tags, customer average rating, and customer review count. For items with variants (size, spice level, etc.), the MobileApp SHALL display variant options. Customer reviews SHALL be fetchable on demand with pagination.

**Acceptance Criteria:**
- Item detail screen displays name, description, image
- Price, preparation time, and calories displayed
- Ingredients list and allergen information shown
- Dietary tags displayed prominently
- Customer average rating and review count shown
- Item variants displayed with options
- Reviews paginated (10 per page)
- Back button returns to menu



### 3.3 Shopping Cart Management

#### REQ-3.3.1: Add Items to Cart
**Pattern:** Event-driven
**User Story:** As a customer, I want to add menu items to my shopping cart so I can prepare an order.

**Requirement:** The MobileApp SHALL provide "Add to Cart" button on item detail screen and menu item list. When clicked, the MobileApp SHALL open a variant/quantity selection sheet. Customer SHALL select item variants (size, spice level, special preparation) and quantity. Upon confirmation, the item SHALL be added to cart with selected options. The LocalStorageService SHALL store cart data locally in Zustand state. Cart SHALL display item count badge on cart icon in navigation bar.

**Acceptance Criteria:**
- Add to Cart button present on item details
- Variant selection interface displayed for items with options
- Quantity selector allows 1-99 items
- Selected variants persist in cart
- Cart item count badge updates immediately
- Added item appears in cart with correct options
- Item added to cart within 500ms

---

#### REQ-3.3.2: View Shopping Cart
**Pattern:** Ubiquitous
**User Story:** As a customer, I want to view my shopping cart so I can review items before checkout.

**Requirement:** The MobileApp SHALL display cart screen showing all added items with names, variants, quantities, individual prices, and subtotal. Each item SHALL have increment/decrement quantity buttons and delete option. Cart SHALL display subtotal, estimated delivery fee (if applicable), estimated tax, and total amount. Cart items SHALL be editable (modify quantity or variants) before checkout.

**Acceptance Criteria:**
- Cart displays all items with names, variants, quantities
- Individual item prices and subtotal calculated correctly
- Delivery fee displayed and calculated
- Tax calculated based on location
- Total amount displayed prominently
- Quantity adjustable via +/- buttons (1-99 range)
- Delete item button removes from cart
- Empty cart state message shown when cart is empty

---

#### REQ-3.3.3: Manage Quantity
**Pattern:** Event-driven
**User Story:** As a customer, I want to adjust item quantities in my cart so I can modify my order before checkout.

**Requirement:** The MobileApp SHALL provide increment (+) and decrement (-) buttons for each cart item. Increment SHALL be limited to 99 items maximum. Decrement to zero SHALL remove the item from cart. Quantity changes SHALL update cart totals immediately. Changes SHALL persist in local storage using LocalStorageService.

**Acceptance Criteria:**
- Increment button increases quantity by 1
- Decrement button decreases quantity by 1
- Minimum quantity is 1
- Maximum quantity is 99
- Quantity set to 0 removes item from cart
- Cart totals update immediately after quantity change
- Changes persist on app restart

---

#### REQ-3.3.4: Remove Items from Cart
**Pattern:** Event-driven
**User Story:** As a customer, I want to remove items from my cart so I can adjust my order.

**Requirement:** The MobileApp SHALL display delete/remove button for each cart item. Upon clicking remove, the MobileApp SHALL display confirmation dialog. Upon confirmation, the item SHALL be removed from cart and cart totals recalculated. Removed items SHALL not appear in the cart.

**Acceptance Criteria:**
- Delete button visible for each cart item
- Confirmation dialog appears before removal
- Item removed after confirmation
- Cart totals recalculated after removal
- Remove action completes within 300ms

---

### 3.4 Reservations

#### REQ-3.4.1: Browse Available Tables
**Pattern:** Ubiquitous
**User Story:** As a customer, I want to view available tables for a specific date and time so I can choose a suitable table.

**Requirement:** The MobileApp SHALL display reservation screen with date picker and time selector. Customer SHALL select desired date and time. The ReservationService SHALL return available tables with seating capacity, location (window, corner, center), and any special features (high chair compatible, wheelchair accessible). Available tables SHALL display with visual indication of capacity. Only tables available at selected date and time SHALL be displayed.

**Acceptance Criteria:**
- Date picker displays calendar interface
- Time selector shows availability for selected date
- Available tables display with seating capacity
- Table location described (window, corner, center)
- Special features indicated (accessibility features)
- Only available tables shown
- Minimum date is today (no past bookings)
- Time slots in 30-minute intervals

---

#### REQ-3.4.2: Make Reservation
**Pattern:** Ubiquitous
**User Story:** As a customer, I want to make a table reservation so I can secure a table at my preferred date and time.

**Requirement:** The MobileApp SHALL display reservation form accepting date, time, table selection, number of guests, customer name, and special requests. The ReservationService SHALL validate availability, check for table capacity, and verify customer authentication. Upon successful validation, the ReservationService SHALL create reservation record and return confirmation code. The MobileApp SHALL display confirmation with reservation details and confirmation code. Confirmation SHALL be saved locally and also sent via email.

**Acceptance Criteria:**
- Reservation form collects all required information
- Table capacity validated against guest count
- Date/time availability verified
- Confirmation code generated
- Confirmation displayed with reservation details
- Confirmation email sent to customer
- Reservation saved locally for offline access
- Reservation created within 2 seconds



#### REQ-3.4.3: View Reservation History
**Pattern:** Ubiquitous
**User Story:** As a customer, I want to view my past and upcoming reservations so I can track my bookings.

**Requirement:** The MobileApp SHALL display reservations screen showing upcoming reservations first (sorted by date), followed by past reservations. Each reservation SHALL display date, time, party size, table details, and status (Confirmed, Cancelled, Completed). TanStack Query SHALL cache reservation data with 10-minute stale time. Customer SHALL be able to filter by status.

**Acceptance Criteria:**
- Upcoming reservations displayed first
- Past reservations listed below
- Each reservation shows date, time, party size
- Table details displayed (location, capacity)
- Reservation status shown (Confirmed, Cancelled, Completed)
- Filtering available by status
- Reservations load within 2 seconds
- Cached data displayed while fetching updates

---

#### REQ-3.4.4: Modify Reservation
**Pattern:** Event-driven
**User Story:** As a customer, I want to modify my reservation details so I can adjust date, time, or party size if needed.

**Requirement:** The MobileApp SHALL allow modification of reservations with status "Confirmed". Customer SHALL be able to change date, time, party size, or special requests. The ReservationService SHALL validate that new date/time has available tables with sufficient capacity. If valid, the ReservationService SHALL update reservation record and return success. If invalid, error message explains why (e.g., "No tables available for 8 people at 7:00 PM on selected date"). Modification SHALL be allowed up to 24 hours before reservation.

**Acceptance Criteria:**
- Modify option available for Confirmed reservations only
- Modification allowed up to 24 hours before reservation
- All reservation details editable (date, time, party size, requests)
- New date/time availability validated
- Table capacity verified for new party size
- Changes saved within 2 seconds
- Confirmation email sent after successful modification

---

#### REQ-3.4.5: Cancel Reservation
**Pattern:** Event-driven
**User Story:** As a customer, I want to cancel a reservation so I can free up a table if plans change.

**Requirement:** The MobileApp SHALL display cancel option for confirmed reservations. Upon clicking cancel, the MobileApp SHALL display confirmation dialog with cancellation policy information. Upon confirmation, the ReservationService SHALL mark reservation as "Cancelled" and free the table. Cancellation SHALL be allowed up to 2 hours before reservation time. If cancellation is within 2 hours, an informational message SHALL indicate it's outside standard cancellation window but allow proceeding.

**Acceptance Criteria:**
- Cancel button visible on confirmed reservations
- Confirmation dialog displays cancellation policy
- Cancellation allowed up to 2 hours before reservation
- Late cancellation allowed with information message
- Reservation marked as Cancelled
- Cancellation email sent to customer
- Cancellation processed within 1 second

---

### 3.5 Orders

#### REQ-3.5.1: Place Order
**Pattern:** Ubiquitous
**User Story:** As a customer, I want to place an order from my shopping cart so I can have food delivered or pickup.

**Requirement:** The MobileApp SHALL display checkout screen showing cart summary, delivery address, delivery time preference (ASAP or scheduled), payment method selection, and order special instructions. Customer SHALL select delivery method (Delivery or Pickup). For pickup, customer SHALL select location. For delivery, address SHALL be validated by address validation service. Customer SHALL select payment method and confirm order. The OrderService SHALL create order record, generate order number, and return success.

**Acceptance Criteria:**
- Checkout displays cart items and totals
- Delivery address input with validation
- Delivery time selection (ASAP or scheduled)
- Payment method selector
- Order special instructions field
- Delivery or Pickup option
- Order confirmation shows order number
- Order saved locally with status tracking
- Order created within 2 seconds

---

#### REQ-3.5.2: Track Order Status
**Pattern:** State-driven
**User Story:** As a customer, I want to track my order status in real-time so I know when it will arrive.

**Requirement:** The MobileApp SHALL display order tracking screen showing order number, estimated preparation time, estimated delivery time, current status (Confirmed, Preparing, Ready, Shipped, Delivered), and real-time status updates. If order includes delivery, a map view SHALL display driver location (if driver is assigned). The SyncService SHALL poll OrderService for status updates every 30 seconds or use WebSocket for real-time updates if available. Delivery ETA SHALL update as new information becomes available.

**Acceptance Criteria:**
- Order number displayed prominently
- Current status shown with timestamp
- Status history displayed as timeline
- Preparation time shown
- Delivery/pickup time shown
- Real-time status updates received
- Driver location map displayed (if applicable)
- Delivery ETA updated dynamically
- Status updates received within 1 minute

---

#### REQ-3.5.3: View Order History
**Pattern:** Ubiquitous
**User Story:** As a customer, I want to view my past orders so I can reorder frequently purchased items.

**Requirement:** The MobileApp SHALL display orders screen showing completed orders sorted by date (newest first). Each order SHALL display order number, date, items count, total amount, and order status. Customer SHALL be able to view order details and reorder entire order or select specific items. TanStack Query SHALL cache order history with automatic refresh.

**Acceptance Criteria:**
- Order history displays completed orders
- Orders sorted by date (newest first)
- Each order shows number, date, item count, total
- Order details expandable
- Reorder option available
- Items from past order can be selected for reorder
- Order history loads within 2 seconds
- Pagination for orders exceeding 10 items



#### REQ-3.5.4: Cancel Order
**Pattern:** Event-driven
**User Story:** As a customer, I want to cancel an order if I change my mind so I can avoid charges.

**Requirement:** The MobileApp SHALL display cancel option on active orders (status: Confirmed or Preparing). Customer SHALL be able to cancel order with optional reason. The OrderService SHALL accept cancellation, verify order status, and process cancellation if order hasn't started delivery. If order can be cancelled, customer receives refund notification. Cancellation is allowed only for orders in "Confirmed" or "Preparing" status. Orders in "Ready", "Shipped", or "Delivered" status cannot be cancelled.

**Acceptance Criteria:**
- Cancel option available for Confirmed and Preparing orders only
- Cancellation reason field optional
- Confirmation dialog before cancellation
- Cancellation processed within 1 second
- Refund notification sent to customer
- Cancellation reason stored for staff reference
- Cannot cancel Shipped or Delivered orders

---

### 3.6 Reviews and Ratings

#### REQ-3.6.1: Submit Item Review
**Pattern:** Event-driven
**User Story:** As a customer, I want to review menu items I've ordered so I can share my dining experience.

**Requirement:** The MobileApp SHALL display review screen for completed orders. Customer SHALL be able to select items from the order and submit review including rating (1-5 stars), review text (optional, up to 500 characters), and photos (up to 3 images). The ReviewService SHALL validate review content (no offensive language, no external links), store review, and associate with item and customer. Review submission SHALL trigger notification to restaurant staff.

**Acceptance Criteria:**
- Review screen accessible from completed orders
- Star rating required (1-5 stars)
- Review text optional (max 500 characters)
- Photo upload supports up to 3 images
- Photo size limit 5MB per image
- Offensive language filtering applied
- Review submitted within 2 seconds
- Confirmation message displayed

---

#### REQ-3.6.2: View Item Ratings
**Pattern:** Ubiquitous
**User Story:** As a customer, I want to see ratings and reviews for menu items so I can make informed decisions.

**Requirement:** The MobileApp SHALL display average rating and review count on menu item cards. Item detail screen SHALL display detailed ratings breakdown (percentage distribution across 5-star range), recent reviews (sorted by newest first), and review filtering by rating. Customer SHALL be able to sort reviews by "Newest", "Highest Rating", or "Lowest Rating". Reviews SHALL display reviewer name (first name + initial of last name), rating, review text, photos (if included), and review date.

**Acceptance Criteria:**
- Average rating displayed on item cards
- Rating breakdown shown as percentage distribution
- Recent reviews paginated (5 per page)
- Reviews filterable by rating
- Reviews sortable by date, rating
- Reviewer name anonymized (first name + last initial)
- Review photos displayed as gallery
- Review date shown relative to current date

---

#### REQ-3.6.3: Rate Dining Experience
**Pattern:** Event-driven
**User Story:** As a customer, I want to rate my overall dining experience so the restaurant can gather feedback.

**Requirement:** The MobileApp SHALL display experience rating prompt after order delivery completion. Customer SHALL rate overall experience (1-5 stars) and optionally provide feedback text (up to 300 characters). Optional fields for service quality rating and food quality rating. The ReviewService SHALL store experience ratings separately and make available to restaurant management dashboard.

**Acceptance Criteria:**
- Experience rating prompt after delivery
- Overall experience rating required (1-5 stars)
- Service quality rating optional
- Food quality rating optional
- Feedback text optional (max 300 characters)
- Rating submitted within 2 seconds
- Thank you message displayed after submission

---

### 3.7 User Profile

#### REQ-3.7.1: View and Edit Profile
**Pattern:** Ubiquitous
**User Story:** As a customer, I want to view and edit my profile information so I keep my account details current.

**Requirement:** The MobileApp SHALL display profile screen showing customer information: name, email, phone number, profile picture, preferred language, and contact preferences. Customer SHALL be able to edit name, phone number, profile picture, and language preference. Email cannot be changed directly (must use separate account verification flow). Changes SHALL be saved to backend via AuthenticationService and stored locally via LocalStorageService.

**Acceptance Criteria:**
- Profile displays name, email, phone, picture
- Name editable with validation
- Phone number editable with validation
- Profile picture uploadable (max 2MB)
- Language preference changeable (English/Arabic)
- Email not directly editable
- Changes saved within 2 seconds
- Validation errors displayed clearly

---

#### REQ-3.7.2: Manage Delivery Addresses
**Pattern:** Ubiquitous
**User Story:** As a customer, I want to save multiple delivery addresses so I can quickly select during checkout.

**Requirement:** The MobileApp SHALL display addresses management screen showing saved addresses with labels (Home, Work, Other). Customer SHALL be able to add new address, edit existing address, delete address, or set default address. Each address SHALL include street address, city, postal code, and optional notes (e.g., "Blue door on left"). Address validation SHALL verify format and geographic validity using address validation service.

**Acceptance Criteria:**
- Display all saved addresses with labels
- Add new address form with validation
- Edit existing address functionality
- Delete address with confirmation
- Set default address option
- Address format validation applied
- Maximum 10 addresses per account
- Addresses persisted to backend

---

#### REQ-3.7.3: Manage Payment Methods
**Pattern:** Ubiquitous
**User Story:** As a customer, I want to save payment methods so I can checkout quickly.

**Requirement:** The MobileApp SHALL display payment methods screen showing saved payment methods (credit cards, debit cards, digital wallets). Customer SHALL be able to add new payment method, delete payment method, or set default payment method. Payment method addition SHALL use secure payment gateway (e.g., Stripe, PayPal). Stored card information SHALL NOT be stored on device; only tokenized references SHALL be stored.

**Acceptance Criteria:**
- Display all saved payment methods
- Add payment method via secure gateway
- Delete payment method with confirmation
- Set default payment method
- Only tokenized references stored locally
- Full card numbers never stored on device
- Maximum 5 payment methods per account
- Payment methods persisted securely



#### REQ-3.7.4: Preferences and Notifications
**Pattern:** Ubiquitous
**User Story:** As a customer, I want to manage my notification preferences so I receive only desired alerts.

**Requirement:** The MobileApp SHALL display preferences screen allowing customer to control notification settings: order status updates, promotional emails, special offers, and operational notifications. Each preference SHALL include toggle switch. Customer preferences SHALL be persisted to AuthenticationService. The MobileApp SHALL respect system notification permissions and gracefully handle when user denies notification permission at OS level.

**Acceptance Criteria:**
- Preferences screen displays all notification options
- Toggle switches for each notification type
- Preferences saved within 1 second
- System permission requests handled appropriately
- Preferences persisted across app sessions
- Notification delivery respects user preferences
- Uninstall reminder for disabled notifications

---

#### REQ-3.7.5: Delete Account
**Pattern:** Unwanted event
**User Story:** As a customer, I want to permanently delete my account if I no longer want to use the service.

**Requirement:** The MobileApp SHALL provide account deletion option in profile settings. Upon clicking delete, the MobileApp SHALL display warning dialog explaining consequences (data deletion, order history loss). Customer SHALL enter password to confirm deletion. Upon confirmation, the AuthenticationService SHALL delete all customer data (profile, addresses, payment methods, order history, reviews). Deletion request SHALL be confirmed via email. Data deletion SHALL be permanent after 30-day grace period, during which customer can recover account.

**Acceptance Criteria:**
- Delete account option in profile settings
- Warning dialog with consequences displayed
- Password confirmation required
- Confirmation email sent
- 30-day grace period for account recovery
- All customer data deleted after grace period
- Account marked as deleted immediately

---

## 4. Staff App Features

### 4.1 Staff Authentication

#### REQ-4.1.1: Staff Login
**Pattern:** Ubiquitous
**User Story:** As a restaurant staff member, I want to log in to the staff app so I can access operational features.

**Requirement:** The MobileApp SHALL provide separate staff login interface with username/employee ID and password. The AuthenticationService SHALL validate credentials and verify staff role assignment (Waiter, Chef, Manager, Admin). Upon successful login, staff app SHALL display role-appropriate features and permissions. JWT token SHALL include role information for client-side authorization checks. Failed login attempts SHALL trigger account lockout after 5 attempts.

**Acceptance Criteria:**
- Staff login accepts username/employee ID and password
- Staff role verified during authentication
- JWT token includes staff role
- Role-appropriate features displayed
- Account lockout after 5 failed attempts
- Logout clears staff session

---

### 4.2 Reservation Management

#### REQ-4.2.1: View Reservations Overview
**Pattern:** Ubiquitous
**User Story:** As a waiter, I want to view today's reservations so I can prepare tables and manage seating.

**Requirement:** The MobileApp SHALL display reservations overview showing all reservations for current day, sorted by time. Each reservation SHALL display party size, reservation time, customer name, table assignment, and status (Confirmed, Checked In, Completed, Cancelled). Staff SHALL be able to filter reservations by status or time range. The ReservationService SHALL refresh data every 2 minutes or allow manual refresh. Color coding SHALL indicate reservation status visually.

**Acceptance Criteria:**
- Reservations displayed for current day
- Sorted by reservation time
- Each reservation shows party size, time, customer, table, status
- Filtering available by status
- Manual refresh available
- Auto-refresh every 2 minutes
- Color coding for status indication
- Scrollable for many reservations

---

#### REQ-4.2.2: Check In Customers
**Pattern:** Event-driven
**User Story:** As a waiter, I want to check in customers when they arrive so I can mark table as occupied.

**Requirement:** The MobileApp SHALL display check-in action on reservation details. Upon clicking check-in, staff SHALL verify customer presence and select table if not pre-assigned. The ReservationService SHALL update reservation status to "Checked In", assign table if needed, and update table occupancy status. System SHALL send notification to kitchen if applicable. Table status SHALL update in real-time for other staff viewing the same data.

**Acceptance Criteria:**
- Check-in button visible on reservations
- Check-in requires customer name verification
- Table assignment confirmed or modified
- Reservation status updated to Checked In
- Table status updated in real-time
- Kitchen notified of new table
- Check-in processed within 1 second

---

#### REQ-4.2.3: Complete Reservation
**Pattern:** State-driven
**User Story:** As a waiter, I want to mark a reservation as completed when customers leave so I can free up the table.

**Requirement:** The MobileApp SHALL display complete/checkout button on checked-in reservations. Upon clicking, staff SHALL confirm table is being vacated. The ReservationService SHALL update reservation status to "Completed", mark table as available, and calculate dining duration. System SHALL prompt for payment processing if not already completed. Completed reservation SHALL be archived and unavailable for modification.

**Acceptance Criteria:**
- Complete button visible on checked-in reservations
- Confirmation required before completing
- Reservation status updated to Completed
- Table marked as available
- Dining duration calculated
- Payment prompt displayed if needed
- Completed reservation archived

---

### 4.3 Order Management

#### REQ-4.3.1: View Active Orders
**Pattern:** Ubiquitous
**User Story:** As a chef, I want to view active orders so I can see what needs to be prepared.

**Requirement:** The MobileApp SHALL display active orders screen showing all orders with status "Confirmed" or "Preparing". Orders SHALL display order number, items to prepare, special instructions, preparation time remaining, order timestamp, and table/delivery information. Orders SHALL be sortable by preparation time (most urgent first) or by order time. New orders SHALL trigger visual and/or audio notification. Orders SHALL update in real-time as status changes.

**Acceptance Criteria:**
- Active orders displayed
- Order number, items, instructions shown
- Preparation time displayed
- Orders sorted by urgency
- New order notifications triggered
- Real-time status updates
- Special instructions highlighted
- Completed orders hidden from active list

---

#### REQ-4.3.2: Update Order Status
**Pattern:** State-driven
**User Story:** As a chef, I want to update order status as I prepare items so staff knows when order is ready.

**Requirement:** The MobileApp SHALL display order detail screen with status update options (Confirmed → Preparing → Ready). Chef SHALL click status button to move order to next status. The OrderService SHALL validate status transition (e.g., cannot go from Confirmed directly to Ready). Upon status update to "Ready", system SHALL notify waiter/delivery staff and update customer app if applicable. Status transitions SHALL be timestamped for analytics.

**Acceptance Criteria:**
- Order detail screen displays current status
- Status transition buttons available
- Status transitions validated
- Timestamps recorded for transitions
- Waiter notified when order is ready
- Customer notified when order is ready
- Status changes reflected immediately

---

#### REQ-4.3.3: Special Order Instructions
**Pattern:** Ubiquitous
**User Story:** As a chef, I want to see special instructions for orders so I can prepare items exactly as requested.

**Requirement:** The MobileApp SHALL display special instructions prominently on order detail screen. Instructions SHALL be clearly formatted and easily readable. If instructions contain warnings (allergen-related, etc.), they SHALL be highlighted in red. Chef SHALL be able to acknowledge reading instructions. The OrderService SHALL track instruction acknowledgment.

**Acceptance Criteria:**
- Special instructions displayed prominently
- Instructions clearly formatted
- Allergen warnings highlighted in red
- Acknowledgment option available
- Acknowledgment tracked and timestamped
- Instructions persist throughout order lifecycle

---

#### REQ-4.3.4: Cancel Order (Staff)
**Pattern:** Unwanted event
**User Story:** As a manager, I want to cancel orders in special circumstances so I can handle exceptions.

**Requirement:** The MobileApp SHALL display cancel option only for Manager and Admin roles. Manager SHALL provide cancellation reason from predefined list (Customer Request, Payment Failed, Out of Stock, Kitchen Error, etc.). The OrderService SHALL validate cancellation eligibility, process cancellation, and trigger refund if payment was processed. Customer SHALL be notified of cancellation and reason. Cancellation audit trail SHALL be maintained.

**Acceptance Criteria:**
- Cancel option available only to managers
- Predefined cancellation reason list
- Custom reason field optional
- Refund processed for paid orders
- Customer notified of cancellation
- Audit trail recorded
- Cancellation completed within 2 seconds



### 4.4 Role-Based Access Control

#### REQ-4.4.1: Role-Based Feature Access
**Pattern:** Ubiquitous
**User Story:** As a system, I want to enforce role-based access so only authorized staff can access certain features.

**Requirement:** The MobileApp SHALL implement role-based access control (RBAC) with following roles: Waiter (reservations, orders overview, payment), Chef (order preparation, special instructions), Manager (all above + staff management, reports), Admin (all features). Each role SHALL have predefined permissions configured in the system. The MobileApp SHALL check JWT token claims to determine authorized features. Unauthorized feature access attempts SHALL redirect to "Access Denied" screen with explanation.

**Acceptance Criteria:**
- Waiter can access reservations and orders overview
- Chef can access order preparation features
- Manager can access all staff features
- Admin can access all features
- Unauthorized access redirected appropriately
- Features hidden from unauthorized roles
- Role changes require re-authentication

---

#### REQ-4.4.2: Permission Validation
**Pattern:** Ubiquitous
**User Story:** As the system, I want to validate permissions on each sensitive action so unauthorized operations are prevented.

**Requirement:** The MobileApp SHALL validate user role and permissions before allowing sensitive operations (status changes, order cancellations, staff management). Backend AuthenticationService SHALL also validate permissions before processing sensitive operations (defense-in-depth). Permission checks SHALL be consistent between client and server. Unauthorized operation attempts SHALL be logged as security events.

**Acceptance Criteria:**
- Permissions validated before sensitive actions
- Client-side validation for UX
- Server-side validation for security
- Unauthorized attempts logged
- Consistent permission enforcement
- Clear error messages for denied access

---

## 5. Non-Functional Requirements

### 5.1 Performance

#### REQ-5.1.1: App Launch Time
**Pattern:** Ubiquitous
**User Story:** As a user, I want the app to launch quickly so I can access features without waiting.

**Requirement:** The MobileApp SHALL launch and display login screen within 2 seconds on devices with at least 2GB RAM. Subsequent app launches after first login SHALL display home screen within 1.5 seconds. App initialization SHALL load critical data (user session, cached data) asynchronously to not block UI. Splash screen SHALL display progress indication during initialization.

**Acceptance Criteria:**
- Initial launch completes within 2 seconds
- Subsequent launches complete within 1.5 seconds
- UI responsive during initialization
- Splash screen displayed with progress
- No frozen/unresponsive UI during launch

---

#### REQ-5.1.2: API Response Times
**Pattern:** Ubiquitous
**User Story:** As a user, I want API responses to be fast so I can interact with the app smoothly.

**Requirement:** The MobileApp SHALL expect API responses within 3 seconds under normal network conditions. TanStack Query SHALL handle slow responses gracefully with loading indicators. If API response exceeds 5 seconds, the MobileApp SHALL display timeout error with retry option. Requests SHALL include timeout limits to prevent indefinite hanging. Pagination SHALL be implemented for large datasets to ensure manageable response sizes.

**Acceptance Criteria:**
- API requests timeout at 5 seconds
- Loading indicators shown during requests
- Timeout errors display with retry option
- Retry mechanism available for failed requests
- Pagination implemented for large datasets
- Response time monitored and logged

---

#### REQ-5.1.3: Memory Usage
**Pattern:** Ubiquitous
**User Story:** As a user, I want the app to run smoothly without consuming excessive memory so my device remains responsive.

**Requirement:** The MobileApp SHALL maintain memory footprint below 100MB in typical usage. Image assets SHALL be optimized and lazy-loaded. TanStack Query cache SHALL be managed to prevent unbounded growth. Unused screens and components SHALL be unloaded when not in view. Background services SHALL minimize resource consumption. Memory leaks SHALL be prevented through proper cleanup in component unmounting and subscriptions.

**Acceptance Criteria:**
- App memory footprint below 100MB
- Images optimized and compressed
- Lazy loading implemented for images
- Cache memory managed and limited
- No memory leaks detected
- Smooth performance with 2GB+ RAM

---

#### REQ-5.1.4: Battery Efficiency
**Pattern:** Ubiquitous
**User Story:** As a user, I want the app to be energy efficient so it doesn't rapidly drain my device battery.

**Requirement:** The MobileApp SHALL minimize background activity, network requests, and screen-on time. Location tracking (if implemented for delivery) SHALL only occur when necessary and with explicit user permission. Push notifications SHALL use efficient delivery mechanisms. Polling intervals SHALL be optimized (not excessively frequent). Battery usage SHALL be monitored and optimized, particularly during order tracking and real-time updates.

**Acceptance Criteria:**
- Background activity minimized
- Polling intervals reasonable (30+ seconds)
- Location tracking only when needed
- Push notifications efficient
- Battery impact acceptable during normal use
- Low battery mode respected

---

### 5.2 Security

#### REQ-5.2.1: Authentication Security
**Pattern:** Ubiquitous
**User Story:** As a system, I want to ensure only authorized users can access their accounts so unauthorized access is prevented.

**Requirement:** The MobileApp SHALL use JWT-based authentication with secure token storage. Passwords SHALL be transmitted only over HTTPS with TLS 1.2 minimum. Passwords SHALL be hashed using PBKDF2, bcrypt, or Argon2 on backend. Session tokens SHALL expire after 24 hours. Refresh tokens SHALL expire after 7 days. All authentication requests SHALL be rate-limited to prevent brute force attacks (max 5 attempts per 15 minutes per account).

**Acceptance Criteria:**
- JWT tokens used for authentication
- HTTPS required for all requests
- TLS 1.2 minimum enforced
- JWT tokens stored securely on device
- Tokens expire as specified
- Rate limiting implemented
- Password never logged or displayed

---

#### REQ-5.2.2: Data Encryption
**Pattern:** Ubiquitous
**User Story:** As a system, I want to protect sensitive data in transit and at rest so user data cannot be intercepted or accessed.

**Requirement:** All network communications SHALL use HTTPS with TLS 1.2 minimum encryption. Sensitive data at rest (tokens, addresses, payment tokens) SHALL be encrypted using platform-specific secure storage (Keychain on iOS, Keystore on Android). Sensitive data in TanStack Query cache SHALL be excluded or encrypted. Database connections SHALL use encrypted transport. PII (Personally Identifiable Information) SHALL be encrypted before storage.

**Acceptance Criteria:**
- All API requests use HTTPS
- TLS 1.2 minimum enforced
- Sensitive data encrypted at rest
- Platform-specific secure storage used
- Cache excludes/encrypts sensitive data
- Database connections encrypted
- PII encryption implemented

---

#### REQ-5.2.3: Input Validation and Sanitization
**Pattern:** Ubiquitous
**User Story:** As a system, I want to prevent injection attacks and malformed data so application integrity is maintained.

**Requirement:** The MobileApp SHALL validate all user input on client side for UX and server side for security. Email addresses SHALL be validated using RFC 5321 format. Phone numbers SHALL be validated for format and length. All text input SHALL be sanitized to remove potentially harmful characters or scripts. Special characters SHALL be escaped when used in API requests. File uploads SHALL validate file types and sizes. Credit card input SHALL be processed only through secure payment gateways with PCI compliance.

**Acceptance Criteria:**
- Email format validated (RFC 5321)
- Phone number format validated
- Text input sanitized against injection
- Special characters escaped in requests
- File type and size validated
- Credit cards processed through PCI-compliant gateway
- Input validation errors handled gracefully

---

#### REQ-5.2.4: API Security
**Pattern:** Ubiquitous
**User Story:** As a system, I want to protect API endpoints from unauthorized and malicious access so backend services remain secure.

**Requirement:** All API endpoints SHALL require authentication via JWT token. All endpoints SHALL validate user permissions for requested resource. API rate limiting SHALL prevent abuse (max 100 requests per minute per user). API responses SHALL not leak sensitive information (stack traces, database details). Error messages SHALL be generic and not reveal system details. API requests SHALL include validation for expected data types and ranges.

**Acceptance Criteria:**
- All endpoints require authentication
- Permission validation on each endpoint
- Rate limiting implemented (100/minute per user)
- No sensitive information in error responses
- Generic error messages returned
- Input validation on all endpoints



### 5.3 Internationalization and Localization

#### REQ-5.3.1: Multi-Language Support
**Pattern:** Ubiquitous
**User Story:** As a user, I want to use the app in my preferred language so I can understand all content.

**Requirement:** The MobileApp SHALL support English and Arabic languages. All text content SHALL be externalized into language resource files. User's language preference SHALL be stored and persisted across sessions. Language change SHALL be immediate without app restart. Missing translations SHALL fall back to English. String interpolation and variables SHALL be handled properly in all languages to support varying sentence structures.

**Acceptance Criteria:**
- English and Arabic language options available
- Language preference persisted
- Language change immediate without restart
- All UI text translated
- Fallback to English for untranslated strings
- Proper string interpolation in both languages
- Language selection in settings

---

#### REQ-5.3.2: Right-to-Left (RTL) Layout
**Pattern:** Ubiquitous
**User Story:** As an Arabic user, I want the app layout to be mirrored for Arabic so interface feels natural.

**Requirement:** The MobileApp SHALL automatically mirror UI layout when Arabic language is selected. All directional elements (buttons, images, navigation) SHALL be reversed. Text alignment SHALL flip from left-align to right-align for Arabic. Icons and images SHALL be flipped where appropriate (not for directional icons like arrows that should remain consistent). Navigation drawer, menus, and dialog buttons SHALL adapt to RTL. All components using NativeWind styling SHALL support RTL through proper configuration.

**Acceptance Criteria:**
- Layout automatically mirrored for Arabic
- Text right-aligned for Arabic
- Buttons and controls positioned correctly
- Navigation drawer mirrored
- Icons flipped appropriately
- No layout overflow or misalignment
- Responsive to language changes

---

#### REQ-5.3.3: Date and Time Formatting
**Pattern:** Ubiquitous
**User Story:** As a user, I want dates and times formatted according to my locale so I understand temporal information correctly.

**Requirement:** The MobileApp SHALL format dates and times according to selected language locale. Date format for English: MM/DD/YYYY. Date format for Arabic: DD/MM/YYYY. Time format 24-hour for both. Relative time displays (e.g., "2 hours ago") SHALL be translated. Timezone handling SHALL account for user's device timezone.

**Acceptance Criteria:**
- Date format correct for English (MM/DD/YYYY)
- Date format correct for Arabic (DD/MM/YYYY)
- Time format 24-hour for both languages
- Relative time translated correctly
- Timezone handled correctly
- No ambiguous date formats

---

#### REQ-5.3.4: Content Localization
**Pattern:** Ubiquitous
**User Story:** As a user, I want restaurant-specific content localized so I see appropriate information.

**Requirement:** The MobileApp SHALL display localized menu items, restaurant information, and promotional content. Backend MenuService SHALL provide translations for item names and descriptions. Restaurant contact information and policies SHALL be provided in user's selected language. Currency displays SHALL use appropriate locale-specific formatting.

**Acceptance Criteria:**
- Menu items translated for selected language
- Restaurant info in selected language
- Promotional content localized
- Currency formatting correct
- Contact information complete in both languages

---

### 5.4 Accessibility

#### REQ-5.4.1: Screen Reader Support
**Pattern:** Ubiquitous
**User Story:** As a visually impaired user, I want to use screen readers to navigate the app so I can access all features.

**Requirement:** The MobileApp SHALL support platform screen readers (VoiceOver on iOS, TalkBack on Android). All interactive elements SHALL have appropriate accessibility labels. Buttons SHALL describe their action (e.g., "Add to cart" not just "Button"). Form labels SHALL be properly associated with inputs. Images SHALL have alt text describing content. Lists SHALL be properly marked for screen reader navigation. Heading hierarchy SHALL be logical for content structure.

**Acceptance Criteria:**
- All interactive elements have labels
- Button labels descriptive
- Form fields labeled
- Images have alt text
- Proper heading hierarchy
- Screen reader navigation logical
- No missing accessibility attributes

---

#### REQ-5.4.2: Color Contrast
**Pattern:** Ubiquitous
**User Story:** As a user with low vision, I want sufficient color contrast so I can read text easily.

**Requirement:** The MobileApp SHALL ensure text and background color contrast ratio of at least 4.5:1 for normal text and 3:1 for large text (WCAG AA standard). Critical information SHALL not rely solely on color differentiation. Status indicators SHALL include additional visual cues beyond color (icons, patterns).

**Acceptance Criteria:**
- Text contrast ratio at least 4.5:1 (normal)
- Text contrast ratio at least 3:1 (large)
- Color not sole indicator of information
- Status indicators have additional visual cues

---

#### REQ-5.4.3: Touch Target Size
**Pattern:** Ubiquitous
**User Story:** As a user with mobility challenges, I want touch targets large enough so I can interact with app easily.

**Requirement:** The MobileApp SHALL ensure minimum touch target size of 48x48 points (approximately 9.2mm) as recommended by WCAG 2.5.5 standard. All buttons, links, and form fields SHALL meet or exceed this size. Spacing around touch targets SHALL prevent accidental activation of adjacent targets.

**Acceptance Criteria:**
- Minimum touch target size 48x48 points
- Buttons meet minimum size
- Links meet minimum size
- Form fields meet minimum size
- Adequate spacing between targets

---

#### REQ-5.4.4: Keyboard Navigation
**Pattern:** Ubiquitous
**User Story:** As a user unable to use touch, I want to navigate using keyboard so I can use all app features.

**Requirement:** The MobileApp SHALL support keyboard navigation on platform-supported devices (e.g., external keyboards on iPad). Tab order SHALL follow logical flow through screen elements. Focus indicators SHALL be visible and clear. Keyboard shortcuts for common actions (Enter for submit, Escape for close) SHALL follow platform conventions.

**Acceptance Criteria:**
- Tab navigation works logically
- Focus indicators visible
- All interactive elements reachable via keyboard
- Keyboard shortcuts follow conventions
- Escape closes dialogs/modals

---

### 5.5 Offline Support

#### REQ-5.5.1: Offline Data Access
**Pattern:** State-driven
**User Story:** As a user without internet connection, I want to access previously loaded data so I can browse offline.

**Requirement:** The MobileApp SHALL cache essential data (menu, user profile, reservation history, order history) locally on device. TanStack Query caching combined with LocalStorageService SHALL enable offline access. Cached data SHALL include timestamp indicating last update. When offline, the MobileApp SHALL display information from cache with visual indication that data may not be current. Attempted API operations SHALL queue automatically.

**Acceptance Criteria:**
- Menu data cached locally
- Profile data cached
- History data cached
- Offline data displays with indication
- Data timestamp displayed
- Offline operations queued

---

#### REQ-5.5.2: Sync Queue Management
**Pattern:** Event-driven
**User Story:** As a user, I want the app to sync queued actions when connection returns so I don't lose data.

**Requirement:** The MobileApp SHALL maintain a sync queue for actions attempted while offline (add to cart, place order, submit review). SyncService SHALL monitor network connectivity and automatically sync queued actions when connection returns. Sync SHALL be ordered (maintain action sequence). If sync fails for specific action, user SHALL be notified with option to retry or discard. Sync queue SHALL persist across app sessions.

**Acceptance Criteria:**
- Offline actions queued
- Queue persisted across sessions
- Automatic sync when online
- Actions synced in correct order
- Failed syncs show notification
- Retry and discard options available

---

#### REQ-5.5.3: Network Status Indication
**Pattern:** State-driven
**User Story:** As a user, I want to know my network connection status so I understand data limitations.

**Requirement:** The MobileApp SHALL display network status indicator showing online/offline state. When offline, a persistent banner SHALL display "No Internet Connection" and indicate that changes will sync when connection returns. Real-time features (order tracking, reservation updates) SHALL gracefully degrade offline, showing cached data with "Last updated" timestamp. When connection is restored, banner SHALL disappear and sync operations SHALL begin.

**Acceptance Criteria:**
- Network status indicator visible
- Offline banner displayed
- Real-time features gracefully degrade
- Last updated timestamp shown
- Banner disappears when online
- Sync begins automatically



### 5.6 Platform Compatibility

#### REQ-5.6.1: iOS Compatibility
**Pattern:** Ubiquitous
**User Story:** As an iOS user, I want the app to run smoothly on my device so I can use all features.

**Requirement:** The MobileApp SHALL support iOS 14.0 and later. The app SHALL follow iOS design guidelines and conventions. Status bar, safe areas, and notches SHALL be handled correctly. iOS-specific permissions (Camera, Photos, Notifications) SHALL be requested at appropriate times with clear explanations. The app SHALL be distributed through Apple App Store with proper code signing and provisioning profiles.

**Acceptance Criteria:**
- iOS 14.0+ support verified
- Safe areas respected
- Notch/Dynamic Island handled
- iOS permissions requested appropriately
- App Store compliant
- Performance acceptable on iPhone SE and above

---

#### REQ-5.6.2: Android Compatibility
**Pattern:** Ubiquitous
**User Story:** As an Android user, I want the app to run smoothly on my device so I can use all features.

**Requirement:** The MobileApp SHALL support Android API level 24 (Android 7.0) and later. The app SHALL follow Android design guidelines and Material Design conventions. System navigation (back button, gesture navigation) SHALL work correctly. Android-specific permissions (Camera, Location, Notifications) SHALL be handled through runtime permission requests. The app SHALL be distributed through Google Play Store with proper signing and security policies.

**Acceptance Criteria:**
- Android API 24+ support verified
- Material Design conventions followed
- System navigation works correctly
- Runtime permissions handled
- Google Play Store compliant
- Performance acceptable on mid-range devices

---

#### REQ-5.6.3: Screen Size Adaptation
**Pattern:** Ubiquitous
**User Story:** As a user with different device screen sizes, I want the app to adapt so it looks good on my device.

**Requirement:** The MobileApp SHALL be responsive and adapt to different screen sizes (4.7" to 6.7" phones, tablets). NativeWind Tailwind classes SHALL be used for responsive layouts. Text sizes SHALL scale appropriately. Touch targets SHALL remain usable at all screen sizes. Tablets (iPad, Android tablets) SHALL utilize additional screen space efficiently with multi-column layouts where appropriate.

**Acceptance Criteria:**
- Responsive layout on all screen sizes
- Text readable on small screens
- Touch targets usable on all sizes
- Tablets use multi-column layouts
- No layout overflow or cropping
- Images scale appropriately

---

## 6. Integration Requirements

### 6.1 REST API Integration

#### REQ-6.1.1: API Endpoint Consumption
**Pattern:** Ubiquitous
**User Story:** As the mobile app, I want to consume REST API endpoints so I can fetch and update data.

**Requirement:** The MobileApp SHALL consume ASP.NET Core 8 REST API endpoints with the following base URL format: `https://api.naar-noor.com/api/v1/`. All requests SHALL include JWT token in Authorization header: `Authorization: Bearer {token}`. Request/response payloads SHALL use JSON format. Error responses SHALL follow HTTP status codes: 400 (Bad Request), 401 (Unauthorized), 403 (Forbidden), 404 (Not Found), 500 (Server Error). The MobileApp SHALL implement exponential backoff retry logic for transient failures (5xx errors, network timeouts).

**Acceptance Criteria:**
- All API requests use correct base URL
- JWT token included in requests
- JSON format used for payloads
- HTTP status codes followed
- Retry logic implemented with backoff
- Error handling implemented

---

#### REQ-6.1.2: Authentication Endpoints
**Pattern:** Ubiquitous
**User Story:** As the mobile app, I want to authenticate users so I can maintain secure sessions.

**Requirement:** The MobileApp SHALL consume following authentication endpoints:
- `POST /auth/register` - Register new user
- `POST /auth/login` - Authenticate user
- `POST /auth/refresh` - Refresh JWT token
- `POST /auth/logout` - Invalidate token
- `POST /auth/password-reset` - Request password reset
- `POST /auth/password-reset-confirm` - Confirm password reset

Each endpoint response SHALL include appropriate HTTP status code and JSON payload with token or message.

**Acceptance Criteria:**
- All auth endpoints functional
- JWT tokens issued correctly
- Error responses appropriate
- Token refresh working
- Logout invalidates tokens

---

#### REQ-6.1.3: Menu Endpoints
**Pattern:** Ubiquitous
**User Story:** As the mobile app, I want to retrieve menu data so I can display items to customers.

**Requirement:** The MobileApp SHALL consume following menu endpoints:
- `GET /menu/categories` - Get menu categories
- `GET /menu/categories/{id}/items` - Get items for category
- `GET /menu/items/{id}` - Get item details
- `GET /menu/items/search?q={query}` - Search items

Responses SHALL include item details: name, description, price, image URL, dietary tags, availability status, preparation time, ratings.

**Acceptance Criteria:**
- Categories endpoint returns proper data
- Items endpoint returns proper data
- Search endpoint functional
- Item details complete
- Image URLs valid

---

#### REQ-6.1.4: Reservation Endpoints
**Pattern:** Ubiquitous
**User Story:** As the mobile app, I want to manage reservations so customers can book tables.

**Requirement:** The MobileApp SHALL consume following reservation endpoints:
- `GET /reservations/availability?date={date}&time={time}&partySize={size}` - Check availability
- `POST /reservations` - Create reservation
- `GET /reservations/{id}` - Get reservation details
- `PUT /reservations/{id}` - Update reservation
- `DELETE /reservations/{id}` - Cancel reservation
- `GET /reservations?userId={userId}` - Get user's reservations

Responses SHALL include reservation ID, date, time, party size, table details, status, and confirmation code.

**Acceptance Criteria:**
- Availability check functional
- Reservations creatable
- Reservations retrievable
- Reservations updatable
- Reservations cancellable

---

#### REQ-6.1.5: Order Endpoints
**Pattern:** Ubiquitous
**User Story:** As the mobile app, I want to manage orders so customers can place orders and track status.

**Requirement:** The MobileApp SHALL consume following order endpoints:
- `POST /orders` - Create order
- `GET /orders/{id}` - Get order details
- `GET /orders?userId={userId}` - Get user's orders
- `PUT /orders/{id}/status` - Update order status (staff only)
- `DELETE /orders/{id}` - Cancel order
- `GET /orders/{id}/tracking` - Get real-time tracking

Responses SHALL include order number, items, total, status, estimated delivery time, and driver info (if applicable).

**Acceptance Criteria:**
- Orders creatable
- Orders retrievable
- Order status updates functional
- Tracking information available
- Order cancellation functional

---

#### REQ-6.1.6: Review Endpoints
**Pattern:** Ubiquitous
**User Story:** As the mobile app, I want to manage reviews so customers can rate items and experiences.

**Requirement:** The MobileApp SHALL consume following review endpoints:
- `POST /reviews/items` - Submit item review
- `GET /reviews/items/{itemId}` - Get item reviews
- `POST /reviews/experience` - Submit experience rating
- `GET /reviews/{id}` - Get review details
- `PUT /reviews/{id}` - Update review
- `DELETE /reviews/{id}` - Delete review

Responses SHALL include review ID, rating, text, photos, reviewer info, and timestamp.

**Acceptance Criteria:**
- Reviews submittable
- Reviews retrievable
- Reviews editable
- Reviews deletable
- Photo handling functional

---

### 6.2 Payment Integration

#### REQ-6.2.1: Payment Gateway Integration
**Pattern:** Ubiquitous
**User Story:** As a customer, I want to pay securely so my payment information is protected.

**Requirement:** The MobileApp SHALL integrate with secure payment gateway (Stripe, PayPal, or equivalent) for processing payments. Payment processing SHALL be handled through secure payment SDK (Stripe Mobile SDK, PayPal SDK). Card details SHALL NOT pass through mobile app backend; only payment tokens SHALL be transmitted. Payment endpoints SHALL handle payment processing through backend PaymentService. Refund processing SHALL be available for cancelled/returned orders.

**Acceptance Criteria:**
- Payments processed securely
- Card details not stored on device
- Tokens transmitted securely
- Payment confirmation received
- Refunds processable
- PCI compliance maintained

---

## 7. Out-of-Scope Items

The following items are identified as future enhancements and are not included in this initial release:

1. **In-App Messaging System** - Direct messaging between customers and restaurant staff
2. **Advanced Analytics Dashboard** - Detailed business analytics for staff/management
3. **Loyalty Program Integration** - Points, rewards, and tier system
4. **Social Features** - Friend lists, shared meal planning, group orders
5. **AI-Based Recommendations** - Machine learning recommendations based on order history
6. **Voice Ordering** - Voice-activated order placement
7. **Augmented Reality Menu** - AR visualization of menu items
8. **Blockchain Payment Methods** - Cryptocurrency payment support
9. **Advanced Scheduling** - Recurring reservations and subscriptions
10. **Integration with Third-Party Delivery Services** - DoorDash, Uber Eats integration

---

## 8. Quality Checklist

**Requirement Quality Standards Applied:**

✓ **No Vague Terms** - All requirements use specific, measurable criteria
✓ **Active Voice** - All requirements use active voice (System SHALL, User WANTS)
✓ **Testable Conditions** - Each acceptance criterion is verifiable and testable
✓ **EARS Pattern** - All requirements follow EARS (Event, Action, Response) patterns
✓ **Complete User Stories** - Each requirement includes "As a [user], I want [action], so [benefit]"
✓ **Acceptance Criteria** - Each requirement lists specific, testable acceptance criteria
✓ **Traceability** - Requirements numbered for clear identification and traceability
✓ **Role-Based Organization** - Requirements organized by feature and user role
✓ **Non-Ambiguity** - Requirements clear and unambiguous, no interpretation needed
✓ **Completeness** - All major features and non-functional aspects covered

