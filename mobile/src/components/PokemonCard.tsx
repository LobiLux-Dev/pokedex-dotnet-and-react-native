import { Pressable, StyleSheet, Text, View } from 'react-native';

import { PokemonImage } from '@/components/PokemonImage';
import { TypeBadge } from '@/components/TypeBadge';
import { palette, radius, shadow, spacing } from '@/constants/theme';
import type { PokemonSummary } from '@/lib/types';

interface PokemonCardProps {
  pokemon: PokemonSummary;
  onPress: () => void;
}

/**
 * Card of the catalogue grid.
 */
export function PokemonCard({ pokemon, onPress }: PokemonCardProps) {
  return (
    <Pressable
      onPress={onPress}
      accessibilityRole="button"
      accessibilityLabel={`${pokemon.displayName}, número ${pokemon.id}`}
      style={({ pressed }) => [styles.card, pressed && styles.cardPressed]}
    >
      <Text style={styles.number}>#{String(pokemon.id).padStart(3, '0')}</Text>

      <View style={styles.artworkWrapper}>
        <PokemonImage
          uri={pokemon.artworkUrl}
          fallbackUri={pokemon.spriteUrl}
          style={styles.artwork}
        />
      </View>

      <Text style={styles.name} numberOfLines={1}>
        {pokemon.displayName}
      </Text>

      <View style={styles.types}>
        {pokemon.types.map((type) => (
          <TypeBadge key={type} type={type} size="sm" />
        ))}
      </View>
    </Pressable>
  );
}

const styles = StyleSheet.create({
  card: {
    flexBasis: '47%',
    maxWidth: '48%',
    backgroundColor: palette.surface,
    borderRadius: radius.lg,
    paddingVertical: spacing.md,
    paddingHorizontal: spacing.md,
    alignItems: 'center',
    ...shadow.card,
  },
  cardPressed: {
    opacity: 0.75,
    transform: [{ scale: 0.98 }],
  },
  number: {
    alignSelf: 'flex-start',
    fontSize: 12,
    fontWeight: '700',
    color: palette.textTertiary,
    letterSpacing: 0.5,
  },
  artworkWrapper: {
    width: '100%',
    height: 112,
    alignItems: 'center',
    justifyContent: 'center',
    marginTop: spacing.xs,
  },
  artwork: {
    width: 112,
    height: 112,
  },
  name: {
    marginTop: spacing.sm,
    fontSize: 15,
    fontWeight: '700',
    color: palette.text,
    textAlign: 'center',
  },
  types: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    justifyContent: 'center',
    gap: spacing.xs,
    marginTop: spacing.sm,
  },
});
