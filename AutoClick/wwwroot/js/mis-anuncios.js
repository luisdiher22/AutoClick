// Mis Anuncios JavaScript Functionality

document.addEventListener('DOMContentLoaded', function() {
    initializeFilters();
    initializeListingActions();
    initializePaginationScroll();
});

// Filter functionality
function initializeFilters() {
    // Province/Canton cascading dropdown
    const provinciaSelect = document.getElementById('provincia');
    const cantonSelect = document.getElementById('canton');
    
    if (provinciaSelect && cantonSelect) {
        provinciaSelect.addEventListener('change', function() {
            updateCantonOptions(this.value);
        });
    }
    
    // Brand/Model cascading dropdown
    const marcaSelect = document.getElementById('marca');
    const modeloSelect = document.getElementById('modelo');
    
    if (marcaSelect && modeloSelect) {
        marcaSelect.addEventListener('change', function() {
            updateModeloOptions(this.value);
        });
    }
    
    // Auto-submit form on sort change
    const sortSelect = document.querySelector('select[name="ordenarPor"]');
    if (sortSelect) {
        sortSelect.addEventListener('change', function() {
            this.closest('form').submit();
        });
    }
    
    // Price range validation
    const precioMin = document.getElementById('precioMin');
    const precioMax = document.getElementById('precioMax');
    
    if (precioMin && precioMax) {
        precioMin.addEventListener('change', validatePriceRange);
        precioMax.addEventListener('change', validatePriceRange);
    }
    
    // Kilometraje range validation
    const kmMin = document.getElementById('kmMin');
    const kmMax = document.getElementById('kmMax');
    
    if (kmMin && kmMax) {
        kmMin.addEventListener('change', validateKilometrajeRange);
        kmMax.addEventListener('change', validateKilometrajeRange);
    }
    
    // Year range validation
    const añoMin = document.getElementById('añoMin');
    const añoMax = document.getElementById('añoMax');
    
    if (añoMin && añoMax) {
        añoMin.addEventListener('change', validateAñoRange);
        añoMax.addEventListener('change', validateAñoRange);
    }
}

// Update canton options based on selected province
function updateCantonOptions(provincia) {
    const cantonSelect = document.getElementById('canton');
    if (!cantonSelect) return;
    
    // Clear existing options except the first one
    cantonSelect.innerHTML = '<option value="">Todos los cantones</option>';
    
    const cantonesPorProvincia = {
        'San José': ['San José', 'Escazú', 'Desamparados', 'Puriscal', 'Tarrazú', 'Aserrí', 'Mora', 'Goicoechea', 'Santa Ana', 'Alajuelita', 'Coronado', 'Acosta', 'Tibás', 'Moravia', 'Montes de Oca', 'Turrubares', 'Dota', 'Curridabat', 'Pérez Zeledón', 'León Cortés Castro'],
        'Alajuela': ['Alajuela', 'San Ramón', 'Grecia', 'San Mateo', 'Atenas', 'Naranjo', 'Palmares', 'Poás', 'Orotina', 'San Carlos', 'Zarcero', 'Sarchí', 'Upala', 'Los Chiles', 'Guatuso'],
        'Cartago': ['Cartago', 'Paraíso', 'La Unión', 'Jiménez', 'Turrialba', 'Alvarado', 'Oreamuno', 'El Guarco'],
        'Heredia': ['Heredia', 'Barva', 'Santo Domingo', 'Santa Bárbara', 'San Rafael', 'San Isidro', 'Belén', 'Flores', 'San Pablo', 'Sarapiquí'],
        'Guanacaste': ['Liberia', 'Nicoya', 'Santa Cruz', 'Bagaces', 'Carrillo', 'Cañas', 'Abangares', 'Tilarán', 'Nandayure', 'La Cruz', 'Hojancha'],
        'Puntarenas': ['Puntarenas', 'Esparza', 'Buenos Aires', 'Montes de Oro', 'Osa', 'Quepos', 'Golfito', 'Coto Brus', 'Parrita', 'Corredores', 'Garabito'],
        'Limón': ['Limón', 'Pococí', 'Siquirres', 'Talamanca', 'Matina', 'Guácimo']
    };
    
    if (provincia && cantonesPorProvincia[provincia]) {
        cantonesPorProvincia[provincia].forEach(canton => {
            const option = document.createElement('option');
            option.value = canton;
            option.textContent = canton;
            cantonSelect.appendChild(option);
        });
    }
}

