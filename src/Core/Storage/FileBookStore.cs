using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Abstractions;
using Core.Domain;

namespace Core.Storage;

public class FileBookStore : IBookStore
{
    private readonly string _path;

    public FileBookStore(string path)
    {
        _path = path;
        if (!File.Exists(_path)) File.WriteAllText(_path, "");
    }

    public void Add(BookCopy book)
    {
        File.AppendAllLines(_path, new[] { $"{book.Id};{book.Isbn};{book.IsIssued}" });
    }

    public BookCopy? GetById(string id) => GetAll().FirstOrDefault(b => b.Id == id);

    // ДОДАНО: реалізація відсутніх методів
    public IEnumerable<BookCopy> GetAll()
    {
        if (!File.Exists(_path)) return Enumerable.Empty<BookCopy>();
        var lines = File.ReadAllLines(_path);
        var result = new List<BookCopy>();
        foreach (var line in lines)
        {
            var parts = line.Split(';');
            if (parts.Length >= 2)
            {
                var book = BookCopy.Create(parts[0], parts[1]);
                if (parts.Length >= 3 && bool.TryParse(parts[2], out var issued))
                {
                    book.IsIssued = issued;
                }
                result.Add(book);
            }
        }
        return result;
    }

    public void Update(BookCopy book)
    {
        var all = GetAll().ToList();
        var index = all.FindIndex(b => b.Id == book.Id);
        if (index >= 0)
        {
            all[index] = book;
            File.WriteAllLines(_path, all.Select(b => $"{b.Id};{b.Isbn};{b.IsIssued}"));
        }
    }
}
