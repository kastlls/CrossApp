using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Abstractions;
using Core.Domain;
using Core.Exceptions;

namespace Core.Services;

public class LendingService
{
    private readonly IBookStore _store;
    public event EventHandler<EventArgs>? Changed;

    public LendingService(IBookStore store)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    public IEnumerable<BookCopy> All() => _store.GetAll();

    public BookCopy AddBook(string isbn)
    {
        var id = "B-" + Guid.NewGuid().ToString("N").Substring(0, 4).ToUpper();
        var book = BookCopy.Create(id, isbn);
        _store.Add(book);
        Changed?.Invoke(this, EventArgs.Empty);
        return book;
    }

    public void IssueCopy(string id)
    {
        var book = _store.GetById(id) ?? throw new ArgumentException("Книгу не знайдено");
        if (book.IsIssued) throw new DomainException("Примірник уже видано");
        book.IsIssued = true;
        _store.Update(book);
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void ReturnCopy(string id)
    {
        var book = _store.GetById(id) ?? throw new ArgumentException("Книгу не знайдено");
        if (!book.IsIssued) throw new DomainException("Примірник не був виданий");
        book.IsIssued = false;
        _store.Update(book);
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public async Task<int> ImportAsync(IBookImporter importer, string path, CancellationToken ct = default)
    {
        var items = await importer.LoadAsync(path, ct);
        var added = 0;
        foreach (var item in items)
        {
            ct.ThrowIfCancellationRequested();
            if (_store.GetById(item.Id) is not null) continue; // Ідемпотентність
            _store.Add(item);
            Changed?.Invoke(this, EventArgs.Empty);
            added++;
        }
        return added;
    }
}
