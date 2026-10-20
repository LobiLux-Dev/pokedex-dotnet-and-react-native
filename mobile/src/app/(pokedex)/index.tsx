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
import { PokemonCard } from '@/components/PokemonCard';
import { EmptyState, ErrorState } from '@/components/StateViews';
import { palette, spacing } from '@/constants/theme';
import { GENERATIONS } from '@/constants/generations';
import { typeLabel } from '@/constants/typeColors';
import { useDebouncedValue } from '@/hooks/useDebouncedValue';
import { usePokemonList, usePokemonTypes } from '@/hooks/usePokemonCatalog';
import type { PokemonSummary } from '@/lib/types';

export default function PokemonListScreen() {
  const router = useRouter();

  const [search, setSearch] = useState('');
  const [selectedType, setSelectedType] = useState<string | null>(null);
  const [selectedGeneration, setSelectedGeneration] = useState<number | null>(null);

  const debouncedSearch = useDebouncedValue(search, 350);
  const typesQuery = usePokemonTypes();
  const listQuery = usePokemonList(debouncedSearch, selectedType, selectedGeneration);

  const items = useMemo(
    () => listQuery.data?.pages.flatMap((page) => page.items) ?? [],
    [listQuery.data],
  );

  const totalItems = listQuery.data?.pages[0]?.totalItems ?? 0;
  const hasFilters =
    debouncedSearch.length > 0 || selectedType !== null || selectedGeneration !== null;

  const filterGroups: FilterGroup[] = useMemo(
    () => [
      {
        key: 'type',
        title: 'Tipo',
        options: [
          { label: 'Todos', value: null },
          ...(typesQuery.data ?? []).map((type) => ({
            label: `${typeLabel(type.name)} (${type.pokemonCount})`,
            value: type.name,
          })),
        ],
        selected: selectedType,
        onSelect: setSelectedType,
      },
      {
        key: 'generation',
        title: 'Generación',
        options: [
          { label: 'Todas', value: null },
          ...GENERATIONS.map((generation) => ({
            label: generation.label,
            value: String(generation.value),
          })),
        ],
        selected: selectedGeneration === null ? null : String(selectedGeneration),
        onSelect: (value) => setSelectedGeneration(value === null ? null : Number(value)),
      },
    ],
    [selectedGeneration, selectedType, typesQuery.data],
  );

  const clearFilters = () => {
    setSearch('');
    setSelectedType(null);
    setSelectedGeneration(null);
  };

  const openDetail = (pokemon: PokemonSummary) => {
    router.push({ pathname: '/(pokedex)/pokemon/[name]', params: { name: pokemon.name } });
  };

  const showInitialLoading = listQuery.isPending && items.length === 0;
  const showError = listQuery.isError && items.length === 0;
  const isRefreshing = listQuery.isRefetching && items.length > 0;

  return (
    <View style={styles.screen}>
      {/* Fixed toolbar: always visible, never scrolled away by the list. */}
      <View style={styles.toolbar}>
        <CatalogToolbar
          search={search}
          onSearchChange={setSearch}
          placeholder="Buscar por nombre o número"
          searchLabel="Buscar Pokémon"
          groups={filterGroups}
          onClearAll={clearFilters}
        />

        <View style={styles.summaryRow}>
          <Text style={styles.summary}>
            {hasFilters
              ? `${totalItems} ${totalItems === 1 ? 'resultado' : 'resultados'}`
              : `${totalItems} Pokémon`}
          </Text>

          {listQuery.isFetchingNextPage ? (
            <ActivityIndicator size="small" color={palette.primary} />
          ) : null}
        </View>
      </View>

      {showInitialLoading ? (
        <View style={styles.centered}>
          <ActivityIndicator size="large" color={palette.primary} />
          <Text style={styles.loadingLabel}>Cargando el Pokédex…</Text>
        </View>
      ) : showError ? (
        <ErrorState
          message={
            listQuery.error instanceof Error
              ? listQuery.error.message
              : 'Error desconocido al contactar con la API.'
          }
          onRetry={() => listQuery.refetch()}
        />
      ) : (
        <FlatList
          style={styles.list}
          data={items}
          keyExtractor={(item) => String(item.id)}
          numColumns={2}
          columnWrapperStyle={styles.column}
          contentContainerStyle={styles.listContent}
          renderItem={({ item }) => (
            <PokemonCard pokemon={item} onPress={() => openDetail(item)} />
          )}
          onEndReached={() => {
            if (listQuery.hasNextPage && !listQuery.isFetchingNextPage) {
              listQuery.fetchNextPage();
            }
          }}
          onEndReachedThreshold={0.6}
          refreshControl={
            <RefreshControl
              refreshing={isRefreshing}
              onRefresh={() => listQuery.refetch()}
              tintColor={palette.primary}
            />
          }
          ListEmptyComponent={
            <EmptyState
              title="No hay coincidencias"
              message="Prueba con otro nombre, número o tipo."
              icon="search-outline"
            />
          }
          ListFooterComponent={
            items.length > 0 && !listQuery.hasNextPage ? (
              <Text style={styles.footer}>Fin del catálogo</Text>
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
  listContent: {
    paddingHorizontal: spacing.lg,
    paddingTop: spacing.md,
    paddingBottom: spacing.xxl,
  },
  column: {
    gap: spacing.md,
    marginBottom: spacing.md,
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
