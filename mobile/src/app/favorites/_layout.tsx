import { Stack } from 'expo-router';

import { palette } from '@/constants/theme';

/**
 * Native stack of the favourites tab.
 */
export default function FavoritesLayout() {
  return (
    <Stack
      screenOptions={{
        headerStyle: { backgroundColor: palette.background },
        headerTintColor: palette.text,
        headerShadowVisible: false,
        headerTitleStyle: { fontWeight: '700' },
        contentStyle: { backgroundColor: palette.background },
      }}
    >
      <Stack.Screen
        name="index"
        options={{ title: 'Favoritos', headerLargeTitle: true }}
      />
      <Stack.Screen
        name="pokemon/[name]"
        options={{ title: '', headerBackTitle: 'Favoritos' }}
      />
    </Stack>
  );
}
