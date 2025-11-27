// Auth Page JavaScript functionality
document.addEventListener('DOMContentLoaded', function() {
    initializeAuthPage();
});

function initializeAuthPage() {
    initializeUserTypeSelection();
    initializeFormValidation();
    // initializeSocialButtons(); // Removed - social login disabled
    checkRememberedUser();
}

function initializeUserTypeSelection() {
    const userTypeRadios = document.querySelectorAll('input[name="TipoUsuario"]');
    const continueButton = document.getElementById('btnContinueRegister');
    
    userTypeRadios.forEach(radio => {
        radio.addEventListener('change', function() {
            updateContinueButton();
            animateCardSelection(this);
        });
    });
    
    function updateContinueButton() {
        const selectedType = document.querySelector('input[name="TipoUsuario"]:checked');
        if (continueButton) {
            if (selectedType) {
                continueButton.disabled = false;
                continueButton.textContent = `Continuar como ${selectedType.value}`;
            } else {
                continueButton.disabled = true;
                continueButton.textContent = 'Continuar registro';
            }
        }
    }
    
    function animateCardSelection(selectedRadio) {
        // Remove animation from all cards
        const allCards = document.querySelectorAll('.type-card');
        allCards.forEach(card => {
            card.style.transform = 'scale(1)';
        });
        
        // Add animation to selected card
        const selectedCard = selectedRadio.nextElementSibling;
        if (selectedCard) {
            selectedCard.style.transform = 'scale(1.02)';
            setTimeout(() => {
                selectedCard.style.transform = 'scale(1)';
            }, 200);
        }
    }
}

function initializeFormValidation() {
    const loginForm = document.getElementById('loginForm');
    const registerForm = document.getElementById('registerForm');
    
    if (loginForm) {
        const emailInput = document.getElementById('loginEmail');
        const passwordInput = document.getElementById('loginPassword');
        
        emailInput.addEventListener('input', function() {
            validateEmail(this);
        });
        
        passwordInput.addEventListener('input', function() {
            validatePassword(this);
        });
        
        loginForm.addEventListener('submit', function(e) {
            if (!validateLoginForm()) {
                e.preventDefault();
            }
        });
    }
    
    if (registerForm) {
        registerForm.addEventListener('submit', function(e) {
            const selectedType = document.querySelector('input[name="TipoUsuario"]:checked');
            if (!selectedType) {
                e.preventDefault();
                showError('Por favor, seleccione un tipo de usuario');
            }
        });
    }
}

function validateEmail(input) {
    const email = input.value.trim();
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    
    clearFieldError(input);
    
    if (email && !emailRegex.test(email)) {
        showFieldError(input, 'Ingrese un correo electrónico válido');
        return false;
    }
    
    return true;
}

function validatePassword(input) {
    const password = input.value;
    
    clearFieldError(input);
    
    if (password && password.length < 6) {
        showFieldError(input, 'La contraseña debe tener al menos 6 caracteres');
        return false;
    }
    
    return true;
}

function validateLoginForm() {
    const emailInput = document.getElementById('loginEmail');
    const passwordInput = document.getElementById('loginPassword');
    
    let isValid = true;
    
    // Validate email using new function
    if (!validateEmailField(emailInput)) {
        isValid = false;
    }
    
    // Validate password using new function
    if (!validatePasswordField(passwordInput)) {
        isValid = false;
    }
    
    return isValid;
}

// Removed duplicate function - using the one defined later

function clearFieldError(input) {
    const existingError = input.parentNode.querySelector('.field-error');
    if (existingError) {
        existingError.remove();
    }
    input.style.borderColor = 'rgba(255, 255, 255, 0.5)';
}

function clearAllErrors() {
    const allErrors = document.querySelectorAll('.field-error');
    allErrors.forEach(error => error.remove());
    
    const allInputs = document.querySelectorAll('.form-input');
    allInputs.forEach(input => {
        input.style.borderColor = 'rgba(255, 255, 255, 0.5)';
    });
}

