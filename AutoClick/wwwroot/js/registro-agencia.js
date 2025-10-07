// Registro Agencia JavaScript

document.addEventListener('DOMContentLoaded', function() {
    initializeRegistroAgencia();
});

function initializeRegistroAgencia() {
    // Inicializar validación de formulario
    initializeFormValidation();
    
    // Inicializar funcionalidad de FAQ
    initializeFaqFunctionality();
    
    // Inicializar funciones de navegación
    initializeNavigation();
    
    // Inicializar autoguardado
    initializeAutoSave();
}

// Validación de formulario en tiempo real
function initializeFormValidation() {
    const form = document.getElementById('registroAgenciaForm');
    if (!form) return;
    
    const inputs = form.querySelectorAll('input[required]');
    
    inputs.forEach(input => {
        // Validación en tiempo real
        input.addEventListener('blur', function() {
            validateField(this);
        });
        
        input.addEventListener('input', function() {
            clearFieldError(this);
            
            // Validación específica por tipo de campo
            if (this.type === 'email') {
                validateEmail(this);
            } else if (this.name === 'ConfirmEmail') {
                validateEmailConfirmation(this);
            } else if (this.type === 'password') {
                validatePassword(this);
            } else if (this.name === 'ConfirmPassword') {
                validatePasswordConfirmation(this);
            } else if (this.type === 'tel') {
                validatePhone(this);
            }
        });
    });
    
    // Validación al enviar formulario
    form.addEventListener('submit', function(e) {
        e.preventDefault();
        
        if (validateForm()) {
            submitForm();
        }
    });
}

function validateField(field) {
    const value = field.value.trim();
    const fieldName = field.name;
    let isValid = true;
    let errorMessage = '';
    
    // Validación de campos requeridos
    if (field.hasAttribute('required') && !value) {
        isValid = false;
        errorMessage = `El campo ${getFieldDisplayName(fieldName)} es requerido`;
    }
    
    // Validaciones específicas
    if (isValid && value) {
        switch (fieldName) {
            case 'Email':
                if (!isValidEmail(value)) {
                    isValid = false;
                    errorMessage = 'El formato del correo electrónico no es válido';
                }
                break;
                
            case 'ConfirmEmail':
                const emailField = document.getElementById('Email');
                if (value !== emailField.value) {
                    isValid = false;
                    errorMessage = 'Los correos electrónicos no coinciden';
                }
                break;
                
            case 'Password':
                if (value.length < 6) {
                    isValid = false;
                    errorMessage = 'La contraseña debe tener al menos 6 caracteres';
                }
                break;
                
            case 'ConfirmPassword':
                const passwordField = document.getElementById('Password');
                if (value !== passwordField.value) {
                    isValid = false;
                    errorMessage = 'Las contraseñas no coinciden';
                }
                break;
                
            case 'Telefono1':
            case 'WhatsApp':
                if (!isValidPhone(value)) {
                    isValid = false;
                    errorMessage = 'El formato del teléfono no es válido';
                }
                break;
                
            case 'CedulaJuridica':
                if (!isValidCedula(value)) {
                    isValid = false;
                    errorMessage = 'El formato de la cédula no es válido';
                }
                break;
        }
    }
    
    if (!isValid) {
        showFieldError(field, errorMessage);
    } else {
        clearFieldError(field);
    }
    
    return isValid;
}

function validateEmail(field) {
    const email = field.value.trim();
    if (email && !isValidEmail(email)) {
        showFieldError(field, 'El formato del correo electrónico no es válido');
        return false;
    }
    return true;
}

function validateEmailConfirmation(field) {
    const emailField = document.getElementById('Email');
    if (field.value && field.value !== emailField.value) {
        showFieldError(field, 'Los correos electrónicos no coinciden');
        return false;
    }
    return true;
}

function validatePassword(field) {
    const password = field.value;
    if (password && password.length < 6) {
        showFieldError(field, 'La contraseña debe tener al menos 6 caracteres');
        return false;
    }
    return true;
}

