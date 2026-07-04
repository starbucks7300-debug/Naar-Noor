import axios, { AxiosInstance, AxiosError } from 'axios';
import { useAuthStore } from '@stores/useAuthStore';
import { secureTokenStorage } from '@services/storage/secureTokenStorage';
import { tokenManager } from './tokenManager';
import { logger } from '@services/logger';

const API_BASE_URL = process.env.EXPO_PUBLIC_API_URL || 'http://localhost:5000/api';
const API_TIMEOUT = parseInt(process.env.EXPO_PUBLIC_API_TIMEOUT || '30000');

export const apiClient: AxiosInstance = axios.create({
  baseURL: API_BASE_URL,
  timeout: API_TIMEOUT,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request Interceptor - Inject JWT Token
apiClient.interceptors.request.use(
  async (config) => {
    try {
      const token = await secureTokenStorage.getAccessToken();
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
    } catch (error) {
      logger.error('Error retrieving token', error as Error);
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response Interceptor - Handle Errors & Token Refresh
apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as any;

    // Handle 401 Unauthorized - attempt token refresh
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      logger.warn('Received 401, attempting token refresh');

      try {
        const newAccessToken = await tokenManager.requestTokenRefresh();
        
        // Retry original request with new token
        originalRequest.headers.Authorization = `Bearer ${newAccessToken}`;
        return apiClient(originalRequest);
      } catch (refreshError) {
        logger.error('Token refresh failed, user logged out', refreshError as Error);
        // Refresh failed - logout user
        useAuthStore.getState().clearAuth();
        await secureTokenStorage.clear();
        tokenManager.clearSchedule();
      }
    }

    return Promise.reject(error);
  }
);

export default apiClient;
