using System.Linq;
using Xunit;
using Core.Reports;
namespace Core.Tests;
public class ReportServiceTests
{
    [Fact] // 3) Звіт: перевірка правильності підрахунку
    public void Totals_CountsCorrectly()
    {
        var items = TestData.Items(); // Тестовий набір має 3 книги, 1 з них видана
        var totals = ReportService.Totals(items);
        Assert.Equal(1, totals.Issued);
    }

    [Fact] // 4) Звіт: детермінованість топ-N
    public void TopBooks_IsDeterministic() => 
        Assert.Equal(2, ReportService.TopBooks(TestData.Items(), 2).Count);
}
