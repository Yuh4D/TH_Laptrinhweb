using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NDHuy_W345_S2.Extensions;
using NDHuy_W345_S2.Models;
using NDHuy_W345_S2.Repositories;

namespace NDHuy_W345_S2.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ShoppingCartController(
            IProductRepository productRepository,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _productRepository = productRepository;
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.GetCurrentCart();
            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy sản phẩm.";
                return RedirectToAction("Index", "Product");
            }

            var cartItem = new CartItem
            {
                ProductId = product.Id,
                Name = product.Name,
                Price = product.Price,
                Quantity = quantity
            };

            var cart = HttpContext.GetCurrentCart();
            cart.AddItem(cartItem);
            HttpContext.SaveCurrentCart(cart);

            TempData["Success"] = $"Sản phẩm \"{product.Name}\" đã vào giỏ hàng.";
            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.GetCurrentCart();

            if (cart.Items.Any())
            {
                cart.RemoveItem(productId);
                HttpContext.SaveCurrentCart(cart);
                TempData["Success"] = "Đã xoá sản phẩm khỏi giỏ hàng.";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = HttpContext.GetCurrentCart();

            if (cart.Items.Any())
            {
                cart.UpdateQuantity(productId, quantity);
                HttpContext.SaveCurrentCart(cart);
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult Checkout()
        {
            var cart = HttpContext.GetCurrentCart();

            if (!cart.Items.Any())
            {
                TempData["Error"] = "Giỏ hàng trống, vui lòng thêm vào giỏ hàng trước khi thanh toán.";
                return RedirectToAction("Index");
            }

            return View(new Order());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart = HttpContext.GetCurrentCart();

            if (!cart.Items.Any())
            {
                TempData["Error"] = "Giỏ hàng trống, vui lòng thêm vào giỏ hàng trước khi thanh toán.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng nhập thông tin giao hàng.";
                return View(order);
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            try
            {
                order.UserId = user.Id;
                order.OrderDate = DateTime.UtcNow;
                order.TotalPrice = cart.Items.Sum(i => i.Price * i.Quantity);
                order.OrderDetails = cart.Items.Select(i => new OrderDetail
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList();

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                HttpContext.RemoveCurrentCart();

                return View("OrderCompleted", order.Id);
            }
            catch (Exception)
            {
                TempData["Error"] = "Có lỗi xảy ra khi đặt hàng, vui lòng thao tác lại sau.";
                return View(order);
            }
        }
    }
}
