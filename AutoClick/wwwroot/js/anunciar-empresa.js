// AnunciarEmpresa.js - JavaScript functionality for business advertising page

document.addEventListener('DOMContentLoaded', function() {
    initializeFormValidation();
    initializeFAQ();
    initializeScrollToForm();
    initializePhoneValidation();
    initializeFormSubmit();
});

// Form validation functionality
function initializeFormValidation() {
    const form = document.querySelector('.empresa-form');
    const inputs = form.querySelectorAll('.form-control');
    
    if (!form) return;
    
    // Real-time validation for each input
    inputs.forEach(input => {
        // Remove validation error when user starts typing
        input.addEventListener('input', function() {
            clearFieldError(this);
            validateField(this);
        });
        
        // Validate on blur
        input.addEventListener('blur', function() {
            validateField(this);
        });
    });
    
    // Form submission validation
    form.addEventListener('submit', function(e) {
        if (!validateForm()) {
            e.preventDefault();
            showFormError('Por favor, complete todos los campos requeridos correctamente.');
        }
    });
}

// Validate individual field
function validateField(field) {
    const fieldName = field.name;
    const value = field.value.trim();
    let isValid = true;
    let errorMessage = '';
    
    // Clear previous errors
    clearFieldError(field);
    
    // Required field validation
    if (field.hasAttribute('data-val-required') || field.required) {
        if (!value) {
            errorMessage = 'Este campo es requerido';
            isValid = false;
        }
    }
    
    // Specific validations
    if (value && isValid) {
        switch (fieldName) {
            case 'NombreEmpresa':
                if (value.length < 2) {
                    errorMessage = 'El nombre de la empresa debe tener al menos 2 caracteres';
                    isValid = false;
                } else if (value.length > 100) {
                    errorMessage = 'El nombre de la empresa no puede exceder 100 caracteres';
                    isValid = false;
                }
                break;
                
            case 'RepresentanteLegal':
                if (value.length < 2) {
                    errorMessage = 'El nombre del representante debe tener al menos 2 caracteres';
                    isValid = false;
                } else if (value.length > 100) {
                    errorMessage = 'El nombre del representante no puede exceder 100 caracteres';
                    isValid = false;
                }
                break;
                
            case 'CorreoElectronico':
                if (!isValidEmail(value)) {
                    errorMessage = 'Por favor, ingrese un correo electrónico válido';
                    isValid = false;
                }
                break;
                
            case 'Telefono':
                if (!isValidCostaRicaPhone(value)) {
                    errorMessage = 'Por favor, ingrese un número de teléfono válido de Costa Rica';
                    isValid = false;
                }
                break;
                
            case 'DescripcionEmpresa':
                if (value.length < 10) {
                    errorMessage = 'La descripción debe tener al menos 10 caracteres';
                    isValid = false;
                } else if (value.length > 1000) {
                    errorMessage = 'La descripción no puede exceder 1000 caracteres';
                    isValid = false;
                }
                break;
                
            case 'Industria':
                const validIndustrias = ['Automotriz', 'Seguros', 'Financiero', 'Detailing', 'Repuestos', 'Concesionarios', 'Talleres', 'Otros'];
                if (!validIndustrias.includes(value)) {
                    errorMessage = 'Por favor, seleccione una industria válida';
                    isValid = false;
                }
                break;
        }
    }
    
    // Show error if validation failed
    if (!isValid) {
        showFieldError(field, errorMessage);
    } else {
        // Add success styling
        field.style.borderColor = 'rgba(0, 255, 0, 0.5)';
    }
    
    return isValid;
}

// Validate entire form
function validateForm() {
    const form = document.querySelector('.empresa-form');
    const requiredFields = form.querySelectorAll('.form-control[required], .form-control[data-val-required]');
    let isValid = true;
    
    requiredFields.forEach(field => {
        if (!validateField(field)) {
            isValid = false;
        }
    });
    
    return isValid;
}

// Email validation
function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

// Costa Rica phone validation
function isValidCostaRicaPhone(phone) {
    // Remove common formatting characters
    let cleanPhone = phone.replace(/[\s\-\(\)]/g, '');
    
    // Handle international formats
    if (cleanPhone.startsWith('+506')) {
        cleanPhone = cleanPhone.substring(4);
    } else if (cleanPhone.startsWith('506')) {
        cleanPhone = cleanPhone.substring(3);
    }
    
    // Should be 8 digits for Costa Rica
    return /^\d{8}$/.test(cleanPhone);
}

// Show field error
function showFieldError(field, message) {
    clearFieldError(field);
    field.style.borderColor = '#ff0000';
    
    const errorSpan = document.createElement('span');
    errorSpan.className = 'validation-error';
    errorSpan.textContent = message;
    
    field.parentNode.appendChild(errorSpan);
}

// Clear field error
function clearFieldError(field) {
    field.style.borderColor = 'rgba(255, 255, 255, 0.5)';
    
    const existingError = field.parentNode.querySelector('.validation-error');
    if (existingError) {
        existingError.remove();
    }
}

// Show form error
function showFormError(message) {
    // Remove existing form error
    const existingError = document.querySelector('.form-error-message');
    if (existingError) {
        existingError.remove();
    }
    
    // Create new error message
    const errorDiv = document.createElement('div');
    errorDiv.className = 'form-error-message error-message';
    errorDiv.textContent = message;
    
    // Insert at the top of the form
    const form = document.querySelector('.empresa-form');
    const formHeader = form.querySelector('.form-section-title');
    formHeader.parentNode.insertBefore(errorDiv, formHeader.nextSibling);
    
    // Scroll to error
    errorDiv.scrollIntoView({ behavior: 'smooth', block: 'center' });
}

