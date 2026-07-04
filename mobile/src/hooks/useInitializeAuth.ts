import { useEffect } from 'react';
import { useAuthStore } from '@stores/useAuthStore';
import { secureTokenStorage } from '@services/storage/secureTokenStorage';
import { tokenManager } from '@services/api/tokenManager';
import { logger } from '@services/logger';

/**
 * Hook to initialize authentication on app startup
 * Restores tokens from secure storage and schedules token refresh
 */
export const useInitializeAuth = () => {
  const setTokens = useAuthStore((state: any) => state.setTokens);
  const setLoading = useAuthStore((state: any) => state.setLoading);
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated);

  useEffect(() => {
    const initializeAuth = async () => {
      try {
        const accessToken = await secureTokenStorage.getAccessToken();
        const refreshToken = await secureTokenStorage.getRefreshToken();

        if (accessToken && refreshToken) {
          logger.debug('Restoring tokens from secure storage');

          // Restore tokens in store
          // Note: expiresIn will be recalculated or fetched from backend on next refresh
          setTokens({
            accessToken,
            refreshToken,
            expiresIn: 3600, // Default 1 hour until next refresh
          });

          // Schedule token refresh
          tokenManager.scheduleTokenRefresh();
        }
      } catch (error) {
        logger.error('Error initializing auth', error as Error);
      } finally {
        setLoading(false);
      }
    };

    initializeAuth();
  }, [setTokens, setLoading]);

  return { isAuthenticated };
};
