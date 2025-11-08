// Mi Perfil JavaScript Functionality

document.addEventListener('DOMContentLoaded', function() {
    // Initialize components
    initSidebarNavigation();
    initFileUploads();
    initFormValidation();
    initAddressDropdowns();
    initProfileForm();
    initAutoResizeTextareas();
    
    console.log('Mi Perfil page initialized successfully');
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

// File Upload Handling
function initFileUploads() {
    // Logo upload
    const logoArea = document.getElementById('logoUpload1');
    const logoInput = document.getElementById('logoFile1');
    
    if (logoArea && logoInput) {
        // Click handler for logo area
        logoArea.addEventListener('click', function() {
            logoInput.click();
        });
        
        // File change handler
        logoInput.addEventListener('change', function(e) {
            const file = e.target.files[0];
            if (file) {
                handleImageUpload(file, logoArea, 'logo');
            }
        });
        
        // Drag and drop functionality
        logoArea.addEventListener('dragover', function(e) {
            e.preventDefault();
            logoArea.classList.add('dragover');
        });
        
        logoArea.addEventListener('dragleave', function() {
            logoArea.classList.remove('dragover');
        });
        
        logoArea.addEventListener('drop', function(e) {
            e.preventDefault();
            logoArea.classList.remove('dragover');
            
            const file = e.dataTransfer.files[0];
            if (file && file.type.startsWith('image/')) {
                handleImageUpload(file, logoArea, 'logo');
                // Update input files
                const dataTransfer = new DataTransfer();
                dataTransfer.items.add(file);
                logoInput.files = dataTransfer.files;
            }
        });
    }
}

// Handle image upload and preview
function handleImageUpload(file, container, type) {
    // Validate file
    if (!file.type.startsWith('image/')) {
        showStatusMessage('Por favor selecciona un archivo de imagen válido', 'error');
        return;
    }
    
    if (file.size > 5 * 1024 * 1024) { // 5MB limit
        showStatusMessage('El archivo no debe exceder 5MB', 'error');
        return;
    }
    
    // Create file reader
    const reader = new FileReader();
    reader.onload = function(e) {
        // Update logo display
        const placeholder = container.querySelector('.logo-placeholder');
        if (placeholder) {
            placeholder.innerHTML = `<img src="${e.target.result}" style="width: 100%; height: 100%; object-fit: cover; border-radius: 4px;" alt="Logo">`;
        }
        
        showStatusMessage('Imagen cargada exitosamente. Haz clic en "Guardar cambios" para guardarla.', 'success');
    };
    
    reader.onerror = function() {
        showStatusMessage('Error al cargar la imagen', 'error');
    };
    
    reader.readAsDataURL(file);
}

// Form Validation
function initFormValidation() {
    const form = document.querySelector('.profile-form');
    if (!form) {
        console.log('Profile form not found');
        return;
    }
    
    const inputs = form.querySelectorAll('input, textarea, select');
    
    // Real-time validation
    inputs.forEach(input => {
        input.addEventListener('blur', function() {
            validateField(this);
        });
        
        input.addEventListener('input', function() {
            // Clear error state on input
            clearFieldError(this);
        });
    });
    
    // Form submit validation
    form.addEventListener('submit', function(e) {
        let isValid = true;
        
        inputs.forEach(input => {
            if (!validateField(input)) {
                isValid = false;
            }
        });
        
        if (!isValid) {
            e.preventDefault();
            showStatusMessage('Por favor corrige los errores en el formulario', 'error');
            
            // Scroll to first error
            const firstError = form.querySelector('.validation-message');
            if (firstError) {
                firstError.scrollIntoView({ behavior: 'smooth', block: 'center' });
            }
        }
    });
}

// Validate individual field
function validateField(field) {
    const value = field.value.trim();
    const isRequired = field.hasAttribute('required');
    let isValid = true;
    let errorMessage = '';
    
    // Clear previous errors
    clearFieldError(field);
    
    // Required field validation
    if (isRequired && !value) {
        errorMessage = 'Este campo es obligatorio';
        isValid = false;
    }
    
    // Specific field validations
    if (value && field.type === 'email') {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(value)) {
            errorMessage = 'Ingresa un correo electrónico válido';
            isValid = false;
        }
    }
    
    if (value && field.type === 'tel') {
        const phoneRegex = /^[\d\s\-\(\)\+]+$/;
        if (!phoneRegex.test(value)) {
            errorMessage = 'Ingresa un número de teléfono válido';
            isValid = false;
        }
    }
    
    if (value && field.name === 'Website') {
        const urlRegex = /^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$/;
        if (!urlRegex.test(value)) {
            errorMessage = 'Ingresa una URL válida';
            isValid = false;
        }
    }
    
    // Show error if invalid
    if (!isValid) {
        showFieldError(field, errorMessage);
    }
    
    return isValid;
}

// Show field error
function showFieldError(field, message) {
    const formGroup = field.closest('.form-group');
    if (formGroup) {
        // Remove existing error
        const existingError = formGroup.querySelector('.validation-message');
        if (existingError) {
            existingError.remove();
        }
        
        // Add new error
        const errorDiv = document.createElement('div');
        errorDiv.className = 'validation-message';
        errorDiv.textContent = message;
        formGroup.appendChild(errorDiv);
        
        // Add error styling
        field.style.borderColor = '#FF0000';
    }
}

// Clear field error
function clearFieldError(field) {
    const formGroup = field.closest('.form-group');
    if (formGroup) {
        const errorMessage = formGroup.querySelector('.validation-message');
        if (errorMessage) {
            errorMessage.remove();
        }
        
        // Reset border color
        field.style.borderColor = 'rgba(255, 255, 255, 0.5)';
    }
}

// Address Dropdowns (Costa Rica)
function initAddressDropdowns() {
    // No cascading dropdowns needed - canton is now a text input
    // This function can be used for future address functionality if needed
}

// Profile Form Management
function initProfileForm() {
    const editBtn = document.querySelector('.edit-profile-btn');
    const form = document.querySelector('.profile-form');
    
    if (!form) {
        console.log('Profile form not found');
        return;
    }
    
    const inputs = form.querySelectorAll('input, textarea, select');
    
    let isEditing = false;
    
    if (editBtn) {
        editBtn.addEventListener('click', function() {
            isEditing = !isEditing;
            
            if (isEditing) {
                enableFormEditing(inputs, editBtn);
            } else {
                disableFormEditing(inputs, editBtn);
            }
        });
    }
    
    // Auto-save functionality (optional)
    let autoSaveTimeout;
    inputs.forEach(input => {
        input.addEventListener('input', function() {
            // Clear previous timeout
            clearTimeout(autoSaveTimeout);
            
            // Set new timeout for auto-save
            autoSaveTimeout = setTimeout(() => {
                console.log('Auto-saving form data...');
                // Here you could implement auto-save functionality
            }, 2000);
        });
    });
}

// Enable form editing
function enableFormEditing(inputs, editBtn) {
    inputs.forEach(input => {
        input.removeAttribute('readonly');
        input.removeAttribute('disabled');
    });
    
    editBtn.textContent = 'Cancelar edición';
    editBtn.style.background = 'rgba(239, 68, 68, 0.1)';
    editBtn.style.borderColor = '#EF4444';
    editBtn.style.color = '#EF4444';
    
    showStatusMessage('Modo de edición activado', 'success');
}

// Disable form editing
function disableFormEditing(inputs, editBtn) {
    inputs.forEach(input => {
        input.setAttribute('readonly', 'readonly');
    });
    
    editBtn.innerHTML = '<span class="edit-icon"></span>Editar perfil';
    editBtn.style.background = 'transparent';
    editBtn.style.borderColor = 'rgba(255, 255, 255, 0.3)';
    editBtn.style.color = 'white';
    
    showStatusMessage('Modo de edición desactivado', 'success');
}

// Status Messages
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

// Newsletter Form (in footer)
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

// Initialize newsletter when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    initNewsletterForm();
});

