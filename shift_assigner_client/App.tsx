import { StatusBar } from "expo-status-bar";
import { StyleSheet, Text, View } from "react-native";
import React from "react";
import { NavigationContainer } from "@react-navigation/native";

export default function App(): React.JSX.Element {
  return (
    <>
      <StatusBar style="auto" />
      <NavigationContainer>
        <View style={styles.container}>
          <Text>Welcome to ShiftAssigner TypeScript App! 2222</Text>
        </View>
      </NavigationContainer>
    </>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: "#fff",
    alignItems: "center",
    justifyContent: "center",
  },
});
