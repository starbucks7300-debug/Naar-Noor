import { validateEmail, validateEmailFormat, normalizeEmail } from './emailValidation';

describe('Email Validation', () => {
  describe('validateEmailFormat', () => {
    it('should validate correct email addresses', () => {
      expect(validateEmailFormat('user@example.com')).toBe(true);
      expect(validateEmailFormat('test.user@domain.co.uk')).toBe(true);
      expect(validateEmailFormat('name+tag@example.com')).toBe(true);
    });

    it('should reject invalid emails', () => {
      expect(validateEmailFormat('invalid.email')).toBe(false);
      expect(validateEmailFormat('@example.com')).toBe(false);
      expect(validateEmailFormat('user@')).toBe(false);
      expect(validateEmailFormat('user name@example.com')).toBe(false);
    });
  });

  describe('validateEmail', () => {
    it('should validate correct emails', () => {
      const result = validateEmail('user@example.com');
      expect(result.valid).toBe(true);
      expect(result.errors).toHaveLength(0);
    });

    it('should reject invalid emails', () => {
      const result = validateEmail('invalid.email');
      expect(result.valid).toBe(false);
      expect(result.errors.length).toBeGreaterThan(0);
    });

    it('should require email', () => {
      const result = validateEmail('');
      expect(result.valid).toBe(false);
      expect(result.errors).toContain('Email is required');
    });
  });

  describe('normalizeEmail', () => {
    it('should convert to lowercase', () => {
      expect(normalizeEmail('User@Example.COM')).toBe('user@example.com');
    });

    it('should trim whitespace', () => {
      expect(normalizeEmail('  user@example.com  ')).toBe('user@example.com');
    });
  });
});
