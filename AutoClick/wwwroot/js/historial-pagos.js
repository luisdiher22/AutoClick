// Historial de Pagos JavaScript Functionality

document.addEventListener('DOMContentLoaded', function() {
    // Initialize components
    initSidebarNavigation();
    initPaymentsTable();
    initNewsletterForm();
    initStatusMessages();
    
    console.log('Historial de Pagos page initialized successfully');
});

// Sidebar Navigation
function initSidebarNavigation() {
    const indicator = document.querySelector('.sidebar-indicator');
    const activeItem = document.querySelector('.nav-item.active');
    
    // Position indicator on the active item
    if (indicator && activeItem) {
        const navItems = document.querySelectorAll('.nav-item');
        const activeIndex = Array.from(navItems).indexOf(activeItem);
        indicator.style.top = `${23 + (activeIndex * 56)}px`;
    }
    
    // No need for click handlers since we're using real links now
}

// Payments Table Functionality
function initPaymentsTable() {
    const table = document.querySelector('.payments-table');
    const rows = table.querySelectorAll('tbody tr');
    
    // Add hover effects to rows
    rows.forEach(row => {
        row.addEventListener('mouseenter', function() {
            this.style.backgroundColor = 'rgba(255, 255, 255, 0.05)';
        });
        
        row.addEventListener('mouseleave', function() {
            // Reset background based on even/odd
            const index = Array.from(this.parentNode.children).indexOf(this);
            this.style.backgroundColor = index % 2 === 1 ? '#02081C' : 'transparent';
        });
        
        // Add click functionality for receipts
        const receiptIcon = row.querySelector('.receipt-icon');
        if (receiptIcon) {
            receiptIcon.addEventListener('click', function(e) {
                e.preventDefault();
                e.stopPropagation();
                
                const paymentDate = row.querySelector('.payment-date').textContent;
                const paymentAmount = row.querySelector('.payment-amount').textContent;
                
                showReceiptModal(paymentDate, paymentAmount);
            });
            
            // Add cursor pointer style
            receiptIcon.style.cursor = 'pointer';
            receiptIcon.title = 'Ver comprobante de pago';
        }
    });
    
    // Add sorting functionality (basic)
    const headers = table.querySelectorAll('thead th');
    headers.forEach((header, index) => {
        header.style.cursor = 'pointer';
        header.title = 'Click para ordenar';
        
        header.addEventListener('click', function() {
            sortTable(index, this);
        });
    });
}

// Sort table functionality
function sortTable(columnIndex, header) {
    const table = document.querySelector('.payments-table tbody');
    const rows = Array.from(table.querySelectorAll('tr'));
    const isAscending = header.classList.contains('sort-desc');
    
    // Remove sort classes from all headers
    document.querySelectorAll('thead th').forEach(th => {
        th.classList.remove('sort-asc', 'sort-desc');
    });
    
    // Add appropriate sort class
    header.classList.add(isAscending ? 'sort-asc' : 'sort-desc');
    
    rows.sort((a, b) => {
        const aValue = a.children[columnIndex].textContent.trim();
        const bValue = b.children[columnIndex].textContent.trim();
        
        // Handle different data types
        if (columnIndex === 0) { // Date column
            const aDate = new Date(aValue.split('/').reverse().join('-'));
            const bDate = new Date(bValue.split('/').reverse().join('-'));
            return isAscending ? aDate - bDate : bDate - aDate;
        } else if (columnIndex === 3) { // Amount column
            const aAmount = parseFloat(aValue.replace('₡', '').replace(',', ''));
            const bAmount = parseFloat(bValue.replace('₡', '').replace(',', ''));
            return isAscending ? aAmount - bAmount : bAmount - aAmount;
        } else { // Text columns
            return isAscending ? aValue.localeCompare(bValue) : bValue.localeCompare(aValue);
        }
    });
    
    // Re-append sorted rows
    rows.forEach(row => table.appendChild(row));
    
    showStatusMessage(`Tabla ordenada por ${header.textContent}`, 'success');
}

