using System;
using Core.Dto; // Підключаємо DTO з 3-го тижня

namespace Core.Domain;

public sealed class BookCopy
{
    public string Id { get; }
    public string Isbn { get; }
    
    public bool IsIssued { get; private set; }

    private BookCopy(string id, string isbn, bool isIssued)
    {
        Id = id;
        Isbn = isbn;
        IsIssued = isIssued;
    }

    // Додано isIssued = false для зручності відновлення стану з DTO
    public static BookCopy Create(string id, string isbn, bool isIssued = false)
    {
        if (string.IsNullOrWhiteSpace(id)) 
            throw new ArgumentException("Ідентифікатор обов'язковий", nameof(id));
        if (string.IsNullOrWhiteSpace(isbn)) 
            throw new ArgumentException("ISBN не може бути порожнім", nameof(isbn));

        return new BookCopy(id.Trim(), isbn.Trim(), isIssued);
    }

    public void Issue()
    {
        if (IsIssued)
            throw new InvalidOperationException($"Примірник {Id} вже виданий, повторна видача неможлива");
            
        IsIssued = true;
    }

    public void Return()
    {
        if (!IsIssued)
            throw new InvalidOperationException($"Примірник {Id} не був виданий, його не можна повернути");
            
        IsIssued = false;
    }

    // --- КРОК 6: Мапінг ToDto та FromDto ---[cite: 4]
    
    // Сутність -> DTO (для збереження)
    public BookCopyDto ToDto() => new(Id, Isbn, IsIssued);

    // DTO -> Сутність (відновлення). Проходить ТІ САМІ перевірки, що й Create[cite: 4, 10]
    public static BookCopy FromDto(BookCopyDto dto) =>
        Create(dto.Id, dto.Isbn, dto.IsIssued);

    // Зрозумілий вивід для консолі
    public override string ToString() => $"{(IsIssued ? "[ВИДАНИЙ]" : "[ДОСТУПНИЙ]")} {Id} [{Isbn}]";
}
