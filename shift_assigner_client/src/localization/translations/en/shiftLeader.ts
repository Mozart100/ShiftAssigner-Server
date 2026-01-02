export default {
  translation: {
    // Title
    title: 'Shift Leader Registration',
    
    // Sections
    personalInfo: 'Personal Information',
    securityInfo: 'Security Information',
    
    // Field Labels
    id: 'ID',
    firstName: 'First Name',
    lastName: 'Last Name',
    phoneNumber: 'Phone Number',
    dateOfBirth: 'Date of Birth',
    password: 'Password',
    confirmPassword: 'Confirm Password',
    
    // Placeholders
    placeholders: {
      id: 'Enter your ID',
      firstName: 'Enter first name',
      lastName: 'Enter last name',
      phoneNumber: 'Enter phone number',
      dateOfBirth: 'YYYY-MM-DD',
      password: 'Enter password',
      confirmPassword: 'Confirm your password',
    },
    
    // Buttons
    register: 'Register Shift Leader',
    resetForm: 'Reset Form',
    submitting: 'Registering...',
    
    // Validation Errors
    errors: {
      idRequired: 'ID is required',
      firstNameRequired: 'First name is required',
      lastNameRequired: 'Last name is required',
      phoneRequired: 'Phone number is required',
      dobRequired: 'Date of birth is required',
      passwordRequired: 'Password is required',
      passwordTooShort: 'Password must be at least 6 characters',
      passwordMismatch: 'Passwords do not match',
      phoneInvalid: 'Please enter a valid phone number',
      dobInvalid: 'Please enter date in YYYY-MM-DD format',
    },
    
    // Messages
    messages: {
      registrationSuccess: 'Shift Leader registered successfully!',
      registrationError: 'Registration failed. Please try again.',
    },
    
    // Helper Text
    passwordHelp: 'Minimum 6 characters',
    notice: 'All fields marked with * are required. Your information will be used to create your shift leader account.',
  }
};