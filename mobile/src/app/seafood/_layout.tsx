import { Stack } from 'expo-router';

import { palette } from '@/constants/theme';

/**
 * Native stack of the seafood guide tab.
 */
export default function SeafoodLayout() {
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
        options={{ title: 'Seafood' }}
      />
      <Stack.Screen
        name="[slug]"
        options={{ title: '', headerBackTitle: 'Seafood' }}
      />
    </Stack>
  );
}