function validatePasswordConfirmation(field) {
    const passwordField = document.getElementById('Password');
    if (field.value && field.value !== passwordField.value) {
        showFieldError(field, 'Las contraseñas no coinciden');
        return false;
    }
    return true;
}

function validatePhone(field) {
    const phone = field.value.trim();
    if (phone && !isValidPhone(phone)) {
        showFieldError(field, 'El formato del teléfono no es válido');
        return false;
    }
    return true;
}

function validateForm() {
    const form = document.getElementById('registroAgenciaForm');
    const inputs = form.querySelectorAll('input[required]');
    let isFormValid = true;
    
    inputs.forEach(input => {
        if (!validateField(input)) {
            isFormValid = false;
        }
    });
    
    return isFormValid;
}

// Funciones de utilidad para validación
function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

function isValidPhone(phone) {
    // Acepta números de teléfono costarricenses
    const phoneRegex = /^[0-9]{8}$/;
    const cleanPhone = phone.replace(/\D/g, '');
    return phoneRegex.test(cleanPhone);
}

function isValidCedula(cedula) {
    // Validación básica para cédula costarricense
    const cedulaRegex = /^[0-9]{9,12}$/;
    const cleanCedula = cedula.replace(/\D/g, '');
    return cedulaRegex.test(cleanCedula);
}

function getFieldDisplayName(fieldName) {
    const displayNames = {
        'NombreComercial': 'Nombre Comercial',
        'CedulaJuridica': 'Cédula Jurídica',
        'RepresentanteLegal': 'Representante Legal',
        'Email': 'Correo Electrónico',
        'ConfirmEmail': 'Confirmación de Correo',
        'Telefono1': 'Teléfono Principal',
        'WhatsApp': 'WhatsApp',
        'Password': 'Contraseña',
        'ConfirmPassword': 'Confirmación de Contraseña'
    };
    return displayNames[fieldName] || fieldName;
}

// Funciones para mostrar/ocultar errores
function showFieldError(field, message) {
    clearFieldError(field);
    
    field.classList.add('input-validation-error');
    
    const errorElement = document.createElement('span');
    errorElement.className = 'field-validation-error';
    errorElement.textContent = message;
    
    field.parentNode.appendChild(errorElement);
}

function clearFieldError(field) {
    field.classList.remove('input-validation-error');
    
    const existingError = field.parentNode.querySelector('.field-validation-error');
    if (existingError) {
        existingError.remove();
    }
}

// Funcionalidad de FAQ
function initializeFaqFunctionality() {
    const faqQuestions = document.querySelectorAll('.faq-question');
    
    faqQuestions.forEach(question => {
        question.addEventListener('click', function() {
            toggleFaq(this);
        });
    });
}

function toggleFaq(questionElement) {
    const faqItem = questionElement.parentElement;
    const isActive = faqItem.classList.contains('active');
    
    // Cerrar todos los FAQs
    document.querySelectorAll('.faq-item').forEach(item => {
        item.classList.remove('active');
    });
    
    // Si no estaba activo, abrirlo
    if (!isActive) {
        faqItem.classList.add('active');
    }
}

// Navegación
function initializeNavigation() {
    const backButton = document.querySelector('.btn-back');
    const nextButton = document.querySelector('.btn-next');
    
    if (backButton) {
        backButton.addEventListener('click', function() {
            // Confirmar si hay cambios sin guardar
            if (hasUnsavedChanges()) {
                if (confirm('¿Estás seguro de que quieres salir? Los cambios no guardados se perderán.')) {
                    window.history.back();
                }
            } else {
                window.history.back();
            }
        });
    }
}

function hasUnsavedChanges() {
    const form = document.getElementById('registroAgenciaForm');
    if (!form) return false;
    
    const inputs = form.querySelectorAll('input');
    for (let input of inputs) {
        if (input.value.trim() !== '') {
            return true;
        }
    }
    return false;
}

// Autoguardado
function initializeAutoSave() {
    const form = document.getElementById('registroAgenciaForm');
    if (!form) return;
    
    const inputs = form.querySelectorAll('input');
    
    inputs.forEach(input => {
        input.addEventListener('input', debounce(function() {
            saveFormData();
        }, 2000)); // Guardar después de 2 segundos de inactividad
    });
    
    // Cargar datos guardados al inicializar
    loadSavedFormData();
}

