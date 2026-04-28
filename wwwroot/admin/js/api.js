/**
 * Global Fetch Wrapper for Admin API
 * Handles authentication headers, 401 redirects, and standardized responses.
 */

async function apiFetch(url, options = {}) {
    const token = localStorage.getItem("admin_token");
    
    // Merge headers
    const headers = {
        'Accept': 'application/json',
        ...options.headers
    };

    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }

    // Default to JSON if body is provided and not FormData
    if (options.body && !(options.body instanceof FormData)) {
        headers['Content-Type'] = 'application/json';
        if (typeof options.body !== 'string') {
            options.body = JSON.stringify(options.body);
        }
    }

    try {
        const response = await fetch(url, { ...options, headers });

        if (response.status === 401) {
            console.warn("Unauthorized! Redirecting to login...");
            localStorage.removeItem("admin_token");
            window.location.href = "/Users/Login?returnUrl=" + encodeURIComponent(window.location.pathname + window.location.search + window.location.hash);
            return null;
        }

        const result = await response.json();

        if (!result.success && result.message) {
            if (typeof showToast === 'function') {
                showToast(result.message, "error");
            } else {
                console.error("API Error:", result.message);
            }
        }

        return result;
    } catch (error) {
        console.error("Fetch Error:", error);
        if (typeof showToast === 'function') {
            showToast("Network error or server is down", "error");
        }
        return { success: false, message: "Network error" };
    }
}

// Utility to show global loader
function showLoader() {
    const loader = document.getElementById('global-loader');
    if (loader) loader.style.display = 'flex';
}

function hideLoader() {
    const loader = document.getElementById('global-loader');
    if (loader) loader.style.display = 'none';
}
