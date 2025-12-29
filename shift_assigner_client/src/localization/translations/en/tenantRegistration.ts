export default {
  title: "Tenant Registration",
  personalInfo: "Personal Information", 
  tenantInfo: "Tenant Information",
  shiftConfig: "Shift Configuration",
  
  // Fields
  firstName: "First Name",
  lastName: "Last Name",
  phoneNumber: "Phone Number",
  dateOfBirth: "Date of Birth",
  tenantName: "Tenant Name",
  role: "Role",
  
  // Placeholders
  placeholders: {
    firstName: "Enter first name",
    lastName: "Enter last name", 
    phoneNumber: "Enter phone number",
    dateOfBirth: "YYYY-MM-DD",
    tenantName: "Enter tenant/company name"
  },

  // Roles with gender support
  roles: {
    boss: "Boss",
    admin: "Admin"
  },

  // Shifts
  shifts: {
    morning: "Morning Shift",
    day: "Day Shift",
    evening: "Evening Shift"
  },

  // Actions  
  register: "Register Tenant",
  submitting: "Submitting...",
  resetForm: "Reset Form",

  // Messages with interpolation
  messages: {
    success: "Tenant registration completed successfully!",
    fillRequired: "Please fill all required fields correctly", 
    registrationFailed: "Registration failed",
    processingTime: "Processing will take approximately {duration, number} seconds",
    registeredOn: "Registered on {date, date, long} at {date, time, medium}"
  },
  
  // Validation messages with plurals
  validation: {
    minLength: `{field} must be at least {count, plural,
      one {# character}
      other {# characters}
    } long`,
    
    maxLength: `{field} cannot exceed {count, plural, 
      one {# character}
      other {# characters}
    }`,
    
    phoneFormat: "Please enter a valid phone number"
  }
};