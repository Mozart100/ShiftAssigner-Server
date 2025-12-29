import { useTranslation } from 'react-i18next';
import { useAppDispatch, useAppSelector } from '../store';
import { TenantRegistrationActions } from '../store/tenantReducer';
import type { SupportedLanguage } from './i18n';
import { isRTL } from './i18n';

/**
 * Enhanced hook for language management with advanced features
 * Use this in any component that needs language switching
 */
export const useLanguage = (namespace?: string | string[]) => {
  const { t, i18n } = useTranslation(namespace);
  const dispatch = useAppDispatch();
  const currentLanguage = useAppSelector(state => state.tenantRegistration.currentLanguage);

  const changeLanguage = (language: SupportedLanguage) => {
    // Update Redux state - LanguageSync will handle i18n sync
    dispatch(TenantRegistrationActions.setLanguage(language));
  };

  // Advanced translation function with ICU support
  const tAdvanced = (key: string, options?: any) => {
    return t(key, {
      // Default options
      returnObjects: false,
      ...options
    });
  };

  // Pluralization helper
  const tPlural = (key: string, count: number, options?: any) => {
    return t(key, { count, ...options });
  };

  // ICU message format helper
  const tICU = (key: string, values: Record<string, any>) => {
    return t(key, { ...values, interpolation: { escapeValue: false } });
  };

  // Format helpers
  const formatters = {
    currency: (value: number) => new Intl.NumberFormat(currentLanguage, { 
      style: 'currency', 
      currency: 'USD' 
    }).format(value),
    
    date: (date: Date) => new Intl.DateTimeFormat(currentLanguage).format(date),
    
    time: (date: Date) => new Intl.DateTimeFormat(currentLanguage, { 
      timeStyle: 'short' 
    }).format(date),
    
    number: (value: number) => new Intl.NumberFormat(currentLanguage).format(value),
    
    percent: (value: number) => new Intl.NumberFormat(currentLanguage, { 
      style: 'percent' 
    }).format(value)
  };

  return {
    // Basic
    t: tAdvanced,
    currentLanguage,
    changeLanguage,
    isReady: i18n.isInitialized,
    
    // Advanced
    tPlural,
    tICU,
    formatters,
    
    // RTL support
    isRTL: isRTL(currentLanguage),
    direction: isRTL(currentLanguage) ? 'rtl' : 'ltr',
    textAlign: isRTL(currentLanguage) ? 'right' : 'left',
    
    // Namespace helpers
    exists: (key: string) => i18n.exists(key),
    getResource: (key: string) => i18n.getResource(currentLanguage, namespace as string, key)
  };
};