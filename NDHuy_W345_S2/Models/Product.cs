using System.ComponentModel.DataAnnotations;

namespace NDHuy_W345_S2.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Tên sản phẩm không được để trống"), StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage ="Giá sản phẩm không được để trống")]
        [Range(0.01, 10000.00, ErrorMessage ="Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage ="Mô tả sản phẩm không được để trống")]
        public string Description { get; set; }
        public string? ImageUrl { get; set; }
        public List<ProductImage>? Images { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
