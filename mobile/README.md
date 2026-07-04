# Naar-Noor Mobile Application

Cross-platform React Native mobile application for Naar-Noor restaurant system. Built with Expo, TypeScript, TanStack Query, and Zustand.

## Features

- **Customer App**
  - Browse menu with categories and search
  - Shopping cart management
  - Table reservations
  - Order placement and tracking
  - Reviews and ratings
  - User profile management

- **Staff App**
  - Reservation management dashboard
  - Order management and status updates
  - Role-based access control
  - Real-time updates

- **General Features**
  - Full bilingual support (English + Arabic)
  - RTL layout for Arabic
  - Offline-first architecture
  - Secure authentication with JWT
  - Push notifications
  - Local caching and sync queue

## Tech Stack

- **Framework**: React Native with Expo
- **Language**: TypeScript
- **State Management**: TanStack Query + Zustand
- **Navigation**: Expo Router
- **UI**: NativeWind (Tailwind CSS)
- **API**: Axios with JWT interceptors
- **Authentication**: JWT with secure storage
- **i18n**: i18next with English/Arabic
- **Testing**: Jest + React Native Testing Library
- **E2E Testing**: Detox

## Getting Started

### Prerequisites

- Node.js >= 18.0.0
- npm >= 9.0.0
- Expo CLI: `npm install -g expo-cli`
- For iOS: Xcode (macOS)
- For Android: Android Studio

### Installation

```bash
cd mobile
npm install
```

### Environment Setup

Copy `.env.example` to `.env.local` and configure:

```bash
cp .env.example .env.local
```

Update with your API URL and other configuration.

### Running the App

```bash
# Development server
npm run start

# iOS simulator
npm run ios

# Android emulator
npm run android

# Web preview
npm run web
```

### Building

```bash
# Production APK (Android)
npm run build:eas-android

# Production IPA (iOS)
npm run build:eas-ios
```

## Project Structure

```
mobile/
├── src/
│   ├── app/                 # Expo Router screens (file-based routing)
│   │   ├── (auth)/          # Authentication screens
│   │   ├── (app)/           # Main application screens
│   │   │   ├── (home)/      # Menu browsing
│   │   │   ├── (cart)/      # Shopping cart
│   │   │   ├── (orders)/    # Orders management
│   │   │   ├── (reservations)/ # Reservations
│   │   │   ├── (profile)/   # User profile
│   │   │   ├── (dashboard)/ # Staff dashboard
│   │   │   └── _layout.tsx  # Main navigation
│   │   └── _layout.tsx      # Root layout
│   ├── components/          # Reusable UI components
│   ├── screens/             # Complex screen components
│   ├── services/            # API clients and business logic
│   │   └── api/
│   ├── stores/              # Zustand stores (state management)
│   ├── hooks/               # Custom React hooks
│   ├── types/               # TypeScript type definitions
│   ├── utils/               # Utility functions
│   ├── i18n/                # Internationalization
│   └── config/              # Configuration files
├── assets/                  # Images, icons, fonts
├── tests/                   # Test files
├── app.json                 # Expo configuration
├── tsconfig.json            # TypeScript configuration
├── babel.config.js          # Babel configuration
├── eas.json                 # EAS build configuration
└── package.json             # Dependencies
```

## Development Guidelines

### Creating New Features

1. **Create API Service** (`src/services/api/*.ts`)
   - Use Axios client
   - Follow existing patterns

2. **Create Zustand Store** (`src/stores/*.ts`) if needed
   - For local state management
   - Use persist middleware for persistence

3. **Create Screens/Components** (`src/app/` or `src/components/`)
   - Use TypeScript for type safety
   - Use NativeWind for styling
   - Follow React Native best practices

4. **Write Tests**
   - Unit tests for utilities
   - Integration tests for features
   - E2E tests for critical flows

### Code Style

- ESLint + Prettier configured
- Format before committing: `npm run format`
- Lint check: `npm run lint`

### TypeScript

- Strict mode enabled
- Always use proper typing
- No `any` without justification

### Styling

- Use NativeWind (Tailwind CSS for React Native)
- Mobile-first responsive design
- Support both light and dark modes

## Testing

```bash
# Run unit tests
npm test

# Run tests in watch mode
npm run test:watch

# Generate coverage report
npm run test:coverage

# Run E2E tests (iOS)
npm run test:e2e:ios

# Run E2E tests (Android)
npm run test:e2e:android
```

## API Integration

All API calls go through the centralized Axios client (`src/services/api/client.ts`):

- Automatic JWT injection
- Token refresh on 401 responses
- Centralized error handling
- Request/response logging

### Creating API Services

```typescript
// src/services/api/menuApi.ts
export const menuApi = {
  async getCategories() {
    const { data } = await apiClient.get('/menu/categories');
    return data;
  },
};
```

### Using API Hooks

```typescript
// src/hooks/useMenuCategories.ts
export function useMenuCategories() {
  return useQuery({
    queryKey: ['menu', 'categories'],
    queryFn: () => menuApi.getCategories(),
  });
}
```

## Offline Support

- Menu, reservations, and orders cached locally
- Automatic sync when connection returns
- Sync queue for offline mutations
- Network status monitoring

## Internationalization

- English and Arabic supported
- Automatic RTL layout for Arabic
- Translation keys in `src/i18n/locales/`
- Use `useTranslation()` hook in components

```typescript
const { t } = useTranslation();
return <Text>{t('menu.categories')}</Text>;
```

## Security

- JWT tokens stored securely (Keychain/Keystore)
- HTTPS enforced for all API calls
- Input validation on both client and server
- No sensitive data logged
- Rate limiting on auth endpoints

## Performance

- App launch time: < 2 seconds
- Screen load time: < 1.5 seconds
- Image caching and optimization
- Code splitting for lazy loading
- Memory usage: < 100MB

## Accessibility

- WCAG AA compliance
- Screen reader support
- Color contrast verification
- Keyboard navigation
- Touch targets: 48x48 points minimum

## Troubleshooting

### App Won't Start
```bash
npm install
npm start
```

### Port 8081 Already in Use
```bash
kill $(lsof -t -i :8081)  # macOS/Linux
netstat -ano | findstr :8081  # Windows
```

### Expo Go Issues
- Make sure you're on the same network
- Try clearing the Expo cache: `expo start --clear`

### Build Errors
- Clear node_modules: `rm -rf node_modules && npm install`
- Clear Expo cache: `expo start --clear`

## Deployment

### iOS App Store

1. Configure iOS code signing: `eas credentials`
2. Build for production: `npm run build:eas-ios`
3. Upload to TestFlight or App Store

### Google Play Store

1. Configure Android signing: `eas credentials`
2. Build for production: `npm run build:eas-android`
3. Upload to Google Play Store

## Contributing

- Follow the existing code structure
- Write tests for new features
- Update documentation
- Follow TypeScript best practices
- Use ESLint and Prettier

## License

Proprietary - Naar-Noor Restaurant Management System

## Support

For issues or questions, please contact the development team.

---

**Last Updated**: July 4, 2026  
**Version**: 1.0.0-alpha
