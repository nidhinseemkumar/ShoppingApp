// AJAX Cart Updates
async function updateCart(productId, change, element) {
    const url = `/Carts/UpdateQuantity?productId=${productId}&change=${change}`;
    try {
        const response = await fetch(url, {
            headers: {
                'X-Requested-With': 'XMLHttpRequest'
            }
        });
        const data = await response.json();
        
        if (data.success) {
            // Update the quantity display if it's on the cart page
            const quantitySpan = element.parentElement.querySelector('.cart-quantity');
            if (quantitySpan) {
                quantitySpan.textContent = data.newQuantity;
                if (data.newQuantity === 0) {
                    location.reload(); // Reload if item removed from cart page
                }
            } else {
                location.reload(); // Simple fallback for home/list pages to update button state
            }
            
            // Update navbar cart badge
            updateCartBadge();
        } else if (data.redirect) {
            window.location.href = data.redirect;
        }
    } catch (error) {
        console.error('Error updating cart:', error);
    }
}

async function updateCartBadge() {
    // This could be an AJAX call to get the count, but for now we reload
    // or just increment locally. A simple reload of the navbar part would be better.
    // For simplicity, we'll reload the page if it's a critical state change.
}

// Global Image Fallback
function handleImageError(img) {
    const productName = img.alt || 'Product';
    img.onerror = null; // Prevent infinite loop
    img.src = `https://via.placeholder.com/400x400?text=${encodeURIComponent(productName)}`;
}

async function quickUpdateStock(productId, stock) {
    try {
        const response = await fetch('/Products/UpdateStock', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            },
            body: `productId=${productId}&stock=${stock}`
        });
        const result = await response.json();
        if (result.success) {
            console.log('Stock updated');
        }
    } catch (error) {
        console.error('Error updating stock:', error);
    }
}

async function quickUpdatePrice(productId, price) {
    try {
        const response = await fetch('/Products/UpdatePrice', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
            },
            body: `productId=${productId}&price=${price}`
        });
        const result = await response.json();
        if (result.success) {
            console.log('Price updated');
        }
    } catch (error) {
        console.error('Error updating price:', error);
    }
}

// Search Suggestions
$(document).ready(function() {
    const $searchInput = $('#mainSearchInput');
    const $suggestions = $('#searchSuggestions');

    $searchInput.on('input', async function() {
        const term = $(this).val().trim();
        if (term.length < 2) {
            $suggestions.hide();
            return;
        }

        try {
            const response = await fetch(`/Products/GetSuggestions?term=${encodeURIComponent(term)}`);
            const data = await response.json();

            if (data.length > 0) {
                $suggestions.empty();
                data.forEach(item => {
                    $suggestions.append(`
                        <button type="button" class="dropdown-item rounded-3 py-2 suggestion-item">
                            <i class="bi bi-search me-2 text-muted small"></i>${item}
                        </button>
                    `);
                });
                $suggestions.show();
            } else {
                $suggestions.hide();
            }
        } catch (error) {
            console.error('Error fetching suggestions:', error);
        }
    });

    $(document).on('click', '.suggestion-item', function() {
        const text = $(this).text().trim();
        $searchInput.val(text);
        $searchInput.closest('form').submit();
    });

    $(document).on('click', function(e) {
        if (!$(e.target).closest('.search-container').length) {
            $suggestions.hide();
        }
    });
});
