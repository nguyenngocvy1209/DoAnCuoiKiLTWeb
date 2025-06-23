using Documentt.Data;
using Documentt.Models.Interface;
using Documentt.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; // Cần thiết cho IdentityUser và IdentityRole
using Documentt.Models; // <-- Thêm dòng này nếu bạn dùng ApplicationUser tùy chỉnh
using System; // Cần cho Exception, TimeSpan

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Cấu hình DbContext
builder.Services.AddDbContext<TailieuContextDB>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("TailieuContextDBConnection")));

// Cấu hình ASP.NET Core Identity
// Nếu bạn có một class ApplicationUser tùy chỉnh (như chúng ta đã nói), hãy thay thế IdentityUser bằng ApplicationUser.
// Nếu không, hãy giữ nguyên IdentityUser.
builder.Services.AddDefaultIdentity<ApplicationUser>(options => // <-- THAY ĐỔI TỪ IdentityUser SANG ApplicationUser NẾU BẠN CÓ CUSTOM USER
    options.SignIn.RequireConfirmedAccount = false) // Đặt false để không yêu cầu xác nhận email khi đăng nhập
    .AddRoles<IdentityRole>() // <-- THÊM DÒNG NÀY ĐỂ HỖ TRỢ ROLES
    .AddEntityFrameworkStores<TailieuContextDB>();

// Đăng ký HttpContextAccessor (Cần thiết cho ShoppingCartRepository)
builder.Services.AddHttpContextAccessor();

// Đăng ký các Repository của bạn
builder.Services.AddScoped<IHomeRepository, HomeRepository>();
builder.Services.AddScoped<IAuthourRepository, AuthourRepository>(); // Đảm bảo bạn đã định nghĩa IAuthourRepository và AuthourRepository
// Đăng ký ShoppingCartRepository với phương thức GetCart tĩnh
builder.Services.AddScoped<IShoppingCartRepository>(sp => ShoppingCartRepository.GetCart(sp));
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Thêm Session service
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian sống của session
    options.Cookie.HttpOnly = true; // Cookie chỉ truy cập qua HTTP
    options.Cookie.IsEssential = true; // Cookie này là cần thiết để ứng dụng hoạt động
});


var app = builder.Build();

// Đặt DbInitializer ở đây để tạo Admin Role và Admin User khi ứng dụng khởi chạy
// Sử dụng using scope để đảm bảo services được dispose đúng cách
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Có thể tự động apply migration ở đây nếu bạn muốn, nhưng hãy cẩn thận trong môi trường Production
        // var context = services.GetRequiredService<TailieuContextDB>();
        // context.Database.Migrate();

        await DbInitializer.SeedRolesAndUsers(services); // <-- GỌI HÀM KHỞI TẠO DỮ LIỆU ADMIN ROLE VÀ ADMIN USER
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting(); // <-- PHẢI CÓ DÒNG NÀY TRƯỚC UseAuthentication và UseAuthorization

app.UseSession(); // <-- UseSession PHẢI ĐẶT TRƯỚC UseAuthentication/Authorization nếu bạn dùng Session cho auth/cart

app.UseAuthentication(); // <-- XÁC THỰC NGƯỜI DÙNG (CÓ COOKIE KHÔNG?)
app.UseAuthorization();  // <-- KIỂM TRA QUYỀN TRUY CẬP (CÓ ĐƯỢC PHÉP TRUY CẬP KHÔNG?)

app.MapRazorPages(); // Quan trọng cho Identity Razor Pages (đăng nhập, đăng ký...)

// Cấu hình route cho Area "Admin" (PHẢI ĐẶT TRƯỚC default route)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}"); // <-- THÊM ROUTE CHO AREAS

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();