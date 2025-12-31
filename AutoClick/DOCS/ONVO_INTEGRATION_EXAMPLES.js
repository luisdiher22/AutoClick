/* =================================================================
   EJEMPLOS PRÁCTICOS DE INTEGRACIÓN ONVO PAY EN AUTOCLICK
   =================================================================
   
   Este archivo contiene ejemplos específicos de cómo integrar
   ONVO Pay en las diferentes páginas existentes de AutoClick.
*/


// =================================================================
// EJEMPLO 1: Integrar en Página de Crear/Editar Anuncio
// =================================================================

// En: Pages/AnunciarEmpresa.cshtml.cs o similar

// Agregar al final del método OnPostAsync:
/*
public async Task<IActionResult> OnPostAsync()
{
    // ... tu código existente para crear el anuncio ...
    
    // Después de guardar el anuncio:
    var anuncioId = nuevoAnuncio.Id;
    var planSeleccionado = Request.Form["plan"]; // o como lo captures
    
    int monto = 0;
    string descripcion = "";
    
    switch(planSeleccionado)
    {
        case "basico":
            monto = 1000; // $10.00
            descripcion = "Plan Básico - 7 días";
            break;
        case "estandar":
            monto = 2500; // $25.00
            descripcion = "Plan Estándar - 15 días";
            break;
        case "premium":
            monto = 5000; // $50.00
            descripcion = "Plan Premium - 30 días";
            break;
    }
    
    // Redirigir a página de pago
    return RedirectToPage("/Pagos/ProcessPayment", new {
        anuncioId = anuncioId,
        amount = monto,
        currency = "USD",
        description = descripcion
    });
}
*/


// =================================================================
// EJEMPLO 2: Botón de Pago en Lista de Anuncios Inactivos
// =================================================================

// En: Pages/Publicidad/MisAnuncios.cshtml

// HTML para mostrar anuncios que necesitan pago:
/*
@foreach (var anuncio in Model.AnunciosInactivos)
{
    <div class="card mb-3">
        <div class="card-body">
            <h5>@anuncio.Titulo</h5>
            <p class="text-muted">Este anuncio está inactivo. Realiza el pago para activarlo.</p>
            
            <div class="btn-group">
                <!-- Opción 1: Plan Básico -->
                <a href="/Pagos/ProcessPayment?anuncioId=@anuncio.Id&amount=1000&currency=USD&description=Plan Básico - 7 días"
                   class="btn btn-sm btn-outline-primary">
                    7 días - $10
                </a>
                
                <!-- Opción 2: Plan Estándar -->
                <a href="/Pagos/ProcessPayment?anuncioId=@anuncio.Id&amount=2500&currency=USD&description=Plan Estándar - 15 días"
                   class="btn btn-sm btn-outline-primary">
                    15 días - $25
                </a>
                
                <!-- Opción 3: Plan Premium -->
                <a href="/Pagos/ProcessPayment?anuncioId=@anuncio.Id&amount=5000&currency=USD&description=Plan Premium - 30 días"
                   class="btn btn-sm btn-primary">
                    30 días - $50
                </a>
            </div>
        </div>
    </div>
}
*/


// =================================================================
// EJEMPLO 3: Modal de Selección de Plan con JavaScript
// =================================================================

// HTML Modal:
/*
<div class="modal fade" id="modalSeleccionPlan" tabindex="-1">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Selecciona tu Plan</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <input type="hidden" id="anuncioIdSeleccionado" />
                
                <div class="row">
                    <div class="col-md-4">
                        <div class="card plan-card" onclick="seleccionarPlan('basico')">
                            <div class="card-body text-center">
                                <h6>Básico</h6>
                                <h3>$10</h3>
                                <p>7 días</p>
                                <button class="btn btn-outline-primary">Seleccionar</button>
                            </div>
                        </div>
                    </div>
                    
                    <div class="col-md-4">
                        <div class="card plan-card" onclick="seleccionarPlan('estandar')">
                            <div class="card-body text-center">
                                <h6>Estándar</h6>
                                <h3>$25</h3>
                                <p>15 días</p>
                                <button class="btn btn-outline-primary">Seleccionar</button>
                            </div>
                        </div>
                    </div>
                    
                    <div class="col-md-4">
                        <div class="card plan-card active" onclick="seleccionarPlan('premium')">
                            <div class="card-body text-center">
                                <h6>Premium</h6>
                                <h3>$50</h3>
                                <p>30 días</p>
                                <span class="badge bg-success">Recomendado</span>
                                <button class="btn btn-primary">Seleccionar</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
*/

// JavaScript para el modal:
const planes = {
    basico: { dias: 7, precio: 1000, nombre: 'Plan Básico' },
    estandar: { dias: 15, precio: 2500, nombre: 'Plan Estándar' },
    premium: { dias: 30, precio: 5000, nombre: 'Plan Premium' }
};

