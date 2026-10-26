using System;
using System.Collections.Generic;
using System.Linq;
using Core.Abstractions;
using Core.Domain;
using Core.Events;

namespace Core.Services;

public sealed class LendingService
{
    private readonly IBookStore _store;
    private readonly IEnumerable<INotificationSink> _sinks;

    // ДОДАТКОВЕ 1: Впровадження залежностей (DI) через конструктор
    public LendingService(IBookStore store, IEnumerable<INotificationSink>? sinks = null)
    {
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _sinks = sinks ?? Array.Empty<INotificationSink>();
    }

    public event EventHandler<LibraryChangedEventArgs>? Changed;

    private void OnChanged(string id, LibraryChangeKind kind)
    {
        var args = new LibraryChangedEventArgs(id, kind);
        
        // 1. Сповіщення через стандартну подію (Основне завдання)
        Changed?.Invoke(this, args);
        
        // 2. Сповіщення через DI Sinks (Додаткове завдання 1)
        foreach (var sink in _sinks)
        {
            sink.Notify(args);
        }
    }

    public BookCopy AddBook(string isbn)
    {
        var id = "B-" + Guid.NewGuid().ToString("N")[..6];
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
        copy.Issue(); 
        _store.Update(copy);
        OnChanged(copy.Id, LibraryChangeKind.Issued);
    }

    public void ReturnCopy(string id)
    {
        var copy = _store.GetById(id) ?? throw new InvalidOperationException($"Немає запису з id={id}");
        copy.Return(); 
        _store.Update(copy);
        OnChanged(copy.Id, LibraryChangeKind.Returned);
    }

    public BookCopy? Find(string id) => _store.GetById(id);
    
    public IReadOnlyList<BookCopy> All() => _store.List();

    public IReadOnlyList<BookCopy> FindAll(Func<BookCopy, bool> predicate) =>
        _store.List().Where(predicate).ToList();

    // ДОДАТКОВЕ 2: Метод імпорту з Action<string>
    public int Import(string[] lines, Action<string> onSkipped)
    {
        int addedCount = 0;
        foreach (var line in lines)
        {
            // Проста перевірка: чи рядок не порожній і чи схожий на ISBN
            if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("978-"))
            {
                // Повертаємо повідомлення про помилку через Action
                onSkipped($"Пропущено некоректний рядок: '{line}'");
                continue;
            }
            AddBook(line.Trim());
            addedCount++;
        }
        return addedCount;
    }
}
