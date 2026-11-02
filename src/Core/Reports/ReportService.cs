using System;
using System.Collections.Generic;
using System.Linq;
using Core.Domain;

namespace Core.Reports;

public static class ReportService
{
    // 1) Фільтр + сортування + проєкція: доступні книги (не видані)
    public static IReadOnlyList<StockLine> AvailableBooks(IEnumerable<BookCopy> source) =>
        source.Where(b => !b.IsIssued)
              .OrderBy(b => b.Isbn, StringComparer.Ordinal)
              .ThenBy(b => b.Id, StringComparer.Ordinal)
              .Select(b => new StockLine(b.Id, b.Isbn, b.IsIssued))
              .ToList();

    // 2) Топ-N зі стабільним порядком: останні додані за ID (для стабільності сортування)
    public static IReadOnlyList<StockLine> TopBooks(IEnumerable<BookCopy> source, int n) => 
        source.OrderByDescending(b => b.Isbn, StringComparer.Ordinal)
              .ThenByDescending(b => b.Id, StringComparer.Ordinal)
              .Take(n)
              .Select(b => new StockLine(b.Id, b.Isbn, b.IsIssued))
              .ToList();

    // 3) Групування з підрахунком (за ISBN)
    public static IReadOnlyList<GroupSummary> ByIsbn(IEnumerable<BookCopy> source) =>
        source.GroupBy(b => b.Isbn, StringComparer.OrdinalIgnoreCase)
              .Select(g => new GroupSummary(g.Key, g.Count()))
              .OrderByDescending(s => s.Count)
              .ToList();

    // 4) Агрегати з обробкою порожньої послідовності (Totals)
    public static (int Items, int Issued, double IssuedPercentage) Totals(IEnumerable<BookCopy> source)
    {
        var list = source.ToList();
        if (list.Count == 0) return (0, 0, 0d);
        
        int issued = list.Count(b => b.IsIssued);
        double percentage = (double)issued / list.Count * 100;
        
        return (list.Count, issued, percentage);
    }
}
