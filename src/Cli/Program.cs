using System;
using System.IO;
using System.Linq;
using Core;
using Core.Abstractions;
using Core.Services;
using Core.Storage;

bool useFile = args.Contains("--file");
string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");

IBookStore store = useFile
    ? new FileBookStore(dataPath)
    : new InMemoryBookStore(SampleData.Books());

var service = new LendingService(store);

Console.WriteLine($"Сховище: {store.GetType().Name}");

// 1. Додавання нового примірника, зміна стану (видача та повернення)
var created = service.AddBook("978-9660000001");
Console.WriteLine($"Додано новий примірник: {created.Id} (ISBN: {created.Isbn})");

service.IssueCopy(created.Id);
Console.WriteLine($"Після видачі (Find): {service.Find(created.Id)}");

service.ReturnCopy(created.Id);
Console.WriteLine($"Після повернення (Find): {service.Find(created.Id)}");

// 2. Вивід списку всіх записів
Console.WriteLine($"\nУсього записів у сховищі ({service.All().Count}):");
foreach (var b in service.All())
{
    Console.WriteLine($" {b.Id,-10} ISBN: {b.Isbn,-16} Видано: {b.IsIssued}");
}

// 3. Сценарії відмови (перехоплення винятків)
Console.WriteLine("\n=== СЦЕНАРІЇ ВІДМОВИ ===");
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
