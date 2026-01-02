export default {
  translation: {
    // Title
    title: 'Регистрация руководителя смены',
    
    // Sections
    personalInfo: 'Личная информация',
    securityInfo: 'Информация безопасности',
    
    // Field Labels
    id: 'ID',
    firstName: 'Имя',
    lastName: 'Фамилия',
    phoneNumber: 'Номер телефона',
    dateOfBirth: 'Дата рождения',
    password: 'Пароль',
    confirmPassword: 'Подтвердите пароль',
    
    // Placeholders
    placeholders: {
      id: 'Введите ваш ID',
      firstName: 'Введите имя',
      lastName: 'Введите фамилию',
      phoneNumber: 'Введите номер телефона',
      dateOfBirth: 'ГГГГ-ММ-ДД',
      password: 'Введите пароль',
      confirmPassword: 'Подтвердите пароль',
    },
    
    // Buttons
    register: 'Зарегистрировать руководителя смены',
    resetForm: 'Сбросить форму',
    submitting: 'Регистрация...',
    
    // Validation Errors
    errors: {
      idRequired: 'ID обязателен',
      firstNameRequired: 'Имя обязательно',
      lastNameRequired: 'Фамилия обязательна',
      phoneRequired: 'Номер телефона обязателен',
      dobRequired: 'Дата рождения обязательна',
      passwordRequired: 'Пароль обязателен',
      passwordTooShort: 'Пароль должен содержать не менее 6 символов',
      passwordMismatch: 'Пароли не совпадают',
      phoneInvalid: 'Введите действительный номер телефона',
      dobInvalid: 'Введите дату в формате ГГГГ-ММ-ДД',
    },
    
    // Messages
    messages: {
      registrationSuccess: 'Руководитель смены успешно зарегистрирован!',
      registrationError: 'Ошибка регистрации. Попробуйте еще раз.',
    },
    
    // Helper Text
    passwordHelp: 'Минимум 6 символов',
    notice: 'Все поля, отмеченные *, обязательны. Ваша информация будет использоваться для создания учетной записи руководителя смены.',
  }
};