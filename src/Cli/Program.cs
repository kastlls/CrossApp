using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using Core;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// Отримуємо дані середовища через Core (жодних прямих викликів RuntimeInformation тут)
EnvironmentReport report = EnvironmentInfo.Collect();

var information = new
{
    OSDescription = report.OsDescription,
    FrameworkDescription = report.FrameworkDescription,
    ProcessArchitecture = report.ProcessArchitecture,
    DetectedRid = report.DetectedRid,
    ReportedRid = report.ReportedRid,
    ApplicationDirectory = report.BaseDirectory,
    BuildNote = report.BuildNote, // Додано сюди для JSON
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
}
else
{
    Console.WriteLine("CrossApp – практикум з крос-платформного програмування");
    Console.WriteLine("Студент: Кулик Арсен, група: ФЕІ-33");
    Console.WriteLine(new string('-', 52));

    Console.WriteLine($"ОС             : {report.OsDescription}");
    Console.WriteLine($"Runtime        : {report.FrameworkDescription}");
    Console.WriteLine($"Архітектура    : {report.ProcessArchitecture}");
    Console.WriteLine($"RID (визначено): {report.DetectedRid}");
    Console.WriteLine($"RID (від .NET) : {report.ReportedRid}");
    Console.WriteLine($"Каталог        : {report.BaseDirectory}");
    Console.WriteLine($"TFM Примітка   : {report.BuildNote}"); // Додано сюди для консолі

    Console.WriteLine(new string('-', 52));

    Console.WriteLine("Предметна область: Бібліотека");
    Console.WriteLine("Сутності: Book, BookCopy, Reader, Loan");
    Console.WriteLine("Призначення: облік видач примірників книг читачам.");
}