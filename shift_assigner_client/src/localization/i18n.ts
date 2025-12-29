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
      format: (value, format, lng) => {
        if (format === 'uppercase') return value.toUpperCase();
        if (format === 'lowercase') return value.toLowerCase();
        if (format === 'capitalize') return value.charAt(0).toUpperCase() + value.slice(1);
        if (format === 'currency') return new Intl.NumberFormat(lng, { style: 'currency', currency: 'USD' }).format(value);
        if (format === 'date') return new Intl.DateTimeFormat(lng).format(new Date(value));
        if (format === 'time') return new Intl.DateTimeFormat(lng, { timeStyle: 'short' }).format(new Date(value));
        return value;
      }
    },

    // Pluralization rules (built into i18next)
    pluralSeparator: '_',
    contextSeparator: '_',

    // Enable debugging in development
    debug: false, // Disable debug to avoid console spam
    
    // Namespace configuration for performance
    defaultNS: 'common',
    ns: ['common', 'tenantRegistration', 'auth', 'navigation'],
    
    // Key separator (supports nested keys like 'common.save')
    keySeparator: '.',
    nsSeparator: ':',
    
    // Return key if translation is missing (for debugging)
    returnNull: false,
    returnEmptyString: false,
    
    // Load missing keys
    saveMissing: __DEV__, // Only in development
    
    // Performance optimizations
    load: 'languageOnly', // Load 'en' instead of 'en-US'
    preload: supportedLanguages,
    
    // ICU options for advanced formatting
    icu: {
      memoize: true, // Cache parsed ICU messages
      bindI18n: 'languageChanged', // Re-parse on language change
    }
  })
  .catch((error) => {
    console.warn('i18n initialization failed:', error);
  });

export default i18n;