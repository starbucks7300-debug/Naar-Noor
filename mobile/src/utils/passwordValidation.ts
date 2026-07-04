import { PasswordValidation } from '@types/auth.types';

/**
 * Validate password complexity
 */
export const validatePassword = (password: string): PasswordValidation => {
  const minLength = password.length >= 8;
  const hasUppercase = /[A-Z]/.test(password);
  const hasLowercase = /[a-z]/.test(password);
  const hasNumber = /\d/.test(password);
  const hasSpecialChar = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password);

  const errors: string[] = [];
  if (!minLength) errors.push('Password must be at least 8 characters');
  if (!hasUppercase) errors.push('Password must contain an uppercase letter');
  if (!hasLowercase) errors.push('Password must contain a lowercase letter');
  if (!hasNumber) errors.push('Password must contain a number');
  if (!hasSpecialChar) errors.push('Password must contain a special character');

  return {
    valid: minLength && hasUppercase && hasLowercase && hasNumber && hasSpecialChar,
    minLength,
    hasUppercase,
    hasLowercase,
    hasNumber,
    hasSpecialChar,
    errors,
  };
};

/**
 * Get password strength score (0-5)
 */
export const getPasswordStrength = (password: string): number => {
  if (!password) return 0;
  
  const validation = validatePassword(password);
  const checks = [
    validation.minLength,
    validation.hasUppercase,
    validation.hasLowercase,
    validation.hasNumber,
    validation.hasSpecialChar,
  ].filter(Boolean).length;

  return checks;
};

/**
 * Get password strength label
 */
export const getPasswordStrengthLabel = (password: string): string => {
  const strength = getPasswordStrength(password);
  
  switch (strength) {
    case 0:
    case 1:
      return 'Weak';
    case 2:
    case 3:
      return 'Fair';
    case 4:
      return 'Good';
    case 5:
      return 'Strong';
    default:
      return 'Unknown';
  }
};

/**
 * Get password strength color
 */
export const getPasswordStrengthColor = (password: string): string => {
  const strength = getPasswordStrength(password);
  
  switch (strength) {
    case 0:
    case 1:
      return '#FF6B6B'; // Red
    case 2:
    case 3:
      return '#FFA500'; // Orange
    case 4:
      return '#FFD700'; // Yellow
    case 5:
      return '#4CAF50'; // Green
    default:
      return '#CCCCCC'; // Gray
  }
};
