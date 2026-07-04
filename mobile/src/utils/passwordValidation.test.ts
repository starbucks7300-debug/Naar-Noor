import { validatePassword, getPasswordStrength, getPasswordStrengthLabel } from './passwordValidation';

describe('Password Validation', () => {
  describe('validatePassword', () => {
    it('should validate strong passwords', () => {
      const result = validatePassword('SecurePass123!');
      expect(result.valid).toBe(true);
      expect(result.errors).toHaveLength(0);
    });

    it('should reject password less than 8 characters', () => {
      const result = validatePassword('Short1!');
      expect(result.valid).toBe(false);
      expect(result.minLength).toBe(false);
      expect(result.errors).toContain('Password must be at least 8 characters');
    });

    it('should reject password without uppercase', () => {
      const result = validatePassword('lowercase123!');
      expect(result.valid).toBe(false);
      expect(result.hasUppercase).toBe(false);
    });

    it('should reject password without lowercase', () => {
      const result = validatePassword('UPPERCASE123!');
      expect(result.valid).toBe(false);
      expect(result.hasLowercase).toBe(false);
    });

    it('should reject password without number', () => {
      const result = validatePassword('NoNumbers!');
      expect(result.valid).toBe(false);
      expect(result.hasNumber).toBe(false);
    });

    it('should reject password without special character', () => {
      const result = validatePassword('NoSpecial123');
      expect(result.valid).toBe(false);
      expect(result.hasSpecialChar).toBe(false);
    });
  });

  describe('getPasswordStrength', () => {
    it('should return 0 for empty password', () => {
      expect(getPasswordStrength('')).toBe(0);
    });

    it('should return 3 for weak password', () => {
      // "Weak12" has: length (5 - fails), uppercase W (passes), lowercase (passes), number (passes), special char (fails)
      expect(getPasswordStrength('Weak12')).toBe(3);
    });

    it('should return 5 for strong password', () => {
      expect(getPasswordStrength('StrongPass123!')).toBe(5);
    });
  });

  describe('getPasswordStrengthLabel', () => {
    it('should return "Weak" for weak passwords', () => {
      expect(getPasswordStrengthLabel('weak')).toBe('Weak');
    });

    it('should return "Strong" for strong passwords', () => {
      expect(getPasswordStrengthLabel('StrongPass123!')).toBe('Strong');
    });
  });
});
