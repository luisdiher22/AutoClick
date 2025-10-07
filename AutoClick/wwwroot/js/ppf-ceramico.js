// PPF y Cerámico Page JavaScript

document.addEventListener('DOMContentLoaded', function() {
    // Initialize all interactive features
    initializeFAQ();
    initializeContactForm();
    initializeScrollAnimations();
    initializeDropdownMenus();
    
    console.log('PPF y Cerámico page initialized');
});

// FAQ Functionality
function initializeFAQ() {
    const faqItems = document.querySelectorAll('.faq-item');
    
    faqItems.forEach(item => {
        const question = item.querySelector('.faq-question');
        
        question.addEventListener('click', function() {
            // Close all other FAQ items
            faqItems.forEach(otherItem => {
                if (otherItem !== item) {
                    otherItem.classList.remove('active');
                }
            });
            
            // Toggle current item
            item.classList.toggle('active');
        });
    });
}

// Contact Form Enhancement
function initializeContactForm() {
    const form = document.querySelector('.contact-form');
    const inputs = form.querySelectorAll('input, select, textarea');
    const checkboxes = form.querySelectorAll('input[type="checkbox"]');
    
    // Add floating label effect
    inputs.forEach(input => {
        input.addEventListener('focus', function() {
            this.parentElement.classList.add('focused');
        });
        
        input.addEventListener('blur', function() {
            if (!this.value) {
                this.parentElement.classList.remove('focused');
            }
        });
        
        // Check if input has value on load
        if (input.value) {
            input.parentElement.classList.add('focused');
        }
    });
    
    // Checkbox styling enhancement
    checkboxes.forEach(checkbox => {
        checkbox.addEventListener('change', function() {
            const checkmark = this.nextElementSibling;
            if (this.checked) {
                checkmark.style.transform = 'scale(1.1)';
                setTimeout(() => {
                    checkmark.style.transform = 'scale(1)';
                }, 150);
            }
        });
    });
    
    // Form validation enhancement
    form.addEventListener('submit', function(e) {
        const servicioCheckboxes = form.querySelectorAll('input[name="ServicioInteres"]:checked');
        
        if (servicioCheckboxes.length === 0) {
            e.preventDefault();
            showValidationError('Debe seleccionar al menos un servicio de interés');
            return;
        }
        
        // Show loading state
        const submitBtn = form.querySelector('.submit-button');
        const originalText = submitBtn.textContent;
        submitBtn.textContent = 'Enviando...';
        submitBtn.disabled = true;
        
        // Reset button after form submission (if validation fails)
        setTimeout(() => {
            submitBtn.textContent = originalText;
            submitBtn.disabled = false;
        }, 3000);
    });
}

// Scroll Animations
function initializeScrollAnimations() {
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };
    
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, observerOptions);
    
    // Observe elements for animation
    const animatedElements = document.querySelectorAll('.product-card, .info-card, .faq-item');
    animatedElements.forEach(el => {
        el.style.opacity = '0';
        el.style.transform = 'translateY(30px)';
        el.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
        observer.observe(el);
    });
}

// Dropdown Menu Functionality (inherited from site.js)
function initializeDropdownMenus() {
    const dropdownContainers = document.querySelectorAll('.nav-dropdown-container');
    console.log(`Found ${dropdownContainers.length} dropdown containers`);
    
    dropdownContainers.forEach(container => {
        const trigger = container.querySelector('.nav-dropdown');
        const menu = container.querySelector('.dropdown-menu');
        
        if (!trigger || !menu) return;
        
        let hoverTimeout;
        
        // Show dropdown on hover
        container.addEventListener('mouseenter', () => {
            clearTimeout(hoverTimeout);
            menu.style.opacity = '1';
            menu.style.visibility = 'visible';
            menu.style.transform = 'translateY(0)';
            menu.style.pointerEvents = 'auto';
        });
        
        // Hide dropdown with delay
        container.addEventListener('mouseleave', () => {
            hoverTimeout = setTimeout(() => {
                menu.style.opacity = '0';
                menu.style.visibility = 'hidden';
                menu.style.transform = 'translateY(-10px)';
                menu.style.pointerEvents = 'none';
            }, 150);
        });
    });
}

// Utility Functions
function scrollToContact() {
    const contactSection = document.getElementById('contact-form');
    if (contactSection) {
        contactSection.scrollIntoView({
            behavior: 'smooth',
            block: 'start'
        });
    }
}

function showValidationError(message) {
    // Create or update error message
    let errorDiv = document.querySelector('.form-validation-error');
    if (!errorDiv) {
        errorDiv = document.createElement('div');
        errorDiv.className = 'form-validation-error error-message';
        const form = document.querySelector('.contact-form');
        form.insertBefore(errorDiv, form.firstChild);
    }
    
    errorDiv.textContent = message;
    errorDiv.scrollIntoView({ behavior: 'smooth', block: 'center' });
    
    // Remove error after 5 seconds
    setTimeout(() => {
        errorDiv.remove();
    }, 5000);
}

// Product Card Interactions
document.addEventListener('DOMContentLoaded', function() {
    const productCards = document.querySelectorAll('.product-card');
    
    productCards.forEach(card => {
        card.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-10px) scale(1.02)';
        });
        
        card.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0) scale(1)';
        });
        
        // Click handler for product cards
        card.addEventListener('click', function() {
            const title = this.querySelector('.product-title').textContent;
            scrollToContact();
            
            // Pre-select the related service
            setTimeout(() => {
                const checkboxes = document.querySelectorAll('input[name="ServicioInteres"]');
                checkboxes.forEach(cb => {
                    if (title.toLowerCase().includes('ppf') || title.toLowerCase().includes('protection')) {
                        if (cb.value === 'PPF') cb.checked = true;
                    }
                    if (title.toLowerCase().includes('cerámico') || title.toLowerCase().includes('ceramic')) {
                        if (cb.value === 'Ceramico') cb.checked = true;
                    }
                    if (title.toLowerCase().includes('combo') || title.toLowerCase().includes('completo')) {
                        if (cb.value === 'Combo') cb.checked = true;
                    }
                });
            }, 500);
        });
    });
});

// Smooth scrolling for internal links
document.addEventListener('click', function(e) {
    if (e.target.tagName === 'A' && e.target.getAttribute('href')?.startsWith('#')) {
        e.preventDefault();
        const targetId = e.target.getAttribute('href').substring(1);
        const targetElement = document.getElementById(targetId);
        
        if (targetElement) {
            targetElement.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }
    }
});

// Newsletter form handling
document.addEventListener('DOMContentLoaded', function() {
    const newsletterForm = document.querySelector('.newsletter form');
    if (newsletterForm) {
        newsletterForm.addEventListener('submit', function(e) {
            e.preventDefault();
            const email = this.querySelector('input[type="email"]').value;
            
            if (email) {
                alert('¡Gracias por suscribirse! Recibirá noticias y promociones en: ' + email);
                this.querySelector('input[type="email"]').value = '';
            }
        });
    }
});