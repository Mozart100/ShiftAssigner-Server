import React from 'react';
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  StyleSheet,
  ScrollView,
  Alert,
  Switch
} from 'react-native';
import { useLanguage } from '../localization';
import { RoleState, TenantRegistrationActions, BossTenant, submitTenantRegistration } from '../store/tenantReducer';
import { RootState, useAppDispatch, useAppSelector } from '../store';
import { useSelector } from 'react-redux';
import { LanguageSwitcher } from './LanguageSwitcher';

export const TenantRegistrationForm: React.FC = () => {
  const { t, tPlural, tICU, isRTL, direction } = useLanguage(['tenantRegistration', 'common']);
  const dispatch = useAppDispatch();
  const tenant = useSelector<AppState, BossTenant>(state => state.tenantRegistration.tenant);
  const isSubmitting = useSelector<AppState,boolean>(state=> state.tenantRegistration.isSubmitting);
  const error = useSelector<AppState, string | undefined>(state => state.tenantRegistration.error);
  const isFormValid = useSelector<AppState, boolean>(state => {
    const { tenant } = state.tenantRegistration;
    return (
      tenant.firstName.trim() !== "" &&
      tenant.lastName.trim() !== "" &&
      tenant.phoneNumber.trim() !== "" &&
      tenant.tenant.trim() !== ""
    );
  });
  const isSuccess = useSelector<AppState, boolean>(state => state.tenantRegistration.isSuccess);

  const updateField = <K extends keyof typeof tenant>(key: K, value: typeof tenant[K]) => {
    dispatch(TenantRegistrationActions.setField({ key, value }));
  };

  const handleSubmit = () => {
    if (!isFormValid) {
      Alert.alert(t('common:error'), t('tenantRegistration:messages.fillRequired'));
      return;
    }
    dispatch(submitTenantRegistration());
  };

  const handleShiftConfigChange = (shift: keyof NonNullable<typeof tenant.shiftConfig>, value: boolean) => {
    const currentConfig = tenant.shiftConfig || { morning: false, day: false, evening: false };
    const newConfig = { ...currentConfig, [shift]: value };
    dispatch(TenantRegistrationActions.setShiftConfig(newConfig));
  };

  React.useEffect(() => {
    if (isSuccess) {
      Alert.alert(t('common:success'), t('tenantRegistration:messages.success'));
    }
  }, [isSuccess, t]);

  React.useEffect(() => {
    if (error) {
      Alert.alert(t('common:error'), error);
    }
  }, [error, t]);

  return (
    <ScrollView 
      style={[styles.container, { direction }]} 
      contentContainerStyle={styles.contentContainer}
      showsVerticalScrollIndicator={false}
    >
      <LanguageSwitcher />
      <Text style={[styles.title, { textAlign: isRTL ? 'right' : 'center' }]}>
        {t('tenantRegistration:title')}
      </Text>

      {/* Personal Information */}
      <View style={styles.section}>
        <Text style={[styles.sectionTitle, { textAlign: isRTL ? 'right' : 'left' }]}>
          {t('tenantRegistration:personalInfo')}
        </Text>
        
        <View style={styles.inputGroup}>
          <Text style={[styles.label, { textAlign: isRTL ? 'right' : 'left' }]}>
            {t('tenantRegistration:firstName')} *
          </Text>
          <TextInput
            style={[styles.input, { textAlign: isRTL ? 'right' : 'left' }]}
            value={tenant.firstName}
            onChangeText={(value) => updateField('firstName', value)}
            placeholder={t('tenantRegistration:placeholders.firstName')}
          />
        </View>

        <View style={styles.inputGroup}>
          <Text style={[styles.label, { textAlign: isRTL ? 'right' : 'left' }]}>
            {t('tenantRegistration:lastName')} *
          </Text>
          <TextInput
            style={[styles.input, { textAlign: isRTL ? 'right' : 'left' }]}
            value={tenant.lastName}
            onChangeText={(value) => updateField('lastName', value)}
            placeholder={t('tenantRegistration:placeholders.lastName')}
          />
        </View>

        <View style={styles.inputGroup}>
          <Text style={styles.label}>Phone Number *</Text>
          <TextInput
            style={styles.input}
            value={tenant.phoneNumber}
            onChangeText={(value) => updateField('phoneNumber', value)}
            placeholder="Enter phone number"
            keyboardType="phone-pad"
          />
        </View>

        <View style={styles.inputGroup}>
          <Text style={styles.label}>Date of Birth *</Text>
          <TextInput
            style={styles.input}
            value={tenant.dateOfBirth}
            onChangeText={(value) => updateField('dateOfBirth', value)}
            placeholder="YYYY-MM-DD"
          />
        </View>
      </View>

      {/* Tenant Information */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Tenant Information</Text>
        
        <View style={styles.inputGroup}>
          <Text style={styles.label}>Tenant Name *</Text>
          <TextInput
            style={styles.input}
            value={tenant.tenant}
            onChangeText={(value) => updateField('tenant', value)}
            placeholder="Enter tenant/company name"
          />
        </View>

        <View style={styles.inputGroup}>
          <Text style={styles.label}>Role</Text>
          <View style={styles.roleContainer}>
          <TouchableOpacity 
            style={[styles.roleButton, tenant.role === RoleState.Boss && styles.roleButtonActive]}
            onPress={() => updateField('role', RoleState.Boss)}
          >
            <Text style={[styles.roleButtonText, tenant.role === RoleState.Boss && styles.roleButtonTextActive]}>
              Boss
            </Text>
          </TouchableOpacity>
          <TouchableOpacity 
            style={[styles.roleButton, tenant.role === RoleState.Admin && styles.roleButtonActive]}
            onPress={() => updateField('role', RoleState.Admin)}
          >
            <Text style={[styles.roleButtonText, tenant.role === RoleState.Admin && styles.roleButtonTextActive]}>
              Admin
            </Text>
          </TouchableOpacity>
          </View>
        </View>
      </View>

      {/* Shift Configuration */}
      <View style={styles.section}>
        <Text style={styles.sectionTitle}>Shift Configuration</Text>
        
        <View style={styles.shiftRow}>
          <Text style={styles.shiftLabel}>Morning Shift</Text>
          <Switch
            value={tenant.shiftConfig?.morning || false}
            onValueChange={(value) => handleShiftConfigChange('morning', value)}
          />
        </View>

        <View style={styles.shiftRow}>
          <Text style={styles.shiftLabel}>Day Shift</Text>
          <Switch
            value={tenant.shiftConfig?.day || false}
            onValueChange={(value) => handleShiftConfigChange('day', value)}
          />
        </View>

        <View style={styles.shiftRow}>
          <Text style={styles.shiftLabel}>Evening Shift</Text>
          <Switch
            value={tenant.shiftConfig?.evening || false}
            onValueChange={(value) => handleShiftConfigChange('evening', value)}
          />
        </View>
      </View>

      {/* Submit Button */}
      <TouchableOpacity
        style={[styles.submitButton, (!isFormValid || isSubmitting) && styles.submitButtonDisabled]}
        onPress={handleSubmit}
        disabled={!isFormValid || isSubmitting}
      >
        <Text style={styles.submitButtonText}>
          {isSubmitting ? 'Submitting...' : 'Register Tenant'}
        </Text>
      </TouchableOpacity>

      {/* Reset Button */}
      <TouchableOpacity
        style={styles.resetButton}
        onPress={() => dispatch(TenantRegistrationActions.resetForm())}
      >
        <Text style={styles.resetButtonText}>Reset Form</Text>
      </TouchableOpacity>

      <View style={styles.spacer} />
    </ScrollView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#f5f5f5',
  },
  contentContainer: {
    flexGrow: 1,
    padding: 16,
  },
  title: {
    fontSize: 24,
    fontWeight: 'bold',
    textAlign: 'center',
    marginBottom: 24,
    color: '#333',
  },
  section: {
    backgroundColor: '#fff',
    padding: 16,
    marginBottom: 16,
    borderRadius: 8,
    elevation: 2,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 2,
  },
  sectionTitle: {
    fontSize: 18,
    fontWeight: '600',
    marginBottom: 12,
    color: '#333',
  },
  inputGroup: {
    marginBottom: 16,
  },
  label: {
    fontSize: 14,
    fontWeight: '500',
    marginBottom: 4,
    color: '#333',
  },
  input: {
    borderWidth: 1,
    borderColor: '#ddd',
    borderRadius: 6,
    padding: 12,
    fontSize: 16,
    backgroundColor: '#fff',
  },
  roleContainer: {
    flexDirection: 'row',
    gap: 8,
  },
  roleButton: {
    flex: 1,
    padding: 12,
    borderWidth: 1,
    borderColor: '#ddd',
    borderRadius: 6,
    alignItems: 'center',
    backgroundColor: '#fff',
  },
  roleButtonActive: {
    backgroundColor: '#007AFF',
    borderColor: '#007AFF',
  },
  roleButtonText: {
    fontSize: 14,
    color: '#333',
  },
  roleButtonTextActive: {
    color: '#fff',
    fontWeight: '500',
  },
  shiftRow: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingVertical: 8,
  },
  shiftLabel: {
    fontSize: 16,
    color: '#333',
  },
  submitButton: {
    backgroundColor: '#007AFF',
    padding: 16,
    borderRadius: 8,
    alignItems: 'center',
    marginTop: 24,
  },
  submitButtonDisabled: {
    backgroundColor: '#ccc',
  },
  submitButtonText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: '600',
  },
  resetButton: {
    backgroundColor: '#fff',
    padding: 16,
    borderRadius: 8,
    alignItems: 'center',
    marginTop: 8,
    borderWidth: 1,
    borderColor: '#ddd',
  },
  resetButtonText: {
    color: '#333',
    fontSize: 16,
  },
  spacer: {
    height: 32,
  },
});