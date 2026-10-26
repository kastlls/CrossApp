using System;
using System.IO;
using System.Linq;
using Cli;
using Core.Services;
using Core.Events;

var store = StoreFactory.Create(args);
var service = new LendingService(store);

bool useFile = args.Contains("--file");
Console.WriteLine($"Сховище: {(useFile ? "FileBookStore" : "InMemoryBookStore")}\n");

// --- КРОК 3: Підписка локальним методом з виведенням у лог ---
var logPath = Path.Combine(AppContext.BaseDirectory, "logs", "app.log");
Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);

void OnLibraryChanged(object? sender, LibraryChangedEventArgs e)
{
    var line = $"{e.At:HH:mm:ss} {e.Kind,-8} {e.Id}";
    Console.WriteLine(line);
    File.AppendAllText(logPath, line + Environment.NewLine);
}

service.Changed += OnLibraryChanged;

// --- КРОК 4: Другий підписник (лямбда із замиканням) ---
int changes = 0;
service.Changed += (_, _) => changes++;

// --- КРОК 5: Сценарій (зміни -> відписка -> тиха зміна) ---
Console.WriteLine("=== СЦЕНАРІЙ: ЗМІНИ І ПОДІЇ ===");
var created = service.AddBook("978-9660000001");
service.IssueCopy(created.Id);
Console.WriteLine($"Подій отримано: {changes}");

Console.WriteLine("\n=== СЦЕНАРІЙ: ВІДПИСКА ОСНОВНОГО ОБРОБНИКА ===");
service.Changed -= OnLibraryChanged;
service.ReturnCopy(created.Id); // Тиха операція: у консолі та лозі нічого не буде
Console.WriteLine($"Після тихого повернення. Подій отримано: {changes} (+1 лише від лямбди-лічильника)");

// --- КРОК 6: Сценарії відмови (подій не має бути) ---
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

// --- КРОК 7: Делегат Func замість події ---
Console.WriteLine("\n=== ПОШУК ЧЕРЕЗ FUNC (FindAll) ===");
service.IssueCopy(created.Id); // видамо знову для тесту
var issuedBooks = service.FindAll(b => b.IsIssued == true);
Console.WriteLine($"Знайдено виданих книг через Func (b => b.IsIssued == true): {issuedBooks.Count}");