function saveFormData() {
    const form = document.getElementById('registroAgenciaForm');
    const formData = new FormData(form);
    const dataObject = {};
    
    for (let [key, value] of formData.entries()) {
        // No guardar contraseñas por seguridad
        if (!key.toLowerCase().includes('password')) {
            dataObject[key] = value;
        }
    }
    
    localStorage.setItem('registroAgenciaData', JSON.stringify(dataObject));
}

function loadSavedFormData() {
    const savedData = localStorage.getItem('registroAgenciaData');
    if (!savedData) return;
    
    try {
        const dataObject = JSON.parse(savedData);
        
        for (let [key, value] of Object.entries(dataObject)) {
            const field = document.getElementsByName(key)[0];
            if (field && field.type !== 'password') {
                field.value = value;
            }
        }
    } catch (error) {
        console.error('Error loading saved form data:', error);
    }
}

function clearSavedFormData() {
    localStorage.removeItem('registroAgenciaData');
}

// Envío de formulario
async function submitForm() {
    const form = document.getElementById('registroAgenciaForm');
    const submitButton = document.querySelector('.btn-next');
    
    // Mostrar estado de carga
    form.classList.add('form-loading');
    submitButton.disabled = true;
    submitButton.textContent = 'Procesando...';
    
    try {
        // Enviar formulario usando fetch para mejor control
        const formData = new FormData(form);
        
        const response = await fetch(form.action || window.location.pathname, {
            method: 'POST',
            body: formData,
            headers: {
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
            }
        });
        
        if (response.ok) {
            // Limpiar datos guardados
            clearSavedFormData();
            
            // Redirigir o mostrar éxito
            const result = await response.text();
            if (result.includes('RedirectToPage')) {
                window.location.href = '/RegistroAgenciaUbicacion';
            } else {
                // Mostrar errores del servidor si los hay
                document.documentElement.innerHTML = result;
            }
        } else {
            throw new Error('Error en el servidor');
        }
    } catch (error) {
        console.error('Error submitting form:', error);
        alert('Ocurrió un error al procesar el registro. Por favor, intente nuevamente.');
    } finally {
        // Restaurar estado normal
        form.classList.remove('form-loading');
        submitButton.disabled = false;
        submitButton.innerHTML = 'Siguiente sección <span class="icon">→</span> <span class="step-indicator">(1/3)</span>';
    }
}

// Función de utilidad para debounce
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

// Función para formatear números de teléfono mientras se escriben
function formatPhoneNumber(input) {
    let value = input.value.replace(/\D/g, '');
    if (value.length >= 4) {
        value = value.substring(0, 4) + '-' + value.substring(4, 8);
    }
    input.value = value;
}

// Agregar formateo automático a campos de teléfono
document.addEventListener('DOMContentLoaded', function() {
    const phoneFields = document.querySelectorAll('input[name="Telefono1"], input[name="WhatsApp"]');
    phoneFields.forEach(field => {
        field.addEventListener('input', function() {
            formatPhoneNumber(this);
        });
    });
});

// Función para mostrar/ocultar contraseña
function togglePasswordVisibility(inputId) {
    const input = document.getElementById(inputId);
    const button = input.nextElementSibling;
    
    if (input.type === 'password') {
        input.type = 'text';
        button.textContent = '👁️‍🗨️';
    } else {
        input.type = 'password';
        button.textContent = '👁️';
    }
}

// Prevenir envío accidental del formulario con Enter
document.addEventListener('DOMContentLoaded', function() {
    const form = document.getElementById('registroAgenciaForm');
    if (form) {
        form.addEventListener('keypress', function(e) {
            if (e.key === 'Enter' && e.target.type !== 'submit') {
                e.preventDefault();
                // Mover al siguiente campo
                const inputs = Array.from(form.querySelectorAll('input:not([type="hidden"])'));
                const currentIndex = inputs.indexOf(e.target);
                if (currentIndex < inputs.length - 1) {
                    inputs[currentIndex + 1].focus();
                }
            }
        });
    }
});