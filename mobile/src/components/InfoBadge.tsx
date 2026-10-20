import { StyleSheet, Text, View } from 'react-native';

import { palette, radius, spacing } from '@/constants/theme';

export type BadgeTone = 'neutral' | 'primary' | 'accent' | 'success' | 'warning' | 'danger';

interface InfoBadgeProps {
  label: string;
  tone?: BadgeTone;
  icon?: string;
}

const TONE_BACKGROUNDS: Record<BadgeTone, string> = {
  neutral: palette.surfaceMuted,
  primary: palette.primarySoft,
  accent: '#E6EEFF',
  success: '#E3F5EF',
  warning: '#FDF1DC',
  danger: '#FBE7E9',
};

const TONE_COLORS: Record<BadgeTone, string> = {
  neutral: palette.textSecondary,
  primary: palette.primaryDark,
  accent: '#2451C4',
  success: '#1B7A6B',
  warning: '#A96A12',
  danger: palette.danger,
};

/**
 * Small pill used to show categories, difficulties, sustainability ratings, ...
 */
export function InfoBadge({ label, tone = 'neutral' }: InfoBadgeProps) {
  return (
    <View style={[styles.badge, { backgroundColor: TONE_BACKGROUNDS[tone] }]}>
      <Text style={[styles.label, { color: TONE_COLORS[tone] }]} numberOfLines={1}>
        {label}
      </Text>
    </View>
  );
}

const styles = StyleSheet.create({
  badge: {
    borderRadius: radius.pill,
    paddingHorizontal: spacing.md,
    paddingVertical: 4,
    alignSelf: 'flex-start',
  },
  label: {
    fontSize: 12,
    fontWeight: '700',
  },
});
