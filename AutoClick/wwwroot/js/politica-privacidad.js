// Política de Privacidad JavaScript

document.addEventListener('DOMContentLoaded', function() {
    initSmoothScrolling();
    initAdBanner();
});

// Smooth scrolling for internal links
function initSmoothScrolling() {
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

// Ad banner interaction
function initAdBanner() {
    const adBanner = document.querySelector('.ad-banner');
    
    if (adBanner) {
        adBanner.addEventListener('click', function() {
            console.log('Ad banner clicked');
            // Add tracking or redirect logic here if needed
        });
    }
}
