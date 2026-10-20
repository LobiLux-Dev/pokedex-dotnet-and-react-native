import Constants from 'expo-constants';
import { Platform } from 'react-native';

import type {
  PagedResponse,
  PokemonDetail,
  PokemonSummary,
  PokemonType,
  RecipeDetail,
  RecipeFacets,
  RecipeSummary,
  SeafoodDetail,
  SeafoodFacets,
  SeafoodSummary,
} from './types';

/** Port the .NET API listens on (see src/Pokedex.Api/Properties/launchSettings.json). */
const DEFAULT_PORT = 5080;

const PROBE_TIMEOUT_MS = 2500;
const REQUEST_TIMEOUT_MS = 20000;

/**
 * Candidate hosts, ordered by likelihood:
 *  1. the URL configured through EXPO_PUBLIC_API_URL
 *  2. the host used by the Expo dev server (works on a physical device over Wi-Fi)
 *  3. the platform default (10.0.2.2 on the Android emulator, localhost elsewhere)
 */
function candidateBaseUrls(): string[] {
  const configured = process.env.EXPO_PUBLIC_API_URL?.trim();
  if (configured) {
    return [configured.replace(/\/+$/, '')];
  }

  const candidates: string[] = [];

  const hostUri =
    Constants.expoConfig?.hostUri ?? Constants.expoGoConfig?.hostUri ?? undefined;
  if (hostUri) {
    const host = hostUri.split(':')[0];
    if (host && host !== 'localhost' && host !== '127.0.0.1') {
      candidates.push(`http://${host}:${DEFAULT_PORT}`);
    }
  }

  if (Platform.OS === 'web' && typeof window !== 'undefined' && window.location.hostname) {
    const host = window.location.hostname;
    if (host !== 'localhost' && host !== '127.0.0.1') {
      candidates.push(`http://${host}:${DEFAULT_PORT}`);
    }
  }

  if (Platform.OS === 'android') {
    candidates.push(`http://10.0.2.2:${DEFAULT_PORT}`);
  } else {
    candidates.push(`http://localhost:${DEFAULT_PORT}`);
  }

  return Array.from(new Set(candidates));
}

async function probe(baseUrl: string): Promise<boolean> {
  const controller = new AbortController();
  const timer = setTimeout(() => controller.abort(), PROBE_TIMEOUT_MS);

  try {
    const response = await fetch(`${baseUrl}/health`, { signal: controller.signal });
    return response.ok;
  } catch {
    return false;
  } finally {
    clearTimeout(timer);
  }
}

let resolvedBaseUrl: string | null = null;
let resolving: Promise<string> | null = null;

/**
 * Finds the API host once and keeps it for the rest of the session.
 */
export function resolveBaseUrl(): Promise<string> {
  if (resolvedBaseUrl) {
    return Promise.resolve(resolvedBaseUrl);
  }

  if (!resolving) {
    resolving = (async () => {
      const candidates = candidateBaseUrls();

      for (const candidate of candidates) {
        if (await probe(candidate)) {
          resolvedBaseUrl = candidate;
          return candidate;
        }
      }

      resolvedBaseUrl = candidates[0] ?? `http://localhost:${DEFAULT_PORT}`;
      return resolvedBaseUrl;
    })();
  }

  return resolving;
}

export class ApiError extends Error {
  constructor(
    message: string,
    readonly status?: number,
  ) {
    super(message);
    this.name = 'ApiError';
  }
}

async function request<T>(path: string): Promise<T> {
  const baseUrl = await resolveBaseUrl();
  const controller = new AbortController();
  const timer = setTimeout(() => controller.abort(), REQUEST_TIMEOUT_MS);

  try {
    const response = await fetch(`${baseUrl}${path}`, {
      signal: controller.signal,
      headers: { Accept: 'application/json' },
    });

    if (!response.ok) {
      let detail = `Error ${response.status}`;
      try {
        const body = (await response.json()) as { detail?: string; title?: string };
        detail = body?.detail ?? body?.title ?? detail;
      } catch {
        // The body is not a JSON problem document, keep the generic message.
      }
      throw new ApiError(detail, response.status);
    }

    return (await response.json()) as T;
  } catch (error) {
    if (error instanceof ApiError) {
      throw error;
    }

    const reason = error instanceof Error ? error.message : 'error de red';
    throw new ApiError(`No se pudo conectar con la API (${baseUrl}). ${reason}`);
  } finally {
    clearTimeout(timer);
  }
}

