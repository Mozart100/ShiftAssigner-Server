import React, { useState } from 'react';
import { Alert } from 'react-native';
import { useLanguage } from '../localization';

// Design System Components
import {
  Typography,
  Heading4,
  Heading5,
  SafeContainer,
  Section,
  VStack,
  Button,
  Input,
} from '../design-system';

// Types
interface ShiftLeaderFormData {
  ID: string;
  FirstName: string;
  LastName: string;
  PhoneNumber: string;
  DateOfBirth: string;
  Password: string;
}

export const ShiftLeaderRegistrationForm: React.FC = () => {
  const { t } = useLanguage(['shiftLeader', 'common']);

  // Form state
  const [formData, setFormData] = useState<ShiftLeaderFormData>({
    ID: '',
    FirstName: '',
    LastName: '',
    PhoneNumber: '',
    DateOfBirth: '',
    Password: '',
  });

  const [confirmPassword, setConfirmPassword] = useState('');
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [errors, setErrors] = useState<Partial<ShiftLeaderFormData>>({});

  // Update field helper
  const updateField = <K extends keyof ShiftLeaderFormData>(
    key: K,
    value: ShiftLeaderFormData[K]
  ) => {
    setFormData(prev => ({ ...prev, [key]: value }));
    // Clear error when user starts typing
    if (errors[key]) {
      setErrors(prev => ({ ...prev, [key]: undefined }));
    }
  };

  // Validation
  const validateForm = (): boolean => {
    const newErrors: Partial<ShiftLeaderFormData> = {};

    // Required field validation
    if (!formData.ID.trim()) {
      newErrors.ID = String(t('shiftLeader:errors.idRequired', 'ID is required'));
    }
    if (!formData.FirstName.trim()) {
      newErrors.FirstName = String(t('shiftLeader:errors.firstNameRequired', 'First name is required'));
    }
    if (!formData.LastName.trim()) {
      newErrors.LastName = String(t('shiftLeader:errors.lastNameRequired', 'Last name is required'));
    }
    if (!formData.PhoneNumber.trim()) {
      newErrors.PhoneNumber = String(t('shiftLeader:errors.phoneRequired', 'Phone number is required'));
    }
    if (!formData.DateOfBirth.trim()) {
      newErrors.DateOfBirth = String(t('shiftLeader:errors.dobRequired', 'Date of birth is required'));
    }
    if (!formData.Password.trim()) {
      newErrors.Password = String(t('shiftLeader:errors.passwordRequired', 'Password is required'));
    }

    // Password validation
    if (formData.Password && formData.Password.length < 6) {
      newErrors.Password = String(t('shiftLeader:errors.passwordTooShort', 'Password must be at least 6 characters'));
    }

    // Confirm password validation
    if (formData.Password !== confirmPassword) {
      Alert.alert(
        String(t('common:error', 'Error')),
        String(t('shiftLeader:errors.passwordMismatch', 'Passwords do not match'))
      );
      return false;
    }

    // Phone number format validation (basic)
    const phoneRegex = /^\+?[\d\s\-\(\)]{10,}$/;
    if (formData.PhoneNumber && !phoneRegex.test(formData.PhoneNumber)) {
      newErrors.PhoneNumber = String(t('shiftLeader:errors.phoneInvalid', 'Please enter a valid phone number'));
    }

    // Date validation (basic YYYY-MM-DD format)
    const dateRegex = /^\d{4}-\d{2}-\d{2}$/;
    if (formData.DateOfBirth && !dateRegex.test(formData.DateOfBirth)) {
      newErrors.DateOfBirth = String(t('shiftLeader:errors.dobInvalid', 'Please enter date in YYYY-MM-DD format'));
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  // Form submission
  const handleSubmit = async () => {
    if (!validateForm()) {
      return;
    }

    setIsSubmitting(true);
    
    try {
      // TODO: Implement actual API call to register shift leader
      console.log('Registering Shift Leader:', formData);
      
      // Simulate API call
      await new Promise(resolve => setTimeout(resolve, 2000));
      
      Alert.alert(
        String(t('common:success', 'Success')),
        String(t('shiftLeader:messages.registrationSuccess', 'Shift Leader registered successfully!'))
      );

      // Reset form
      setFormData({
        ID: '',
        FirstName: '',
        LastName: '',
        PhoneNumber: '',
        DateOfBirth: '',
        Password: '',
      });
      setConfirmPassword('');
      
    } catch (error) {
      Alert.alert(
        String(t('common:error', 'Error')),
        String(t('shiftLeader:messages.registrationError', 'Registration failed. Please try again.'))
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  // Form validation check
  const isFormValid = 
    formData.ID.trim() !== '' &&
    formData.FirstName.trim() !== '' &&
    formData.LastName.trim() !== '' &&
    formData.PhoneNumber.trim() !== '' &&
    formData.DateOfBirth.trim() !== '' &&
    formData.Password.trim() !== '' &&
    confirmPassword.trim() !== '' &&
    Object.keys(errors).length === 0;

  return (
    <SafeContainer>
      <Heading4 align="center" style={{ marginBottom: 24 }}>
        {String(t('shiftLeader:title', 'Shift Leader Registration'))}
      </Heading4>

      {/* Personal Information */}
      <Section>
        <Heading5 style={{ marginBottom: 16 }}>
          {String(t('shiftLeader:personalInfo', 'Personal Information'))}
        </Heading5>
        
        <VStack gap={4}>
          <Input
            label={`${String(t('shiftLeader:id', 'ID'))} *`}
            value={formData.ID}
            onChangeText={(value) => updateField('ID', value)}
            placeholder={String(t('shiftLeader:placeholders.id', 'Enter your ID'))}
            error={errors.ID}
            size="lg"
          />

          <Input
            label={`${String(t('shiftLeader:firstName', 'First Name'))} *`}
            value={formData.FirstName}
            onChangeText={(value) => updateField('FirstName', value)}
            placeholder={String(t('shiftLeader:placeholders.firstName', 'Enter first name'))}
            error={errors.FirstName}
            size="lg"
          />

          <Input
            label={`${String(t('shiftLeader:lastName', 'Last Name'))} *`}
            value={formData.LastName}
            onChangeText={(value) => updateField('LastName', value)}
            placeholder={String(t('shiftLeader:placeholders.lastName', 'Enter last name'))}
            error={errors.LastName}
            size="lg"
          />

          <Input
            label={`${String(t('shiftLeader:phoneNumber', 'Phone Number'))} *`}
            value={formData.PhoneNumber}
            onChangeText={(value) => updateField('PhoneNumber', value)}
            placeholder={String(t('shiftLeader:placeholders.phoneNumber', 'Enter phone number'))}
            keyboardType="phone-pad"
            error={errors.PhoneNumber}
            size="lg"
          />

          <Input
            label={`${String(t('shiftLeader:dateOfBirth', 'Date of Birth'))} *`}
            value={formData.DateOfBirth}
            onChangeText={(value) => updateField('DateOfBirth', value)}
            placeholder={String(t('shiftLeader:placeholders.dateOfBirth', 'YYYY-MM-DD'))}
            error={errors.DateOfBirth}
            size="lg"
          />
        </VStack>
      </Section>

      {/* Security Information */}
      <Section>
        <Heading5 style={{ marginBottom: 16 }}>
          {String(t('shiftLeader:securityInfo', 'Security Information'))}
        </Heading5>
        
        <VStack gap={4}>
          <Input
            label={`${String(t('shiftLeader:password', 'Password'))} *`}
            value={formData.Password}
            onChangeText={(value) => updateField('Password', value)}
            placeholder={String(t('shiftLeader:placeholders.password', 'Enter password'))}
            secureTextEntry
            error={errors.Password}
            helperText={String(t('shiftLeader:passwordHelp', 'Minimum 6 characters'))}
            size="lg"
          />

          <Input
            label={`${String(t('shiftLeader:confirmPassword', 'Confirm Password'))} *`}
            value={confirmPassword}
            onChangeText={setConfirmPassword}
            placeholder={String(t('shiftLeader:placeholders.confirmPassword', 'Confirm your password'))}
            secureTextEntry
            size="lg"
          />
        </VStack>
      </Section>

      {/* Submit Button */}
      <VStack gap={3} style={{ marginTop: 24 }}>
        <Button
          variant="primary"
          size="lg"
          fullWidth
          loading={isSubmitting}
          disabled={!isFormValid || isSubmitting}
          onPress={handleSubmit}
        >
          {isSubmitting 
            ? String(t('shiftLeader:submitting', 'Registering...'))
            : String(t('shiftLeader:register', 'Register Shift Leader'))
          }
        </Button>

        <Button
          variant="outline-secondary"
          size="md"
          fullWidth
          onPress={() => {
            setFormData({
              ID: '',
              FirstName: '',
              LastName: '',
              PhoneNumber: '',
              DateOfBirth: '',
              Password: '',
            });
            setConfirmPassword('');
            setErrors({});
          }}
        >
          {String(t('shiftLeader:resetForm', 'Reset Form'))}
        </Button>
      </VStack>

      {/* Information Notice */}
      <Section style={{ marginTop: 16 }}>
        <Typography variant="caption" color="text-secondary" align="center">
          {String(t('shiftLeader:notice', 'All fields marked with * are required. Your information will be used to create your shift leader account.'))}
        </Typography>
      </Section>
    </SafeContainer>
  );
};