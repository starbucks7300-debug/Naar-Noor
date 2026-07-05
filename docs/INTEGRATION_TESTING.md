# Integration Testing Guide - Naar-Noor

## Overview

This guide covers testing all components together:
- Backend API with PostgreSQL
- Frontend Angular web
- Mobile Expo app
- Desktop WinForms app

---

## 1. Starting the Development Environment

### Step 1: Initialize Docker Services

```bash
cd Naar-Noor

# Set environment
export PGPASSWORD=dev_password_change_me

# Start all services
docker-compose -f docker-compose.dev.yml up -d

# Verify all services running
docker-compose -f docker-compose.dev.yml ps
```

Expected output:
```
NAME                         STATUS
naar-noor-dev-web           Up (healthy)
naar-noor-dev-api           Up (healthy)
naar-noor-dev-database      Up (healthy)
naar-noor-dev-adminer       Up
```

### Step 2: Verify Database Connection

```bash
# Access Adminer UI
open http://localhost:8081

# Or test via CLI
docker-compose -f docker-compose.dev.yml exec naar-noor-dev-database \
  psql -U postgres -d postgres -c "SELECT version();"
```

### Step 3: Check Backend API

```bash
# Health check
curl http://localhost:8080/health

# Swagger docs
open http://localhost:8080/swagger/index.html

# Test endpoint
curl -X GET http://localhost:8080/api/menu/items
```

### Step 4: Verify Frontend Web

```bash
# Open in browser
open http://localhost:4200

# Check console for errors
# Should see: "Angular app initialized"
```

---

## 2. Backend API Integration Tests

### Test Authentication Flow

```bash
# 1. Register new user
curl -X POST http://localhost:8080/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "TestPassword123!",
    "firstName": "Test",
    "lastName": "User"
  }'

# Response should include: userId, token

# 2. Login
curl -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "TestPassword123!"
  }'

# Save the token for next requests
TOKEN="your_token_here"

# 3. Access protected endpoint
curl -X GET http://localhost:8080/api/reservations/my \
  -H "Authorization: Bearer $TOKEN"
```

### Test Menu API

```bash
# Get all menu items
curl http://localhost:8080/api/menu/items

# Create menu item (requires auth)
TOKEN="your_token_here"
curl -X POST http://localhost:8080/api/menu/items \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Grilled Fish",
    "description": "Fresh grilled fish",
    "price": 25.99,
    "category": "Main Course"
  }'

# Update menu item
curl -X PUT http://localhost:8080/api/menu/items/1 \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Grilled Fish (Updated)",
    "price": 27.99
  }'

# Delete menu item
curl -X DELETE http://localhost:8080/api/menu/items/1 \
  -H "Authorization: Bearer $TOKEN"
```

### Test Reservations API

```bash
TOKEN="your_token_here"

# Create reservation
curl -X POST http://localhost:8080/api/reservations \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "date": "2026-07-15",
    "time": "19:00",
    "partySize": 4,
    "specialRequests": "Window seat if available"
  }'

# Get my reservations
curl http://localhost:8080/api/reservations/my \
  -H "Authorization: Bearer $TOKEN"

# Cancel reservation
curl -X DELETE http://localhost:8080/api/reservations/1 \
  -H "Authorization: Bearer $TOKEN"
```

### Run Backend Unit Tests

```bash
cd api-server

# Run all tests
dotnet test

# Run specific test class
dotnet test --filter "ClassName=AuthenticationServiceTests"

# With coverage
dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura
```

---

## 3. Frontend Web Integration Tests

### Manual Testing Checklist

- [ ] **Login Screen**
  - Navigate to http://localhost:4200
  - Enter test credentials
  - Verify login success and redirect to dashboard

- [ ] **Dashboard**
  - Verify menu metrics load
  - Verify reservation stats display
  - Check responsive design on mobile breakpoints

- [ ] **Menu Management**
  - Create new menu item
  - Edit existing item
  - Delete item with confirmation
  - Verify changes reflected in API

- [ ] **Reservations**
  - Make new reservation
  - View reservation history
  - Cancel reservation
  - Verify confirmation message

- [ ] **Reports**
  - Generate revenue report
  - Filter by date range
  - Export to CSV/PDF
  - Verify data accuracy

### Run Frontend Tests

```bash
cd naar-noor

# Run unit tests
npm test

# Run with coverage
npm run test:coverage

# Run E2E tests (if configured)
npm run e2e

# Run linting
npm run lint
```

---

## 4. Mobile App Integration Tests

### Setup Mobile Environment

```bash
cd mobile

# Install dependencies
npm install

# Start Expo server
npm start
```

### Test on Simulator/Emulator

**iOS Simulator:**
```bash
# Press 'i' in Expo terminal
# Or
npx expo run:ios
```

**Android Emulator:**
```bash
# Press 'a' in Expo terminal
# Or
npx expo run:android
```

### Manual Testing Checklist

- [ ] **Login/Register**
  - Load login screen
  - Enter credentials
  - Verify successful authentication
  - Check token storage in SecureStore

- [ ] **Home Screen**
  - Load menu items (API call)
  - Verify list renders correctly
  - Test infinite scroll/pagination
  - Check theme colors (light/dark mode)

- [ ] **Browse Menu**
  - Filter by category
  - Search functionality
  - Add items to cart
  - Verify cart count updates

- [ ] **Reservations**
  - Make new reservation
  - Select date and time
  - Verify availability
  - Submit and confirm

- [ ] **User Profile**
  - Edit profile information
  - Change password
  - View order history
  - Logout functionality

- [ ] **Network Resilience**
  - Enable airplane mode
  - Verify offline message
  - Disable airplane mode
  - Verify reconnection and data sync

### Run Mobile Tests

