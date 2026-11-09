using System;

namespace Core.Domain;

public class BookCopy
{
    public string Id { get; }
    public string Isbn { get; }
    public bool IsIssued { get; set; } // Додано публічний set;

    private BookCopy(string id, string isbn)
    {
        Id = id;
        Isbn = isbn;
        IsIssued = false;
    }

    public static BookCopy Create(string id, string isbn)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("ID не може бути порожнім");
        if (string.IsNullOrWhiteSpace(isbn)) throw new ArgumentException("ISBN не може бути порожнім");
        return new BookCopy(id, isbn);
    }
}
