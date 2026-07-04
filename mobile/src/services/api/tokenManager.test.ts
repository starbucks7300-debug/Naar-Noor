import { tokenManager } from './tokenManager';
import { secureTokenStorage } from '@services/storage/secureTokenStorage';
import { useAuthStore } from '@stores/useAuthStore';
import { logger } from '@services/logger';

jest.mock('@services/storage/secureTokenStorage');
jest.mock('@stores/useAuthStore');
jest.mock('@services/logger');

describe('TokenManager', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    jest.useFakeTimers();
    global.fetch = jest.fn();
  });

  afterEach(() => {
    jest.useRealTimers();
  });

  describe('getAccessToken', () => {
    it('should retrieve access token from secure storage', async () => {
      const mockToken = 'mock_access_token';
      (secureTokenStorage.getAccessToken as jest.Mock).mockResolvedValue(mockToken);

      const token = await tokenManager.getAccessToken();

      expect(token).toBe(mockToken);
      expect(secureTokenStorage.getAccessToken).toHaveBeenCalled();
    });

    it('should handle error retrieving token', async () => {
      (secureTokenStorage.getAccessToken as jest.Mock).mockRejectedValue(
        new Error('Storage error')
      );

      const token = await tokenManager.getAccessToken();

      expect(token).toBeNull();
    });
  });

  describe('getRefreshToken', () => {
    it('should retrieve refresh token from secure storage', async () => {
      const mockToken = 'mock_refresh_token';
      (secureTokenStorage.getRefreshToken as jest.Mock).mockResolvedValue(mockToken);

      const token = await tokenManager.getRefreshToken();

      expect(token).toBe(mockToken);
      expect(secureTokenStorage.getRefreshToken).toHaveBeenCalled();
    });
  });

  describe('scheduleTokenRefresh', () => {
    it('should schedule refresh before token expiry', () => {
      const mockStore = {
        tokens: {
          accessToken: 'token',
          refreshToken: 'refresh',
          expiresIn: 3600, // 1 hour
        },
      };
      (useAuthStore.getState as jest.Mock).mockReturnValue(mockStore);

      tokenManager.scheduleTokenRefresh();

      // Should schedule refresh 30 minutes before expiry
      expect(setTimeout).toHaveBeenCalledWith(expect.any(Function), expect.any(Number));
    });

    it('should not schedule refresh if no tokens', () => {
      const mockStore = {
        tokens: null,
      };
      (useAuthStore.getState as jest.Mock).mockReturnValue(mockStore);

      tokenManager.scheduleTokenRefresh();

      expect(setTimeout).not.toHaveBeenCalled();
    });

    it('should clear previous refresh timeout', () => {
      const mockStore = {
        tokens: {
          accessToken: 'token',
          refreshToken: 'refresh',
          expiresIn: 3600,
        },
      };
      (useAuthStore.getState as jest.Mock).mockReturnValue(mockStore);

      tokenManager.scheduleTokenRefresh();
      const firstCallCount = (setTimeout as jest.Mock).mock.calls.length;

      tokenManager.scheduleTokenRefresh();
      const secondCallCount = (setTimeout as jest.Mock).mock.calls.length;

      expect(secondCallCount).toBeGreaterThan(firstCallCount);
    });
  });

  describe('requestTokenRefresh', () => {
    it('should handle successful token refresh', async () => {
      const mockRefreshToken = 'mock_refresh_token';
      const newAccessToken = 'new_access_token';
      const newRefreshToken = 'new_refresh_token';

      (secureTokenStorage.getRefreshToken as jest.Mock).mockResolvedValue(
        mockRefreshToken
      );

      (global.fetch as jest.Mock).mockResolvedValue({
        ok: true,
        json: jest.fn().mockResolvedValue({
          accessToken: newAccessToken,
          refreshToken: newRefreshToken,
          expiresIn: 3600,
        }),
      });

      const mockStore = {
        tokens: { accessToken: 'old', refreshToken: mockRefreshToken, expiresIn: 3600 },
        setTokens: jest.fn(),
        clearAuth: jest.fn(),
      };
      (useAuthStore.getState as jest.Mock).mockReturnValue(mockStore);

      const token = await tokenManager.requestTokenRefresh();

      expect(token).toBe(newAccessToken);
      expect(secureTokenStorage.setAccessToken).toHaveBeenCalledWith(newAccessToken);
      expect(secureTokenStorage.setRefreshToken).toHaveBeenCalledWith(newRefreshToken);
      expect(mockStore.setTokens).toHaveBeenCalledWith({
        accessToken: newAccessToken,
        refreshToken: newRefreshToken,
        expiresIn: 3600,
      });
    });

    it('should handle refresh token expired', async () => {
      const mockRefreshToken = 'expired_refresh_token';

      (secureTokenStorage.getRefreshToken as jest.Mock).mockResolvedValue(
        mockRefreshToken
      );

      (global.fetch as jest.Mock).mockResolvedValue({
        ok: false,
        status: 401,
      });

      const mockStore = {
        tokens: { accessToken: 'token', refreshToken: mockRefreshToken, expiresIn: 3600 },
        clearAuth: jest.fn(),
      };
      (useAuthStore.getState as jest.Mock).mockReturnValue(mockStore);

      await expect(tokenManager.requestTokenRefresh()).rejects.toThrow(
        'Refresh token expired'
      );

      expect(mockStore.clearAuth).toHaveBeenCalled();
      expect(secureTokenStorage.clear).toHaveBeenCalled();
    });

    it('should handle no refresh token available', async () => {
      (secureTokenStorage.getRefreshToken as jest.Mock).mockResolvedValue(null);

      const mockStore = {
        tokens: { accessToken: 'token', refreshToken: null, expiresIn: 3600 },
        clearAuth: jest.fn(),
      };
      (useAuthStore.getState as jest.Mock).mockReturnValue(mockStore);

      await expect(tokenManager.requestTokenRefresh()).rejects.toThrow(
        'No refresh token available'
      );
    });

    it('should debounce concurrent refresh requests', async () => {
      const mockRefreshToken = 'mock_refresh_token';
      const newAccessToken = 'new_access_token';

      (secureTokenStorage.getRefreshToken as jest.Mock).mockResolvedValue(
        mockRefreshToken
      );

      (global.fetch as jest.Mock).mockResolvedValue({
        ok: true,
        json: jest.fn().mockResolvedValue({
          accessToken: newAccessToken,
          refreshToken: mockRefreshToken,
          expiresIn: 3600,
        }),
      });

      const mockStore = {
        tokens: { accessToken: 'old', refreshToken: mockRefreshToken, expiresIn: 3600 },
        setTokens: jest.fn(),
        clearAuth: jest.fn(),
      };
      (useAuthStore.getState as jest.Mock).mockReturnValue(mockStore);

      // Make concurrent requests
      const promise1 = tokenManager.requestTokenRefresh();
      const promise2 = tokenManager.requestTokenRefresh();

      const [token1, token2] = await Promise.all([promise1, promise2]);

      expect(token1).toBe(newAccessToken);
      expect(token2).toBe(newAccessToken);
      // fetch should only be called once due to debouncing
      expect(global.fetch).toHaveBeenCalledTimes(1);
    });
  });

  describe('clearSchedule', () => {
    it('should clear refresh timeout', () => {
      const mockStore = {
        tokens: {
          accessToken: 'token',
          refreshToken: 'refresh',
          expiresIn: 3600,
        },
      };
      (useAuthStore.getState as jest.Mock).mockReturnValue(mockStore);

      tokenManager.scheduleTokenRefresh();
      tokenManager.clearSchedule();

      expect(clearTimeout).toHaveBeenCalled();
    });
  });

  describe('isTokenExpired', () => {
    it('should return true if token expires in less than 5 minutes', () => {
      const mockStore = {
        tokens: {
          accessToken: 'token',
          refreshToken: 'refresh',
          expiresIn: 200, // Less than 5 minutes (300 seconds)
        },
      };
      (useAuthStore.getState as jest.Mock).mockReturnValue(mockStore);

      const isExpired = tokenManager.isTokenExpired();

      expect(isExpired).toBe(true);
    });

    it('should return false if token has more than 5 minutes', () => {
      const mockStore = {
        tokens: {
          accessToken: 'token',
          refreshToken: 'refresh',
          expiresIn: 600, // More than 5 minutes
        },
      };
      (useAuthStore.getState as jest.Mock).mockReturnValue(mockStore);

      const isExpired = tokenManager.isTokenExpired();

      expect(isExpired).toBe(false);
    });

    it('should return true if no tokens', () => {
      const mockStore = {
        tokens: null,
      };
      (useAuthStore.getState as jest.Mock).mockReturnValue(mockStore);

      const isExpired = tokenManager.isTokenExpired();

      expect(isExpired).toBe(true);
    });
  });
});
