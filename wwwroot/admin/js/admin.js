/**
 * Core Admin Logic
 * Handles Sidebar, Routing, and UI interactions
 */

document.addEventListener("DOMContentLoaded", () => {
    initSidebar();
    initRouting();
    initLogout();
});

// --- SIDEBAR LOGIC ---

function initSidebar() {
    const toggles = document.querySelectorAll(".accordion-toggle");
    const sidebarToggle = document.getElementById("sidebarToggle");
    const wrapper = document.querySelector(".admin-wrapper");

    // Accordion behavior
    toggles.forEach(toggle => {
        toggle.addEventListener("click", () => {
            const parent = toggle.closest(".nav-item");
            const submenu = parent.querySelector(".nav-submenu");
            const isOpen = parent.classList.contains("open");

            // Close others
            document.querySelectorAll(".nav-item.accordion").forEach(item => {
                if (item !== parent) {
                    item.classList.remove("open");
                    item.querySelector(".nav-submenu").classList.remove("show");
                }
            });

            // Toggle current
            parent.classList.toggle("open");
            submenu.classList.toggle("show");

            // Persist state
            if (parent.classList.contains("open")) {
                localStorage.setItem("admin_sidebar_open", toggle.dataset.id);
            } else {
                localStorage.removeItem("admin_sidebar_open");
            }
        });
    });

    // Restore sidebar state
    const savedId = localStorage.getItem("admin_sidebar_open");
    if (savedId) {
        const savedToggle = document.querySelector(`.accordion-toggle[data-id="${savedId}"]`);
        if (savedToggle) {
            const parent = savedToggle.closest(".nav-item");
            parent.classList.add("open");
            parent.querySelector(".nav-submenu").classList.add("show");
        }
    }

    // Mobile toggle
    if (sidebarToggle) {
        sidebarToggle.addEventListener("click", () => {
            wrapper.classList.toggle("sidebar-open");
        });
    }
}

// --- ROUTING LOGIC ---

function initRouting() {
    window.addEventListener("hashchange", handleRoute);
    handleRoute(); // Handle initial load
}

async function handleRoute() {
    const hash = window.location.hash || "#dashboard";
    const pageId = hash.substring(1);
    
    // Update Active Link
    updateActiveLink(hash);

    // Update Page Title
    const titleMap = {
        "dashboard": "Dashboard Overview",
        "add-product": "Add New Product",
        "view-products": "Manage Products",
        "categories": "Product Categories",
        "users": "User Management",
        "orders": "Order History",
        "import-products": "Import Products",
        "import-users": "Import Users",
        "export-data": "Export Data"
    };
    document.getElementById("pageTitle").textContent = titleMap[pageId] || "Admin";

    // Load Content
    await loadPageContent(pageId);
}

function updateActiveLink(hash) {
    document.querySelectorAll(".nav-link, .sub-link").forEach(link => {
        link.classList.remove("active");
        if (link.getAttribute("href") === hash) {
            link.classList.add("active");
            
            // If it's a sub-link, ensure parent is open
            if (link.classList.contains("sub-link")) {
                const parent = link.closest(".nav-item.accordion");
                if (parent && !parent.classList.contains("open")) {
                    parent.classList.add("open");
                    parent.querySelector(".nav-submenu").classList.add("show");
                }
            }
        }
    });
}

async function loadPageContent(pageId) {
    const contentArea = document.getElementById("contentArea");
    showLoader();

    try {
        // In a real SPA, we might fetch HTML fragments. 
        // For this task, I'll generate the HTML dynamically in JS for each module
        // to stay within the "Pure HTML/JS" constraint without needing multiple .html files.
        
        let html = "";
        switch(pageId) {
            case "dashboard":
                html = await renderDashboard();
                break;
            case "add-product":
                html = await renderAddProduct();
                break;
            case "view-products":
                html = await renderProducts();
                break;
            case "categories":
                html = await renderCategories();
                break;
            case "users":
                html = await renderUsers();
                break;
            case "orders":
                html = await renderOrders();
                break;
            case "import-products":
                html = await renderImportProducts();
                break;
            case "export-data":
                html = await renderExportData();
                break;
            default:
                html = `<div class="text-center py-5">
                            <i class="bi bi-hammer fs-1 text-muted"></i>
                            <h3 class="mt-3">Module Under Construction</h3>
                            <p class="text-muted">The ${pageId} module is being upgraded.</p>
                        </div>`;
        }

        contentArea.innerHTML = html;
        
        // Post-render logic (initialize charts, etc.)
        if (pageId === "dashboard") initDashboardCharts();
        if (pageId === "view-products") initProductsTable();
        if (pageId === "add-product") initProductForm();
        if (pageId === "categories") initCategories();
        if (pageId === "users") initUsers();
        if (pageId === "orders") initOrders();
        if (pageId === "import-products") initImportProducts();

    } catch (error) {
        console.error("Error loading page:", error);
        contentArea.innerHTML = `<div class="alert alert-danger">Error loading content. Please try again.</div>`;
    } finally {
        hideLoader();
    }
}

// --- LOGOUT LOGIC ---

function initLogout() {
    const btn = document.getElementById("logoutBtn");
    if (btn) {
        btn.addEventListener("click", () => {
            localStorage.removeItem("admin_token");
            window.location.href = "/Users/Login";
        });
    }
}

// --- UTILS ---

function showToast(message, type = "success") {
    const toastEl = document.getElementById("liveToast");
    const toastBody = toastEl.querySelector(".toast-body");
    
    toastEl.classList.remove("bg-success", "bg-danger", "text-white");
    if (type === "error") {
        toastEl.classList.add("bg-danger", "text-white");
    } else {
        toastEl.classList.add("bg-success", "text-white");
    }

    toastBody.textContent = message;
    const toast = new bootstrap.Toast(toastEl);
    toast.show();
}

// --- MODULE PLACEHOLDERS (To be expanded in Step 5-6) ---

async function renderProducts() {
    return `
        <div class="card border-0 shadow-sm rounded-4 p-4">
            <div class="d-flex justify-content-between align-items-center mb-4">
                <div class="search-box position-relative" style="width: 300px;">
                    <i class="bi bi-search position-absolute top-50 start-0 translate-middle-y ms-3 text-muted"></i>
                    <input type="text" id="productSearch" class="form-control ps-5 rounded-pill border-0 bg-light" placeholder="Search products...">
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
                        <tr><td colspan="5" class="text-center py-5">Loading products...</td></tr>
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
    console.log("Initializing products table...");
}
