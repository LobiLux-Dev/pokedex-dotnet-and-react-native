import { Stack, useLocalSearchParams } from 'expo-router';
import { useState } from 'react';
import {
  ActivityIndicator,
  Pressable,
  ScrollView,
  StyleSheet,
  Text,
  View,
} from 'react-native';

import { EvolutionChainView } from '@/components/EvolutionChainView';
import { FavoriteButton } from '@/components/FavoriteButton';
import { MoveList } from '@/components/MoveList';
import { PokemonImage } from '@/components/PokemonImage';
import { Section } from '@/components/Section';
import { ErrorState } from '@/components/StateViews';
import { StatBar } from '@/components/StatBar';
import { TypeBadge } from '@/components/TypeBadge';
import { artworkUrl, palette, radius, shadow, spacing } from '@/constants/theme';
import { STAT_COLORS, typeColor } from '@/constants/typeColors';
import { useFavorites } from '@/hooks/useFavorites';
import { usePokemonDetail } from '@/hooks/usePokemonCatalog';

const MOVES_PREVIEW = 10;

export function PokemonDetailScreen() {
  const params = useLocalSearchParams<{ name: string }>();
  const name = Array.isArray(params.name) ? params.name[0] : params.name;

  const query = usePokemonDetail(name);
  const { isFavorite, toggleFavorite } = useFavorites();
  const [showAllMoves, setShowAllMoves] = useState(false);

  const pokemon = query.data;
  const favorite = pokemon ? isFavorite(pokemon.id) : false;

  if (query.isPending) {
    return (
      <View style={styles.centered}>
        <ActivityIndicator size="large" color={palette.primary} />
        <Text style={styles.loadingLabel}>Cargando…</Text>
      </View>
    );
  }

  if (query.isError || !pokemon) {
    return (
      <ErrorState
        message={
          query.error instanceof Error
            ? query.error.message
            : 'No se pudo cargar el Pokémon.'
        }
        onRetry={() => query.refetch()}
      />
    );
  }

  const primaryType = pokemon.types[0] ?? 'normal';
  const accent = typeColor(primaryType);

  const handleToggleFavorite = () => {
    toggleFavorite({
      id: pokemon.id,
      name: pokemon.name,
      displayName: pokemon.displayName,
      types: pokemon.types,
      spriteUrl: '',
      artworkUrl: '',
    });
  };

  return (
    <>
      <Stack.Screen
        options={{
          title: pokemon.displayName,
          headerRight: () => (
            <FavoriteButton isFavorite={favorite} onPress={handleToggleFavorite} />
          ),
        }}
      />

      <ScrollView
        contentContainerStyle={styles.content}
        showsVerticalScrollIndicator={false}
      >
        <View style={styles.hero}>
          <View style={[styles.heroCircle, { backgroundColor: `${accent}22` }]} />

          <PokemonImage
            uri={artworkUrl(pokemon.id)}
            fallbackUri={pokemon.sprites.frontDefault ?? undefined}
            style={styles.heroImage}
          />

          <Text style={styles.number}>
            #{String(pokemon.id).padStart(3, '0')}
          </Text>
          <Text style={styles.name}>{pokemon.displayName}</Text>

          <View style={styles.types}>
            {pokemon.types.map((type) => (
              <TypeBadge key={type} type={type} />
            ))}
          </View>

          {pokemon.genus ? <Text style={styles.genus}>{pokemon.genus}</Text> : null}
        </View>

        <View style={styles.facts}>
          <Fact label="Altura" value={`${pokemon.heightInMeters.toFixed(1)} m`} />
          <Fact label="Peso" value={`${pokemon.weightInKg.toFixed(1)} kg`} />
          <Fact
            label="Exp. base"
            value={pokemon.baseExperience ? String(pokemon.baseExperience) : '—'}
          />
        </View>

        {pokemon.description ? (
          <Section title="Descripción">
            <Text style={styles.description}>{pokemon.description}</Text>
          </Section>
        ) : null}

        <Section title="Estadísticas">
          <View style={styles.stats}>
            {pokemon.stats.map((stat) => (
              <StatBar
                key={stat.name}
                label={stat.name}
                value={stat.baseValue}
                color={STAT_COLORS[stat.name] ?? accent}
              />
            ))}
          </View>

          <View style={styles.totalRow}>
            <Text style={styles.totalLabel}>Total</Text>
            <Text style={styles.totalValue}>{pokemon.totalStats}</Text>
          </View>
        </Section>

        <Section title="Habilidades">
          <View style={styles.abilities}>
            {pokemon.abilities.map((ability) => (
              <View key={ability.name} style={styles.ability}>
                <Text style={styles.abilityName}>
                  {ability.name.replace(/-/g, ' ')}
                </Text>
                {ability.isHidden ? (
                  <Text style={styles.abilityTag}>Oculta</Text>
                ) : null}
              </View>
            ))}
          </View>
        </Section>

        {pokemon.evolution ? (
          <Section title="Cadena evolutiva">
            <EvolutionChainView root={pokemon.evolution.root} />
          </Section>
        ) : null}

        <Section title="Movimientos">
          <MoveList
            moves={pokemon.moves}
            limit={showAllMoves ? undefined : MOVES_PREVIEW}
          />

          {pokemon.moves.length > MOVES_PREVIEW ? (
            <Pressable
              onPress={() => setShowAllMoves((value) => !value)}
              accessibilityRole="button"
              style={styles.moreButton}
            >
              <Text style={styles.moreLabel}>
                {showAllMoves
                  ? 'Ver menos'
                  : `Ver los ${pokemon.moves.length} movimientos`}
              </Text>
            </Pressable>
          ) : null}
        </Section>
      </ScrollView>
    </>
  );
}

