// Configuración de Cuenta JavaScript Functionality

document.addEventListener('DOMContentLoaded', function() {
    // Initialize components
    initSidebarNavigation();
    initFormHandling();
    initModalHandlers();
    initNewsletterForm();
    initStatusMessages();
    initPasswordStrengthChecker();
    
    console.log('Configuración de Cuenta page initialized successfully');
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

// Form Handling
function initFormHandling() {
    const forms = document.querySelectorAll('form');
    
    forms.forEach(form => {
        form.addEventListener('submit', function(e) {
            const submitButton = this.querySelector('button[type="submit"]');
            if (submitButton) {
                submitButton.disabled = true;
                submitButton.textContent = 'Procesando...';
                
                // Re-enable button after form submission
                setTimeout(() => {
                    submitButton.disabled = false;
                    submitButton.textContent = submitButton.getAttribute('data-original-text') || 'Enviar';
                }, 3000);
            }
        });
    });
    
    // Store original button texts
    const submitButtons = document.querySelectorAll('button[type="submit"]');
    submitButtons.forEach(button => {
        button.setAttribute('data-original-text', button.textContent);
    });
}

// Modal Handlers
function initModalHandlers() {
    // Close modals when clicking outside
    document.querySelectorAll('.modal-overlay').forEach(overlay => {
        overlay.addEventListener('click', function(e) {
            if (e.target === this) {
                closeModal(this.id);
            }
        });
    });
    
    // ESC key to close modals
    document.addEventListener('keydown', function(e) {
        if (e.key === 'Escape') {
            const openModal = document.querySelector('.modal-overlay:not([style*="display: none"])');
            if (openModal) {
                closeModal(openModal.id);
            }
        }
    });
}

// Enable email editing
function enableEmailEdit() {
    showModal('changeEmailModal');
}

// Show change email modal
function showChangeEmail() {
    showModal('changeEmailModal');
}

// Show forgot password modal
function showForgotPassword() {
    showModal('forgotPasswordModal');
}

// Show change password modal
function showChangePassword() {
    showModal('changePasswordModal');
}

// Show delete account modal
function showDeleteAccountModal() {
    showModal('deleteAccountModal');
}

// Show modal function
function showModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.style.display = 'flex';
        
        // Focus first input
        const firstInput = modal.querySelector('input[type="email"], input[type="password"]');
        if (firstInput) {
            setTimeout(() => firstInput.focus(), 100);
        }
        
        // Prevent body scrolling
        document.body.style.overflow = 'hidden';
    }
}

// Close modal function
function closeModal(modalId) {
    const modal = document.getElementById(modalId);
    if (modal) {
        modal.style.display = 'none';
        
        // Clear form data
        const form = modal.querySelector('form');
        if (form) {
            form.reset();
            clearValidationErrors(form);
        }
        
        // Restore body scrolling
        document.body.style.overflow = 'auto';
    }
}

// Clear validation errors
function clearValidationErrors(form) {
    const errorMessages = form.querySelectorAll('.validation-message');
    errorMessages.forEach(error => {
        error.textContent = '';
    });
    
    const inputs = form.querySelectorAll('input');
    inputs.forEach(input => {
        input.classList.remove('error');
    });
}

// Password strength checker
function initPasswordStrengthChecker() {
    const passwordInputs = document.querySelectorAll('input[type="password"]');
    
    passwordInputs.forEach(input => {
        if (input.name === 'NewPassword') {
            input.addEventListener('input', function() {
                checkPasswordStrength(this);
            });
        }
    });
}

function checkPasswordStrength(passwordInput) {
    const password = passwordInput.value;
    const strengthIndicator = getOrCreateStrengthIndicator(passwordInput);
    
    // Remove existing strength indicator
    const existingIndicator = passwordInput.parentNode.querySelector('.password-strength');
    if (existingIndicator) {
        existingIndicator.remove();
    }
    
    if (password.length === 0) {
        return;
    }
    
    const strength = calculatePasswordStrength(password);
    const strengthDiv = document.createElement('div');
    strengthDiv.className = `password-strength ${strength.class}`;
    strengthDiv.innerHTML = `
        <div class="strength-bar">
            <div class="strength-fill" style="width: ${strength.percentage}%"></div>
        </div>
        <span class="strength-text">${strength.text}</span>
    `;
    
    passwordInput.parentNode.appendChild(strengthDiv);
}

