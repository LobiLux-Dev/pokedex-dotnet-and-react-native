import { StyleSheet, Text, View } from 'react-native';

import { radius, spacing } from '@/constants/theme';
import { typeColor, typeLabel, typeTextColor } from '@/constants/typeColors';

interface TypeBadgeProps {
  type: string;
  size?: 'sm' | 'md';
}

/**
 * Coloured pill showing a Pokémon type.
 */
export function TypeBadge({ type, size = 'md' }: TypeBadgeProps) {
  const backgroundColor = typeColor(type);
  const color = typeTextColor(type);
  const isSmall = size === 'sm';

  return (
    <View
      style={[
        styles.badge,
        isSmall && styles.badgeSmall,
        { backgroundColor },
      ]}
    >
      <Text
        style={[
          styles.label,
          isSmall && styles.labelSmall,
          { color },
        ]}
        numberOfLines={1}
      >
        {typeLabel(type)}
      </Text>
    </View>
  );
}

const styles = StyleSheet.create({
  badge: {
    borderRadius: radius.pill,
    paddingHorizontal: spacing.md,
    paddingVertical: 5,
    alignSelf: 'flex-start',
  },
  badgeSmall: {
    paddingHorizontal: spacing.sm + 2,
    paddingVertical: 3,
  },
  label: {
    fontSize: 13,
    fontWeight: '700',
    letterSpacing: 0.2,
  },
  labelSmall: {
    fontSize: 11,
  },
});
