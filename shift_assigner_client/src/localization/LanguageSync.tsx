import React, { useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { useAppSelector, useAppDispatch } from '../store';
import { TenantRegistrationActions } from '../store/tenantReducer';
import { getDeviceLanguage } from './i18n';

/**
 * Language synchronization component
 * Keeps Redux state in sync with i18next
 * Should be placed at the root of the app
 */
export const LanguageSync: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { i18n } = useTranslation();
  const dispatch = useAppDispatch();
  const currentLanguage = useAppSelector(state => state.tenantRegistration.currentLanguage);

  // Initialize language from device settings
  useEffect(() => {
    const deviceLanguage = getDeviceLanguage();
    if (currentLanguage === 'en' && deviceLanguage !== 'en') {
      // Only update if Redux still has default value
      dispatch(TenantRegistrationActions.setLanguage(deviceLanguage));
    }
  }, [dispatch, currentLanguage]);

  // Sync Redux language changes to i18n
  useEffect(() => {
    if (i18n.language !== currentLanguage) {
      i18n.changeLanguage(currentLanguage);
    }
  }, [currentLanguage, i18n]);

  // Sync i18n language changes back to Redux (if changed externally)
  useEffect(() => {
    const handleLanguageChange = (lng: string) => {
      if (lng !== currentLanguage && (lng === 'en' || lng === 'ru')) {
        dispatch(TenantRegistrationActions.setLanguage(lng as any));
      }
    };

    i18n.on('languageChanged', handleLanguageChange);
    return () => i18n.off('languageChanged', handleLanguageChange);
  }, [i18n, currentLanguage, dispatch]);

  return <>{children}</>;
};