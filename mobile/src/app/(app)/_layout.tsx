import { Tabs } from 'expo-router';
import { useUserStore } from '@stores/useAuthStore';

export default function AppLayout() {
  const { user } = useUserStore();
  const isStaff = user?.role !== 'customer';

  if (isStaff) {
    return (
      <Tabs
        screenOptions={{
          headerShown: true,
          tabBarLabelPosition: 'below-icon',
        }}
      >
        <Tabs.Screen
          name="(dashboard)"
          options={{
            title: 'Dashboard',
          }}
        />
        <Tabs.Screen
          name="(orders)"
          options={{
            title: 'Orders',
          }}
        />
        <Tabs.Screen
          name="(profile)"
          options={{
            title: 'Profile',
          }}
        />
      </Tabs>
    );
  }

  // Customer app
  return (
    <Tabs
      screenOptions={{
        headerShown: true,
        tabBarLabelPosition: 'below-icon',
      }}
    >
      <Tabs.Screen
        name="(home)"
        options={{
          title: 'Menu',
        }}
      />
      <Tabs.Screen
        name="(reservations)"
        options={{
          title: 'Reservations',
        }}
      />
      <Tabs.Screen
        name="(cart)"
        options={{
          title: 'Cart',
        }}
      />
      <Tabs.Screen
        name="(orders)"
        options={{
          title: 'Orders',
        }}
      />
      <Tabs.Screen
        name="(profile)"
        options={{
          title: 'Profile',
        }}
      />
    </Tabs>
  );
}