// Utility Functions
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

function throttle(func, limit) {
    let inThrottle;
    return function() {
        const args = arguments;
        const context = this;
        if (!inThrottle) {
            func.apply(context, args);
            inThrottle = true;
            setTimeout(() => inThrottle = false, limit);
        }
    };
}

// Smooth scrolling for internal links
document.addEventListener('DOMContentLoaded', function() {
    const internalLinks = document.querySelectorAll('a[href^="#"]');
    
    internalLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            
            const targetId = this.getAttribute('href').substring(1);
            const targetElement = document.getElementById(targetId);
            
            if (targetElement) {
                targetElement.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
});

// Handle window resize for responsive behavior
window.addEventListener('resize', debounce(function() {
    // Adjust layout for mobile if needed
    const isMobile = window.innerWidth <= 768;
    const sidebar = document.querySelector('.profile-sidebar');
    
    if (sidebar) {
        if (isMobile) {
            sidebar.classList.add('mobile');
        } else {
            sidebar.classList.remove('mobile');
        }
    }
}, 250));

// Keyboard navigation
document.addEventListener('keydown', function(e) {
    // ESC key to cancel editing
    if (e.key === 'Escape') {
        const editBtn = document.querySelector('.edit-profile-btn');
        if (editBtn && editBtn.textContent.includes('Cancelar')) {
            editBtn.click();
        }
    }
    
    // Ctrl+S to save form
    if (e.ctrlKey && e.key === 's') {
        e.preventDefault();
        const form = document.querySelector('.profile-form form');
        if (form) {
            const submitBtn = form.querySelector('.btn-primary');
            if (submitBtn) {
                submitBtn.click();
            }
        }
    }
});

// Print functionality (if needed)
function printProfile() {
    const printContent = document.querySelector('.profile-content').cloneNode(true);
    const printWindow = window.open('', '_blank');
    
    printWindow.document.write(`
        <html>
            <head>
                <title>Mi Perfil - AutoClick.cr</title>
                <style>
                    body { font-family: 'Montserrat', sans-serif; }
                    .form-input, .form-textarea { border: 1px solid #ccc; }
                    .profile-sidebar { display: none; }
                </style>
            </head>
            <body>
                ${printContent.outerHTML}
            </body>
        </html>
    `);
    
    printWindow.document.close();
    printWindow.print();
}

// Auto-resize textareas
function initAutoResizeTextareas() {
    const textareas = document.querySelectorAll('.form-textarea');
    
    textareas.forEach(textarea => {
        // Set initial height based on content
        autoResize(textarea);
        
        // Add event listeners for auto-resize
        textarea.addEventListener('input', function() {
            autoResize(this);
        });
        
        textarea.addEventListener('paste', function() {
            // Delay to allow paste content to be processed
            setTimeout(() => {
                autoResize(this);
            }, 10);
        });
    });
}

function autoResize(textarea) {
    // Reset height to auto to get the correct scrollHeight
    textarea.style.height = 'auto';
    
    // Get the scroll height (content height)
    const scrollHeight = textarea.scrollHeight;
    
    // Set minimum height (44px to match single line)
    const minHeight = 44;
    
    // Set the height to either scrollHeight or minHeight, whichever is larger
    textarea.style.height = Math.max(scrollHeight, minHeight) + 'px';
}

// Export profile data (if needed)
function exportProfileData() {
    const form = document.querySelector('.profile-form form');
    const formData = new FormData(form);
    const profileData = {};
    
    for (let [key, value] of formData.entries()) {
        profileData[key] = value;
    }
    
    const dataStr = JSON.stringify(profileData, null, 2);
    const dataBlob = new Blob([dataStr], { type: 'application/json' });
    
    const link = document.createElement('a');
    link.href = URL.createObjectURL(dataBlob);
    link.download = 'mi-perfil.json';
    link.click();
}