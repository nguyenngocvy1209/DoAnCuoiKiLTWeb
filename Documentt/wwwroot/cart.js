// Lấy thông báo và loại thông báo từ TempData
var sweetAlertMessage = '@TempData["SweetAlertMessage"]';
var sweetAlertType = '@TempData["SweetAlertType"]';

// Chỉ hiển thị SweetAlert nếu có thông báo (sau khi thanh toán thành công)
if (sweetAlertMessage && sweetAlertMessage !== '') {
    Swal.fire({
        title: 'Thông báo',
        text: sweetAlertMessage,
        icon: sweetAlertType,
        confirmButtonText: 'Đóng'
    });
}

// --- JavaScript cho Popup Xác nhận thanh toán (Popup lần 1) ---
document.getElementById('checkout-btn').addEventListener('click', function (event) {
    event.preventDefault(); // Ngăn chặn hành vi mặc định của nút

    // Lấy tổng số tiền từ ViewBag và định dạng lại
    // SỬA CHỖ NÀY: Xử lý null an toàn cho ViewBag.TotalCart
    var totalCartAmount = '@(ViewBag.TotalCart?.ToString("N0") ?? "0")';
    var numberOfItems = '@Model.Count()'; // Lấy số lượng item từ Model

    // Kiểm tra xem giỏ hàng có trống không
    if (parseInt(numberOfItems) === 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Giỏ hàng trống',
            text: 'Vui lòng thêm sản phẩm vào giỏ hàng trước khi thanh toán.',
            confirmButtonText: 'Đóng'
        });
        return; // Dừng lại nếu giỏ hàng trống
    }

    Swal.fire({
        title: 'Xác nhận thanh toán',
        html: `
                            <p>Bạn có chắc chắn muốn thanh toán đơn hàng này?</p>
                            <p>Tổng số tiền: <strong>${totalCartAmount} VNĐ</strong></p>
                            <p>Số lượng sản phẩm: <strong>${numberOfItems}</strong></p>
                        `,
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Xác nhận thanh toán',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            // Nếu người dùng nhấn "Xác nhận", submit form thanh toán ẩn
            document.getElementById('checkoutForm').submit();
        }
    });
});