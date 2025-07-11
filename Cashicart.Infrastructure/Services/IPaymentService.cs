using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashicart.Infrastructure.Services
{
    public interface IPaymentService
    {
        Task<string> ProcessPayment(decimal amount, string paymentMethodId);
        Task RefundPayment(Guid transactionId, decimal amount);
    }
}
