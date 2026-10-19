using System;
using System.Collections.Generic;
using Core.Abstractions;
using Core.Domain;

namespace Core.Services;

public sealed class LendingService(IBookStore store)
{
    private readonly IBookStore _store = store ?? throw new ArgumentNullException(nameof(store));

    public BookCopy AddBook(string isbn)
    {
        var id = "B-" + Guid.NewGuid().ToString("N")[..6].ToUpper();
        var copy = BookCopy.Create(id, isbn);
        _store.Add(copy);
        return copy;
    }

    public BookCopy AddBookWithId(string id, string isbn)
    {
        var copy = BookCopy.Create(id, isbn);
        _store.Add(copy);
        return copy;
    }

    public void IssueCopy(string id)
    {
        var copy = _store.GetById(id)
            ?? throw new InvalidOperationException($"Немає примірника з id={id}.");
        copy.Issue();
        _store.Update(copy);
    }

    public void ReturnCopy(string id)
    {
        var copy = _store.GetById(id)
            ?? throw new InvalidOperationException($"Немає примірника з id={id}.");
        copy.Return();
        _store.Update(copy);
    }

    public IReadOnlyList<BookCopy> All() => _store.List();

    public BookCopy? Find(string id) => _store.GetById(id);
}
