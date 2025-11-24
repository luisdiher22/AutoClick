// Sistema de Favoritos para AutoClick
// Este archivo maneja la funcionalidad de agregar/quitar favoritos en los tiles de autos

// URLs de los iconos
const ICON_URLS = {
    empty: 'https://autoclickstorage.blob.core.windows.net/uploads/Component%20183.png',
    filled: '/images/icons/CORAZONLLENO.svg'
};

// Cache de favoritos del usuario (se carga una vez y se actualiza localmente)
let userFavorites = new Set();

// Inicializar el sistema de favoritos
async function initializeFavoritos(emailUsuario = null) {
    try {
        // Cargar favoritos del usuario
        const response = await fetch(`/api/Favoritos/GetUserFavorites${emailUsuario ? `?emailUsuario=${encodeURIComponent(emailUsuario)}` : ''}`);
        const data = await response.json();
        
        if (data.favoritos) {
            userFavorites = new Set(data.favoritos);
        }
        
        // Actualizar todos los botones de favoritos en la página
        updateAllFavoriteButtons();
    } catch (error) {
        console.error('Error al cargar favoritos:', error);
    }
}

// Actualizar todos los botones de favoritos en la página
function updateAllFavoriteButtons() {
    // Seleccionar tanto .favorite-btn como .favorite-btn-inline
    document.querySelectorAll('.favorite-btn, .favorite-btn-inline').forEach(btn => {
        const autoId = parseInt(btn.getAttribute('data-auto-id'));
        const img = btn.querySelector('img');
        
        if (userFavorites.has(autoId)) {
            img.src = ICON_URLS.filled;
            btn.classList.add('is-favorite');
        } else {
            img.src = ICON_URLS.empty;
            btn.classList.remove('is-favorite');
        }
    });
}

// Toggle favorito
async function toggleFavorito(autoId, emailUsuario = null) {
    try {
        const response = await fetch('/api/Favoritos/Toggle', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                AutoId: autoId,
                EmailUsuario: emailUsuario
            })
        });
        
        const data = await response.json();
        
        if (data.success) {
            // Actualizar el cache local
            if (data.isFavorite) {
                userFavorites.add(autoId);
            } else {
                userFavorites.delete(autoId);
            }
            
            // Actualizar todos los botones con este autoId (tanto .favorite-btn como .favorite-btn-inline)
            document.querySelectorAll(`.favorite-btn[data-auto-id="${autoId}"], .favorite-btn-inline[data-auto-id="${autoId}"]`).forEach(btn => {
                const img = btn.querySelector('img');
                if (data.isFavorite) {
                    img.src = ICON_URLS.filled;
                    btn.classList.add('is-favorite');
                } else {
                    img.src = ICON_URLS.empty;
                    btn.classList.remove('is-favorite');
                }
            });
            
            // Mostrar notificación opcional
            showFavoriteNotification(data.isFavorite);
            
            return data;
        } else {
            console.error('Error al toggle favorito:', data.message);
            return null;
        }
    } catch (error) {
        console.error('Error al toggle favorito:', error);
        return null;
    }
}

// Mostrar notificación de favorito (opcional)
function showFavoriteNotification(isFavorite) {
    // Puedes implementar una notificación toast aquí si lo deseas
}

// Event listener para los botones de favoritos (tanto .favorite-btn como .favorite-btn-inline)
document.addEventListener('click', async function(e) {
    const favoriteBtn = e.target.closest('.favorite-btn, .favorite-btn-inline');
    if (favoriteBtn) {
        // Prevenir cualquier comportamiento por defecto y propagación
        e.preventDefault();
        e.stopPropagation();
        e.stopImmediatePropagation();
        
        const autoId = parseInt(favoriteBtn.getAttribute('data-auto-id'));
        const emailUsuario = favoriteBtn.getAttribute('data-email-usuario') || null;
        
        await toggleFavorito(autoId, emailUsuario);
        
        // Retornar false para mayor compatibilidad
        return false;
    }
}, true); // Usar captura en lugar de bubbling

// Inicializar cuando el DOM esté listo
document.addEventListener('DOMContentLoaded', function() {
    // Obtener el email del usuario si está disponible
    const emailUsuario = document.body.getAttribute('data-user-email') || null;
    initializeFavoritos(emailUsuario);
});
