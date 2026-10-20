import type { ReactNode } from 'react';
import { StyleSheet, Text, View } from 'react-native';

import { palette, radius, shadow, spacing } from '@/constants/theme';

interface SectionProps {
  title: string;
  children: ReactNode;
  action?: ReactNode;
}

/**
 * Card used to group the sections of the detail screen.
 */
export function Section({ title, children, action }: SectionProps) {
  return (
    <View style={styles.container}>
      <View style={styles.header}>
        <Text style={styles.title}>{title}</Text>
        {action}
      </View>
      {children}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    backgroundColor: palette.surface,
    borderRadius: radius.lg,
    padding: spacing.lg,
    gap: spacing.md,
    ...shadow.card,
  },
  header: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
  },
  title: {
    fontSize: 13,
    fontWeight: '800',
    letterSpacing: 0.9,
    textTransform: 'uppercase',
    color: palette.textTertiary,
  },
});
