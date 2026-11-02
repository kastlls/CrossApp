using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Cli;
using Core;
using Core.Abstractions;
using Core.Domain;
using Core.Reports;
using Core.Services;

var store = StoreFactory.Create(args);
var service = new LendingService(store);

// Правильний підхід: ініціалізуємо тестові дані через виклик зовнішнього класу
SampleData.Initialize(service);

// ==========================================================
// КРОК 3. Демонстрація відкладеного виконання LINQ
// ==========================================================
Console.WriteLine("\n=== ДЕМО: Відкладене виконання ===");
var sourceList = service.All().ToList();
var query = sourceList.Where(b => !b.IsIssued); 

Console.WriteLine($"Доступних до змін: {query.Count()}"); 
sourceList.Add(BookCopy.Create("B-TMP99", "978-999")); 
Console.WriteLine($"Доступних після змін (лічильник виріс): {query.Count()}");

var snapshot = query.ToList(); 
sourceList.Clear();
Console.WriteLine($"У знімку лишилось (не залежить від джерела): {snapshot.Count}");

// ==========================================================
// КРОК 4. Підкоманди звітів через switch
// ==========================================================
Console.WriteLine("\n=== ЗВІТИ (CLI) ===");
var items = service.All();
var command = args.FirstOrDefault(a => !a.StartsWith("--")) ?? "report-top";

switch (command)
{
    case "report-top":
        Console.WriteLine("Топ 5 книг (стабільне сортування):");
        Print(ReportService.TopBooks(items, 5));
        break;
    case "report-available":
        Console.WriteLine("Доступні книги:");
        Print(ReportService.AvailableBooks(items));
        break;
    case "report-groups":
        Console.WriteLine("Групування за ISBN:");
        foreach (var g in ReportService.ByIsbn(items))
            Console.WriteLine($" ISBN {g.Key,-10} | Кількість: {g.Count,3}");
        break;
    case "report-sum":
        var t = ReportService.Totals(items);
        Console.WriteLine($"Всього позицій: {t.Items}, Видано: {t.Issued}, Відсоток виданих: {t.IssuedPercentage:F2}%");
        break;
    default:
        Console.WriteLine("Доступні команди: report-top | report-available | report-groups | report-sum");
        break;
}

static void Print(IReadOnlyList<StockLine> lines)
{
    foreach (var l in lines)
        Console.WriteLine($" {l.Id,-10} ISBN: {l.Isbn,-12} Видано: {l.IsIssued}");
}
