using System;
using System.IO;
using Core.Abstractions;
using Core.Events;

namespace Cli.Sinks;

public class FileSink : INotificationSink
{
    private readonly string _path;
    public FileSink(string path)
    {
        _path = path;
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
    }

    public void Notify(LibraryChangedEventArgs e)
    {
        File.AppendAllText(_path, $"[DI FileSink] {e.At:HH:mm:ss} {e.Kind} {e.Id}\n");
    }
}
