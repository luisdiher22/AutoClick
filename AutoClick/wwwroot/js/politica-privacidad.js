// Política de Privacidad JavaScript Functionality

document.addEventListener('DOMContentLoaded', function() {
    // Initialize all functionality
    initSidebarNavigation();
    initSmoothScrolling();
    initSectionHighlighting();
    initNewsletterForm();
    initDataRequestFunctionality();
    initAccessibilityFeatures();
    initPrintFunctionality();
    initMobileOptimizations();
});

// Sidebar Navigation
function initSidebarNavigation() {
    const navItems = document.querySelectorAll('.nav-item');
    const sections = document.querySelectorAll('.privacy-section');

    // Add click handlers to navigation items
    navItems.forEach(item => {
        item.addEventListener('click', function(e) {
            e.preventDefault();
            
            // Remove active class from all items
            navItems.forEach(nav => nav.classList.remove('active'));
            
            // Add active class to clicked item
            this.classList.add('active');
            
            // Get target section
            const targetId = this.getAttribute('href').substring(1);
            const targetSection = document.getElementById(targetId);
            
            if (targetSection) {
                // Scroll to section with offset for fixed header
                const offset = 100;
                const elementPosition = targetSection.offsetTop;
                const offsetPosition = elementPosition - offset;
                
                window.scrollTo({
                    top: offsetPosition,
                    behavior: 'smooth'
                });
            }
        });
    });

    // Highlight current section on scroll
    window.addEventListener('scroll', throttle(function() {
        let current = '';
        const scrollPosition = window.scrollY + 150;
        
        sections.forEach(section => {
            const sectionTop = section.offsetTop;
            const sectionHeight = section.offsetHeight;
            
            if (scrollPosition >= sectionTop && scrollPosition < sectionTop + sectionHeight) {
                current = section.getAttribute('id');
            }
        });
        
        // Update active navigation item
        navItems.forEach(item => {
            item.classList.remove('active');
            if (item.getAttribute('href') === `#${current}`) {
                item.classList.add('active');
            }
        });
    }, 100));
}

// Smooth Scrolling Enhancement
function initSmoothScrolling() {
    // Handle all internal links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function(e) {
            e.preventDefault();
            const targetId = this.getAttribute('href');
            const targetElement = document.querySelector(targetId);
            
            if (targetElement) {
                const headerOffset = 100;
                const elementPosition = targetElement.getBoundingClientRect().top;
                const offsetPosition = elementPosition + window.scrollY - headerOffset;
                
                window.scrollTo({
                    top: offsetPosition,
                    behavior: 'smooth'
                });
            }
        });
    });
}

// Section Highlighting
function initSectionHighlighting() {
    const sections = document.querySelectorAll('.privacy-section');
    
    // Add subtle animation when sections come into view
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('section-visible');
            }
        });
    }, {
        threshold: 0.1,
        rootMargin: '-50px 0px'
    });
    
    sections.forEach(section => {
        observer.observe(section);
    });
    
    // Add CSS for section animation
    const style = document.createElement('style');
    style.textContent = `
        .privacy-section {
            opacity: 0.8;
            transform: translateY(10px);
            transition: opacity 0.6s ease, transform 0.6s ease;
        }
        
        .privacy-section.section-visible {
            opacity: 1;
            transform: translateY(0);
        }
        
        @media (prefers-reduced-motion: reduce) {
            .privacy-section {
                opacity: 1;
                transform: none;
                transition: none;
            }
        }
    `;
    document.head.appendChild(style);
}

