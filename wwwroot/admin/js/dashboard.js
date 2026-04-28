/**
 * Dashboard Module
 */

async function renderDashboard() {
    return `
        <div class="row g-4" id="statsContainer">
            <div class="col-md-3">
                <div class="card stat-card shadow-sm bg-primary text-white">
                    <div class="d-flex align-items-center">
                        <div class="icon-box bg-white text-primary me-3">
                            <i class="bi bi-box-seam"></i>
                        </div>
                        <div>
                            <h6 class="mb-0 opacity-75">Products</h6>
                            <h3 class="mb-0 fw-bold" id="stat-products">0</h3>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="card stat-card shadow-sm bg-success text-white">
                    <div class="d-flex align-items-center">
                        <div class="icon-box bg-white text-success me-3">
                            <i class="bi bi-currency-rupee"></i>
                        </div>
                        <div>
                            <h6 class="mb-0 opacity-75">Revenue</h6>
                            <h3 class="mb-0 fw-bold" id="stat-revenue">₹0</h3>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="card stat-card shadow-sm bg-info text-white">
                    <div class="d-flex align-items-center">
                        <div class="icon-box bg-white text-info me-3">
                            <i class="bi bi-cart-check"></i>
                        </div>
                        <div>
                            <h6 class="mb-0 opacity-75">Orders</h6>
                            <h3 class="mb-0 fw-bold" id="stat-orders">0</h3>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="card stat-card shadow-sm bg-warning text-dark">
                    <div class="d-flex align-items-center">
                        <div class="icon-box bg-white text-warning me-3">
                            <i class="bi bi-people"></i>
                        </div>
                        <div>
                            <h6 class="mb-0 opacity-75">Users</h6>
                            <h3 class="mb-0 fw-bold" id="stat-users">0</h3>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row g-4 mt-2">
            <div class="col-lg-8">
                <div class="card border-0 shadow-sm rounded-4 p-4 h-100">
                    <h5 class="fw-bold mb-4">Sales Trend (Last 7 Days)</h5>
                    <canvas id="salesChart" height="300"></canvas>
                </div>
            </div>
            <div class="col-lg-4">
                <div class="card border-0 shadow-sm rounded-4 p-4 h-100">
                    <h5 class="fw-bold mb-4">Sales by Category</h5>
                    <div style="height: 300px; display: flex; align-items: center; justify-content: center;">
                        <canvas id="categoryChart"></canvas>
                    </div>
                </div>
            </div>
        </div>

        <div class="row g-4 mt-2">
            <div class="col-lg-6">
                <div class="card border-0 shadow-sm rounded-4 p-4 h-100">
                    <div class="d-flex justify-content-between align-items-center mb-4">
                        <h5 class="fw-bold mb-0">Recent Orders</h5>
                        <a href="#orders" class="btn btn-link text-primary p-0 text-decoration-none small fw-bold">View All</a>
                    </div>
                    <div class="table-responsive">
                        <table class="table table-hover align-middle mb-0">
                            <thead>
                                <tr class="text-muted small">
                                    <th>Order ID</th>
                                    <th>Customer</th>
                                    <th class="text-end">Amount</th>
                                </tr>
                            </thead>
                            <tbody id="recentOrdersBody">
                                <!-- Dynamic -->
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <div class="col-lg-6">
                <div class="card border-0 shadow-sm rounded-4 p-4 h-100">
                    <h5 class="fw-bold mb-4">Top Selling Products</h5>
                    <div class="table-responsive">
                        <table class="table table-hover align-middle mb-0">
                            <thead>
                                <tr class="text-muted small">
                                    <th>Product</th>
                                    <th class="text-end">Sold</th>
                                </tr>
                            </thead>
                            <tbody id="topProductsBody">
                                <!-- Dynamic -->
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    `;
}

async function initDashboardCharts() {
    const result = await apiFetch("/api/admin/stats");
    if (!result || !result.success) return;

    const stats = result.data;

    // Update Stats
    document.getElementById("stat-products").textContent = stats.productCount;
    document.getElementById("stat-revenue").textContent = "₹" + stats.totalRevenue.toLocaleString();
    document.getElementById("stat-orders").textContent = stats.orderCount;
    document.getElementById("stat-users").textContent = stats.userCount;

    // Sales Trend Chart
    const salesCtx = document.getElementById('salesChart').getContext('2d');
    new Chart(salesCtx, {
        type: 'line',
        data: {
            labels: stats.salesTrend.map(d => d.date),
            datasets: [{
                label: 'Revenue',
                data: stats.salesTrend.map(d => d.sales),
                borderColor: '#0d6efd',
                backgroundColor: 'rgba(13, 110, 253, 0.1)',
                fill: true,
                tension: 0.4
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: { legend: { display: false } },
            scales: { y: { beginAtZero: true } }
        }
    });

    // Category Chart
    const categoryCtx = document.getElementById('categoryChart').getContext('2d');
    new Chart(categoryCtx, {
        type: 'doughnut',
        data: {
            labels: stats.categorySales.map(d => d.category),
            datasets: [{
                data: stats.categorySales.map(d => d.sales),
                backgroundColor: ['#0d6efd', '#198754', '#0dcaf0', '#ffc107', '#6f42c1']
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: { legend: { position: 'bottom' } },
            cutout: '70%'
        }
    });

    // Recent Orders
    const recentOrdersBody = document.getElementById("recentOrdersBody");
    recentOrdersBody.innerHTML = stats.recentOrders.map(o => `
        <tr>
            <td class="fw-bold text-primary">#${o.orderId}</td>
            <td>${o.user?.firstName || 'Guest'}</td>
            <td class="text-end fw-bold">₹${o.totalAmount?.toLocaleString()}</td>
        </tr>
    `).join('');

    // Top Products
    const topProductsBody = document.getElementById("topProductsBody");
    topProductsBody.innerHTML = stats.topProducts.map(p => `
        <tr>
            <td><div class="fw-bold">${p.name}</div></td>
            <td class="text-end"><span class="badge bg-primary-subtle text-primary rounded-pill">${p.quantity} sold</span></td>
        </tr>
    `).join('');
}
