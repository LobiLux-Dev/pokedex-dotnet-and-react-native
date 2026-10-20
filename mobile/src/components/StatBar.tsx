import { StyleSheet, Text, View } from 'react-native';

import { palette, radius, spacing } from '@/constants/theme';

interface StatBarProps {
  label: string;
  value: number;
  color: string;
  /** Highest base stat in the games, used to normalise the bar. */
  max?: number;
}

const STAT_LABELS: Record<string, string> = {
  hp: 'PS',
  attack: 'Ataque',
  defense: 'Defensa',
  'special-attack': 'At. Especial',
  'special-defense': 'Def. Especial',
  speed: 'Velocidad',
};

export function StatBar({ label, value, color, max = 200 }: StatBarProps) {
  const percentage = Math.max(0, Math.min(100, (value / max) * 100));

  return (
    <View style={styles.container}>
      <Text style={styles.label}>{STAT_LABELS[label] ?? label}</Text>

      <View style={styles.track}>
        <View style={[styles.fill, { width: `${percentage}%`, backgroundColor: color }]} />
      </View>

      <Text style={styles.value}>{value}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.md,
  },
  label: {
    width: 92,
    fontSize: 13,
    fontWeight: '600',
    color: palette.textSecondary,
  },
  track: {
    flex: 1,
    height: 10,
    borderRadius: radius.pill,
    backgroundColor: palette.track,
    overflow: 'hidden',
  },
  fill: {
    height: '100%',
    borderRadius: radius.pill,
  },
  value: {
    width: 34,
    textAlign: 'right',
    fontSize: 13,
    fontWeight: '700',
    color: palette.text,
  },
});
