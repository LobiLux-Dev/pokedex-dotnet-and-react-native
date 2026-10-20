import Ionicons from '@expo/vector-icons/Ionicons';
import { Pressable, StyleSheet } from 'react-native';

import { palette } from '@/constants/theme';

interface FavoriteButtonProps {
  isFavorite: boolean;
  onPress: () => void;
  color?: string;
}

/**
 * Star button used in the detail screen header.
 */
export function FavoriteButton({
  isFavorite,
  onPress,
  color = palette.primary,
}: FavoriteButtonProps) {
  return (
    <Pressable
      onPress={onPress}
      hitSlop={12}
      accessibilityRole="button"
      accessibilityLabel={isFavorite ? 'Quitar de favoritos' : 'Añadir a favoritos'}
      accessibilityState={{ selected: isFavorite }}
      style={({ pressed }) => [styles.button, pressed && styles.pressed]}
    >
      <Ionicons
        name={isFavorite ? 'star' : 'star-outline'}
        size={22}
        color={isFavorite ? color : palette.textTertiary}
      />
    </Pressable>
  );
}

const styles = StyleSheet.create({
  button: {
    padding: 6,
  },
  pressed: {
    opacity: 0.6,
    transform: [{ scale: 0.92 }],
  },
});
