// File: Documentt/Models/Contact.cs
using System.ComponentModel.DataAnnotations;

namespace Documentt.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nội dung tin nhắn là bắt buộc.")]
        [StringLength(1000, ErrorMessage = "Tin nhắn không được vượt quá 1000 ký tự.")]
        public string Message { get; set; } = string.Empty;

        public DateTime SentDate { get; set; } = DateTime.Now; // Thời gian gửi
    }
}