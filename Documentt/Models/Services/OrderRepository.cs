// File: Documentt/Models/Services/OrderRepository.cs
using Documentt.Data;
using Documentt.Models.Interface;
using System.Linq; // Thêm nếu chưa có, để dùng .Any()

namespace Documentt.Models.Services
{
    public class OrderRepository : IOrderRepository
    {
        private readonly TailieuContextDB _dbContext; // Đổi tên để nhất quán
        private readonly IShoppingCartRepository _shoppingCartRepository; // Đổi tên để nhất quán

        public OrderRepository(TailieuContextDB dbContext, IShoppingCartRepository shoppingCartRepository)
        {
            _dbContext = dbContext;
            _shoppingCartRepository = shoppingCartRepository;
        }

        public void PlaceOrder(Order order)
        {
            // Lấy các mặt hàng hiện có trong giỏ hàng
            var shoppingCartItems = _shoppingCartRepository.GetAllShoppingCartItems();

            // Đảm bảo OrderTotal được tính toán và gán vào Order model
            // (Mặc dù có thể tính trong Controller, nhưng làm ở đây an toàn hơn)
            order.OrderTotal = _shoppingCartRepository.GetShoppingCartTotal();

            // Đảm bảo ngày đặt hàng được set (cũng có thể set trong Controller)
            order.OrderPlaced = DateTime.Now;

            // Thêm Order chính vào DbContext
            // Entity Framework sẽ bắt đầu theo dõi đối tượng 'order' và gán Id tự động
            // (nếu Id được cấu hình là Identity trong database)
            _dbContext.Orders.Add(order);

            // Khởi tạo danh sách OrderDetails nếu nó chưa được khởi tạo
            // Điều này đảm bảo không bị lỗi NullReferenceException nếu OrderDetails là null
            order.OrderDetails = new List<OrderDetail>();

            // Kiểm tra xem có sản phẩm trong giỏ hàng không trước khi tạo OrderDetails
            if (shoppingCartItems != null && shoppingCartItems.Any())
            {
                foreach (var item in shoppingCartItems)
                {
                    var orderDetail = new OrderDetail
                    {
                        Quantity = item.Qty,
                        ProductId = item.Product.ProductId,
                        Price = item.Product.Price,
                        // Quan trọng: Liên kết OrderDetail này với Order chính
                        // Khi bạn thêm Order chính vào DbContext và liên kết các OrderDetail như này,
                        // Entity Framework sẽ tự động xử lý mối quan hệ và lưu cả hai.
                        Order = order
                    };

                    // Quan trọng: Thêm từng OrderDetail vào DbContext
                    // Nếu không có dòng này, OrderDetails sẽ không được lưu vào database.
                    _dbContext.OrderDetails.Add(orderDetail);

                    // Đồng thời, thêm OrderDetail vào danh sách OrderDetails của đối tượng Order
                    // Điều này giữ cho đối tượng Order trong bộ nhớ luôn đồng bộ với dữ liệu sẽ được lưu.
                    order.OrderDetails.Add(orderDetail);
                }
            }

            // LƯU TẤT CẢ THAY ĐỔI VÀO DATABASE trong một giao dịch duy nhất
            // Đây là bước cuối cùng để commit Order và tất cả OrderDetails đã được thêm và liên kết.
            _dbContext.SaveChanges();
        }
    }
}