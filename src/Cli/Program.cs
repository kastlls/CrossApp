using System;
using Core.Domain;

// === СЦЕНАРІЙ 1: Успішна робота (дотримання інваріантів) ===
Console.WriteLine("=== СЦЕНАРІЙ 1: Успішний ===");
var copy = BookCopy.Create("B-001", "978-1234567890");
Console.WriteLine($"Створено: {copy}");

copy.Issue();
Console.WriteLine($"Після видачі: {copy}");

var loan = Loan.Open("L-100", copy, "R-999", DateTime.Now.AddDays(-5));
Console.WriteLine($"Відкрито видачу на читача {loan.ReaderId}");

loan.Close(DateTime.Now);
copy.Return();
Console.WriteLine($"Після повернення: {copy}");
Console.WriteLine();


// === СЦЕНАРІЙ 2: Порушення інваріантів (try / catch) ===
Console.WriteLine("=== СЦЕНАРІЙ 2: Порушення інваріантів ===");

// Порушення 1: Подвійна видача (InvalidOperationException)
try
{
    Console.WriteLine("-> Спроба видати примірник, який ВЖЕ виданий:");
    var copy2 = BookCopy.Create("B-002", "978-0000000000");
    copy2.Issue(); // Успішно
    copy2.Issue(); // Помилка: вже виданий
}
catch (Exception ex)
{
    // Виводимо ЛИШЕ Message, без страшного червоного stack trace
    Console.WriteLine($"[ПОМИЛКА]: {ex.Message}\n");
}

// Порушення 2: Неправильні аргументи при створенні (ArgumentException)
try
{
    Console.WriteLine("-> Спроба створити примірник з порожнім ISBN:");
    var badCopy = BookCopy.Create("B-003", "");
}
catch (Exception ex)
{
    Console.WriteLine($"[ПОМИЛКА]: {ex.Message}\n");
}

// Порушення 3: Машина часу (ArgumentOutOfRangeException)
try
{
    Console.WriteLine("-> Спроба повернути книгу заднім числом:");
    var badLoan = Loan.Open("L-101", copy, "R-999", DateTime.Now);
    badLoan.Close(DateTime.Now.AddDays(-2)); // Дата закриття раніше дати видачі
}
catch (Exception ex)
{
    Console.WriteLine($"[ПОМИЛКА]: {ex.Message}\n");
}
