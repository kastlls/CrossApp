using System;
using System.Collections.Generic;
using System.Linq;
using Core.Abstractions;
using Core.Domain;
using Core.Events;

namespace Core.Services;

public sealed class LendingService(IBookStore store)
{
    private readonly IBookStore _store = store ?? throw new ArgumentNullException(nameof(store));
    public event EventHandler<LibraryChangedEventArgs>? Changed;

    private void OnChanged(string id, LibraryChangeKind kind) =>
        Changed?.Invoke(this, new LibraryChangedEventArgs(id, kind));

    public BookCopy AddBook(string isbn)
    {
        var id = "B-" + Guid.NewGuid().ToString("N")[..6];
        // Використовуємо фабричний метод або конструктор з правильними параметрами
        var copy = BookCopy.Create(id, isbn); 
        _store.Add(copy);
        OnChanged(copy.Id, LibraryChangeKind.Added);
        return copy;
    }

    public void AddBookWithId(string id, string isbn)
    {
        var copy = BookCopy.Create(id, isbn);
        _store.Add(copy);
        OnChanged(copy.Id, LibraryChangeKind.Added);
    }

    public void IssueCopy(string id)
    {
        var copy = _store.GetById(id) ?? throw new InvalidOperationException($"Немає запису з id={id}");
        // Використовуємо доменний метод замість прямого доступу до IsIssued
        copy.Issue(); 
        _store.Update(copy);
        OnChanged(copy.Id, LibraryChangeKind.Issued);
    }

    public void ReturnCopy(string id)
    {
        var copy = _store.GetById(id) ?? throw new InvalidOperationException($"Немає запису з id={id}");
        // Використовуємо доменний метод
        copy.Return(); 
        _store.Update(copy);
        OnChanged(copy.Id, LibraryChangeKind.Returned);
    }

    public BookCopy? Find(string id) => _store.GetById(id);
    
    public IReadOnlyList<BookCopy> All() => _store.List();

    public IReadOnlyList<BookCopy> FindAll(Func<BookCopy, bool> predicate) =>
        _store.List().Where(predicate).ToList();
}
