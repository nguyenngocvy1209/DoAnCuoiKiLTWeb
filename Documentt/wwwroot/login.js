// File: wwwroot/js/login.js

document.addEventListener('DOMContentLoaded', function () {
    const loginForm = document.getElementById('login-form');

    if (loginForm) {
        const emailInput = document.getElementById('login-email');
        const passwordInput = document.getElementById('login-password');
        const emailError = document.getElementById('login-email-error');
        const passwordError = document.getElementById('login-password-error');
        const validationSummary = document.getElementById('validation-summary'); // For custom JS validation summary

        // Hàm kiểm tra định dạng email
        function isValidEmail(email) {
            const re = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            return re.test(String(email).toLowerCase());
        }

        loginForm.addEventListener('submit', function (event) {
            let isValid = true;

            // Xóa tất cả các thông báo lỗi hiện có từ JS client-side
            emailError.classList.add('hidden');
            passwordError.classList.add('hidden');
            validationSummary.classList.add('hidden');
            validationSummary.innerHTML = ''; // Xóa nội dung lỗi cũ

            // Validate Email
            if (emailInput.value.trim() === '') {
                emailError.textContent = 'Email không được để trống.';
                emailError.classList.remove('hidden');
                isValid = false;
            } else if (!isValidEmail(emailInput.value.trim())) {
                emailError.textContent = 'Email không đúng định dạng.';
                emailError.classList.remove('hidden');
                isValid = false;
            }

            // Validate Password
            if (passwordInput.value.trim() === '') {
                passwordError.textContent = 'Mật khẩu không được để trống.';
                passwordError.classList.remove('hidden');
                isValid = false;
            } else if (passwordInput.value.trim().length < 6) {
                passwordError.textContent = 'Mật khẩu phải có ít nhất 6 ký tự.';
                passwordError.classList.remove('hidden');
                isValid = false;
            }

            if (!isValid) {
                // If client-side validation fails, prevent default form submission
                event.preventDefault();

                // Hiển thị thông báo lỗi tổng quát nếu có lỗi
                // Chỉ hiển thị summary nếu có ít nhất một lỗi cụ thể đã được hiển thị
                if (validationSummary && (!emailError.classList.contains('hidden') || !passwordError.classList.contains('hidden'))) {
                    validationSummary.textContent = 'Vui lòng kiểm tra lại thông tin đăng nhập.';
                    validationSummary.classList.remove('hidden');
                }
            } else {
                // If client-side validation passes, allow the form to submit normally.
                // DO NOT call event.preventDefault() here.
                // The form will automatically submit to the server (OnPostAsync in LoginModel.cs).
                // Server-side validation and authentication will then take over.
            }
        });
    }
});