function showError(message) {
    // Remove existing error messages
    const existingErrors = document.querySelectorAll('.temp-error-message');
    existingErrors.forEach(error => error.remove());
    
    const errorDiv = document.createElement('div');
    errorDiv.className = 'temp-error-message';
    errorDiv.textContent = message;
    errorDiv.style.cssText = `
        background: rgba(255, 0, 0, 0.1);
        border: 1px solid #FF0000;
        border-radius: 4px;
        padding: 12px;
        margin-bottom: 20px;
        color: #FF0000;
        font-size: 14px;
        font-family: 'Montserrat', sans-serif;
        font-weight: 400;
        text-align: center;
    `;
    
    // Insert at the top of the register section
    const registerSection = document.querySelector('.register-section .auth-form');
    if (registerSection) {
        registerSection.insertBefore(errorDiv, registerSection.firstChild);
        
        // Auto remove after 5 seconds
        setTimeout(() => {
            if (errorDiv.parentNode) {
                errorDiv.remove();
            }
        }, 5000);
    }
}

/* Social login functions - REMOVED
function initializeSocialButtons() {
    const googleBtn = document.querySelector('.google-btn');
    const facebookBtn = document.querySelector('.facebook-btn');
    
    if (googleBtn) {
        googleBtn.addEventListener('click', function() {
            handleSocialLogin('google');
        });
    }
    
    if (facebookBtn) {
        facebookBtn.addEventListener('click', function() {
            handleSocialLogin('facebook');
        });
    }
}

function handleSocialLogin(provider) {
    // Show loading state
    const button = document.querySelector(`.${provider}-btn`);
    const originalText = button.textContent;
    button.textContent = 'Conectando...';
    button.disabled = true;
    
    // Simulate social login process
    setTimeout(() => {
        // In a real implementation, this would redirect to the OAuth provider
        alert(`Funcionalidad de ${provider} estará disponible pronto`);
        
        // Restore button state
        button.textContent = originalText;
        button.disabled = false;
    }, 1500);
}
*/

function checkRememberedUser() {
    // Check if there's a remember me cookie
    const rememberedEmail = getCookie('AutoClickRememberMe');
    if (rememberedEmail) {
        const emailInput = document.getElementById('loginEmail');
        const rememberCheckbox = document.querySelector('input[name="RememberMe"]');
        
        if (emailInput) {
            emailInput.value = rememberedEmail;
        }
        
        if (rememberCheckbox) {
            rememberCheckbox.checked = true;
        }
    }
}

function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) {
        return parts.pop().split(';').shift();
    }
    return null;
}

// Form submission animations
function animateFormSubmission(form, buttonSelector) {
    const button = form.querySelector(buttonSelector);
    if (button) {
        const originalText = button.textContent;
        button.textContent = 'Procesando...';
        button.disabled = true;
        
        // Add loading animation
        const loadingSpinner = document.createElement('div');
        loadingSpinner.style.cssText = `
            display: inline-block;
            width: 16px;
            height: 16px;
            border: 2px solid rgba(2, 8, 28, 0.3);
            border-radius: 50%;
            border-top-color: #02081C;
            animation: spin 1s ease-in-out infinite;
            margin-right: 8px;
        `;
        
        button.insertBefore(loadingSpinner, button.firstChild);
        
        // Add CSS animation
        if (!document.getElementById('spinner-animation')) {
            const style = document.createElement('style');
            style.id = 'spinner-animation';
            style.textContent = `
                @keyframes spin {
                    to { transform: rotate(360deg); }
                }
            `;
            document.head.appendChild(style);
        }
    }
}