export interface PokemonListParams {
  page: number;
  pageSize: number;
  search?: string;
  type?: string;
}

export function fetchPokemonPage(
  params: PokemonListParams,
): Promise<PagedResponse<PokemonSummary>> {
  const query = new URLSearchParams({
    page: String(params.page),
    pageSize: String(params.pageSize),
  });

  if (params.search) {
    query.set('search', params.search);
  }
  if (params.type) {
    query.set('type', params.type);
  }

  return request<PagedResponse<PokemonSummary>>(`/api/v1/pokemon?${query.toString()}`);
}

export function fetchPokemonDetail(nameOrId: string): Promise<PokemonDetail> {
  return request<PokemonDetail>(`/api/v1/pokemon/${encodeURIComponent(nameOrId)}`);
}

export function fetchPokemonTypes(): Promise<PokemonType[]> {
  return request<PokemonType[]>('/api/v1/types');
}

/* ------------------------------------------------------------------ */
/* Recetario                                                          */
/* ------------------------------------------------------------------ */

export interface RecipeListParams {
  page: number;
  pageSize: number;
  search?: string;
  category?: string;
  difficulty?: string;
  maxMinutes?: number;
  tag?: string;
}

export function fetchRecipes(params: RecipeListParams): Promise<PagedResponse<RecipeSummary>> {
  const query = new URLSearchParams({
    page: String(params.page),
    pageSize: String(params.pageSize),
  });

  if (params.search) query.set('search', params.search);
  if (params.category) query.set('category', params.category);
  if (params.difficulty) query.set('difficulty', params.difficulty);
  if (params.maxMinutes) query.set('maxMinutes', String(params.maxMinutes));
  if (params.tag) query.set('tag', params.tag);

  return request<PagedResponse<RecipeSummary>>(`/api/v1/recipes?${query.toString()}`);
}

export function fetchRecipe(idOrSlug: string): Promise<RecipeDetail> {
  return request<RecipeDetail>(`/api/v1/recipes/${encodeURIComponent(idOrSlug)}`);
}

export function fetchRecipeFacets(): Promise<RecipeFacets> {
  return request<RecipeFacets>('/api/v1/recipes/facets');
}

/* ------------------------------------------------------------------ */
/* Seafood                                                            */
/* ------------------------------------------------------------------ */

export interface SeafoodListParams {
  page: number;
  pageSize: number;
  search?: string;
  category?: string;
  sustainability?: string;
  maxPrepMinutes?: number;
}

export function fetchSeafood(params: SeafoodListParams): Promise<PagedResponse<SeafoodSummary>> {
  const query = new URLSearchParams({
    page: String(params.page),
    pageSize: String(params.pageSize),
  });

  if (params.search) query.set('search', params.search);
  if (params.category) query.set('category', params.category);
  if (params.sustainability) query.set('sustainability', params.sustainability);
  if (params.maxPrepMinutes) query.set('maxPrepMinutes', String(params.maxPrepMinutes));

  return request<PagedResponse<SeafoodSummary>>(`/api/v1/seafood?${query.toString()}`);
}

export function fetchSeafoodEntry(idOrSlug: string): Promise<SeafoodDetail> {
  return request<SeafoodDetail>(`/api/v1/seafood/${encodeURIComponent(idOrSlug)}`);
}

export function fetchSeafoodFacets(): Promise<SeafoodFacets> {
  return request<SeafoodFacets>('/api/v1/seafood/facets');
}

/** Base URL currently in use, useful to show it in the error screens. */
export function currentBaseUrl(): string {
  return resolvedBaseUrl ?? candidateBaseUrls()[0] ?? `http://localhost:${DEFAULT_PORT}`;
}
