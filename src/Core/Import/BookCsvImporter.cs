using System.Globalization;
using Core.Dto;

namespace Core.Import;

public static class BookCsvImporter
{
    private const char Separator = ';';

    // Повертаємо object, бо тепер у нас два різні типи: BookDto і ReaderDto
    public static ImportResult<object> Load(string path)
    {
        var items = new List<object>();
        var errors = new List<string>();
        string[] lines = File.ReadAllLines(path);

        for (int i = 0; i < lines.Length; i++)
        {
            int num = i + 1;
            string line = lines[i];

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#')) continue;
            if (num == 1 && line.StartsWith("type", StringComparison.OrdinalIgnoreCase)) continue;

            switch (ParseLine(line))
            {
                case ParseOk ok: items.Add(ok.Value); break;
                case ParseFailed failed: errors.Add($"рядок {num}: {failed.Reason}"); break;
            }
        }
        return new ImportResult<object>(items, errors);
    }

    private static ParseOutcome ParseLine(string line)
    {
        string[] parts = line.Split(Separator, StringSplitOptions.TrimEntries);
        return parts switch
        {
            // --- ПАТЕРНИ ДЛЯ КНИГ (префікс "B") ---
            ["B", _, "", _, ..] or ["B", _, _, "", ..] 
                => new ParseFailed("ISBN або назва порожні"),
            ["B", _, _, _, var year, ..] when !int.TryParse(year, NumberStyles.Integer, CultureInfo.InvariantCulture, out int y) || y < 1450 || y > DateTime.Now.Year
                => new ParseFailed($"рік '{year}' поза межами"),
            ["B", var id, var isbn, var title, var year]
                => new ParseOk(new BookDto(id, isbn, title, int.Parse(year, CultureInfo.InvariantCulture))),
            ["B", var id, var isbn, var title, var year, var author]
                => new ParseOk(new BookDto(id, isbn, title, int.Parse(year, CultureInfo.InvariantCulture), string.IsNullOrWhiteSpace(author) ? null : author)),

            // --- ПАТЕРНИ ДЛЯ ЧИТАЧІВ (префікс "R") ---
            ["R", var id, var name, var phone] 
                => new ParseOk(new ReaderDto(id, name, phone)),
            ["R", var id, var name, var phone, var email] 
                => new ParseOk(new ReaderDto(id, name, phone, string.IsNullOrWhiteSpace(email) ? null : email)),

            // --- ПОМИЛКИ ПРЕФІКСІВ ---
            [var prefix, ..] when prefix != "B" && prefix != "R" 
                => new ParseFailed($"невідомий префікс '{prefix}'"),
            
            _ => new ParseFailed($"неправильний формат рядка: {parts.Length} колонок")
        };
    }

    private abstract record ParseOutcome;
    private sealed record ParseOk(object Value) : ParseOutcome;
    private sealed record ParseFailed(string Reason) : ParseOutcome;
}