function mostrarModalPlanes(anuncioId) {
    document.getElementById('anuncioIdSeleccionado').value = anuncioId;
    const modal = new bootstrap.Modal(document.getElementById('modalSeleccionPlan'));
    modal.show();
}

function seleccionarPlan(tipoPlan) {
    const anuncioId = document.getElementById('anuncioIdSeleccionado').value;
    const plan = planes[tipoPlan];
    
    const url = `/Pagos/ProcessPayment?` +
        `anuncioId=${anuncioId}&` +
        `amount=${plan.precio}&` +
        `currency=USD&` +
        `description=${encodeURIComponent(plan.nombre + ' - ' + plan.dias + ' días')}`;
    
    window.location.href = url;
}


// =================================================================
// EJEMPLO 4: Página de Checkout Personalizada (Resumen de Compra)
// =================================================================

// En: Pages/Publicidad/Checkout.cshtml.cs

/*
public class CheckoutModel : PageModel
{
    private readonly IOnvoPayService _onvoPayService;
    
    public CheckoutModel(IOnvoPayService onvoPayService)
    {
        _onvoPayService = onvoPayService;
    }
    
    public AnuncioPublicidad Anuncio { get; set; }
    public string PlanSeleccionado { get; set; }
    public int Monto { get; set; }
    public string PaymentIntentId { get; set; }
    public string PublishableKey { get; set; }
    
    public async Task<IActionResult> OnGetAsync(int anuncioId, string plan)
    {
        // Cargar datos del anuncio
        Anuncio = await _context.AnunciosPublicidad.FindAsync(anuncioId);
        if (Anuncio == null) return NotFound();
        
        // Determinar plan y monto
        PlanSeleccionado = plan;
        Monto = plan switch
        {
            "basico" => 1000,
            "estandar" => 2500,
            "premium" => 5000,
            _ => 5000
        };
        
        // Crear Payment Intent
        var paymentIntent = await _onvoPayService.CreatePaymentIntentAsync(
            amount: Monto,
            currency: "USD",
            description: $"Publicidad {Anuncio.Id} - Plan {PlanSeleccionado}",
            anuncioPublicidadId: anuncioId
        );
        
        if (paymentIntent != null)
        {
            PaymentIntentId = paymentIntent.id;
            PublishableKey = _onvoPayService.GetPublishableKey();
        }
        
        return Page();
    }
}
*/

// Y en Checkout.cshtml:
/*
@page
@model CheckoutModel

<div class="container">
    <div class="row">
        <div class="col-md-8">
            <h3>Completar Pago</h3>
            
            <!-- Contenedor ONVO -->
            <div id="onvo-payment-container"></div>
        </div>
        
        <div class="col-md-4">
            <div class="card">
                <div class="card-header">
                    <h5>Resumen</h5>
                </div>
                <div class="card-body">
                    <p><strong>Anuncio:</strong> #@Model.Anuncio.Id</p>
                    <p><strong>Plan:</strong> @Model.PlanSeleccionado</p>
                    <hr />
                    <p><strong>Total:</strong> $@(Model.Monto / 100m:F2)</p>
                </div>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    <script src="https://sdk.onvopay.com/sdk.js"></script>
    <script>
        const onvoInstance = onvo.pay({
            publicKey: '@Model.PublishableKey',
            paymentIntentId: '@Model.PaymentIntentId',
            paymentType: 'one_time',
            locale: 'es',
            onSuccess: (data) => {
                if (data.status === 'succeeded') {
                    window.location.href = '/Publicidad/PagoExitoso?anuncioId=@Model.Anuncio.Id';
                }
            },
            onError: (error) => {
                alert('Error: ' + error.message);
            }
        });
        
        onvoInstance.render('#onvo-payment-container');
    </script>
}
*/


// =================================================================
// EJEMPLO 5: Validar Estado de Pago antes de Mostrar Anuncio
// =================================================================

// En cualquier página que muestre anuncios:
/*
public async Task OnGetAsync()
{
    var anuncios = await _context.AnunciosPublicidad
        .Include(a => a.PagosOnvo)
        .ToListAsync();
    
    // Filtrar solo anuncios con pago aprobado
    AnunciosActivos = anuncios.Where(a => 
        a.Activo && 
        a.PagosOnvo.Any(p => p.Status == "succeeded")
    ).ToList();
}
*/


// =================================================================
// EJEMPLO 6: Dashboard de Pagos para Administrador
// =================================================================

