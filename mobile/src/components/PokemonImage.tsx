import { Image } from 'expo-image';
import { useCallback, useState } from 'react';
import type { ImageStyle, StyleProp } from 'react-native';

interface PokemonImageProps {
  uri: string;
  fallbackUri?: string;
  style?: StyleProp<ImageStyle>;
  contentFit?: 'contain' | 'cover' | 'fill';
  transition?: number;
}

/**
 * Image with a graceful fallback: if the official artwork is missing the small sprite is used.
 */
export function PokemonImage({
  uri,
  fallbackUri,
  style,
  contentFit = 'contain',
  transition = 200,
}: PokemonImageProps) {
  const [source, setSource] = useState(uri);

  const handleError = useCallback(() => {
    if (fallbackUri && source !== fallbackUri) {
      setSource(fallbackUri);
    }
  }, [fallbackUri, source]);

  return (
    <Image
      source={{ uri: source }}
      style={style}
      contentFit={contentFit}
      transition={transition}
      onError={handleError}
      accessibilityIgnoresInvertColors
    />
  );
}
