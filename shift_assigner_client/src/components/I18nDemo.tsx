import React from 'react';
import { View, Text, StyleSheet, TouchableOpacity } from 'react-native';
import { useLanguage } from '../localization';

export const I18nDemo: React.FC = () => {
  const { t, tPlural, tICU, formatters, isRTL, direction } = useLanguage(['common']);
  const [count, setCount] = React.useState(1);

  return (
    <View style={styles.container}>
      <Text style={styles.title}>🌍 Advanced i18n Features</Text>
      
      {/* Basic Translation */}
      <Text style={styles.demo}>
        Basic: {String(t('common:welcome', { name: 'John' }))}
      </Text>
      
      {/* Pluralization */}
      <View style={styles.row}>
        <TouchableOpacity onPress={() => setCount(Math.max(0, count - 1))}>
          <Text style={styles.button}>-</Text>
        </TouchableOpacity>
        <Text style={styles.demo}>
          Plural: {String(tPlural('common:items', count))}
        </Text>
        <TouchableOpacity onPress={() => setCount(count + 1)}>
          <Text style={styles.button}>+</Text>
        </TouchableOpacity>
      </View>
      
      {/* ICU Messages */}
      <Text style={styles.demo}>
        ICU: {tICU('common:notifications', { count: 5 })}
      </Text>
      
      <Text style={styles.demo}>
        Gender: {tICU('common:greeting', { gender: 'male', name: 'Alex' })}
      </Text>
      
      {/* Formatting */}
      <Text style={styles.demo}>
        Currency: {formatters.currency(1234.56)}
      </Text>
      
      <Text style={styles.demo}>
        Date: {formatters.date(new Date())}
      </Text>
      
      <Text style={styles.demo}>
        Time: {formatters.time(new Date())}
      </Text>
      
      <Text style={styles.demo}>
        Number: {formatters.number(987654321)}
      </Text>
      
      <Text style={styles.demo}>
        Percent: {formatters.percent(0.85)}
      </Text>
      
      {/* RTL Demo */}
      <Text style={styles.demo}>
        RTL Mode: {isRTL ? 'Active' : 'Inactive'}
      </Text>
      
      <Text style={styles.demo}>
        Text Direction: {direction}
      </Text>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    padding: 16,
    backgroundColor: '#f0f8ff',
    borderRadius: 8,
    margin: 16,
  },
  title: {
    fontSize: 18,
    fontWeight: 'bold',
    marginBottom: 16,
    textAlign: 'center',
  },
  demo: {
    fontSize: 14,
    marginVertical: 4,
    padding: 8,
    backgroundColor: '#fff',
    borderRadius: 4,
  },
  row: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'space-between',
  },
  button: {
    fontSize: 24,
    fontWeight: 'bold',
    color: '#007AFF',
    paddingHorizontal: 12,
  },
});