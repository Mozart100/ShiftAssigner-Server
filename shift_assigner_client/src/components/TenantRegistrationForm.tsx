import React, { useState } from 'react';
import {
  Alert,
  Switch
} from 'react-native';
import { useLanguage } from '../localization';
import { RoleState, TenantRegistrationActions, BossTenant, submitTenantRegistration } from '../store/tenantReducer';
import { RootState, useAppDispatch, useAppSelector } from '../store';
import { useSelector } from 'react-redux';
import { LanguageSwitcher } from './LanguageSwitcher';

// Design System Components
import {
  Typography,
  Heading4,
  Heading5,
  Label,
  SafeContainer,
  Section,
  VStack,
  HStack,
  Button,
  Input,
  Heading6,
  Heading3,
  Heading1,
  Heading2,
} from '../design-system';

export const TenantRegistrationForm: React.FC = () => {
  const { t, tPlural, tICU, isRTL, direction, textAlign } = useLanguage(['tenantRegistration', 'common']);
  const dispatch = useAppDispatch();
  const tenant = useSelector<RootState, BossTenant>(state => state.tenantRegistration.tenant);
  const isSubmitting = useSelector<RootState,boolean>(state=> state.tenantRegistration.isSubmitting);
  const error = useSelector<RootState, string | undefined>(state => state.tenantRegistration.error);
  
  // State for dynamic shifts
  const [shifts, setShifts] = useState<Array<{ id: string; name: string; enabled: boolean }>>([
    { id: '1', name: 'Morning Shift', enabled: false },
    { id: '2', name: 'Day Shift', enabled: false },
    { id: '3', name: 'Evening Shift', enabled: false }
  ]);
  const [newShiftName, setNewShiftName] = useState('');
  const isFormValid = useSelector<RootState, boolean>(state => {
    const { tenant } = state.tenantRegistration;
    return (
      tenant.firstName.trim() !== "" &&
      tenant.lastName.trim() !== "" &&
      tenant.phoneNumber.trim() !== "" &&
      tenant.tenant.trim() !== ""
    );
  });
  const isSuccess = useSelector<RootState, boolean>(state => state.tenantRegistration.isSuccess);

  const updateField = <K extends keyof typeof tenant>(key: K, value: typeof tenant[K]) => {
    dispatch(TenantRegistrationActions.setField({ key, value }));
  };

  const handleSubmit = () => {
    if (!isFormValid) {
      Alert.alert(t('common:error') as string, t('tenantRegistration:messages.fillRequired') as string);
      return;
    }
    dispatch(submitTenantRegistration() as any);
  };

  const handleShiftConfigChange = (shift: keyof NonNullable<typeof tenant.shiftConfig>, value: boolean) => {
    const currentConfig = tenant.shiftConfig || { morning: false, day: false, evening: false };
    const newConfig = { ...currentConfig, [shift]: value };
    dispatch(TenantRegistrationActions.setShiftConfig(newConfig));
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
      Alert.alert(t('common:success') as string, t('tenantRegistration:messages.success') as string);
    }
  }, [isSuccess, t]);

  React.useEffect(() => {
    if (error) {
      Alert.alert(t('common:error') as string, error);
    }
  }, [error, t]);

  return (
    <SafeContainer>
      <LanguageSwitcher />
      
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
            value={tenant.firstName}
            onChangeText={(value) => updateField('firstName', value)}
            placeholder={String(t('tenantRegistration:placeholders.firstName'))}
          />

          <Input
            label={`${String(t('tenantRegistration:lastName'))} *`}
            value={tenant.lastName}
            onChangeText={(value) => updateField('lastName', value)}
            placeholder={String(t('tenantRegistration:placeholders.lastName'))}
          />

          <Input
            label={`${String(t('tenantRegistration:phoneNumber'))} *`}
            value={tenant.phoneNumber}
            onChangeText={(value) => updateField('phoneNumber', value)}
            placeholder={String(t('tenantRegistration:placeholders.phoneNumber'))}
            keyboardType="phone-pad"
          />

          <Input
            label={`${String(t('tenantRegistration:dateOfBirth'))} *`}
            value={tenant.dateOfBirth}
            onChangeText={(value) => updateField('dateOfBirth', value)}
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
            value={tenant.tenant}
            onChangeText={(value) => updateField('tenant', value)}
            placeholder={String(t('tenantRegistration:placeholders.tenantName'))}
            size='lg'
          />

          <VStack gap={2}>
            <Label>{String(t('tenantRegistration:role'))}</Label>
            <HStack gap={2}>
              <Button
                variant={tenant.role === RoleState.Boss ? 'primary' : 'outline-primary'}
                size="md"
                style={{ flex: 1 }}
                onPress={() => updateField('role', RoleState.Boss)}
              >
                {String(t('tenantRegistration:roles.boss'))}
              </Button>
              <Button
                variant={tenant.role === RoleState.Admin ? 'primary' : 'outline-primary'}
                size="md"
                style={{ flex: 1 }}
                onPress={() => updateField('role', RoleState.Admin)}
              >
                {String(t('tenantRegistration:roles.admin'))}
              </Button>
            </HStack>
          </VStack>
        </VStack>
      </Section>

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
  );
};

