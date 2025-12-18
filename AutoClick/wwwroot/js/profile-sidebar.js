// Profile Sidebar Floating Menu JavaScript
// For MiPerfil, HistorialPagos, and ConfiguracionCuenta views

function toggleProfileSidebar() {
    const sidebar = document.querySelector('.profile-sidebar-floating');
    const overlay = document.querySelector('.profile-sidebar-overlay');
    
    if (sidebar && overlay) {
        sidebar.classList.toggle('active');
        overlay.classList.toggle('active');
    }
}

function closeProfileSidebar() {
    const sidebar = document.querySelector('.profile-sidebar-floating');
    const overlay = document.querySelector('.profile-sidebar-overlay');
    
    if (sidebar && overlay) {
        sidebar.classList.remove('active');
        overlay.classList.remove('active');
    }
}

// Close sidebar when clicking overlay
document.addEventListener('DOMContentLoaded', function() {
    const overlay = document.querySelector('.profile-sidebar-overlay');
    if (overlay) {
        overlay.addEventListener('click', closeProfileSidebar);
    }
    
    // Position sidebar indicator based on active nav item
    // Try to find active nav-item in any of the three sidebars
    const sidebarSelectors = [
        '.profile-sidebar',
        '.account-sidebar', 
        '.payments-sidebar'
    ];
    
    let activeNavItem = null;
    let parentSidebar = null;
    
    for (const selector of sidebarSelectors) {
        const sidebar = document.querySelector(selector);
        if (sidebar) {
            activeNavItem = sidebar.querySelector('.nav-item.active');
            if (activeNavItem) {
                parentSidebar = sidebar;
                break;
            }
        }
    }
    
    if (activeNavItem && parentSidebar) {
        // Get the indicator from the SAME sidebar
        const indicator = parentSidebar.querySelector('.sidebar-indicator');
        
        if (indicator) {
            // Get all nav items from the parent sidebar
            const navItems = parentSidebar.querySelectorAll('.nav-item');
            let activeIndex = 0;
            
            navItems.forEach((item, index) => {
                if (item === activeNavItem) {
                    activeIndex = index;
                }
            });
            
            // Calculate top position: 23px initial + (56px height per item * index)
            const topPosition = 23 + (56 * activeIndex);
            indicator.style.top = topPosition + 'px';
            
            console.log('Sidebar indicator positioned:', {
                sidebar: parentSidebar.className,
                activeIndex: activeIndex,
                topPosition: topPosition + 'px',
                indicatorFound: true
            });
        } else {
            console.log('Sidebar indicator NOT found in:', parentSidebar.className);
        }
    } else {
        console.log('Active nav item or parent sidebar not found');
    }
});
