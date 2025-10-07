// Gumout Financing Page JavaScript

document.addEventListener('DOMContentLoaded', function() {
    initializeFormCalculations();
    initializeFAQ();
    initializeFormValidation();
});

// Smooth scroll to calculator
function scrollToCalculator() {
    const calculator = document.getElementById('calculator');
    if (calculator) {
        calculator.scrollIntoView({ 
            behavior: 'smooth',
            block: 'start'
        });
    }
}

// Initialize loan calculations
function initializeFormCalculations() {
    const precioInput = document.querySelector('input[name="PrecioVehiculo"]');
    const plazoSelect = document.querySelector('select[name="Plazo"]');
    const primaInput = document.querySelector('input[name="Prima"]');
    const tasaInput = document.querySelector('input[name="TasaAnual"]');
    const cuotaInput = document.querySelector('input[name="CuotaMensual"]');

    // Set default interest rate
    if (tasaInput && !tasaInput.value) {
        tasaInput.value = '8.95';
    }

    function calculateLoan() {
        const precio = parseFloat(precioInput?.value) || 0;
        const plazo = parseInt(plazoSelect?.value) || 0;
        const prima = parseFloat(primaInput?.value) || 0;
        const tasa = parseFloat(tasaInput?.value) || 8.95;

        if (precio > 0 && plazo > 0) {
            const montoPrestamo = precio - prima;
            
            if (montoPrestamo > 0) {
                const tasaMensual = tasa / 100 / 12;
                const numeroPagos = plazo;
                
                let cuota;
                if (tasaMensual > 0) {
                    cuota = montoPrestamo * 
                           (tasaMensual * Math.pow(1 + tasaMensual, numeroPagos)) /
                           (Math.pow(1 + tasaMensual, numeroPagos) - 1);
                } else {
                    cuota = montoPrestamo / plazo;
                }

                if (cuotaInput) {
                    cuotaInput.value = cuota.toFixed(2);
                }
            }
        }
    }

    // Add event listeners for real-time calculation
    [precioInput, plazoSelect, primaInput, tasaInput].forEach(element => {
        if (element) {
            element.addEventListener('input', calculateLoan);
            element.addEventListener('change', calculateLoan);
        }
    });
}

// Initialize FAQ functionality
function initializeFAQ() {
    const faqItems = document.querySelectorAll('.faq-item');
    
    faqItems.forEach(item => {
        const question = item.querySelector('.faq-question');
        if (question) {
            question.addEventListener('click', () => toggleFaq(question));
        }
    });
}

function toggleFaq(questionElement) {
    const faqItem = questionElement.closest('.faq-item');
    const isActive = faqItem.classList.contains('active');
    
    // Close all FAQ items
    document.querySelectorAll('.faq-item').forEach(item => {
        item.classList.remove('active');
    });
    
    // Open clicked item if it wasn't already active
    if (!isActive) {
        faqItem.classList.add('active');
    }
}

// Initialize form validation
function initializeFormValidation() {
    const form = document.querySelector('.finance-form');
    const requiredFields = form?.querySelectorAll('input[required], select[required]');

    // Add visual feedback for required fields
    requiredFields?.forEach(field => {
        field.addEventListener('blur', validateField);
        field.addEventListener('input', clearFieldError);
    });

    // Custom validation messages
    const precioInput = document.querySelector('input[name="PrecioVehiculo"]');
    const primaInput = document.querySelector('input[name="Prima"]');

    if (precioInput) {
        precioInput.addEventListener('input', function() {
            const precio = parseFloat(this.value) || 0;
            const prima = parseFloat(primaInput?.value) || 0;
            
            if (prima > precio && precio > 0) {
                showFieldError(primaInput, 'La prima no puede ser mayor al precio del vehículo');
            }
        });
    }

    if (primaInput) {
        primaInput.addEventListener('input', function() {
            const prima = parseFloat(this.value) || 0;
            if (prima > 0 && prima < 10000) {
                showFieldError(this, 'La prima mínima es $10,000');
            }
        });
    }

    // Phone number formatting for future phone field
    const phoneInputs = document.querySelectorAll('input[type="tel"]');
    phoneInputs.forEach(input => {
        input.addEventListener('input', formatCostaRicanPhone);
    });
}

