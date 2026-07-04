import { setupServer } from 'msw/node';
import { authHandlers } from './handlers/auth';
import { menuHandlers } from './handlers/menu';
import { orderHandlers } from './handlers/orders';

/**
 * Mock API Server
 * Combines all handlers from different feature areas
 */
export const server = setupServer(...authHandlers, ...menuHandlers, ...orderHandlers);

// Enable API mocking before all tests
beforeAll(() => {
  server.listen({ onUnhandledRequest: 'warn' });
});

// Reset handlers after each test
afterEach(() => {
  server.resetHandlers();
});

// Clean up after all tests
afterAll(() => {
  server.close();
});
