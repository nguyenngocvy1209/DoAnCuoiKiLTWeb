let products = [
    { id: '1', name: 'Tài liệu Kinh tế v1', price: 50000, category: 'Kinh tế', image: 'https://via.placeholder.com/150', stock: 10, rating: 4.5, reviews: ['Good', 'Very informative'], description: 'Tài liệu Kinh tế cơ bản.', fileUrl: 'https://example.com/files/economics.pdf' },
    { id: '2', name: 'Lập trình JavaScript', price: 45000, category: 'Lập trình', image: 'https://via.placeholder.com/150', stock: 500, rating: 4.8, reviews: ['Rất chi tiết', 'Hữu ích cho người mới'], description: 'Học JavaScript từ cơ bản đến nâng cao.', fileUrl: 'https://example.com/files/javascript.pdf' },
    { id: '3', name: 'Toán cao cấp', price: 60000, category: 'Toán học', image: 'https://via.placeholder.com/150', stock: 100, rating: 4.0, reviews: [], description: 'Tài liệu ôn tập Toán cao cấp.', fileUrl: 'https://example.com/files/math.pdf' },
    { id: '4', name: 'Kỹ thuật cơ khí', price: 90000, category: 'Kỹ thuật', image: 'https://via.placeholder.com/150', stock: 20, rating: 4.2, reviews: [], description: 'Kiến thức cơ bản về kỹ thuật cơ khí.', fileUrl: 'https://example.com/files/mechanical.pdf' },
    { id: '5', name: 'Khoa học dữ liệu', price: 85000, category: 'Khoa học dữ liệu', image: 'https://via.placeholder.com/150', stock: 30, rating: 4.7, reviews: ['Tuyệt vời!'], description: 'Giới thiệu về khoa học dữ liệu.', fileUrl: 'https://example.com/files/datascience.pdf' },
    { id: '6', name: 'Vật lý đại cương', price: 55000, category: 'Vật lý', image: 'https://via.placeholder.com/150', stock: 40, rating: 4.3, reviews: [], description: 'Tài liệu Vật lý cơ bản.', fileUrl: 'https://example.com/files/physics.pdf' },
    { id: '7', name: 'Thiết kế đồ họa', price: 70000, category: 'Thiết kế', image: 'https://via.placeholder.com/150', stock: 25, rating: 4.6, reviews: [], description: 'Học thiết kế đồ họa với Photoshop.', fileUrl: 'https://example.com/files/design.pdf' },
    { id: '8', name: 'Hóa học cơ bản', price: 65000, category: 'Hóa học', image: 'https://via.placeholder.com/150', stock: 50, rating: 4.4, reviews: [], description: 'Tài liệu Hóa học cơ bản.', fileUrl: 'https://example.com/files/chemistry.pdf' },
    { id: '9', name: 'Ngôn ngữ học nhập môn', price: 60000, category: 'Ngôn ngữ học', image: 'https://via.placeholder.com/150', stock: 20, rating: 4.4, reviews: [], description: 'Giới thiệu cơ bản về ngôn ngữ học.', fileUrl: 'https://example.com/files/linguistics.pdf' },
    { id: '10', name: 'Quản trị chiến lược', price: 80000, category: 'Quản trị kinh doanh', image: 'https://via.placeholder.com/150', stock: 15, rating: 4.6, reviews: [], description: 'Kiến thức về quản trị chiến lược doanh nghiệp.', fileUrl: 'https://example.com/files/strategy.pdf' }
];

function safeGetStorage(key) {
    try {
        return JSON.parse(localStorage.getItem(key));
    } catch (e) {
        console.error(`Error reading ${key} from localStorage`, e);
        return null;
    }
}

function safeSetStorage(key, value) {
    try {
        localStorage.setItem(key, JSON.stringify(value));
    } catch (e) {
        console.error(`Error saving ${key} to localStorage`, e);
    }
}

function showToast(message, type = 'info') {
    const toast = document.createElement('div');
    toast.className = `fixed bottom-4 right-4 px-4 py-2 rounded-lg text-white ${type === 'error' ? 'bg-red-500' : 'bg-blue-500'}`;
    toast.textContent = message;
    document.body.appendChild(toast);
    setTimeout(() => toast.remove(), 3000);
}