// Show receipt modal
function showReceiptModal(date, amount) {
    // Create modal overlay
    const overlay = document.createElement('div');
    overlay.className = 'receipt-modal-overlay';
    overlay.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: rgba(0, 0, 0, 0.8);
        display: flex;
        justify-content: center;
        align-items: center;
        z-index: 10000;
    `;
    
    // Create modal content
    const modal = document.createElement('div');
    modal.className = 'receipt-modal';
    modal.style.cssText = `
        background: #030D2B;
        border-radius: 8px;
        border: 1px solid rgba(255, 255, 255, 0.3);
        padding: 30px;
        max-width: 400px;
        width: 90%;
        color: white;
        font-family: 'Montserrat', sans-serif;
    `;
    
    modal.innerHTML = `
        <div class="receipt-header" style="text-align: center; margin-bottom: 20px;">
            <h3 style="margin: 0; color: white; font-weight: 700;">Comprobante de Pago</h3>
        </div>
        <div class="receipt-details" style="margin-bottom: 20px;">
            <div style="margin-bottom: 10px;">
                <strong>Fecha:</strong> ${date}
            </div>
            <div style="margin-bottom: 10px;">
                <strong>Monto:</strong> ${amount}
            </div>
            <div style="margin-bottom: 10px;">
                <strong>Servicio:</strong> Posteo Instantáneo
            </div>
            <div style="margin-bottom: 10px;">
                <strong>Estado:</strong> <span style="color: #00CC00;">Aprobado</span>
            </div>
            <div style="margin-bottom: 10px;">
                <strong>ID Transacción:</strong> TXN${Math.random().toString(36).substr(2, 9).toUpperCase()}
            </div>
        </div>
        <div class="receipt-actions" style="display: flex; gap: 10px; justify-content: center;">
            <button class="btn-download" style="
                background: #4A90E2;
                color: white;
                border: none;
                padding: 10px 20px;
                border-radius: 4px;
                cursor: pointer;
                font-family: 'Montserrat', sans-serif;
                font-weight: 600;
            ">Descargar PDF</button>
            <button class="btn-close" style="
                background: transparent;
                color: white;
                border: 1px solid rgba(255, 255, 255, 0.3);
                padding: 10px 20px;
                border-radius: 4px;
                cursor: pointer;
                font-family: 'Montserrat', sans-serif;
                font-weight: 600;
            ">Cerrar</button>
        </div>
    `;
    
    overlay.appendChild(modal);
    document.body.appendChild(overlay);
    
    // Add event listeners
    modal.querySelector('.btn-close').addEventListener('click', () => {
        document.body.removeChild(overlay);
    });
    
    modal.querySelector('.btn-download').addEventListener('click', () => {
        downloadReceipt(date, amount);
    });
    
    overlay.addEventListener('click', (e) => {
        if (e.target === overlay) {
            document.body.removeChild(overlay);
        }
    });
    
    // ESC key to close
    const handleEscape = (e) => {
        if (e.key === 'Escape') {
            document.body.removeChild(overlay);
            document.removeEventListener('keydown', handleEscape);
        }
    };
    document.addEventListener('keydown', handleEscape);
}

// Download receipt functionality
function downloadReceipt(date, amount) {
    // Simulate receipt download
    showStatusMessage('Descargando comprobante...', 'info');
    
    setTimeout(() => {
        // Create a simple text receipt for demonstration
        const receiptContent = `
AUTOCLICK.CR - COMPROBANTE DE PAGO
==================================

Fecha: ${date}
Monto: ${amount}
Servicio: Posteo Instantáneo
Estado: Aprobado
ID Transacción: TXN${Math.random().toString(36).substr(2, 9).toUpperCase()}

Gracias por su pago.
AutoClick.cr - Tu plataforma de autos de confianza
        `;
        
        const blob = new Blob([receiptContent], { type: 'text/plain' });
        const url = URL.createObjectURL(blob);
        
        const a = document.createElement('a');
        a.href = url;
        a.download = `comprobante-${date.replace(/\//g, '-')}.txt`;
        a.click();
        
        URL.revokeObjectURL(url);
        showStatusMessage('Comprobante descargado exitosamente', 'success');
    }, 1000);
}

// Newsletter Form
function initNewsletterForm() {
    const newsletterForm = document.querySelector('.newsletter-form');
    const newsletterInput = document.querySelector('.newsletter-input');
    const newsletterBtn = document.querySelector('.newsletter-btn');
    
    if (newsletterForm && newsletterInput && newsletterBtn) {
        newsletterForm.addEventListener('submit', function(e) {
            e.preventDefault();
            
            const email = newsletterInput.value.trim();
            
            if (!email) {
                showStatusMessage('Por favor ingresa tu correo electrónico', 'error');
                return;
            }
            
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(email)) {
                showStatusMessage('Por favor ingresa un correo electrónico válido', 'error');
                return;
            }
            
            // Simulate newsletter subscription
            newsletterBtn.textContent = 'Suscribiendo...';
            newsletterBtn.disabled = true;
            
            setTimeout(() => {
                showStatusMessage('¡Te has suscrito exitosamente al newsletter!', 'success');
                newsletterInput.value = '';
                newsletterBtn.textContent = 'Suscribirse';
                newsletterBtn.disabled = false;
            }, 1500);
        });
    }
}

