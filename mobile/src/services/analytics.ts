/**
 * Analytics and event tracking service
 * Integrates with Firebase Analytics, Mixpanel, or similar
 */

import { logger } from './logger';

interface AnalyticsEvent {
  name: string;
  params?: Record<string, any>;
  timestamp: string;
}

class Analytics {
  private isInitialized: boolean = false;
  private eventQueue: AnalyticsEvent[] = [];
  private userId?: string;
  private sessionId: string;

  constructor() {
    this.sessionId = this.generateSessionId();
  }

  /**
   * Initialize analytics
   */
  async initialize(): Promise<void> {
    try {
      // In production, initialize with Firebase Analytics or Mixpanel
      // import analytics from '@react-native-firebase/analytics';
      // await analytics().setAnalyticsCollectionEnabled(true);

      this.isInitialized = true;
      logger.info('Analytics initialized successfully');

      // Log app launch event
      this.trackEvent('app_launched', {
        sessionId: this.sessionId,
        timestamp: new Date().toISOString(),
      });
    } catch (error) {
      logger.error('Failed to initialize analytics', error as Error);
    }
  }

  /**
   * Set user ID for analytics
   */
  async setUserId(userId: string): Promise<void> {
    this.userId = userId;

    try {
      // In production, set user ID with analytics provider
      // import analytics from '@react-native-firebase/analytics';
      // await analytics().setUserId(userId);

      logger.debug('Analytics user ID set', { userId });
    } catch (error) {
      logger.error('Failed to set analytics user ID', error as Error);
    }
  }

  /**
   * Clear user ID (e.g., on logout)
   */
  async clearUserId(): Promise<void> {
    this.userId = undefined;

    try {
      // In production, clear user ID with analytics provider
      // import analytics from '@react-native-firebase/analytics';
      // await analytics().setUserId(null);

      logger.debug('Analytics user ID cleared');
    } catch (error) {
      logger.error('Failed to clear analytics user ID', error as Error);
    }
  }

  /**
   * Track a custom event
   */
  async trackEvent(eventName: string, params?: Record<string, any>): Promise<void> {
    const event: AnalyticsEvent = {
      name: eventName,
      params: {
        ...params,
        sessionId: this.sessionId,
        userId: this.userId,
      },
      timestamp: new Date().toISOString(),
    };

    // Add to queue
    this.eventQueue.push(event);

    // Log the event
    logger.debug(`Analytics event tracked: ${eventName}`, event.params);

    try {
      // In production, send to analytics provider
      // import analytics from '@react-native-firebase/analytics';
      // await analytics().logEvent(eventName, event.params);

      // For now, just queue the event
    } catch (error) {
      logger.error(`Failed to track analytics event: ${eventName}`, error as Error);
    }
  }

  /**
   * Track a screen view
   */
  async trackScreenView(screenName: string, params?: Record<string, any>): Promise<void> {
    try {
      // In production, track screen view with analytics provider
      // import analytics from '@react-native-firebase/analytics';
      // await analytics().logScreenView({
      //   screen_name: screenName,
      //   screen_class: screenName,
      //   ...params,
      // });

      logger.debug('Analytics screen view tracked', { screenName, ...params });
    } catch (error) {
      logger.error('Failed to track analytics screen view', error as Error);
    }
  }

  /**
   * Track a user action event
   */
  async trackUserAction(action: string, category: string, params?: Record<string, any>): Promise<void> {
    await this.trackEvent(action, {
      category,
      ...params,
    });
  }

  /**
   * Track an error event
   */
  async trackError(error: Error, errorCategory: string = 'error'): Promise<void> {
    await this.trackEvent('app_error', {
      error_message: error.message,
      error_stack: error.stack,
      error_category: errorCategory,
    });
  }

  /**
   * Get event queue
   */
  getEventQueue(): AnalyticsEvent[] {
    return [...this.eventQueue];
  }

  /**
   * Clear event queue
   */
  clearEventQueue(): void {
    this.eventQueue = [];
  }

  /**
   * Get session ID
   */
  getSessionId(): string {
    return this.sessionId;
  }

  /**
   * Generate unique session ID
   */
  private generateSessionId(): string {
    return `session-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
  }

  /**
   * Check if analytics is initialized
   */
  isReady(): boolean {
    return this.isInitialized;
  }
}

// Export singleton instance
export const analytics = new Analytics();
