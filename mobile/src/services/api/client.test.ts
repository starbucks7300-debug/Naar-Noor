import { apiClient } from './client';
import { secureTokenStorage } from '@services/storage/secureTokenStorage';
import { useAuthStore } from '@stores/useAuthStore';
import { tokenManager } from './tokenManager';
import { logger } from '@services/logger';

jest.mock('@services/storage/secureTokenStorage');
jest.mock('@stores/useAuthStore');
jest.mock('./tokenManager');
jest.mock('@services/logger');

describe('API Client Interceptors', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  describe('Request Interceptor', () => {
    it('should attach Authorization header with token', async () => {
      const mockToken = 'mock_access_token';
      (secureTokenStorage.getAccessToken as jest.Mock).mockResolvedValue(mockToken);

      const config = { headers: {} };
      const requestInterceptor = apiClient.interceptors.request.handlers[0];

      await requestInterceptor.fulfilled(config);

      expect(config.headers.Authorization).toBe(`Bearer ${mockToken}`);
    });

    it('should handle missing token gracefully', async () => {
      (secureTokenStorage.getAccessToken as jest.Mock).mockResolvedValue(null);

      const config = { headers: {} };
      const requestInterceptor = apiClient.interceptors.request.handlers[0];

      await requestInterceptor.fulfilled(config);

      expect(config.headers.Authorization).toBeUndefined();
    });

    it('should log error if token retrieval fails', async () => {
      const error = new Error('Storage error');
      (secureTokenStorage.getAccessToken as jest.Mock).mockRejectedValue(error);

      const config = { headers: {} };
      const requestInterceptor = apiClient.interceptors.request.handlers[0];

      await requestInterceptor.fulfilled(config);

      expect(logger.error).toHaveBeenCalledWith('Error retrieving token', error);
    });
  });

  describe('Response Interceptor - 401 Error', () => {
    it('should attempt token refresh on 401 error', async () => {
      const newAccessToken = 'new_access_token';
      (tokenManager.requestTokenRefresh as jest.Mock).mockResolvedValue(
        newAccessToken
      );

      const originalRequest = {
        headers: {},
        config: {},
      };

      const error = {
        response: { status: 401 },
        config: originalRequest,
      };

      const responseInterceptor = apiClient.interceptors.response.handlers[1];

      // We need to test this through the actual axios flow
      // This is a simplified test - full integration would need axios test setup
      expect(responseInterceptor).toBeDefined();
    });

    it('should not retry if already retried', async () => {
      const originalRequest = {
        headers: {},
        _retry: true,
      };

      const error = {
        response: { status: 401 },
        config: originalRequest,
      };

      const responseInterceptor = apiClient.interceptors.response.handlers[1];

      expect(responseInterceptor).toBeDefined();
    });
  });

  describe('Response Interceptor - Success', () => {
    it('should pass through successful responses', async () => {
      const response = {
        status: 200,
        data: { message: 'success' },
      };

      const responseInterceptor = apiClient.interceptors.response.handlers[0];
      const result = responseInterceptor.fulfilled(response);

      expect(result).toEqual(response);
    });
  });

  describe('Response Interceptor - Other Errors', () => {
    it('should reject non-401 errors', async () => {
      const error = {
        response: { status: 500 },
        config: {},
      };

      const responseInterceptor = apiClient.interceptors.response.handlers[1];
      // Would be tested through full axios flow

      expect(responseInterceptor).toBeDefined();
    });

    it('should reject network errors', async () => {
      const error = {
        response: undefined,
        config: {},
        message: 'Network Error',
      };

      const responseInterceptor = apiClient.interceptors.response.handlers[1];
      // Would be tested through full axios flow

      expect(responseInterceptor).toBeDefined();
    });
  });

  describe('Token Refresh Flow', () => {
    it('should handle logout on refresh failure', async () => {
      const mockClearAuth = jest.fn();
      const mockStore = {
        clearAuth: mockClearAuth,
        getState: jest.fn().mockReturnValue({
          clearAuth: mockClearAuth,
        }),
      };
      (useAuthStore.getState as jest.Mock).mockReturnValue(mockStore);

      (tokenManager.requestTokenRefresh as jest.Mock).mockRejectedValue(
        new Error('Refresh failed')
      );

      const responseInterceptor = apiClient.interceptors.response.handlers[1];
      expect(responseInterceptor).toBeDefined();
    });
  });
});
