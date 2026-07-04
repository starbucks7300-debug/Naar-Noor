import { create } from 'zustand';
import { persist, createJSONStorage } from 'zustand/middleware';
import AsyncStorage from '@react-native-async-storage/async-storage';

interface UIStore {
  language: 'en' | 'ar';
  theme: 'light' | 'dark';
  loading: boolean;

  setLanguage: (language: 'en' | 'ar') => void;
  setTheme: (theme: 'light' | 'dark') => void;
  setLoading: (loading: boolean) => void;
}

export const useUIStore = create<UIStore>()(
  persist(
    (set) => ({
      language: 'en',
      theme: 'light',
      loading: false,

      setLanguage: (language: 'en' | 'ar') => set({ language }),
      setTheme: (theme: 'light' | 'dark') => set({ theme }),
      setLoading: (loading: boolean) => set({ loading }),
    }),
    {
      name: 'ui-storage',
      storage: createJSONStorage(() => AsyncStorage),
    }
  )
);
