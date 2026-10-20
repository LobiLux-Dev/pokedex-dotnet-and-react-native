import Ionicons from '@expo/vector-icons/Ionicons';
import { useState } from 'react';
import { Pressable, ScrollView, StyleSheet, Text, View } from 'react-native';

import { SearchField } from '@/components/SearchField';
import { palette, radius, spacing } from '@/constants/theme';

export interface FilterOption {
  label: string;
  value: string | null;
}

export interface FilterGroup {
  key: string;
  title: string;
  options: FilterOption[];
  selected: string | null;
  onSelect: (value: string | null) => void;
}

interface CatalogToolbarProps {
  search: string;
  onSearchChange: (value: string) => void;
  placeholder: string;
  /** Accessible name of the search field, e.g. "Buscar recetas". */
  searchLabel?: string;
  /** The first group is always visible, the rest live behind the "Filtros" button. */
  groups: FilterGroup[];
  onClearAll: () => void;
}

/**
 * Search field plus the filter chips shared by every catalogue of the app.
 */
export function CatalogToolbar({
  search,
  onSearchChange,
  placeholder,
  searchLabel,
  groups,
  onClearAll,
}: CatalogToolbarProps) {
  const [expanded, setExpanded] = useState(false);

  const [primary, ...extra] = groups;
  const activeCount = groups.reduce(
    (total, group) => total + (group.selected === null ? 0 : 1),
    0,
  );

  return (
    <View style={styles.container}>
      <SearchField
        value={search}
        onChangeText={onSearchChange}
        placeholder={placeholder}
        accessibilityLabel={searchLabel ?? placeholder}
      />

      {primary ? (
        <ChipRow
          options={primary.options}
          selected={primary.selected}
          onSelect={primary.onSelect}
        />
      ) : null}

      {extra.length > 0 ? (
        <View style={styles.actionsRow}>
          <Pressable
            onPress={() => setExpanded((value) => !value)}
            accessibilityRole="button"
            accessibilityState={{ expanded }}
            style={({ pressed }) => [
              styles.filterButton,
              activeCount > 0 && styles.filterButtonActive,
              pressed && styles.pressed,
            ]}
          >
            <Ionicons
              name="options-outline"
              size={15}
              color={activeCount > 0 ? palette.onPrimary : palette.textSecondary}
            />
            <Text style={[styles.filterButtonLabel, activeCount > 0 && styles.filterButtonLabelActive]}>
              Filtros{activeCount > 0 ? ` (${activeCount})` : ''}
            </Text>
            <Ionicons
              name={expanded ? 'chevron-up' : 'chevron-down'}
              size={15}
              color={activeCount > 0 ? palette.onPrimary : palette.textSecondary}
            />
          </Pressable>

          {activeCount > 0 ? (
            <Pressable
              onPress={onClearAll}
              accessibilityRole="button"
              style={({ pressed }) => [styles.clearButton, pressed && styles.pressed]}
            >
              <Text style={styles.clearLabel}>Limpiar</Text>
            </Pressable>
          ) : null}
        </View>
      ) : null}

      {expanded
        ? extra.map((group) => (
            <View key={group.key} style={styles.group}>
              <Text style={styles.groupTitle}>{group.title}</Text>
              <ChipRow
                options={group.options}
                selected={group.selected}
                onSelect={group.onSelect}
              />
            </View>
          ))
        : null}
    </View>
  );
}

function ChipRow({
  options,
  selected,
  onSelect,
}: {
  options: FilterOption[];
  selected: string | null;
  onSelect: (value: string | null) => void;
}) {
  return (
    <ScrollView
      horizontal
      showsHorizontalScrollIndicator={false}
      contentContainerStyle={styles.chipRow}
      keyboardShouldPersistTaps="handled"
    >
      {options.map((option) => {
        const isSelected = option.value === selected;
        return (
          <Pressable
            key={option.value ?? '__all__'}
            onPress={() => onSelect(option.value)}
            accessibilityRole="button"
            accessibilityState={{ selected: isSelected }}
            style={({ pressed }) => [
              styles.chip,
              isSelected && styles.chipSelected,
              pressed && styles.pressed,
            ]}
          >
            <Text style={[styles.chipLabel, isSelected && styles.chipLabelSelected]}>
              {option.label}
            </Text>
          </Pressable>
        );
      })}
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    gap: spacing.sm,
  },
  chipRow: {
    paddingVertical: spacing.xs,
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
  actionsRow: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.sm,
  },
  filterButton: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.xs,
    paddingHorizontal: spacing.md,
    height: 32,
    borderRadius: radius.pill,
    borderWidth: 1,
    borderColor: palette.border,
    backgroundColor: palette.surface,
  },
  filterButtonActive: {
    backgroundColor: palette.primary,
    borderColor: palette.primary,
  },
  filterButtonLabel: {
    fontSize: 13,
    fontWeight: '700',
    color: palette.textSecondary,
  },
  filterButtonLabelActive: {
    color: palette.onPrimary,
  },
  clearButton: {
    paddingHorizontal: spacing.md,
    height: 32,
    justifyContent: 'center',
  },
  clearLabel: {
    fontSize: 13,
    fontWeight: '700',
    color: palette.danger,
  },
  group: {
    gap: spacing.xs,
  },
  groupTitle: {
    fontSize: 11,
    fontWeight: '800',
    letterSpacing: 0.8,
    textTransform: 'uppercase',
    color: palette.textTertiary,
    paddingHorizontal: spacing.lg,
  },
  pressed: {
    opacity: 0.7,
  },
});
