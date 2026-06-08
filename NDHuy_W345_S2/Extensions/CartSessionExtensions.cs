using NDHuy_W345_S2.Models;
using System.Security.Claims;

namespace NDHuy_W345_S2.Extensions
{
    public static class CartSessionExtensions
    {
        private const string GuestCartKey = "Cart_Guest";
        private const string UserCartPrefix = "Cart_";

        public static ShoppingCart GetCurrentCart(this HttpContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var cartKey = GetCartSessionKey(context.User);
            return context.Session.GetObjectFromJson<ShoppingCart>(cartKey) ?? new ShoppingCart();
        }

        public static void SaveCurrentCart(this HttpContext context, ShoppingCart cart)
        {
            ArgumentNullException.ThrowIfNull(context);

            var cartKey = GetCartSessionKey(context.User);
            context.Session.SetObjectAsJson(cartKey, cart ?? new ShoppingCart());
        }

        public static void RemoveCurrentCart(this HttpContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            var cartKey = GetCartSessionKey(context.User);
            context.Session.Remove(cartKey);
        }

        public static void MergeGuestCartIntoUserCart(this HttpContext context, string userId)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (string.IsNullOrWhiteSpace(userId))
            {
                return;
            }

            var guestCart = context.Session.GetObjectFromJson<ShoppingCart>(GuestCartKey);
            if (guestCart == null || !guestCart.Items.Any())
            {
                return;
            }

            var userCartKey = GetCartSessionKey(userId);
            var userCart = context.Session.GetObjectFromJson<ShoppingCart>(userCartKey) ?? new ShoppingCart();

            foreach (var item in guestCart.Items)
            {
                userCart.AddItem(new CartItem
                {
                    ProductId = item.ProductId,
                    Name = item.Name,
                    Price = item.Price,
                    Quantity = item.Quantity,
           
                });
            }

            context.Session.SetObjectAsJson(userCartKey, userCart);
            context.Session.Remove(GuestCartKey);
        }

        public static string GetCartSessionKey(this ClaimsPrincipal user)
        {
            ArgumentNullException.ThrowIfNull(user);

            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAuthenticated = user.Identity?.IsAuthenticated ?? false;

            return GetCartSessionKey(userId, isAuthenticated);
        }

        private static string GetCartSessionKey(string? userId, bool isAuthenticated)
        {
            if (!isAuthenticated || string.IsNullOrWhiteSpace(userId))
            {
                return GuestCartKey;
            }

            return GetCartSessionKey(userId);
        }

        private static string GetCartSessionKey(string userId)
        {
            return $"{UserCartPrefix}{userId}";
        }
    }
}
