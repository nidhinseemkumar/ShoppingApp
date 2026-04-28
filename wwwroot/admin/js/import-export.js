/**
 * Import / Export Module
 */

async function renderImportProducts() {
    return `
        <div class="row justify-content-center">
            <div class="col-lg-6">
                <div class="card border-0 shadow-sm rounded-4 p-4">
                    <div class="text-center mb-4">
                        <div class="icon-box bg-primary-subtle text-primary rounded-circle mx-auto mb-3" style="width: 64px; height: 64px; font-size: 2rem;">
                            <i class="bi bi-upload"></i>
                        </div>
                        <h4 class="fw-bold">Upload Products</h4>
                        <p class="text-muted">Upload a .csv or .xlsx file to import products in bulk.</p>
                    </div>
                    
                    <form id="importProductsForm">
                        <div class="mb-4">
                            <label class="form-label fw-bold">Select File (Max 5MB)</label>
                            <input type="file" name="file" class="form-control rounded-3" accept=".csv, .xlsx" required>
                        </div>
                        <div id="importProgress" class="progress mb-4" style="display: none; height: 10px;">
                            <div class="progress-bar progress-bar-striped progress-bar-animated" role="progressbar" style="width: 100%"></div>
                        </div>
                        <button type="submit" class="btn btn-primary w-100 rounded-pill py-2 fw-bold">
                            <i class="bi bi-cloud-upload me-2"></i>Start Import
                        </button>
                    </form>

                    <div class="mt-4 p-3 bg-light rounded-4 small">
                        <h6 class="fw-bold mb-2">Instructions:</h6>
                        <ul class="mb-0 ps-3">
                            <li>Supported formats: <strong>.csv, .xlsx</strong></li>
                            <li>Columns: Name, Price, Stock, Category, Description</li>
                            <li>Existing products with same name will be duplicated.</li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
    `;
}

function initImportProducts() {
    const form = document.getElementById("importProductsForm");
    form.addEventListener("submit", async (e) => {
        e.preventDefault();
        const formData = new FormData(form);
        const progress = document.getElementById("importProgress");
        
        progress.style.display = "flex";
        const result = await apiFetch("/api/admin/import/products", {
            method: "POST",
            body: formData
        });
        progress.style.display = "none";

        if (result && result.success) {
            showToast(result.message);
            form.reset();
        }
    });
}

async function renderExportData() {
    return `
        <div class="row g-4">
            <div class="col-md-6">
                <div class="card border-0 shadow-sm rounded-4 p-4 text-center">
                    <h5 class="fw-bold mb-3">Export Products</h5>
                    <p class="text-muted small">Download all product data in your preferred format.</p>
                    <div class="d-grid gap-2">
                        <button class="btn btn-outline-primary rounded-pill" onclick="downloadData('products', 'csv')">
                            <i class="bi bi-filetype-csv me-2"></i>CSV Format
                        </button>
                        <button class="btn btn-outline-primary rounded-pill" onclick="downloadData('products', 'excel')">
                            <i class="bi bi-file-earmark-excel me-2"></i>Excel Format
                        </button>
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="card border-0 shadow-sm rounded-4 p-4 text-center">
                    <h5 class="fw-bold mb-3">Export Users</h5>
                    <p class="text-muted small">Download user account details for offline reporting.</p>
                    <div class="d-grid gap-2">
                        <button class="btn btn-outline-success rounded-pill" onclick="downloadData('users', 'csv')">
                            <i class="bi bi-filetype-csv me-2"></i>CSV Format
                        </button>
                        <button class="btn btn-outline-success rounded-pill" onclick="downloadData('users', 'excel')">
                            <i class="bi bi-file-earmark-excel me-2"></i>Excel Format
                        </button>
                    </div>
                </div>
            </div>
        </div>
    `;
}

async function downloadData(type, format) {
    const token = localStorage.getItem("admin_token");
    const url = `/api/admin/export/${type}?format=${format}`;
    
    showLoader();
    try {
        const response = await fetch(url, {
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (response.status === 401) {
            localStorage.removeItem("admin_token");
            window.location.href = "/Users/Login";
            return;
        }

        const blob = await response.blob();
        const downloadUrl = window.URL.createObjectURL(blob);
        const a = document.createElement("a");
        a.href = downloadUrl;
        a.download = `${type}_export_${new Date().toISOString().slice(0,10)}.${format === 'excel' ? 'xlsx' : 'csv'}`;
        document.body.appendChild(a);
        a.click();
        window.URL.revokeObjectURL(downloadUrl);
        showToast("Download started!");
    } catch (error) {
        showToast("Download failed", "error");
    } finally {
        hideLoader();
    }
}

// Attach to window for onclick
window.downloadData = downloadData;
