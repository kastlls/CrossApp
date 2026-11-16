using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Cli;
using Core;
using Core.Configuration;
using Core.Exceptions;
using Core.Reports;
using Core.Services;
using Core.Storage;
using Microsoft.Extensions.Configuration;

var envName = Environment.GetEnvironmentVariable("CROSSAPP_ENVIRONMENT") ?? "Development";

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile($"appsettings.{envName}.json", optional: true)
    .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
    .AddEnvironmentVariables(prefix: "CROSSAPP_")
    .AddCommandLine(args)
    .Build();

var options = config.Get<AppOptions>() ?? new AppOptions();
var legacyDataPath = Environment.GetEnvironmentVariable("CROSSAPP_DATA_PATH");

var storage = new StorageOptions
{
    DataPath = string.IsNullOrWhiteSpace(legacyDataPath) ? options.Storage.DataPath : legacyDataPath,
    FileName = options.Storage.FileName
};

var dataFilePath = storage.ResolveFullPath(AppContext.BaseDirectory);
var apiKey = config["Integration:ApiKey"];

Console.WriteLine($"Середовище: {envName} | Термін видачі: {options.Library.LoanDays} дн.");
Console.WriteLine($"Файл даних: {dataFilePath}");
Console.WriteLine(apiKey is null ? "ApiKey: не заданий" : $"ApiKey: присутній, довжина {apiKey.Length}");

var store = StoreFactory.Create(args, dataFilePath);
var service = new LendingService(store);

if (!service.All().Any()) SampleData.Initialize(service);

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

var command = args.FirstOrDefault(a => !a.StartsWith("--")) ?? "report-top";

switch (command)
{
    case "export":
        var allBooks = service.All().ToList();
        foreach (var b in allBooks) store.Update(b);
        Console.WriteLine($"Експортовано {allBooks.Count} записів.");
        break;

    case "import":
        var importPathArg = args.SkipWhile(a => a != "--file").Skip(1).FirstOrDefault();
        var importPath = string.IsNullOrWhiteSpace(importPathArg) ? dataFilePath : Path.GetFullPath(importPathArg);
        try
        {
            var added = await service.ImportAsync(new JsonBookImporter(), importPath, cts.Token);
            Console.WriteLine($"Імпортовано записів: {added}");
        }
        catch (OperationCanceledException) { Console.WriteLine("Імпорт скасовано."); }
        catch (DomainException ex) { Console.WriteLine($"Дані відхилено: {ex.Message}"); }
        catch (FileNotFoundException) { Console.WriteLine($"Файл не знайдено: {importPath}"); }
        break;

    case "report-top":
        Console.WriteLine("\n=== ЗВІТИ (CLI) ===");
        Print(ReportService.TopBooks(service.All(), 5));
        break;

    case "report-available":
        Console.WriteLine("\n=== ЗВІТИ (CLI) ===");
        Print(ReportService.AvailableBooks(service.All()));
        break;

    case "report-groups":
        Console.WriteLine("\n=== ЗВІТИ (CLI) ===");
        foreach (var g in ReportService.ByIsbn(service.All()))
            Console.WriteLine($" ISBN {g.Key,-10} | Кількість: {g.Count,3}");
        break;

    case "report-sum":
        Console.WriteLine("\n=== ЗВІТИ (CLI) ===");
        var t = ReportService.Totals(service.All());
        Console.WriteLine($"Всього: {t.Items}, Видано: {t.Issued}");
        break;

    default:
        Console.WriteLine("Команди: export | import [--file path] | report-top | report-available | report-groups | report-sum");
        break;
}

static void Print(IReadOnlyList<StockLine> lines)
{
    foreach (var l in lines) Console.WriteLine($" {l.Id,-10} ISBN: {l.Isbn,-12} Видано: {l.IsIssued}");
}
