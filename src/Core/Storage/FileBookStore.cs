using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using Core.Abstractions;
using Core.Domain;
using Core.Exceptions;

namespace Core.Storage;

public sealed class FileBookStore : IBookStore
{
    private readonly string _path;
    private readonly Dictionary<string, BookCopy> _cache = new();
    private bool _loaded;

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private sealed record StoredBookDto(string Id, string Isbn, bool IsIssued);

    public FileBookStore(string path)
    {
        _path = path ?? throw new ArgumentNullException(nameof(path));
    }

    private void EnsureLoaded()
    {
        if (_loaded) return;
        if (!File.Exists(_path)) { _loaded = true; return; }
        try
        {
            var json = File.ReadAllText(_path);
            if (string.IsNullOrWhiteSpace(json)) { _loaded = true; return; }
            var dtos = JsonSerializer.Deserialize<List<StoredBookDto>>(json, Options) ?? [];
            _cache.Clear();
            foreach (var dto in dtos)
            {
                var book = BookCopy.Create(dto.Id, dto.Isbn);
                book.IsIssued = dto.IsIssued;
                _cache[book.Id] = book;
            }
            _loaded = true;
        }
        catch (JsonException ex)
        {
            throw new DomainException($"Пошкоджений файл сховища {_path}: {ex.Message}", ex);
        }
    }

    private void Flush()
    {
        var dtos = _cache.Values.Select(b => new StoredBookDto(b.Id, b.Isbn, b.IsIssued)).ToList();
        var dir = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
        var tmp = _path + ".tmp";
        File.WriteAllText(tmp, JsonSerializer.Serialize(dtos, Options));
        File.Move(tmp, _path, overwrite: true);
    }

    public void Add(BookCopy book) { EnsureLoaded(); _cache[book.Id] = book; Flush(); }
    public BookCopy? GetById(string id) { EnsureLoaded(); return _cache.TryGetValue(id, out var b) ? b : null; }
    public IEnumerable<BookCopy> GetAll() { EnsureLoaded(); return _cache.Values.ToList(); }
    public void Update(BookCopy book) { EnsureLoaded(); _cache[book.Id] = book; Flush(); }
}