function calculatePasswordStrength(password) {
    let score = 0;
    let feedback = [];
    
    // Length check
    if (password.length >= 8) score += 25;
    else feedback.push('al menos 8 caracteres');
    
    // Uppercase check
    if (/[A-Z]/.test(password)) score += 25;
    else feedback.push('una mayúscula');
    
    // Lowercase check
    if (/[a-z]/.test(password)) score += 25;
    else feedback.push('una minúscula');
    
    // Number check
    if (/\d/.test(password)) score += 25;
    else feedback.push('un número');
    
    // Special character check
    if (/[@$!%*?&]/.test(password)) score += 25;
    else feedback.push('un carácter especial');
    
    let strengthClass, strengthText;
    
    if (score < 50) {
        strengthClass = 'weak';
        strengthText = `Débil - Necesita: ${feedback.join(', ')}`;
    } else if (score < 75) {
        strengthClass = 'medium';
        strengthText = 'Media - Necesita: ' + feedback.join(', ');
    } else if (score < 100) {
        strengthClass = 'good';
        strengthText = 'Buena - Necesita: ' + feedback.join(', ');
    } else {
        strengthClass = 'strong';
        strengthText = 'Fuerte';
    }
    
    return {
        percentage: Math.min(score, 100),
        class: strengthClass,
        text: strengthText
    };
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
    // Check for server-side messages
    const successMessage = document.querySelector('[data-success-message]');
    const errorMessage = document.querySelector('[data-error-message]');
    
    if (successMessage) {
        showStatusMessage(successMessage.getAttribute('data-success-message'), 'success');
    }
    
    if (errorMessage) {
        showStatusMessage(errorMessage.getAttribute('data-error-message'), 'error');
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
    
    // Auto-remove after 4 seconds
    setTimeout(() => {
        if (messageDiv.parentNode) {
            messageDiv.remove();
        }
    }, 4000);
}

// Email validation for real-time feedback
function initEmailValidation() {
    const emailInputs = document.querySelectorAll('input[type="email"]');
    
    emailInputs.forEach(input => {
        input.addEventListener('blur', function() {
            validateEmail(this);
        });
        
        input.addEventListener('input', function() {
            clearFieldError(this);
        });
    });
}

function validateEmail(emailInput) {
    const email = emailInput.value.trim();
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    
    clearFieldError(emailInput);
    
    if (email && !emailRegex.test(email)) {
        showFieldError(emailInput, 'Ingresa un correo electrónico válido');
        return false;
    }
    
    return true;
}

function showFieldError(field, message) {
    clearFieldError(field);
    
    field.classList.add('error');
    
    const errorDiv = document.createElement('div');
    errorDiv.className = 'validation-message';
    errorDiv.textContent = message;
    
    field.parentNode.appendChild(errorDiv);
}

function clearFieldError(field) {
    field.classList.remove('error');
    
    const errorMessage = field.parentNode.querySelector('.validation-message');
    if (errorMessage) {
        errorMessage.remove();
    }
}

// Confirm password matching
function initPasswordMatching() {
    const confirmPasswordInputs = document.querySelectorAll('input[name*="Confirm"]');
    
    confirmPasswordInputs.forEach(confirmInput => {
        confirmInput.addEventListener('input', function() {
            const passwordFieldName = this.name.replace('Confirm', '');
            const passwordInput = document.querySelector(`input[name="${passwordFieldName}"]`);
            
            if (passwordInput) {
                validatePasswordMatch(passwordInput, this);
            }
        });
    });
}

function validatePasswordMatch(passwordInput, confirmInput) {
    clearFieldError(confirmInput);
    
    if (confirmInput.value && passwordInput.value !== confirmInput.value) {
        showFieldError(confirmInput, 'Las contraseñas no coinciden');
        return false;
    }
    
    return true;
}

// Auto-save functionality for account settings
function initAutoSave() {
    let autoSaveTimeout;
    const inputs = document.querySelectorAll('.account-form input');
    
    inputs.forEach(input => {
        if (input.type !== 'password') {
            input.addEventListener('input', function() {
                clearTimeout(autoSaveTimeout);
                
                autoSaveTimeout = setTimeout(() => {
                    console.log('Auto-saving account settings...');
                    // Here you could implement auto-save functionality
                }, 2000);
            });
        }
    });
}

// Account deletion confirmation
function confirmAccountDeletion() {
    const confirmation = confirm(
        '¿Estás seguro de que deseas eliminar tu cuenta?\n\n' +
        'Esta acción no se puede deshacer y perderás todos tus datos permanentemente.'
    );
    
    if (confirmation) {
        showDeleteAccountModal();
    }
}

// Export account data (GDPR compliance)
function exportAccountData() {
    showStatusMessage('Preparando exportación de datos...', 'info');
    
    setTimeout(() => {
        const accountData = {
            email: 'usuario@autoclick.cr',
            dateCreated: '2024-01-15',
            lastLogin: '2025-09-27',
            preferences: {
                newsletter: true,
                notifications: true
            },
            statistics: {
                adsPosted: 5,
                adsViewed: 150,
                favoritesCount: 12
            }
        };
        
        const dataStr = JSON.stringify(accountData, null, 2);
        const dataBlob = new Blob([dataStr], { type: 'application/json' });
        
        const link = document.createElement('a');
        link.href = URL.createObjectURL(dataBlob);
        link.download = `autoclick-account-data-${new Date().toISOString().split('T')[0]}.json`;
        link.click();
        
        showStatusMessage('Datos de cuenta exportados exitosamente', 'success');
    }, 2000);
}

// Initialize all form validations
document.addEventListener('DOMContentLoaded', function() {
    initEmailValidation();
    initPasswordMatching();
    initAutoSave();
});

// Keyboard shortcuts
document.addEventListener('keydown', function(e) {
    // Ctrl+S to save (if applicable)
    if (e.ctrlKey && e.key === 's') {
        e.preventDefault();
        const submitBtn = document.querySelector('.account-form button[type="submit"]');
        if (submitBtn) {
            submitBtn.click();
        }
    }
    
    // Ctrl+E to export data
    if (e.ctrlKey && e.key === 'e') {
        e.preventDefault();
        exportAccountData();
    }
});

// Utility functions
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