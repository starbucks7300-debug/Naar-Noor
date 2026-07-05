import { useAuthStore } from '@stores/useAuthStore';
import { secureTokenStorage } from '@services/storage/secureTokenStorage';
import { logger } from '@services/logger';

class TokenManager {
  private refreshPromise: Promise<string> | null = null;
  private refreshTimeout: ReturnType<typeof setTimeout> | null = null;

  /**
   * Get current access token from secure storage
   */
  async getAccessToken(): Promise<string | null> {
    return await secureTokenStorage.getAccessToken();
  }

  /**
   * Get current refresh token from secure storage
   */
  async getRefreshToken(): Promise<string | null> {
    return await secureTokenStorage.getRefreshToken();
  }

  /**
   * Schedule token refresh (30 minutes before expiry)
   */
  scheduleTokenRefresh(): void {
    if (this.refreshTimeout) {
      clearTimeout(this.refreshTimeout);
    }

    const tokens = useAuthStore.getState().tokens;
    if (!tokens || !tokens.expiresIn) return;

    // Refresh 30 minutes before expiry
    const refreshIn = (tokens.expiresIn * 1000) - (30 * 60 * 1000);

    if (refreshIn > 0) {
      this.refreshTimeout = setTimeout(() => {
        this.requestTokenRefresh();
      }, refreshIn);

      logger.debug('Token refresh scheduled', { refreshIn, expiresIn: tokens.expiresIn });
    }
  }

  /**
   * Request token refresh (debounced)
   */
  async requestTokenRefresh(): Promise<string> {
    // If refresh is already in progress, return the promise
    if (this.refreshPromise) {
      return this.refreshPromise;
    }

    // Prevent duplicate refresh requests
    this.refreshPromise = this.performRefresh();

    try {
      const newToken = await this.refreshPromise;
      this.scheduleTokenRefresh();
      return newToken;
    } finally {
      this.refreshPromise = null;
    }
  }

  /**
   * Perform the actual token refresh
   */
  private async performRefresh(): Promise<string> {
    try {
      const refreshToken = await secureTokenStorage.getRefreshToken();
      if (!refreshToken) {
        throw new Error('No refresh token available');
      }

      const apiUrl = process.env.EXPO_PUBLIC_API_URL || 'http://localhost:5000/api';
      const response = await fetch(`${apiUrl}/auth/refresh`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ refreshToken }),
      });

      if (!response.ok) {
        if (response.status === 401) {
          // Refresh token expired, force logout
          useAuthStore.getState().clearAuth();
          throw new Error('Refresh token expired');
        }
        throw new Error(`Token refresh failed: ${response.statusText}`);
      }

      const data = await response.json();
      const newAccessToken = data.accessToken;
      const newRefreshToken = data.refreshToken || refreshToken;
      const expiresIn = data.expiresIn;

      // Update secure storage
      await secureTokenStorage.setAccessToken(newAccessToken);
      await secureTokenStorage.setRefreshToken(newRefreshToken);

      // Update store with new token pair
      useAuthStore.getState().setTokens({
        accessToken: newAccessToken,
        refreshToken: newRefreshToken,
        expiresIn: expiresIn,
      });

      logger.info('Token refreshed successfully');

      return newAccessToken;
    } catch (error) {
      logger.error('Token refresh failed', error as Error);
      useAuthStore.getState().clearAuth();
      await secureTokenStorage.clear();
      throw error;
    }
  }

  /**
   * Clear refresh schedule
   */
  clearSchedule(): void {
    if (this.refreshTimeout) {
      clearTimeout(this.refreshTimeout);
      this.refreshTimeout = null;
    }
  }

  /**
   * Check if token is expired
   */
  isTokenExpired(): boolean {
    const tokens = useAuthStore.getState().tokens;
    if (!tokens || !tokens.expiresIn) return true;

    // Token expires in less than 5 minutes
    return tokens.expiresIn < 300;
  }
}

export const tokenManager = new TokenManager();
