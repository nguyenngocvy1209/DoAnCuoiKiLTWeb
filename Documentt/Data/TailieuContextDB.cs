using Documentt.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Documentt.Data
{
    public class TailieuContextDB : IdentityDbContext<ApplicationUser>
    {
        public TailieuContextDB(DbContextOptions<TailieuContextDB> options)
       : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Authour> Authours { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ShopingcartItem> ShoppingCartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Authour)
                .WithMany(t => t.Products)
                .HasForeignKey(p => p.AuthourId);

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    CategoryId = 1,
                    CategoryName = "Kinh Tế"
                }, new Category
                {
                    CategoryId = 2,
                    CategoryName = "Lập trình"
                },
new Category
{
    CategoryId = 3,
    CategoryName = "Toán học"
},
new Category
{
    CategoryId = 4,
    CategoryName = "Kỹ thuật"
},
new Category
{
    CategoryId = 5,
    CategoryName = "Khoa học dữ liệu"
},
new Category
{
    CategoryId = 6,
    CategoryName = "Vật lý"
},
new Category
{
    CategoryId = 7,
    CategoryName = "Thiết kế"
},
new Category
{
    CategoryId = 8,
    CategoryName = "Hóa học"
},
new Category
{
    CategoryId = 9,
    CategoryName = "Ngôn ngữ học"
},
new Category
{
    CategoryId = 10,
    CategoryName = "Quản trị kinh doanh"
}
                );
            modelBuilder.Entity<Authour>().HasData
                (
                new Authour
                {
                    AuthourId = 1,
                    AuthourName = "Nguyễn Ngọc Vỹ",
                    Detail = "Chuyên gia kinh tế với 10 năm kinh nghiệm giảng dạy.",
                    Img = "https://i.pinimg.com/564x/a8/5a/75/a85a7549a2926f1698af2709f40aafed.jpg",
                    noibat = true
                },
                new Authour
                {
                    AuthourId = 2,
                    AuthourName = "Anh Ngọc Vỹ",
                    Detail = "Giảng viên lập trình, tác giả nhiều tài liệu về JavaScript..",
                    Img = "https://minhtuanmobile.com/uploads/blog/bo-hinh-nen-meme-meo-duong-tang-thinh-ca-cuc-cute-dang-them-hinh-240216120205.jpg",
                    noibat = true
                },
                new Authour
                {
                    AuthourId = 3,
                    AuthourName = "Vỹ DZ",
                    Detail = "Tiến sĩ Toán học, chuyên cung cấp tài liệu Toán cao cấp.",
                    Img = "https://2vet.vn/wp-content/uploads/2024/07/meme-cho-meo-cute-3-03-11-27-39.jpg",
                    noibat = true
                },
                new Authour
                {
                    AuthourId = 4,
                    AuthourName = "Soái Ca Cam Ranh",
                    Detail = "Kỹ sư cơ khí, tác giả tài liệu kỹ thuật nổi tiếng.",
                    Img = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcROwlC-Q9wKSBkgT8fDECsIrJj3mFjfsVOpcQ&s",
                    noibat = true
                }
                );
            modelBuilder.Entity<Product>().HasData
                (
                new Product
                {
                    ProductId = 1,
                    Name = "Tài Liệu Kinh Tế V1",
                    CategoryId = 1,
                    img = "https://storage.googleapis.com/onthisinhvien.appspot.com/images/237670237-1706179100848-snapedit_1706179072311.png",
                    Description = "Tài liệu Kinh tế cơ bản.",
                    link = "https://drive.google.com/file/d/1lQGn8qZV2esjBOc0-UpR6_iGUivWr5Jk/view",
                    AuthourId=1,
                    Price = 50000,
                    New = true,
                    Trending = true,
                },
                new Product
                {
                    ProductId = 2,
                    Name = "Giáo trình Lập trình Python cơ bản",
                    CategoryId = 2, // Lập trình
                    img = "https://storage.googleapis.com/onthisinhvien.appspot.com/images/default_python_book.png", // Placeholder image
                    Description = "Tài liệu hướng dẫn lập trình Python từ cơ bản đến nâng cao cho người mới bắt đầu.",
                    link = "https://drive.google.com/file/d/example_python_link/view", // Placeholder link
                    AuthourId = 2,
                    Price = 75000,
                    New = true,
                    Trending = false,
                },
new Product
{
    ProductId = 3,
    Name = "Tuyển tập đề thi Toán cao cấp A1",
    CategoryId = 3, // Toán học
    img = "https://storage.googleapis.com/onthisinhvien.appspot.com/images/default_math_book.png", // Placeholder image
    Description = "Bộ sưu tập các đề thi và lời giải chi tiết môn Toán cao cấp A1.",
    link = "https://drive.google.com/file/d/example_math_link/view", // Placeholder link
    AuthourId = 3,
    Price = 60000,
    New = false,
    Trending = true,
},
new Product
{
    ProductId = 4,
    Name = "Sổ tay Kỹ thuật Điện tử căn bản",
    CategoryId = 4, // Kỹ thuật
    img = "https://storage.googleapis.com/onthisinhvien.appspot.com/images/default_engineering_book.png", // Placeholder image
    Description = "Tổng hợp kiến thức cơ bản và công thức thiết yếu trong kỹ thuật điện tử.",
    link = "https://drive.google.com/file/d/example_electronics_link/view", // Placeholder link
    AuthourId = 4,
    Price = 45000,
    New = true,
    Trending = false,
},
new Product
{
    ProductId = 5,
    Name = "Nhập môn Khoa học dữ liệu với R",
    CategoryId = 5, // Khoa học dữ liệu
    img = "https://storage.googleapis.com/onthisinhvien.appspot.com/images/default_data_science_book.png", // Placeholder image
    Description = "Tài liệu giới thiệu về Khoa học dữ liệu và cách sử dụng ngôn ngữ R.",
    link = "https://drive.google.com/file/d/example_data_science_link/view", // Placeholder link
    AuthourId = 1,
    Price = 90000,
    New = true,
    Trending = true,
},
new Product
{
    ProductId = 6,
    Name = "Bài tập Vật lý Đại cương A",
    CategoryId = 6, // Vật lý
    img = "https://storage.googleapis.com/onthisinhvien.appspot.com/images/default_physics_book.png", // Placeholder image
    Description = "Tập hợp các bài tập Vật lý Đại cương kèm lời giải chi tiết.",
    link = "https://drive.google.com/file/d/example_physics_link/view", // Placeholder link
    AuthourId = 1,
    Price = 55000,
    New = false,
    Trending = false,
}
                );
        }
    }
}
