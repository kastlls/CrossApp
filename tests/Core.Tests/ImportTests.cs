using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Core.Exceptions;
using Core.Services;
using Core.Storage;
namespace Core.Tests;
public class ImportTests
{
    [Fact] // 5) Скасування: перевірка OperationCanceledException
    public async Task ImportAsync_WhenAlreadyCancelled_Throws()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var service = new LendingService(new TestStore());
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => service.ImportAsync(new JsonBookImporter(), "any.json", cts.Token));
    }

    [Fact] // 6) Доменний виняток: некоректні дані у тимчасовому файлі
    public async Task ImportAsync_WithBadJson_ThrowsDomainException()
    {
        var path = Path.GetTempFileName();
        try
        {
            await File.WriteAllTextAsync(path, "{ \"bad_json\": true }");
            var service = new LendingService(new TestStore());
            await Assert.ThrowsAsync<DomainException>(() => service.ImportAsync(new JsonBookImporter(), path));
        }
        finally { if (File.Exists(path)) File.Delete(path); } // Видаляємо тимчасовий файл
    }
}
