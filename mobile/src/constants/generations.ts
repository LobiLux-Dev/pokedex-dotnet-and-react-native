/**
 * National Pokédex generations. The ranges are a fixed fact, so they live in the app;
 * the filtering itself happens in the API (`?generation=`).
 */
export const GENERATIONS = [
  { value: 1, label: 'Gen I' },
  { value: 2, label: 'Gen II' },
  { value: 3, label: 'Gen III' },
  { value: 4, label: 'Gen IV' },
  { value: 5, label: 'Gen V' },
  { value: 6, label: 'Gen VI' },
  { value: 7, label: 'Gen VII' },
  { value: 8, label: 'Gen VIII' },
  { value: 9, label: 'Gen IX' },
] as const;
