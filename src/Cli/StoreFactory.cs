using System;
using System.IO;
using System.Linq;
using Core;
using Core.Abstractions;
using Core.Storage;

namespace Cli;

public static class StoreFactory
{
    public static IBookStore Create(string[] args)
    {
        bool useFile = args.Contains("--file");
        string dataPath = Path.Combine(AppContext.BaseDirectory, "data", "catalog.json");
        IBookStore baseStore = useFile ? new FileBookStore(dataPath) : new InMemoryBookStore(SampleData.Books());
        
        // Обертаємо базове сховище у декоратор
        return new CachingBookStore(baseStore);
    }
}
