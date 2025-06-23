// File: Documentt/Models/Category.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Thêm dòng này

namespace Documentt.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Tên danh mục là bắt buộc.")] 
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự.")] 
        public string CategoryName { get; set; } = string.Empty; 

        public List<Product>? Products { get; set; } 
    }
}