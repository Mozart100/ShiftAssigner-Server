/**
 * Basic Info Step Component
 */

import React from 'react';
import { useLanguage } from '../localization';
import { BossTenant } from '../store/tenantReducer';
import { PasswordConfirmation } from './ConfirmPassword';
import {
  Heading5,
  Section,
  VStack,
  Input,
} from '../design-system';

interface BasicInfoStepProps {
  tenant: BossTenant;
  onUpdateField: <K extends keyof BossTenant>(key: K, value: BossTenant[K]) => void;
  onPasswordConfirm: (password: string) => void;
  isPasswordConfirmed: boolean;
}

export const BasicInfoStep: React.FC<BasicInfoStepProps> = ({
  tenant,
  onUpdateField,
  onPasswordConfirm,
  isPasswordConfirmed,
}) => {
  const { t } = useLanguage(['tenantRegistration', 'common']);

  return (
    <>
      {/* Personal Information */}
      <Section>
        <Heading5 style={{ marginBottom: 24 }}>
          {String(t('tenantRegistration:personalInfo'))}
        </Heading5>
        
        <VStack gap={4}>
          <Input
            label={`${String(t('tenantRegistration:firstName'))} *`}
            value={tenant.firstName}
            onChangeText={(value) => onUpdateField('firstName', value)}
            placeholder={String(t('tenantRegistration:placeholders.firstName'))}
          />

          <Input
            label={`${String(t('tenantRegistration:lastName'))} *`}
            value={tenant.lastName}
            onChangeText={(value) => onUpdateField('lastName', value)}
            placeholder={String(t('tenantRegistration:placeholders.lastName'))}
          />

          <Input
            label={`${String(t('tenantRegistration:phoneNumber'))} *`}
            value={tenant.phoneNumber}
            onChangeText={(value) => onUpdateField('phoneNumber', value)}
            placeholder={String(t('tenantRegistration:placeholders.phoneNumber'))}
            keyboardType="phone-pad"
          />

          <Input
            label={`${String(t('tenantRegistration:dateOfBirth'))} *`}
            value={tenant.dateOfBirth}
            onChangeText={(value) => onUpdateField('dateOfBirth', value)}
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
            onChangeText={(value) => onUpdateField('tenant', value)}
            placeholder={String(t('tenantRegistration:placeholders.tenantName'))}
            size='lg'
          />
        </VStack>
      </Section>

      {/* Security Information */}
      <PasswordConfirmation
        onPasswordConfirm={onPasswordConfirm}
        title="Security Information"
        minLength={6}
      />
    </>
  );
};