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
 * It is rendered outside the list so it stays visible while the results scroll.
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
  const activeFilterCount = groups.reduce(
    (total, group) => total + (group.selected === null ? 0 : 1),
    0,
  );
  const hasActiveSearch = search.trim().length > 0;
  const hasActiveState = activeFilterCount > 0 || hasActiveSearch;

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

      {extra.length > 0 || hasActiveState ? (
        <View style={styles.actionsRow}>
          {extra.length > 0 ? (
            <Pressable
              onPress={() => setExpanded((value) => !value)}
              accessibilityRole="button"
              accessibilityState={{ expanded }}
              style={({ pressed }) => [
                styles.filterButton,
                activeFilterCount > 0 && styles.filterButtonActive,
                pressed && styles.pressed,
              ]}
            >
              <Ionicons
                name="options-outline"
                size={15}
                color={activeFilterCount > 0 ? palette.onPrimary : palette.textSecondary}
              />
              <Text
                style={[
                  styles.filterButtonLabel,
                  activeFilterCount > 0 && styles.filterButtonLabelActive,
                ]}
              >
                Filtros{activeFilterCount > 0 ? ` (${activeFilterCount})` : ''}
              </Text>
              <Ionicons
                name={expanded ? 'chevron-up' : 'chevron-down'}
                size={15}
                color={activeFilterCount > 0 ? palette.onPrimary : palette.textSecondary}
              />
            </Pressable>
          ) : null}

          {hasActiveState ? (
            <Pressable
              onPress={onClearAll}
              accessibilityRole="button"
              accessibilityLabel="Limpiar búsqueda y filtros"
              style={({ pressed }) => [styles.clearButton, pressed && styles.pressed]}
            >
              <Ionicons name="close-circle" size={15} color={palette.danger} />
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
    // A horizontal ScrollView without an explicit height collapses to zero on iOS.
    <View style={styles.chipRowWrapper}>
      <ScrollView
        horizontal
        showsHorizontalScrollIndicator={false}
        contentContainerStyle={styles.chipRow}
        style={styles.chipScroll}
        keyboardShouldPersistTaps="handled"
      >
        {options.map((option) => {
          const isSelected = option.value === selected;

          return (
            <Pressable
              key={option.value ?? '__all__'}
              // Tapping the active chip clears the filter, so it can always be removed.
              onPress={() => onSelect(isSelected && option.value !== null ? null : option.value)}
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
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    gap: spacing.sm,
  },
  chipRowWrapper: {
    // Fixed height so the horizontal ScrollView can never collapse on iOS.
    height: 44,
  },
  chipScroll: {
    height: 44,
    flexGrow: 0,
  },
  chipRow: {
    paddingVertical: 5,
    gap: spacing.sm,
    alignItems: 'center',
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
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.xs,
    paddingHorizontal: spacing.md,
    height: 32,
    borderRadius: radius.pill,
    backgroundColor: palette.primarySoft,
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
