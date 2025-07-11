using Cashicart.Common.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;

namespace Cashicart.Infrastructure.Services;
public class PaymentService : IPaymentService
{
    private readonly ILogger<PaymentService> _logger;
    private readonly string _stripeApiKey;

    //public PaymentService(ILogger<PaymentService> logger)
    //{
    //    _logger = logger;
    //    StripeConfiguration.ApiKey = "sk_test_..."; // Configure via appsettings.json
    //}

    public PaymentService(ILogger<PaymentService> logger, IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _stripeApiKey = Environment.GetEnvironmentVariable("STRIPE_API_KEY") ?? configuration["Stripe:ApiKey"];
        if (string.IsNullOrEmpty(_stripeApiKey))
            throw new InfrastructureException("Stripe API key is missing.");
        StripeConfiguration.ApiKey = _stripeApiKey;
    }

    public async Task<string> ProcessPayment(decimal amount, string paymentMethodId)
    {
        try
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100),
                Currency = "usd",
                PaymentMethod = paymentMethodId,
                Confirm = true
            };
            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);
            _logger.LogInformation("Payment processed successfully: {PaymentIntentId}", paymentIntent.Id);
            return paymentIntent.Id;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Payment processing failed");
            throw new InfrastructureException("Payment processing failed", ex);
        }
    }

    public async Task RefundPayment(Guid transactionId, decimal amount)
    {
        try
        {
            var options = new RefundCreateOptions
            {
                PaymentIntent = transactionId.ToString(),
                Amount = (long)(amount * 100)
            };
            var service = new RefundService();
            await service.CreateAsync(options);
            _logger.LogInformation("Refund processed for transaction {TransactionId}", transactionId);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Refund processing failed for transaction {TransactionId}", transactionId);
            throw new InfrastructureException("Refund processing failed", ex);
        }
    }
}