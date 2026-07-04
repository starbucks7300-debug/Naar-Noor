import { useEffect } from 'react';
import { Stack } from 'expo-router';
import { useAuthStore } from '@stores/useAuthStore';
import { useUIStore } from '@stores/useUIStore';
import i18n from '@i18n/i18n';

export const unstable_settings = {
  initialRouteName: '(auth)',
};

export default function RootLayout() {
  const { isAuthenticated, isLoading } = useAuthStore();
  const { language } = useUIStore();

  useEffect(() => {
    // Set language on app initialization
    i18n.changeLanguage(language);
  }, [language]);

  if (isLoading) {
    return null; // Show splash screen
  }

  return (
    <Stack>
      {isAuthenticated ? (
        <Stack.Screen
          name="(app)"
          options={{
            headerShown: false,
            animationEnabled: false,
          }}
        />
      ) : (
        <Stack.Screen
          name="(auth)"
          options={{
            headerShown: false,
            animationEnabled: false,
          }}
        />
      )}
    </Stack>
  );
}
