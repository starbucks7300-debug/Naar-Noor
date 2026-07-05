import { apiClient } from './client';
import {
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  RegisterResponse,
  PasswordResetRequest,
  PasswordResetConfirm,
} from '@/types/auth.types';

/**
 * User registration
 */
export const registerUser = async (data: RegisterRequest): Promise<RegisterResponse> => {
  const response = await apiClient.post<RegisterResponse>('/api/auth/register', data);
  return response.data;
};

/**
 * User login
 */
export const loginUser = async (data: LoginRequest): Promise<LoginResponse> => {
  const response = await apiClient.post<LoginResponse>('/api/auth/login', data);
  return response.data;
};

/**
 * Refresh access token
 */
export const refreshAccessToken = async (refreshToken: string): Promise<{ accessToken: string; expiresIn: number }> => {
  const response = await apiClient.post('/api/auth/refresh', { refreshToken });
  return response.data;
};

/**
 * Logout user
 */
export const logoutUser = async (): Promise<void> => {
  await apiClient.post('/api/auth/logout');
};

/**
 * Get current user profile
 */
export const getCurrentUser = async () => {
  const response = await apiClient.get('/api/auth/me');
  return response.data;
};

/**
 * Request password reset
 */
export const requestPasswordReset = async (data: PasswordResetRequest): Promise<void> => {
  await apiClient.post('/api/auth/password-reset-request', data);
};

/**
 * Confirm password reset
 */
export const confirmPasswordReset = async (data: PasswordResetConfirm): Promise<void> => {
  await apiClient.post('/api/auth/password-reset-confirm', data);
};

/**
 * Check if email is available
 */
export const checkEmailAvailability = async (email: string): Promise<{ available: boolean }> => {
  const response = await apiClient.post('/api/auth/check-email', { email });
  return response.data;
};

/**
 * Verify email token
 */
export const verifyEmailToken = async (token: string): Promise<void> => {
  await apiClient.post('/api/auth/verify-email', { token });
};

/**
 * Resend verification email
 */
export const resendVerificationEmail = async (email: string): Promise<void> => {
  await apiClient.post('/api/auth/resend-verification', { email });
};
