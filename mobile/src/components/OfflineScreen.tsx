import Ionicons from '@expo/vector-icons/Ionicons';
import { Pressable, StyleSheet, Text, View } from 'react-native';
import { SafeAreaView } from 'react-native-safe-area-context';

import { palette, radius, spacing } from '@/constants/theme';

interface OfflineScreenProps {
  onRetry: () => void;
}

/**
 * Full screen blocking state used when the device has no network.
 */
export function OfflineScreen({ onRetry }: OfflineScreenProps) {
  return (
    <SafeAreaView style={styles.safeArea}>
      <View style={styles.container}>
        <View style={styles.iconCircle}>
          <Ionicons name="cloud-offline-outline" size={40} color={palette.danger} />
        </View>

        <Text style={styles.title}>Sin conexión</Text>
        <Text style={styles.message}>
          No hay conexión a internet. Revisa tu red o tus datos móviles y vuelve a intentarlo.
        </Text>

        <Pressable
          onPress={onRetry}
          accessibilityRole="button"
          accessibilityLabel="Volver a comprobar la conexión"
          style={({ pressed }) => [styles.button, pressed && styles.pressed]}
        >
          <Ionicons name="refresh" size={16} color={palette.onPrimary} />
          <Text style={styles.buttonLabel}>Reintentar</Text>
        </Pressable>
      </View>
    </SafeAreaView>
  );
}

const styles = StyleSheet.create({
  safeArea: {
    flex: 1,
    backgroundColor: palette.background,
  },
  container: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    padding: spacing.xxl,
    gap: spacing.sm,
  },
  iconCircle: {
    width: 84,
    height: 84,
    borderRadius: radius.pill,
    backgroundColor: palette.primarySoft,
    alignItems: 'center',
    justifyContent: 'center',
    marginBottom: spacing.md,
  },
  title: {
    fontSize: 22,
    fontWeight: '800',
    color: palette.text,
    textAlign: 'center',
  },
  message: {
    fontSize: 15,
    lineHeight: 22,
    color: palette.textSecondary,
    textAlign: 'center',
    maxWidth: 340,
  },
  button: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.sm,
    marginTop: spacing.lg,
    backgroundColor: palette.primary,
    paddingHorizontal: spacing.xxl,
    paddingVertical: spacing.md,
    borderRadius: radius.pill,
  },
  pressed: {
    opacity: 0.85,
  },
  buttonLabel: {
    color: palette.onPrimary,
    fontWeight: '700',
    fontSize: 15,
  },
});