// Newsletter Form Functionality
function initNewsletterForm() {
    const newsletterForm = document.getElementById('newsletterForm');
    const emailInput = document.getElementById('newsletterEmail');
    const submitBtn = newsletterForm.querySelector('.newsletter-btn');
    
    if (newsletterForm) {
        newsletterForm.addEventListener('submit', async function(e) {
            e.preventDefault();
            
            const email = emailInput.value.trim();
            
            // Validate email
            if (!isValidEmail(email)) {
                showNotification('Por favor, introduce una dirección de email válida.', 'error');
                return;
            }
            
            // Show loading state
            const originalText = submitBtn.textContent;
            submitBtn.textContent = 'Suscribiendo...';
            submitBtn.disabled = true;
            
            try {
                const response = await fetch('/PoliticaPrivacidad', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/x-www-form-urlencoded',
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                    },
                    body: `Email=${encodeURIComponent(email)}&handler=Newsletter`
                });
                
                if (response.ok) {
                    const result = await response.json();
                    if (result.success) {
                        showNotification('¡Te has suscrito exitosamente a nuestro boletín!', 'success');
                        emailInput.value = '';
                    } else {
                        showNotification(result.message || 'Error al suscribirse. Inténtalo de nuevo.', 'error');
                    }
                } else {
                    throw new Error('Network response was not ok');
                }
            } catch (error) {
                console.error('Newsletter subscription error:', error);
                showNotification('Error de conexión. Por favor, inténtalo más tarde.', 'error');
            } finally {
                submitBtn.textContent = originalText;
                submitBtn.disabled = false;
            }
        });
        
        // Real-time email validation
        emailInput.addEventListener('input', function() {
            const email = this.value.trim();
            if (email && !isValidEmail(email)) {
                this.setCustomValidity('Por favor, introduce una dirección de email válida.');
            } else {
                this.setCustomValidity('');
            }
        });
    }
}

// Data Request Functionality
function initDataRequestFunctionality() {
    // Add data request buttons to rights cards
    const rightsCards = document.querySelectorAll('.right-card');
    
    rightsCards.forEach((card, index) => {
        const rightType = ['access', 'rectification', 'deletion', 'portability', 'opposition'][index];
        
        if (rightType) {
            const button = document.createElement('button');
            button.className = 'data-request-btn';
            button.textContent = 'Solicitar';
            button.style.cssText = `
                background: #3b82f6;
                color: white;
                border: none;
                padding: 8px 16px;
                border-radius: 6px;
                font-size: 14px;
                font-weight: 500;
                cursor: pointer;
                margin-top: 10px;
                transition: background-color 0.3s ease;
            `;
            
            button.addEventListener('mouseenter', () => {
                button.style.backgroundColor = '#1d4ed8';
            });
            
            button.addEventListener('mouseleave', () => {
                button.style.backgroundColor = '#3b82f6';
            });
            
            button.addEventListener('click', () => {
                openDataRequestModal(rightType);
            });
            
            card.appendChild(button);
        }
    });
}

// Data Request Modal
function openDataRequestModal(requestType) {
    const modal = createDataRequestModal(requestType);
    document.body.appendChild(modal);
    
    // Focus trap for accessibility
    const focusableElements = modal.querySelectorAll(
        'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
    );
    const firstElement = focusableElements[0];
    const lastElement = focusableElements[focusableElements.length - 1];
    
    firstElement.focus();
    
    modal.addEventListener('keydown', (e) => {
        if (e.key === 'Escape') {
            closeModal(modal);
        }
        
        if (e.key === 'Tab') {
            if (e.shiftKey) {
                if (document.activeElement === firstElement) {
                    e.preventDefault();
                    lastElement.focus();
                }
            } else {
                if (document.activeElement === lastElement) {
                    e.preventDefault();
                    firstElement.focus();
                }
            }
        }
    });
}

