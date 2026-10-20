import { QueryClientProvider } from '@tanstack/react-query';
import type { ErrorBoundaryProps } from 'expo-router';
import { NativeTabs } from 'expo-router/unstable-native-tabs';
import { StatusBar } from 'expo-status-bar';
import { Pressable, StyleSheet, Text, View } from 'react-native';

import { OfflineScreen } from '@/components/OfflineScreen';
import { palette, radius, spacing } from '@/constants/theme';
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
        <RootNavigator />
      </FavoritesProvider>
    </QueryClientProvider>
  );
}

/**
 * The navigator is rendered by default: the offline screen only replaces it when the device
 * explicitly reports no connection and the app has not been online yet. If the connectivity
 * state is unknown, the app opens normally.
 */
function RootNavigator() {
  const { isOffline, hasBeenOnline, recheck } = useNetworkStatus();

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

/**
 * Last resort: if anything throws while rendering, show the message instead of a blank
 * screen, with a button to remount the app.
 */
export function ErrorBoundary({ error, retry }: ErrorBoundaryProps) {
  return (
    <View style={styles.errorContainer}>
      <Text style={styles.errorTitle}>Algo ha fallado</Text>
      <Text style={styles.errorMessage}>{error.message}</Text>

      <Pressable
        onPress={retry}
        accessibilityRole="button"
        accessibilityLabel="Reintentar la carga de la aplicación"
        style={({ pressed }) => [styles.errorButton, pressed && styles.pressed]}
      >
        <Text style={styles.errorButtonLabel}>Reintentar</Text>
      </Pressable>
    </View>
  );
}

const styles = StyleSheet.create({
  errorContainer: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    padding: spacing.xxl,
    gap: spacing.sm,
    backgroundColor: palette.background,
  },
  errorTitle: {
    fontSize: 20,
    fontWeight: '800',
    color: palette.text,
    textAlign: 'center',
  },
  errorMessage: {
    fontSize: 14,
    lineHeight: 20,
    color: palette.textSecondary,
    textAlign: 'center',
  },
  errorButton: {
    marginTop: spacing.lg,
    backgroundColor: palette.primary,
    paddingHorizontal: spacing.xxl,
    paddingVertical: spacing.md,
    borderRadius: radius.pill,
  },
  pressed: {
    opacity: 0.85,
  },
  errorButtonLabel: {
    color: palette.onPrimary,
    fontWeight: '700',
    fontSize: 15,
  },
});
