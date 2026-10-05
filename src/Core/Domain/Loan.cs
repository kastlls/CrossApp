using System;
using Core.Dto; // Підключаємо DTO з 3-го тижня

namespace Core.Domain;

public sealed class Loan
{
    public string Id { get; }
    public string CopyId { get; }
    public string ReaderId { get; }
    public DateTime IssuedOn { get; }
    
    public DateTime? ReturnedOn { get; private set; }

    private Loan(string id, string copyId, string readerId, DateTime issuedOn, DateTime? returnedOn)
    {
        Id = id;
        CopyId = copyId;
        ReaderId = readerId;
        IssuedOn = issuedOn;
        ReturnedOn = returnedOn;
    }

    // Відкриття нової видачі (вимагає сам об'єкт примірника)
    public static Loan Open(string id, BookCopy copy, string readerId, DateTime issuedOn)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Ідентифікатор обов'язковий", nameof(id));
        if (copy == null) throw new ArgumentNullException(nameof(copy));
        if (string.IsNullOrWhiteSpace(readerId)) throw new ArgumentException("Ідентифікатор читача обов'язковий", nameof(readerId));

        return new Loan(id.Trim(), copy.Id, readerId.Trim(), issuedOn, null);
    }

    // Внутрішня фабрика для відновлення з файлу (проходить всі перевірки інваріантів)
    public static Loan Restore(string id, string copyId, string readerId, DateTime issuedOn, DateTime? returnedOn)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Ідентифікатор обов'язковий", nameof(id));
        if (string.IsNullOrWhiteSpace(copyId)) throw new ArgumentException("CopyId обов'язковий", nameof(copyId));
        if (string.IsNullOrWhiteSpace(readerId)) throw new ArgumentException("ReaderId обов'язковий", nameof(readerId));
        
        // Інваріант: перевірка дати під час відновлення[cite: 11]
        if (returnedOn.HasValue && returnedOn < issuedOn)
            throw new ArgumentOutOfRangeException(nameof(returnedOn), returnedOn, "Дата повернення не може бути раніше дати видачі");

        return new Loan(id.Trim(), copyId.Trim(), readerId.Trim(), issuedOn, returnedOn);
    }

    public void Close(DateTime returnedOn)
    {
        if (ReturnedOn.HasValue)
            throw new InvalidOperationException($"Видача {Id} вже закрита");
        if (returnedOn < IssuedOn)
            throw new ArgumentOutOfRangeException(nameof(returnedOn), returnedOn, "Дата повернення не може бути раніше дати видачі");

        ReturnedOn = returnedOn;
    }

    // --- КРОК 6: Мапінг ToDto та FromDto ---[cite: 4]
    
    public LoanDto ToDto() => new(Id, CopyId, ReaderId, IssuedOn, ReturnedOn);

    // Відновлення. Використовує Restore, щоб перевірити всі дані перед створенням[cite: 10, 14]
    public static Loan FromDto(LoanDto dto) =>
        Restore(dto.Id, dto.CopyId, dto.ReaderId, dto.IssuedOn, dto.ReturnedOn);

    public override string ToString() => $"Видача {Id}: Примірник {CopyId} -> Читач {ReaderId} ({(ReturnedOn.HasValue ? "Закрита" : "Відкрита")})";
}
