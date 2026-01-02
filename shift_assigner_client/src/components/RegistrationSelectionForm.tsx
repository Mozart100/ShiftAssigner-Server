import React, { useState } from 'react';
import { useLanguage } from '../localization';

// Design System Components
import {
  Typography,
  Heading2,
  Body,
  SafeContainer,
  VStack,
  HStack,
  Button,
  Card,
} from '../design-system';

// Import the registration forms
import { TenantRegistrationForm } from './TenantRegistrationForm';
import { ShiftLeaderRegistrationForm } from './ShiftLeaderRegistrationForm';

// Types
type RegistrationType = 'selection' | 'tenant' | 'shiftLeader';

export const RegistrationSelectionForm: React.FC = () => {
  const { t } = useLanguage(['registration', 'common']);
  const [selectedType, setSelectedType] = useState<RegistrationType>('selection');

  // Handle selection
  const handleSelection = (type: 'tenant' | 'shiftLeader') => {
    setSelectedType(type);
  };

  // Handle back to selection
  const handleBackToSelection = () => {
    setSelectedType('selection');
  };

  // Render the appropriate form based on selection
  if (selectedType === 'tenant') {
    return (
      <SafeContainer>
        <VStack gap={4} style={{ marginBottom: 24 }}>
          <Button
            variant="ghost"
            size="sm"
            onPress={handleBackToSelection}
            style={{ alignSelf: 'flex-start' }}
          >
            ← {String(t('common:back', 'Back'))}
          </Button>
        </VStack>
        <TenantRegistrationForm />
      </SafeContainer>
    );
  }

  if (selectedType === 'shiftLeader') {
    return (
      <SafeContainer>
        <VStack gap={4} style={{ marginBottom: 24 }}>
          <Button
            variant="ghost"
            size="sm"
            onPress={handleBackToSelection}
            style={{ alignSelf: 'flex-start' }}
          >
            ← {String(t('common:back', 'Back'))}
          </Button>
        </VStack>
        <ShiftLeaderRegistrationForm />
      </SafeContainer>
    );
  }

  // Selection screen
  return (
    <SafeContainer 
      contentContainerStyle={{ 
        justifyContent: 'center', 
        paddingTop: 60,
        flexGrow: 1 
      }}
    >
      <VStack gap={6} align="center">
        {/* Header */}
        <VStack gap={3} align="center">
          <Heading2 align="center" weight="bold">
            {String(t('registration:title', 'Register'))}
          </Heading2>
          
          <Body 
            color="text-secondary" 
            align="center" 
            style={{ maxWidth: 300, lineHeight: 24 }}
          >
            {String(t('registration:subtitle', 'Sign in to your account or register your company.'))}
          </Body>
        </VStack>

        {/* Selection Cards */}
        <VStack gap={4} style={{ width: '100%', maxWidth: 400 }}>
          {/* Shift Leader Option */}
          <Card 
            padding={0} 
            shadow="sm" 
            style={{ overflow: 'hidden' }}
          >
            <Button
              variant="ghost"
              size="lg"
              fullWidth
              onPress={() => handleSelection('shiftLeader')}
              style={{
                paddingVertical: 20,
                paddingHorizontal: 24,
                justifyContent: 'flex-start',
                backgroundColor: 'transparent',
                borderRadius: 0,
              }}
            >
              <VStack gap={1} align="flex-start" style={{ width: '100%' }}>
                <Typography variant="h5" weight="semibold">
                  {String(t('registration:shiftLeader', 'Shift Leader'))}
                </Typography>
                <Typography variant="body2" color="text-secondary">
                  {String(t('registration:shiftLeaderDesc', 'Register as a shift leader to manage teams'))}
                </Typography>
              </VStack>
            </Button>
          </Card>

          {/* Tenant Option */}
          <Card 
            padding={0} 
            shadow="sm" 
            style={{ overflow: 'hidden' }}
          >
            <Button
              variant="ghost"
              size="lg"
              fullWidth
              onPress={() => handleSelection('tenant')}
              style={{
                paddingVertical: 20,
                paddingHorizontal: 24,
                justifyContent: 'flex-start',
                backgroundColor: 'transparent',
                borderRadius: 0,
              }}
            >
              <VStack gap={1} align="flex-start" style={{ width: '100%' }}>
                <Typography variant="h5" weight="semibold">
                  {String(t('registration:tenant', 'Tenant'))}
                </Typography>
                <Typography variant="body2" color="text-secondary">
                  {String(t('registration:tenantDesc', 'Register your company and manage operations'))}
                </Typography>
              </VStack>
            </Button>
          </Card>
        </VStack>

        {/* Alternative: Button Style (Simpler) */}
        <VStack gap={3} style={{ width: '100%', maxWidth: 320, marginTop: 20 }}>
          <Typography variant="overline" color="text-secondary" align="center">
            {String(t('registration:orChoose', 'Or choose registration type:'))}
          </Typography>
          
          <HStack gap={3}>
            <Button
              variant="outline-primary"
              size="lg"
              style={{ flex: 1 }}
              onPress={() => handleSelection('shiftLeader')}
            >
              {String(t('registration:shiftLeader', 'Shift Leader'))}
            </Button>
            
            <Button
              variant="primary"
              size="lg"
              style={{ flex: 1 }}
              onPress={() => handleSelection('tenant')}
            >
              {String(t('registration:tenant', 'Tenant'))}
            </Button>
          </HStack>
        </VStack>

        {/* Footer Info */}
        <VStack gap={2} style={{ marginTop: 40 }}>
          <Typography variant="caption" color="text-secondary" align="center">
            {String(t('registration:helpText', 'Not sure which option to choose?'))}
          </Typography>
          <Typography variant="caption" color="primary" align="center">
            {String(t('registration:contactUs', 'Contact support for guidance'))}
          </Typography>
        </VStack>
      </VStack>
    </SafeContainer>
  );
};