﻿using System.ComponentModel.DataAnnotations;

namespace Shopping_Tutorial.Models
{
    public class CategoryModel
    {
		[Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "yêu cầu nhập tên Thương Hiệu")]
        public string Name { get; set; }
        [Required(ErrorMessage = "yêu cầu nhập tên Thương Hiệu")]
        public string Description { get; set; }
        public string Slug { get; set; }
        public int Status { get; set; }

    }
}