// Update model options based on selected brand
function updateModeloOptions(marca) {
    const modeloSelect = document.getElementById('modelo');
    if (!modeloSelect) return;
    
    // Clear existing options except the first one
    modeloSelect.innerHTML = '<option value="">Todos los modelos</option>';
    
    const modelosPorMarca = {
        'Toyota': ['Corolla', 'Camry', 'Prius', 'RAV4', 'Highlander', 'Tacoma', 'Tundra', 'Sienna', 'Avalon', 'C-HR'],
        'Honda': ['Civic', 'Accord', 'CR-V', 'Pilot', 'Odyssey', 'Fit', 'HR-V', 'Passport', 'Ridgeline', 'Insight'],
        'Nissan': ['Sentra', 'Altima', 'Maxima', 'Rogue', 'Murano', 'Pathfinder', 'Armada', 'Frontier', 'Titan', 'Kicks'],
        'Hyundai': ['Elantra', 'Sonata', 'Tucson', 'Santa Fe', 'Palisade', 'Kona', 'Venue', 'Accent', 'Veloster', 'Genesis'],
        'Mazda': ['Mazda3', 'Mazda6', 'CX-3', 'CX-5', 'CX-9', 'MX-5 Miata', 'CX-30', 'Mazda2', 'BT-50'],
        'BMW': ['Serie 1', 'Serie 2', 'Serie 3', 'Serie 4', 'Serie 5', 'Serie 6', 'Serie 7', 'X1', 'X2', 'X3', 'X4', 'X5', 'X6', 'X7'],
        'Mercedes-Benz': ['Clase A', 'Clase B', 'Clase C', 'Clase E', 'Clase S', 'GLA', 'GLB', 'GLC', 'GLE', 'GLS'],
        'Audi': ['A1', 'A3', 'A4', 'A5', 'A6', 'A7', 'A8', 'Q2', 'Q3', 'Q5', 'Q7', 'Q8', 'TT', 'R8']
    };
    
    if (marca && modelosPorMarca[marca]) {
        modelosPorMarca[marca].forEach(modelo => {
            const option = document.createElement('option');
            option.value = modelo;
            option.textContent = modelo;
            modeloSelect.appendChild(option);
        });
    }
}

// Validate price range
function validatePriceRange() {
    const precioMin = document.getElementById('precioMin');
    const precioMax = document.getElementById('precioMax');
    
    if (precioMin && precioMax && precioMin.value && precioMax.value) {
        const min = parseFloat(precioMin.value);
        const max = parseFloat(precioMax.value);
        
        if (min > max) {
            showValidationMessage('El precio mínimo no puede ser mayor al precio máximo');
            precioMin.style.borderColor = '#EF4444';
            precioMax.style.borderColor = '#EF4444';
        } else {
            precioMin.style.borderColor = '';
            precioMax.style.borderColor = '';
        }
    }
}

// Validate kilometraje range
function validateKilometrajeRange() {
    const kmMin = document.getElementById('kmMin');
    const kmMax = document.getElementById('kmMax');
    
    if (kmMin && kmMax && kmMin.value && kmMax.value) {
        const min = parseInt(kmMin.value);
        const max = parseInt(kmMax.value);
        
        if (min > max) {
            showValidationMessage('El kilometraje mínimo no puede ser mayor al kilometraje máximo');
            kmMin.style.borderColor = '#EF4444';
            kmMax.style.borderColor = '#EF4444';
        } else {
            kmMin.style.borderColor = '';
            kmMax.style.borderColor = '';
        }
    }
}

// Validate año range
function validateAñoRange() {
    const añoMin = document.getElementById('añoMin');
    const añoMax = document.getElementById('añoMax');
    
    if (añoMin && añoMax && añoMin.value && añoMax.value) {
        const min = parseInt(añoMin.value);
        const max = parseInt(añoMax.value);
        const currentYear = new Date().getFullYear();
        
        if (min > max) {
            showValidationMessage('El año mínimo no puede ser mayor al año máximo');
            añoMin.style.borderColor = '#EF4444';
            añoMax.style.borderColor = '#EF4444';
        } else if (max > currentYear + 1) {
            showValidationMessage('El año máximo no puede ser mayor a ' + (currentYear + 1));
            añoMax.style.borderColor = '#EF4444';
        } else {
            añoMin.style.borderColor = '';
            añoMax.style.borderColor = '';
        }
    }
}

