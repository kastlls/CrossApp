using System.Collections.Generic;
using Core.Abstractions;
using Core.Domain;

namespace Core.Storage;

public class CachingBookStore : IBookStore
{
    private readonly IBookStore _inner;
    private readonly Dictionary<string, BookCopy> _cache = new();

    public CachingBookStore(IBookStore inner)
    {
        _inner = inner;
    }

    public void Add(BookCopy book)
    {
        _inner.Add(book);
        _cache[book.Id] = book;
    }

    public BookCopy? GetById(string id)
    {
        if (_cache.TryGetValue(id, out var cached)) return cached;
        var book = _inner.GetById(id);
        if (book != null) _cache[id] = book;
        return book;
    }

    // ДОДАНО: реалізація відсутніх методів
    public IEnumerable<BookCopy> GetAll() => _inner.GetAll();
    
    public void Update(BookCopy book)
    {
        _inner.Update(book);
        _cache[book.Id] = book;
    }
}
