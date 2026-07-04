import { User, TokenPair } from '@types/auth.types';

/**
 * Factory function to generate mock user data for testing
 */
export const createMockUser = (overrides?: Partial<User>): User => {
  return {
    id: 'user-123',
    email: 'test@example.com',
    firstName: 'John',
    lastName: 'Doe',
    phone: '+12025551234',
    role: 'customer',
    createdAt: new Date().toISOString(),
    updatedAt: new Date().toISOString(),
    ...overrides,
  };
};

/**
 * Factory function to generate mock token pair for testing
 */
export const createMockTokenPair = (overrides?: Partial<TokenPair>): TokenPair => {
  return {
    accessToken: 'mock-access-token-' + Math.random().toString(36).substr(2, 9),
    refreshToken: 'mock-refresh-token-' + Math.random().toString(36).substr(2, 9),
    expiresIn: 3600,
    ...overrides,
  };
};

/**
 * Factory function for authenticated user state
 */
export const createMockAuthState = (overrides?: any) => {
  const user = createMockUser(overrides?.user);
  const tokenPair = createMockTokenPair(overrides?.tokenPair);

  return {
    user,
    tokenPair,
    isAuthenticated: true,
    isLoading: false,
    error: null,
    ...overrides,
  };
};
