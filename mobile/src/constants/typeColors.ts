/**
 * Canonical colour of every Pokémon type.
 */
export const TYPE_COLORS: Record<string, string> = {
  normal: '#9FA19F',
  fire: '#E62829',
  water: '#2980EF',
  electric: '#FAC000',
  grass: '#3FA129',
  ice: '#3DCEF3',
  fighting: '#FF8000',
  poison: '#9141CB',
  ground: '#915121',
  flying: '#81B9EF',
  psychic: '#EF4179',
  bug: '#91A119',
  rock: '#AFA981',
  ghost: '#704170',
  dragon: '#5060E1',
  dark: '#624D4E',
  steel: '#60A1B8',
  fairy: '#EF70EF',
};

export const FALLBACK_TYPE_COLOR = '#8B93A7';

export function typeColor(type: string): string {
  return TYPE_COLORS[type.toLowerCase()] ?? FALLBACK_TYPE_COLOR;
}

/**
 * Types with a light background need dark text, the rest use white.
 */
export function typeTextColor(type: string): string {
  const lightTypes = new Set(['electric', 'ice', 'normal', 'rock', 'steel', 'flying']);
  return lightTypes.has(type.toLowerCase()) ? '#141829' : '#FFFFFF';
}

export function typeLabel(type: string): string {
  return type.charAt(0).toUpperCase() + type.slice(1);
}

/**
 * Colour of every base stat, following the classic Pokédex palette.
 */
export const STAT_COLORS: Record<string, string> = {
  hp: '#FF6B6B',
  attack: '#F2C14E',
  defense: '#4D96FF',
  'special-attack': '#FF9F68',
  'special-defense': '#4ECDC4',
  speed: '#7BC950',
};
