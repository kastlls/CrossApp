using System.Collections.Generic;
using System.Linq;
using Core.Domain;
using Core.Services;
namespace Core.Tests;
public static class TestData
{
    public static List<BookCopy> Items()
    {
        var store = new TestStore();
        var srv = new LendingService(store);
        var b1 = srv.AddBook("978-100");
        var b2 = srv.AddBook("978-200");
        var b3 = srv.AddBook("978-200");
        srv.IssueCopy(b1.Id);
        return store.GetAll().ToList();
    }
}
