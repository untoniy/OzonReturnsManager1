# Инструкция по настройке токена авторизации

## Как хранить токен безопасно

1. **Создайте файл `token.txt`** в папке с проектом (рядом с файлом `.sln`) или в папке с собранным приложением (`bin/Debug` или `bin/Release`).

2. **Поместите токен в файл** - просто строку токена без дополнительных символов:
   ```
   your-secure-token
   ```

3. **Файл уже добавлен в `.gitignore`** - он не попадёт в репозиторий Git.

## Где искать токен при запуске

Приложение ищет токен в следующем порядке:
1. В папке собранного приложения (`AppDomain.CurrentDomain.BaseDirectory`)
2. В папке проекта (для отладки)

## Структура проекта

```
OzonReturnsManager1/
├── token.txt              <- Создайте этот файл с вашим токеном
├── .gitignore             <- token.txt уже в игноре
├── OzonReturnsManager1.sln
├── OzonReturnsManager1.csproj
├── Form1.cs
├── Form1.Designer.cs
├── Program.cs
├── Models/
│   ├── ReturnStatus.cs
│   └── ReturnRecord.cs
└── Services/
    ├── TokenService.cs
    └── ReturnsApiClient.cs
```

## Важно!

- Никогда не коммитьте файл `token.txt` в Git
- Не передавайте токен третьим лицам
- При сборке Release скопируйте `token.txt` в папку `bin/Release`
