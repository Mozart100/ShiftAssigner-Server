export default {
  // Common translations
  save: "Сохранить",
  cancel: "Отмена",
  submit: "Отправить", 
  reset: "Сброс",
  loading: "Загрузка...",
  error: "Ошибка",
  success: "Успех",
  required: "Обязательно",
  
  // Pluralization examples (Russian has complex plural rules)
  items_one: "{{count}} предмет", 
  items_few: "{{count}} предмета",
  items_many: "{{count}} предметов",
  items_other: "{{count}} предметов",
  
  // ICU message format examples
  welcome: "Добро пожаловать, {name}!",
  lastSeen: "Последний визит {date, date, short} в {date, time, short}",
  
  // Complex pluralization with ICU
  notifications: `{count, plural,
    =0 {Нет уведомлений}
    one {# уведомление}
    few {# уведомления} 
    many {# уведомлений}
    other {# уведомлений}
  }`,
  
  // Gender-aware messages
  greeting: `{gender, select,
    male {Добро пожаловать, г-н {name}}
    female {Добро пожаловать, г-жа {name}}
    other {Добро пожаловать, {name}}
  }`,
  
  // Time formatting
  timeAgo: `{minutes, plural,
    =0 {только что}
    one {# минуту назад}
    few {# минуты назад}
    many {# минут назад}
    other {# минут назад}
  }`
};