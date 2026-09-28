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

// --- ЧАСТИНА 2: Імпорт CSV/JSON (Лаб 3) ---
string path = args.FirstOrDefault(a => a != "--json") ?? Path.Combine("data", "sample.csv");

if (!File.Exists(path))
{
    Console.WriteLine($"Файл не знайдено: {Path.GetFullPath(path)}");
    return 1;
}

// ДОДАНО: Вибір імпортера за розширенням
ImportResult<object> result = Path.GetExtension(path).ToLowerInvariant() switch
{
    ".csv" => BookCsvImporter.Load(path),
    ".json" => BookJsonImporter.Load(path),
    var ext => throw new InvalidOperationException($"Непідтримуваний формат файлу: {ext}")
};

Console.WriteLine($"Завантажено записів: {result.Items.Count}");
Console.WriteLine(new string('-', 85));

// ДОДАНО: Розпізнавання різних типів для виводу
foreach (object item in result.Items.Take(5))
{
    switch (item)
    {
        case BookDto b:
            Console.WriteLine($" [КНИГА] {b.Id,-6} | {b.Isbn,-17} | {b.Title,-32} | {b.Year,4} | {b.Author ?? "-"}");
            break;
        case ReaderDto r:
            Console.WriteLine($" [ЧИТАЧ] {r.Id,-6} | {r.FullName,-17} | {r.Phone,-32} | {r.Email ?? "-"}");
            break;
    }
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

// ДОДАНО: Статистика одним рядком
Console.WriteLine(new string('-', 85));
int total = result.Items.Count + result.Errors.Count;
double errorPercent = total > 0 ? (double)result.Errors.Count / total * 100 : 0;
Console.WriteLine($"СТАТИСТИКА: Усього: {total} | Прийнято: {result.Items.Count} | Пропущено: {result.Errors.Count} | % помилок: {errorPercent:F1}%");

return 0;
