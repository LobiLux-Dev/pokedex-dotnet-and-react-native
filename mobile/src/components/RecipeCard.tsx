import { Pressable, StyleSheet, Text, View } from 'react-native';

import { InfoBadge, type BadgeTone } from '@/components/InfoBadge';
import { MetaItem } from '@/components/MetaItem';
import { palette, radius, shadow, spacing } from '@/constants/theme';
import type { RecipeSummary } from '@/lib/types';

interface RecipeCardProps {
  recipe: RecipeSummary;
  onPress: () => void;
}

function difficultyTone(difficulty: string): BadgeTone {
  switch (difficulty) {
    case 'Fácil':
      return 'success';
    case 'Media':
      return 'warning';
    default:
      return 'danger';
  }
}

export function RecipeCard({ recipe, onPress }: RecipeCardProps) {
  return (
    <Pressable
      onPress={onPress}
      accessibilityRole="button"
      accessibilityLabel={recipe.title}
      style={({ pressed }) => [styles.card, pressed && styles.pressed]}
    >
      <Text style={styles.title} numberOfLines={2}>
        {recipe.title}
      </Text>

      <Text style={styles.summary} numberOfLines={2}>
        {recipe.summary}
      </Text>

      <View style={styles.badges}>
        <InfoBadge label={recipe.category} tone="accent" />
        <InfoBadge label={recipe.difficulty} tone={difficultyTone(recipe.difficulty)} />
      </View>

      <View style={styles.metaRow}>
        <MetaItem icon="time-outline" label={`${recipe.totalMinutes} min`} />
        <MetaItem icon="people-outline" label={`${recipe.servings} raciones`} />
        <MetaItem icon="list-outline" label={`${recipe.ingredientCount} ingredientes`} />
      </View>

      <View style={styles.pokemonRow}>
        {recipe.pokemon.map((pokemon) => (
          <View key={pokemon.id} style={styles.pokemonChip}>
            <Text style={styles.pokemonName}>{pokemon.displayName}</Text>
          </View>
        ))}
      </View>
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
  title: {
    fontSize: 17,
    fontWeight: '800',
    color: palette.text,
  },
  summary: {
    fontSize: 13,
    lineHeight: 19,
    color: palette.textSecondary,
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
  pokemonRow: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: spacing.xs,
    marginTop: spacing.xs,
  },
  pokemonChip: {
    backgroundColor: palette.primarySoft,
    borderRadius: radius.pill,
    paddingHorizontal: spacing.md,
    paddingVertical: 3,
  },
  pokemonName: {
    fontSize: 11,
    fontWeight: '700',
    color: palette.primaryDark,
  },
});