function validateField(event) {
    const field = event.target;
    const value = field.value.trim();
    
    if (field.hasAttribute('required') && !value) {
        showFieldError(field, 'Este campo es requerido');
        return false;
    }
    
    if (field.type === 'email' && value && !isValidEmail(value)) {
        showFieldError(field, 'Por favor ingrese un email válido');
        return false;
    }
    
    if (field.type === 'number' && value) {
        const num = parseFloat(value);
        if (isNaN(num) || num <= 0) {
            showFieldError(field, 'Por favor ingrese un número válido mayor a 0');
            return false;
        }
    }
    
    clearFieldError(event);
    return true;
}

function showFieldError(field, message) {
    clearFieldError({ target: field });
    
    const errorDiv = document.createElement('div');
    errorDiv.className = 'field-error';
    errorDiv.textContent = message;
    errorDiv.style.cssText = `
        color: #ef4444;
        font-size: 12px;
        margin-top: 4px;
        font-family: 'Montserrat', sans-serif;
    `;
    
    field.style.borderColor = '#ef4444';
    field.parentNode.appendChild(errorDiv);
}

function clearFieldError(event) {
    const field = event.target;
    const existingError = field.parentNode.querySelector('.field-error');
    
    if (existingError) {
        existingError.remove();
    }
    
    field.style.borderColor = 'rgba(255, 255, 255, 0.5)';
}

function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

function formatCostaRicanPhone(event) {
    let value = event.target.value.replace(/\D/g, '');
    
    if (value.length >= 8) {
        value = value.substring(0, 8);
        value = value.replace(/(\d{4})(\d{4})/, '$1-$2');
    }
    
    event.target.value = value;
}

// Currency formatting
function formatCurrency(input) {
    let value = input.value.replace(/[^\d.]/g, '');
    let parts = value.split('.');
    
    if (parts[1] && parts[1].length > 2) {
        parts[1] = parts[1].substring(0, 2);
    }
    
    parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ',');
    
    input.value = parts.join('.');
}

// Add currency formatting to number inputs
document.addEventListener('DOMContentLoaded', function() {
    const currencyInputs = document.querySelectorAll('input[name="PrecioVehiculo"], input[name="Prima"]');
    
    currencyInputs.forEach(input => {
        input.addEventListener('blur', function() {
            if (this.value) {
                formatCurrency(this);
            }
        });
        
        input.addEventListener('focus', function() {
            this.value = this.value.replace(/,/g, '');
        });
    });
});

// Loan calculator utilities
const LoanCalculator = {
    calculateMonthlyPayment: function(principal, annualRate, months) {
        if (principal <= 0 || months <= 0) return 0;
        
        if (annualRate === 0) {
            return principal / months;
        }
        
        const monthlyRate = annualRate / 100 / 12;
        const payment = principal * 
                       (monthlyRate * Math.pow(1 + monthlyRate, months)) /
                       (Math.pow(1 + monthlyRate, months) - 1);
        
        return payment;
    },
    
    calculateTotalInterest: function(monthlyPayment, months, principal) {
        return (monthlyPayment * months) - principal;
    },
    
    generateAmortizationSchedule: function(principal, annualRate, months) {
        const schedule = [];
        const monthlyRate = annualRate / 100 / 12;
        const monthlyPayment = this.calculateMonthlyPayment(principal, annualRate, months);
        let remainingBalance = principal;
        
        for (let i = 1; i <= months; i++) {
            const interestPayment = remainingBalance * monthlyRate;
            const principalPayment = monthlyPayment - interestPayment;
            remainingBalance -= principalPayment;
            
            schedule.push({
                month: i,
                monthlyPayment: monthlyPayment,
                principalPayment: principalPayment,
                interestPayment: interestPayment,
                remainingBalance: Math.max(0, remainingBalance)
            });
        }
        
        return schedule;
    }
};

// Export for potential future use
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { LoanCalculator, toggleFaq, scrollToCalculator };
}