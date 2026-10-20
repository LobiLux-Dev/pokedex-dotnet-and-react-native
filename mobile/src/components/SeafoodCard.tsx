import { Pressable, StyleSheet, Text, View } from 'react-native';

import { InfoBadge, type BadgeTone } from '@/components/InfoBadge';
import { MetaItem } from '@/components/MetaItem';
import { PokemonImage } from '@/components/PokemonImage';
import { artworkUrl, palette, radius, shadow, spacing, spriteUrl } from '@/constants/theme';
import type { SeafoodSummary } from '@/lib/types';

interface SeafoodCardProps {
  entry: SeafoodSummary;
  onPress: () => void;
}

function sustainabilityTone(sustainability: string): BadgeTone {
  switch (sustainability) {
    case 'Excelente':
      return 'success';
    case 'Buena':
      return 'accent';
    case 'Moderada':
      return 'warning';
    default:
      return 'danger';
  }
}

export function SeafoodCard({ entry, onPress }: SeafoodCardProps) {
  return (
    <Pressable
      onPress={onPress}
      accessibilityRole="button"
      accessibilityLabel={`${entry.title}, ${entry.realName}`}
      style={({ pressed }) => [styles.card, pressed && styles.pressed]}
    >
      <View style={styles.header}>
        <PokemonImage
          uri={artworkUrl(entry.pokemonId)}
          fallbackUri={spriteUrl(entry.pokemonId)}
          style={styles.thumbnail}
        />

        <View style={styles.headline}>
          <Text style={styles.title} numberOfLines={1}>
            {entry.title}
          </Text>
          <Text style={styles.realName} numberOfLines={1}>
            {entry.realName}
          </Text>
        </View>
      </View>

      <View style={styles.badges}>
        <InfoBadge label={entry.category} tone="accent" />
        <InfoBadge
          label={entry.sustainability}
          tone={sustainabilityTone(entry.sustainability)}
        />
      </View>

      <View style={styles.metaRow}>
        <MetaItem icon="flame-outline" label={`${entry.calories} kcal`} />
        <MetaItem icon="fitness-outline" label={`${entry.proteinGrams} g proteína`} />
        <MetaItem icon="time-outline" label={`${entry.prepMinutes} min`} />
      </View>

      <Text style={styles.season} numberOfLines={1}>
        Temporada: {entry.season}
      </Text>
    </Pressable>
  );
}

const styles = StyleSheet.create({
  card: {
    backgroundColor: palette.surface,
    borderRadius: radius.lg,
    padding: spacing.lg,
    gap: spacing.sm,
    ...shadow.card,
  },
  pressed: {
    opacity: 0.8,
    transform: [{ scale: 0.99 }],
  },
  header: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.md,
  },
  thumbnail: {
    width: 64,
    height: 64,
  },
  headline: {
    flex: 1,
    gap: 2,
  },
  title: {
    fontSize: 17,
    fontWeight: '800',
    color: palette.text,
  },
  realName: {
    fontSize: 13,
    fontWeight: '600',
    color: palette.textSecondary,
    textTransform: 'capitalize',
  },
  badges: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: spacing.xs,
  },
  metaRow: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: spacing.md,
    marginTop: spacing.xs,
  },
  season: {
    fontSize: 12,
    color: palette.textTertiary,
    fontWeight: '600',
  },
});
