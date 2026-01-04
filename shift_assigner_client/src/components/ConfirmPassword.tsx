import React, { useState } from "react";

import { Button, Heading5, HStack, Input, Section, VStack, Typography } from "../design-system";
import { t } from "i18next";

interface PasswordConfirmationProps {
    onPasswordConfirm: (password: string) => void;
    title?: string;
    minLength?: number;
}

export const PasswordConfirmation: React.FC<PasswordConfirmationProps> = ({ 
    onPasswordConfirm, 
    title = "Security Information",
    minLength = 6 
}) => {

    const [password, setPassword] = useState<string>("");
    const [confirmPassword, setConfirmPassword] = useState<string>("");
    const [error, setError] = useState<string>("");

    function resetPassword() {
        setPassword("");
        setConfirmPassword("");
        setError("");
    }

    function submitHandler() {
        setError(""); // Clear previous errors
        
        if (!password || !confirmPassword) {
            setError("Please fill in both password fields");
            return;
        }

        if (password !== confirmPassword) {
            setError("Passwords don't match");
            return;
        }

        if (password.length < minLength) {
            setError(`Password must be at least ${minLength} characters long`);
            return;
        }

        // All validations passed
        onPasswordConfirm(password);
    }


    return (
        <Section>
            <Heading5 style={{ marginBottom: 16 }}>{title}</Heading5>

            <VStack gap={4}>
                <Input
                    label="Password *"
                    value={password}
                    onChangeText={setPassword}
                    placeholder="Enter password"
                    secureTextEntry
                    helperText={`Minimum ${minLength} characters`}
                    size="lg"
                />
                <Input
                    label="Confirm Password *"
                    value={confirmPassword}
                    onChangeText={setConfirmPassword}
                    placeholder="Confirm your password"
                    secureTextEntry
                    size="lg"
                />
                
                {error && (
                    <VStack>
                        <Typography 
                            variant="body2" 
                            color="danger" 
                            style={{ marginTop: 4, fontSize: 14 }}
                        >
                            {error}
                        </Typography>
                    </VStack>
                )}
            </VStack>

            <VStack gap={2} marginTop={3}>
            {/* <VStack gap={2} style={{marginTop: 3}}> */}
                <Button
                    variant="primary"
                    size="md"
                    fullWidth
                    onPress={submitHandler}
                >
                    {t("common:confirm", "Confirm")}
                </Button>

                <Button
                    variant="outline-secondary"
                    size="md"
                    fullWidth
                    onPress={resetPassword}
                >
                    {t("common:reset", "Reset")}
                </Button>

            </VStack>
        </Section>
    );
};


