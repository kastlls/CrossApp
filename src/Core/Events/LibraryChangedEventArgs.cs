using System;

namespace Core.Events;

public sealed class LibraryChangedEventArgs(string id, LibraryChangeKind kind) : EventArgs
{
    public string Id { get; } = id;
    public LibraryChangeKind Kind { get; } = kind;
    public DateTimeOffset At { get; } = DateTimeOffset.Now;
}
