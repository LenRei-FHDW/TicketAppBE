using Stripe;
using Stripe.Checkout;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Stripe;

namespace TicketAPI.Services.Scoped;

public interface IPaymentService
{
    Task<Session> CreateCheckoutSession(OrderDTO orderDTO, string userEmail);
    Task<ServiceResponse<bool>> CompletOrder(HttpRequest request);
}

public class PaymentService : IPaymentService
{
    const string secret = "hgdfgj";
    
    public PaymentService(IConfiguration configuration)
    {
        StripeConfiguration.ApiKey = configuration.GetValue<string>("StripeConfiguration:ApiKeyS");
    }
    
    public async Task<Session> CreateCheckoutSession(OrderDTO order, string userEmail)
    {

        var lineItems = new List<SessionLineItemOptions>();

        foreach (var orderItem in order.OrderItems)
        {
            var item = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = orderItem.SinglePrice * 100,
                    Currency = "eur",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = orderItem.Name,
                        Images = new List<string> { "https://www1.wdr.de/nachrichten/deutschland-ticket-104~_v-HintergrundL.jpg" }
                    }
                },
                Quantity = orderItem.Quantity
            };
            
            lineItems.Add(item);
        }

        
        var options = new SessionCreateOptions
        {
            CustomerEmail = userEmail,
            ExtraParams = new Dictionary<string, object>
            {
                { "metadata[orderId]", "14aa57f8-6a38-4d91-a94a-dee2c445ee3f" }
            },
            ShippingAddressCollection = new SessionShippingAddressCollectionOptions
            {
                AllowedCountries = new List<string> { "DE" }
            },
            PaymentMethodTypes = new List<string>
            {
                "card",
            },
            Currency = "eur",
            LineItems = lineItems,
            Mode = "payment",
            SuccessUrl = "https://localhost:7145/api/Order/CompletOrder/14aa57f8-6a38-4d91-a94a-dee2c445ee3f",
            CancelUrl = "https://pfax423.store",
        };
        
        var service = new SessionService();
        Session session = service.Create(options);
        return session;
    }

    public async Task<ServiceResponse<bool>> CompletOrder(HttpRequest request)
    {
        var json = await new StreamReader(request.Body).ReadToEndAsync();
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(json, request.Headers["Stripe-Signature"], secret);

            if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted) {
                var session = stripeEvent.Data.Object as Session;
                //var user = await _authService.GetUserByEmail(session.CustomerEmail);
                //await _orderService.PlaceOrder(user.Id);
            }

            return new ServiceResponse<bool> { Data = true };
        }
        catch (StripeException e)
        {
            return new ServiceResponse<bool> { Data = false, Success = false, Message = e.Message };
        }
    }
}