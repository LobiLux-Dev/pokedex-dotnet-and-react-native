/**
 * Design tokens of the app. Every colour, spacing and radius used by the UI comes from here.
 */

export const palette = {
  background: '#F2F4F9',
  surface: '#FFFFFF',
  surfaceMuted: '#EDF0F7',
  border: '#E1E5F0',
  text: '#141829',
  textSecondary: '#5A6178',
  textTertiary: '#8B93A7',
  primary: '#E63946',
  primaryDark: '#C1121F',
  primarySoft: '#FDECEE',
  accent: '#3A6FF7',
  onPrimary: '#FFFFFF',
  danger: '#D7263D',
  success: '#2A9D8F',
  overlay: 'rgba(20, 24, 41, 0.55)',
  track: '#E7EAF3',
} as const;

export const spacing = {
  xs: 4,
  sm: 8,
  md: 12,
  lg: 16,
  xl: 20,
  xxl: 28,
} as const;

export const radius = {
  sm: 8,
  md: 12,
  lg: 16,
  xl: 22,
  pill: 999,
} as const;

export const fontSize = {
  xs: 11,
  sm: 13,
  md: 15,
  lg: 18,
  xl: 22,
  xxl: 28,
  display: 36,
} as const;

export const shadow = {
  card: {
    shadowColor: '#0B1020',
    shadowOpacity: 0.06,
    shadowRadius: 12,
    shadowOffset: { width: 0, height: 4 },
    elevation: 2,
  },
  raised: {
    shadowColor: '#0B1020',
    shadowOpacity: 0.12,
    shadowRadius: 18,
    shadowOffset: { width: 0, height: 8 },
    elevation: 6,
  },
} as const;

/** Official sprite host used by PokeAPI. */
export const SPRITE_BASE_URL =
  'https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon';

/** Official artwork host used by PokeAPI. */
export const ARTWORK_BASE_URL =
  'https://raw.githubusercontent.com/PokeAPI/sprites/master/sprites/pokemon/other/official-artwork';

export function spriteUrl(id: number): string {
  return `${SPRITE_BASE_URL}/${id}.png`;
}

export function artworkUrl(id: number): string {
  return `${ARTWORK_BASE_URL}/${id}.png`;
}
