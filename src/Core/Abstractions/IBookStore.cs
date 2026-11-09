using System.Collections.Generic;
using Core.Domain;

namespace Core.Abstractions;

public interface IBookStore
{
    void Add(BookCopy book);
    BookCopy? GetById(string id);
    IEnumerable<BookCopy> GetAll();
    void Update(BookCopy book);
}
