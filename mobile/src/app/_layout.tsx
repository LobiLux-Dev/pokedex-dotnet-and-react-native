import { QueryClientProvider } from '@tanstack/react-query';
import { NativeTabs } from 'expo-router/unstable-native-tabs';
import { StatusBar } from 'expo-status-bar';

import { FavoritesProvider } from '@/hooks/useFavorites';
import { queryClient } from '@/lib/queryClient';

/**
 * Root layout: the native tab bar (iOS UITabBarController / Android BottomNavigationView)
 * hosts the two top level destinations of the app.
 */
export default function RootLayout() {
  return (
    <QueryClientProvider client={queryClient}>
      <FavoritesProvider>
        <StatusBar style="dark" />

        <NativeTabs>
          <NativeTabs.Trigger name="(pokedex)">
            <NativeTabs.Trigger.Label>Pokédex</NativeTabs.Trigger.Label>
            <NativeTabs.Trigger.Icon
              sf={{ default: 'square.grid.2x2', selected: 'square.grid.2x2.fill' }}
              md="pets"
            />
          </NativeTabs.Trigger>

          <NativeTabs.Trigger name="favorites">
            <NativeTabs.Trigger.Label>Favoritos</NativeTabs.Trigger.Label>
            <NativeTabs.Trigger.Icon
              sf={{ default: 'star', selected: 'star.fill' }}
              md="star"
            />
          </NativeTabs.Trigger>
        </NativeTabs>
      </FavoritesProvider>
    </QueryClientProvider>
  );
}
