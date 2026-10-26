using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Cli;
using Cli.Sinks;
using Core.Abstractions;
using Core.Events;
using Core.Services;

var store = StoreFactory.Create(args);

// --- ДОДАТКОВЕ 1: Налаштовуємо DI Sinks і передаємо їх у сервіс ---
var diLogPath = Path.Combine(AppContext.BaseDirectory, "logs", "di_app.log");
var sinks = new List<INotificationSink> 
{ 
    new ConsoleSink(), 
    new FileSink(diLogPath) 
};
var service = new LendingService(store, sinks);

bool useFile = args.Contains("--file");
Console.WriteLine($"Сховище: {(useFile ? "FileBookStore" : "InMemoryBookStore")}\n");

// ==========================================================
// ОСНОВНЕ ЗАВДАННЯ (Твій попередній робочий код)
// ==========================================================
var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "app.log");
Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

void OnLibraryChanged(object? sender, LibraryChangedEventArgs e)
{
    var line = $"{e.At:HH:mm:ss} {e.Kind,-8} {e.Id}";
    Console.WriteLine(line);
    File.AppendAllText(logPath, line + Environment.NewLine);
}

service.Changed += OnLibraryChanged;

int changes = 0;
service.Changed += (_, _) => changes++;

Console.WriteLine("=== СЦЕНАРІЙ: ЗМІНИ І ПОДІЇ ===");
var created = service.AddBook("978-9660000001");
service.IssueCopy(created.Id);
Console.WriteLine($"Подій отримано: {changes}");

Console.WriteLine("\n=== СЦЕНАРІЙ: ВІДПИСКА ОСНОВНОГО ОБРОБНИКА ===");
service.Changed -= OnLibraryChanged;
service.ReturnCopy(created.Id); 
Console.WriteLine($"Після тихого повернення. Подій отримано: {changes} (+1 лише від лямбди-лічильника)");

Console.WriteLine("\n=== СЦЕНАРІЇ ВІДМОВИ (ПОДІЙ НЕ БУДЕ) ===");
try
{
    Console.WriteLine("-> Спроба видати неіснуючий примірник B-999:");
    service.IssueCopy("B-999");
}
catch (Exception ex)
{
    Console.WriteLine($"[ПОМИЛКА]: {ex.Message}");
}

try
{
    Console.WriteLine($"-> Спроба додати дублікат ID ({created.Id}):");
    service.AddBookWithId(created.Id, "978-0000000000");
}
catch (Exception ex)
{
    Console.WriteLine($"[ПОМИЛКА]: {ex.Message}");
}

Console.WriteLine("\n=== ПОШУК ЧЕРЕЗ FUNC (FindAll) ===");
service.IssueCopy(created.Id); 
var issuedBooks = service.FindAll(b => b.IsIssued == true);
Console.WriteLine($"Знайдено виданих книг через Func (b => b.IsIssued == true): {issuedBooks.Count}");


// ==========================================================
// ДОДАТКОВЕ ЗАВДАННЯ 2
// ==========================================================
Console.WriteLine("\n=== ДОДАТКОВЕ 2: Метод Import з Action<string> ===");
string[] importData = 
{
    "978-1111111111", // Валідний
    "поганий_рядок",   // Невалідний (немає 978-)
    " ",              // Порожній
    "978-2222222222"  // Валідний
};

// Викликаємо метод імпорту і передаємо лямбду, яка обробить помилкові рядки
int importedCount = service.Import(
    importData, 
    errorMsg => Console.WriteLine($"[ЛОГ ІМПОРТУ]: {errorMsg}") 
);

Console.WriteLine($"Успішно імпортовано книг: {importedCount}");