```bash
cd mobile

# Run unit tests
npm test

# Run with coverage
npm run test:coverage

# Run integration tests
npm run test:integration

# Watch mode for development
npm run test:watch
```

### Test on Physical Device

```bash
# Start Expo
npm start

# Scan QR code with Expo Go app
# On your device, open Expo Go app
# Scan the QR code displayed in terminal

# App should load on device
# Test all features on real hardware
```

---

## 5. Desktop App Integration Tests

### Build and Run

```bash
cd desktop

# Restore dependencies
dotnet restore

# Run application
dotnet run --project src/NaarNoor.Desktop.WinForms

# Or with hot reload
dotnet watch run --project src/NaarNoor.Desktop.WinForms
```

### Manual Testing Checklist

- [ ] **Login Form**
  - Enter credentials
  - Verify API connection to backend
  - Check error handling for invalid credentials
  - Test "Remember Me" checkbox

- [ ] **Dashboard**
  - Verify theme applied (light/dark)
  - Check RTL/LTR layout switching
  - Verify all metrics load
  - Test tab navigation

- [ ] **Menu Management**
  - Load menu items from API
  - Create new item with validation
  - Edit existing items
  - Delete with confirmation
  - Verify real-time API sync

- [ ] **Reservations**
  - View all reservations
  - Filter by date range
  - Confirm/cancel reservations
  - Check conflict detection

- [ ] **Staff Management**
  - Add new staff member
  - Edit roles/permissions
  - Deactivate staff
  - View staff activity logs

- [ ] **Reports**
  - Generate revenue reports
  - Export to CSV/Excel
  - Filter by date range
  - Verify calculation accuracy

- [ ] **Localization**
  - Switch language (English/Arabic)
  - Verify all UI text updates
  - Check RTL layout applies
  - Test number/date formatting

### Run Desktop Tests

```bash
cd desktop

# Run all tests
dotnet test

# Run specific test file
dotnet test --filter "ClassName=LoginViewModelTests"

# Run with coverage
dotnet test /p:CollectCoverage=true

# Run property-based tests
dotnet test --filter "Category=PropertyTest"
```

---

## 6. Cross-Platform Integration Scenarios

### Scenario 1: Complete User Journey

**Desktop Admin:**
1. Login to desktop app
2. Create new menu item
3. Verify item appears in API

**Mobile User:**
1. Open mobile app
2. Refresh menu (or restart app)
3. Verify new item appears
4. Make reservation

**Web Admin:**
1. Login to web dashboard
2. View new reservation
3. Confirm reservation
4. Send confirmation email

**Mobile User:**
1. Check notification (if configured)
2. View reservation confirmation

---

### Scenario 2: Real-Time Updates

**Desktop Admin:**
1. Update menu item price
2. Save changes

**Mobile User:**
1. Already has app open
2. Should see price update (if real-time sync enabled)
3. Or verify update on next refresh

**Web User:**
1. Refresh menu
2. Verify price update

---

### Scenario 3: Offline Resilience

**Mobile User:**
1. Load app and browse menu (online)
2. Enable airplane mode
3. App should show cached data
4. Disable airplane mode
5. Verify sync of any pending changes

---

## 7. Performance Testing

### Backend API Load Test

```bash
# Using Apache Bench
ab -n 1000 -c 10 http://localhost:8080/api/menu/items

# Using wrk
wrk -t4 -c100 -d30s http://localhost:8080/api/menu/items
```

### Frontend Performance

```bash
# Audit with Lighthouse
npm install -g lighthouse
lighthouse http://localhost:4200 --view

# Check bundle size
npm run build -- --prod
# View: dist/naar-noor/browser/assets/
```

### Mobile Performance

```bash
cd mobile

# Check bundle size
npm run build:web
# View: dist/bundle.js size

# Profile with React Native Debugger
# Download: https://github.com/jhen0409/react-native-debugger
```

---

## 8. Security Testing Checklist

- [ ] **Authentication**
  - Token expiration works
  - Invalid tokens rejected
  - Credentials not stored in logs

- [ ] **Authorization**
  - Users can't access admin endpoints
  - Can't modify other users' data
  - Permissions enforced consistently

- [ ] **API Security**
  - CORS properly configured
  - Rate limiting active
  - Input validation working
  - SQL injection attempts blocked

- [ ] **Mobile Security**
  - Tokens stored securely (SecureStore)
  - API calls over HTTPS
  - No sensitive data in localStorage

- [ ] **Desktop Security**
  - Credentials not saved in code
  - Secure token storage
  - Session timeout implemented

---

## 9. Troubleshooting Integration Issues

### Frontend can't connect to API

```bash
# Check API is running
curl http://localhost:8080/health

# Check CORS in backend
# Should see Access-Control-Allow-Origin header
curl -I http://localhost:8080/api/menu/items

# Check frontend environment
echo $API_URL
# Should be: http://localhost:8080
```

### Mobile can't reach desktop backend

```bash
# Get your machine IP
ipconfig  # Windows
ifconfig  # Mac/Linux

# Update mobile .env
EXPO_PUBLIC_API_URL=http://YOUR_IP:8080

# Test from phone
curl http://YOUR_IP:8080/health
```

### Database shows stale data

```bash
# Clear database and restart
docker-compose down -v
docker-compose up

# Or directly
docker-compose exec naar-noor-dev-database \
  psql -U postgres -d postgres -c "DROP SCHEMA public CASCADE; CREATE SCHEMA public;"
```

---

## 10. Automated Integration Tests (CI/CD)

See `.github/workflows/` for GitHub Actions that run:
- Backend tests
- Frontend tests
- Mobile tests
- Security scans
- Code coverage reports

Tests run automatically on push to `main` branch.
