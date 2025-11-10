// Raloy Detailing Page JavaScript

document.addEventListener('DOMContentLoaded', function() {
    // Initialize all interactive features
    initializeFAQ();
    initializeContactForm();
    initializeTestimonialSlider();
    initializeGallery();
    initializeScrollAnimations();
    initializeDropdownMenus();
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

// Testimonial Slider
function initializeTestimonialSlider() {
    const track = document.querySelector('.testimonials-track');
    const navBtn = document.querySelector('.slider-nav-btn');
    const cards = document.querySelectorAll('.testimonial-card');
    
    if (!track || !navBtn || cards.length === 0) return;
    
    let currentIndex = 0;
    const totalCards = cards.length;
    const cardWidth = 825; // 805px + 20px gap
    
    // Auto-slide functionality
    function nextSlide() {
        currentIndex = (currentIndex + 1) % totalCards;
        updateSlider();
    }
    
    function updateSlider() {
        const translateX = -currentIndex * cardWidth;
        track.style.transform = `translateX(${translateX}px)`;
    }
    
    // Manual navigation
    navBtn.addEventListener('click', nextSlide);
    
    // Auto-slide every 5 seconds
    setInterval(nextSlide, 5000);
    
    // Testimonial interactions
    initializeTestimonialInteractions();
}

// Testimonial Interactions
function initializeTestimonialInteractions() {
    const likeButtons = document.querySelectorAll('.like-btn');
    const commentInputs = document.querySelectorAll('.comment-input');
    
    // Like button interactions
    likeButtons.forEach(btn => {
        btn.addEventListener('click', function() {
            this.classList.toggle('active');
            
            // Add like animation
            const icon = this.querySelector('.like-icon');
            icon.style.transform = 'scale(1.2)';
            setTimeout(() => {
                icon.style.transform = 'scale(1)';
            }, 150);
        });
    });
    
    // Comment input interactions
    commentInputs.forEach(input => {
        input.addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                const comment = this.value.trim();
                if (comment) {
                    addComment(comment, this);
                    this.value = '';
                }
            }
        });
    });
}

function addComment(text, inputElement) {
    // Create new comment element
    const commentItem = document.createElement('div');
    commentItem.className = 'comment-item';
    
    commentItem.innerHTML = `
        <img src="https://placehold.co/40x40" alt="Your avatar" class="comment-avatar" />
        <div class="comment-content">
            <div class="comment-bubble">
                <div class="comment-author">Usted</div>
                <div class="comment-text">${text}</div>
            </div>
            <div class="comment-actions">
                <span>Like</span>
                <span>Reply</span>
                <span>Ahora</span>
            </div>
        </div>
    `;
    
    // Insert before the add-comment section
    const addComment = inputElement.closest('.add-comment');
    addComment.parentNode.insertBefore(commentItem, addComment);
    
    // Animate new comment
    commentItem.style.opacity = '0';
    commentItem.style.transform = 'translateY(20px)';
    setTimeout(() => {
        commentItem.style.transition = 'all 0.3s ease';
        commentItem.style.opacity = '1';
        commentItem.style.transform = 'translateY(0)';
    }, 100);
}

// Gallery Functionality
function initializeGallery() {
    const mainImage = document.querySelector('.main-image');
    const thumbnails = document.querySelectorAll('.thumbnail-image');
    
    // Click on thumbnails to change main image
    thumbnails.forEach((thumbnail, index) => {
        thumbnail.addEventListener('click', function() {
            const tempSrc = mainImage.src;
            mainImage.src = this.src;
            this.src = tempSrc;
            
            // Add click animation
            this.style.transform = 'scale(0.95)';
            setTimeout(() => {
                this.style.transform = 'scale(1)';
            }, 150);
        });
    });
    
    // Lightbox functionality for main image
    mainImage.addEventListener('click', function() {
        openLightbox(this.src);
    });
}

