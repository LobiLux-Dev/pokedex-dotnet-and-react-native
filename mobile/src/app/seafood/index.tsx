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
import { SeafoodCard } from '@/components/SeafoodCard';
import { EmptyState, ErrorState } from '@/components/StateViews';
import { palette, spacing } from '@/constants/theme';
import { useSeafood, useSeafoodFacets } from '@/hooks/useCatalogs';
import { useDebouncedValue } from '@/hooks/useDebouncedValue';

export default function SeafoodScreen() {
  const router = useRouter();
  const [search, setSearch] = useState('');
  const [category, setCategory] = useState<string | null>(null);
  const [sustainability, setSustainability] = useState<string | null>(null);
  const [maxPrepMinutes, setMaxPrepMinutes] = useState<number | null>(null);

  const debouncedSearch = useDebouncedValue(search, 350);
  const facets = useSeafoodFacets();
  const query = useSeafood({
    search: debouncedSearch,
    category,
    sustainability,
    maxPrepMinutes,
  });

  const items = useMemo(
    () => query.data?.pages.flatMap((page) => page.items) ?? [],
    [query.data],
  );

  const totalItems = query.data?.pages[0]?.totalItems ?? 0;
  const hasFilters =
    debouncedSearch.length > 0 ||
    category !== null ||
    sustainability !== null ||
    maxPrepMinutes !== null;

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
        key: 'sustainability',
        title: 'Sostenibilidad',
        options: [
          { label: 'Cualquiera', value: null },
          ...(facets.data?.sustainabilities ?? []).map((facet) => ({
            label: `${facet.name} (${facet.count})`,
            value: facet.name,
          })),
        ],
        selected: sustainability,
        onSelect: setSustainability,
      },
      {
        key: 'time',
        title: 'Tiempo de preparación',
        options: [
          { label: 'Cualquiera', value: null },
          { label: '≤ 15 min', value: '15' },
          { label: '≤ 30 min', value: '30' },
          { label: '≤ 60 min', value: '60' },
        ],
        selected: maxPrepMinutes === null ? null : String(maxPrepMinutes),
        onSelect: (value) => setMaxPrepMinutes(value === null ? null : Number(value)),
      },
    ],
    [category, facets.data, maxPrepMinutes, sustainability],
  );

  const clearAll = () => {
    setSearch('');
    setCategory(null);
    setSustainability(null);
    setMaxPrepMinutes(null);
  };

  if (query.isPending && items.length === 0) {
    return (
      <View style={styles.centered}>
        <ActivityIndicator size="large" color={palette.primary} />
        <Text style={styles.loadingLabel}>Cargando la guía de mariscos…</Text>
      </View>
    );
  }

  if (query.isError && items.length === 0) {
    return (
      <ErrorState
        message={query.error instanceof Error ? query.error.message : 'Error desconocido.'}
        onRetry={() => query.refetch()}
      />
    );
  }

  return (
    <FlatList
      data={items}
      keyExtractor={(item) => String(item.id)}
      contentContainerStyle={styles.listContent}
      renderItem={({ item }) => (
        <SeafoodCard
          entry={item}
          onPress={() =>
            router.push({ pathname: '/seafood/[slug]', params: { slug: item.slug } })
          }
        />
      )}
      ItemSeparatorComponent={() => <View style={styles.separator} />}
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
      ListHeaderComponent={
        <View style={styles.header}>
          <CatalogToolbar
            search={search}
            onSearchChange={setSearch}
            placeholder="Buscar pescado, marisco o Pokémon"
            searchLabel="Buscar mariscos"
            groups={groups}
            onClearAll={clearAll}
          />

          <View style={styles.summaryRow}>
            <Text style={styles.summary}>
              {hasFilters
                ? `${totalItems} ${totalItems === 1 ? 'entrada' : 'entradas'}`
                : `${totalItems} entradas`}
            </Text>
            {query.isFetchingNextPage ? (
              <ActivityIndicator size="small" color={palette.primary} />
            ) : null}
          </View>
        </View>
      }
      ListEmptyComponent={
        <EmptyState
          title="No hay mariscos que coincidan"
          message="Prueba con otro nombre, categoría o nivel de sostenibilidad."
          icon="fish-outline"
        />
      }
      ListFooterComponent={
        items.length > 0 && !query.hasNextPage ? (
          <Text style={styles.footer}>Fin de la guía</Text>
        ) : null
      }
      keyboardShouldPersistTaps="handled"
    />
  );
}

const styles = StyleSheet.create({
  listContent: {
    paddingHorizontal: spacing.lg,
    paddingBottom: spacing.xxl,
    width: '100%',
    maxWidth: 760,
    alignSelf: 'center',
  },
  separator: {
    height: spacing.md,
  },
  header: {
    gap: spacing.sm,
    paddingTop: spacing.sm,
    paddingBottom: spacing.md,
  },
  summaryRow: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
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