function renderProducts(page = 1, category = 'all', sort = 'default') {
    const productsList = document.getElementById('products-list');
    if (!productsList) return;

    let filteredProducts = category === 'all' ? [...products] : products.filter(p => p.category === category);

    if (sort === 'price-asc') filteredProducts.sort((a, b) => a.price - b.price);
    else if (sort === 'price-desc') filteredProducts.sort((a, b) => b.price - a.price);
    else if (sort === 'rating-desc') filteredProducts.sort((a, b) => b.rating - a.rating);

    const productsPerPage = 8;
    const start = (page - 1) * productsPerPage;
    const end = start + productsPerPage;
    const paginatedProducts = filteredProducts.slice(start, end);

    productsList.innerHTML = paginatedProducts.map(product => `
        <div class="product-card bg-white rounded-lg shadow-md p-4 hover:shadow-lg transition cursor-pointer" data-product-id="${product.id}">
            <img src="${product.image || 'https://via.placeholder.com/150'}" alt="${product.name}" class="w-full h-40 object-cover rounded-t-lg mb-4">
            <h3 class="text-lg font-semibold mb-2">${product.name}</h3>
            <p class="text-blue-600 font-bold mb-2">${product.price.toLocaleString('vi-VN')} VNĐ</p>
            <p class="text-sm text-gray-600 mb-2">${product.category}</p>
            <div class="text-yellow-500 mb-2">${'★'.repeat(Math.floor(product.rating))}${'☆'.repeat(5 - Math.floor(product.rating))}</div>
            <p class="text-sm text-gray-600">Còn lại: ${product.stock}</p>
        </div>
    `).join('');

    const loadMoreBtn = document.getElementById('load-more');
    if (loadMoreBtn) {
        loadMoreBtn.classList.toggle('hidden', end >= filteredProducts.length);
        loadMoreBtn.dataset.page = page + 1;
    }

    document.querySelectorAll('.product-card').forEach(card => {
        card.addEventListener('click', () => {
            const productId = card.dataset.productId;
            window.location.href = `/product-detail.html?id=${productId}`;
        });
    });
}

