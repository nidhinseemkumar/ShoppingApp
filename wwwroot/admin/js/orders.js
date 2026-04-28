/**
 * Orders Module
 */

async function renderOrders() {
    return `
        <div class="card border-0 shadow-sm rounded-4 p-4">
            <h5 class="fw-bold mb-4">Order History</h5>
            <div class="table-responsive">
                <table class="table table-hover align-middle">
                    <thead class="bg-light">
                        <tr>
                            <th>Order ID</th>
                            <th>Customer</th>
                            <th>Date</th>
                            <th>Status</th>
                            <th class="text-end">Amount</th>
                            <th class="text-end">Actions</th>
                        </tr>
                    </thead>
                    <tbody id="ordersTableBody">
                        <!-- Dynamic -->
                    </tbody>
                </table>
            </div>
        </div>
    `;
}

async function initOrders() {
    showLoader();
    const result = await apiFetch("/api/admin/orders");
    hideLoader();

    if (!result || !result.success) return;

    const body = document.getElementById("ordersTableBody");
    body.innerHTML = result.data.map(o => `
        <tr>
            <td class="fw-bold text-primary">#${o.orderId}</td>
            <td>${o.user?.firstName || 'Guest'}</td>
            <td class="small text-muted">${new Date(o.orderDate).toLocaleDateString()}</td>
            <td>
                <select class="form-select form-select-sm rounded-pill border-0 bg-light" style="width: 130px;" onchange="updateOrderStatus(${o.orderId}, this.value)">
                    <option value="Pending" ${o.status === 'Pending' ? 'selected' : ''}>Pending</option>
                    <option value="Shipped" ${o.status === 'Shipped' ? 'selected' : ''}>Shipped</option>
                    <option value="Delivered" ${o.status === 'Delivered' ? 'selected' : ''}>Delivered</option>
                    <option value="Cancelled" ${o.status === 'Cancelled' ? 'selected' : ''}>Cancelled</option>
                </select>
            </td>
            <td class="text-end fw-bold">₹${o.totalAmount?.toLocaleString()}</td>
            <td class="text-end">
                <button class="btn btn-sm btn-outline-primary rounded-circle" onclick="viewOrderDetails(${o.orderId})"><i class="bi bi-eye"></i></button>
            </td>
        </tr>
    `).join('');
}

window.updateOrderStatus = async (id, status) => {
    showLoader();
    const result = await apiFetch(`/api/admin/orders/${id}/status`, {
        method: "PUT",
        body: JSON.stringify(status)
    });
    hideLoader();
    if (result && result.success) {
        showToast("Order status updated");
    }
};

window.viewOrderDetails = async (id) => {
    showToast("Order details view coming soon!");
};