function Fact({ label, value }: { label: string; value: string }) {
  return (
    <View style={styles.fact}>
      <Text style={styles.factValue}>{value}</Text>
      <Text style={styles.factLabel}>{label}</Text>
    </View>
  );
}

const styles = StyleSheet.create({
  content: {
    padding: spacing.lg,
    paddingBottom: spacing.xxl * 2,
    gap: spacing.lg,
  },
  hero: {
    alignItems: 'center',
    paddingVertical: spacing.lg,
  },
  heroCircle: {
    position: 'absolute',
    top: 0,
    width: 220,
    height: 220,
    borderRadius: radius.pill,
  },
  heroImage: {
    width: 180,
    height: 180,
  },
  number: {
    marginTop: spacing.sm,
    fontSize: 13,
    fontWeight: '700',
    letterSpacing: 1,
    color: palette.textTertiary,
  },
  name: {
    fontSize: 28,
    fontWeight: '800',
    color: palette.text,
    marginTop: 2,
  },
  types: {
    flexDirection: 'row',
    gap: spacing.sm,
    marginTop: spacing.md,
  },
  genus: {
    marginTop: spacing.md,
    fontSize: 14,
    fontStyle: 'italic',
    color: palette.textSecondary,
  },
  facts: {
    flexDirection: 'row',
    gap: spacing.md,
  },
  fact: {
    flex: 1,
    alignItems: 'center',
    backgroundColor: palette.surface,
    borderRadius: radius.lg,
    paddingVertical: spacing.lg,
    ...shadow.card,
  },
  factValue: {
    fontSize: 17,
    fontWeight: '800',
    color: palette.text,
  },
  factLabel: {
    marginTop: 2,
    fontSize: 12,
    fontWeight: '600',
    color: palette.textTertiary,
  },
  description: {
    fontSize: 15,
    lineHeight: 23,
    color: palette.textSecondary,
  },
  stats: {
    gap: spacing.sm,
  },
  totalRow: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
    borderTopWidth: StyleSheet.hairlineWidth,
    borderTopColor: palette.border,
    paddingTop: spacing.md,
  },
  totalLabel: {
    fontSize: 13,
    fontWeight: '800',
    letterSpacing: 0.8,
    textTransform: 'uppercase',
    color: palette.textTertiary,
  },
  totalValue: {
    fontSize: 18,
    fontWeight: '800',
    color: palette.text,
  },
  abilities: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: spacing.sm,
  },
  ability: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.sm,
    backgroundColor: palette.surfaceMuted,
    borderRadius: radius.pill,
    paddingHorizontal: spacing.lg,
    paddingVertical: spacing.sm,
  },
  abilityName: {
    fontSize: 14,
    fontWeight: '600',
    color: palette.text,
    textTransform: 'capitalize',
  },
  abilityTag: {
    fontSize: 10,
    fontWeight: '800',
    letterSpacing: 0.6,
    textTransform: 'uppercase',
    color: palette.primary,
  },
  moreButton: {
    alignSelf: 'center',
    paddingVertical: spacing.sm,
    paddingHorizontal: spacing.lg,
    borderRadius: radius.pill,
    backgroundColor: palette.primarySoft,
  },
  moreLabel: {
    fontSize: 13,
    fontWeight: '700',
    color: palette.primaryDark,
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
