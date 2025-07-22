
namespace Cashicart.Infrastructure.Services
{
    public interface IPaymentService
    {
        Task<string> ProcessPayment(decimal amount, string paymentMethodId);
        Task RefundPayment(Guid transactionId, decimal amount);
    }
}
