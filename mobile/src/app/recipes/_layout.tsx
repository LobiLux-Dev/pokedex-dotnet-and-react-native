import { Stack } from 'expo-router';

import { palette } from '@/constants/theme';

/**
 * Native stack of the recipe book tab.
 */
export default function RecipesLayout() {
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
        options={{ title: 'Recetario', headerLargeTitle: true }}
      />
      <Stack.Screen
        name="[slug]"
        options={{ title: '', headerBackTitle: 'Recetario' }}
      />
    </Stack>
  );
}