function renderProductDetailPage() {
    const urlParams = new URLSearchParams(window.location.search);
    const productId = urlParams.get('id');
    const product = products.find(p => p.id === productId);
    const productDetails = document.getElementById('product-details');

    if (!product || !productDetails) {
        productDetails.innerHTML = `
            <div class="container mx-auto px-4 text-center">
                <h1 class="text-3xl font-bold text-red-600 mb-4">Không tìm thấy tài liệu</h1>
                <p class="text-gray-600 mb-4">Tài liệu bạn tìm kiếm không tồn tại hoặc đã bị xóa.</p>
                <a href="/products.html" class="bg-blue-600 text-white py-2 px-6 rounded-lg hover:bg-blue-700 transition">Quay lại danh sách sản phẩm</a>
            </div>
        `;
        showToast('Không tìm thấy tài liệu.', 'error');
        return;
    }

    document.getElementById('breadcrumb-product').textContent = product.name;
    productDetails.innerHTML = `
        <div class="container mx-auto px-4">
            <div class="flex flex-col md:flex-row gap-8">
                <img id="product-image" src="${product.image || 'https://via.placeholder.com/150'}" alt="${product.name}" class="w-full md:w-1/2 h-96 object-cover rounded-lg">
                <div class="w-full md:w-1/2">
                    <h1 id="product-title" class="text-3xl font-bold mb-4">${product.name}</h1>
                    <p id="product-price" class="text-2xl font-bold text-blue-600 mb-4">${product.price.toLocaleString('vi-VN')} VNĐ</p>
                    <p id="product-description" class="text-gray-600 mb-4">${product.description || 'Không có mô tả'}</p>
                    <p id="product-category" class="text-sm text-gray-600 mb-2">Danh mục: ${product.category}</p>
                    <p id="product-rating" class="text-sm text-yellow-500 mb-2">${'★'.repeat(Math.floor(product.rating))}${'☆'.repeat(5 - Math.floor(product.rating))} (${product.rating})</p>
                    <p id="product-stock" class="text-sm text-gray-600 mb-4">Còn lại: ${product.stock}</p>
                    <div class="flex gap-4">
                        <button id="add-to-cart-btn" class="bg-blue-600 text-white py-2 px-6 rounded-lg hover:bg-blue-700 transition" data-product-id="${product.id}">Thêm vào giỏ</button>
                        <button id="buy-now-btn" class="bg-green-600 text-white py-2 px-6 rounded-lg hover:bg-green-700 transition" data-product-id="${product.id}">Mua ngay</button>
                    </div>
                </div>
            </div>
        </div>
    `;

    const previewCarousel = document.getElementById('preview-carousel');
    if (previewCarousel) {
        previewCarousel.innerHTML = Array.from({ length: 5 }, (_, i) => `
            <img src="https://via.placeholder.com/800x400?text=Xem+trước+trang+${i+1}" alt="Xem trước trang ${i+1}" class="w-full h-64 object-cover flex-shrink-0">
        `).join('');
    }

    const relatedProducts = products.filter(p => p.category === product.category && p.id !== productId).slice(0, 3);
    const relatedProductsContainer = document.getElementById('related-products');
    if (relatedProductsContainer) {
        relatedProductsContainer.innerHTML = relatedProducts.map(p => `
            <div class="product-card bg-white rounded-lg shadow-md p-4 hover:shadow-lg transition cursor-pointer" data-product-id="${p.id}">
                <img src="${p.image || 'https://via.placeholder.com/150'}" alt="${p.name}" class="w-full h-40 object-cover rounded-t-lg mb-4">
                <h3 class="text-lg font-semibold mb-2">${p.name}</h3>
                <p class="text-blue-600 font-bold mb-2">${p.price.toLocaleString('vi-VN')} VNĐ</p>
                <p class="text-sm text-gray-600 mb-2">${p.category}</p>
            </div>
        `).join('');
        relatedProductsContainer.querySelectorAll('.product-card').forEach(card => {
            card.addEventListener('click', () => {
                window.location.href = `/product-detail.html?id=${card.dataset.productId}`;
            });
        });
    }

    const addToCartBtn = document.getElementById('add-to-cart-btn');
    if (addToCartBtn) {
        addToCartBtn.addEventListener('click', () => {
            let cart = safeGetStorage('cart') || [];
            const existingItem = cart.find(item => item.id === product.id);
            if (existingItem) {
                existingItem.quantity += 1;
            } else {
                cart.push({ id: product.id, name: product.name, price: product.price, quantity: 1 });
            }
            safeSetStorage('cart', cart);
            showToast('Đã thêm vào giỏ hàng!', 'info');
            updateCartCount();
        });
    }

    const buyNowBtn = document.getElementById('buy-now-btn');
    if (buyNowBtn) {
        buyNowBtn.addEventListener('click', () => {
            const user = safeGetStorage('user');
            if (!user) {
                showToast('Vui lòng đăng nhập để mua hàng.', 'error');
                // TODO: Show login modal
                return;
            }
            if (user.balance < product.price) {
                showToast('Số dư không đủ. Vui lòng nạp tiền.', 'error');
                // TODO: Show top-up modal
                return;
            }
            user.balance -= product.price;
            user.purchases = user.purchases || [];
            user.purchases.push({
                productId: product.id,
                name: product.name,
                price: product.price,
                timestamp: new Date().toISOString()
            });
            safeSetStorage('user', user);
            showToast('Mua hàng thành công!', 'success');
        });
    }

    const slides = document.querySelector('.slides');
    const prevBtn = document.querySelector('.slider-btn.prev');
    const nextBtn = document.querySelector('.slider-btn.next');
    let currentIndex = 0;

    if (slides && prevBtn && nextBtn) {
        const slideCount = slides.children.length;
        prevBtn.addEventListener('click', () => {
            currentIndex = (currentIndex - 1 + slideCount) % slideCount;
            slides.style.transform = `translateX(-${currentIndex * 100}%)`;
        });
        nextBtn.addEventListener('click', () => {
            currentIndex = (currentIndex + 1) % slideCount;
            slides.style.transform = `translateX(-${currentIndex * 100}%)`;
        });
    }
}

function updateCartCount() {
    const cart = safeGetStorage('cart') || [];
    const cartCount = document.getElementById('cart-count');
    if (cartCount) {
        const totalItems = cart.reduce((sum, item) => sum + item.quantity, 0);
        cartCount.textContent = totalItems;
        cartCount.classList.toggle('hidden', totalItems === 0);
    }
}

document.addEventListener('DOMContentLoaded', () => {
    if (document.getElementById('products-list')) {
        renderProducts();
        document.getElementById('sort-select')?.addEventListener('change', e => renderProducts(1, document.querySelector('.category-btn.active')?.dataset.category || 'all', e.target.value));
        document.querySelectorAll('.category-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                document.querySelector('.category-btn.active')?.classList.remove('active', 'bg-blue-900', 'text-white');
                btn.classList.add('active', 'bg-blue-900', 'text-white');
                renderProducts(1, btn.dataset.category);
            });
        });
        document.getElementById('load-more')?.addEventListener('click', () => {
            const page = parseInt(document.getElementById('load-more').dataset.page);
            renderProducts(page, document.querySelector('.category-btn.active')?.dataset.category || 'all', document.getElementById('sort-select')?.value || 'default');
        });
    }

    if (document.getElementById('product-details')) {
        renderProductDetailPage();
    }

    updateCartCount();
});