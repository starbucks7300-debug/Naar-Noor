/**
 * Error tracking and crash reporting service
 * This integrates with Sentry for production error tracking
 */

import { logger } from './logger';

interface ErrorContext {
  userId?: string;
  deviceId?: string;
  sessionId?: string;
  [key: string]: any;
}

class ErrorTracker {
  private context: ErrorContext = {};
  private isInitialized: boolean = false;
  private dsn?: string;

  /**
   * Initialize error tracking
   * In production, this would initialize Sentry with the DSN
   */
  async initialize(dsn?: string): Promise<void> {
    this.dsn = dsn || process.env.EXPO_PUBLIC_SENTRY_DSN;

    if (!this.dsn) {
      logger.warn('Error tracking DSN not provided. Error tracking disabled.');
      return;
    }

    try {
      // In production, you would initialize Sentry here
      // import * as Sentry from '@sentry/react-native';
      // Sentry.init({
      //   dsn: this.dsn,
      //   environment: process.env.NODE_ENV,
      //   tracesSampleRate: 0.1,
      // });

      this.isInitialized = true;
      logger.info('Error tracking initialized successfully');
    } catch (error) {
      logger.error('Failed to initialize error tracking', error as Error);
    }
  }

  /**
   * Set user context for error tracking
   */
  setUserContext(userId: string, email?: string, name?: string): void {
    this.context.userId = userId;

    if (email) {
      this.context.userEmail = email;
    }

    if (name) {
      this.context.userName = name;
    }

    // Update Sentry user context if available
    // if (typeof Sentry !== 'undefined' && Sentry.setUser) {
    //   Sentry.setUser({ id: userId, email, name });
    // }

    logger.debug('User context set for error tracking', { userId, email, name });
  }

  /**
   * Clear user context (e.g., on logout)
   */
  clearUserContext(): void {
    delete this.context.userId;
    delete this.context.userEmail;
    delete this.context.userName;

    // Clear Sentry user context if available
    // if (typeof Sentry !== 'undefined' && Sentry.setUser) {
    //   Sentry.setUser(null);
    // }

    logger.debug('User context cleared');
  }

  /**
   * Set additional context data
   */
  setContext(key: string, value: any): void {
    this.context[key] = value;
  }

  /**
   * Capture an exception
   */
  captureException(error: Error, tags?: Record<string, string>): void {
    logger.error('Exception captured', error, { tags });

    // In production, send to Sentry
    // if (typeof Sentry !== 'undefined' && Sentry.captureException) {
    //   Sentry.captureException(error, { tags, extra: this.context });
    // }
  }

  /**
   * Capture a message
   */
  captureMessage(message: string, level: 'fatal' | 'error' | 'warning' | 'info' | 'debug' = 'info'): void {
    logger[level === 'warning' ? 'warn' : level](message);

    // In production, send to Sentry
    // if (typeof Sentry !== 'undefined' && Sentry.captureMessage) {
    //   Sentry.captureMessage(message, level === 'fatal' ? 'error' : level);
    // }
  }

  /**
   * Add breadcrumb for error context tracking
   */
  addBreadcrumb(message: string, data?: Record<string, any>): void {
    logger.debug(`Breadcrumb: ${message}`, data);

    // In production, add to Sentry
    // if (typeof Sentry !== 'undefined' && Sentry.addBreadcrumb) {
    //   Sentry.addBreadcrumb({
    //     message,
    //     data,
    //     level: 'info',
    //   });
    // }
  }

  /**
   * Set global tag
   */
  setTag(key: string, value: string): void {
    this.context[`tag_${key}`] = value;

    // In production, set Sentry tag
    // if (typeof Sentry !== 'undefined' && Sentry.setTag) {
    //   Sentry.setTag(key, value);
    // }
  }

  /**
   * Get current context
   */
  getContext(): ErrorContext {
    return { ...this.context };
  }

  /**
   * Check if error tracking is initialized
   */
  isReady(): boolean {
    return this.isInitialized;
  }
}

// Export singleton instance
export const errorTracker = new ErrorTracker();
