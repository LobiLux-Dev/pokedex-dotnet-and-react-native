import AsyncStorage from '@react-native-async-storage/async-storage';
import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react';

import type { PokemonSummary } from '@/lib/types';

const STORAGE_KEY = 'pokedex.favorites.v1';

export interface Favorite {
  id: number;
  name: string;
  displayName: string;
  types: string[];
}

interface FavoritesContextValue {
  favorites: Favorite[];
  isReady: boolean;
  isFavorite: (id: number) => boolean;
  toggleFavorite: (pokemon: PokemonSummary) => void;
  clearFavorites: () => void;
}

const FavoritesContext = createContext<FavoritesContextValue | null>(null);

export function FavoritesProvider({ children }: { children: ReactNode }) {
  const [favorites, setFavorites] = useState<Favorite[]>([]);
  const [isReady, setIsReady] = useState(false);

  useEffect(() => {
    let cancelled = false;

    async function load() {
      try {
        const raw = await AsyncStorage.getItem(STORAGE_KEY);
        if (!cancelled && raw) {
          const parsed = JSON.parse(raw) as Favorite[];
          if (Array.isArray(parsed)) {
            setFavorites(parsed);
          }
        }
      } catch {
        // A corrupted or unavailable storage must not break the app: start with no favourites.
      } finally {
        if (!cancelled) {
          setIsReady(true);
        }
      }
    }

    void load();

    return () => {
      cancelled = true;
    };
  }, []);

  const persist = useCallback((next: Favorite[]) => {
    AsyncStorage.setItem(STORAGE_KEY, JSON.stringify(next)).catch(() => {
      // Storage failures are not recoverable from the UI, the in-memory state stays correct.
    });
  }, []);

  const toggleFavorite = useCallback(
    (pokemon: PokemonSummary) => {
      setFavorites((previous) => {
        const alreadySaved = previous.some((favorite) => favorite.id === pokemon.id);
        const next = alreadySaved
          ? previous.filter((favorite) => favorite.id !== pokemon.id)
          : [
              ...previous,
              {
                id: pokemon.id,
                name: pokemon.name,
                displayName: pokemon.displayName,
                types: pokemon.types,
              },
            ];

        persist(next);
        return next;
      });
    },
    [persist],
  );

  const clearFavorites = useCallback(() => {
    setFavorites([]);
    persist([]);
  }, [persist]);

  const isFavorite = useCallback(
    (id: number) => favorites.some((favorite) => favorite.id === id),
    [favorites],
  );

  const value = useMemo<FavoritesContextValue>(
    () => ({ favorites, isReady, isFavorite, toggleFavorite, clearFavorites }),
    [favorites, isReady, isFavorite, toggleFavorite, clearFavorites],
  );

  return <FavoritesContext.Provider value={value}>{children}</FavoritesContext.Provider>;
}

export function useFavorites(): FavoritesContextValue {
  const context = useContext(FavoritesContext);
  if (!context) {
    throw new Error('useFavorites must be used inside a FavoritesProvider.');
  }
  return context;
}
