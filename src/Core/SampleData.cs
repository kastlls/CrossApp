using System;
using Core.Services;

namespace Core;

public static class SampleData
{
    public static void Initialize(LendingService service)
    {
        // Генеруємо 17 записів з різними ISBN для нормального групування
        var b1 = service.AddBook("978-001");
        var b2 = service.AddBook("978-001");
        var b3 = service.AddBook("978-001");
        var b4 = service.AddBook("978-002");
        var b5 = service.AddBook("978-002");
        var b6 = service.AddBook("978-003");
        var b7 = service.AddBook("978-003");
        var b8 = service.AddBook("978-004");
        var b9 = service.AddBook("978-005");
        var b10 = service.AddBook("978-006");
        var b11 = service.AddBook("978-006");
        var b12 = service.AddBook("978-007");
        var b13 = service.AddBook("978-008");
        var b14 = service.AddBook("978-009");
        var b15 = service.AddBook("978-010");
        var b16 = service.AddBook("978-010");
        var b17 = service.AddBook("978-010");

        // Робимо частину книг виданими, щоб працював звіт про доступні (фільтр)
        service.IssueCopy(b1.Id);
        service.IssueCopy(b4.Id);
        service.IssueCopy(b7.Id);
        service.IssueCopy(b10.Id);
        service.IssueCopy(b15.Id);
    }
}
