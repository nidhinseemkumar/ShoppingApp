/**
 * Products Module
 */

let currentProductsPage = 1;
let productsSearchTerm = "";

async function renderProducts() {
    return `
        <div class="card border-0 shadow-sm rounded-4 p-4">
            <div class="d-flex justify-content-between align-items-center mb-4">
                <div class="search-box position-relative" style="width: 300px;">
                    <i class="bi bi-search position-absolute top-50 start-0 translate-middle-y ms-3 text-muted"></i>
                    <input type="text" id="productSearch" class="form-control ps-5 rounded-pill border-0 bg-light" placeholder="Search products..." value="${productsSearchTerm}">
                </div>
                <a href="#add-product" class="btn btn-primary rounded-pill px-4">
                    <i class="bi bi-plus-lg me-2"></i>Add Product
                </a>
            </div>
            <div class="table-responsive">
                <table class="table table-hover align-middle">
                    <thead class="bg-light">
                        <tr>
                            <th>Product</th>
                            <th>Category</th>
                            <th>Price</th>
                            <th>Stock</th>
                            <th class="text-end">Actions</th>
                        </tr>
                    </thead>
                    <tbody id="productsTableBody">
                        <!-- Dynamic -->
                    </tbody>
                </table>
            </div>
            <div id="pagination" class="d-flex justify-content-between align-items-center mt-4">
                <span class="text-muted small">Showing <span id="showingCount">0</span> of <span id="totalCount">0</span></span>
                <nav><ul class="pagination pagination-sm mb-0" id="paginationList"></ul></nav>
            </div>
        </div>
    `;
}

function initProductsTable() {
    const searchInput = document.getElementById("productSearch");
    let timeout;

    searchInput.addEventListener("input", () => {
        clearTimeout(timeout);
        timeout = setTimeout(() => {
            productsSearchTerm = searchInput.value;
            currentProductsPage = 1;
            loadProducts();
        }, 300); // Debounce
    });

    loadProducts();
}

async function loadProducts() {
    showLoader();
    const result = await apiFetch(`/api/admin/products?page=${currentProductsPage}&pageSize=10&search=${encodeURIComponent(productsSearchTerm)}`);
    hideLoader();

    if (!result || !result.success) return;

    const { items, totalCount } = result.data;
    const body = document.getElementById("productsTableBody");

    if (items.length === 0) {
        body.innerHTML = `<tr><td colspan="5" class="text-center py-5 text-muted">No products found.</td></tr>`;
    } else {
        body.innerHTML = items.map(p => `
            <tr>
                <td>
                    <div class="d-flex align-items-center">
                        <img src="${p.imageUrl || '/images/placeholder.png'}" class="rounded-3 me-3" style="width: 40px; height: 40px; object-fit: cover;">
                        <div>
                            <div class="fw-bold text-dark">${p.name}</div>
                            <div class="small text-muted text-truncate" style="max-width: 200px;">${p.description || ''}</div>
                        </div>
                    </div>
                </td>
                <td><span class="badge bg-light text-dark border">${p.categoryName || 'Uncategorized'}</span></td>
                <td class="fw-bold">₹${p.price?.toLocaleString()}</td>
                <td>
                    <span class="badge ${p.stock < 10 ? 'bg-danger-subtle text-danger' : 'bg-success-subtle text-success'} rounded-pill">
                        ${p.stock} in stock
                    </span>
                </td>
                <td class="text-end">
                    <div class="btn-group">
                        <button class="btn btn-sm btn-outline-secondary rounded-circle me-1" onclick="editProduct(${p.productId})"><i class="bi bi-pencil"></i></button>
                        <button class="btn btn-sm btn-outline-danger rounded-circle" onclick="confirmDeleteProduct(${p.productId})"><i class="bi bi-trash"></i></button>
                    </div>
                </td>
            </tr>
        `).join('');
    }

    // Update Pagination Info
    document.getElementById("showingCount").textContent = items.length;
    document.getElementById("totalCount").textContent = totalCount;
    renderPagination(totalCount);
}

function renderPagination(totalCount) {
    const totalPages = Math.ceil(totalCount / 10);
    const list = document.getElementById("paginationList");
    let html = "";

    for (let i = 1; i <= totalPages; i++) {
        html += `<li class="page-item ${i === currentProductsPage ? 'active' : ''}">
                    <button class="page-link" onclick="changeProductsPage(${i})">${i}</button>
                 </li>`;
    }
    list.innerHTML = html;
}

function changeProductsPage(page) {
    currentProductsPage = page;
    loadProducts();
}

// --- ADD / EDIT PRODUCT ---

