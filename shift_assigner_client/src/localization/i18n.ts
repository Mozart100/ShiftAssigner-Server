import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import * as Localization from 'expo-localization';

import en from './translations/en';
import ru from './translations/ru';

// Supported languages
export const supportedLanguages = ['en', 'ru'] as const;
export type SupportedLanguage = typeof supportedLanguages[number];

// Get device language or fallback to English
export const getDeviceLanguage = (): SupportedLanguage => {
  try {
    const locale = Localization.locale || Localization.locales?.[0] || 'en-US';
    const deviceLanguage = locale.split('-')[0]; // 'en-US' -> 'en'
    return supportedLanguages.includes(deviceLanguage as SupportedLanguage) 
      ? (deviceLanguage as SupportedLanguage) 
      : 'en';
  } catch (error) {
    console.warn('Failed to get device language:', error);
    return 'en';
  }
};

// Translation resources
const resources = {
  en: { translation: en },
  ru: { translation: ru }
};

// Initialize i18next
i18n
  .use(initReactI18next)
  .init({
    resources,
    lng: getDeviceLanguage(), // Default to device language
    fallbackLng: 'en',
    
    interpolation: {
      escapeValue: false, // React already escapes values
    },

    // Enable debugging in development
    debug: false, // Disable debug to avoid console spam
    
    // Namespace configuration
    defaultNS: 'translation',
    
    // Key separator (supports nested keys like 'common.save')
    keySeparator: '.',
    
    // Return key if translation is missing (for debugging)
    returnNull: false,
    returnEmptyString: false,
  })
  .catch((error) => {
    console.warn('i18n initialization failed:', error);
  });

export default i18n;