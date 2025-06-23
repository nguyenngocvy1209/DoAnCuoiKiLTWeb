using Microsoft.EntityFrameworkCore;
using Documentt.Data;
using Documentt.Models.Interface;
using Documentt.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq; // Đảm bảo có using này cho FirstOrDefault

namespace Documentt.Models.Services
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private TailieuContextDB dbContext;

        public List<ShopingcartItem> shopingcartItems { get; set; } // <--- Đảm bảo dòng này TỒN TẠI VÀ CHÍNH XÁC

        public string? ShoppingCartId { get; set; }

        public ShoppingCartRepository(TailieuContextDB dbContext)
        {
            this.dbContext = dbContext;
            // Khởi tạo shopingcartItems ở đây hoặc trong GetAllShoppingCartItems()
            // Tốt nhất là nó được gán giá trị khi GetAllShoppingCartItems() được gọi,
            // hoặc khi đối tượng được khởi tạo nếu nó là một cache.
            // Để tránh lỗi null, có thể khởi tạo rỗng:
            this.shopingcartItems = new List<ShopingcartItem>();
        }

        public static ShoppingCartRepository GetCart(IServiceProvider services)
        {
            ISession? session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session;

            TailieuContextDB context = services.GetService<TailieuContextDB>()
                ?? throw new Exception("Error initializing TailieuContextDB");

            string cartId = session?.GetString("CartId") ?? Guid.NewGuid().ToString();
            session?.SetString("CartId", cartId);

            return new ShoppingCartRepository(context)
            {
                ShoppingCartId = cartId,
                // Khi tạo mới đối tượng, cũng nên khởi tạo shopingcartItems
                shopingcartItems = new List<ShopingcartItem>()
            };
        }

        public void AddToCart(Product product)
        {
            var shoppingCartItem = dbContext.ShoppingCartItems
                .Include(s => s.Product)
                .FirstOrDefault(s => s.Product.ProductId == product.ProductId && s.ShoppingCartId == ShoppingCartId);

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShopingcartItem
                {
                    ShoppingCartId = ShoppingCartId,
                    Product = product,
                    Qty = 1,
                };
                dbContext.ShoppingCartItems.Add(shoppingCartItem);
            }
            // else: không làm gì cả, theo yêu cầu "chỉ 1 sl"

            dbContext.SaveChanges();

            // Cập nhật thuộc tính shopingcartItems sau khi thêm/sửa vào DB
            // Điều này đảm bảo shopingcartItems trong đối tượng repository luôn đồng bộ
            this.shopingcartItems = GetAllShoppingCartItems();
        }

        public int RemoveFromCart(Product product)
        {
            var shoppingCartItem = dbContext.ShoppingCartItems
                .Include(s => s.Product)
                .FirstOrDefault(s => s.Product.ProductId == product.ProductId && s.ShoppingCartId == ShoppingCartId);

            int quantity = 0; // Luôn trả về 0 sau khi xóa

            if (shoppingCartItem != null)
            {
                dbContext.ShoppingCartItems.Remove(shoppingCartItem);
                dbContext.SaveChanges();
            }

            // Cập nhật thuộc tính shopingcartItems sau khi xóa khỏi DB
            this.shopingcartItems = GetAllShoppingCartItems();

            return quantity;
        }

        public List<ShopingcartItem> GetAllShoppingCartItems()
        {
            // Luôn tải dữ liệu mới nhất từ DB
            // Sau đó gán vào thuộc tính shopingcartItems của lớp để fulfill interface.
            this.shopingcartItems = dbContext.ShoppingCartItems
                .Where(s => s.ShoppingCartId == ShoppingCartId)
                .Include(p => p.Product)
                .Include(p => p.Product.Category)
                .ToList();

            return this.shopingcartItems; // Trả về list đã gán
        }

        public void ClearCart()
        {
            var cartItems = dbContext.ShoppingCartItems
                .Where(s => s.ShoppingCartId == ShoppingCartId);

            dbContext.ShoppingCartItems.RemoveRange(cartItems);
            dbContext.SaveChanges();

            // Cập nhật thuộc tính shopingcartItems sau khi xóa hết khỏi DB
            this.shopingcartItems = new List<ShopingcartItem>();
        }

        public decimal GetShoppingCartTotal()
        {
            var totalCost = dbContext.ShoppingCartItems
                .Where(s => s.ShoppingCartId == ShoppingCartId)
                .Select(s => s.Product.Price * s.Qty)
                .Sum();

            return totalCost;
        }
    }
}