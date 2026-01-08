/**
 * Shift Configuration Table Component
 */

import React, { useState } from 'react';
import { View, TouchableOpacity, Dimensions } from 'react-native';
import { useLanguage } from '../localization';
import { TenantShiftScheduling } from '../store/tenantReducer';
import {
  Typography,
  Heading5,
  Section,
  VStack,
  HStack,
  Button,
  Input,
} from '../design-system';

interface ShiftConfigurationTableProps {
  shifts: TenantShiftScheduling[];
  onUpdateShifts: (shifts: TenantShiftScheduling[]) => void;
}

export const ShiftConfigurationTable: React.FC<ShiftConfigurationTableProps> = ({
  shifts,
  onUpdateShifts,
}) => {
  const { t } = useLanguage(['tenantRegistration', 'common']);
  const [selectedRowIndex, setSelectedRowIndex] = useState<number | null>(null);
  const [newShift, setNewShift] = useState({
    shiftName: '',
    minimumAmountOfWorkers: '',
    maximumAmountOfWorkers: '',
  });

  const addShift = () => {
    if (!newShift.shiftName.trim() || !newShift.minimumAmountOfWorkers || !newShift.maximumAmountOfWorkers) {
      return;
    }

    const minWorkers = parseInt(newShift.minimumAmountOfWorkers);
    const maxWorkers = parseInt(newShift.maximumAmountOfWorkers);

    if (minWorkers >= maxWorkers) {
      return; // Invalid: min should be less than max
    }

    const shift: TenantShiftScheduling = {
      shiftName: newShift.shiftName.trim(),
      minimumAmountOfWorkers: minWorkers,
      maximumAmountOfWorkers: maxWorkers,
    };

    if (selectedRowIndex !== null) {
      // Update existing shift
      const updatedShifts = [...shifts];
      updatedShifts[selectedRowIndex] = shift;
      onUpdateShifts(updatedShifts);
    } else {
      // Add new shift
      onUpdateShifts([...shifts, shift]);
    }
    
    // Reset form and selection
    setNewShift({
      shiftName: '',
      minimumAmountOfWorkers: '',
      maximumAmountOfWorkers: '',
    });
    setSelectedRowIndex(null);
  };

  const removeShift = (index: number) => {
    const updatedShifts = shifts.filter((_, i) => i !== index);
    onUpdateShifts(updatedShifts);
    // Clear selection if the selected row was removed or adjust index
    if (selectedRowIndex === index) {
      setSelectedRowIndex(null);
    } else if (selectedRowIndex !== null && selectedRowIndex > index) {
      setSelectedRowIndex(selectedRowIndex - 1);
    }
  };

  const toggleRowSelection = (index: number) => {
    if (selectedRowIndex === index) {
      // Deselecting - clear form
      setSelectedRowIndex(null);
      setNewShift({
        shiftName: '',
        minimumAmountOfWorkers: '',
        maximumAmountOfWorkers: '',
      });
    } else {
      // Selecting new row - populate form with selected row data
      setSelectedRowIndex(index);
      const selectedShift = shifts[index];
      setNewShift({
        shiftName: selectedShift.shiftName,
        minimumAmountOfWorkers: selectedShift.minimumAmountOfWorkers.toString(),
        maximumAmountOfWorkers: selectedShift.maximumAmountOfWorkers.toString(),
      });
    }
  };

  const clearSelection = () => {
    setSelectedRowIndex(null);
    setNewShift({
      shiftName: '',
      minimumAmountOfWorkers: '',
      maximumAmountOfWorkers: '',
    });
  };

  const screenData = Dimensions.get('window');
  const isLandscape = screenData.width > screenData.height;
  const maxFormWidth = isLandscape ? 600 : undefined;

  return (
    <Section>
      <HStack justify="space-between" align="center" style={{ marginBottom: isLandscape ? 12 : 16 }}>
        <HStack gap={2} align="center">
          <Heading5>
            {String(t('tenantRegistration:shiftConfig'))}
          </Heading5>
          <TouchableOpacity
            onPress={clearSelection}
            style={{
              backgroundColor: '#3b82f6',
              paddingHorizontal: 12,
              paddingVertical: 6,
              borderRadius: 6,
              alignItems: 'center',
              justifyContent: 'center'
            }}
          >
            <Typography style={{ 
              color: 'white', 
              fontSize: 16,
              fontWeight: '600'
            }}>
              📅
            </Typography>
          </TouchableOpacity>
        </HStack>
        
        {shifts.length > 0 && selectedRowIndex !== null && (
          <HStack gap={1}>
            <Typography variant="body2" style={{ color: '#6b7280', marginRight: 8 }}>
              Row {selectedRowIndex + 1} selected
            </Typography>
            <Button
              variant="outline-secondary"
              size="sm"
              onPress={clearSelection}
              style={{ paddingHorizontal: 12 }}
            >
              Clear Selection
            </Button>
          </HStack>
        )}
      </HStack>

      {/* Add New Shift Form */}
      <View style={{ 
        maxWidth: maxFormWidth,
        alignSelf: 'center',
        width: '100%'
      }}>
        <View style={{
          backgroundColor: '#f8f9fa',
          padding: isLandscape ? 12 : 16,
          borderRadius: 8,
          marginBottom: isLandscape ? 12 : 16,
          borderWidth: 1,
          borderColor: '#e5e7eb'
        }}>
          <Typography variant="body1" style={{ 
            fontWeight: '600', 
            marginBottom: isLandscape ? 8 : 12,
            color: '#374151'
          }}>
            {selectedRowIndex !== null ? 'Edit Shift' : 'Add New Shift'}
          </Typography>
          
          {isLandscape ? (
            // Landscape layout: compact horizontal form
            <HStack gap={1.5} align="flex-end">
              <Input
                label="Shift Name"
                value={newShift.shiftName}
                onChangeText={(value) => setNewShift(prev => ({ ...prev, shiftName: value }))}
                placeholder="Morning, Day, Night"
                containerStyle={{ flex: 2.5 }}
                size="sm"
              />
              <Input
                label="Min"
                value={newShift.minimumAmountOfWorkers}
                onChangeText={(value) => setNewShift(prev => ({ ...prev, minimumAmountOfWorkers: value }))}
                placeholder="2"
                keyboardType="numeric"
                containerStyle={{ flex: 1 }}
                size="sm"
              />
              <Input
                label="Max"
                value={newShift.maximumAmountOfWorkers}
                onChangeText={(value) => setNewShift(prev => ({ ...prev, maximumAmountOfWorkers: value }))}
                placeholder="5"
                keyboardType="numeric"
                containerStyle={{ flex: 1 }}
                size="sm"
              />
              <View style={{ paddingBottom: 2 }}>
                <Button
                  variant="success"
                  size="sm"
                  onPress={addShift}
                  disabled={!newShift.shiftName.trim() || !newShift.minimumAmountOfWorkers || !newShift.maximumAmountOfWorkers}
                  style={{ minWidth: 80, paddingHorizontal: 12 }}
                >
                  {selectedRowIndex !== null ? 'Update' : '+ Add'}
                </Button>
              </View>
              {selectedRowIndex !== null && (
                <View style={{ paddingBottom: 2 }}>
                  <Button
                    variant="outline-secondary"
                    size="sm"
                    onPress={clearSelection}
                    style={{ minWidth: 80, paddingHorizontal: 12 }}
                  >
                    Cancel
                  </Button>
                </View>
              )}
            </HStack>
          ) : (
            // Portrait layout: original vertical layout with better styling
            <VStack gap={2}>
              <Input
                label="Shift Name *"
                value={newShift.shiftName}
                onChangeText={(value) => setNewShift(prev => ({ ...prev, shiftName: value }))}
                placeholder="e.g., Morning, Evening, Night"
              />

              <HStack gap={2}>
                <Input
                  label="Min Workers *"
                  value={newShift.minimumAmountOfWorkers}
                  onChangeText={(value) => setNewShift(prev => ({ ...prev, minimumAmountOfWorkers: value }))}
                  placeholder="1"
                  keyboardType="numeric"
                  containerStyle={{ flex: 1 }}
                />
                <Input
                  label="Max Workers *"
                  value={newShift.maximumAmountOfWorkers}
                  onChangeText={(value) => setNewShift(prev => ({ ...prev, maximumAmountOfWorkers: value }))}
                  placeholder="10"
                  keyboardType="numeric"
                  containerStyle={{ flex: 1 }}
                />
              </HStack>

              <Button
                variant="success"
                size="md"
                onPress={addShift}
                disabled={!newShift.shiftName.trim() || !newShift.minimumAmountOfWorkers || !newShift.maximumAmountOfWorkers}
              >
                {selectedRowIndex !== null ? 'Update Shift' : 'Add Shift'}
              </Button>
              
              {selectedRowIndex !== null && (
                <Button
                  variant="outline-secondary"
                  size="md"
                  onPress={clearSelection}
                >
                  Cancel Edit
                </Button>
              )}
            </VStack>
          )}
        </View>
      </View>

      {/* Shifts Table */}
      <View style={{ 
        maxWidth: maxFormWidth,
        alignSelf: 'center',
        width: '100%'
      }}>
        <View
          style={{
            borderWidth: 1,
            borderColor: '#e5e7eb',
            borderRadius: 8,
            overflow: 'hidden',
            backgroundColor: '#ffffff',
            ...(isLandscape && { 
              shadowColor: '#000',
              shadowOffset: { width: 0, height: 2 },
              shadowOpacity: 0.1,
              shadowRadius: 4,
              elevation: 3
            })
          }}
        >
          {/* Table Header */}
          <HStack
            style={{
              backgroundColor: isLandscape ? '#f1f5f9' : '#f8f9fa',
              borderBottomWidth: 1,
              borderBottomColor: '#e5e7eb',
              paddingVertical: isLandscape ? 10 : 12,
              paddingHorizontal: isLandscape ? 12 : 16,
            }}
          >
            <Typography 
              variant="body2" 
              style={{ 
                flex: isLandscape ? 3 : 2, 
                fontWeight: '700',
                color: '#374151',
                fontSize: isLandscape ? 12 : 14
              }}
            >
              Shift Name
            </Typography>
            <Typography 
              variant="body2" 
              style={{ 
                flex: 1, 
                fontWeight: '700', 
                textAlign: 'center',
                color: '#374151',
                fontSize: isLandscape ? 12 : 14
              }}
            >
              {isLandscape ? 'Min' : 'Min Workers'}
            </Typography>
            <Typography 
              variant="body2" 
              style={{ 
                flex: 1, 
                fontWeight: '700', 
                textAlign: 'center',
                color: '#374151',
                fontSize: isLandscape ? 12 : 14
              }}
            >
              {isLandscape ? 'Max' : 'Max Workers'}
            </Typography>
            <Typography 
              variant="body2" 
              style={{ 
                flex: 1, 
                fontWeight: '700', 
                textAlign: 'center',
                color: '#374151',
                fontSize: isLandscape ? 12 : 14
              }}
            >
              Action
            </Typography>
          </HStack>

          {/* Table Body */}
          {shifts.length === 0 ? (
            <View
              style={{
                padding: isLandscape ? 20 : 24,
                alignItems: 'center',
                backgroundColor: '#f9fafb',
              }}
            >
              <Typography 
                variant="body2" 
                color="text-secondary" 
                align="center"
                italic
                style={{
                  fontSize: isLandscape ? 13 : 14,
                  color: '#6b7280'
                }}
              >
                No shifts configured yet. Add your first shift above.
              </Typography>
            </View>
          ) : (
            shifts.map((shift, index) => {
              const isSelected = selectedRowIndex === index;
              
              return (
                <TouchableOpacity
                  key={`${shift.shiftName}-${index}`}
                  onPress={() => toggleRowSelection(index)}
                  style={{
                    borderBottomWidth: index < shifts.length - 1 ? 1 : 0,
                    borderBottomColor: '#f1f5f9',
                  }}
                >
                  <HStack
                    style={{
                      paddingVertical: isLandscape ? 10 : 12,
                      paddingHorizontal: isLandscape ? 12 : 16,
                      backgroundColor: isSelected 
                        ? '#f0f9ff' 
                        : index % 2 === 0 ? '#ffffff' : '#f9fafb',
                      borderLeftWidth: isSelected ? 3 : 0,
                      borderLeftColor: isSelected ? '#0ea5e9' : 'transparent',
                    }}
                  >
                    <Typography 
                      variant="body1" 
                      style={{ 
                        flex: isLandscape ? 3 : 2,
                        fontWeight: '500',
                        color: isSelected ? '#0369a1' : '#111827',
                        fontSize: isLandscape ? 13 : 16
                      }}
                    >
                      {shift.shiftName}
                    </Typography>
                    <Typography 
                      variant="body1" 
                      style={{ 
                        flex: 1, 
                        textAlign: 'center',
                        color: isSelected ? '#0369a1' : '#6b7280',
                        fontWeight: '500',
                        fontSize: isLandscape ? 13 : 16
                      }}
                    >
                      {shift.minimumAmountOfWorkers}
                    </Typography>
                    <Typography 
                      variant="body1" 
                      style={{ 
                        flex: 1, 
                        textAlign: 'center',
                        color: isSelected ? '#0369a1' : '#6b7280',
                        fontWeight: '500',
                        fontSize: isLandscape ? 13 : 16
                      }}
                    >
                      {shift.maximumAmountOfWorkers}
                    </Typography>
                    <View style={{ flex: 1, alignItems: 'center' }}>
                      <TouchableOpacity
                        onPress={(e) => {
                          e.stopPropagation();
                          removeShift(index);
                        }}
                        style={{
                          backgroundColor: '#ef4444',
                          paddingHorizontal: isLandscape ? 8 : 12,
                          paddingVertical: isLandscape ? 4 : 6,
                          borderRadius: 6,
                          minWidth: isLandscape ? 60 : 80,
                          alignItems: 'center'
                        }}
                      >
                        <Typography style={{ 
                          color: 'white', 
                          fontWeight: '600',
                          fontSize: isLandscape ? 14 : 16
                        }}>
                          🗑️
                        </Typography>
                      </TouchableOpacity>
                    </View>
                  </HStack>
                </TouchableOpacity>
              );
            })
          )}
        </View>
      </View>
    </Section>
  );
};