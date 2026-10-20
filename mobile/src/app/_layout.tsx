import { QueryClientProvider } from '@tanstack/react-query';
import { NativeTabs } from 'expo-router/unstable-native-tabs';
import { StatusBar } from 'expo-status-bar';
import { ActivityIndicator, StyleSheet, View } from 'react-native';

import { OfflineScreen } from '@/components/OfflineScreen';
import { palette } from '@/constants/theme';
import { FavoritesProvider } from '@/hooks/useFavorites';
import { useNetworkStatus } from '@/hooks/useNetworkStatus';
import { queryClient } from '@/lib/queryClient';

/**
 * Root layout: the native tab bar (iOS UITabBarController / Android BottomNavigationView)
 * hosts the top level destinations of the app.
 */
export default function RootLayout() {
  return (
    <QueryClientProvider client={queryClient}>
      <FavoritesProvider>
        <StatusBar style="dark" />
        <NetworkGate />
      </FavoritesProvider>
    </QueryClientProvider>
  );
}

/**
 * Without network the app cannot be entered: a blocking error screen is shown and it
 * disappears by itself as soon as the connection comes back.
 */
function NetworkGate() {
  const { isReady, isOffline, hasBeenOnline, recheck } = useNetworkStatus();

  if (!isReady) {
    return (
      <View style={styles.splash}>
        <ActivityIndicator size="large" color={palette.primary} />
      </View>
    );
  }

  // Only block on entry: a connection drop in the middle of a session must not throw the
  // user out of the screen they are on.
  if (isOffline && !hasBeenOnline) {
    return <OfflineScreen onRetry={recheck} />;
  }

  return (
    <NativeTabs>
      <NativeTabs.Trigger name="(pokedex)">
        <NativeTabs.Trigger.Label>Pokédex</NativeTabs.Trigger.Label>
        <NativeTabs.Trigger.Icon
          sf={{ default: 'square.grid.2x2', selected: 'square.grid.2x2.fill' }}
          md="pets"
        />
      </NativeTabs.Trigger>

      <NativeTabs.Trigger name="recipes">
        <NativeTabs.Trigger.Label>Recetario</NativeTabs.Trigger.Label>
        <NativeTabs.Trigger.Icon
          sf={{ default: 'book', selected: 'book.fill' }}
          md="menu_book"
        />
      </NativeTabs.Trigger>

      <NativeTabs.Trigger name="seafood">
        <NativeTabs.Trigger.Label>Seafood</NativeTabs.Trigger.Label>
        <NativeTabs.Trigger.Icon
          sf={{ default: 'fish', selected: 'fish.fill' }}
          md="set_meal"
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
  );
}

const styles = StyleSheet.create({
  splash: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: palette.background,
  },
});