async function renderAddProduct() {
    const categories = await apiFetch("/api/admin/categories");
    const catOptions = categories?.data?.map(c => `<option value="${c.categoryId}">${c.categoryName}</option>`).join('') || '';

    return `
        <div class="row justify-content-center">
            <div class="col-lg-8">
                <div class="card border-0 shadow-sm rounded-4 p-4">
                    <form id="productForm">
                        <input type="hidden" name="productId" id="form-productId">
                        <div class="row g-3">
                            <div class="col-md-12">
                                <label class="form-label fw-bold">Product Name</label>
                                <input type="text" name="name" class="form-control rounded-3" required>
                            </div>
                            <div class="col-md-6">
                                <label class="form-label fw-bold">Category</label>
                                <select name="categoryId" class="form-select rounded-3" required>
                                    <option value="">Select Category</option>
                                    ${catOptions}
                                </select>
                            </div>
                            <div class="col-md-6">
                                <label class="form-label fw-bold">Price (₹)</label>
                                <input type="number" name="price" class="form-control rounded-3" step="0.01" required>
                            </div>
                            <div class="col-md-6">
                                <label class="form-label fw-bold">Stock Quantity</label>
                                <input type="number" name="stock" class="form-control rounded-3" required>
                            </div>
                            <div class="col-md-6">
                                <label class="form-label fw-bold">Product Image</label>
                                <input type="file" name="image" class="form-control rounded-3" accept="image/*">
                            </div>
                            <div class="col-md-12">
                                <label class="form-label fw-bold">Description</label>
                                <textarea name="description" class="form-control rounded-3" rows="4"></textarea>
                            </div>
                            <div class="col-md-12 text-end mt-4">
                                <button type="button" class="btn btn-light rounded-pill px-4 me-2" onclick="window.location.hash='#view-products'">Cancel</button>
                                <button type="submit" class="btn btn-primary rounded-pill px-5">Save Product</button>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    `;
}

function initProductForm() {
    const form = document.getElementById("productForm");
    form.addEventListener("submit", async (e) => {
        e.preventDefault();
        const formData = new FormData(form);
        const productId = document.getElementById("form-productId").value;
        
        showLoader();
        const url = productId ? `/api/admin/products/${productId}` : "/api/admin/products";
        const method = productId ? "PUT" : "POST";

        const result = await apiFetch(url, {
            method: method,
            body: formData // FormData handles multipart automatically
        });
        hideLoader();

        if (result && result.success) {
            showToast(productId ? "Product updated!" : "Product added!");
            window.location.hash = "#view-products";
        }
    });
}

// --- CATEGORIES MODULE ---

async function renderCategories() {
    return `
        <div class="row g-4">
            <div class="col-md-4">
                <div class="card border-0 shadow-sm rounded-4 p-4">
                    <h5 class="fw-bold mb-4">Add Category</h5>
                    <form id="categoryForm">
                        <div class="mb-3">
                            <label class="form-label small fw-bold">Category Name</label>
                            <input type="text" name="categoryName" class="form-control rounded-3" required>
                        </div>
                        <button type="submit" class="btn btn-primary w-100 rounded-pill">Save Category</button>
                    </form>
                </div>
            </div>
            <div class="col-md-8">
                <div class="card border-0 shadow-sm rounded-4 p-4">
                    <h5 class="fw-bold mb-4">Existing Categories</h5>
                    <div class="table-responsive">
                        <table class="table table-hover align-middle">
                            <thead class="bg-light">
                                <tr>
                                    <th>Name</th>
                                    <th class="text-end">Actions</th>
                                </tr>
                            </thead>
                            <tbody id="categoriesTableBody">
                                <!-- Dynamic -->
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    `;
}

function initCategories() {
    loadCategories();
    
    document.getElementById("categoryForm").addEventListener("submit", async (e) => {
        e.preventDefault();
        const name = e.target.categoryName.value;
        const result = await apiFetch("/api/admin/categories", {
            method: "POST",
            body: { categoryName: name }
        });
        if (result && result.success) {
            showToast("Category added!");
            e.target.reset();
            loadCategories();
        }
    });
}

async function loadCategories() {
    const result = await apiFetch("/api/admin/categories");
    if (!result || !result.success) return;

    const body = document.getElementById("categoriesTableBody");
    body.innerHTML = result.data.map(c => `
        <tr>
            <td class="fw-bold">${c.categoryName}</td>
            <td class="text-end">
                <button class="btn btn-sm btn-outline-danger rounded-circle" onclick="deleteCategory(${c.categoryId})"><i class="bi bi-trash"></i></button>
            </td>
        </tr>
    `).join('');
}

// --- GLOBAL HANDLERS ---

window.editProduct = async (id) => {
    window.location.hash = "#add-product";
    setTimeout(async () => {
        showLoader();
        const result = await apiFetch(`/api/admin/products/${id}`);
        hideLoader();
        if (result && result.success) {
            const p = result.data;
            const form = document.getElementById("productForm");
            document.getElementById("form-productId").value = p.productId;
            form.name.value = p.name;
            form.categoryId.value = p.categoryId; // Assuming categoryId is returned
            form.price.value = p.price;
            form.stock.value = p.stock;
            form.description.value = p.description;
            
            document.getElementById("pageTitle").textContent = "Edit Product";
        }
    }, 100);
};

window.confirmDeleteProduct = async (id) => {
    if (confirm("Are you sure you want to delete this product?")) {
        showLoader();
        const result = await apiFetch(`/api/admin/products/${id}`, { method: "DELETE" });
        hideLoader();
        if (result && result.success) {
            showToast("Product deleted");
            loadProducts();
        }
    }
};

window.deleteCategory = async (id) => {
    if (confirm("Are you sure you want to delete this category?")) {
        showLoader();
        const result = await apiFetch(`/api/admin/categories/${id}`, { method: "DELETE" });
        hideLoader();
        if (result && result.success) {
            showToast("Category deleted");
            loadCategories();
        }
    }
};
