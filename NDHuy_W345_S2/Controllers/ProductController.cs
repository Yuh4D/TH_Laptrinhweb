using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NDHuy_W345_S2.Models;
using NDHuy_W345_S2.Repositories;

namespace NDHuy_W345_S2.Controllers
{
    
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ApplicationDbContext _context;
        public ProductController(IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ApplicationDbContext context)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }
        public async Task<IActionResult> Add()
        {
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageUrl)
        {
            if (ModelState.IsValid)
            {
                if (imageUrl != null)
                {
                    // Lưu hình ảnh đại diện tham khảo bài 02 hàm SaveImage

                    product.ImageUrl = await SaveImage(imageUrl);

                }
                await _productRepository.AddAsync(product);
                return RedirectToAction(nameof(Index));
            }
            // Nếu ModelState không hợp lệ, hiển thị form với dữ liệu đã nhập
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        private async Task<string?> SaveImage(IFormFile imageUrl)
        {
            if (imageUrl == null || imageUrl.Length == 0)
            {
                return null;
            }

            // Tạo tên file duy nhất để tránh trùng lặp
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageUrl.FileName);

            // Đường dẫn thư mục lưu ảnh
            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "images");

            // Nếu thư mục chưa tồn tại thì tạo mới
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Đường dẫn đầy đủ của file
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Lưu file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageUrl.CopyToAsync(fileStream);
            }

            // Trả về đường dẫn tương đối để lưu DB
            return "/images/" + fileName;
        }

        public async Task<IActionResult> Display(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        // Hiển thị form cập nhật sản phẩm
        public async Task<IActionResult> Update(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name",
            product.CategoryId);
            return View(product);
        }

        // Xử lý cập nhật sản phẩm
        [HttpPost]
        public async Task<IActionResult> Update(int id, Product product,
        IFormFile imageUrl)
        {
            ModelState.Remove("ImageUrl"); // Loại bỏ xác thực ModelState cho ImageUrl
        if (id != product.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var existingProduct = await
                _productRepository.GetByIdAsync(id); // Giả định có phương thức GetByIdAsync
                                                     // Giữ nguyên thông tin hình ảnh nếu không có hình mới được tải lên
            if (imageUrl == null)
                {
                    product.ImageUrl = existingProduct.ImageUrl;
                }
                else
                {
                    // Lưu hình ảnh mới

                    product.ImageUrl = await SaveImage(imageUrl);

                }
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.ImageUrl = product.ImageUrl;
                await _productRepository.UpdateAsync(existingProduct);
                return RedirectToAction(nameof(Index));
            }
            var categories = await _categoryRepository.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            return View(product);
        }

        // Hiển thị form xác nhận xóa sản phẩm
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        // Xử lý xóa sản phẩm
        [HttpPost, ActionName("DeleteConfirmed")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // ================================================================
        // QUẢN LÝ ẢNH SẢN PHẨM
        // ================================================================

        // GET: Hiển thị trang thêm ảnh
        public async Task<IActionResult> AddImage(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Xử lý upload nhiều ảnh
        [HttpPost]
        public async Task<IActionResult> AddImage(int id, List<IFormFile> imageFiles)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            if (imageFiles == null || imageFiles.Count == 0 || imageFiles.All(f => f.Length == 0))
            {
                TempData["Error"] = "Vui lòng chọn ít nhất một file ảnh.";
                return RedirectToAction(nameof(AddImage), new { id });
            }

            // Tạo thư mục riêng cho sản phẩm (dùng slug từ tên)
            var folderName = Slugify(product.Name);
            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot", "images", folderName);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var newImages = new List<ProductImage>();

            foreach (var file in imageFiles)
            {
                if (file.Length == 0) continue;

                // Kiểm tra định dạng ảnh
                var ext = Path.GetExtension(file.FileName).ToLower();
                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".webp" && ext != ".gif")
                {
                    continue;
                }

                // Tạo tên file duy nhất
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Lưu file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Tạo bản ghi ProductImage
                newImages.Add(new ProductImage
                {
                    Url = $"/images/{folderName}/{fileName}",
                    ProductId = id
                });
            }

            if (newImages.Count > 0)
            {
                await _context.ProductImages.AddRangeAsync(newImages);
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = $"Đã thêm {newImages.Count} ảnh thành công.";
            return RedirectToAction(nameof(AddImage), new { id });
        }

        // POST: Xóa một ảnh trong gallery
        [HttpPost]
        public async Task<IActionResult> DeleteImage(int imageId, int productId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image != null)
            {
                // Xóa file ảnh trên disk
                var filePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    image.Url.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Xóa bản ghi trong DB
                _context.ProductImages.Remove(image);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Đã xóa ảnh.";
            }
            else
            {
                TempData["Error"] = "Không tìm thấy ảnh.";
            }

            return RedirectToAction(nameof(AddImage), new { id = productId });
        }

        // Helper: tạo slug từ tên sản phẩm (dùng làm tên thư mục)
        private string Slugify(string text)
        {
            if (string.IsNullOrEmpty(text)) return "product";

            return string.Join("-", text.ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(word => new string(word.Where(c => char.IsLetterOrDigit(c) || c == '-').ToArray()))
                .Where(word => !string.IsNullOrEmpty(word)));
        }
    }

}
