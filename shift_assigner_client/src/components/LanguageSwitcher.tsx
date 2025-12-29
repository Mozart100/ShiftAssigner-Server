import React from 'react';
import { View, TouchableOpacity, Text, StyleSheet } from 'react-native';
import { useLanguage } from '../localization';

export const LanguageSwitcher: React.FC = () => {
  const { currentLanguage, changeLanguage } = useLanguage();

  return (
    <View style={styles.container}>
      <TouchableOpacity
        style={[styles.button, currentLanguage === 'en' && styles.activeButton]}
        onPress={() => changeLanguage('en')}
      >
        <Text style={[styles.buttonText, currentLanguage === 'en' && styles.activeButtonText]}>
          🇺🇸 EN
        </Text>
      </TouchableOpacity>

      <TouchableOpacity
        style={[styles.button, currentLanguage === 'ru' && styles.activeButton]}
        onPress={() => changeLanguage('ru')}
      >
        <Text style={[styles.buttonText, currentLanguage === 'ru' && styles.activeButtonText]}>
          🇷🇺 RU
        </Text>
      </TouchableOpacity>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flexDirection: 'row',
    alignItems: 'center',
    marginBottom: 16,
  },
  button: {
    paddingHorizontal: 12,
    paddingVertical: 6,
    marginHorizontal: 4,
    borderRadius: 4,
    borderWidth: 1,
    borderColor: '#ddd',
    backgroundColor: '#fff',
  },
  activeButton: {
    backgroundColor: '#007AFF',
    borderColor: '#007AFF',
  },
  buttonText: {
    fontSize: 14,
    color: '#333',
  },
  activeButtonText: {
    color: '#fff',
    fontWeight: '500',
  },
});