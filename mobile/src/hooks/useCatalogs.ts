import { useInfiniteQuery, useQuery } from '@tanstack/react-query';

import {
  fetchRecipe,
  fetchRecipeFacets,
  fetchRecipes,
  fetchSeafood,
  fetchSeafoodEntry,
  fetchSeafoodFacets,
} from '@/lib/api';

const RECIPE_PAGE_SIZE = 12;
const SEAFOOD_PAGE_SIZE = 12;

export interface RecipeFilters {
  search: string;
  category: string | null;
  difficulty: string | null;
  maxMinutes: number | null;
}

export function useRecipes(filters: RecipeFilters) {
  return useInfiniteQuery({
    queryKey: ['recipes', 'list', filters],
    initialPageParam: 1,
    queryFn: ({ pageParam }) =>
      fetchRecipes({
        page: pageParam,
        pageSize: RECIPE_PAGE_SIZE,
        search: filters.search.trim() || undefined,
        category: filters.category ?? undefined,
        difficulty: filters.difficulty ?? undefined,
        maxMinutes: filters.maxMinutes ?? undefined,
      }),
    getNextPageParam: (lastPage) =>
      lastPage.hasNextPage ? lastPage.page + 1 : undefined,
  });
}

export function useRecipe(idOrSlug: string | undefined) {
  return useQuery({
    queryKey: ['recipes', 'detail', idOrSlug],
    queryFn: () => fetchRecipe(idOrSlug as string),
    enabled: Boolean(idOrSlug),
  });
}

export function useRecipeFacets() {
  return useQuery({
    queryKey: ['recipes', 'facets'],
    queryFn: fetchRecipeFacets,
    staleTime: 60 * 60 * 1000,
  });
}

export interface SeafoodFilters {
  search: string;
  category: string | null;
  sustainability: string | null;
  maxPrepMinutes: number | null;
}

export function useSeafood(filters: SeafoodFilters) {
  return useInfiniteQuery({
    queryKey: ['seafood', 'list', filters],
    initialPageParam: 1,
    queryFn: ({ pageParam }) =>
      fetchSeafood({
        page: pageParam,
        pageSize: SEAFOOD_PAGE_SIZE,
        search: filters.search.trim() || undefined,
        category: filters.category ?? undefined,
        sustainability: filters.sustainability ?? undefined,
        maxPrepMinutes: filters.maxPrepMinutes ?? undefined,
      }),
    getNextPageParam: (lastPage) =>
      lastPage.hasNextPage ? lastPage.page + 1 : undefined,
  });
}

export function useSeafoodEntry(idOrSlug: string | undefined) {
  return useQuery({
    queryKey: ['seafood', 'detail', idOrSlug],
    queryFn: () => fetchSeafoodEntry(idOrSlug as string),
    enabled: Boolean(idOrSlug),
  });
}

export function useSeafoodFacets() {
  return useQuery({
    queryKey: ['seafood', 'facets'],
    queryFn: fetchSeafoodFacets,
    staleTime: 60 * 60 * 1000,
  });
}
