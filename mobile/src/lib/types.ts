/**
 * Shapes returned by the .NET API. They mirror the DTOs of the Pokedex.Api project.
 */

export interface PokemonSummary {
  id: number;
  name: string;
  displayName: string;
  types: string[];
  spriteUrl: string;
  artworkUrl: string;
}

export interface PagedResponse<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface PokemonAbility {
  name: string;
  isHidden: boolean;
}

export interface PokemonStat {
  name: string;
  baseValue: number;
  effort: number;
}

export interface PokemonSprites {
  frontDefault: string | null;
  frontShiny: string | null;
  backDefault: string | null;
  backShiny: string | null;
  frontFemale: string | null;
  frontShinyFemale: string | null;
}

export interface PokemonMove {
  name: string;
  type: string | null;
  damageClass: string | null;
  power: number | null;
  accuracy: number | null;
  level: number;
  learnMethod: string;
  versionGroup: string;
}

export interface EvolutionDetail {
  trigger: string | null;
  minLevel: number | null;
  item: string | null;
  heldItem: string | null;
  knownMove: string | null;
  timeOfDay: string | null;
  turnUpsideDown: boolean | null;
  gender: number | null;
}

export interface EvolutionNode {
  speciesName: string;
  details: EvolutionDetail[];
  evolvesTo: EvolutionNode[];
}

export interface EvolutionChain {
  id: number;
  root: EvolutionNode;
}

export interface PokemonDetail {
  id: number;
  name: string;
  displayName: string;
  types: string[];
  heightInMeters: number;
  weightInKg: number;
  baseExperience: number | null;
  totalStats: number;
  abilities: PokemonAbility[];
  stats: PokemonStat[];
  sprites: PokemonSprites;
  genus: string | null;
  description: string | null;
  habitat: string | null;
  color: string | null;
  isLegendary: boolean;
  isMythical: boolean;
  moves: PokemonMove[];
  evolution: EvolutionChain | null;
}

export interface PokemonType {
  name: string;
  pokemonCount: number;
}
