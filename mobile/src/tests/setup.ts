// Test setup and configuration
import '@testing-library/jest-dom';

// Mock Expo modules
jest.mock('expo-constants', () => ({
  default: {
    expoConfig: {
      extra: {
        eas: {
          projectId: 'test-project-id',
        },
      },
    },
  },
}));

jest.mock('expo-localization', () => ({
  locales: ['en'],
  locale: 'en',
  getCalendars: () => [],
  getNumberFormatSettings: () => ({
    decimalSeparator: '.',
    groupingSeparator: ',',
  }),
}));

// Mock SecureStore
jest.mock('expo-secure-store', () => ({
  setItemAsync: jest.fn().mockResolvedValue(undefined),
  getItemAsync: jest.fn().mockResolvedValue(null),
  deleteItemAsync: jest.fn().mockResolvedValue(undefined),
  getItem: jest.fn().mockReturnValue(null),
  setItem: jest.fn(),
  removeItem: jest.fn(),
}));

// Mock AsyncStorage
jest.mock('@react-native-async-storage/async-storage', () => ({
  setItem: jest.fn().mockResolvedValue(undefined),
  getItem: jest.fn().mockResolvedValue(null),
  removeItem: jest.fn().mockResolvedValue(undefined),
  clear: jest.fn().mockResolvedValue(undefined),
  getAllKeys: jest.fn().mockResolvedValue([]),
  multiSet: jest.fn().mockResolvedValue(undefined),
  multiGet: jest.fn().mockResolvedValue([]),
  multiRemove: jest.fn().mockResolvedValue(undefined),
}));

// Mock expo-notifications
jest.mock('expo-notifications', () => ({
  setNotificationHandler: jest.fn(),
  addNotificationReceivedListener: jest.fn().mockReturnValue({
    remove: jest.fn(),
  }),
  addNotificationResponseReceivedListener: jest.fn().mockReturnValue({
    remove: jest.fn(),
  }),
}));

// Mock axios with interceptor support
jest.mock('axios', () => {
  const actualAxios = jest.requireActual('axios');
  return {
    ...actualAxios,
    create: jest.fn(() => ({
      ...actualAxios.create(),
      interceptors: {
        request: {
          use: jest.fn((fulfilled, rejected) => ({
            fulfilled,
            rejected,
          })),
          handlers: [
            {
              fulfilled: jest.fn(),
              rejected: jest.fn(),
            },
          ],
        },
        response: {
          use: jest.fn((fulfilled, rejected) => ({
            fulfilled,
            rejected,
          })),
          handlers: [
            {
              fulfilled: jest.fn(),
              rejected: jest.fn(),
            },
            {
              fulfilled: jest.fn(),
              rejected: jest.fn(),
            },
          ],
        },
      },
    })),
  };
});

// Suppress console errors in tests (optional, remove if you want to see them)
global.console = {
  ...console,
  error: jest.fn(),
  warn: jest.fn(),
};

// Mock fetch if needed
global.fetch = jest.fn();
