import { useInfiniteQuery, useQuery } from '@tanstack/react-query';

import { fetchPokemonDetail, fetchPokemonPage, fetchPokemonTypes } from '@/lib/api';

const PAGE_SIZE = 24;

export function usePokemonList(search: string, type: string | null) {
  return useInfiniteQuery({
    queryKey: ['pokemon', 'list', search, type],
    initialPageParam: 1,
    queryFn: ({ pageParam }) =>
      fetchPokemonPage({
        page: pageParam,
        pageSize: PAGE_SIZE,
        search: search.trim() || undefined,
        type: type ?? undefined,
      }),
    getNextPageParam: (lastPage) =>
      lastPage.hasNextPage ? lastPage.page + 1 : undefined,
  });
}

export function usePokemonDetail(nameOrId: string | undefined) {
  return useQuery({
    queryKey: ['pokemon', 'detail', nameOrId],
    queryFn: () => fetchPokemonDetail(nameOrId as string),
    enabled: Boolean(nameOrId),
  });
}

export function usePokemonTypes() {
  return useQuery({
    queryKey: ['pokemon', 'types'],
    queryFn: fetchPokemonTypes,
    staleTime: 60 * 60 * 1000,
  });
}
