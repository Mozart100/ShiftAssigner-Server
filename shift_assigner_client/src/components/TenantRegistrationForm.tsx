import React, { useState } from 'react';
import {
  Alert} from 'react-native';
import { useLanguage } from '../localization';
import { TenantRegistrationActions, BossTenant, submitTenantRegistration, initialTenantRegistrationState, TenantRegistrationReducer } from '../store/tenantReducer';
import { RootState, useAppDispatch } from '../store';
import { useSelector } from 'react-redux';
import { PasswordConfirmation } from './ConfirmPassword';
import { GlobalSpinner } from './GlobalSpinner';
import { ToastContainer } from './ToastNotification';
import { showError, showSuccess } from '../store/toastReducer';

// Design System Components
import {
  Typography,
  Heading4,
  Heading5,
  SafeContainer,
  Section,
  VStack,
  HStack,
  Button,
  Input,
  Spinner,
} from '../design-system';

export const TenantRegistrationForm: React.FC = () => {
  const { t, tPlural, tICU, isRTL, direction, textAlign } = useLanguage(['tenantRegistration', 'common']);
  const dispatch = useAppDispatch();
  const reduxTenant = useSelector<RootState, BossTenant>(state => state.tenantRegistration.tenant);
  const isSubmitting = useSelector<RootState,boolean>(state=> state.tenantRegistration.isSubmitting);
  const error = useSelector<RootState, string | undefined>(state => state.tenantRegistration.error);
  
  // Local form state - not connected to Redux until submit
  const [localTenant, setLocalTenant] = useState<BossTenant>(initialTenantRegistrationState.tenant);


  
  // State for dynamic shifts
  const [shifts, setShifts] = useState<Array<{ id: string; name: string; enabled: boolean }>>([
    { id: '1', name: 'Morning Shift', enabled: false },
    { id: '2', name: 'Day Shift', enabled: false },
    { id: '3', name: 'Evening Shift', enabled: false }
  ]);
  const [newShiftName, setNewShiftName] = useState('');
  
  // State for password confirmation
  const [isPasswordConfirmed, setIsPasswordConfirmed] = useState(false);
  
  // Form validation using local state and reducer validation
  const getValidationError = () => {
    return TenantRegistrationReducer.validateTenant(localTenant);
  };
  
  const isSuccess = useSelector<RootState, boolean>(state => state.tenantRegistration.isSuccess);
  const isFormValid = getValidationError() === null && isPasswordConfirmed;

  const updateLocalField = <K extends keyof BossTenant>(key: K, value: BossTenant[K]) => {
    setLocalTenant(prev => ({ ...prev, [key]: value }));
  };

  const handleSubmit = () => {
    const validationError = getValidationError();
    
    if (validationError || !isPasswordConfirmed) {
      const errorMessage = validationError || "Please confirm your password";
      dispatch(showError(errorMessage));
      return;
    }

    // Now update Redux store with all the local form data
    dispatch(TenantRegistrationActions.setTenant(localTenant));
    dispatch(submitTenantRegistration() as any);
  };

  const handlePasswordConfirm = (confirmedPassword: string) => {
    updateLocalField('password', confirmedPassword);
    setIsPasswordConfirmed(true);
  };


  // Dynamic shift management functions
  const addShift = () => {
    if (newShiftName.trim()) {
      const newShift = {
        id: Date.now().toString(),
        name: newShiftName.trim(),
        enabled: false
      };
      setShifts([...shifts, newShift]);
      setNewShiftName('');
    }
  };

  const removeShift = (id: string) => {
    setShifts(shifts.filter(shift => shift.id !== id));
  };

  const toggleShift = (id: string) => {
    setShifts(shifts.map(shift => 
      shift.id === id ? { ...shift, enabled: !shift.enabled } : shift
    ));
  };

  React.useEffect(() => {
    if (isSuccess) {
      dispatch(showSuccess(t('tenantRegistration:messages.success') as string));
    }
  }, [isSuccess]);

  React.useEffect(() => {
    if (error) {
      dispatch(showError(error));
    }
  }, [error]);

  return (
    <>
      {/* Global loading handled by GlobalSpinner - outside SafeContainer for full screen coverage */}
      <GlobalSpinner />
      
      {/* Toast notifications */}
      <ToastContainer />
      
      <SafeContainer>
        {/* <LanguageSwitcher /> */}
      
      <Heading4 align="center" style={{ marginBottom: 24 }}>
        {String(t('tenantRegistration:title'))}
      </Heading4>

      {/* Personal Information */}
      <Section>
        <Heading5 style={{ marginBottom: 24 }}>
          {String(t('tenantRegistration:personalInfo'))}
        </Heading5>
        
        <VStack gap={4}>
          <Input
            label={`${String(t('tenantRegistration:firstName'))} *`}
            value={localTenant.firstName}
            onChangeText={(value) => updateLocalField('firstName', value)}
            placeholder={String(t('tenantRegistration:placeholders.firstName'))}
          />

          <Input
            label={`${String(t('tenantRegistration:lastName'))} *`}
            value={localTenant.lastName}
            onChangeText={(value) => updateLocalField('lastName', value)}
            placeholder={String(t('tenantRegistration:placeholders.lastName'))}
          />

          <Input
            label={`${String(t('tenantRegistration:phoneNumber'))} *`}
            value={localTenant.phoneNumber}
            onChangeText={(value) => updateLocalField('phoneNumber', value)}
            placeholder={String(t('tenantRegistration:placeholders.phoneNumber'))}
            keyboardType="phone-pad"
          />

          <Input
            label={`${String(t('tenantRegistration:dateOfBirth'))} *`}
            value={localTenant.dateOfBirth}
            onChangeText={(value) => updateLocalField('dateOfBirth', value)}
            placeholder={String(t('tenantRegistration:placeholders.dateOfBirth'))}
          />
        </VStack>
      </Section>

      {/* Tenant Information */}
      <Section>
        <Heading5 style={{ marginBottom: 16 }}>
          {String(t('tenantRegistration:tenantInfo'))}
        </Heading5>
        
        <VStack gap={4}>
          <Input
            label={`${String(t('tenantRegistration:tenantName'))} *`}
            value={localTenant.tenant}
            onChangeText={(value) => updateLocalField('tenant', value)}
            placeholder={String(t('tenantRegistration:placeholders.tenantName'))}
            size='lg'
          />

        </VStack>
      </Section>

      {/* Security Information */}
      <PasswordConfirmation
        onPasswordConfirm={handlePasswordConfirm}
        title="Security Information"
        minLength={6}
      />

      {/* Shift Configuration */}
      <Section>
        <Heading5 style={{ marginBottom: 16 }}>
          {String(t('tenantRegistration:shiftConfig'))}
        </Heading5>
        
        <VStack gap={4}>
          {/* Add new shift input */}
          <HStack gap={2}>
            <Input
              value={newShiftName}
              onChangeText={setNewShiftName}
              placeholder={String(t('tenantRegistration:shiftNamePlaceholder'))}
              containerStyle={{ flex: 1 }}
            />
            <Button
              variant="success"
              size="md"
              disabled={!newShiftName.trim()}
              onPress={addShift}
            >
              {String(t('tenantRegistration:addShift'))}
            </Button>
          </HStack>

          {/* Dynamic shift list */}
          {shifts.length === 0 ? (
            <Typography 
              variant="body2" 
              color="text-secondary" 
              align="center"
              italic
              style={{ paddingVertical: 16 }}
            >
              {String(t('tenantRegistration:noShifts'))}
            </Typography>
          ) : (
            <VStack gap={2}>
              {shifts.map((shift) => (
                <HStack 
                  key={shift.id} 
                  justify="space-between" 
                  align="center"
                  padding={3}
                  style={{
                    borderBottomWidth: 1,
                    borderBottomColor: '#f0f0f0',
                  }}
                >
                  <HStack align="center" gap={3} style={{ flex: 1 }}>
                    <Typography variant="body1" style={{ flex: 1 }}>
                      {shift.name}
                    </Typography>
                    <Button
                      variant="danger"
                      size="sm"
                      onPress={() => removeShift(shift.id)}
                    >
                      {String(t('tenantRegistration:removeShift'))}
                    </Button>
                  </HStack>
                </HStack>
              ))}
            </VStack>
          )}
        </VStack>
      </Section>

      {/* Action Buttons */}
      <VStack gap={2} style={{ marginTop: 24 }}>
        <Button
          variant="primary"
          size="lg"
          fullWidth
          loading={isSubmitting}
          disabled={!isFormValid}
          onPress={handleSubmit}
        >
          {isSubmitting ? String(t('tenantRegistration:submitting')) : String(t('tenantRegistration:register'))}
        </Button>

        <Button
          variant="outline-secondary"
          size="md"
          fullWidth
          onPress={() => dispatch(TenantRegistrationActions.resetForm())}
        >
          {String(t('tenantRegistration:resetForm'))}
        </Button>
      </VStack>
    </SafeContainer>
    </>
  );
};

