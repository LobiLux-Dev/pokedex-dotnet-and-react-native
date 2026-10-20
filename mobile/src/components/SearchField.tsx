import Ionicons from '@expo/vector-icons/Ionicons';
import { Pressable, StyleSheet, TextInput, View } from 'react-native';

import { palette, radius, spacing } from '@/constants/theme';

interface SearchFieldProps {
  value: string;
  onChangeText: (value: string) => void;
  placeholder?: string;
}

/**
 * Rounded search field used to filter the catalogue by name or number.
 */
export function SearchField({
  value,
  onChangeText,
  placeholder = 'Buscar por nombre o número',
}: SearchFieldProps) {
  return (
    <View style={styles.container}>
      <Ionicons name="search" size={18} color={palette.textTertiary} />
      <TextInput
        value={value}
        onChangeText={onChangeText}
        placeholder={placeholder}
        placeholderTextColor={palette.textTertiary}
        style={styles.input}
        autoCapitalize="none"
        autoCorrect={false}
        returnKeyType="search"
        clearButtonMode="never"
        accessibilityLabel="Buscar Pokémon"
      />
      {value.length > 0 ? (
        <Pressable
          onPress={() => onChangeText('')}
          hitSlop={10}
          accessibilityRole="button"
          accessibilityLabel="Limpiar búsqueda"
        >
          <Ionicons name="close-circle" size={18} color={palette.textTertiary} />
        </Pressable>
      ) : null}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flexDirection: 'row',
    alignItems: 'center',
    gap: spacing.sm,
    backgroundColor: palette.surface,
    borderRadius: radius.pill,
    paddingHorizontal: spacing.lg,
    height: 46,
    borderWidth: 1,
    borderColor: palette.border,
  },
  input: {
    flex: 1,
    fontSize: 15,
    color: palette.text,
    padding: 0,
  },
});
