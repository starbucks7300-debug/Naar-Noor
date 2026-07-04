import { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { loginUser } from '@services/api/authApi';
import { useAuthStore } from '@stores/useAuthStore';
import { secureTokenStorage } from '@services/storage/secureTokenStorage';
import { tokenManager } from '@services/api/tokenManager';
import type { LoginRequest, AuthError } from '../types/auth.types';
import { validateEmail } from '@utils/emailValidation';

interface LoginForm {
  email: string;
  password: string;
}

interface FormErrors {
  email?: string;
  password?: string;
  general?: string;
}

export const useLogin = () => {
  const [formErrors, setFormErrors] = useState<FormErrors>({});
  const [remainingAttempts, setRemainingAttempts] = useState(5);
  const [isLockedOut, setIsLockedOut] = useState(false);
  const [lockoutTime, setLockoutTime] = useState<Date | null>(null);

  const setAuthState = useAuthStore((state: any) => state.setAuthState);

  const mutation = useMutation({
    mutationFn: async (data: LoginRequest) => {
      return await loginUser(data);
    },
    onSuccess: async (data: any) => {
      const tokenPair = {
        accessToken: data.accessToken,
        refreshToken: data.refreshToken,
        expiresIn: data.expiresIn,
      };

      // Store tokens in secure storage
      await secureTokenStorage.setAccessToken(tokenPair.accessToken);
      await secureTokenStorage.setRefreshToken(tokenPair.refreshToken);

      // Update auth store
      setAuthState(data.user, tokenPair);

      // Schedule token refresh
      tokenManager.scheduleTokenRefresh();

      setFormErrors({});
      setRemainingAttempts(5);
      setIsLockedOut(false);
    },
    onError: (error: any) => {
      const errorResponse = error.response?.data as AuthError;
      const status = error.response?.status;

      // Handle account lockout
      if (status === 429) {
        setIsLockedOut(true);
        setLockoutTime(new Date(Date.now() + 15 * 60 * 1000)); // 15 minutes
        setFormErrors({
          general: 'Account locked. Please try again in 15 minutes or reset your password.',
        });
      } else if (status === 401) {
        // Invalid credentials
        const attempts = remainingAttempts - 1;
        setRemainingAttempts(Math.max(0, attempts));

        if (attempts === 0) {
          setIsLockedOut(true);
          setLockoutTime(new Date(Date.now() + 15 * 60 * 1000));
          setFormErrors({
            general: 'Account locked due to too many failed attempts. Try again in 15 minutes.',
          });
        } else {
          setFormErrors({
            general: `Invalid email or password. ${attempts} attempts remaining.`,
          });
        }
      } else {
        setFormErrors({
          general: errorResponse?.message || 'Login failed. Please try again.',
        });
      }
    },
  });

  const validateForm = (form: LoginForm): boolean => {
    const errors: FormErrors = {};

    // Email validation
    const emailValidation = validateEmail(form.email);
    if (!emailValidation.valid) {
      errors.email = emailValidation.errors[0];
    }

    // Password
    if (!form.password || form.password.length === 0) {
      errors.password = 'Password is required';
    }

    setFormErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const login = (form: LoginForm) => {
    if (isLockedOut) {
      return;
    }

    if (!validateForm(form)) {
      return;
    }

    mutation.mutate({
      email: form.email,
      password: form.password,
    });
  };

  const getLockoutRemainingTime = (): number => {
    if (!lockoutTime) return 0;
    const now = new Date();
    const diff = lockoutTime.getTime() - now.getTime();
    return Math.max(0, Math.ceil(diff / 1000));
  };

  return {
    login,
    isLoading: mutation.isPending,
    isSuccess: mutation.isSuccess,
    formErrors,
    error: mutation.error,
    remainingAttempts,
    isLockedOut,
    lockoutRemainingTime: getLockoutRemainingTime(),
  };
};
