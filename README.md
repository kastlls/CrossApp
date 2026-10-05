# CrossApp

Наскрізний проєкт з крос-платформного програмування.

## Предметна область

**Бібліотека**

### Сутності

- Book — книга
- BookCopy — примірник книги
- Reader — читач
- Loan — видача книги

### Призначення

Застосунок призначений для обліку видач примірників книг читачам і їх повернень.

## Структура рішення

```text
CrossApp/
├── CrossApp.sln
├── README.md
├── .gitignore
└── src/
    ├── Core/
    │   ├── Core.csproj
    │   └── EnvironmentInfo.cs
    └── Cli/
        ├── Cli.csproj
        └── Program.cs
```

### Домовленість про каталоги в Core (план на семестр):
- Core/Dto/ — record-типи формату даних (тиждень 3).
- Core/Domain/ — сутності з поведінкою та інваріантами (тиждень 4).
- Core/Storage/ — реалізації сховищ (тиждень 5).

## Запуск

Для збірки проєкту:

```bash
dotnet build
dotnet run --project src/Cli
```

## Публікація та порівняння режимів

Проєкт було опубліковано у self-contained та framework-dependent режимах.

| RID | Режим | Розмір publish | Потрібен runtime |
|---|---|---:|:---:|
| win-x64 | self-contained | 77.04 MB | Ні |
| win-x64 | framework-dependent | 0.20 MB | Так (.NET) |
| linux-x64 | self-contained | 78.83 MB | Ні |

### Пояснення режимів (різниця)

- **Self-contained**: застосунок публікується разом із вбудованою копією .NET Runtime. Він займає значно більше місця (~77-78 MB), але може працювати на цільовому комп'ютері без попередньо встановленого .NET.
- **Framework-dependent**: застосунок містить виключно ваш код та залежності. Займає мінімум місця (~0.20 MB), але вимагає, щоб на комп'ютері користувача був встановлений сумісний .NET Runtime.

Команди публікації:

```bash
dotnet publish src/Cli -c Release -r win-x64 --self-contained true
dotnet publish src/Cli -c Release -r linux-x64 --self-contained true
dotnet publish src/Cli -c Release -r win-x64 --self-contained false
```
## Лабораторна робота 4: Доменна модель
**Перелік реалізованих інваріантів:**
1. **Рядковий ключ не порожній:** Id, ISBN та ідентифікатор читача є обов'язковими при створенні. Викидає `ArgumentException` у фабричних методах.
2. **Захист від подвійної видачі:** Неможливо видати примірник, який уже виданий (перевірка стану `IsIssued`). Викидає `InvalidOperationException` у методі `BookCopy.Issue()`.
3. **Захист від подвійного закриття:** Неможливо закрити видачу, яка вже має дату повернення. Викидає `InvalidOperationException` у методі `Loan.Close()`.
4. **Хронологія дат:** Дата повернення не може бути раніше дати видачі. Викидає `ArgumentOutOfRangeException` у методі `Loan.Close()`.
