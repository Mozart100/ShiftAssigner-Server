import { useTranslation } from 'react-i18next';
import { useAppDispatch, useAppSelector } from '../store';
import { TenantRegistrationActions } from '../store/tenantReducer';
import type { SupportedLanguage } from './i18n';

/**
 * Custom hook for language management
 * Use this in any component that needs language switching
 */
export const useLanguage = () => {
  const { t, i18n } = useTranslation();
  const dispatch = useAppDispatch();
  const currentLanguage = useAppSelector(state => state.tenantRegistration.currentLanguage);

  const changeLanguage = (language: SupportedLanguage) => {
    // Update Redux state - LanguageSync will handle i18n sync
    dispatch(TenantRegistrationActions.setLanguage(language));
  };

  return {
    t,
    currentLanguage,
    changeLanguage,
    isReady: i18n.isInitialized
  };
};