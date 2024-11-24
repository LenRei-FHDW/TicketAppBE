using AutoMapper;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using TicketAPI.Data.Models;
using TicketAPI.Data.Repositories;
using TicketAPI.Services.DTO;
using TicketAPI.Services.Stripe;

namespace TicketAPI.Services.Scoped;

public interface IPaymentService
{
    Task<Session> CreateCheckoutSession(OrderDTO orderDTO, string userEmail);
    Task<ServiceResponse<bool>> CompletOrder(HttpRequest request);
    Task<Guid> CanceledOrder(Guid orderId);
}

public class PaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PaymentService> _logger;
    private readonly OrderService _orderService;
    private readonly IRepository<TicketAPI.Data.Models.Order, Guid> _orderRepository;
    private IMapper _mapper;
    
    public PaymentService(IConfiguration configuration, ILogger<PaymentService> logger, OrderService orderService, IRepository<TicketAPI.Data.Models.Order, Guid> orderRepository, IMapper mapper)
    {
        StripeConfiguration.ApiKey = configuration.GetValue<string>("StripeConfiguration:ApiKeyS");
        _configuration = configuration;
        _logger = logger;
        _orderService = orderService;
        _orderRepository = orderRepository;
        _mapper = mapper;
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
                { "metadata[orderId]", order.OrderId }
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
            SuccessUrl = "https://localhost:7145/api/Order/CompletOrder",
            CancelUrl = $"https://localhost:7145/api/Order/CanceledOrder/{order.OrderId}",
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
            var stripeEvent = EventUtility.ConstructEvent(json, request.Headers["Stripe-Signature"], _configuration.GetValue<string>("StripeConfiguration:WebhookSecret"));

            if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted) {
                var session = stripeEvent.Data.Object as Session;
                
                var orderId = session.Metadata["orderId"];
                var stripeId = session.Id;

                var order = await _orderRepository.GetByIdAsync(new Guid(orderId));
                order.StripeId = stripeId;
                order.PlaymentStatus = PlaymentStatus.Success;
                
                await _orderRepository.UpdateAsync(order);
            }
            return new ServiceResponse<bool> { Data = true };
        }
        catch (StripeException e)
        {
            return new ServiceResponse<bool> { Data = false, Success = false, Message = e.Message };
        }
    }
    
    public async Task<Guid> CanceledOrder(Guid orderId)
    {
        var order = await _orderService.GetOrderById(orderId);
        var shoppingCartItems = new List<ShoppingCartItem>();
        foreach (var orderItem in order.OrderItems)
        {
            shoppingCartItems.Add(_mapper.Map<ShoppingCartItem>(orderItem));
        }
        
        var newOrder = await _orderService.CreateNewOrder(order.ApplicationUserId, shoppingCartItems);

        return newOrder.OrderId;
    }
}