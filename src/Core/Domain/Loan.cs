using System;
using Core.Dto;

namespace Core.Domain;

// 1. Явний стан-перелічування (enum)
public enum LoanStatus 
{ 
    Active,   // Активна видача
    Closed,   // Успішно повернуто
    Lost      // Книгу втрачено
}

public sealed class Loan
{
    public string Id { get; }
    public string CopyId { get; }
    public string ReaderId { get; }
    public DateTime IssuedOn { get; }
    public DateTime? ReturnedOn { get; private set; }
    
    // Властивість з новим enum
    public LoanStatus Status { get; private set; }

    private Loan(string id, string copyId, string readerId, DateTime issuedOn, DateTime? returnedOn, LoanStatus status)
    {
        Id = id;
        CopyId = copyId;
        ReaderId = readerId;
        IssuedOn = issuedOn;
        ReturnedOn = returnedOn;
        Status = status;
    }

    public static Loan Open(string id, BookCopy copy, string readerId, DateTime issuedOn, int activeLoansCount = 0)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Id обов'язковий", nameof(id));
        if (copy == null) throw new ArgumentNullException(nameof(copy));
        if (string.IsNullOrWhiteSpace(readerId)) throw new ArgumentException("ReaderId обов'язковий", nameof(readerId));
        if (activeLoansCount >= 5) throw new InvalidOperationException($"Читач {readerId} має {activeLoansCount} видач. Ліміт.");

        // При створенні статус завжди Active
        return new Loan(id.Trim(), copy.Id, readerId.Trim(), issuedOn, null, LoanStatus.Active);
    }

    public static Loan Restore(string id, string copyId, string readerId, DateTime issuedOn, DateTime? returnedOn)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Id обов'язковий", nameof(id));
        if (string.IsNullOrWhiteSpace(copyId)) throw new ArgumentException("CopyId обов'язковий", nameof(copyId));
        if (string.IsNullOrWhiteSpace(readerId)) throw new ArgumentException("ReaderId обов'язковий", nameof(readerId));
        if (returnedOn.HasValue && returnedOn < issuedOn) throw new ArgumentOutOfRangeException(nameof(returnedOn), returnedOn, "Дата помилкова");

        var status = returnedOn.HasValue ? LoanStatus.Closed : LoanStatus.Active;
        return new Loan(id.Trim(), copyId.Trim(), readerId.Trim(), issuedOn, returnedOn, status);
    }

    // 2. Перевірка допустимих переходів через switch expression
    public void ChangeStatus(LoanStatus newStatus)
    {
        Status = (Status, newStatus) switch
        {
            (LoanStatus.Active, LoanStatus.Closed) => newStatus, // Можна успішно закрити
            (LoanStatus.Active, LoanStatus.Lost) => newStatus,   // Можна позначити як втрачену
            
            // Всі інші переходи (наприклад, з Closed у Lost) ЗАБОРОНЕНІ
            _ => throw new InvalidOperationException($"Неможливий перехід статусу з {Status} у {newStatus}")
        };
    }

    public void Close(DateTime returnedOn)
    {
        // Перевіряємо через наш новий метод, чи дозволений перехід у статус Closed
        ChangeStatus(LoanStatus.Closed);
        
        if (returnedOn < IssuedOn) throw new ArgumentOutOfRangeException(nameof(returnedOn), returnedOn, "Дата повернення не може бути раніше дати видачі");
        ReturnedOn = returnedOn;
    }

    public LoanDto ToDto() => new(Id, CopyId, ReaderId, IssuedOn, ReturnedOn);
    public static Loan FromDto(LoanDto dto) => Restore(dto.Id, dto.CopyId, dto.ReaderId, dto.IssuedOn, dto.ReturnedOn);
    public override string ToString() => $"Видача {Id}: Примірник {CopyId} -> Читач {ReaderId} [{Status}]";
}
