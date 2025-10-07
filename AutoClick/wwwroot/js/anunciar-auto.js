// AnunciarMiAuto JavaScript functionality
document.addEventListener('DOMContentLoaded', function() {
    // Form navigation state
    let currentSection = 1;
    const totalSections = 8;
    
    console.log('DOM Content Loaded - Initializing AnunciarMiAuto form');
    
    // Add global click listener for debugging
    document.addEventListener('click', function(e) {
        if (e.target.id === 'btn-next') {
            console.log('Next button clicked via global listener');
        }
    });
    
    // Initialize form
    initializeForm();
    
    function initializeForm() {
        debugFormStructure();
        showSection(1);
        updateSectionIndicator();
        initializeEventListeners();
        initializePaymentTabs();
        initializeFileUploads();
        initializeFAQ();
    }
    
    function debugFormStructure() {
        console.log('=== Form Structure Debug ===');
        
        // Check for all sections
        for (let i = 1; i <= totalSections; i++) {
            const section = document.querySelector(`#seccion${i}`);
            if (section) {
                console.log(`✓ Section ${i} found:`, section);
                console.log(`  - Visible:`, section.offsetWidth > 0 && section.offsetHeight > 0);
                console.log(`  - Display:`, getComputedStyle(section).display);
                console.log(`  - Classes:`, section.className);
            } else {
                console.log(`✗ Section ${i} NOT FOUND`);
            }
        }
        
        // Check form sections container
        const formSections = document.querySelector('#form-sections');
        console.log('Form sections container:', formSections);
        
        // Check for any slider containers
        const slider = document.querySelector('.form-sections-slider');
        const wrapper = document.querySelector('.form-sections-wrapper');
        console.log('Slider container:', slider);
        console.log('Wrapper container:', wrapper);
        
        console.log('=== End Form Structure Debug ===');
    }
    
    function initializeEventListeners() {
        // Navigation buttons
        const nextBtn = document.querySelector('#btn-next');
        const backBtn = document.querySelector('#btn-back');
        
        console.log('NextBtn found:', nextBtn);
        console.log('BackBtn found:', backBtn);
        console.log('NextBtn innerHTML:', nextBtn ? nextBtn.innerHTML : 'not found');
        console.log('NextBtn disabled:', nextBtn ? nextBtn.disabled : 'not found');
        
        if (nextBtn) {
            // Remove any existing event listeners first
            nextBtn.removeEventListener('click', handleNextClick);
            
            // Add only ONE event listener
            nextBtn.addEventListener('click', handleNextClick);
            
            console.log('Next button event listener added');
        } else {
            console.error('Next button not found!');
        }
        
        if (backBtn) {
            backBtn.removeEventListener('click', handleBackClick);
            backBtn.addEventListener('click', handleBackClick);
            console.log('Back button event listener added');
        } else {
            console.log('Back button not found (this is normal for section 1)');
        }
        
        // Form validation on input change
        const formInputs = document.querySelectorAll('input, select, textarea');
        formInputs.forEach(input => {
            input.addEventListener('change', validateCurrentSection);
            input.addEventListener('input', validateCurrentSection);
            
            // Add character counter for description field
            if (input.name === 'Formulario.Descripcion') {
                input.addEventListener('input', function() {
                    updateCharacterCounter(this);
                });
                updateCharacterCounter(input); // Initialize counter
            }
        });
        
        // Equipment checkboxes
        const equipmentCheckboxes = document.querySelectorAll('.equipment-item input[type="checkbox"]');
        equipmentCheckboxes.forEach(checkbox => {
            checkbox.addEventListener('change', updateEquipmentSelection);
        });
        
        // Visibility plan selection
        const planRadios = document.querySelectorAll('input[name="plan-visibilidad"]');
        planRadios.forEach(radio => {
            radio.addEventListener('change', updatePlanSelection);
        });
        
        // Tag checkboxes
        const tagCheckboxes = document.querySelectorAll('.tag-option input[type="checkbox"]');
        tagCheckboxes.forEach(checkbox => {
            checkbox.addEventListener('change', updateTagSelection);
        });
        
        // Marca/Modelo cascade
        const marcaSelect = document.querySelector('#marca');
        if (marcaSelect) {
            marcaSelect.addEventListener('change', updateModelos);
        }
        
        // Province/canton cascade  
        const provinciaSelect = document.querySelector('#provincia');
        if (provinciaSelect) {
            provinciaSelect.addEventListener('change', function() {
                updateCantones(this);
            });
        }

        // Input formatting
        const precioInput = document.querySelector('#precio');
        if (precioInput) {
            precioInput.addEventListener('input', () => formatPrice(precioInput));
        }

        const kilometrajeInput = document.querySelector('#kilometraje');
        if (kilometrajeInput) {
            kilometrajeInput.addEventListener('input', () => formatKilometer(kilometrajeInput));
        }

        // File upload handlers
        const fileInputs = document.querySelectorAll('input[type="file"]');
        fileInputs.forEach(input => {
            input.addEventListener('change', handleFileUpload);
        });

        // Payment method toggle
        const paymentMethods = document.querySelectorAll('input[name="payment-method"]');
        paymentMethods.forEach(radio => {
            radio.addEventListener('change', togglePaymentMethod);
        });
    }
    
    function handleNextClick(e) {
        console.log('handleNextClick called');
        e.preventDefault();
        
        const isValid = validateCurrentSection();
        console.log('Current section valid:', isValid, 'Current section:', currentSection);
        
        if (isValid) {
            if (currentSection < totalSections) {
                const previousSection = currentSection;
                currentSection++;
                console.log(`Moving from section ${previousSection} to section ${currentSection}`);
                
                showSection(currentSection);
                updateSectionIndicator();
                updateNavigationButtons();
            } else {
                // Final submission
                console.log('Final submission');
                submitForm();
            }
        } else {
            console.log('Validation failed for current section');
        }
    }
    
    function handleBackClick(e) {
        console.log('handleBackClick called');
        e.preventDefault();
        
        if (currentSection > 1) {
            currentSection--;
            console.log('Moving back to section:', currentSection);
            showSection(currentSection);
            updateSectionIndicator();
            updateNavigationButtons();
        }
    }
    

    
    function showSection(sectionNumber) {
        console.log(`showSection called with section ${sectionNumber}`);
        
        // Hide all sections first
        const sections = document.querySelectorAll('.form-section');
        sections.forEach(section => {
            section.classList.remove('active');
        });
        
        // Show the target section
        const currentSectionElement = document.querySelector(`#seccion${sectionNumber}`);
        if (currentSectionElement) {
            currentSectionElement.classList.add('active');
            console.log(`Activated section: seccion${sectionNumber}`);
        } else {
            console.error(`Section seccion${sectionNumber} not found!`);
        }
        
        updateNavigationButtons();
        updateSectionIndicator();
        
        console.log(`Section ${sectionNumber} is now active`);
    }
    
    function updateSectionIndicator() {
        const indicator = document.querySelector('#current-section');
        if (indicator) {
            indicator.textContent = currentSection;
        }
        
        // Update section indicator in header
        const headerIndicator = document.querySelector('.section-indicator span');
        if (headerIndicator) {
            headerIndicator.textContent = currentSection;
        }
    }
    
    function updateNavigationButtons() {
        const nextBtn = document.querySelector('#btn-next');
        const backBtn = document.querySelector('#btn-back');
        
        if (backBtn) {
            backBtn.style.display = currentSection === 1 ? 'none' : 'flex';
        }
        
        if (nextBtn) {
            if (currentSection === totalSections) {
                nextBtn.textContent = 'Publicar anuncio';
            } else {
                nextBtn.textContent = 'Siguiente sección';
            }
        }
    }

    // Price and kilometer formatting
    window.formatPrice = function(input) {
        let value = input.value.replace(/[^\d]/g, '');
        if (value) {
            value = parseInt(value).toLocaleString('es-CR');
            input.value = value;
        }
    };

    window.formatKilometer = function(input) {
        let value = input.value.replace(/[^\d]/g, '');
        if (value) {
            value = parseInt(value).toLocaleString('es-CR');
            input.value = value;
        }
    };

    function validateCurrentSection() {
        console.log(`Validating section ${currentSection}`);
        const currentSectionElement = document.querySelector(`#seccion${currentSection}`);
        if (!currentSectionElement) {
            console.log(`Section element not found for section ${currentSection}`);
            return true;
        }
        
        const requiredFields = currentSectionElement.querySelectorAll('input[required], select[required], textarea[required]');
        console.log(`Found ${requiredFields.length} required fields in section ${currentSection}`);
        
        let isValid = true;
        
        requiredFields.forEach(field => {
            const fieldName = field.name || field.id;
            const fieldValue = field.value ? field.value.trim() : '';
            console.log(`Checking field ${fieldName}: "${fieldValue}"`);
            
            if (!fieldValue) {
                console.log(`Field ${fieldName} is empty`);
                showFieldError(field, 'Este campo es requerido');
                isValid = false;
            } else {
                clearFieldError(field);
            }
        });
        
        // Section-specific validations
        switch (currentSection) {
            case 1: // Datos del vehículo
                console.log('Running vehicle data validation...');
                const vehicleValid = validateVehicleData();
                isValid = vehicleValid && isValid;
                console.log('Vehicle validation result:', vehicleValid);
                break;
            case 2: // Equipamiento (opcional)
                console.log('Running equipment validation (optional)...');
                // Equipment section is optional but must be shown to user
                // User must click "Next" to continue, regardless of selections
                isValid = true;
                console.log('Equipment validation result: true (optional)');
                break;
            case 3: // Ubicación
                console.log('Running location validation...');
                const locationValid = validateLocationData();
                isValid = locationValid && isValid;
                console.log('Location validation result:', locationValid);
                break;
            case 6: // Pago
                console.log('Running payment validation...');
                const paymentValid = validatePaymentData();
                isValid = paymentValid && isValid;
                console.log('Payment validation result:', paymentValid);
                break;
            default:
                // For sections without specific validation, check only required fields
                console.log(`Section ${currentSection} using default validation`);
                break;
        }
        
        console.log(`Final validation result for section ${currentSection}:`, isValid);
        return isValid;
    }
    

    
    function validateVehicleData() {
        const año = document.querySelector('select[name="Formulario.Año"]');
        const kilometraje = document.querySelector('input[name="Formulario.Kilometraje"]');
        const descripcion = document.querySelector('textarea[name="Formulario.Descripcion"]');
        const precio = document.querySelector('input[name="Formulario.Precio"]');
        const marca = document.querySelector('select[name="Formulario.Marca"]');
        const modelo = document.querySelector('select[name="Formulario.Modelo"]');
        const carroceria = document.querySelector('select[name="Formulario.Carroceria"]');
        const combustible = document.querySelector('select[name="Formulario.Combustible"]');
        const cilindrada = document.querySelector('input[name="Formulario.Cilindrada"]');
        const colorExterior = document.querySelector('input[name="Formulario.ColorExterior"]');
        const colorInterior = document.querySelector('input[name="Formulario.ColorInterior"]');
        const puertas = document.querySelector('select[name="Formulario.Puertas"]');
        const pasajeros = document.querySelector('select[name="Formulario.Pasajeros"]');
        const transmision = document.querySelector('select[name="Formulario.Transmision"]');
        const traccion = document.querySelector('select[name="Formulario.Traccion"]');
        const condicion = document.querySelector('select[name="Formulario.Condicion"]');
        
        let isValid = true;
        
        console.log('Validating vehicle data - found fields:', {
            marca: !!marca,
            modelo: !!modelo,
            año: !!año,
            carroceria: !!carroceria,
            combustible: !!combustible,
            cilindrada: !!cilindrada,
            colorExterior: !!colorExterior,
            colorInterior: !!colorInterior,
            puertas: !!puertas,
            pasajeros: !!pasajeros,
            transmision: !!transmision,
            traccion: !!traccion,
            kilometraje: !!kilometraje,
            condicion: !!condicion,
            precio: !!precio,
            descripcion: !!descripcion
        });
        
        // Required field validations
        if (!marca || !marca.value) {
            if (marca) showFieldError(marca, 'La marca es requerida');
            isValid = false;
        } else {
            if (marca) clearFieldError(marca);
        }
        
        if (!modelo || !modelo.value) {
            if (modelo) showFieldError(modelo, 'El modelo es requerido');
            isValid = false;
        } else {
            if (modelo) clearFieldError(modelo);
        }
        
        // Year validation
        if (!año || !año.value) {
            if (año) showFieldError(año, 'El año es requerido');
            isValid = false;
        } else {
            const yearValue = parseInt(año.value);
            const currentYear = new Date().getFullYear();
            if (yearValue < 1900 || yearValue > currentYear + 1) {
                showFieldError(año, 'Ingrese un año válido');
                isValid = false;
            } else {
                clearFieldError(año);
            }
        }
        
        if (!carroceria || !carroceria.value) {
            if (carroceria) showFieldError(carroceria, 'El tipo de carrocería es requerido');
            isValid = false;
        } else {
            if (carroceria) clearFieldError(carroceria);
        }
        
        if (!combustible || !combustible.value) {
            if (combustible) showFieldError(combustible, 'El tipo de combustible es requerido');
            isValid = false;
        } else {
            if (combustible) clearFieldError(combustible);
        }
        
        // Mileage validation
        if (!kilometraje || !kilometraje.value) {
            if (kilometraje) showFieldError(kilometraje, 'El kilometraje es requerido');
            isValid = false;
        } else {
            const kmValue = parseInt(kilometraje.value.replace(/\D/g, ''));
            if (kmValue < 0 || kmValue > 999999) {
                showFieldError(kilometraje, 'Ingrese un kilometraje válido');
                isValid = false;
            } else {
                clearFieldError(kilometraje);
            }
        }
        
        if (!cilindrada || !cilindrada.value) {
            if (cilindrada) showFieldError(cilindrada, 'La cilindrada es requerida');
            isValid = false;
        } else {
            if (cilindrada) clearFieldError(cilindrada);
        }
        
        if (!colorExterior || !colorExterior.value.trim()) {
            if (colorExterior) showFieldError(colorExterior, 'El color exterior es requerido');
            isValid = false;
        } else {
            if (colorExterior) clearFieldError(colorExterior);
        }
        
        if (!colorInterior || !colorInterior.value.trim()) {
            if (colorInterior) showFieldError(colorInterior, 'El color interior es requerido');
            isValid = false;
        } else {
            if (colorInterior) clearFieldError(colorInterior);
        }
        
        if (!puertas || !puertas.value) {
            if (puertas) showFieldError(puertas, 'El número de puertas es requerido');
            isValid = false;
        } else {
            if (puertas) clearFieldError(puertas);
        }
        
        if (!pasajeros || !pasajeros.value) {
            if (pasajeros) showFieldError(pasajeros, 'El número de pasajeros es requerido');
            isValid = false;
        } else {
            if (pasajeros) clearFieldError(pasajeros);
        }
        
        if (!transmision || !transmision.value) {
            if (transmision) showFieldError(transmision, 'El tipo de transmisión es requerido');
            isValid = false;
        } else {
            if (transmision) clearFieldError(transmision);
        }
        
        if (!traccion || !traccion.value) {
            if (traccion) showFieldError(traccion, 'El tipo de tracción es requerido');
            isValid = false;
        } else {
            if (traccion) clearFieldError(traccion);
        }
        
        if (!condicion || !condicion.value) {
            if (condicion) showFieldError(condicion, 'La condición del vehículo es requerida');
            isValid = false;
        } else {
            if (condicion) clearFieldError(condicion);
        }
        
        // Price validation
        if (!precio || !precio.value) {
            if (precio) showFieldError(precio, 'El precio es requerido');
            isValid = false;
        } else {
            const precioValue = parseFloat(precio.value.replace(/[,\s]/g, ''));
            if (precioValue <= 0 || isNaN(precioValue)) {
                showFieldError(precio, 'Ingrese un precio válido');
                isValid = false;
            } else {
                clearFieldError(precio);
            }
        }
        
        // Description validation
        if (!descripcion || !descripcion.value.trim()) {
            if (descripcion) showFieldError(descripcion, 'La descripción del vehículo es requerida');
            isValid = false;
        } else {
            if (descripcion.value.trim().length < 10) {
                showFieldError(descripcion, 'La descripción debe tener al menos 10 caracteres');
                isValid = false;
            } else {
                clearFieldError(descripcion);
            }
        }
        
        console.log('Vehicle data validation result:', isValid);
        return isValid;
    }
    
    function validateLocationData() {
        const provincia = document.querySelector('select[name="Formulario.Provincia"]');
        let isValid = true;
        
        console.log('Validating location data - provincia found:', !!provincia);
        
        if (!provincia || !provincia.value) {
            if (provincia) showFieldError(provincia, 'Seleccione una provincia');
            isValid = false;
        } else {
            if (provincia) clearFieldError(provincia);
        }
        
        console.log('Location validation result:', isValid);
        return isValid;
    }
    
    function validatePaymentData() {
        console.log('Validating payment data...');
        
        // Por ahora, permitir continuar sin validación estricta de pago
        // ya que no todos los campos de pago están implementados aún
        let isValid = true;
        
        console.log('Payment validation result:', isValid);
        return isValid;
    }
    
    function showFieldError(field, message) {
        clearFieldError(field);
        
        const errorDiv = document.createElement('div');
        errorDiv.className = 'field-error';
        errorDiv.textContent = message;
        errorDiv.style.color = '#FF0000';
        errorDiv.style.fontSize = '12px';
        errorDiv.style.marginTop = '4px';
        
        field.parentNode.appendChild(errorDiv);
        field.style.borderColor = '#FF0000';
    }
    
    function clearFieldError(field) {
        const existingError = field.parentNode.querySelector('.field-error');
        if (existingError) {
            existingError.remove();
        }
        field.style.borderColor = 'rgba(255, 255, 255, 0.5)';
    }
    
    function showGlobalError(message) {
        clearGlobalError();
        
        const errorDiv = document.createElement('div');
        errorDiv.className = 'global-error';
        errorDiv.textContent = message;
        errorDiv.style.cssText = `
            color: #FF0000;
            background: rgba(255, 0, 0, 0.1);
            border: 1px solid #FF0000;
            padding: 12px;
            border-radius: 4px;
            margin-bottom: 20px;
            text-align: center;
        `;
        
        const currentSectionElement = document.querySelector(`#seccion${currentSection}`);
        if (currentSectionElement) {
            currentSectionElement.insertBefore(errorDiv, currentSectionElement.firstChild);
        }
    }
    
    function clearGlobalError() {
        const existingError = document.querySelector('.global-error');
        if (existingError) {
            existingError.remove();
        }
    }
    
    function updateCharacterCounter(textarea) {
        const currentLength = textarea.value.length;
        const minLength = 10;
        
        // Remove existing counter
        const existingCounter = textarea.parentNode.querySelector('.character-counter');
        if (existingCounter) {
            existingCounter.remove();
        }
        
        // Create new counter
        const counter = document.createElement('div');
        counter.className = 'character-counter';
        counter.style.cssText = `
            font-size: 12px;
            margin-top: 4px;
            color: ${currentLength >= minLength ? '#4CAF50' : '#FF9800'};
        `;
        counter.textContent = `${currentLength}/${minLength} caracteres mínimos`;
        
        textarea.parentNode.appendChild(counter);
    }
    
    function updateEquipmentSelection() {
        // Update equipment selection logic if needed
        console.log('Equipment selection updated');
    }
    
    function updatePlanSelection() {
        const selectedPlan = document.querySelector('input[name="PlanVisibilidad"]:checked');
        if (selectedPlan) {
            updatePaymentSummary();
        }
    }
    
    function updateTagSelection() {
        updatePaymentSummary();
    }
    
    function updatePaymentSummary() {
        const selectedPlan = document.querySelector('input[name="PlanVisibilidad"]:checked');
        const selectedTags = document.querySelectorAll('.tag-option input[type="checkbox"]:checked');
        
        let planPrice = 0;
        let tagsPrice = 0;
        
        if (selectedPlan) {
            planPrice = parseFloat(selectedPlan.dataset.price || 0);
        }
        
        selectedTags.forEach(tag => {
            tagsPrice += parseFloat(tag.dataset.price || 0);
        });
        
        const subtotal = planPrice + tagsPrice;
        const tax = subtotal * 0.13; // 13% IVA
        const total = subtotal + tax;
        
        // Update summary display
        updateSummaryLine('Plan seleccionado', `₡${planPrice.toLocaleString()}`);
        updateSummaryLine('Tags adicionales', `₡${tagsPrice.toLocaleString()}`);
        updateSummaryLine('Subtotal', `₡${subtotal.toLocaleString()}`);
        updateSummaryLine('IVA (13%)', `₡${tax.toLocaleString()}`);
        updateSummaryTotal(`₡${total.toLocaleString()}`);
    }
    
    function updateSummaryLine(label, value) {
        const summaryLines = document.querySelectorAll('.summary-line');
        summaryLines.forEach(line => {
            const labelSpan = line.querySelector('span:first-child');
            if (labelSpan && labelSpan.textContent === label) {
                const valueSpan = line.querySelector('span:last-child');
                if (valueSpan) {
                    valueSpan.textContent = value;
                }
            }
        });
    }
    
    function updateSummaryTotal(total) {
        const totalElement = document.querySelector('.total-amount');
        if (totalElement) {
            totalElement.textContent = total;
        }
    }
    
    function initializePaymentTabs() {
        const paymentTabs = document.querySelectorAll('.payment-tab');
        const paymentContents = document.querySelectorAll('.payment-tab-content');
        
        paymentTabs.forEach(tab => {
            tab.addEventListener('click', function() {
                const targetTab = this.dataset.tab;
                
                // Remove active class from all tabs and contents
                paymentTabs.forEach(t => t.classList.remove('active'));
                paymentContents.forEach(c => c.classList.remove('active'));
                
                // Add active class to clicked tab and corresponding content
                this.classList.add('active');
                const targetContent = document.querySelector(`#${targetTab}`);
                if (targetContent) {
                    targetContent.classList.add('active');
                }
            });
        });
    }
    
    function initializeFileUploads() {
        const uploadAreas = document.querySelectorAll('.upload-area');
        console.log('Initializing file uploads - found upload areas:', uploadAreas.length);
        
        uploadAreas.forEach((area, index) => {
            console.log(`Upload area ${index}:`, area, 'Classes:', area.className);
            
            area.addEventListener('click', function() {
                console.log('Upload area clicked:', this);
                const fileInput = document.createElement('input');
                fileInput.type = 'file';
                fileInput.accept = this.classList.contains('video') ? 'video/*' : 'image/*';
                fileInput.multiple = !this.classList.contains('video');
                
                fileInput.addEventListener('change', function(e) {
                    console.log('File selected:', e.target.files);
                    handleFileUpload(e.target.files, area);
                });
                
                fileInput.click();
            });
            
            // Drag and drop functionality
            area.addEventListener('dragover', function(e) {
                e.preventDefault();
                this.style.borderColor = 'white';
                this.style.background = 'rgba(255, 255, 255, 0.05)';
            });
            
            area.addEventListener('dragleave', function(e) {
                e.preventDefault();
                this.style.borderColor = 'rgba(255, 255, 255, 0.5)';
                this.style.background = '#02081C';
            });
            
            area.addEventListener('drop', function(e) {
                e.preventDefault();
                this.style.borderColor = 'rgba(255, 255, 255, 0.5)';
                this.style.background = '#02081C';
                
                handleFileUpload(e.dataTransfer.files, this);
            });
        });
    }
    
    function handleFileUpload(files, uploadArea) {
        console.log('handleFileUpload called with:', files.length, 'files');
        console.log('Upload area:', uploadArea);
        
        const fileArray = Array.from(files);
        const isVideo = uploadArea.classList.contains('video');
        console.log('Is video area:', isVideo);
        
        // Para videos: solo permitir 1 archivo
        if (isVideo && fileArray.length > 1) {
            alert('Solo se permite subir 1 video');
            return;
        }
        
        // Para imágenes: permitir hasta 10 archivos
        if (!isVideo) {
            const currentImages = uploadArea.querySelectorAll('.image-preview').length;
            if (currentImages + fileArray.length > 10) {
                alert('Solo se permiten máximo 10 imágenes');
                return;
            }
        }
        
        fileArray.forEach(file => {
            console.log('Processing file:', file.name, 'Type:', file.type);
            if (isVideo && !file.type.startsWith('video/')) {
                alert('Por favor, seleccione solo archivos de video');
                return;
            }
            
            if (!isVideo && !file.type.startsWith('image/')) {
                alert('Por favor, seleccione solo archivos de imagen');
                return;
            }
            
            // Create preview
            const reader = new FileReader();
            reader.onload = function(e) {
                if (isVideo) {
                    updateVideoPreview(uploadArea, e.target.result, file.name);
                } else {
                    addImagePreview(uploadArea, e.target.result, file.name, file);
                }
            };
            reader.readAsDataURL(file);
        });
    }
    
    function updateVideoPreview(uploadArea, src, fileName) {
        const placeholder = uploadArea.querySelector('.upload-placeholder');
        placeholder.innerHTML = `
            <video style="width: 100%; height: 100%; object-fit: contain; background: rgba(0,0,0,0.1);" controls>
                <source src="${src}" type="video/mp4">
            </video>
            <p style="position: absolute; bottom: 10px; left: 50%; transform: translateX(-50%); margin: 0; font-size: 12px; color: white; background: rgba(0,0,0,0.7); padding: 2px 8px; border-radius: 4px;">${fileName}</p>
        `;
    }
    
    // Global array to store uploaded files
    let uploadedFiles = [];

    function addImagePreview(uploadArea, src, fileName, file) {
        // Store file reference
        uploadedFiles.push(file);
        console.log(`File added to uploadedFiles: ${fileName}, Total files: ${uploadedFiles.length}`);
        
        // Buscar o crear el contenedor de previsualizaciones
        let previewContainer = uploadArea.querySelector('.images-preview-container');
        if (!previewContainer) {
            // Crear toggle de visualización
            const viewToggle = document.createElement('div');
            viewToggle.className = 'image-view-toggle';
            viewToggle.innerHTML = `
                <label>Vista:</label>
                <button type="button" class="view-mode-btn active" data-mode="contain">Completa</button>
                <button type="button" class="view-mode-btn" data-mode="cover">Ajustada</button>
            `;
            uploadArea.appendChild(viewToggle);
            
            // Añadir event listeners para los botones de vista
            viewToggle.querySelectorAll('.view-mode-btn').forEach(btn => {
                btn.addEventListener('click', function() {
                    viewToggle.querySelectorAll('.view-mode-btn').forEach(b => b.classList.remove('active'));
                    this.classList.add('active');
                    updateImageViewMode(previewContainer, this.dataset.mode);
                });
            });
            
            previewContainer = document.createElement('div');
            previewContainer.className = 'images-preview-container';
            previewContainer.style.cssText = `
                display: flex;
                flex-wrap: wrap;
                gap: 15px;
                margin-top: 15px;
            `;
            uploadArea.appendChild(previewContainer);
            
            // Ocultar el placeholder
            const placeholder = uploadArea.querySelector('.upload-placeholder');
            if (placeholder) {
                placeholder.style.display = 'none';
            }
        }
        
        // Crear elemento de preview individual
        const imagePreview = document.createElement('div');
        imagePreview.className = 'image-preview';
        imagePreview.draggable = true;
        imagePreview.style.cssText = `
            position: relative;
            width: 100px;
            height: 100px;
            border-radius: 8px;
            overflow: hidden;
            cursor: move;
            border: 2px solid rgba(255,255,255,0.3);
        `;
        
        imagePreview.innerHTML = `
            <img src="${src}" alt="${fileName}" class="preview-image" style="width: 100%; height: 100%; object-fit: contain; background: rgba(0,0,0,0.1); transition: all 0.3s;">
            <button class="remove-image" style="
                position: absolute;
                top: 5px;
                right: 5px;
                width: 20px;
                height: 20px;
                border-radius: 50%;
                background: rgba(255,0,0,0.8);
                color: white;
                border: none;
                cursor: pointer;
                font-size: 12px;
                display: flex;
                align-items: center;
                justify-content: center;
                transition: all 0.3s;
            ">×</button>
        `;
        
        // Agregar event listeners para drag & drop y remove
        setupImagePreviewEvents(imagePreview, previewContainer, file);
        
        previewContainer.appendChild(imagePreview);
        
        // Actualizar el botón de orden de fotos
        updatePhotoOrderSection();
    }
    
    function setupImagePreviewEvents(imagePreview, container, file) {
        // Remove button
        const removeBtn = imagePreview.querySelector('.remove-image');
        removeBtn.addEventListener('click', function(e) {
            e.stopPropagation();
            
            // Remove file from uploadedFiles array
            const fileIndex = uploadedFiles.indexOf(file);
            if (fileIndex > -1) {
                uploadedFiles.splice(fileIndex, 1);
                console.log(`File removed from uploadedFiles: ${file.name}, Remaining files: ${uploadedFiles.length}`);
            }
            
            imagePreview.remove();
            
            // Si no quedan imágenes, mostrar el placeholder de nuevo
            if (container.children.length === 0) {
                const placeholder = container.parentElement.querySelector('.upload-placeholder');
                if (placeholder) {
                    placeholder.style.display = 'flex';
                }
                container.remove();
            }
            
            updatePhotoOrderSection();
        });
        
        // Drag and drop para reordenar
        imagePreview.addEventListener('dragstart', function(e) {
            e.dataTransfer.setData('text/plain', '');
            imagePreview.classList.add('dragging');
        });
        
        imagePreview.addEventListener('dragend', function() {
            imagePreview.classList.remove('dragging');
        });
        
        container.addEventListener('dragover', function(e) {
            e.preventDefault();
            const afterElement = getDragAfterElement(container, e.clientX);
            const dragging = document.querySelector('.dragging');
            if (afterElement == null) {
                container.appendChild(dragging);
            } else {
                container.insertBefore(dragging, afterElement);
            }
        });
    }
    
    function getDragAfterElement(container, x) {
        const draggableElements = [...container.querySelectorAll('.image-preview:not(.dragging)')];
        
        return draggableElements.reduce((closest, child) => {
            const box = child.getBoundingClientRect();
            const offset = x - box.left - box.width / 2;
            
            if (offset < 0 && offset > closest.offset) {
                return { offset: offset, element: child };
            } else {
                return closest;
            }
        }, { offset: Number.NEGATIVE_INFINITY }).element;
    }
    
    function updatePhotoOrderSection() {
        const orderSection = document.querySelector('.photo-order-section');
        const imagesPreviews = document.querySelectorAll('.image-preview');
        
        if (orderSection && imagesPreviews.length > 0) {
            orderSection.style.display = 'block';
            // Aquí podrías agregar lógica adicional para actualizar la sección de orden
        } else if (orderSection) {
            orderSection.style.display = 'none';
        }
    }
    
    function initializeFAQ() {
        const faqQuestions = document.querySelectorAll('.faq-question');
        
        faqQuestions.forEach(question => {
            question.addEventListener('click', function() {
                const faqItem = this.parentElement;
                const answer = faqItem.querySelector('.faq-answer');
                const arrow = this.querySelector('.faq-arrow');
                
                if (answer.style.display === 'block') {
                    answer.style.display = 'none';
                    arrow.style.transform = 'rotate(0deg)';
                } else {
                    // Close other open answers
                    document.querySelectorAll('.faq-answer').forEach(a => a.style.display = 'none');
                    document.querySelectorAll('.faq-arrow').forEach(a => a.style.transform = 'rotate(0deg)');
                    
                    answer.style.display = 'block';
                    arrow.style.transform = 'rotate(180deg)';
                }
            });
        });
    }
    
    function submitForm() {
        console.log('=== SUBMIT FORM CALLED ===');
        
        // Show loading state
        const nextBtn = document.querySelector('.btn-next');
        if (nextBtn) {
            nextBtn.textContent = 'Procesando...';
            nextBtn.disabled = true;
        }
        
        // Prepare equipment data before submission
        prepareEquipmentData();
        
        // Prepare file data before submission
        prepareFileData();
        
        // Add action input to form
        const form = document.querySelector('#anuncioForm');
        if (form) {
            // Remove any existing action input
            const existingAction = form.querySelector('input[name="accion"]');
            if (existingAction) {
                existingAction.remove();
            }
            
            // Add new action input
            const actionInput = document.createElement('input');
            actionInput.type = 'hidden';
            actionInput.name = 'accion';
            actionInput.value = 'finalizar';
            form.appendChild(actionInput);
            
            console.log('=== SUBMITTING FORM ===');
            form.submit();
        }
    }
    
    function prepareEquipmentData() {
        console.log('=== PREPARING EQUIPMENT DATA ===');
        
        const form = document.querySelector('#anuncioForm');
        if (!form) {
            console.error('Form not found!');
            return;
        }
        
        // Remove any existing equipment inputs to avoid duplicates
        const existingEquipmentInputs = form.querySelectorAll('input[name*="ExtrasExterior"], input[name*="ExtrasInterior"], input[name*="ExtrasMultimedia"], input[name*="ExtrasSeguridad"], input[name*="ExtrasRendimiento"], input[name*="ExtrasAntiRobo"]');
        existingEquipmentInputs.forEach(input => input.remove());
        
        // Find section 2 container
        const section2 = document.querySelector('#seccion2');
        if (!section2) {
            console.error('Section 2 not found!');
            return;
        }
        
        console.log('Section 2 found, analyzing structure...');
        
        // Simple approach: find all checkboxes in section 2 and organize by closest H2
        const allCheckboxes = section2.querySelectorAll('input[type="checkbox"]:checked');
        console.log(`Total checked checkboxes in section 2: ${allCheckboxes.length}`);
        
        // If no checkboxes are checked, let's see all checkboxes
        if (allCheckboxes.length === 0) {
            const allCheckboxesUnchecked = section2.querySelectorAll('input[type="checkbox"]');
            console.log(`Total checkboxes (including unchecked) in section 2: ${allCheckboxesUnchecked.length}`);
            
            if (allCheckboxesUnchecked.length > 0) {
                console.log('Sample checkbox:', allCheckboxesUnchecked[0]);
                console.log('Sample checkbox container:', allCheckboxesUnchecked[0].closest('div'));
            }
        }
        
        // Collect equipment by sections based on H2 headers
        const equipmentSections = {
            'Equipamiento del vehículo': 'Formulario.ExtrasExterior',
            'Equipamiento Interior': 'Formulario.ExtrasInterior', 
            'Multimedia': 'Formulario.ExtrasMultimedia',
            'Seguridad al volante': 'Formulario.ExtrasSeguridad',
            'Desempeño': 'Formulario.ExtrasRendimiento',
            'Antirrobo': 'Formulario.ExtrasAntiRobo'
        };
        
        // Process each equipment category
        Object.entries(equipmentSections).forEach(([sectionTitle, fieldName]) => {
            console.log(`Processing section: ${sectionTitle} -> ${fieldName}`);
            
            // Find the H2 header with this title in section 2
            const headers = section2.querySelectorAll('h2');
            let targetHeader = null;
            
            headers.forEach(header => {
                if (header.textContent.trim() === sectionTitle) {
                    targetHeader = header;
                }
            });
            
            if (targetHeader) {
                console.log(`Found header for ${sectionTitle}:`, targetHeader);
                
                // Look for checkboxes only in the next sibling container until the next H2
                let checkboxes = [];
                let nextElement = targetHeader.nextElementSibling;
                
                console.log(`Looking for checkboxes after header: ${sectionTitle}`);
                
                while (nextElement && nextElement.tagName !== 'H2') {
                    console.log(`Checking element:`, nextElement.tagName, nextElement.className);
                    
                    if (nextElement.classList && nextElement.classList.contains('equipment-grid')) {
                        const gridCheckboxes = nextElement.querySelectorAll('input[type="checkbox"]:checked');
                        checkboxes.push(...gridCheckboxes);
                        console.log(`Found equipment-grid with ${gridCheckboxes.length} checked items`);
                        break; // Stop after finding the first equipment-grid for this header
                    }
                    
                    nextElement = nextElement.nextElementSibling;
                }
                
                console.log(`Final checkboxes for ${sectionTitle}: ${checkboxes.length}`)
                
                console.log(`Found ${checkboxes.length} checked items in ${sectionTitle}`);
                
                // Create hidden inputs for each checked item
                checkboxes.forEach((checkbox, index) => {
                    const hiddenInput = document.createElement('input');
                    hiddenInput.type = 'hidden';
                    hiddenInput.name = `${fieldName}[${index}]`;
                    hiddenInput.value = checkbox.value || checkbox.getAttribute('data-value') || 'unknown';
                    form.appendChild(hiddenInput);
                    
                    console.log(`Added: ${fieldName}[${index}] = ${hiddenInput.value}`);
                });
            } else {
                console.warn(`Section header not found: ${sectionTitle}`);
                // Let's see what H2s we do have
                const availableHeaders = Array.from(headers).map(h => h.textContent.trim());
                console.log('Available H2 headers in section 2:', availableHeaders);
            }
        });
        
        console.log('=== EQUIPMENT DATA PREPARATION COMPLETE ===');
    }
    
    function prepareFileData() {
        console.log('=== PREPARING FILE DATA ===');
        
        const fotosInput = document.getElementById('fotosInput');
        if (!fotosInput) {
            console.error('Fotos input not found!');
            return;
        }
        
        if (uploadedFiles.length > 0) {
            console.log(`Preparing ${uploadedFiles.length} files for upload...`);
            
            // Create a new DataTransfer object to hold our files
            const dt = new DataTransfer();
            
            // Add each file to the DataTransfer
            uploadedFiles.forEach((file, index) => {
                console.log(`Adding file ${index + 1}: ${file.name}`);
                dt.items.add(file);
            });
            
            // Assign the files to the input
            fotosInput.files = dt.files;
            
            console.log(`Files assigned to input. Input now has ${fotosInput.files.length} files`);
        } else {
            console.log('No files to prepare');
        }
        
        console.log('=== FILE DATA PREPARATION COMPLETE ===');
    }
    
    // Utility functions for form inputs
    window.formatKilometraje = function(input) {
        let value = input.value.replace(/\D/g, '');
        value = value.replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        input.value = value;
    };
    
    window.formatPrice = function(input) {
        let value = input.value.replace(/\D/g, '');
        value = value.replace(/\B(?=(\d{3})+(?!\d))/g, ',');
        input.value = value;
    };
    
    // Province/Canton cascade
    window.updateCantones = function(provinciaSelect) {
        const cantonSelect = document.querySelector('#canton');
        const provincia = provinciaSelect.value;
        
        // Clear current options
        cantonSelect.innerHTML = '<option value="">Elige un cantón</option>';
        
        // Add cantones based on province
        const cantonesPorProvincia = {
            'San José': ['Central', 'Escazú', 'Desamparados', 'Puriscal', 'Tarrazú', 'Aserrí', 'Mora', 'Goicoechea', 'Santa Ana', 'Alajuelita', 'Coronado', 'Acosta', 'Tibás', 'Moravia', 'Montes de Oca', 'Turrubares', 'Dota', 'Curridabat', 'Pérez Zeledón', 'León Cortés'],
            'Alajuela': ['Central', 'San Ramón', 'Grecia', 'San Mateo', 'Atenas', 'Naranjo', 'Palmares', 'Poás', 'Orotina', 'San Carlos', 'Zarcero', 'Sarchí', 'Upala', 'Los Chiles', 'Guatuso'],
            'Cartago': ['Central', 'Paraíso', 'La Unión', 'Jiménez', 'Turrialba', 'Alvarado', 'Oreamuno', 'El Guarco'],
            'Heredia': ['Central', 'Barva', 'Santo Domingo', 'Santa Bárbara', 'San Rafael', 'San Isidro', 'Belén', 'Flores', 'San Pablo', 'Sarapiquí'],
            'Guanacaste': ['Liberia', 'Nicoya', 'Santa Cruz', 'Bagaces', 'Carrillo', 'Cañas', 'Abangares', 'Tilarán', 'Nandayure', 'La Cruz', 'Hojancha'],
            'Puntarenas': ['Central', 'Esparza', 'Buenos Aires', 'Montes de Oro', 'Osa', 'Quepos', 'Golfito', 'Coto Brus', 'Parrita', 'Corredores', 'Garabito'],
            'Limón': ['Central', 'Pococí', 'Siquirres', 'Talamanca', 'Matina', 'Guácimo']
        };
        
        if (cantonesPorProvincia[provincia]) {
            cantonesPorProvincia[provincia].forEach(canton => {
                const option = document.createElement('option');
                option.value = canton;
                option.textContent = canton;
                cantonSelect.appendChild(option);
            });
        }
    };

    // Marca/Modelo cascade
    function updateModelos() {
        const marcaSelect = document.querySelector('#marca');
        const modeloSelect = document.querySelector('#modelo');
        const marca = marcaSelect.value;
        
        // Clear current options
        modeloSelect.innerHTML = '<option value="">Elige un modelo</option>';
        
        // Add models based on brand
        const modelosPorMarca = {
            'toyota': ['Corolla', 'Camry', 'Prius', 'RAV4', 'Highlander', 'Tacoma', 'Tundra', '4Runner', 'Sienna', 'Avalon'],
            'honda': ['Civic', 'Accord', 'CR-V', 'Pilot', 'Fit', 'HR-V', 'Passport', 'Ridgeline', 'Insight', 'Odyssey'],
            'nissan': ['Sentra', 'Altima', 'Maxima', 'Rogue', 'Pathfinder', 'Murano', 'Frontier', 'Titan', 'Armada', 'Leaf'],
            'hyundai': ['Elantra', 'Sonata', 'Tucson', 'Santa Fe', 'Kona', 'Palisade', 'Accent', 'Veloster', 'Genesis', 'Ioniq'],
            'kia': ['Forte', 'Optima', 'Sportage', 'Sorento', 'Soul', 'Seltos', 'Telluride', 'Stinger', 'Niro', 'Carnival'],
            'mazda': ['Mazda3', 'Mazda6', 'CX-3', 'CX-5', 'CX-9', 'MX-5', 'CX-30', 'Mazda2', 'BT-50'],
            'chevrolet': ['Spark', 'Sonic', 'Cruze', 'Malibu', 'Equinox', 'Traverse', 'Silverado', 'Tahoe', 'Suburban'],
            'ford': ['Fiesta', 'Focus', 'Fusion', 'Escape', 'Explorer', 'F-150', 'Ranger', 'Expedition', 'Mustang'],
            'volkswagen': ['Jetta', 'Passat', 'Tiguan', 'Atlas', 'Golf', 'Beetle', 'Touareg', 'Arteon']
        };
        
        if (modelosPorMarca[marca]) {
            modelosPorMarca[marca].forEach(modelo => {
                const option = document.createElement('option');
                option.value = modelo.toLowerCase();
                option.textContent = modelo;
                modeloSelect.appendChild(option);
            });
        }
    }

    // Utility functions for form interactions
    // handleFileUpload function moved to avoid duplication - see line 748 for the main implementation

    function togglePaymentMethod() {
        const selectedMethod = document.querySelector('input[name="payment-method"]:checked');
        const paymentDetails = document.querySelectorAll('.payment-details');
        
        paymentDetails.forEach(detail => {
            detail.style.display = 'none';
        });
        
        if (selectedMethod) {
            const targetDetail = document.querySelector(`#${selectedMethod.value}-details`);
            if (targetDetail) {
                targetDetail.style.display = 'block';
            }
        }
    }

    function updateImageViewMode(container, mode) {
        if (!container) return;
        
        const images = container.querySelectorAll('.preview-image');
        const objectFitValue = mode === 'cover' ? 'cover' : 'contain';
        
        images.forEach(img => {
            img.style.objectFit = objectFitValue;
        });
        
        console.log(`Updated image view mode to: ${mode}`);
    }

    function updateEquipmentSelection(event) {
        const checkbox = event.target;
        const equipmentItem = checkbox.closest('.equipment-item');
        
        if (checkbox.checked) {
            equipmentItem.classList.add('selected');
        } else {
            equipmentItem.classList.remove('selected');
        }
    }

    function updatePlanSelection(event) {
        const radio = event.target;
        const planItems = document.querySelectorAll('.plan-item');
        
        planItems.forEach(item => {
            item.classList.remove('selected');
        });
        
        const selectedPlan = radio.closest('.plan-item');
        if (selectedPlan) {
            selectedPlan.classList.add('selected');
        }
    }

    function updateTagSelection(event) {
        const checkbox = event.target;
        const tagOption = checkbox.closest('.tag-option');
        
        if (checkbox.checked) {
            tagOption.classList.add('selected');
        } else {
            tagOption.classList.remove('selected');
        }
    }
});