// Show validation message
function showValidationMessage(message) {
    // Remove existing validation messages
    const existingMessage = document.querySelector('.validation-message');
    if (existingMessage) {
        existingMessage.remove();
    }
    
    // Create new validation message
    const messageElement = document.createElement('div');
    messageElement.className = 'validation-message';
    messageElement.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: #EF4444;
        color: white;
        padding: 12px 20px;
        border-radius: 8px;
        font-size: 14px;
        z-index: 9999;
        animation: slideInRight 0.3s ease;
    `;
    messageElement.textContent = message;
    
    document.body.appendChild(messageElement);
    
    // Remove message after 3 seconds
    setTimeout(() => {
        if (messageElement && messageElement.parentNode) {
            messageElement.remove();
        }
    }, 3000);
}

// Initialize listing actions
function initializeListingActions() {
    // Edit buttons - handled by inline onclick in HTML
    // Delete buttons - handled by inline onclick in HTML
}

// Show options menu
function showOptionsMenu(button, listingId) {
    // Remove existing menus
    document.querySelectorAll('.options-menu').forEach(menu => menu.remove());
    
    const menu = document.createElement('div');
    menu.className = 'options-menu';
    menu.style.cssText = `
        position: absolute;
        top: 100%;
        right: 0;
        background: rgba(255, 255, 255, 0.95);
        border-radius: 8px;
        padding: 8px 0;
        box-shadow: 0 8px 25px rgba(0, 0, 0, 0.3);
        z-index: 1000;
        min-width: 150px;
    `;
    
    const options = [
        { text: 'Editar anuncio', action: () => editListing(listingId) },
        { text: 'Ver estadísticas', action: () => viewStats(listingId) },
        { text: 'Pausar anuncio', action: () => pauseListing(listingId) },
        { text: 'Renovar anuncio', action: () => renewListing(listingId) },
        { text: 'Marcar como vendido', action: () => markAsSold(listingId) }
    ];
    
    options.forEach(option => {
        const menuItem = document.createElement('div');
        menuItem.style.cssText = `
            padding: 8px 16px;
            cursor: pointer;
            color: #333;
            font-size: 14px;
            transition: background-color 0.2s;
        `;
        menuItem.textContent = option.text;
        menuItem.addEventListener('click', option.action);
        menuItem.addEventListener('mouseenter', () => {
            menuItem.style.backgroundColor = 'rgba(74, 144, 226, 0.1)';
        });
        menuItem.addEventListener('mouseleave', () => {
            menuItem.style.backgroundColor = '';
        });
        menu.appendChild(menuItem);
    });
    
    button.parentElement.style.position = 'relative';
    button.parentElement.appendChild(menu);
    
    // Close menu when clicking outside
    setTimeout(() => {
        document.addEventListener('click', function closeMenu(e) {
            if (!menu.contains(e.target) && e.target !== button) {
                menu.remove();
                document.removeEventListener('click', closeMenu);
            }
        });
    }, 100);
}

// Confirm delete listing
function confirmDeleteListing(buttonElement, listingId, listingTitle) {
    const modal = document.createElement('div');
    modal.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        background: rgba(0, 0, 0, 0.7);
        display: flex;
        align-items: center;
        justify-content: center;
        z-index: 10000;
    `;
    
    const modalContent = document.createElement('div');
    modalContent.style.cssText = `
        background: #030D2B;
        padding: 30px;
        border-radius: 12px;
        max-width: 400px;
        text-align: center;
        color: white;
    `;
    
    modalContent.innerHTML = `
        <h3 style="margin-bottom: 16px; color: #EF4444;">Confirmar eliminación</h3>
        <p style="margin-bottom: 24px; color: #8A92B2;">¿Estás seguro que deseas eliminar "${listingTitle}"?</p>
        <div style="display: flex; gap: 12px; justify-content: center;">
            <button class="cancel-btn" style="padding: 10px 20px; background: rgba(255,255,255,0.1); color: white; border: none; border-radius: 6px; cursor: pointer;">Cancelar</button>
            <button class="delete-confirm-btn" style="padding: 10px 20px; background: #EF4444; color: white; border: none; border-radius: 6px; cursor: pointer;">Eliminar</button>
        </div>
    `;
    
    modal.appendChild(modalContent);
    document.body.appendChild(modal);
    
    modalContent.querySelector('.cancel-btn').addEventListener('click', () => {
        document.body.removeChild(modal);
    });
    
    modalContent.querySelector('.delete-confirm-btn').addEventListener('click', () => {
        deleteListing(listingId, buttonElement, modal);
    });
}

