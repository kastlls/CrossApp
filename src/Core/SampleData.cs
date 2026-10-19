using System.Collections.Generic;
using Core.Domain;

namespace Core;

public static class SampleData
{
    public static IEnumerable<BookCopy> Books() =>
    [
        BookCopy.Create("B-001", "978-0132350884"),
        BookCopy.Create("B-002", "978-0201633610"),
        BookCopy.Create("B-003", "978-0134494166"),
        BookCopy.Create("B-004", "978-0321125217"),
        BookCopy.Create("B-005", "978-0135974445"),
        BookCopy.Create("B-006", "978-0201485677"),
        BookCopy.Create("B-007", "978-0321127426"),
        BookCopy.Create("B-008", "978-0137081073"),
        BookCopy.Create("B-009", "978-0596007126"),
        BookCopy.Create("B-010", "978-1491950357"),
        BookCopy.Create("B-011", "978-1617294532"),
        BookCopy.Create("B-012", "978-0134685991"),
        BookCopy.Create("B-013", "978-0201616224"),
        BookCopy.Create("B-014", "978-0735619678"),
        BookCopy.Create("B-015", "978-1449373320")
    ];
}
