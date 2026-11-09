using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Core.Abstractions;
using Core.Domain;
using Core.Exceptions;

namespace Core.Storage;

public record BookDto(string Id, string Isbn);

public sealed class JsonBookImporter : IBookImporter
{
    public async Task<IReadOnlyList<BookCopy>> LoadAsync(string path, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested(); 
        
        await using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
        List<BookDto>? dtos;
        try {
            dtos = await JsonSerializer.DeserializeAsync<List<BookDto>>(stream, cancellationToken: ct)
                ?? throw new DomainException($"Файл {Path.GetFileName(path)} не містить списку.");
        }
        catch (JsonException ex) {
            throw new DomainException("Помилка читання JSON", ex);
        }

        var result = new List<BookCopy>(dtos.Count);
        for (var i = 0; i < dtos.Count; i++)
        {
            ct.ThrowIfCancellationRequested();
            
            // ШТУЧНА ЗАТРИМКА (2 сек) ДЛЯ ДЕМОНСТРАЦІЇ СКАСУВАННЯ
            await Task.Delay(2000, ct);
            
            try {
                result.Add(BookCopy.Create(dtos[i].Id, dtos[i].Isbn));
            }
            catch (ArgumentException ex) {
                throw new DomainException($"Запис {i + 1}: {ex.Message}", ex);
            }
        }
        return result;
    }
}
