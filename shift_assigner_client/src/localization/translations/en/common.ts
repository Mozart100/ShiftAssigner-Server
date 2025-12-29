export default {
  // Common translations
  save: "Save",
  cancel: "Cancel", 
  submit: "Submit",
  reset: "Reset",
  loading: "Loading...",
  error: "Error",
  success: "Success",
  required: "Required",
  
  // Pluralization examples
  items_one: "{{count}} item",
  items_other: "{{count}} items",
  
  // ICU message format examples
  welcome: "Welcome, {name}!",
  lastSeen: "Last seen {date, date, short} at {date, time, short}",
  
  // Complex pluralization with ICU
  notifications: `{count, plural,
    =0 {No notifications}
    one {# notification} 
    other {# notifications}
  }`,
  
  // Gender-aware messages  
  greeting: `{gender, select,
    male {Welcome, Mr. {name}}
    female {Welcome, Ms. {name}}
    other {Welcome, {name}}
  }`,
  
  // Time formatting
  timeAgo: `{minutes, plural,
    =0 {just now}
    one {# minute ago}
    other {# minutes ago}
  }`
};