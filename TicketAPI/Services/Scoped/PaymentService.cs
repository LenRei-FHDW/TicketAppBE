using AutoMapper;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using TicketAPI.Data.Models;
using TicketAPI.Data.Models.DTO;
using TicketAPI.Data.Repositories;
using TicketAPI.Services.DTO;
using Order = TicketAPI.Data.Models.Order;

namespace TicketAPI.Services.Scoped;

public interface IPaymentService
{
    Session CreateCheckoutSession(IEnumerable<ShoppingCartItem> shoppingCartItems,string userId, string userEmail);
    Task<ServiceResponseDTO<bool>> CompletOrder(HttpRequest request);
}

/// <summary>
/// This Service is for the Payment with Stripe
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<PaymentService> _logger;
    private readonly OrderService _orderService;
    private readonly IRepository<TicketAPI.Data.Models.Order, Guid> _orderRepository;
    private readonly IMapper _mapper;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IUserRepository _userRepository;
    
    public PaymentService(IConfiguration configuration, ILogger<PaymentService> logger, OrderService orderService, IRepository<TicketAPI.Data.Models.Order, Guid> orderRepository, IMapper mapper, IShoppingCartService shoppingCartService, IUserRepository userRepository)
    {
        StripeConfiguration.ApiKey = configuration.GetValue<string>("StripeConfiguration:ApiKeyS");
        _configuration = configuration;
        _logger = logger;
        _orderService = orderService;
        _orderRepository = orderRepository;
        _mapper = mapper;
        _shoppingCartService = shoppingCartService;
        _userRepository = userRepository;
    }
    
    /// <summary>
    /// Creates a Checkout Session for Stripe
    /// </summary>
    /// <param name="shoppingCartItems">ShopingCartItems for the order</param>
    /// <param name="userId">Id of the User</param>
    /// <param name="userEmail">Email of the user</param>
    /// <returns>Create Stripe Session</returns>
    public  Session CreateCheckoutSession(IEnumerable<ShoppingCartItem> shoppingCartItems, string userId, string userEmail)
    {
        var lineItems = new List<SessionLineItemOptions>();

        foreach (var cartItem in shoppingCartItems)
        {
           
            var item = new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = cartItem.Product.Price * 100,
                    Currency = "eur",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name =  cartItem.Product.Name,
                        Images = new List<string> { _configuration.GetValue<string>("BackEnd:BaseUrl") + _configuration.GetValue<string>("BackEnd:Images") + cartItem.Product.ImageName }
                    }
                },
                Quantity = cartItem.Quantity
            };
            
            lineItems.Add(item);
        }

        
        var options = new SessionCreateOptions
        {
            CustomerEmail = userEmail,
            ExtraParams = new Dictionary<string, object>
            {
                { "metadata[userId]", userId }
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
            SuccessUrl = _configuration.GetValue<string>("BackEnd:BaseUrl") + _configuration.GetValue<string>("BackEnd:CallbackSuccess"),
            CancelUrl = _configuration.GetValue<string>("BackEnd:BaseUrl") + _configuration.GetValue<string>("BackEnd:CallbackCanceled"),
        };
        
        var service = new SessionService();
        Session session = service.Create(options);
        return session;
    }

    /// <summary>
    /// Creates Order form User
    /// </summary>
    /// <param name="request">Data from Stripe</param>
    /// <returns>Status for Stripe</returns>
    public async Task<ServiceResponseDTO<bool>> CompletOrder(HttpRequest request)
    {
        var json = await new StreamReader(request.Body).ReadToEndAsync();
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(json, request.Headers["Stripe-Signature"], _configuration.GetValue<string>("StripeConfiguration:WebhookSecret"));

            if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted) {
                var session = stripeEvent.Data.Object as Session;

                var userId = session.Metadata["userId"];
                var user = await _userRepository.GetUserWithAddressAsync(userId);
                 if (user == null)
                     throw new NullReferenceException("User not found");
                
                 if (user.Addresse == null)
                 {
                     user.Addresse = new TicketAPI.Data.Models.Address
                     {
                         ApplicationUserId = userId,
                         StreetLine1 = session.ShippingDetails.Address.Line1,
                         StreetLine2 = session.ShippingDetails.Address.Line2,
                         City = session.ShippingDetails.Address.City,
                         State = session.ShippingDetails.Address.State,
                         Zip = session.ShippingDetails.Address.PostalCode
                     };
                 }
                 else
                 {
                     user.Addresse.StreetLine1 = session.ShippingDetails.Address.Line1;
                     user.Addresse.StreetLine2 = session.ShippingDetails.Address.Line2;
                     user.Addresse.City = session.ShippingDetails.Address.City;
                     user.Addresse.State = session.ShippingDetails.Address.State;
                     user.Addresse.Zip = session.ShippingDetails.Address.PostalCode;
                 }
                
                await _userRepository.UpdateUserAsync(user);
                
                var shoppingCartItems = await _shoppingCartService.GetModelItemsOfUser(user.Id);
                if (shoppingCartItems.Count() == 0)
                {
                    _logger.LogWarning("No Product in ShoppingCart");
                    throw new NullReferenceException("No Product in ShoppingCart");
                }

                var stripeId = session.Id;
                
                var createOrder = await _orderService.CreateNewOrder(user.Id, stripeId, shoppingCartItems);
                
                await _shoppingCartService.RemoveAllFromCart(user.Id);
            }
            return new ServiceResponseDTO<bool> { Data = true };
        }
        catch (StripeException e)
        {
            return new ServiceResponseDTO<bool> { Data = false, Success = false, Message = e.Message };
        }
    }
}