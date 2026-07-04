import React, { ReactElement } from 'react';
import { renderHook as rtlRenderHook, waitFor } from '@testing-library/react-native';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';

/**
 * Custom renderHook that wraps hooks with necessary providers
 */
export function renderHook<TProps, TResult>(
  hook: (props: TProps) => TResult,
  options?: {
    initialProps?: TProps;
    client?: QueryClient;
  },
) {
  const client = options?.client ?? new QueryClient({
    defaultOptions: {
      queries: {
        retry: false,
      },
      mutations: {
        retry: false,
      },
    },
  });

  function Wrapper({ children }: { children: React.ReactNode }) {
    return (
      <QueryClientProvider client={client}>
        {children}
      </QueryClientProvider>
    );
  }

  return rtlRenderHook(hook, { wrapper: Wrapper, initialProps: options?.initialProps });
}

/**
 * Custom render function that wraps components with necessary providers
 */
export function renderWithProviders(
  ui: ReactElement,
  {
    preloadedState = {},
    // Automatically create a new instance of QueryClient for each test
    client = new QueryClient({
      defaultOptions: {
        queries: {
          retry: false,
        },
        mutations: {
          retry: false,
        },
      },
    }),
    ...renderOptions
  }: {
    preloadedState?: any;
    client?: QueryClient;
  } = {},
) {
  function Wrapper({ children }: { children: React.ReactNode }) {
    return (
      <QueryClientProvider client={client}>
        {children}
      </QueryClientProvider>
    );
  }

  const render = require('@testing-library/react-native').render;
  return {
    client,
    ...render(ui, { wrapper: Wrapper, ...renderOptions }),
  };
}

// Re-export everything from React Testing Library
export * from '@testing-library/react-native';
export { waitFor };
