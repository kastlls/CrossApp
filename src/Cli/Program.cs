using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cli;
using Core;
using Core.Abstractions;
using Core.Domain;
using Core.Exceptions;
using Core.Reports;
using Core.Services;
using Core.Storage;

var store = StoreFactory.Create(args);
var service = new LendingService(store);
SampleData.Initialize(service);

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

var command = args.FirstOrDefault(a => !a.StartsWith("--")) ?? "report-top";

if (command == "import")
{
    try
    {
        Console.WriteLine("Починаємо асинхронний імпорт... (Натисніть Ctrl+C для скасування)");
        var importer = new JsonBookImporter();
        var added = await service.ImportAsync(importer, "data/sample.json", cts.Token);
        Console.WriteLine($"Імпортовано записів: {added}");
    }
    catch (OperationCanceledException) { Console.WriteLine("Імпорт скасовано."); }
    catch (DomainException ex) { Console.WriteLine($"Дані відхилено: {ex.Message}"); }
    catch (FileNotFoundException) { Console.WriteLine("Файл не знайдено: data/sample.json"); }
    return;
}

Console.WriteLine("\n=== ЗВІТИ (CLI) ===");
var items = service.All();
switch (command)
{
    case "report-top": Print(ReportService.TopBooks(items, 5)); break;
    case "report-available": Print(ReportService.AvailableBooks(items)); break;
    case "report-groups":
        foreach (var g in ReportService.ByIsbn(items)) Console.WriteLine($" ISBN {g.Key,-10} | Кількість: {g.Count,3}"); break;
    case "report-sum":
        var t = ReportService.Totals(items); Console.WriteLine($"Всього: {t.Items}, Видано: {t.Issued}"); break;
    default: Console.WriteLine("Команди: import | report-top | report-available | report-groups | report-sum"); break;
}

static void Print(IReadOnlyList<StockLine> lines)
{
    foreach (var l in lines) Console.WriteLine($" {l.Id,-10} ISBN: {l.Isbn,-12} Видано: {l.IsIssued}");
}