// En: Pages/Admin/Pagos.cshtml.cs
/*
public class PagosAdminModel : PageModel
{
    private readonly ApplicationDbContext _context;
    
    public List<PagoOnvo> PagosRecientes { get; set; }
    public decimal TotalIngresos { get; set; }
    public int PagosPendientes { get; set; }
    public int PagosExitosos { get; set; }
    
    public async Task OnGetAsync()
    {
        PagosRecientes = await _context.PagosOnvo
            .Include(p => p.AnuncioPublicidad)
            .OrderByDescending(p => p.CreatedAt)
            .Take(50)
            .ToListAsync();
        
        // Estadísticas
        TotalIngresos = PagosRecientes
            .Where(p => p.Status == "succeeded")
            .Sum(p => p.Amount) / 100m;
        
        PagosPendientes = PagosRecientes.Count(p => p.Status == "processing");
        PagosExitosos = PagosRecientes.Count(p => p.Status == "succeeded");
    }
}
*/


// =================================================================
// EJEMPLO 7: Webhook Notification System (Opcional)
// =================================================================

// Crear un servicio de notificaciones:
/*
public class NotificationService
{
    private readonly IEmailService _emailService;
    
    public async Task NotifyPaymentSuccess(PagoOnvo pago)
    {
        if (!string.IsNullOrEmpty(pago.EmailUsuario))
        {
            await _emailService.SendEmailAsync(
                to: pago.EmailUsuario,
                subject: "✅ Pago Recibido - AutoClick",
                body: $@"
                    <h2>¡Tu pago fue procesado exitosamente!</h2>
                    <p>Detalles:</p>
                    <ul>
                        <li>Monto: ${pago.Amount / 100m:F2}</li>
                        <li>Anuncio ID: {pago.AnuncioPublicidadId}</li>
                        <li>Fecha: {pago.CompletedAt:dd/MM/yyyy HH:mm}</li>
                    </ul>
                    <p>Tu anuncio ha sido activado y ya está visible.</p>
                "
            );
        }
    }
    
    public async Task NotifyPaymentFailed(PagoOnvo pago)
    {
        if (!string.IsNullOrEmpty(pago.EmailUsuario))
        {
            await _emailService.SendEmailAsync(
                to: pago.EmailUsuario,
                subject: "❌ Pago No Procesado - AutoClick",
                body: $@"
                    <h2>Tu pago no pudo ser procesado</h2>
                    <p>Hubo un problema al procesar tu pago.</p>
                    <p>Por favor, intenta nuevamente o contacta a soporte.</p>
                "
            );
        }
    }
}
*/

// Llamar desde el webhook:
/*
// En OnvoWebhookController.cs
case "payment-intent.succeeded":
    // ... código existente ...
    await _notificationService.NotifyPaymentSuccess(pago);
    break;

case "payment-intent.failed":
    // ... código existente ...
    await _notificationService.NotifyPaymentFailed(pago);
    break;
*/


// =================================================================
// EJEMPLO 8: Componente Reutilizable de Precios
// =================================================================

// ViewComponent: Views/Shared/Components/PricingPlans/Default.cshtml
/*
<div class="pricing-plans">
    <div class="row">
        @foreach (var plan in Model.Planes)
        {
            <div class="col-md-4">
                <div class="card @(plan.Destacado ? "border-primary" : "")">
                    <div class="card-body text-center">
                        @if (plan.Destacado)
                        {
                            <span class="badge bg-primary mb-2">Recomendado</span>
                        }
                        <h4>@plan.Nombre</h4>
                        <h2 class="my-3">$@plan.Precio</h2>
                        <p class="text-muted">@plan.Dias días de publicación</p>
                        <ul class="list-unstyled">
                            @foreach (var feature in plan.Caracteristicas)
                            {
                                <li><i class="fas fa-check text-success"></i> @feature</li>
                            }
                        </ul>
                        <a href="@Url.Page("/Pagos/ProcessPayment", new { 
                               anuncioId = Model.AnuncioId, 
                               amount = plan.PrecioCentavos,
                               currency = "USD",
                               description = plan.Nombre + " - " + plan.Dias + " días"
                           })"
                           class="btn @(plan.Destacado ? "btn-primary" : "btn-outline-primary") w-100">
                            Seleccionar
                        </a>
                    </div>
                </div>
            </div>
        }
    </div>
</div>
*/

// Usar en cualquier página:
/*
@await Component.InvokeAsync("PricingPlans", new { anuncioId = Model.AnuncioId })
*/


// =================================================================
// NOTAS FINALES
// =================================================================

/*
IMPORTANTE: Montos en centavos
- $1.00 = 1000 centavos
- $10.00 = 10000 centavos
- $25.50 = 25500 centavos

Estados de pago para manejar:
- "requires_confirmation" -> Recién creado
- "processing" -> En proceso (SINPE)
- "succeeded" -> Exitoso ✅
- "failed" -> Fallido ❌
- "requires_payment_method" -> Rechazado, necesita otro método
- "canceled" -> Cancelado

Recuerda:
1. Siempre validar el monto mínimo ($0.50 = 50 centavos)
2. Manejar errores con try-catch
3. Mostrar mensajes claros al usuario
4. Loggear todo para debugging
5. Probar con tarjetas de prueba primero
*/
