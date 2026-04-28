/**
 * Users Module
 */

async function renderUsers() {
    return `
        <div class="card border-0 shadow-sm rounded-4 p-4">
            <h5 class="fw-bold mb-4">User Accounts</h5>
            <div class="table-responsive">
                <table class="table table-hover align-middle">
                    <thead class="bg-light">
                        <tr>
                            <th>User</th>
                            <th>Email</th>
                            <th>Role</th>
                            <th class="text-end">Actions</th>
                        </tr>
                    </thead>
                    <tbody id="usersTableBody">
                        <!-- Dynamic -->
                    </tbody>
                </table>
            </div>
        </div>
    `;
}

async function initUsers() {
    showLoader();
    const result = await apiFetch("/api/admin/users");
    hideLoader();

    if (!result || !result.success) return;

    const body = document.getElementById("usersTableBody");
    body.innerHTML = result.data.map(u => `
        <tr>
            <td>
                <div class="d-flex align-items-center">
                    <div class="avatar bg-light text-primary rounded-circle me-3">${u.firstName[0]}${u.lastName[0]}</div>
                    <div class="fw-bold">${u.firstName} ${u.lastName}</div>
                </div>
            </td>
            <td>${u.email}</td>
            <td><span class="badge ${u.role === 'Admin' ? 'bg-primary' : 'bg-secondary'} rounded-pill">${u.role}</span></td>
            <td class="text-end">
                <button class="btn btn-sm btn-outline-danger rounded-circle" onclick="deleteUser(${u.userId})"><i class="bi bi-trash"></i></button>
            </td>
        </tr>
    `).join('');
}

window.deleteUser = async (id) => {
    if (confirm("Are you sure you want to delete this user?")) {
        showLoader();
        const result = await apiFetch(`/api/admin/users/${id}`, { method: "DELETE" });
        hideLoader();
        if (result && result.success) {
            showToast("User deleted");
            initUsers();
        }
    }
};
