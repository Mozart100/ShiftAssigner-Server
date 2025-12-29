// Re-export everything from localization
export { default as i18n, supportedLanguages } from './i18n';
export type { SupportedLanguage } from './i18n';
export { LanguageSync } from './LanguageSync';
export { useLanguage } from './useLanguage';

// Translation files
export { default as en } from './translations/en';
export { default as ru } from './translations/ru';