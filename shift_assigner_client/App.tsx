import { StatusBar } from "expo-status-bar";
import { StyleSheet, View } from "react-native";
import React from "react";
import { Provider } from "react-redux";
import { store } from "./src/store";
import { AppNavigator } from "./src/navigation/AppNavigator";
import { LanguageSync } from "./src/localization";
import { LanguageSwitcher } from "./src/components/LanguageSwitcher";
import "./src/localization/i18n"; // Initialize i18n

export default function App(): React.JSX.Element {
  return (
    <Provider store={store}>
      <LanguageSync>
        <View style={styles.container}>
          {/* Global Language Switcher */}
          <View style={styles.languageSwitcher}>
            <LanguageSwitcher />
          </View>
          
          {/* Main App Content */}
          <View style={styles.content}>
            <AppNavigator />
          </View>
        </View>
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
  languageSwitcher: {
    alignSelf: 'flex-start',
    marginTop:40,
    marginLeft:20
  },
  content: {
    flex: 1,
  },
});