// FAQ functionality
function initializeFAQ() {
    const faqItems = document.querySelectorAll('.faq-item');
    
    faqItems.forEach(item => {
        const question = item.querySelector('.faq-question');
        
        if (question) {
            question.addEventListener('click', function(e) {
                e.preventDefault();
                
                const isActive = item.classList.contains('active');
                
                // Close all FAQ items
                faqItems.forEach(faq => {
                    faq.classList.remove('active');
                });
                
                // Open clicked item if it wasn't active
                if (!isActive) {
                    item.classList.add('active');
                }
            });
        }
    });
}

// Scroll to form functionality
function initializeScrollToForm() {
    // Make scrollToForm function global for inline onclick
    window.scrollToForm = function() {
        const contactSection = document.getElementById('contact-form');
        if (contactSection) {
            contactSection.scrollIntoView({ 
                behavior: 'smooth',
                block: 'start'
            });
        }
    };
}

// Phone number formatting
function initializePhoneValidation() {
    const phoneInput = document.querySelector('input[name="Telefono"]');
    
    if (phoneInput) {
        phoneInput.addEventListener('input', function(e) {
            let value = e.target.value.replace(/\D/g, ''); // Remove non-digits
            
            // Limit to 8 digits for Costa Rica
            if (value.length > 8) {
                value = value.substring(0, 8);
            }
            
            // Format as XXXX-XXXX
            if (value.length >= 4) {
                value = value.substring(0, 4) + '-' + value.substring(4);
            }
            
            e.target.value = value;
        });
    }
}

// Character counter for description
function initializeCharacterCounter() {
    const descriptionField = document.querySelector('textarea[name="DescripcionEmpresa"]');
    
    if (descriptionField) {
        // Create character counter
        const counter = document.createElement('div');
        counter.className = 'character-counter';
        counter.style.cssText = 'text-align: right; font-size: 12px; color: rgba(255, 255, 255, 0.5); margin-top: 4px;';
        
        descriptionField.parentNode.appendChild(counter);
        
        // Update counter
        function updateCounter() {
            const length = descriptionField.value.length;
            const maxLength = 1000;
            counter.textContent = `${length}/${maxLength} caracteres`;
            
            if (length > maxLength) {
                counter.style.color = '#ff0000';
            } else if (length > maxLength * 0.9) {
                counter.style.color = '#FF931E';
            } else {
                counter.style.color = 'rgba(255, 255, 255, 0.5)';
            }
        }
        
        descriptionField.addEventListener('input', updateCounter);
        updateCounter(); // Initial count
    }
}

// Initialize character counter when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    initializeCharacterCounter();
});

// Smooth scrolling for navigation links
document.addEventListener('DOMContentLoaded', function() {
    const navLinks = document.querySelectorAll('a[href^="#"]');
    
    navLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            
            const targetId = this.getAttribute('href');
            const targetElement = document.querySelector(targetId);
            
            if (targetElement) {
                targetElement.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
});

// Form submission enhancement
document.addEventListener('DOMContentLoaded', function() {
    const form = document.querySelector('.empresa-form');
    const submitButton = form?.querySelector('.submit-button');
    
    if (form && submitButton) {
        form.addEventListener('submit', function() {
            // Disable button to prevent double submission
            submitButton.disabled = true;
            submitButton.textContent = 'Enviando...';
            
            // Re-enable after 5 seconds as fallback
            setTimeout(() => {
                submitButton.disabled = false;
                submitButton.textContent = 'Enviar';
            }, 5000);
        });
    }
});

// Auto-hide success/error messages
document.addEventListener('DOMContentLoaded', function() {
    const messages = document.querySelectorAll('.success-message, .error-message');
    
    messages.forEach(message => {
        // Auto-hide after 10 seconds
        setTimeout(() => {
            if (message.parentNode) {
                message.style.transition = 'opacity 0.5s ease';
                message.style.opacity = '0';
                
                setTimeout(() => {
                    if (message.parentNode) {
                        message.remove();
                    }
                }, 500);
            }
        }, 10000);
    });
});

// Dropdown menu functionality for navigation
document.addEventListener('DOMContentLoaded', function() {
    const dropdownContainers = document.querySelectorAll('.nav-dropdown-container');
    
    dropdownContainers.forEach(container => {
        const menu = container.querySelector('.dropdown-menu');
        
        if (menu) {
            // Show dropdown on hover
            container.addEventListener('mouseenter', function() {
                menu.style.display = 'block';
                setTimeout(() => {
                    menu.style.opacity = '1';
                    menu.style.visibility = 'visible';
                    menu.style.transform = 'translateY(0)';
                }, 10);
            });
            
            // Hide dropdown when mouse leaves
            container.addEventListener('mouseleave', function() {
                menu.style.opacity = '0';
                menu.style.visibility = 'hidden';
                menu.style.transform = 'translateY(-10px)';
                
                setTimeout(() => {
                    if (menu.style.opacity === '0') {
                        menu.style.display = 'none';
                    }
                }, 300);
            });
        }
    });
});

// Form submit handling
function initializeFormSubmit() {
    const form = document.querySelector('.empresa-form');
    const submitButton = form?.querySelector('.submit-button');
    
    if (form && submitButton) {
        form.addEventListener('submit', function() {
            // Disable button to prevent double submission
            submitButton.disabled = true;
            submitButton.textContent = 'Enviando...';
            
            // Re-enable after 5 seconds as fallback
            setTimeout(() => {
                submitButton.disabled = false;
                submitButton.textContent = 'Enviar';
            }, 5000);
        });
    }
}