import Ionicons from '@expo/vector-icons/Ionicons';
import { StyleSheet, Text, View } from 'react-native';

import { palette, spacing } from '@/constants/theme';

interface MetaItemProps {
  icon: keyof typeof Ionicons.glyphMap;
  label: string;
}

/**
 * Small icon + value pair used in the catalogue cards.
 */
export function MetaItem({ icon, label }: MetaItemProps) {
  return (
    <View style={styles.container}>
      <Ionicons name={icon} size={13} color={palette.textTertiary} />
      <Text style={styles.label}>{label}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.xs,
  },
  label: {
    fontSize: 12,
    fontWeight: '600',
    color: palette.textSecondary,
  },
});
