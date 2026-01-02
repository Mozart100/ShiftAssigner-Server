import { StatusBar } from "expo-status-bar";
import { StyleSheet, View } from "react-native";
import React from "react";
import { NavigationContainer } from "@react-navigation/native";
import { Provider } from "react-redux";
import { store } from "./src/store";
import { RegistrationSelectionForm } from "./src/components/RegistrationSelectionForm";
import { LanguageSync } from "./src/localization";
import './src/localization/i18n'; // Initialize i18n

export default function App(): React.JSX.Element {
  return (
    <Provider store={store}>
      <LanguageSync>
        <NavigationContainer>
          <View style={styles.container}>
            <RegistrationSelectionForm />
          </View>
        </NavigationContainer>
      </LanguageSync>
      <StatusBar style="auto" />
    </Provider>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#f5f5f5",
  },
});