// Status Messages
function initStatusMessages() {
    // Check for TempData messages from server
    const urlParams = new URLSearchParams(window.location.search);
    if (urlParams.get('newsletter') === 'success') {
        showStatusMessage('¡Te has suscrito exitosamente al newsletter!', 'success');
    }
}

function showStatusMessage(message, type = 'success') {
    // Remove existing messages
    const existingMessage = document.querySelector('.status-message');
    if (existingMessage) {
        existingMessage.remove();
    }
    
    // Create new message
    const messageDiv = document.createElement('div');
    messageDiv.className = `status-message ${type}`;
    messageDiv.textContent = message;
    
    document.body.appendChild(messageDiv);
    
    // Auto-remove after 3 seconds
    setTimeout(() => {
        if (messageDiv.parentNode) {
            messageDiv.remove();
        }
    }, 3000);
}

// Filter payments functionality
function initPaymentFilters() {
    // This could be expanded to add filtering by date, status, etc.
    console.log('Payment filters initialized');
}

// Export payments functionality
function exportPayments() {
    const table = document.querySelector('.payments-table');
    const rows = table.querySelectorAll('tbody tr');
    
    let csvContent = 'Fecha,Detalle,Método de pago,Monto,Estado\n';
    
    rows.forEach(row => {
        const cells = row.querySelectorAll('td');
        const rowData = [
            cells[0].textContent.trim(),
            cells[1].textContent.trim(),
            cells[2].textContent.trim(),
            cells[3].textContent.trim(),
            cells[4].querySelector('.status-badge').textContent.trim()
        ];
        csvContent += rowData.join(',') + '\n';
    });
    
    const blob = new Blob([csvContent], { type: 'text/csv' });
    const url = URL.createObjectURL(blob);
    
    const a = document.createElement('a');
    a.href = url;
    a.download = `historial-pagos-${new Date().toISOString().split('T')[0]}.csv`;
    a.click();
    
    URL.revokeObjectURL(url);
    showStatusMessage('Historial exportado exitosamente', 'success');
}

// Keyboard shortcuts
document.addEventListener('keydown', function(e) {
    // Ctrl+E to export
    if (e.ctrlKey && e.key === 'e') {
        e.preventDefault();
        exportPayments();
    }
    
    // Escape to close any open modals
    if (e.key === 'Escape') {
        const modal = document.querySelector('.receipt-modal-overlay');
        if (modal) {
            modal.remove();
        }
    }
});

// Print functionality
function printPaymentHistory() {
    const printContent = document.querySelector('.payments-content').cloneNode(true);
    const printWindow = window.open('', '_blank');
    
    printWindow.document.write(`
        <html>
            <head>
                <title>Historial de Pagos - AutoClick.cr</title>
                <style>
                    body { font-family: 'Montserrat', sans-serif; }
                    .payments-table { border-collapse: collapse; width: 100%; }
                    .payments-table th, .payments-table td { 
                        border: 1px solid #ccc; 
                        padding: 8px; 
                        text-align: left; 
                    }
                    .status-badge { 
                        padding: 4px 8px; 
                        border-radius: 20px; 
                        font-weight: bold; 
                    }
                    .approved { background: #00CC00; color: white; }
                    .help-section { display: none; }
                </style>
            </head>
            <body>
                <h1>Historial de Pagos - AutoClick.cr</h1>
                ${printContent.outerHTML}
            </body>
        </html>
    `);
    
    printWindow.document.close();
    printWindow.print();
}

// Search functionality
function initPaymentSearch() {
    // Create search input (could be added to the UI)
    const searchInput = document.createElement('input');
    searchInput.type = 'text';
    searchInput.placeholder = 'Buscar pagos...';
    searchInput.className = 'payment-search';
    
    searchInput.addEventListener('input', function() {
        const searchTerm = this.value.toLowerCase();
        const rows = document.querySelectorAll('.payments-table tbody tr');
        
        rows.forEach(row => {
            const text = row.textContent.toLowerCase();
            if (text.includes(searchTerm)) {
                row.style.display = '';
            } else {
                row.style.display = 'none';
            }
        });
    });
}

// Utility functions
function formatCurrency(amount) {
    return new Intl.NumberFormat('es-CR', {
        style: 'currency',
        currency: 'CRC'
    }).format(amount);
}

function formatDate(date) {
    return new Date(date).toLocaleDateString('es-CR');
}

// Initialize additional features on load
window.addEventListener('load', function() {
    initPaymentFilters();
    initPaymentSearch();
});