using System.Collections.Generic;
using System.Linq;
using Core.Abstractions;
using Core.Domain;
namespace Core.Tests;
public class TestStore : IBookStore
{
    private readonly List<BookCopy> _books = new();
    public void Add(BookCopy book) => _books.Add(book);
    public BookCopy? GetById(string id) => _books.FirstOrDefault(b => b.Id == id);
    public IEnumerable<BookCopy> GetAll() => _books;
    public void Update(BookCopy book) { }
}
