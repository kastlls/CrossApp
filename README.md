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

## Запуск

Для збірки проєкту:

```bash
dotnet build
dotnet run --project src/Cli

## Self-contained публікація

Проєкт було опубліковано у self-contained режимі для двох різних RID.

| RID | Розмір publish |
|---|---:|
| win-x64 | 77.02 MB |
| linux-x64 | 78.83 MB |

## Порівняння

Windows-версія має розмір 77.02 MB, а Linux-версія — 78.83 MB.

Linux-версія більша на 1.81 MB.
s
Команди публікації:

```bash
dotnet publish src/Cli -c Release -r win-x64 --self-contained true
dotnet publish src/Cli -c Release -r linux-x64 --self-contained true