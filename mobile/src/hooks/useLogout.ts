import { useMutation, useQueryClient } from '@tanstack/react-query';
import { logoutUser } from '@services/api/authApi';
import { useAuthStore } from '@stores/useAuthStore';
import { secureTokenStorage } from '@services/storage/secureTokenStorage';
import { tokenManager } from '@services/api/tokenManager';
import { useRouter } from 'expo-router';
import { logger } from '@services/logger';

export const useLogout = () => {
  const router = useRouter();
  const clearAuth = useAuthStore((state: any) => state.clearAuth);
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: async () => {
      try {
        await logoutUser();
      } catch (error) {
        logger.error('Logout API call failed', error as Error);
        // Still proceed with local logout even if API fails
      }
    },
    onSuccess: async () => {
      // Clear token refresh schedule
      tokenManager.clearSchedule();

      // Clear secure token storage
      await secureTokenStorage.clear();

      // Clear auth state
      clearAuth();

      // Clear all queries
      queryClient.clear();

      // Redirect to login
      router.replace('/(auth)/login');

      logger.info('User logged out successfully');
    },
    onError: (error) => {
      logger.error('Logout failed', error as Error);
    },
  });

  return {
    logout: () => mutation.mutate(),
    isLoading: mutation.isPending,
    error: mutation.error,
  };
};
