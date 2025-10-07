// AutoClick.cr Site JavaScript

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    console.log('AutoClick.cr: Initializing site functionality...');
    initializeDropdownMenus();
    initializeNavigation();
    console.log('AutoClick.cr: Site functionality initialized.');
});

// Dropdown menu functionality
function initializeDropdownMenus() {
    const dropdownContainers = document.querySelectorAll('.nav-dropdown-container');
    console.log('Found dropdown containers:', dropdownContainers.length);
    
    dropdownContainers.forEach(container => {
        const dropdownLink = container.querySelector('.nav-dropdown');
        const dropdownMenu = container.querySelector('.dropdown-menu');
        
        if (dropdownLink && dropdownMenu) {
            // Initialize dropdown as hidden
            dropdownMenu.style.display = 'block';
            dropdownMenu.style.opacity = '0';
            dropdownMenu.style.visibility = 'hidden';
            dropdownMenu.style.transform = 'translateX(-50%) translateY(-10px)';
            dropdownMenu.style.pointerEvents = 'none';
            
            let showTimeout, hideTimeout;
            
            // Show dropdown on hover
            container.addEventListener('mouseenter', function() {
                clearTimeout(hideTimeout);
                showTimeout = setTimeout(() => {
                    dropdownMenu.style.opacity = '1';
                    dropdownMenu.style.visibility = 'visible';
                    dropdownMenu.style.transform = 'translateX(-50%) translateY(0)';
                    dropdownMenu.style.pointerEvents = 'auto';
                }, 50);
            });
            
            // Hide dropdown when mouse leaves
            container.addEventListener('mouseleave', function() {
                clearTimeout(showTimeout);
                hideTimeout = setTimeout(() => {
                    dropdownMenu.style.opacity = '0';
                    dropdownMenu.style.visibility = 'hidden';
                    dropdownMenu.style.transform = 'translateX(-50%) translateY(-10px)';
                    dropdownMenu.style.pointerEvents = 'none';
                }, 100);
            });
            
            // Keep dropdown open when hovering over it
            dropdownMenu.addEventListener('mouseenter', function() {
                clearTimeout(hideTimeout);
            });
            
            dropdownMenu.addEventListener('mouseleave', function() {
                hideTimeout = setTimeout(() => {
                    dropdownMenu.style.opacity = '0';
                    dropdownMenu.style.visibility = 'hidden';
                    dropdownMenu.style.transform = 'translateX(-50%) translateY(-10px)';
                    dropdownMenu.style.pointerEvents = 'none';
                }, 100);
            });
        }
    });
}

// General navigation functionality
function initializeNavigation() {
    // Smooth scrolling for anchor links
    const anchorLinks = document.querySelectorAll('a[href^="#"]');
    
    anchorLinks.forEach(link => {
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
    
    // Active page highlighting
    highlightActivePage();
}

// Highlight active page in navigation
function highlightActivePage() {
    const currentPath = window.location.pathname;
    const navLinks = document.querySelectorAll('.nav-item, .dropdown-item');
    
    navLinks.forEach(link => {
        const href = link.getAttribute('href');
        if (href && href === currentPath) {
            link.classList.add('active');
        }
    });
}

// Utility function for smooth scrolling to element
function scrollToElement(elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollIntoView({
            behavior: 'smooth',
            block: 'start'
        });
    }
}

// Global scroll to form function (for buttons)
window.scrollToForm = function() {
    scrollToElement('contact-form');
};
