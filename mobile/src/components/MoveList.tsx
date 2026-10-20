import { StyleSheet, Text, View } from 'react-native';

import { TypeBadge } from '@/components/TypeBadge';
import { palette, radius, spacing } from '@/constants/theme';
import type { PokemonMove } from '@/lib/types';

const LEARN_METHOD_LABELS: Record<string, string> = {
  'level-up': 'Subiendo de nivel',
  machine: 'Máquina',
  egg: 'Huevo',
  tutor: 'Tutor',
  reminder: 'Recordatorio',
};

interface MoveListProps {
  moves: PokemonMove[];
  limit?: number;
}

/**
 * List of the moves a Pokémon can learn, grouped by learn method.
 */
export function MoveList({ moves, limit }: MoveListProps) {
  const visibleMoves = limit ? moves.slice(0, limit) : moves;

  if (visibleMoves.length === 0) {
    return <Text style={styles.empty}>No hay movimientos registrados.</Text>;
  }

  const grouped = visibleMoves.reduce<Record<string, PokemonMove[]>>((accumulator, move) => {
    const key = move.learnMethod || 'other';
    accumulator[key] = [...(accumulator[key] ?? []), move];
    return accumulator;
  }, {});

  const groups = Object.entries(grouped).sort(([a], [b]) => {
    if (a === 'level-up') return -1;
    if (b === 'level-up') return 1;
    return a.localeCompare(b);
  });

  return (
    <View style={styles.container}>
      {groups.map(([method, groupMoves]) => (
        <View key={method} style={styles.group}>
          <Text style={styles.groupTitle}>
            {LEARN_METHOD_LABELS[method] ?? method}
          </Text>

          <View style={styles.rows}>
            {groupMoves.map((move, index) => (
              <View
                key={`${move.name}-${method}-${index}`}
                style={styles.row}
              >
                <View style={styles.rowMain}>
                  <Text style={styles.moveName} numberOfLines={1}>
                    {move.name.replace(/-/g, ' ')}
                  </Text>
                  {move.type ? <TypeBadge type={move.type} size="sm" /> : null}
                </View>

                <View style={styles.rowMeta}>
                  {move.learnMethod === 'level-up' && move.level > 0 ? (
                    <Text style={styles.meta}>Nv. {move.level}</Text>
                  ) : null}
                  {move.power ? <Text style={styles.meta}>Pot. {move.power}</Text> : null}
                  {move.accuracy ? <Text style={styles.meta}>Prec. {move.accuracy}%</Text> : null}
                </View>
              </View>
            ))}
          </View>
        </View>
      ))}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    gap: spacing.lg,
  },
  group: {
    gap: spacing.sm,
  },
  groupTitle: {
    fontSize: 12,
    fontWeight: '800',
    letterSpacing: 0.8,
    textTransform: 'uppercase',
    color: palette.textTertiary,
  },
  rows: {
    backgroundColor: palette.surfaceMuted,
    borderRadius: radius.md,
    paddingHorizontal: spacing.md,
  },
  row: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    gap: spacing.md,
    paddingVertical: spacing.sm + 2,
    borderBottomWidth: StyleSheet.hairlineWidth,
    borderBottomColor: palette.border,
  },
  rowMain: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.sm,
  },
  moveName: {
    fontSize: 14,
    fontWeight: '600',
    color: palette.text,
    textTransform: 'capitalize',
    flexShrink: 1,
  },
  rowMeta: {
    flexDirection: 'row',
    gap: spacing.sm,
  },
  meta: {
    fontSize: 12,
    color: palette.textSecondary,
    fontWeight: '600',
  },
  empty: {
    fontSize: 14,
    color: palette.textSecondary,
  },
});
