import { PokemonDetailScreen } from '@/components/PokemonDetailScreen';

/**
 * Detail of a Pokémon reached from the favourites tab. It keeps the favourites stack
 * independent from the Pokédex one.
 */
export default function FavoriteDetailRoute() {
  return <PokemonDetailScreen />;
}
