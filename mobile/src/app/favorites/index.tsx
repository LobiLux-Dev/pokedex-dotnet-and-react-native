import { useRouter } from 'expo-router';
import { FlatList, Pressable, StyleSheet, Text, View } from 'react-native';

import { PokemonImage } from '@/components/PokemonImage';
import { TypeBadge } from '@/components/TypeBadge';
import { EmptyState } from '@/components/StateViews';
import { artworkUrl, palette, radius, shadow, spacing, spriteUrl } from '@/constants/theme';
import { useFavorites } from '@/hooks/useFavorites';

export default function FavoritesScreen() {
  const router = useRouter();
  const { favorites, isReady, clearFavorites } = useFavorites();

  if (isReady && favorites.length === 0) {
    return (
      <EmptyState
        title="Aún no tienes favoritos"
        message="Pulsa la estrella en la ficha de un Pokémon para guardarlo aquí."
        icon="star-outline"
      />
    );
  }

  return (
    <FlatList
      data={favorites}
      keyExtractor={(item) => String(item.id)}
      numColumns={2}
      columnWrapperStyle={styles.column}
      contentContainerStyle={styles.content}
      renderItem={({ item }) => (
        <Pressable
          onPress={() =>
            router.push({
              pathname: '/favorites/pokemon/[name]',
              params: { name: item.name },
            })
          }
          accessibilityRole="button"
          accessibilityLabel={`${item.displayName}, número ${item.id}`}
          style={({ pressed }) => [styles.card, pressed && styles.cardPressed]}
        >
          <Text style={styles.number}>#{String(item.id).padStart(3, '0')}</Text>

          <PokemonImage
            uri={artworkUrl(item.id)}
            fallbackUri={spriteUrl(item.id)}
            style={styles.artwork}
          />

          <Text style={styles.name} numberOfLines={1}>
            {item.displayName}
          </Text>

          <View style={styles.types}>
            {item.types.map((type) => (
              <TypeBadge key={type} type={type} size="sm" />
            ))}
          </View>
        </Pressable>
      )}
      ListFooterComponent={
        favorites.length > 0 ? (
          <Pressable
            onPress={clearFavorites}
            accessibilityRole="button"
            style={styles.clearButton}
          >
            <Text style={styles.clearLabel}>Vaciar favoritos</Text>
          </Pressable>
        ) : null
      }
    />
  );
}

const styles = StyleSheet.create({
  content: {
    paddingHorizontal: spacing.lg,
    paddingTop: spacing.sm,
    paddingBottom: spacing.xxl,
  },
  column: {
    gap: spacing.md,
    marginBottom: spacing.md,
  },
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
  artwork: {
    width: 108,
    height: 108,
    marginTop: spacing.xs,
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
  clearButton: {
    alignSelf: 'center',
    marginTop: spacing.lg,
    paddingVertical: spacing.md,
    paddingHorizontal: spacing.xl,
    borderRadius: radius.pill,
    backgroundColor: palette.surface,
    borderWidth: 1,
    borderColor: palette.border,
  },
  clearLabel: {
    fontSize: 14,
    fontWeight: '700',
    color: palette.danger,
  },
});
