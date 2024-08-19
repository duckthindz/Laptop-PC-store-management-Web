using Microsoft.AspNetCore.Mvc;
using Shopping_Tutorial.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping_Tutorial.Models
{
    public class ProductModel 
    {
        [Key]
        public int Id { get; set; }
		[Required(ErrorMessage = "yêu cầu nhập tên Sản Phẩm")]
        [MinLength(4,ErrorMessage = "yêu cầu nhập tên Sản Phẩm số lượng ký tư >= 4")]
		public string Name { get; set; }
        public string Slug { get; set; }
        [Required(ErrorMessage = "yêu cầu nhập Giá Sản Phẩm")]
        [Range(0.01,double.MaxValue)]
        [Column(TypeName ="decimal(8,2)")]
        public decimal Price { get; set; }
		[Required(ErrorMessage = "yêu cầu nhập Mô tả Sản Phẩm")]
        [MinLength(4, ErrorMessage = "yêu cầu nhập mô tả Sản Phẩm số lượng ký tư >= 4")]
        public string Description { get; set; }

        [Required, Range(1,int.MaxValue,ErrorMessage = "Chọn một thương hiệu")]
        public int BrandId { get; set; }
        [Required, Range(1, int.MaxValue, ErrorMessage = "Chọn một danh mục")]
        public int CategoryId {  get; set; }

        public CategoryModel Category { get; set; }
        public BrandModel Brand { get; set; }
        public string Images { get; set; }
        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }
	}
}
