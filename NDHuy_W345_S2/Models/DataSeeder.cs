using Microsoft.EntityFrameworkCore;

namespace NDHuy_W345_S2.Models
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Kiểm tra dữ liệu đã tồn tại chưa - nếu có rồi thì không seed nữa
            if (await context.Categories.AnyAsync() || await context.Products.AnyAsync())
            {
                return;
            }

            // 5 danh mục sản phẩm công nghệ
            var categories = new List<Category>
            {
                new Category { Name = "Điện thoại" },
                new Category { Name = "Laptop" },
                new Category { Name = "Máy tính bảng" },
                new Category { Name = "Tai nghe" },
                new Category { Name = "Phụ kiện" }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            // 2 sản phẩm cho mỗi danh mục
            // ImageUrl = ảnh đại diện, Images = danh sách ảnh gallery
            // Mỗi sản phẩm có thư mục ảnh riêng trong wwwroot/images/
            var products = new List<Product>
            {
                // ====== Điện thoại ======
                new Product
                {
                    Name = "iPhone 15 Pro Max",
                    Price = 34990000,
                    Description = "iPhone 15 Pro Max với chip A17 Pro, màn hình 6.7 inch Super Retina XDR, camera 48MP, khung titan bền bỉ, cổng USB-C.",
                    ImageUrl = "/images/iphone-15-pro-max/1.jpg",
                    CategoryId = categories[0].Id
                },
                new Product
                {
                    Name = "Samsung Galaxy S24 Ultra",
                    Price = 29990000,
                    Description = "Samsung Galaxy S24 Ultra với Galaxy AI, bút S Pen, camera 200MP, màn hình Dynamic AMOLED 2X 6.8 inch, khung titan.",
                    ImageUrl = "/images/samsung-galaxy-s24-ultra/1.jpg",
                    CategoryId = categories[0].Id
                },

                // ====== Laptop ======
                new Product
                {
                    Name = "MacBook Pro 14 M3",
                    Price = 49990000,
                    Description = "MacBook Pro 14 inch với chip M3 Pro, RAM 18GB, SSD 512GB, màn hình Liquid Retina XDR, pin 17 giờ.",
                    ImageUrl = "/images/macbook-pro-14-m3/1.jpg",
                    CategoryId = categories[1].Id
                },
                new Product
                {
                    Name = "Dell XPS 15",
                    Price = 35990000,
                    Description = "Dell XPS 15 với Intel Core i7-13700H, RAM 16GB, SSD 512GB, màn hình OLED 3.5K 15.6 inch, card đồ họa Intel Arc.",
                    ImageUrl = "/images/dell-xps-15/1.jpg",
                    CategoryId = categories[1].Id
                },

                // ====== Máy tính bảng ======
                new Product
                {
                    Name = "iPad Air M2",
                    Price = 16990000,
                    Description = "iPad Air M2 với chip M2, màn hình Liquid Retina 11 inch, hỗ trợ Apple Pencil Pro và Magic Keyboard.",
                    ImageUrl = "/images/ipad-air-m2/1.jpg",
                    CategoryId = categories[2].Id
                },
                new Product
                {
                    Name = "Samsung Galaxy Tab S9",
                    Price = 14990000,
                    Description = "Samsung Galaxy Tab S9 với chip Snapdragon 8 Gen 2, màn hình Dynamic AMOLED 2X 11 inch, chống nước IP68, bút S Pen.",
                    ImageUrl = "/images/samsung-galaxy-tab-s9/1.jpg",
                    CategoryId = categories[2].Id
                },

                // ====== Tai nghe ======
                new Product
                {
                    Name = "AirPods Pro 2",
                    Price = 6990000,
                    Description = "AirPods Pro 2 với chip H2, chống ồn chủ động thích ứng, âm thanh không gian cá nhân hóa, hộp sạc MagSafe USB-C.",
                    ImageUrl = "/images/airpods-pro-2/1.jpg",
                    CategoryId = categories[3].Id
                },
                new Product
                {
                    Name = "Sony WH-1000XM5",
                    Price = 7990000,
                    Description = "Tai nghe chụp tai Sony WH-1000XM5 chống ồn chủ động hàng đầu, âm thanh Hi-Res Audio, Bluetooth 5.3, pin 30 giờ.",
                    ImageUrl = "/images/sony-wh-1000xm5/1.jpg",
                    CategoryId = categories[3].Id
                },

                // ====== Phụ kiện ======
                new Product
                {
                    Name = "Sạc nhanh Anker GaNPrime 65W",
                    Price = 890000,
                    Description = "Củ sạc Anker GaNPrime 65W, sạc nhanh cho laptop, điện thoại, máy tính bảng với công nghệ GaN tiên tiến, nhỏ gọn.",
                    ImageUrl = "/images/anker-ganprime-65w/1.jpg",
                    CategoryId = categories[4].Id
                },
                new Product
                {
                    Name = "Chuột Logitech MX Master 3S",
                    Price = 2490000,
                    Description = "Chuột không dây Logitech MX Master 3S với con lăn từ tính MagSpeed, cảm biến 8K DPI, kết nối Bluetooth và USB-C, pin 70 ngày.",
                    ImageUrl = "/images/logitech-mx-master-3s/1.jpg",
                    CategoryId = categories[4].Id
                }
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();

            // Ảnh gallery cho từng sản phẩm (mỗi SP 3 ảnh trong thư mục riêng)
            var productImages = new List<ProductImage>();

            // Hàm helper tạo danh sách ảnh cho 1 sản phẩm
            void AddImages(int productIndex, string folder)
            {
                for (int i = 1; i <= 3; i++)
                {
                    productImages.Add(new ProductImage
                    {
                        Url = $"/images/{folder}/{i}.jpg",
                        ProductId = products[productIndex].Id
                    });
                }
            }

            AddImages(0,  "iphone-15-pro-max");
            AddImages(1,  "samsung-galaxy-s24-ultra");
            AddImages(2,  "macbook-pro-14-m3");
            AddImages(3,  "dell-xps-15");
            AddImages(4,  "ipad-air-m2");
            AddImages(5,  "samsung-galaxy-tab-s9");
            AddImages(6,  "airpods-pro-2");
            AddImages(7,  "sony-wh-1000xm5");
            AddImages(8,  "anker-ganprime-65w");
            AddImages(9,  "logitech-mx-master-3s");

            await context.ProductImages.AddRangeAsync(productImages);
            await context.SaveChangesAsync();
        }
    }
}
