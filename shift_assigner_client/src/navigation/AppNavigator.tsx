import React from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { createStackNavigator } from '@react-navigation/stack';
import { RegistrationSelectionScreen } from '../screens/RegistrationSelectionScreen';
import { TenantRegistrationScreen } from '../screens/TenantRegistrationScreen';
import { ShiftLeaderRegistrationScreen } from '../screens/ShiftLeaderRegistrationScreen';

// Define navigation types
export type RootStackParamList = {
  RegistrationSelection: undefined;
  TenantRegistration: undefined;
  ShiftLeaderRegistration: undefined;
};

const Stack = createStackNavigator<RootStackParamList>();

export const AppNavigator: React.FC = () => {
  return (
    <NavigationContainer>
      <Stack.Navigator
        initialRouteName="RegistrationSelection"
        screenOptions={{
          headerShown: false, // Hide the header for cleaner design
          cardStyle: { backgroundColor: '#fff' },
          gestureEnabled: true,
        }}
      >
        <Stack.Screen 
          name="RegistrationSelection" 
          component={RegistrationSelectionScreen}
          options={{ title: 'Select Registration Type' }}
        />
        <Stack.Screen 
          name="TenantRegistration" 
          component={TenantRegistrationScreen}
          options={{ 
            title: 'Tenant Registration',
            headerShown: true,
            headerBackTitleVisible: false,
            headerTitleStyle: { fontSize: 18, fontWeight: '600' }
          }}
        />
        <Stack.Screen 
          name="ShiftLeaderRegistration" 
          component={ShiftLeaderRegistrationScreen}
          options={{ 
            title: 'Shift Leader Registration',
            headerShown: true,
            headerBackTitleVisible: false,
            headerTitleStyle: { fontSize: 18, fontWeight: '600' }
          }}
        />
      </Stack.Navigator>
    </NavigationContainer>
  );
};