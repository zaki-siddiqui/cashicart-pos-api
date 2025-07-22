using Cashicart.Common.DTOs;

namespace Cashicart.Common
{
    public static class SalesReportExtensions
    {
        public static List<SalesReportDto> TopProducts(this List<SalesReportDto> reports, int count)
        {
            return reports
                .GroupBy(r => r.TransactionId) // Assuming unique transactions
                .OrderByDescending(g => g.Sum(r => r.TotalAmount))
                .Take(count)
                .SelectMany(g => g)
                .ToList();
        }

        public static int TransactionCount(this List<SalesReportDto> reports)
        {
            return reports.DistinctBy(r => r.TransactionId).Count();
        }
    }
}
