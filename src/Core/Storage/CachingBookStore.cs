using System;
using System.Collections.Generic;
using Core.Abstractions;
using Core.Domain;

namespace Core.Storage;

public sealed class CachingBookStore(IBookStore innerStore) : IBookStore
{
    private readonly Dictionary<string, BookCopy> _cache = new();

    public BookCopy? GetById(string id)
    {
        // Тихо повертаємо з кешу, якщо книга там є
        if (_cache.TryGetValue(id, out var cached)) return cached;
        
        var item = innerStore.GetById(id);
        if (item != null) _cache[id] = item;
        
        return item;
    }

    public IReadOnlyList<BookCopy> List() => innerStore.List();
    public void Add(BookCopy item) => innerStore.Add(item);
    public void Update(BookCopy item) => innerStore.Update(item);
    public bool Remove(string id) => innerStore.Remove(id);
    public IEnumerable<BookCopy> Search(Func<BookCopy, bool> predicate) => innerStore.Search(predicate);
}
