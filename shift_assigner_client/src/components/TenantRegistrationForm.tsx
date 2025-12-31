import React, { useState } from 'react';
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
  const { t, tPlural, tICU, isRTL, direction, textAlign } = useLanguage(['tenantRegistration', 'common']);
  const dispatch = useAppDispatch();
  const tenant = useSelector<AppState, BossTenant>(state => state.tenantRegistration.tenant);
  const isSubmitting = useSelector<AppState,boolean>(state=> state.tenantRegistration.isSubmitting);
  const error = useSelector<AppState, string | undefined>(state => state.tenantRegistration.error);
  
  // State for dynamic shifts
  const [shifts, setShifts] = useState<Array<{ id: string; name: string; enabled: boolean }>>([
    { id: '1', name: 'Morning Shift', enabled: false },
    { id: '2', name: 'Day Shift', enabled: false },
    { id: '3', name: 'Evening Shift', enabled: false }
  ]);
  const [newShiftName, setNewShiftName] = useState('');
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
        <Text style={[styles.sectionTitle, { textAlign }]}>
          {t('tenantRegistration:personalInfo')}
        </Text>
        
        <View style={styles.inputGroup}>
          <Text style={[styles.label, { textAlign }]}>
            {t('tenantRegistration:firstName')} *
          </Text>
          <TextInput
            style={[styles.input, { textAlign }]}
            value={tenant.firstName}
            onChangeText={(value) => updateField('firstName', value)}
            placeholder={t('tenantRegistration:placeholders.firstName')}
          />
        </View>

        <View style={styles.inputGroup}>
          <Text style={[styles.label, { textAlign }]}>
            {t('tenantRegistration:lastName')} *
          </Text>
          <TextInput
            style={[styles.input, { textAlign }]}
            value={tenant.lastName}
            onChangeText={(value) => updateField('lastName', value)}
            placeholder={t('tenantRegistration:placeholders.lastName')}
          />
        </View>

        <View style={styles.inputGroup}>
          <Text style={[styles.label, { textAlign }]}>
            {t('tenantRegistration:phoneNumber')} *
          </Text>
          <TextInput
            style={[styles.input, { textAlign }]}
            value={tenant.phoneNumber}
            onChangeText={(value) => updateField('phoneNumber', value)}
            placeholder={t('tenantRegistration:placeholders.phoneNumber')}
            keyboardType="phone-pad"
          />
        </View>

        <View style={styles.inputGroup}>
          <Text style={[styles.label, { textAlign }]}>
            {t('tenantRegistration:dateOfBirth')} *
          </Text>
          <TextInput
            style={[styles.input, { textAlign }]}
            value={tenant.dateOfBirth}
            onChangeText={(value) => updateField('dateOfBirth', value)}
            placeholder={t('tenantRegistration:placeholders.dateOfBirth')}
          />
        </View>
      </View>

      {/* Tenant Information */}
      <View style={styles.section}>
        <Text style={[styles.sectionTitle, { textAlign }]}>
          {t('tenantRegistration:tenantInfo')}
        </Text>
        
        <View style={styles.inputGroup}>
          <Text style={[styles.label, { textAlign }]}>
            {t('tenantRegistration:tenantName')} *
          </Text>
          <TextInput
            style={[styles.input, { textAlign }]}
            value={tenant.tenant}
            onChangeText={(value) => updateField('tenant', value)}
            placeholder={t('tenantRegistration:placeholders.tenantName')}
          />
        </View>

        <View style={styles.inputGroup}>
          <Text style={[styles.label, { textAlign }]}>
            {t('tenantRegistration:role')}
          </Text>
          <View style={styles.roleContainer}>
          <TouchableOpacity 
            style={[styles.roleButton, tenant.role === RoleState.Boss && styles.roleButtonActive]}
            onPress={() => updateField('role', RoleState.Boss)}
          >
            <Text style={[styles.roleButtonText, tenant.role === RoleState.Boss && styles.roleButtonTextActive]}>
              {t('tenantRegistration:roles.boss')}
            </Text>
          </TouchableOpacity>
          <TouchableOpacity 
            style={[styles.roleButton, tenant.role === RoleState.Admin && styles.roleButtonActive]}
            onPress={() => updateField('role', RoleState.Admin)}
          >
            <Text style={[styles.roleButtonText, tenant.role === RoleState.Admin && styles.roleButtonTextActive]}>
              {t('tenantRegistration:roles.admin')}
            </Text>
          </TouchableOpacity>
          </View>
        </View>
      </View>

      {/* Shift Configuration */}
      <View style={styles.section}>
        <Text style={[styles.sectionTitle, { textAlign }]}>
          {t('tenantRegistration:shiftConfig')}
        </Text>
        
        {/* Add new shift input */}
        <View style={styles.addShiftContainer}>
          <TextInput
            style={[styles.shiftInput, { textAlign }]}
            value={newShiftName}
            onChangeText={setNewShiftName}
            placeholder={t('tenantRegistration:shiftNamePlaceholder')}
          />
          <TouchableOpacity 
            style={[styles.addButton, !newShiftName.trim() && styles.addButtonDisabled]}
            onPress={addShift}
            disabled={!newShiftName.trim()}
          >
            <Text style={styles.addButtonText}>
              {t('tenantRegistration:addShift')}
            </Text>
          </TouchableOpacity>
        </View>

        {/* Dynamic shift list */}
        {shifts.length === 0 ? (
          <Text style={[styles.noShiftsText, { textAlign }]}>
            {t('tenantRegistration:noShifts')}
          </Text>
        ) : (
          shifts.map((shift) => (
            <View key={shift.id} style={styles.shiftRow}>
              <View style={styles.shiftInfo}>
                <Text style={[styles.shiftLabel, { textAlign }]}>
                  {shift.name}
                </Text>
                <TouchableOpacity 
                  style={styles.removeButton}
                  onPress={() => removeShift(shift.id)}
                >
                  <Text style={styles.removeButtonText}>
                    {t('tenantRegistration:removeShift')}
                  </Text>
                </TouchableOpacity>
              </View>
            </View>
          ))
        )}
      </View>

      {/* Submit Button */}
      <TouchableOpacity
        style={[styles.submitButton, (!isFormValid || isSubmitting) && styles.submitButtonDisabled]}
        onPress={handleSubmit}
        disabled={!isFormValid || isSubmitting}
      >
        <Text style={styles.submitButtonText}>
          {isSubmitting ? t('tenantRegistration:submitting') : t('tenantRegistration:register')}
        </Text>
      </TouchableOpacity>

      {/* Reset Button */}
      <TouchableOpacity
        style={styles.resetButton}
        onPress={() => dispatch(TenantRegistrationActions.resetForm())}
      >
        <Text style={styles.resetButtonText}>{t('tenantRegistration:resetForm')}</Text>
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
    paddingVertical: 10,
    borderBottomWidth: 1,
    borderBottomColor: '#f0f0f0',
  },
  shiftLabel: {
    fontSize: 16,
    color: '#333',
    flex: 1,
  },
  addShiftContainer: {
    flexDirection: 'row',
    marginBottom: 16,
    gap: 8,
  },
  shiftInput: {
    flex: 1,
    borderWidth: 1,
    borderColor: '#ddd',
    borderRadius: 6,
    padding: 12,
    fontSize: 16,
    backgroundColor: '#fff',
  },
  addButton: {
    backgroundColor: '#28a745',
    padding: 12,
    borderRadius: 6,
    justifyContent: 'center',
    paddingHorizontal: 16,
  },
  addButtonDisabled: {
    backgroundColor: '#6c757d',
    opacity: 0.6,
  },
  addButtonText: {
    color: '#fff',
    fontSize: 14,
    fontWeight: '500',
  },
  shiftInfo: {
    flex: 1,
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
  },
  removeButton: {
    backgroundColor: '#dc3545',
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: 6,
    marginLeft: 8,
    elevation: 1,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.2,
    shadowRadius: 1,
  },
  removeButtonText: {
    color: '#fff',
    fontSize: 12,
    fontWeight: '600',
    textTransform: 'uppercase',
  },
  noShiftsText: {
    fontSize: 14,
    color: '#666',
    fontStyle: 'italic',
    textAlign: 'center',
    paddingVertical: 16,
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