/**
 * Centralized logging service with support for different log levels
 * Handles console logging, file logging (for mobile), and structured logging
 */

export enum LogLevel {
  DEBUG = 'DEBUG',
  INFO = 'INFO',
  WARN = 'WARN',
  ERROR = 'ERROR',
}

interface LogEntry {
  timestamp: string;
  level: LogLevel;
  message: string;
  data?: Record<string, any>;
  error?: Error;
}

class Logger {
  private logLevel: LogLevel = LogLevel.INFO;
  private logs: LogEntry[] = [];
  private maxLogs: number = 1000;

  constructor() {
    // Set log level based on environment
    const env = process.env.NODE_ENV || 'development';
    this.logLevel = env === 'development' ? LogLevel.DEBUG : LogLevel.INFO;
  }

  /**
   * Set the minimum log level
   */
  setLogLevel(level: LogLevel): void {
    this.logLevel = level;
  }

  /**
   * Debug level logging
   */
  debug(message: string, data?: Record<string, any>): void {
    this.log(LogLevel.DEBUG, message, data);
  }

  /**
   * Info level logging
   */
  info(message: string, data?: Record<string, any>): void {
    this.log(LogLevel.INFO, message, data);
  }

  /**
   * Warning level logging
   */
  warn(message: string, data?: Record<string, any>): void {
    this.log(LogLevel.WARN, message, data);
  }

  /**
   * Error level logging
   */
  error(message: string, error?: Error | Record<string, any>, data?: Record<string, any>): void {
    if (error instanceof Error) {
      this.log(LogLevel.ERROR, message, { ...data, errorMessage: error.message, stack: error.stack }, error);
    } else {
      this.log(LogLevel.ERROR, message, { ...error, ...data });
    }
  }

  /**
   * Main logging method
   */
  private log(level: LogLevel, message: string, data?: Record<string, any>, error?: Error): void {
    // Check if this log level should be logged
    if (!this.shouldLog(level)) {
      return;
    }

    const logEntry: LogEntry = {
      timestamp: new Date().toISOString(),
      level,
      message,
      data,
      error,
    };

    // Add to in-memory log storage
    this.logs.push(logEntry);
    if (this.logs.length > this.maxLogs) {
      this.logs.shift();
    }

    // Output to console
    this.outputToConsole(logEntry);

    // In production, send to error tracking service
    if (level === LogLevel.ERROR && process.env.NODE_ENV === 'production') {
      this.sendToErrorTracking(logEntry);
    }
  }

  /**
   * Check if log level should be logged
   */
  private shouldLog(level: LogLevel): boolean {
    const levels = [LogLevel.DEBUG, LogLevel.INFO, LogLevel.WARN, LogLevel.ERROR];
    const currentLevelIndex = levels.indexOf(this.logLevel);
    const messageLevelIndex = levels.indexOf(level);
    return messageLevelIndex >= currentLevelIndex;
  }

  /**
   * Output log to console with formatting
   */
  private outputToConsole(entry: LogEntry): void {
    const prefix = `[${entry.timestamp}] [${entry.level}]`;

    switch (entry.level) {
      case LogLevel.DEBUG:
        console.log(`${prefix} ${entry.message}`, entry.data || '');
        break;
      case LogLevel.INFO:
        console.log(`${prefix} ${entry.message}`, entry.data || '');
        break;
      case LogLevel.WARN:
        console.warn(`${prefix} ${entry.message}`, entry.data || '');
        break;
      case LogLevel.ERROR:
        console.error(`${prefix} ${entry.message}`, entry.data || '');
        if (entry.error) {
          console.error(entry.error);
        }
        break;
    }
  }

  /**
   * Send error to error tracking service (Sentry, etc.)
   */
  private sendToErrorTracking(entry: LogEntry): void {
    // This will be implemented when integrating with Sentry
    // For now, just a placeholder
    try {
      // await captureException(entry.error, { extra: entry.data });
    } catch (err) {
      // Fail silently to prevent logging errors from breaking the app
    }
  }

  /**
   * Get all stored logs
   */
  getLogs(): LogEntry[] {
    return [...this.logs];
  }

  /**
   * Clear all stored logs
   */
  clearLogs(): void {
    this.logs = [];
  }

  /**
   * Get logs filtered by level
   */
  getLogsByLevel(level: LogLevel): LogEntry[] {
    return this.logs.filter((log) => log.level === level);
  }

  /**
   * Export logs as JSON
   */
  exportLogs(): string {
    return JSON.stringify(this.logs, null, 2);
  }
}

// Export singleton instance
export const logger = new Logger();
