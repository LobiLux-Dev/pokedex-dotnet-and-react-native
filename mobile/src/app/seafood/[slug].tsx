import { Stack, useLocalSearchParams } from 'expo-router';
import { ActivityIndicator, ScrollView, StyleSheet, Text, View } from 'react-native';

import { InfoBadge, type BadgeTone } from '@/components/InfoBadge';
import { PokemonImage } from '@/components/PokemonImage';
import { Section } from '@/components/Section';
import { ErrorState } from '@/components/StateViews';
import { artworkUrl, palette, radius, shadow, spacing, spriteUrl } from '@/constants/theme';
import { useSeafoodEntry } from '@/hooks/useCatalogs';

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

export default function SeafoodDetailScreen() {
  const params = useLocalSearchParams<{ slug: string }>();
  const slug = Array.isArray(params.slug) ? params.slug[0] : params.slug;

  const query = useSeafoodEntry(slug);

  if (query.isPending) {
    return (
      <View style={styles.centered}>
        <ActivityIndicator size="large" color={palette.primary} />
        <Text style={styles.loadingLabel}>Cargando…</Text>
      </View>
    );
  }

  if (query.isError || !query.data) {
    return (
      <ErrorState
        message={
          query.error instanceof Error ? query.error.message : 'No se pudo cargar la entrada.'
        }
        onRetry={() => query.refetch()}
      />
    );
  }

  const entry = query.data;

  return (
    <>
      <Stack.Screen options={{ title: entry.title }} />

      <ScrollView
        contentContainerStyle={styles.content}
        showsVerticalScrollIndicator={false}
      >
        <View style={styles.hero}>
          <View style={styles.heroCircle}>
            <PokemonImage
              uri={artworkUrl(entry.pokemonId)}
              fallbackUri={spriteUrl(entry.pokemonId)}
              style={styles.heroImage}
            />
          </View>

          <Text style={styles.title}>{entry.title}</Text>
          <Text style={styles.realName}>{entry.realName}</Text>

          <View style={styles.badges}>
            <InfoBadge label={entry.category} tone="accent" />
            <InfoBadge
              label={`Sostenibilidad: ${entry.sustainability}`}
              tone={sustainabilityTone(entry.sustainability)}
            />
            <InfoBadge label={entry.season} tone="neutral" />
          </View>
        </View>

        <View style={styles.facts}>
          <Fact label="Calorías" value={`${entry.calories}`} unit="kcal" />
          <Fact label="Proteína" value={`${entry.proteinGrams}`} unit="g" />
          <Fact label="Grasa" value={`${entry.fatGrams}`} unit="g" />
        </View>

        <Section title="Origen y temporada">
          <Text style={styles.paragraph}>{entry.origin}</Text>
          <Text style={styles.paragraphMuted}>Temporada: {entry.season}</Text>
        </Section>

        <Section title="Preparación">
          <Text style={styles.paragraph}>{entry.preparation}</Text>
        </Section>

        <Section title="Sabor y textura">
          <Text style={styles.paragraph}>{entry.flavor}</Text>
        </Section>

        <Section title="Maridaje">
          <Text style={styles.paragraph}>{entry.pairing}</Text>
        </Section>

        <View style={styles.tip}>
          <Text style={styles.tipTitle}>En la cocina</Text>
          <Text style={styles.tipText}>
            {entry.tips ?? `Tiempo de preparación aproximado: ${entry.prepMinutes} minutos.`}
          </Text>
        </View>
      </ScrollView>
    </>
  );
}

function Fact({ label, value, unit }: { label: string; value: string; unit: string }) {
  return (
    <View style={styles.fact}>
      <Text style={styles.factValue}>
        {value}
        <Text style={styles.factUnit}> {unit}</Text>
      </Text>
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
    alignItems: 'center',
    gap: spacing.xs,
    paddingTop: spacing.sm,
  },
  heroCircle: {
    width: 190,
    height: 190,
    borderRadius: radius.pill,
    backgroundColor: palette.surfaceMuted,
    alignItems: 'center',
    justifyContent: 'center',
    marginBottom: spacing.sm,
  },
  heroImage: {
    width: 168,
    height: 168,
  },
  title: {
    fontSize: 26,
    fontWeight: '800',
    color: palette.text,
  },
  realName: {
    fontSize: 15,
    fontWeight: '600',
    color: palette.textSecondary,
    textTransform: 'capitalize',
  },
  badges: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    justifyContent: 'center',
    gap: spacing.xs,
    marginTop: spacing.sm,
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
    fontSize: 19,
    fontWeight: '800',
    color: palette.text,
  },
  factUnit: {
    fontSize: 12,
    fontWeight: '700',
    color: palette.textTertiary,
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
  paragraphMuted: {
    fontSize: 13,
    fontWeight: '600',
    color: palette.textTertiary,
  },
  tip: {
    backgroundColor: '#EAF3FB',
    borderRadius: radius.lg,
    padding: spacing.lg,
    gap: spacing.xs,
    borderWidth: 1,
    borderColor: '#CFE2F3',
  },
  tipTitle: {
    fontSize: 12,
    fontWeight: '800',
    letterSpacing: 0.8,
    textTransform: 'uppercase',
    color: '#2451C4',
  },
  tipText: {
    fontSize: 14,
    lineHeight: 20,
    color: '#26456E',
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
