using System;
using System.IO;
using System.Linq;
using Cli;
using Core.Services;

// Підключення ДОДАТКОВОГО 3 (Фабрика), яка під капотом збирає сховище і декоратор
var store = StoreFactory.Create(args);
var service = new LendingService(store);

// --- ВІДНОВЛЕНИЙ НОРМАЛЬНИЙ ВИВІД ЛАБОРАТОРНОЇ ---
bool useFile = args.Contains("--file");
Console.WriteLine($"Сховище: {(useFile ? "FileBookStore" : "InMemoryBookStore")}");

var created = service.AddBook("978-9660000001");
Console.WriteLine($"Додано новий примірник: {created.Id} (ISBN: {created.Isbn})");

service.IssueCopy(created.Id);
Console.WriteLine($"Після видачі (Find): {service.Find(created.Id)}");

service.ReturnCopy(created.Id);
Console.WriteLine($"Після повернення (Find): {service.Find(created.Id)}");

Console.WriteLine($"\nУсього записів у сховищі ({service.All().Count}):");
foreach (var b in service.All())
{
    Console.WriteLine($" {b.Id,-10} ISBN: {b.Isbn,-16} Видано: {b.IsIssued}");
}

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

// --- НОВИЙ БЛОК З ДОДАТКОВИМИ ЗАВДАННЯМИ ---
Console.WriteLine("\n=== ДОДАТКОВЕ 1: Кеш (Декоратор) ===");
Console.WriteLine("Шукаємо ту саму книгу ще раз. Завдяки CachingBookStore це робиться миттєво з кешу:");
var cachedBook = service.Find(created.Id);
Console.WriteLine($"Знайдено з кешу: {cachedBook?.Id}");

Console.WriteLine("\n=== ДОДАТКОВЕ 2: Пошук з Func ===");
service.IssueCopy(created.Id); // видамо знову для тесту
var issuedBooks = store.Search(b => b.IsIssued == true).ToList();
Console.WriteLine($"Знайдено виданих книг через Func (b => b.IsIssued == true): {issuedBooks.Count}");
