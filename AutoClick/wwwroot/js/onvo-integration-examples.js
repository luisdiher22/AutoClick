// ============================================
// EJEMPLO DE USO DE ONVO PAY DESDE CUALQUIER PÁGINA
// ============================================

// OPCIÓN 1: Redirección Simple
// --------------------------------------------
// Usa esto cuando quieras redirigir a la página de pago dedicada

function pagarAnuncio(anuncioId, monto, moneda = 'USD', descripcion = 'Publicidad AutoClick') {
    const params = new URLSearchParams({
        anuncioId: anuncioId,
        amount: monto,
        currency: moneda,
        description: descripcion
    });
    
    window.location.href = `/Pagos/ProcessPayment?${params.toString()}`;
}

// Ejemplo de uso en un botón:
// <button onclick="pagarAnuncio(123, 5000, 'USD', 'Plan Premium 30 días')">
//     Pagar $50.00
// </button>


// OPCIÓN 2: Integración Embebida
// --------------------------------------------
// Usa esto cuando quieras mostrar el formulario de pago en un modal o sección de tu página

async function iniciarPagoEmbebido(anuncioId, monto, moneda = 'USD', descripcion = 'Publicidad AutoClick') {
    try {
        // 1. Crear el Payment Intent en el backend
        const response = await fetch('/api/pagos/create-payment-intent', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                amount: monto,
                currency: moneda,
                description: descripcion,
                anuncioPublicidadId: anuncioId
            })
        });

        if (!response.ok) {
            const error = await response.json();
            throw new Error(error.error || 'Error al crear sesión de pago');
        }

        const data = await response.json();

        // 2. Inicializar el SDK de ONVO
        const onvoInstance = onvo.pay({
            publicKey: data.publishableKey,
            paymentIntentId: data.paymentIntentId,
            paymentType: 'one_time',
            locale: 'es',
            onSuccess: handlePagoExitoso,
            onError: handlePagoError
        });

        // 3. Renderizar el formulario en un contenedor
        onvoInstance.render('#payment-container');

        // 4. Opcional: Mostrar modal si está oculto
        document.getElementById('payment-modal').style.display = 'block';

    } catch (error) {
        console.error('Error:', error);
        alert('Error al iniciar el pago: ' + error.message);
    }
}

function handlePagoExitoso(data) {
    console.log('Pago procesado:', data);
    
    if (data.status === 'succeeded') {
        alert('¡Pago exitoso! Tu anuncio será activado en breve.');
        window.location.href = '/Publicidad/MisAnuncios';
    } 
    else if (data.status === 'processing') {
        alert('Tu pago está siendo procesado. Te notificaremos cuando se complete.');
        window.location.href = '/Publicidad/MisAnuncios';
    }
    else if (data.status === 'requires_payment_method') {
        alert('El pago fue rechazado. Por favor, verifica tu método de pago e intenta nuevamente.');
    }
}

function handlePagoError(error) {
    console.error('Error en el pago:', error);
    alert('Error procesando el pago: ' + (error.message || 'Intenta nuevamente'));
}


// OPCIÓN 3: Pago con Botón Externo (Manual Submit)
// --------------------------------------------
// Usa esto cuando quieras controlar cuándo se envía el pago

let onvoInstanceGlobal = null;

async function iniciarPagoConBotonExterno(anuncioId, monto, moneda = 'USD', descripcion = 'Publicidad AutoClick') {
    try {
        const response = await fetch('/api/pagos/create-payment-intent', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                amount: monto,
                currency: moneda,
                description: descripcion,
                anuncioPublicidadId: anuncioId
            })
        });

        const data = await response.json();

        // Inicializar con manualSubmit: true
        onvoInstanceGlobal = onvo.pay({
            publicKey: data.publishableKey,
            paymentIntentId: data.paymentIntentId,
            paymentType: 'one_time',
            locale: 'es',
            manualSubmit: true,  // Importante: desactiva el botón interno
            onSuccess: handlePagoExitoso,
            onError: handlePagoError
        });

        onvoInstanceGlobal.render('#payment-container');

        // Mostrar tu botón personalizado
        document.getElementById('mi-boton-pagar').style.display = 'block';

    } catch (error) {
        console.error('Error:', error);
    }
}

function procesarPagoManual() {
    if (onvoInstanceGlobal) {
        // Disparar el pago desde tu botón
        onvoInstanceGlobal.submitPayment();
    }
}