function openLightbox(imageSrc) {
    // Create lightbox overlay
    const lightbox = document.createElement('div');
    lightbox.className = 'lightbox-overlay';
    lightbox.innerHTML = `
        <div class="lightbox-container">
            <img src="${imageSrc}" alt="Gallery Image" class="lightbox-image" />
            <button class="lightbox-close">&times;</button>
        </div>
    `;
    
    // Add lightbox styles
    lightbox.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        width: 100vw;
        height: 100vh;
        background: rgba(0, 0, 0, 0.9);
        z-index: 9999;
        display: flex;
        align-items: center;
        justify-content: center;
        opacity: 0;
        transition: opacity 0.3s ease;
    `;
    
    const container = lightbox.querySelector('.lightbox-container');
    container.style.cssText = `
        position: relative;
        max-width: 90vw;
        max-height: 90vh;
    `;
    
    const image = lightbox.querySelector('.lightbox-image');
    image.style.cssText = `
        max-width: 100%;
        max-height: 100%;
        object-fit: contain;
    `;
    
    const closeBtn = lightbox.querySelector('.lightbox-close');
    closeBtn.style.cssText = `
        position: absolute;
        top: -40px;
        right: 0;
        background: none;
        border: none;
        color: white;
        font-size: 30px;
        cursor: pointer;
        padding: 10px;
    `;
    
    document.body.appendChild(lightbox);
    
    // Show lightbox
    setTimeout(() => {
        lightbox.style.opacity = '1';
    }, 10);
    
    // Close lightbox
    function closeLightbox() {
        lightbox.style.opacity = '0';
        setTimeout(() => {
            document.body.removeChild(lightbox);
        }, 300);
    }
    
    closeBtn.addEventListener('click', closeLightbox);
    lightbox.addEventListener('click', function(e) {
        if (e.target === lightbox) {
            closeLightbox();
        }
    });
    
    document.addEventListener('keydown', function(e) {
        if (e.key === 'Escape') {
            closeLightbox();
        }
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
                
                // Special animation for feature items
                if (entry.target.classList.contains('feature-item')) {
                    const icons = entry.target.querySelectorAll('.feature-icon-small, .feature-icon-large');
                    icons.forEach((icon, index) => {
                        setTimeout(() => {
                            icon.style.transform = 'scale(1.1)';
                            setTimeout(() => {
                                icon.style.transform = 'scale(1)';
                            }, 200);
                        }, index * 100);
                    });
                }
            }
        });
    }, observerOptions);
    
    // Observe elements for animation
    const animatedElements = document.querySelectorAll('.feature-item, .testimonial-card, .faq-item, .thumbnail-image');
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

// Video Player Functionality
document.addEventListener('DOMContentLoaded', function() {
    const videoPlaceholder = document.querySelector('.video-placeholder');
    
    if (videoPlaceholder) {
        videoPlaceholder.addEventListener('click', function() {
            // Replace with actual video embed or open in modal
            alert('Aquí se reproduciría el video de demostración de Raloy Detailing');
        });
    }
});

// Hero CTA Interactions
document.addEventListener('DOMContentLoaded', function() {
    const heroCta = document.querySelector('.hero-cta-btn');
    const finalCta = document.querySelector('.final-cta-btn');
    
    // Add ripple effect to CTA buttons
    [heroCta, finalCta].forEach(button => {
        if (button) {
            button.addEventListener('click', function(e) {
                const ripple = document.createElement('div');
                ripple.style.cssText = `
                    position: absolute;
                    border-radius: 50%;
                    background: rgba(255, 255, 255, 0.5);
                    transform: scale(0);
                    animation: ripple 0.6s linear;
                    pointer-events: none;
                `;
                
                const rect = this.getBoundingClientRect();
                const size = Math.max(rect.width, rect.height);
                ripple.style.width = ripple.style.height = size + 'px';
                ripple.style.left = (e.clientX - rect.left - size / 2) + 'px';
                ripple.style.top = (e.clientY - rect.top - size / 2) + 'px';
                
                this.style.position = 'relative';
                this.style.overflow = 'hidden';
                this.appendChild(ripple);
                
                setTimeout(() => {
                    ripple.remove();
                }, 600);
            });
        }
    });
});

// Add CSS for ripple animation
const style = document.createElement('style');
style.textContent = `
    @keyframes ripple {
        to {
            transform: scale(2);
            opacity: 0;
        }
    }
`;
document.head.appendChild(style);

// Newsletter form handling
document.addEventListener('DOMContentLoaded', function() {
    const newsletterForm = document.querySelector('.newsletter form');
    if (newsletterForm) {
        newsletterForm.addEventListener('submit', function(e) {
            e.preventDefault();
            const email = this.querySelector('input[type="email"]').value;
            
            if (email) {
                alert('¡Gracias por suscribirse! Recibirá noticias sobre Raloy Detailing en: ' + email);
                this.querySelector('input[type="email"]').value = '';
            }
        });
    }
});