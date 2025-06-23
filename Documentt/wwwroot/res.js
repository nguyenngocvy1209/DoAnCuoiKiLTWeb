document.addEventListener('DOMContentLoaded', function () {
    const registerForm = document.getElementById('register-form');
    if (registerForm) {
        const usernameInput = document.getElementById('register-username');
        const emailInput = document.getElementById('register-email');
        const passwordInput = document.getElementById('register-password');
        const confirmPasswordInput = document.getElementById('register-confirm-password');

        const usernameError = document.getElementById('register-username-error');
        const emailError = document.getElementById('register-email-error');
        const passwordError = document.getElementById('register-password-error');
        const confirmPasswordError = document.getElementById('register-confirm-password-error');
        const validationSummary = document.getElementById('validation-summary'); // Cần có trên trang đăng ký nếu muốn lỗi tổng hợp

        // Hàm kiểm tra định dạng email
        function isValidEmail(email) {
            const re = /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            return re.test(String(email).toLowerCase());
        }

        registerForm.addEventListener('submit', function (event) {
            let isValid = true;

            // Xóa tất cả các thông báo lỗi hiện có
            usernameError.classList.add('hidden');
            emailError.classList.add('hidden');
            passwordError.classList.add('hidden');
            confirmPasswordError.classList.add('hidden');
            if (validationSummary) { // Kiểm tra nếu phần tử tồn tại
                validationSummary.classList.add('hidden');
                validationSummary.innerHTML = '';
            }

            // Validate Username
            if (usernameInput.value.trim() === '') {
                usernameError.textContent = 'Tên người dùng không được để trống.';
                usernameError.classList.remove('hidden');
                isValid = false;
            } else if (usernameInput.value.trim().length < 3) {
                usernameError.textContent = 'Tên người dùng phải có ít nhất 3 ký tự.';
                usernameError.classList.remove('hidden');
                isValid = false;
            }

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

            // Validate Confirm Password
            if (confirmPasswordInput.value.trim() === '') {
                confirmPasswordError.textContent = 'Vui lòng xác nhận mật khẩu.';
                confirmPasswordError.classList.remove('hidden');
                isValid = false;
            } else if (passwordInput.value !== confirmPasswordInput.value) {
                confirmPasswordError.textContent = 'Mật khẩu xác nhận không khớp.';
                confirmPasswordError.classList.remove('hidden');
                isValid = false;
            }

            if (!isValid) {
                event.preventDefault(); // Ngăn chặn form gửi đi
                // Hiển thị thông báo lỗi tổng quát nếu có lỗi
                if (validationSummary && !usernameError.classList.contains('hidden') || !emailError.classList.contains('hidden') || !passwordError.classList.contains('hidden') || !confirmPasswordError.classList.contains('hidden')) {
                    validationSummary.textContent = 'Vui lòng kiểm tra lại thông tin đăng ký.';
                    validationSummary.classList.remove('hidden');
                }
            } else {
                // Nếu validation thành công ở client, bạn có thể gửi form bằng AJAX tại đây
                // Ví dụ: gửi form thủ công bằng fetch API
                // event.preventDefault(); // Ngăn chặn submit mặc định nếu dùng AJAX
                // const formData = new FormData(registerForm);
                // fetch('/api/auth/register', {
                //     method: 'POST',
                //     body: formData
                // })
                // .then(response => response.json())
                // .then(data => {
                //     if (data.success) {
                //         alert('Đăng ký thành công! Vui lòng đăng nhập.');
                //         window.location.href = '/login.html'; // Chuyển hướng khi đăng ký thành công
                //     } else {
                //         if (validationSummary) {
                //             validationSummary.textContent = data.message || 'Đăng ký thất bại. Vui lòng thử lại.';
                //             validationSummary.classList.remove('hidden');
                //         }
                //     }
                // })
                // .catch(error => {
                //     console.error('Lỗi khi đăng ký:', error);
                //     if (validationSummary) {
                //         validationSummary.textContent = 'Có lỗi xảy ra. Vui lòng thử lại sau.';
                //         validationSummary.classList.remove('hidden');
                //     }
                // });
            }
        });
    }
});