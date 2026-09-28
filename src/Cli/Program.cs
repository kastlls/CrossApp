using System;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using Core;
using Core.Dto;
using Core.Import;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// --- ЧАСТИНА 1: Інформація про середовище (Лаб 1-2) ---
EnvironmentReport report = EnvironmentInfo.Collect();

var information = new
{
    OSDescription = report.OsDescription,
    FrameworkDescription = report.FrameworkDescription,
    ProcessArchitecture = report.ProcessArchitecture,
    DetectedRid = report.DetectedRid,
    ReportedRid = report.ReportedRid,
    ApplicationDirectory = report.BaseDirectory,
    BuildNote = report.BuildNote,
    Domain = "Бібліотека",
    Entities = "Book, BookCopy, Reader, Loan",
    Purpose = "облік видач примірників книг читачам."
};

if (args.Contains("--json"))
{
    var options = new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    Console.WriteLine(JsonSerializer.Serialize(information, options));
    return 0;
}

Console.WriteLine("CrossApp – практикум з крос-платформного програмування");
Console.WriteLine("Студент: Кулик Арсен, група: ФЕІ-33");
Console.WriteLine(new string('-', 85));
Console.WriteLine($"ОС             : {report.OsDescription}");
Console.WriteLine($"Runtime        : {report.FrameworkDescription}");
Console.WriteLine($"Каталог        : {report.BaseDirectory}");
Console.WriteLine(new string('-', 85));

// --- ЧАСТИНА 2: Імпорт CSV (Лаб 3) ---
string path = args.FirstOrDefault(a => a != "--json") ?? Path.Combine("data", "sample.csv");

if (!File.Exists(path))
{
    Console.WriteLine($"Файл не знайдено: {Path.GetFullPath(path)}");
    return 1;
}

ImportResult<BookDto> result = BookCsvImporter.Load(path);

Console.WriteLine($"Завантажено записів: {result.Items.Count}");
Console.WriteLine(new string('-', 85));

foreach (BookDto b in result.Items.Take(5))
{
    Console.WriteLine($" {b.Id,-6} | {b.Isbn,-17} | {b.Title,-32} | {b.Year,4} | {b.Author ?? "-"}");
}

if (result.Errors.Count > 0)
{
    Console.WriteLine(new string('-', 85));
    Console.WriteLine($"Пропущено рядків: {result.Errors.Count}");
    foreach (string error in result.Errors)
    {
        Console.WriteLine($" ! {error}");
    }
}

return 0;
