import { useRouter } from 'expo-router';
import { useMemo, useState } from 'react';
import {
  ActivityIndicator,
  FlatList,
  RefreshControl,
  StyleSheet,
  Text,
  View,
} from 'react-native';

import { CatalogToolbar, type FilterGroup } from '@/components/CatalogToolbar';
import { RecipeCard } from '@/components/RecipeCard';
import { EmptyState, ErrorState } from '@/components/StateViews';
import { palette, spacing } from '@/constants/theme';
import { useRecipeFacets, useRecipes } from '@/hooks/useCatalogs';
import { useDebouncedValue } from '@/hooks/useDebouncedValue';

export default function RecipesScreen() {
  const router = useRouter();

  const [search, setSearch] = useState('');
  const [category, setCategory] = useState<string | null>(null);
  const [difficulty, setDifficulty] = useState<string | null>(null);
  const [maxMinutes, setMaxMinutes] = useState<number | null>(null);

  const debouncedSearch = useDebouncedValue(search, 350);
  const facets = useRecipeFacets();
  const query = useRecipes({ search: debouncedSearch, category, difficulty, maxMinutes });

  const items = useMemo(
    () => query.data?.pages.flatMap((page) => page.items) ?? [],
    [query.data],
  );

  const totalItems = query.data?.pages[0]?.totalItems ?? 0;
  const hasFilters =
    debouncedSearch.length > 0 || category !== null || difficulty !== null || maxMinutes !== null;

  const groups: FilterGroup[] = useMemo(
    () => [
      {
        key: 'category',
        title: 'Categoría',
        options: [
          { label: 'Todas', value: null },
          ...(facets.data?.categories ?? []).map((facet) => ({
            label: `${facet.name} (${facet.count})`,
            value: facet.name,
          })),
        ],
        selected: category,
        onSelect: setCategory,
      },
      {
        key: 'difficulty',
        title: 'Dificultad',
        options: [
          { label: 'Cualquiera', value: null },
          ...(facets.data?.difficulties ?? []).map((facet) => ({
            label: `${facet.name} (${facet.count})`,
            value: facet.name,
          })),
        ],
        selected: difficulty,
        onSelect: setDifficulty,
      },
      {
        key: 'time',
        title: 'Tiempo total',
        options: [
          { label: 'Cualquiera', value: null },
          { label: '≤ 30 min', value: '30' },
          { label: '≤ 60 min', value: '60' },
          { label: '≤ 90 min', value: '90' },
        ],
        selected: maxMinutes === null ? null : String(maxMinutes),
        onSelect: (value) => setMaxMinutes(value === null ? null : Number(value)),
      },
    ],
    [category, difficulty, facets.data, maxMinutes],
  );

  const clearAll = () => {
    setSearch('');
    setCategory(null);
    setDifficulty(null);
    setMaxMinutes(null);
  };

  const showInitialLoading = query.isPending && items.length === 0;
  const showError = query.isError && items.length === 0;

  return (
    <View style={styles.screen}>
      <View style={styles.toolbar}>
        <CatalogToolbar
          search={search}
          onSearchChange={setSearch}
          placeholder="Buscar recetas, ingredientes o Pokémon"
          searchLabel="Buscar recetas"
          groups={groups}
          onClearAll={clearAll}
        />

        <View style={styles.summaryRow}>
          <Text style={styles.summary}>
            {hasFilters
              ? `${totalItems} ${totalItems === 1 ? 'receta' : 'recetas'}`
              : `${totalItems} recetas`}
          </Text>
          {query.isFetchingNextPage ? (
            <ActivityIndicator size="small" color={palette.primary} />
          ) : null}
        </View>
      </View>

      {showInitialLoading ? (
        <View style={styles.centered}>
          <ActivityIndicator size="large" color={palette.primary} />
          <Text style={styles.loadingLabel}>Cargando el recetario…</Text>
        </View>
      ) : showError ? (
        <ErrorState
          message={query.error instanceof Error ? query.error.message : 'Error desconocido.'}
          onRetry={() => query.refetch()}
        />
      ) : (
        <FlatList
          style={styles.list}
          data={items}
          keyExtractor={(item) => String(item.id)}
          contentContainerStyle={styles.listContent}
          ItemSeparatorComponent={() => <View style={styles.separator} />}
          renderItem={({ item }) => (
            <RecipeCard
              recipe={item}
              onPress={() =>
                router.push({ pathname: '/recipes/[slug]', params: { slug: item.slug } })
              }
            />
          )}
          onEndReached={() => {
            if (query.hasNextPage && !query.isFetchingNextPage) {
              query.fetchNextPage();
            }
          }}
          onEndReachedThreshold={0.6}
          refreshControl={
            <RefreshControl
              refreshing={query.isRefetching && items.length > 0}
              onRefresh={() => query.refetch()}
              tintColor={palette.primary}
            />
          }
          ListEmptyComponent={
            <EmptyState
              title="No hay recetas que coincidan"
              message="Prueba con otro ingrediente, categoría o nivel de dificultad."
              icon="book-outline"
            />
          }
          ListFooterComponent={
            items.length > 0 && !query.hasNextPage ? (
              <Text style={styles.footer}>Fin del recetario</Text>
            ) : null
          }
          keyboardShouldPersistTaps="handled"
          keyboardDismissMode="on-drag"
        />
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  screen: {
    flex: 1,
  },
  toolbar: {
    paddingHorizontal: spacing.lg,
    paddingTop: spacing.sm,
    paddingBottom: spacing.sm,
    gap: spacing.xs,
    backgroundColor: palette.background,
    borderBottomWidth: StyleSheet.hairlineWidth,
    borderBottomColor: palette.border,
  },
  list: {
    flex: 1,
  },
  separator: {
    height: spacing.lg,
  },
  listContent: {
    paddingHorizontal: spacing.lg,
    paddingTop: spacing.md,
    paddingBottom: spacing.xxl,
    width: '100%',
    maxWidth: 760,
    alignSelf: 'center',
  },
  summaryRow: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    marginTop: spacing.xs,
  },
  summary: {
    fontSize: 13,
    fontWeight: '700',
    color: palette.textSecondary,
  },
  footer: {
    textAlign: 'center',
    fontSize: 12,
    color: palette.textTertiary,
    paddingVertical: spacing.lg,
  },
  centered: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    gap: spacing.md,
  },
  loadingLabel: {
    fontSize: 14,
    color: palette.textSecondary,
  },
});