// EJEMPLO HTML COMPLETO
// --------------------------------------------
/*
<!DOCTYPE html>
<html>
<head>
    <title>Pagar Anuncio</title>
    <!-- Incluir SDK de ONVO -->
    <script src="https://sdk.onvopay.com/sdk.js"></script>
</head>
<body>
    <h1>Publicar Anuncio Premium</h1>
    
    <div class="pricing-card">
        <h3>Plan Premium</h3>
        <p class="price">$50.00 USD</p>
        <ul>
            <li>30 días de publicación</li>
            <li>Posición destacada</li>
            <li>Sin límite de vistas</li>
        </ul>
        
        <!-- Botón para iniciar pago -->
        <button onclick="pagarAnuncio(123, 5000, 'USD', 'Plan Premium 30 días')" 
                class="btn btn-primary">
            Pagar Ahora
        </button>
    </div>

    <!-- Modal para pago embebido (opcional) -->
    <div id="payment-modal" style="display: none;">
        <div class="modal-content">
            <h2>Completar Pago</h2>
            <div id="payment-container"></div>
            <button onclick="cerrarModal()">Cancelar</button>
        </div>
    </div>

    <script src="/js/onvo-integration-examples.js"></script>
</body>
</html>
*/


// HELPER: Formatear montos
// --------------------------------------------
function formatearMonto(centavos, moneda = 'USD') {
    const monto = centavos / 100;
    return new Intl.NumberFormat('es-CR', {
        style: 'currency',
        currency: moneda
    }).format(monto);
}

// Ejemplo: formatearMonto(5000, 'USD') => "$50.00"


// HELPER: Consultar estado de un pago
// --------------------------------------------
async function consultarEstadoPago(paymentIntentId) {
    try {
        const response = await fetch(`/api/pagos/status/${paymentIntentId}`);
        
        if (!response.ok) {
            throw new Error('No se pudo consultar el estado del pago');
        }

        const data = await response.json();
        return data;
    } catch (error) {
        console.error('Error consultando estado:', error);
        return null;
    }
}

// Ejemplo de uso:
// const estado = await consultarEstadoPago('cl4de13uc457301lor2o0q9w1');
// console.log('Estado:', estado.status);


// HELPER: Obtener configuración pública
// --------------------------------------------
async function obtenerConfiguracionOnvo() {
    try {
        const response = await fetch('/api/pagos/config');
        const config = await response.json();
        return config.publishableKey;
    } catch (error) {
        console.error('Error obteniendo configuración:', error);
        return null;
    }
}


// EJEMPLOS DE INTEGRACIÓN EN DIFERENTES ESCENARIOS
// ============================================

// ESCENARIO 1: Página de publicar anuncio
// Usuario completa formulario y al final paga
function alPublicarAnuncio() {
    // ... validar formulario ...
    // ... guardar datos del anuncio (sin activar) ...
    const anuncioId = 123; // ID del anuncio recién creado
    const planSeleccionado = obtenerPlanSeleccionado();
    
    pagarAnuncio(anuncioId, planSeleccionado.precio, 'USD', planSeleccionado.nombre);
}

// ESCENARIO 2: Página de mis anuncios - renovar o activar
function renovarAnuncio(anuncioId) {
    const PRECIO_RENOVACION = 5000; // $50.00
    
    if (confirm('¿Deseas renovar este anuncio por 30 días más?')) {
        pagarAnuncio(anuncioId, PRECIO_RENOVACION, 'USD', 'Renovación - 30 días');
    }
}

// ESCENARIO 3: Checkout con múltiples opciones
function mostrarCheckout(anuncioId) {
    const planes = [
        { nombre: 'Básico', dias: 7, precio: 1000 },
        { nombre: 'Estándar', dias: 15, precio: 2500 },
        { nombre: 'Premium', dias: 30, precio: 5000 }
    ];
    
    // Mostrar modal o página con opciones
    // Al seleccionar, llamar:
    const planSeleccionado = planes[2]; // Premium
    pagarAnuncio(anuncioId, planSeleccionado.precio, 'USD', 
                 `Plan ${planSeleccionado.nombre} - ${planSeleccionado.dias} días`);
}

// ESCENARIO 4: Pago en modal sin salir de la página
async function pagarEnModal(anuncioId, monto) {
    // Mostrar el modal
    const modal = document.getElementById('payment-modal');
    modal.style.display = 'block';
    
    // Iniciar pago embebido
    await iniciarPagoEmbebido(anuncioId, monto);
}

function cerrarModal() {
    document.getElementById('payment-modal').style.display = 'none';
}


// NOTAS IMPORTANTES
// ============================================
/*
1. Montos siempre en centavos/céntimos:
   - $1.00 USD = 1000 centavos
   - ₡1,000 CRC = 100000 céntimos

2. SDK debe estar cargado:
   <script src="https://sdk.onvopay.com/sdk.js"></script>

3. Estados de pago:
   - 'succeeded': Pago exitoso
   - 'processing': En proceso (SINPE)
   - 'requires_payment_method': Rechazado
   - 'requires_action': Requiere 3DS
   - 'failed': Fallido
   - 'canceled': Cancelado

4. Manejo de errores:
   - Siempre usar try-catch
   - Mostrar mensajes claros al usuario
   - Loggear errores para debugging

5. Testing:
   - Usar tarjetas de prueba (ver documentación)
   - VISA: 4242424242424242
   - Declinada: 4000000000000002
*/
