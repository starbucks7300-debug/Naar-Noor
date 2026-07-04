import { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { registerUser } from '@services/api/authApi';
import { useAuthStore } from '@stores/useAuthStore';
import { RegisterRequest, AuthError } from '@types/auth.types';
import { validateEmail } from '@utils/emailValidation';
import { validatePassword } from '@utils/passwordValidation';

interface RegistrationForm {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
  phone: string;
}

interface FormErrors {
  email?: string;
  password?: string;
  confirmPassword?: string;
  firstName?: string;
  lastName?: string;
  phone?: string;
  general?: string;
}

export const useRegistration = () => {
  const [formErrors, setFormErrors] = useState<FormErrors>({});
  const setAuthState = useAuthStore((state) => state.setAuthState);

  const mutation = useMutation({
    mutationFn: async (data: RegisterRequest) => {
      return await registerUser(data);
    },
    onSuccess: (data) => {
      // Store user and tokens
      setAuthState(data.user, { accessToken: data.accessToken, refreshToken: data.refreshToken, expiresIn: data.expiresIn });
      setFormErrors({});
    },
    onError: (error: any) => {
      const errorResponse = error.response?.data as AuthError;
      setFormErrors({
        general: errorResponse?.message || 'Registration failed. Please try again.',
      });
    },
  });

  const validateForm = (form: RegistrationForm): boolean => {
    const errors: FormErrors = {};

    // Email validation
    const emailValidation = validateEmail(form.email);
    if (!emailValidation.valid) {
      errors.email = emailValidation.errors[0];
    }

    // Password validation
    const passwordValidation = validatePassword(form.password);
    if (!passwordValidation.valid) {
      errors.password = passwordValidation.errors[0];
    }

    // Confirm password
    if (form.password !== form.confirmPassword) {
      errors.confirmPassword = 'Passwords do not match';
    }

    // First name
    if (!form.firstName || form.firstName.trim().length < 2) {
      errors.firstName = 'First name must be at least 2 characters';
    }

    // Last name
    if (!form.lastName || form.lastName.trim().length < 2) {
      errors.lastName = 'Last name must be at least 2 characters';
    }

    // Phone
    const phoneRegex = /^\+?1?\d{9,15}$/;
    if (!form.phone || !phoneRegex.test(form.phone.replace(/\D/g, ''))) {
      errors.phone = 'Please enter a valid phone number';
    }

    setFormErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const register = (form: RegistrationForm) => {
    if (!validateForm(form)) {
      return;
    }

    mutation.mutate({
      email: form.email,
      password: form.password,
      firstName: form.firstName,
      lastName: form.lastName,
      phone: form.phone,
    });
  };

  return {
    register,
    isLoading: mutation.isPending,
    isSuccess: mutation.isSuccess,
    formErrors,
    error: mutation.error,
  };
};
