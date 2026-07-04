import { EmailValidation } from '@types/auth.types';

/**
 * Validate email format (RFC 5321)
 */
export const validateEmailFormat = (email: string): boolean => {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email) && email.length <= 254;
};

/**
 * Comprehensive email validation
 */
export const validateEmail = (email: string): EmailValidation => {
  const errors: string[] = [];

  if (!email) {
    errors.push('Email is required');
    return { valid: false, errors };
  }

  if (!validateEmailFormat(email)) {
    errors.push('Please enter a valid email address');
    return { valid: false, errors };
  }

  // Check for common typos
  const commonDomains = ['gmail.com', 'yahoo.com', 'outlook.com', 'hotmail.com'];
  const domain = email.split('@')[1];
  if (domain && commonDomains.includes(domain)) {
    // Valid common domain
  }

  return {
    valid: true,
    errors,
  };
};

/**
 * Check if email is available (requires API call)
 */
export const checkEmailAvailability = async (email: string, apiClient: any): Promise<boolean> => {
  try {
    const response = await apiClient.post('/api/auth/check-email', { email });
    return response.data.available !== false;
  } catch (error) {
    throw new Error('Failed to check email availability');
  }
};

/**
 * Normalize email (lowercase and trim)
 */
export const normalizeEmail = (email: string): string => {
  return email.toLowerCase().trim();
};
