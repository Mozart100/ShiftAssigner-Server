import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';
import ICU from 'i18next-icu';
import * as Localization from 'expo-localization';

import en from './translations/en';
import ru from './translations/ru';

// Supported languages with RTL info
export const supportedLanguages = ['en', 'ru'] as const;
export type SupportedLanguage = typeof supportedLanguages[number];

// RTL language detection
export const RTL_LANGUAGES = ['ar', 'he', 'fa'] as const;
export const isRTL = (language: string): boolean => RTL_LANGUAGES.includes(language as any);

// Get device language or fallback to English
export const getDeviceLanguage = (): SupportedLanguage => {
  try {
    const locale = Localization.getLocales()?.[0]?.languageTag || 'en-US';
    const deviceLanguage = locale.split('-')[0]; // 'en-US' -> 'en'
    return supportedLanguages.includes(deviceLanguage as SupportedLanguage) 
      ? (deviceLanguage as SupportedLanguage) 
      : 'en';
  } catch (error) {
    console.warn('Failed to get device language:', error);
    return 'en';
  }
};

// Translation resources with namespaces
const resources = {
  en: en,
  ru: ru
};

// Initialize i18next with advanced features
i18n
  .use(ICU) // Enable ICU formatting for plurals and advanced interpolation
  .use(initReactI18next)
  .init({
    resources,
    lng: getDeviceLanguage(), // Default to device language
    fallbackLng: 'en',
    
    // Advanced interpolation with ICU
    interpolation: {
      escapeValue: false, // React already escapes values
    },

    // Enable debugging in development
    debug: false,
    
    // Namespace configuration for performance
    defaultNS: 'common',
    ns: ['common', 'tenantRegistration', 'shiftLeader', 'registration', 'auth', 'navigation'],
    
    // Key separator (supports nested keys like 'common.save')
    keySeparator: '.',
    nsSeparator: ':',
    
    // Return key if translation is missing (for debugging)
    returnNull: false,
    returnEmptyString: false,
    
    // Performance optimizations
    load: 'languageOnly', // Load 'en' instead of 'en-US'
    preload: supportedLanguages,
  } as any)
  .catch((error: any) => {
    console.warn('i18n initialization failed:', error);
  });

export default i18n;