// Términos y Condiciones - Simple JavaScript// Términos y Condiciones JavaScript Functionality



document.addEventListener('DOMContentLoaded', function() {document.addEventListener('DOMContentLoaded', function() {

    // Initialize smooth scrolling    // Initialize components

    initSmoothScrolling();    initSidebarNavigation();

        initSmoothScrolling();

    // Initialize ad banner click    initSectionHighlighting();

    initAdBanner();    initNewsletterForm();

        initPrintFunctionality();

    console.log('Términos y Condiciones page initialized');    initSearchFunctionality();

});    

    console.log('Términos y Condiciones page initialized successfully');

// Smooth scrolling for anchor links});

function initSmoothScrolling() {

    document.querySelectorAll('a[href^="#"]').forEach(anchor => {// Sidebar Navigation

        anchor.addEventListener('click', function (e) {function initSidebarNavigation() {

            e.preventDefault();    const navItems = document.querySelectorAll('.nav-item');

            const target = document.querySelector(this.getAttribute('href'));    const sections = document.querySelectorAll('.terms-section');

                

            if (target) {    // Handle navigation clicks

                const headerOffset = 100;    navItems.forEach(item => {

                const elementPosition = target.getBoundingClientRect().top;        item.addEventListener('click', function(e) {

                const offsetPosition = elementPosition + window.pageYOffset - headerOffset;            e.preventDefault();

            

                window.scrollTo({            const targetId = this.getAttribute('href').substring(1);

                    top: offsetPosition,            const targetSection = document.getElementById(targetId);

                    behavior: 'smooth'            

                });            if (targetSection) {

            }                // Update active state

        });                updateActiveNavItem(this);

    });                

}                // Scroll to section

                scrollToSection(targetSection);

// Ad banner functionality                

function initAdBanner() {                // Update URL without triggering page reload

    const adBanner = document.querySelector('.ad-banner');                updateURL(targetId);

                }

    if (adBanner) {        });

        adBanner.addEventListener('click', function() {    });

            // Redirect to advertising page or contact page    

            window.location.href = '/Contacto';    // Handle direct URL navigation

        });    handleDirectNavigation();

    }}

}

// Update active navigation item

// Print functionalityfunction updateActiveNavItem(clickedItem) {

function printTerms() {    document.querySelectorAll('.nav-item').forEach(item => {

    window.print();        item.classList.remove('active');

}    });

    clickedItem.classList.add('active');

// Export for testing}

if (typeof module !== 'undefined' && module.exports) {

    module.exports = {// Smooth scrolling to sections

        printTermsfunction initSmoothScrolling() {

    };    // Polyfill for browsers that don't support smooth scrolling

}    if (!CSS.supports('scroll-behavior', 'smooth')) {

        enableSmoothScrollPolyfill();
    }
}

function scrollToSection(targetSection) {
    const headerOffset = 100; // Account for fixed header
    const elementPosition = targetSection.getBoundingClientRect().top;
    const offsetPosition = elementPosition + window.pageYOffset - headerOffset;

    window.scrollTo({
        top: offsetPosition,
        behavior: 'smooth'
    });
}

function enableSmoothScrollPolyfill() {
    // Simple smooth scroll polyfill
    function smoothScrollTo(element, target, duration) {
        const start = element.scrollTop;
        const change = target - start;
        const startDate = +new Date();
        
        const easeInOutQuart = function(t, b, c, d) {
            t /= d / 2;
            if (t < 1) return c / 2 * t * t * t * t + b;
            return -c / 2 * ((t -= 2) * t * t * t - 2) + b;
        };
        
        const animateScroll = function() {
            const currentDate = +new Date();
            const currentTime = currentDate - startDate;
            element.scrollTop = parseInt(easeInOutQuart(currentTime, start, change, duration));
            if (currentTime < duration) {
                requestAnimationFrame(animateScroll);
            } else {
                element.scrollTop = target;
            }
        };
        
        animateScroll();
    }
    
    window.scrollToSmooth = function(target) {
        smoothScrollTo(document.documentElement, target, 500);
    };
}

// Section highlighting on scroll
function initSectionHighlighting() {
    const sections = document.querySelectorAll('.terms-section');
    const navItems = document.querySelectorAll('.nav-item');
    
    function highlightCurrentSection() {
        const scrollPos = window.scrollY + 150;
        
        sections.forEach((section, index) => {
            const sectionTop = section.offsetTop;
            const sectionBottom = sectionTop + section.offsetHeight;
            
            if (scrollPos >= sectionTop && scrollPos < sectionBottom) {
                // Update navigation
                navItems.forEach(item => item.classList.remove('active'));
                if (navItems[index]) {
                    navItems[index].classList.add('active');
                }
                
                // Update URL
                updateURL(section.id, false);
            }
        });
    }
    
    // Throttle scroll events for performance
    let scrollTimer;
    window.addEventListener('scroll', function() {
        if (scrollTimer) {
            cancelAnimationFrame(scrollTimer);
        }
        scrollTimer = requestAnimationFrame(highlightCurrentSection);
    });
}

// URL management
function updateURL(sectionId, pushState = true) {
    const newURL = `${window.location.pathname}#${sectionId}`;
    
    if (pushState && history.pushState) {
        history.pushState(null, null, newURL);
    } else if (!pushState && history.replaceState) {
        history.replaceState(null, null, newURL);
    }
}

function handleDirectNavigation() {
    const hash = window.location.hash;
    
    if (hash) {
        const targetSection = document.querySelector(hash);
        const targetNavItem = document.querySelector(`.nav-item[href="${hash}"]`);
        
        if (targetSection && targetNavItem) {
            setTimeout(() => {
                updateActiveNavItem(targetNavItem);
                scrollToSection(targetSection);
            }, 100);
        }
    }
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
    
    // Style the message
    Object.assign(messageDiv.style, {
        position: 'fixed',
        top: '20px',
        right: '20px',
        backgroundColor: type === 'success' ? '#10b981' : '#ef4444',
        color: '#ffffff',
        padding: '12px 20px',
        borderRadius: '8px',
        boxShadow: '0 4px 12px rgba(0, 0, 0, 0.1)',
        zIndex: '9999',
        fontWeight: '500',
        fontSize: '0.9rem'
    });
    
    document.body.appendChild(messageDiv);
    
    // Auto-remove after 4 seconds
    setTimeout(() => {
        if (messageDiv.parentNode) {
            messageDiv.remove();
        }
    }, 4000);
}

// Print Functionality
function initPrintFunctionality() {
    // Add print button
    const printButton = createPrintButton();
    const header = document.querySelector('.terminos-header');
    if (header) {
        header.appendChild(printButton);
    }
    
    // Handle print styles
    window.addEventListener('beforeprint', function() {
        console.log('Preparing page for printing...');
        
        // Expand all collapsible sections
        expandAllSections();
    });
    
    window.addEventListener('afterprint', function() {
        console.log('Print dialog closed');
    });
}

function createPrintButton() {
    const button = document.createElement('button');
    button.innerHTML = '🖨️ Imprimir Términos';
    button.className = 'print-button';
    
    // Style the button
    Object.assign(button.style, {
        backgroundColor: '#3b82f6',
        color: '#ffffff',
        border: 'none',
        padding: '10px 20px',
        borderRadius: '6px',
        fontSize: '0.9rem',
        fontWeight: '500',
        cursor: 'pointer',
        marginTop: '15px',
        transition: 'background-color 0.3s ease'
    });
    
    button.addEventListener('mouseover', function() {
        this.style.backgroundColor = '#1d4ed8';
    });
    
    button.addEventListener('mouseout', function() {
        this.style.backgroundColor = '#3b82f6';
    });
    
    button.addEventListener('click', function() {
        window.print();
    });
    
    return button;
}

function expandAllSections() {
    // Expand any collapsible content for printing
    const collapsibleElements = document.querySelectorAll('[data-collapsible]');
    collapsibleElements.forEach(element => {
        element.style.display = 'block';
    });
}

// Search Functionality
function initSearchFunctionality() {
    const searchContainer = createSearchContainer();
    const sidebar = document.querySelector('.sidebar-content');
    
    if (sidebar) {
        sidebar.appendChild(searchContainer);
    }
}

function createSearchContainer() {
    const container = document.createElement('div');
    container.className = 'search-container';
    container.style.marginTop = '20px';
    
    const input = document.createElement('input');
    input.type = 'text';
    input.placeholder = 'Buscar en términos...';
    input.className = 'search-input';
    
    // Style the input
    Object.assign(input.style, {
        width: '100%',
        padding: '10px 12px',
        border: '1px solid rgba(255, 255, 255, 0.3)',
        borderRadius: '6px',
        backgroundColor: 'rgba(255, 255, 255, 0.1)',
        color: '#ffffff',
        fontSize: '0.9rem'
    });
    
    input.addEventListener('input', function() {
        performSearch(this.value);
    });
    
    container.appendChild(input);
    return container;
}

function performSearch(query) {
    const sections = document.querySelectorAll('.terms-section');
    const navItems = document.querySelectorAll('.nav-item');
    
    if (!query.trim()) {
        // Reset all sections and nav items
        sections.forEach(section => {
            section.style.display = 'block';
        });
        navItems.forEach(item => {
            item.style.display = 'block';
        });
        return;
    }
    
    const queryLower = query.toLowerCase();
    let hasResults = false;
    
    sections.forEach((section, index) => {
        const content = section.textContent.toLowerCase();
        const navItem = navItems[index];
        
        if (content.includes(queryLower)) {
            section.style.display = 'block';
            if (navItem) navItem.style.display = 'block';
            
            // Highlight matching text
            highlightSearchTerm(section, query);
            hasResults = true;
        } else {
            section.style.display = 'none';
            if (navItem) navItem.style.display = 'none';
        }
    });
    
    if (!hasResults) {
        showNoResultsMessage();
    } else {
        removeNoResultsMessage();
    }
}

function highlightSearchTerm(section, term) {
    // Remove existing highlights
    const highlighted = section.querySelectorAll('.search-highlight');
    highlighted.forEach(element => {
        const parent = element.parentNode;
        parent.replaceChild(document.createTextNode(element.textContent), element);
        parent.normalize();
    });
    
    if (!term.trim()) return;
    
    // Add new highlights
    const walker = document.createTreeWalker(
        section,
        NodeFilter.SHOW_TEXT,
        null,
        false
    );
    
    const textNodes = [];
    let node;
    
    while (node = walker.nextNode()) {
        textNodes.push(node);
    }
    
    textNodes.forEach(textNode => {
        const text = textNode.textContent;
        const regex = new RegExp(`(${escapeRegex(term)})`, 'gi');
        
        if (regex.test(text)) {
            const highlightedHTML = text.replace(regex, '<span class="search-highlight" style="background-color: #fef08a; color: #92400e; font-weight: 600;">$1</span>');
            
            const tempDiv = document.createElement('div');
            tempDiv.innerHTML = highlightedHTML;
            
            while (tempDiv.firstChild) {
                textNode.parentNode.insertBefore(tempDiv.firstChild, textNode);
            }
            
            textNode.parentNode.removeChild(textNode);
        }
    });
}

function escapeRegex(string) {
    return string.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

function showNoResultsMessage() {
    removeNoResultsMessage();
    
    const message = document.createElement('div');
    message.className = 'no-results-message';
    message.textContent = 'No se encontraron resultados para tu búsqueda';
    message.style.cssText = `
        text-align: center;
        padding: 20px;
        color: #64748b;
        font-style: italic;
        margin-top: 20px;
    `;
    
    const mainContent = document.querySelector('.terminos-main');
    if (mainContent) {
        mainContent.appendChild(message);
    }
}

function removeNoResultsMessage() {
    const message = document.querySelector('.no-results-message');
    if (message) {
        message.remove();
    }
}

// Accessibility enhancements
function initAccessibility() {
    // Add skip links
    addSkipLinks();
    
    // Enhance keyboard navigation
    enhanceKeyboardNavigation();
    
    // Add ARIA labels
    addAriaLabels();
}

function addSkipLinks() {
    const skipLinks = document.createElement('div');
    skipLinks.className = 'skip-links';
    skipLinks.innerHTML = `
        <a href="#main-content" class="skip-link">Ir al contenido principal</a>
        <a href="#navigation" class="skip-link">Ir a la navegación</a>
    `;
    
    // Style skip links
    const style = document.createElement('style');
    style.textContent = `
        .skip-links {
            position: absolute;
            top: -40px;
            left: 0;
            right: 0;
            z-index: 10000;
        }
        
        .skip-link {
            position: absolute;
            top: -40px;
            left: 6px;
            background: #000;
            color: #fff;
            padding: 8px;
            text-decoration: none;
            border-radius: 0 0 4px 4px;
        }
        
        .skip-link:focus {
            top: 0;
        }
    `;
    
    document.head.appendChild(style);
    document.body.insertBefore(skipLinks, document.body.firstChild);
}

function enhanceKeyboardNavigation() {
    const navItems = document.querySelectorAll('.nav-item');
    
    navItems.forEach((item, index) => {
        item.addEventListener('keydown', function(e) {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                this.click();
            }
            
            if (e.key === 'ArrowDown' || e.key === 'ArrowUp') {
                e.preventDefault();
                const direction = e.key === 'ArrowDown' ? 1 : -1;
                const nextIndex = (index + direction + navItems.length) % navItems.length;
                navItems[nextIndex].focus();
            }
        });
    });
}

function addAriaLabels() {
    const sidebar = document.querySelector('.terminos-sidebar');
    if (sidebar) {
        sidebar.setAttribute('role', 'navigation');
        sidebar.setAttribute('aria-label', 'Navegación de términos y condiciones');
    }
    
    const mainContent = document.querySelector('.terminos-main');
    if (mainContent) {
        mainContent.setAttribute('role', 'main');
        mainContent.setAttribute('id', 'main-content');
    }
    
    const nav = document.querySelector('.terms-nav');
    if (nav) {
        nav.setAttribute('id', 'navigation');
    }
}

// Initialize accessibility when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    initAccessibility();
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

// Export functions for testing
if (typeof module !== 'undefined' && module.exports) {
    module.exports = {
        updateActiveNavItem,
        scrollToSection,
        performSearch,
        showStatusMessage
    };
}