// Delete listing via API
async function deleteListing(listingId, buttonElement, modal) {
    const deleteButton = modal.querySelector('.delete-confirm-btn');
    const originalText = deleteButton.textContent;
    
    try {
        // Show loading state
        deleteButton.textContent = 'Eliminando...';
        deleteButton.disabled = true;
        
        // Make API call to delete the listing
        console.log('Sending delete request for ID:', listingId);
        console.log('Current URL:', window.location.href);
        
        // Use default POST method with action parameter
        const url = window.location.pathname;
        console.log('Delete URL:', url);
        
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: `action=delete&id=${listingId}`
        });
        
        console.log('Response status:', response.status);
        console.log('Response headers:', response.headers.get('content-type'));
        
        if (!response.ok) {
            console.error('Response not OK:', response.status, response.statusText);
            const responseText = await response.text();
            console.error('Response body:', responseText.substring(0, 500));
            throw new Error(`Server error: ${response.status}`);
        }
        
        const responseText = await response.text();
        console.log('Raw response:', responseText.substring(0, 200));
        
        let result;
        try {
            result = JSON.parse(responseText);
        } catch (parseError) {
            console.error('JSON parse error:', parseError);
            console.error('Response was:', responseText.substring(0, 500));
            throw new Error('Invalid JSON response from server');
        }
        
        if (result.success) {
            // Animate card removal
            const listingCard = buttonElement.closest('.listing-card');
            listingCard.style.animation = 'fadeOut 0.3s ease';
            
            // Show success message
            showNotificationMessage('Anuncio eliminado exitosamente', 'success');
            
            setTimeout(() => {
                listingCard.remove();
                document.body.removeChild(modal);
                
                // Check if no more listings and show empty state if needed
                const remainingCards = document.querySelectorAll('.listing-card');
                if (remainingCards.length === 0) {
                    const listingsGrid = document.querySelector('.listings-grid');
                    listingsGrid.innerHTML = `
                        <div class="no-listings">
                            <h3>No tienes anuncios publicados</h3>
                            <p>¡Comienza a vender tu vehículo ahora!</p>
                            <a href="/anunciar-mi-auto" class="btn-primary">Publicar Anuncio</a>
                        </div>
                    `;
                }
            }, 300);
        } else {
            showNotificationMessage(result.message || 'Error al eliminar el anuncio', 'error');
            deleteButton.textContent = originalText;
            deleteButton.disabled = false;
        }
    } catch (error) {
        console.error('Error deleting listing:', error);
        showNotificationMessage('Error de conexión al eliminar el anuncio', 'error');
        deleteButton.textContent = originalText;
        deleteButton.disabled = false;
    }
}

// Show notification message
function showNotificationMessage(message, type = 'info') {
    // Remove existing messages
    const existingMessage = document.querySelector('.notification-message');
    if (existingMessage) {
        existingMessage.remove();
    }
    
    const backgroundColor = type === 'success' ? '#22C55E' : (type === 'error' ? '#EF4444' : '#4A90E2');
    
    // Create new notification message
    const messageElement = document.createElement('div');
    messageElement.className = 'notification-message';
    messageElement.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: ${backgroundColor};
        color: white;
        padding: 12px 20px;
        border-radius: 8px;
        font-size: 14px;
        z-index: 9999;
        animation: slideInRight 0.3s ease;
        max-width: 300px;
    `;
    messageElement.textContent = message;
    
    document.body.appendChild(messageElement);
    
    // Remove message after 4 seconds
    setTimeout(() => {
        if (messageElement && messageElement.parentNode) {
            messageElement.style.animation = 'slideOutRight 0.3s ease';
            setTimeout(() => {
                if (messageElement && messageElement.parentNode) {
                    messageElement.remove();
                }
            }, 300);
        }
    }, 4000);
}

// Listing action functions (placeholders for actual implementations)
function editListing(id) {
    console.log('=== Edit Listing Function Called ===');
    console.log('ID received:', id);
    console.log('ID type:', typeof id);
    
    if (!id) {
        console.error('No ID provided for editing');
        return;
    }
    
    const url = `/AnunciarMiAuto?edit=${id}`;
    console.log('Generated URL:', url);
    console.log('About to redirect...');
    
    // Add a small delay to see if the logs appear
    setTimeout(() => {
        console.log('Executing redirect now...');
        window.location.href = url;
    }, 100);
}

function viewStats(id) {
    console.log('View stats:', id);
    // Implement stats modal or page
}

function pauseListing(id) {
    console.log('Pause listing:', id);
    // Implement pause functionality
}

function renewListing(id) {
    console.log('Renew listing:', id);
    // Implement renewal functionality
}

function markAsSold(id) {
    console.log('Mark as sold:', id);
    // Implement sold marking functionality
}

// Initialize pagination scroll
function initializePaginationScroll() {
    const paginationLinks = document.querySelectorAll('.pagination a');
    paginationLinks.forEach(link => {
        link.addEventListener('click', function() {
            // Scroll to top of listings when changing page
            const listingsGrid = document.querySelector('.listings-grid');
            if (listingsGrid) {
                listingsGrid.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }
        });
    });
}

// Add CSS animations
const style = document.createElement('style');
style.textContent = `
    @keyframes slideInRight {
        from {
            transform: translateX(100%);
            opacity: 0;
        }
        to {
            transform: translateX(0);
            opacity: 1;
        }
    }
    
    @keyframes slideOutRight {
        from {
            transform: translateX(0);
            opacity: 1;
        }
        to {
            transform: translateX(100%);
            opacity: 0;
        }
    }
    
    @keyframes fadeOut {
        from {
            opacity: 1;
            transform: scale(1);
        }
        to {
            opacity: 0;
            transform: scale(0.9);
        }
    }
`;
document.head.appendChild(style);