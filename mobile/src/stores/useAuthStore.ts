import { create } from 'zustand';
import { persist, createJSONStorage } from 'zustand/middleware';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { User, TokenPair } from '@/types/auth.types';

interface AuthStore {
  user: User | null;
  tokens: TokenPair | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;

  // Actions
  setUser: (user: User) => void;
  setTokens: (tokens: TokenPair) => void;
  setLoading: (loading: boolean) => void;
  setError: (error: string | null) => void;
  setAuthState: (user: User, tokens: TokenPair) => void;
  clearAuth: () => void;
  logout: () => void;
  initialize: () => void;
}

const useUserStore = (set: any) => ({
  user: null as User | null,
  
  setUser: (user: User) => set({ user }),
  setTokens: (tokens: TokenPair) => set({ tokens }),
});

export const useAuthStore = create<AuthStore>()(
  persist(
    (set) => ({
      user: null,
      tokens: null,
      isAuthenticated: false,
      isLoading: true,
      error: null,

      setUser: (user: User) =>
        set({
          user,
          isAuthenticated: !!user,
        }),

      setTokens: (tokens: TokenPair) =>
        set({ tokens }),

      setAuthState: (user: User, tokens: TokenPair) =>
        set({
          user,
          tokens,
          isAuthenticated: true,
          error: null,
        }),

      clearAuth: () =>
        set({
          user: null,
          tokens: null,
          isAuthenticated: false,
          error: null,
        }),

      setLoading: (loading: boolean) =>
        set({ isLoading: loading }),

      setError: (error: string | null) =>
        set({ error }),

      logout: () =>
        set({
          user: null,
          tokens: null,
          isAuthenticated: false,
          error: null,
        }),

      initialize: () => {
        // Initialization logic will be in effect hook
        set({ isLoading: false });
      },
    }),
    {
      name: 'auth-storage',
      storage: createJSONStorage(() => AsyncStorage),
    }
  )
);

export { useUserStore };
