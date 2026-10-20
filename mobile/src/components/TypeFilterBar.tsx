import { Pressable, ScrollView, StyleSheet, Text } from 'react-native';

import { palette, radius, spacing } from '@/constants/theme';
import { typeColor, typeLabel } from '@/constants/typeColors';
import type { PokemonType } from '@/lib/types';

interface TypeFilterBarProps {
  types: PokemonType[];
  selected: string | null;
  onSelect: (type: string | null) => void;
}

/**
 * Horizontal list of type filters. Tapping the active filter clears it.
 */
export function TypeFilterBar({ types, selected, onSelect }: TypeFilterBarProps) {
  if (types.length === 0) {
    return null;
  }

  return (
    <ScrollView
      horizontal
      showsHorizontalScrollIndicator={false}
      contentContainerStyle={styles.content}
      keyboardShouldPersistTaps="handled"
    >
      <Pressable
        onPress={() => onSelect(null)}
        accessibilityRole="button"
        accessibilityState={{ selected: selected === null }}
        style={[styles.chip, selected === null && styles.chipSelected]}
      >
        <Text style={[styles.chipLabel, selected === null && styles.chipLabelSelected]}>
          Todos
        </Text>
      </Pressable>

      {types.map((type) => {
        const isSelected = selected === type.name;
        return (
          <Pressable
            key={type.name}
            onPress={() => onSelect(isSelected ? null : type.name)}
            accessibilityRole="button"
            accessibilityState={{ selected: isSelected }}
            style={[
              styles.chip,
              isSelected && { backgroundColor: typeColor(type.name), borderColor: typeColor(type.name) },
            ]}
          >
            <Text style={[styles.chipLabel, isSelected && styles.chipLabelSelected]}>
              {typeLabel(type.name)}
            </Text>
          </Pressable>
        );
      })}
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  content: {
    paddingHorizontal: spacing.lg,
    paddingVertical: spacing.sm,
    gap: spacing.sm,
  },
  chip: {
    paddingHorizontal: spacing.lg,
    height: 34,
    borderRadius: radius.pill,
    borderWidth: 1,
    borderColor: palette.border,
    backgroundColor: palette.surface,
    alignItems: 'center',
    justifyContent: 'center',
  },
  chipSelected: {
    backgroundColor: palette.primary,
    borderColor: palette.primary,
  },
  chipLabel: {
    fontSize: 13,
    fontWeight: '600',
    color: palette.textSecondary,
  },
  chipLabelSelected: {
    color: palette.onPrimary,
  },
});
