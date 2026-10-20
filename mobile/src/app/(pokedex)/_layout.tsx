import { Stack } from 'expo-router';

import { palette } from '@/constants/theme';

/**
 * Native stack of the Pokédex tab. Every push (the detail screen) uses the native
 * screen transitions of the platform.
 */
export default function PokedexLayout() {
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
        options={{ title: 'Pokédex', headerLargeTitle: true }}
      />
      <Stack.Screen
        name="pokemon/[name]"
        options={{ title: '', headerBackTitle: 'Pokédex' }}
      />
    </Stack>
  );
}