// Add form submission listeners with animations
document.addEventListener('DOMContentLoaded', function() {
    const loginForm = document.getElementById('loginForm');
    const registerForm = document.getElementById('registerForm');
    
    if (loginForm) {
        loginForm.addEventListener('submit', function() {
            if (validateLoginForm()) {
                animateFormSubmission(this, '.btn-login');
            }
        });
    }
    
    if (registerForm) {
        registerForm.addEventListener('submit', function() {
            const selectedType = document.querySelector('input[name="TipoUsuario"]:checked');
            if (selectedType) {
                animateFormSubmission(this, '.btn-register');
            }
        });
    }

    // Prevent any dropdown behavior on login button
    const loginBtn = document.querySelector('.btn-login');
    if (loginBtn) {
        // Remove any Bootstrap dropdown classes
        loginBtn.classList.remove('dropdown-toggle', 'dropdown');
        
        // Prevent any dropdown events
        loginBtn.addEventListener('click', function(e) {
            e.stopPropagation();
            // Ensure no dropdown behavior
            if (this.classList.contains('dropdown-toggle')) {
                this.classList.remove('dropdown-toggle');
            }
        });
        
        // Remove any aria attributes that might trigger dropdown behavior
        loginBtn.removeAttribute('aria-haspopup');
        loginBtn.removeAttribute('aria-expanded');
        loginBtn.removeAttribute('data-toggle');
        loginBtn.removeAttribute('data-bs-toggle');
    }

    // Add real-time validation for login form
    initializeLoginValidation();
});

// Login validation functions
function initializeLoginValidation() {
    const loginEmailInput = document.getElementById('loginEmail');
    const loginPasswordInput = document.getElementById('loginPassword');
    const loginForm = document.getElementById('loginForm');

    if (loginEmailInput) {
        loginEmailInput.addEventListener('blur', function() {
            validateEmailField(this);
        });
        
        loginEmailInput.addEventListener('input', function() {
            clearFieldError(this);
        });
    }

    if (loginPasswordInput) {
        loginPasswordInput.addEventListener('blur', function() {
            validatePasswordField(this);
        });
        
        loginPasswordInput.addEventListener('input', function() {
            clearFieldError(this);
        });
    }
}

function validateEmailField(emailInput) {
    const email = emailInput.value.trim();
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    
    if (!email) {
        showFieldError(emailInput, 'El correo electrónico es requerido');
        return false;
    }
    
    if (!emailRegex.test(email)) {
        showFieldError(emailInput, 'Por favor ingrese un correo electrónico válido');
        return false;
    }
    
    clearFieldError(emailInput);
    return true;
}

function validatePasswordField(passwordInput) {
    const password = passwordInput.value;
    
    if (!password) {
        showFieldError(passwordInput, 'La contraseña es requerida');
        return false;
    }
    
    if (password.length < 6) {
        showFieldError(passwordInput, 'La contraseña debe tener al menos 6 caracteres');
        return false;
    }
    
    clearFieldError(passwordInput);
    return true;
}

function showFieldError(field, message) {
    clearFieldError(field);
    
    const errorDiv = document.createElement('div');
    errorDiv.className = 'field-error';
    errorDiv.textContent = message;
    
    field.classList.add('error');
    field.parentNode.appendChild(errorDiv);
}

function clearFieldError(field) {
    field.classList.remove('error');
    const existingError = field.parentNode.querySelector('.field-error');
    if (existingError) {
        existingError.remove();
    }
}

// Keyboard shortcuts
document.addEventListener('keydown', function(e) {
    // Enter key in login form
    if (e.key === 'Enter') {
        const activeElement = document.activeElement;
        
        // If user is in login section
        if (activeElement && activeElement.closest('.login-section')) {
            const loginForm = document.getElementById('loginForm');
            if (loginForm && validateLoginForm()) {
                loginForm.submit();
            }
        }
    }
});

// Auto-focus on email input when page loads
window.addEventListener('load', function() {
    const emailInput = document.getElementById('loginEmail');
    // Solo auto-focus si la intención explícita es login (hash o query param)
    const urlParams = new URLSearchParams(window.location.search);
    const shouldFocusLogin = (location.hash === '#login') || (urlParams.get('mode') === 'login');
    if (shouldFocusLogin && emailInput && !emailInput.value) {
        setTimeout(() => {
            emailInput.focus();
        }, 300);
    }
});