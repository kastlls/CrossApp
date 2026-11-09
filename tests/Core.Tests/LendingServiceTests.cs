using System;
using Xunit;
using Core.Exceptions;
using Core.Services;
namespace Core.Tests;
public class LendingServiceTests
{
    [Fact] // 1) Інваріант: відмова і незмінний стан
    public void ReturnCopy_WhenNotIssued_ThrowsDomainException_AndKeepsState()
    {
        var store = new TestStore();
        var service = new LendingService(store);
        var b = service.AddBook("978-TEST");
        
        Assert.Throws<DomainException>(() => service.ReturnCopy(b.Id));
        Assert.False(store.GetById(b.Id)!.IsIssued);
    }

    [Fact] // 2) Подія: перевірка, що подія піднімається рівно один раз
    public void AddBook_RaisesChangedExactlyOnce()
    {
        var service = new LendingService(new TestStore());
        int events = 0;
        service.Changed += (_, _) => events++;
        service.AddBook("978-TEST2");
        Assert.Equal(1, events);
    }
}
