import { renderHook, waitFor } from '@tests/utils/testUtils';
import { useInitializeAuth } from './useInitializeAuth';
import { useAuthStore } from '@stores/useAuthStore';
import { secureTokenStorage } from '@services/storage/secureTokenStorage';
import { tokenManager } from '@services/api/tokenManager';
import { logger } from '@services/logger';

jest.mock('@services/storage/secureTokenStorage');
jest.mock('@services/api/tokenManager');
jest.mock('@services/logger');

describe('useInitializeAuth', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  it('should restore tokens from secure storage on init', async () => {
    const mockAccessToken = 'mock_access_token';
    const mockRefreshToken = 'mock_refresh_token';

    (secureTokenStorage.getAccessToken as jest.Mock).mockResolvedValue(
      mockAccessToken
    );
    (secureTokenStorage.getRefreshToken as jest.Mock).mockResolvedValue(
      mockRefreshToken
    );

    const { result } = renderHook(() => useInitializeAuth());

    await waitFor(() => {
      expect(secureTokenStorage.getAccessToken).toHaveBeenCalled();
      expect(secureTokenStorage.getRefreshToken).toHaveBeenCalled();
    });

    // Verify tokens were restored in store
    const store = useAuthStore.getState();
    expect(store.tokens?.accessToken).toBe(mockAccessToken);
    expect(store.tokens?.refreshToken).toBe(mockRefreshToken);
  });

  it('should schedule token refresh after restoring tokens', async () => {
    const mockAccessToken = 'mock_access_token';
    const mockRefreshToken = 'mock_refresh_token';

    (secureTokenStorage.getAccessToken as jest.Mock).mockResolvedValue(
      mockAccessToken
    );
    (secureTokenStorage.getRefreshToken as jest.Mock).mockResolvedValue(
      mockRefreshToken
    );

    renderHook(() => useInitializeAuth());

    await waitFor(() => {
      expect(tokenManager.scheduleTokenRefresh).toHaveBeenCalled();
    });
  });

  it('should handle missing tokens gracefully', async () => {
    (secureTokenStorage.getAccessToken as jest.Mock).mockResolvedValue(null);
    (secureTokenStorage.getRefreshToken as jest.Mock).mockResolvedValue(null);

    renderHook(() => useInitializeAuth());

    await waitFor(() => {
      expect(secureTokenStorage.getAccessToken).toHaveBeenCalled();
      expect(secureTokenStorage.getRefreshToken).toHaveBeenCalled();
    });

    // Token refresh should not be scheduled
    expect(tokenManager.scheduleTokenRefresh).not.toHaveBeenCalled();
  });

  it('should handle errors during initialization', async () => {
    const error = new Error('Storage error');
    (secureTokenStorage.getAccessToken as jest.Mock).mockRejectedValue(error);

    renderHook(() => useInitializeAuth());

    await waitFor(() => {
      expect(logger.error).toHaveBeenCalledWith(
        'Error initializing auth',
        expect.any(Error)
      );
    });
  });

  it('should set loading to false after init', async () => {
    (secureTokenStorage.getAccessToken as jest.Mock).mockResolvedValue(null);
    (secureTokenStorage.getRefreshToken as jest.Mock).mockResolvedValue(null);

    renderHook(() => useInitializeAuth());

    await waitFor(() => {
      const store = useAuthStore.getState();
      expect(store.isLoading).toBe(false);
    });
  });

  it('should return authenticated status', () => {
    const { result } = renderHook(() => useInitializeAuth());

    expect(result.current.isAuthenticated).toBeDefined();
  });
});
