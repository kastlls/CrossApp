using Core.Abstractions;
using Core.Storage;

namespace Cli;

public static class StoreFactory
{
    public static IBookStore Create(string[] args, string? resolvedFilePath = null) =>
        !string.IsNullOrWhiteSpace(resolvedFilePath)
            ? new CachingBookStore(new FileBookStore(resolvedFilePath))
            : new InMemoryBookStore();
}
