// Sobre Nosotros JavaScript Functionality

document.addEventListener('DOMContentLoaded', function() {
    initializeNewsletterForm();
    initializeStatusMessages();
    initializeSmoothScrolling();
    initializeAnimations();
});

// Newsletter form functionality
function initializeNewsletterForm() {
    const form = document.querySelector('.newsletter-form');
    const emailInput = document.querySelector('.newsletter-input');
    
    if (form && emailInput) {
        // Email validation on input
        emailInput.addEventListener('input', function() {
            validateEmail(this);
        });
        
        // Form submission handling
        form.addEventListener('submit', function(e) {
            const email = emailInput.value.trim();
            
            if (!email) {
                e.preventDefault();
                showStatusMessage('Por favor ingrese su correo electrónico', 'error');
                emailInput.focus();
                return false;
            }
            
            if (!isValidEmail(email)) {
                e.preventDefault();
                showStatusMessage('Por favor ingrese un correo electrónico válido', 'error');
                emailInput.focus();
                return false;
            }
            
            // Show loading state
            const submitBtn = form.querySelector('.newsletter-btn');
            if (submitBtn) {
                submitBtn.textContent = 'Suscribiendo...';
                submitBtn.disabled = true;
            }
        });
    }
}

// Email validation
function validateEmail(input) {
    const email = input.value.trim();
    
    if (email && !isValidEmail(email)) {
        input.style.borderColor = '#EF4444';
    } else {
        input.style.borderColor = '';
    }
}

function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
}

// Status messages functionality
function initializeStatusMessages() {
    // Show status messages from server
    const statusMessage = document.querySelector('[data-status-message]');
    const errorMessage = document.querySelector('[data-error-message]');
    
    if (statusMessage) {
        showStatusMessage(statusMessage.textContent, 'success');
    }
    
    if (errorMessage) {
        showStatusMessage(errorMessage.textContent, 'error');
    }
    
    // Auto-hide existing status messages
    const existingMessages = document.querySelectorAll('.status-message');
    existingMessages.forEach(message => {
        setTimeout(() => {
            hideStatusMessage(message);
        }, 5000);
    });
}

function showStatusMessage(message, type = 'success') {
    // Remove existing messages
    const existingMessages = document.querySelectorAll('.status-message');
    existingMessages.forEach(msg => hideStatusMessage(msg));
    
    // Create new message
    const messageElement = document.createElement('div');
    messageElement.className = `status-message ${type}`;
    messageElement.textContent = message;
    
    document.body.appendChild(messageElement);
    
    // Auto-hide after 5 seconds
    setTimeout(() => {
        hideStatusMessage(messageElement);
    }, 5000);
    
    // Click to dismiss
    messageElement.addEventListener('click', () => {
        hideStatusMessage(messageElement);
    });
}

function hideStatusMessage(messageElement) {
    if (messageElement && messageElement.parentNode) {
        messageElement.style.animation = 'slideOutRight 0.3s ease';
        setTimeout(() => {
            if (messageElement.parentNode) {
                messageElement.parentNode.removeChild(messageElement);
            }
        }, 300);
    }
}

// Smooth scrolling functionality
function initializeSmoothScrolling() {
    // Add smooth scrolling to anchor links
    const links = document.querySelectorAll('a[href^="#"]');
    
    links.forEach(link => {
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
}

// Animations functionality
function initializeAnimations() {
    // Intersection Observer for scroll animations
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };
    
    const observer = new IntersectionObserver(function(entries) {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animate-in');
            }
        });
    }, observerOptions);
    
    // Observe content sections
    const sections = document.querySelectorAll('.content-section');
    sections.forEach(section => {
        section.classList.add('animate-ready');
        observer.observe(section);
    });
    
    // Add parallax effect to background text (optional enhancement)
    window.addEventListener('scroll', function() {
        const scrolled = window.pageYOffset;
        const backgroundTexts = document.querySelectorAll('.section-background-text');
        
        backgroundTexts.forEach(text => {
            const speed = 0.1;
            text.style.transform = `translateY(${scrolled * speed}px)`;
        });
    });
}

// Social media links functionality
function initializeSocialLinks() {
    const socialIcons = document.querySelectorAll('.social-icon');
    
    socialIcons.forEach(icon => {
        icon.addEventListener('click', function() {
            let url = '';
            
            if (this.classList.contains('facebook-icon')) {
                url = 'https://facebook.com/autoclick.cr';
            } else if (this.classList.contains('instagram-icon')) {
                url = 'https://instagram.com/autoclick.cr';
            } else if (this.classList.contains('whatsapp-icon')) {
                url = 'https://wa.me/50612345678';
            }
            
            if (url) {
                window.open(url, '_blank', 'noopener,noreferrer');
            }
        });
        
        // Add hover effect
        icon.addEventListener('mouseenter', function() {
            this.style.transform = 'scale(1.1)';
            this.style.transition = 'transform 0.2s ease';
        });
        
        icon.addEventListener('mouseleave', function() {
            this.style.transform = 'scale(1)';
        });
    });
}

// Newsletter success handling
function handleNewsletterSuccess() {
    const form = document.querySelector('.newsletter-form');
    const emailInput = document.querySelector('.newsletter-input');
    const submitBtn = form?.querySelector('.newsletter-btn');
    
    if (submitBtn) {
        submitBtn.textContent = 'Suscribirse';
        submitBtn.disabled = false;
    }
    
    if (emailInput) {
        emailInput.value = '';
    }
}

// Initialize social links when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    initializeSocialLinks();
});

// Add CSS animations
const style = document.createElement('style');
style.textContent = `
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
    
    .animate-ready {
        opacity: 0;
        transform: translateY(30px);
        transition: all 0.6s ease-out;
    }
    
    .animate-in {
        opacity: 1;
        transform: translateY(0);
    }
    
    .social-icon {
        cursor: pointer;
        transition: transform 0.2s ease;
    }
    
    .social-icon:hover {
        transform: scale(1.1);
    }
    
    .newsletter-btn:disabled {
        opacity: 0.6;
        cursor: not-allowed;
    }
    
    .newsletter-input.error {
        border-color: #EF4444 !important;
        box-shadow: 0 0 0 2px rgba(239, 68, 68, 0.1);
    }
`;
document.head.appendChild(style);