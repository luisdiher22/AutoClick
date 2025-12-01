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
});
