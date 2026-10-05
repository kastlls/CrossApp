using System;
using System.Collections.Generic;
using Core.Domain;

Console.WriteLine("=== СЦЕНАРІЙ 1: Успішний життєвий цикл ===");
var copy = BookCopy.Create("B-001", "978-1234567890");
copy.Issue();
var loan = Loan.Open("L-100", copy, "R-999", DateTime.Now.AddDays(-5), activeLoansCount: 0);
Console.WriteLine($"Відкрито видачу: {loan}");
loan.Close(DateTime.Now);
copy.Return();
Console.WriteLine($"Після повернення: {loan}");
Console.WriteLine();

Console.WriteLine("=== СЦЕНАРІЙ 2: Порушення інваріантів та Бонуси 2 і 3 ===");
try { BookCopy.Create("B-003", ""); }
catch (Exception ex) { Console.WriteLine($"[ПОМИЛКА (Порожній ISBN)]: {ex.Message}"); }

try { Loan.Open("L-102", copy, "R-888", DateTime.Now, activeLoansCount: 5); }
catch (Exception ex) { Console.WriteLine($"[ПОМИЛКА (Бонус 2 - Ліміт видач)]: {ex.Message}"); }

try
{
    var testCopy = BookCopy.Create("B-999", "123456");
    var testLoan = Loan.Open("L-999", testCopy, "R-111", DateTime.Now);
    testLoan.Close(DateTime.Now); // Статус стає Closed
    testLoan.ChangeStatus(LoanStatus.Lost); // Спроба змінити Closed на Lost
}
catch (Exception ex) { Console.WriteLine($"[ПОМИЛКА (Бонус 3 - Enum State)]: {ex.Message}\n"); }

Console.WriteLine("=== СЦЕНАРІЙ 3: Обробка імпорту з інваріантами (Бонус 1) ===");
// 1. Імітуємо дані з парсера CSV (DTO + синтаксичні помилки)
var parsedDtos = new List<Core.Dto.BookCopyDto>
{
    new("B-004", "978-1111111111", false), // Нормальний примірник
    new("B-005", "", false),               // Порушує інваріант (порожній ISBN)
    new("   ", "978-2222222222", false)    // Порушує інваріант (порожній ID)
};
var finalErrors = new List<string> { "Помилка парсингу CSV: рядок 2 пропущено" };
var validEntities = new List<BookCopy>();

// 2. Метод мапінгу (перетворення DTO у Domain)
foreach (var dto in parsedDtos)
{
    try
    {
        validEntities.Add(BookCopy.FromDto(dto));
    }
    catch (Exception ex)
    {
        finalErrors.Add($"Рядок [{dto.Id}]: {ex.Message}");
    }
}

// 3. Вивід результату (Дані + Помилки)
Console.WriteLine($"Успішно перетворено сутностей: {validEntities.Count}");
foreach (var entity in validEntities) 
    Console.WriteLine($" [v] {entity}");

Console.WriteLine($"Загальна кількість помилок (синтаксис + інваріанти): {finalErrors.Count}");
foreach (var err in finalErrors) 
    Console.WriteLine($" [x] {err}");
Console.WriteLine();
