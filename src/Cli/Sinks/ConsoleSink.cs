using System;
using Core.Abstractions;
using Core.Events;

namespace Cli.Sinks;

public class ConsoleSink : INotificationSink
{
    public void Notify(LibraryChangedEventArgs e)
    {
        // Додано префікс [DI ConsoleSink], щоб відрізняти від звичайної події в консолі
        Console.WriteLine($"[DI ConsoleSink] {e.At:HH:mm:ss} {e.Kind} {e.Id}");
    }
}
