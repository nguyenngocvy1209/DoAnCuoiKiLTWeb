// File: Documentt/Data/DbInitializer.cs (Tạo file này)
using Documentt.Models; // Nếu bạn dùng ApplicationUser custom
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Documentt.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndUsers(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>(); // Hoặc IdentityUser
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // 1. Tạo vai trò "Admin" nếu chưa tồn tại
            string adminRoleName = "Admin";
            if (await roleManager.FindByNameAsync(adminRoleName) == null)
            {
                await roleManager.CreateAsync(new IdentityRole(adminRoleName));
            }

            // 2. Tạo một người dùng Admin mặc định nếu chưa tồn tại
            string adminEmail = "admin@gmail.com"; // Email của admin
            string adminPassword = "Admin@123"; // Mật khẩu mạnh cho admin (thay đổi trong production!)

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser// Hoặc IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true // Giả định đã xác nhận
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    // 3. Gán vai trò "Admin" cho người dùng này
                    await userManager.AddToRoleAsync(adminUser, adminRoleName);
                }
            }

            // (Optional: Bạn có thể thêm các vai trò và người dùng khác nếu cần)
        }
    }
}