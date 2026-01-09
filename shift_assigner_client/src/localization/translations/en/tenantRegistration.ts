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

  // Dynamic shifts
  addShift: "Add Shift",
  editShift: "Edit Shift", 
  updateShift: "Update Shift",
  cancelEdit: "Cancel Edit",
  removeShift: "Remove",
  shiftName: "Shift Name",
  shiftNamePlaceholder: "Enter shift name",
  minWorkers: "Min Workers",
  maxWorkers: "Max Workers", 
  action: "Action",
  noShifts: "No shifts configured. Click 'Add Shift' to create your first shift.",
  clearSelection: "Clear Selection",
  rowSelected: "Row {number} selected",

  // Actions  
  register: "Register Tenant",
  completeRegistration: "COMPLETE REGISTRATION",
  continueToShiftConfig: "Continue to Shift Configuration",
  back: "BACK",
  submitting: "Submitting...",
  resetForm: "RESET FORM",
  creatingTenant: "Creating Tenant...",
  
  // Step labels
  stepBasicInfo: "Basic Info",
  stepShiftConfig: "Shift Config",

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