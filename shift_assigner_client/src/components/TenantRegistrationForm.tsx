import React, { useState } from 'react';
import { Dimensions } from 'react-native';
import { useLanguage } from '../localization';
import { TenantRegistrationActions, BossTenant, submitTenantRegistration, initialTenantRegistrationState, TenantRegistrationReducer, TenantShiftScheduling } from '../store/tenantReducer';
import { RootState, useAppDispatch } from '../store';
import { useSelector } from 'react-redux';
import { GlobalSpinner } from './GlobalSpinner';
import { ToastContainer } from './ToastNotification';
import { showError, showSuccess } from '../store/toastReducer';
import { BasicInfoStep } from './BasicInfoStep';
import { ShiftConfigurationTable } from './ShiftConfigurationTable';
import { StepIndicator } from './StepIndicator';

// Design System Components
import {
  Heading4,
  SafeContainer,
  VStack,
  HStack,
  Button,
} from '../design-system';

type FormStep = 'basic-info' | 'shift-config';

export const TenantRegistrationForm: React.FC = () => {
  const { t } = useLanguage(['tenantRegistration', 'common']);
  const dispatch = useAppDispatch();
  const reduxTenant = useSelector<RootState, BossTenant>(state => state.tenantRegistration.tenant);
  const isSubmitting = useSelector<RootState, boolean>(state => state.tenantRegistration.isSubmitting);
  const error = useSelector<RootState, string | undefined>(state => state.tenantRegistration.error);
  const isSuccess = useSelector<RootState, boolean>(state => state.tenantRegistration.isSuccess);

  // Multi-step form state
  const [currentStep, setCurrentStep] = useState<FormStep>('basic-info');
  
  // Local form state - not connected to Redux until submit
  const [localTenant, setLocalTenant] = useState<BossTenant>(initialTenantRegistrationState.tenant);
  
  // State for password confirmation
  const [isPasswordConfirmed, setIsPasswordConfirmed] = useState(false);

  const updateLocalField = <K extends keyof BossTenant>(key: K, value: BossTenant[K]) => {
    setLocalTenant(prev => ({ ...prev, [key]: value }));
  };

  const handlePasswordConfirm = (confirmedPassword: string) => {
    updateLocalField('password', confirmedPassword);
    setIsPasswordConfirmed(true);
  };

  const handleShiftsUpdate = (shifts: TenantShiftScheduling[]) => {
    updateLocalField('shiftConfig', shifts);
  };

  // Validation for basic info step
  const getBasicInfoValidationError = () => {
    if (!localTenant.firstName.trim()) return "First name is required";
    if (!localTenant.lastName.trim()) return "Last name is required";
    if (!localTenant.phoneNumber.trim()) return "Phone number is required";
    if (!localTenant.tenant.trim()) return "Tenant name is required";
    if (!localTenant.password.trim()) return "Password is required";
    if (localTenant.password.length < 6) return "Password must be at least 6 characters";
    if (!isPasswordConfirmed) return "Please confirm your password";
    return null;
  };

  const isBasicInfoValid = getBasicInfoValidationError() === null;

  const handleContinue = () => {
    const validationError = getBasicInfoValidationError();
    if (validationError) {
      dispatch(showError(validationError));
      return;
    }
    
    // Initialize default shifts if none exist when moving to shift config step
    if (!localTenant.shiftConfig || localTenant.shiftConfig.length === 0) {
      const defaultShifts: TenantShiftScheduling[] = [
        {
          shiftName: "Morning",
          minimumAmountOfWorkers: 2,
          maximumAmountOfWorkers: 5
        },
        {
          shiftName: "Day",
          minimumAmountOfWorkers: 3,
          maximumAmountOfWorkers: 8
        },
        {
          shiftName: "Evening",
          minimumAmountOfWorkers: 2,
          maximumAmountOfWorkers: 6
        }
      ];
      updateLocalField('shiftConfig', defaultShifts);
    }
    
    setCurrentStep('shift-config');
  };

  const handleBack = () => {
    setCurrentStep('basic-info');
  };

  const handleSubmit = () => {
    // Final validation using the static method
    const validationError = TenantRegistrationReducer.validateTenant(localTenant);
    
    if (validationError) {
      dispatch(showError(validationError));
      return;
    }

    // Now update Redux store with all the local form data
    dispatch(TenantRegistrationActions.setTenant(localTenant));
    dispatch(submitTenantRegistration() as any);
  };

  const handleReset = () => {
    setLocalTenant(initialTenantRegistrationState.tenant);
    setIsPasswordConfirmed(false);
    setCurrentStep('basic-info');
    dispatch(TenantRegistrationActions.resetForm());
  };

  React.useEffect(() => {
    if (isSuccess) {
      dispatch(showSuccess(t('tenantRegistration:messages.success') as string));
      // Reset form after successful submission
      setTimeout(() => {
        handleReset();
      }, 2000);
    }
  }, [isSuccess]);

  React.useEffect(() => {
    if (error) {
      dispatch(showError(error));
    }
  }, [error]);

  const stepLabels = [String(t('tenantRegistration:stepBasicInfo')), String(t('tenantRegistration:stepShiftConfig'))];
  const currentStepNumber = currentStep === 'basic-info' ? 1 : 2;
  const screenData = Dimensions.get('window');
  const isLandscape = screenData.width > screenData.height;

  return (
    <>
      {/* Global loading and notifications */}
      <GlobalSpinner />
      <ToastContainer />
      
      <SafeContainer>
        <Heading4 align="center" style={{ marginBottom: isLandscape ? 16 : 24 }}>
          {String(t('tenantRegistration:title'))}
        </Heading4>

        {/* Step Indicator */}
        <StepIndicator
          currentStep={currentStepNumber}
          totalSteps={2}
          stepLabels={stepLabels}
        />

        {/* Step Content */}
        {currentStep === 'basic-info' ? (
          <BasicInfoStep
            tenant={localTenant}
            onUpdateField={updateLocalField}
            onPasswordConfirm={handlePasswordConfirm}
            isPasswordConfirmed={isPasswordConfirmed}
          />
        ) : (
          <ShiftConfigurationTable
            shifts={localTenant.shiftConfig || []}
            onUpdateShifts={handleShiftsUpdate}
          />
        )}

        {/* Navigation Buttons */}
        <VStack gap={2} style={{ 
          marginTop: isLandscape ? 20 : 32,
          maxWidth: isLandscape ? 600 : undefined,
          alignSelf: 'center',
          width: '100%'
        }}>
          {currentStep === 'basic-info' ? (
            <Button
              variant="primary"
              size="lg"
              fullWidth
              disabled={!isBasicInfoValid}
              onPress={handleContinue}
            >
              {String(t('tenantRegistration:continueToShiftConfig'))}
            </Button>
          ) : (
            <VStack gap={2}>
              <Button
                variant="primary"
                size="lg"
                fullWidth
                loading={isSubmitting}
                onPress={handleSubmit}
              >
                {isSubmitting ? String(t('tenantRegistration:creatingTenant')) : String(t('tenantRegistration:completeRegistration'))}
              </Button>
              <Button
                variant="outline-secondary"
                size="lg"
                fullWidth
                onPress={handleBack}
              >
                {String(t('tenantRegistration:back'))}
              </Button>
            </VStack>
          )}

          <Button
            variant="outline-secondary"
            size="md"
            fullWidth
            onPress={handleReset}
          >
            {String(t('tenantRegistration:resetForm'))}
          </Button>
        </VStack>
      </SafeContainer>
    </>
  );
};