function createDataRequestModal(requestType) {
    const requestTitles = {
        access: 'Solicitud de Acceso a Datos',
        rectification: 'Solicitud de Rectificación',
        deletion: 'Solicitud de Eliminación',
        portability: 'Solicitud de Portabilidad',
        opposition: 'Solicitud de Oposición'
    };
    
    const modal = document.createElement('div');
    modal.className = 'data-request-modal';
    modal.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: rgba(0, 0, 0, 0.8);
        display: flex;
        justify-content: center;
        align-items: center;
        z-index: 1000;
        animation: fadeIn 0.3s ease;
    `;
    
    const modalContent = document.createElement('div');
    modalContent.style.cssText = `
        background: #02081C;
        border: 1px solid rgba(255, 255, 255, 0.2);
        border-radius: 15px;
        padding: 30px;
        max-width: 500px;
        width: 90%;
        max-height: 80vh;
        overflow-y: auto;
    `;
    
    modalContent.innerHTML = `
        <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 25px;">
            <h3 style="color: #ffffff; font-size: 20px; font-weight: 600; margin: 0;">
                ${requestTitles[requestType]}
            </h3>
            <button class="modal-close" style="background: none; border: none; color: #ffffff; font-size: 24px; cursor: pointer; padding: 5px;">
                ×
            </button>
        </div>
        
        <form class="data-request-form">
            <div style="margin-bottom: 20px;">
                <label style="display: block; color: #ffffff; margin-bottom: 8px; font-weight: 500;">
                    Nombre completo *
                </label>
                <input type="text" name="fullName" required style="
                    width: 100%;
                    padding: 12px;
                    border: 1px solid rgba(255, 255, 255, 0.3);
                    border-radius: 6px;
                    background: rgba(255, 255, 255, 0.05);
                    color: #ffffff;
                    font-size: 16px;
                ">
            </div>
            
            <div style="margin-bottom: 20px;">
                <label style="display: block; color: #ffffff; margin-bottom: 8px; font-weight: 500;">
                    Email *
                </label>
                <input type="email" name="email" required style="
                    width: 100%;
                    padding: 12px;
                    border: 1px solid rgba(255, 255, 255, 0.3);
                    border-radius: 6px;
                    background: rgba(255, 255, 255, 0.05);
                    color: #ffffff;
                    font-size: 16px;
                ">
            </div>
            
            <div style="margin-bottom: 20px;">
                <label style="display: block; color: #ffffff; margin-bottom: 8px; font-weight: 500;">
                    Descripción de la solicitud *
                </label>
                <textarea name="description" required rows="4" style="
                    width: 100%;
                    padding: 12px;
                    border: 1px solid rgba(255, 255, 255, 0.3);
                    border-radius: 6px;
                    background: rgba(255, 255, 255, 0.05);
                    color: #ffffff;
                    font-size: 16px;
                    resize: vertical;
                "></textarea>
            </div>
            
            <div style="display: flex; gap: 15px; justify-content: flex-end; margin-top: 30px;">
                <button type="button" class="modal-cancel" style="
                    background: transparent;
                    color: #ffffff;
                    border: 1px solid rgba(255, 255, 255, 0.3);
                    padding: 12px 24px;
                    border-radius: 6px;
                    cursor: pointer;
                    font-size: 16px;
                ">
                    Cancelar
                </button>
                <button type="submit" style="
                    background: #3b82f6;
                    color: #ffffff;
                    border: none;
                    padding: 12px 24px;
                    border-radius: 6px;
                    cursor: pointer;
                    font-size: 16px;
                    font-weight: 500;
                ">
                    Enviar Solicitud
                </button>
            </div>
        </form>
    `;
    
    modal.appendChild(modalContent);
    
    // Event listeners
    const closeBtn = modalContent.querySelector('.modal-close');
    const cancelBtn = modalContent.querySelector('.modal-cancel');
    const form = modalContent.querySelector('.data-request-form');
    
    closeBtn.addEventListener('click', () => closeModal(modal));
    cancelBtn.addEventListener('click', () => closeModal(modal));
    
    modal.addEventListener('click', (e) => {
        if (e.target === modal) {
            closeModal(modal);
        }
    });
    
    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        await handleDataRequest(form, requestType, modal);
    });
    
    return modal;
}

async function handleDataRequest(form, requestType, modal) {
    const formData = new FormData(form);
    const submitBtn = form.querySelector('button[type="submit"]');
    const originalText = submitBtn.textContent;
    
    submitBtn.textContent = 'Enviando...';
    submitBtn.disabled = true;
    
    try {
        const response = await fetch('/PoliticaPrivacidad', {
            method: 'POST',
            headers: {
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: new URLSearchParams({
                fullName: formData.get('fullName'),
                email: formData.get('email'),
                description: formData.get('description'),
                requestType: requestType,
                handler: 'DataRequest'
            })
        });
        
        if (response.ok) {
            const result = await response.json();
            if (result.success) {
                showNotification('Solicitud enviada correctamente. Te contactaremos pronto.', 'success');
                closeModal(modal);
            } else {
                showNotification(result.message || 'Error al enviar la solicitud.', 'error');
            }
        } else {
            throw new Error('Network response was not ok');
        }
    } catch (error) {
        console.error('Data request error:', error);
        showNotification('Error de conexión. Por favor, inténtalo más tarde.', 'error');
    } finally {
        submitBtn.textContent = originalText;
        submitBtn.disabled = false;
    }
}

function closeModal(modal) {
    modal.style.animation = 'fadeOut 0.3s ease';
    setTimeout(() => {
        if (modal.parentNode) {
            modal.parentNode.removeChild(modal);
        }
    }, 300);
}

// Accessibility Features
function initAccessibilityFeatures() {
    // Add skip link for keyboard users
    const skipLink = document.createElement('a');
    skipLink.href = '#main-content';
    skipLink.textContent = 'Saltar al contenido principal';
    skipLink.style.cssText = `
        position: absolute;
        top: -40px;
        left: 6px;
        background: #3b82f6;
        color: white;
        padding: 8px;
        text-decoration: none;
        border-radius: 4px;
        z-index: 1001;
        transition: top 0.3s ease;
    `;
    
    skipLink.addEventListener('focus', () => {
        skipLink.style.top = '6px';
    });
    
    skipLink.addEventListener('blur', () => {
        skipLink.style.top = '-40px';
    });
    
    document.body.insertBefore(skipLink, document.body.firstChild);
    
    // Add main landmark if not exists
    const mainContent = document.querySelector('.privacy-main');
    if (mainContent && !mainContent.id) {
        mainContent.id = 'main-content';
        mainContent.setAttribute('role', 'main');
    }
    
    // Enhance keyboard navigation
    const navItems = document.querySelectorAll('.nav-item');
    navItems.forEach((item, index) => {
        item.setAttribute('role', 'menuitem');
        item.setAttribute('tabindex', '0');
        
        item.addEventListener('keydown', (e) => {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                item.click();
            }
            
            if (e.key === 'ArrowDown') {
                e.preventDefault();
                const nextItem = navItems[index + 1] || navItems[0];
                nextItem.focus();
            }
            
            if (e.key === 'ArrowUp') {
                e.preventDefault();
                const prevItem = navItems[index - 1] || navItems[navItems.length - 1];
                prevItem.focus();
            }
        });
    });
}

// Print Functionality
function initPrintFunctionality() {
    // Add print button
    const printBtn = document.createElement('button');
    printBtn.innerHTML = '🖨️ Imprimir Política';
    printBtn.style.cssText = `
        position: fixed;
        bottom: 30px;
        right: 30px;
        background: #3b82f6;
        color: white;
        border: none;
        padding: 15px 20px;
        border-radius: 50px;
        font-size: 14px;
        font-weight: 500;
        cursor: pointer;
        box-shadow: 0 4px 15px rgba(59, 130, 246, 0.3);
        transition: all 0.3s ease;
        z-index: 100;
    `;
    
    printBtn.addEventListener('mouseenter', () => {
        printBtn.style.transform = 'translateY(-2px)';
        printBtn.style.boxShadow = '0 6px 20px rgba(59, 130, 246, 0.4)';
    });
    
    printBtn.addEventListener('mouseleave', () => {
        printBtn.style.transform = 'translateY(0)';
        printBtn.style.boxShadow = '0 4px 15px rgba(59, 130, 246, 0.3)';
    });
    
    printBtn.addEventListener('click', () => {
        window.print();
    });
    
    document.body.appendChild(printBtn);
    
    // Hide print button on mobile
    if (window.innerWidth <= 768) {
        printBtn.style.display = 'none';
    }
    
    window.addEventListener('resize', () => {
        printBtn.style.display = window.innerWidth <= 768 ? 'none' : 'block';
    });
}

// Mobile Optimizations
function initMobileOptimizations() {
    const sidebar = document.querySelector('.privacy-sidebar');
    const mainContent = document.querySelector('.privacy-main');
    
    if (window.innerWidth <= 1024) {
        // Create mobile navigation toggle
        const mobileToggle = document.createElement('button');
        mobileToggle.innerHTML = '📋 Índice de Contenidos';
        mobileToggle.style.cssText = `
            width: 100%;
            background: #3b82f6;
            color: white;
            border: none;
            padding: 15px;
            border-radius: 8px;
            font-size: 16px;
            font-weight: 500;
            cursor: pointer;
            margin-bottom: 20px;
            display: block;
        `;
        
        const navContent = sidebar.querySelector('.sidebar-content');
        navContent.style.display = 'none';
        
        mobileToggle.addEventListener('click', () => {
            const isHidden = navContent.style.display === 'none';
            navContent.style.display = isHidden ? 'block' : 'none';
            mobileToggle.innerHTML = isHidden ? '❌ Cerrar Índice' : '📋 Índice de Contenidos';
        });
        
        sidebar.insertBefore(mobileToggle, navContent);
    }
    
    // Optimize touch interactions
    const navItems = document.querySelectorAll('.nav-item');
    navItems.forEach(item => {
        item.style.minHeight = '44px';
        item.style.display = 'flex';
        item.style.alignItems = 'center';
    });
}

// Notification System
function showNotification(message, type = 'info') {
    // Remove existing notifications
    const existingNotifications = document.querySelectorAll('.privacy-notification');
    existingNotifications.forEach(notification => notification.remove());
    
    const notification = document.createElement('div');
    notification.className = 'privacy-notification';
    
    const bgColor = type === 'success' ? '#10b981' : type === 'error' ? '#ef4444' : '#3b82f6';
    
    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: ${bgColor};
        color: white;
        padding: 15px 20px;
        border-radius: 8px;
        font-size: 14px;
        font-weight: 500;
        box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
        z-index: 1001;
        animation: slideInRight 0.3s ease;
        max-width: 350px;
    `;
    
    notification.textContent = message;
    document.body.appendChild(notification);
    
    // Auto remove after 5 seconds
    setTimeout(() => {
        notification.style.animation = 'slideOutRight 0.3s ease';
        setTimeout(() => {
            if (notification.parentNode) {
                notification.parentNode.removeChild(notification);
            }
        }, 300);
    }, 5000);
    
    // Add click to dismiss
    notification.addEventListener('click', () => {
        notification.style.animation = 'slideOutRight 0.3s ease';
        setTimeout(() => {
            if (notification.parentNode) {
                notification.parentNode.removeChild(notification);
            }
        }, 300);
    });
}

// Utility Functions
function isValidEmail(email) {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
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

// Add CSS animations
const animationStyles = document.createElement('style');
animationStyles.textContent = `
    @keyframes fadeIn {
        from { opacity: 0; }
        to { opacity: 1; }
    }
    
    @keyframes fadeOut {
        from { opacity: 1; }
        to { opacity: 0; }
    }
    
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
`;
document.head.appendChild(animationStyles);

// Performance optimization: Lazy load heavy content
function initLazyLoading() {
    const sections = document.querySelectorAll('.privacy-section');
    
    const sectionObserver = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('loaded');
                sectionObserver.unobserve(entry.target);
            }
        });
    }, {
        rootMargin: '50px'
    });
    
    sections.forEach(section => {
        sectionObserver.observe(section);
    });
}

// Initialize lazy loading
initLazyLoading();

// Export functions for external use
window.PrivacyPolicyJS = {
    showNotification,
    openDataRequestModal,
    initMobileOptimizations
};