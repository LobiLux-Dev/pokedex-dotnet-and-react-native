import { Stack, useLocalSearchParams } from 'expo-router';
import { ActivityIndicator, ScrollView, StyleSheet, Text, View } from 'react-native';

import { InfoBadge, type BadgeTone } from '@/components/InfoBadge';
import { Section } from '@/components/Section';
import { ErrorState } from '@/components/StateViews';
import { palette, radius, shadow, spacing } from '@/constants/theme';
import { useRecipe } from '@/hooks/useCatalogs';

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

export default function RecipeDetailScreen() {
  const params = useLocalSearchParams<{ slug: string }>();
  const slug = Array.isArray(params.slug) ? params.slug[0] : params.slug;

  const query = useRecipe(slug);

  if (query.isPending) {
    return (
      <View style={styles.centered}>
        <ActivityIndicator size="large" color={palette.primary} />
        <Text style={styles.loadingLabel}>Cargando la receta…</Text>
      </View>
    );
  }

  if (query.isError || !query.data) {
    return (
      <ErrorState
        message={query.error instanceof Error ? query.error.message : 'No se pudo cargar la receta.'}
        onRetry={() => query.refetch()}
      />
    );
  }

  const recipe = query.data;

  return (
    <>
      <Stack.Screen options={{ title: recipe.title }} />

      <ScrollView
        contentContainerStyle={styles.content}
        showsVerticalScrollIndicator={false}
      >
        <View style={styles.hero}>
          <Text style={styles.title}>{recipe.title}</Text>
          <Text style={styles.summary}>{recipe.summary}</Text>

          <View style={styles.badges}>
            <InfoBadge label={recipe.category} tone="accent" />
            <InfoBadge
              label={recipe.difficulty}
              tone={difficultyTone(recipe.difficulty)}
            />
          </View>
        </View>

        <View style={styles.facts}>
          <Fact label="Preparación" value={`${recipe.prepMinutes} min`} />
          <Fact label="Cocción" value={`${recipe.cookMinutes} min`} />
          <Fact label="Raciones" value={String(recipe.servings)} />
        </View>

        <Section title="La receta">
          <Text style={styles.paragraph}>{recipe.description}</Text>
        </Section>

        {recipe.pokemon.length > 0 ? (
          <Section title="Pokémon de la receta">
            <View style={styles.chipRow}>
              {recipe.pokemon.map((pokemon) => (
                <View key={pokemon.id} style={styles.pokemonChip}>
                  <Text style={styles.pokemonName}>{pokemon.displayName}</Text>
                </View>
              ))}
            </View>
          </Section>
        ) : null}

        <Section title="Ingredientes">
          <View style={styles.ingredients}>
            {recipe.ingredients.map((ingredient, index) => (
              <View key={`${ingredient.name}-${index}`} style={styles.ingredient}>
                <View style={styles.quantityBox}>
                  <Text style={styles.quantity}>
                    {ingredient.quantity}
                    {ingredient.unit ? ` ${ingredient.unit}` : ''}
                  </Text>
                </View>
                <Text style={styles.ingredientName}>
                  {ingredient.name}
                  {ingredient.optional ? ' (opcional)' : ''}
                </Text>
              </View>
            ))}
          </View>
        </Section>

        <Section title="Pasos">
          <View style={styles.steps}>
            {recipe.steps.map((step) => (
              <View key={step.order} style={styles.step}>
                <View style={styles.stepBadge}>
                  <Text style={styles.stepNumber}>{step.order}</Text>
                </View>
                <View style={styles.stepBody}>
                  <Text style={styles.stepTitle}>{step.title}</Text>
                  <Text style={styles.stepDescription}>{step.description}</Text>
                  {step.durationMinutes ? (
                    <Text style={styles.stepDuration}>⏱ {step.durationMinutes} min</Text>
                  ) : null}
                </View>
              </View>
            ))}
          </View>
        </Section>

        {recipe.tags.length > 0 || recipe.allergens.length > 0 ? (
          <Section title="Etiquetas y alérgenos">
            <View style={styles.chipRow}>
              {recipe.tags.map((tag) => (
                <View key={tag} style={styles.tagChip}>
                  <Text style={styles.tagLabel}>#{tag}</Text>
                </View>
              ))}
              {recipe.allergens.map((allergen) => (
                <View key={allergen} style={styles.allergenChip}>
                  <Text style={styles.allergenLabel}>Contiene {allergen}</Text>
                </View>
              ))}
            </View>
          </Section>
        ) : null}

        {recipe.tips ? (
          <View style={styles.tip}>
            <Text style={styles.tipTitle}>Consejo del chef</Text>
            <Text style={styles.tipText}>{recipe.tips}</Text>
          </View>
        ) : null}
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
    width: '100%',
    maxWidth: 760,
    alignSelf: 'center',
  },
  hero: {
    gap: spacing.sm,
    paddingTop: spacing.sm,
  },
  title: {
    fontSize: 26,
    fontWeight: '800',
    color: palette.text,
    lineHeight: 32,
  },
  summary: {
    fontSize: 15,
    lineHeight: 22,
    color: palette.textSecondary,
  },
  badges: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: spacing.xs,
    marginTop: spacing.xs,
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
  paragraph: {
    fontSize: 15,
    lineHeight: 23,
    color: palette.textSecondary,
  },
  chipRow: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: spacing.sm,
  },
  pokemonChip: {
    backgroundColor: palette.primarySoft,
    borderRadius: radius.pill,
    paddingHorizontal: spacing.md,
    paddingVertical: 5,
  },
  pokemonName: {
    fontSize: 13,
    fontWeight: '700',
    color: palette.primaryDark,
  },
  ingredients: {
    gap: spacing.sm,
  },
  ingredient: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.md,
  },
  quantityBox: {
    minWidth: 74,
    paddingVertical: 4,
    paddingHorizontal: spacing.sm,
    borderRadius: radius.sm,
    backgroundColor: palette.surfaceMuted,
    alignItems: 'center',
  },
  quantity: {
    fontSize: 12,
    fontWeight: '800',
    color: palette.text,
  },
  ingredientName: {
    flex: 1,
    fontSize: 14,
    fontWeight: '600',
    color: palette.text,
  },
  steps: {
    gap: spacing.lg,
  },
  step: {
    flexDirection: 'row',
    gap: spacing.md,
  },
  stepBadge: {
    width: 28,
    height: 28,
    borderRadius: radius.pill,
    backgroundColor: palette.primary,
    alignItems: 'center',
    justifyContent: 'center',
  },
  stepNumber: {
    color: palette.onPrimary,
    fontWeight: '800',
    fontSize: 13,
  },
  stepBody: {
    flex: 1,
    gap: 2,
  },
  stepTitle: {
    fontSize: 15,
    fontWeight: '700',
    color: palette.text,
  },
  stepDescription: {
    fontSize: 14,
    lineHeight: 20,
    color: palette.textSecondary,
  },
  stepDuration: {
    fontSize: 12,
    fontWeight: '600',
    color: palette.textTertiary,
    marginTop: 2,
  },
  tagChip: {
    backgroundColor: palette.surfaceMuted,
    borderRadius: radius.pill,
    paddingHorizontal: spacing.md,
    paddingVertical: 5,
  },
  tagLabel: {
    fontSize: 12,
    fontWeight: '700',
    color: palette.textSecondary,
  },
  allergenChip: {
    backgroundColor: palette.primarySoft,
    borderRadius: radius.pill,
    paddingHorizontal: spacing.md,
    paddingVertical: 5,
  },
  allergenLabel: {
    fontSize: 12,
    fontWeight: '700',
    color: palette.primaryDark,
  },
  tip: {
    backgroundColor: '#FFF8E6',
    borderRadius: radius.lg,
    padding: spacing.lg,
    gap: spacing.xs,
    borderWidth: 1,
    borderColor: '#F2E2BC',
  },
  tipTitle: {
    fontSize: 12,
    fontWeight: '800',
    letterSpacing: 0.8,
    textTransform: 'uppercase',
    color: '#A96A12',
  },
  tipText: {
    fontSize: 14,
    lineHeight: 20,
    color: '#6B5316',
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
