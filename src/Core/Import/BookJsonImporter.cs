using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Core.Dto;

namespace Core.Import;

public static class BookJsonImporter
{
    public static ImportResult<object> Load(string path)
    {
        var items = new List<object>();
        var errors = new List<string>();

        try
        {
            string jsonString = File.ReadAllText(path);
            using JsonDocument doc = JsonDocument.Parse(jsonString);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            int index = 1;
            foreach (JsonElement element in doc.RootElement.EnumerateArray())
            {
                if (element.TryGetProperty("Type", out JsonElement typeProp))
                {
                    string type = typeProp.GetString() ?? "";
                    try
                    {
                        
                        object? parsedItem = type switch
                        {
                            "B" => element.Deserialize<BookDto>(options),
                            "R" => element.Deserialize<ReaderDto>(options),
                            _ => throw new FormatException($"невідомий префікс '{type}'")
                        };
                        
                        if (parsedItem != null) items.Add(parsedItem);
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"об'єкт {index}: {ex.Message}");
                    }
                }
                else
                {
                    errors.Add($"об'єкт {index}: відсутня властивість 'Type'");
                }
                index++;
            }
            return new ImportResult<object>(items, errors);
        }
        catch (Exception ex)
        {
            return new ImportResult<object>(new List<object>(), new List<string> { $"Помилка читання JSON: {ex.Message}" });
        }
    }
}
