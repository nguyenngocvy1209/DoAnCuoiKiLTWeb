// File: Documentt/Areas/Admin/Models/Users/UserViewModel.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity; // Cần cho IdentityUser

namespace Documentt.Models
{
    // ViewModel này chỉ chứa các thuộc tính có sẵn trong IdentityUser
    public class UserViewModel
    {
        public string Id { get; set; } = string.Empty; // ID người dùng
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; } = string.Empty;
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        [Display(Name = "Email đã xác nhận")]
        public bool EmailConfirmed { get; set; } // Lấy từ IdentityUser
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; } // Lấy từ IdentityUser
        [Display(Name = "Số điện thoại đã xác nhận")]
        public bool PhoneNumberConfirmed { get; set; } // Lấy từ IdentityUser

        [Display(Name = "Vai trò")]
        public List<string> Roles { get; set; } = new List<string>(); // Danh sách các vai trò của người dùng
    }
}