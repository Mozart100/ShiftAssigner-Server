export default {
  title: "Регистрация арендатора",
  personalInfo: "Личная информация",
  tenantInfo: "Информация о арендаторе", 
  shiftConfig: "Настройка смен",
  
  // Fields
  firstName: "Имя",
  lastName: "Фамилия",
  phoneNumber: "Номер телефона",
  dateOfBirth: "Дата рождения",
  tenantName: "Название арендатора", 
  role: "Роль",
  
  // Placeholders
  placeholders: {
    firstName: "Введите имя",
    lastName: "Введите фамилию",
    phoneNumber: "Введите номер телефона",
    dateOfBirth: "ГГГГ-ММ-ДД",
    tenantName: "Введите название арендатора/компании"
  },

  // Roles with gender support
  roles: {
    boss: "Руководитель", 
    admin: "Администратор"
  },

  // Shifts
  shifts: {
    morning: "Утренняя смена",
    day: "Дневная смена",
    evening: "Вечерняя смена"
  },

  // Dynamic shifts
  addShift: "Добавить смену",
  removeShift: "Удалить",
  shiftName: "Название смены",
  shiftNamePlaceholder: "Введите название смены",
  noShifts: "Смены не настроены. Нажмите 'Добавить смену' для создания первой смены.",

  // Actions
  register: "Зарегистрировать арендатора",
  submitting: "Отправка...",
  resetForm: "Сбросить форму",

  // Messages with interpolation  
  messages: {
    success: "Регистрация арендатора успешно завершена!",
    fillRequired: "Пожалуйста, заполните все обязательные поля правильно",
    registrationFailed: "Ошибка регистрации",
    processingTime: "Обработка займёт примерно {duration, number} секунд",
    registeredOn: "Зарегистрирован {date, date, long} в {date, time, medium}"
  },
  
  // Validation messages with plurals
  validation: {
    minLength: `{field} должно содержать минимум {count, plural,
      one {# символ}
      few {# символа}
      many {# символов}
      other {# символов}
    }`,
    
    maxLength: `{field} не может превышать {count, plural,
      one {# символ}
      few {# символа}  
      many {# символов}
      other {# символов}
    }`,
    
    phoneFormat: "Пожалуйста, введите действительный номер телефона"
  }
};