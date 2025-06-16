// wwwroot/js/shared-transporte.js

// Función principal para configurar formularios de transporte
function configurarFormularioTransporte(formId, esEntradaGeneral = false) {
    const form = $(`#${formId}`);

    // 1. Configuración inicial de campos
    $('#numCajaInput, #numSelloInput').prop('readonly', true);

    // 2. Manejo de checkboxes de estado
    $('.estado-checkbox').change(function () {
        if ($(this).is(':checked')) {
            $('.estado-checkbox').not(this).prop('checked', false);

            if ($('#vacioCheckbox').is(':checked') || $('#cargadoCheckbox').is(':checked')) {
                $('#numCajaInput, #numSelloInput').prop('readonly', false);
            } else if ($('#botandoCheckbox').is(':checked')) {
                $('#numCajaInput, #numSelloInput').prop('readonly', true).val('');
            }
        } else {
            $('#numCajaInput, #numSelloInput').prop('readonly', true).val('');
        }
    });

    // 3. Configuración específica para Entrada (no general)
    if (!esEntradaGeneral) {
        const transportistaSeleccionado = $('#transportistaDropdown').val();
        if (transportistaSeleccionado) {
            cargarOperadores(transportistaSeleccionado);
            cargarEconomicos(transportistaSeleccionado);
        }

        $('#transportistaDropdown').change(function () {
            const transportistaId = $(this).val();
            if (transportistaId) {
                cargarOperadores(transportistaId);
                cargarEconomicos(transportistaId);
            } else {
                $('#operadorDropdown, #economicoDropdown').html('<option value="">Seleccione...</option>');
            }
        });
    }

    // 4. Manejo del envío del formulario
    form.on('submit', function (e) {
        e.preventDefault();

        if (!$(this).valid()) {
            mostrarError('Formulario incompleto', 'Por favor complete todos los campos requeridos');
            return;
        }

        const datos = obtenerDatosConfirmacion(esEntradaGeneral);
        mostrarConfirmacion(datos, formId);
    });

    // 5. Añadir estilos dinámicos
    agregarEstilosAnimaciones();
}

// Función para cargar operadores (para Entrada normal)
function cargarOperadores(transportistaId, selectedId = null) {
    $('#operadorDropdown').html('<option value="">Seleccione operador</option><option value="" disabled>Cargando...</option>');

    $.get('/Entrada/GetOperadoresByTransportista', { idTransportista: transportistaId }, function (data) {
        let options = '<option value="">Seleccione operador</option>';
        $.each(data, function () {
            const selected = (selectedId && this.Id == selectedId) ? 'selected' : '';
            options += `<option value="${this.Id}" ${selected}>${this.NombreOperador}</option>`;
        });
        $('#operadorDropdown').html(options);
    }).fail(function () {
        $('#operadorDropdown').html('<option value="">Error al cargar operadores</option>');
    });
}

// Función para cargar económicos (para Entrada normal)
function cargarEconomicos(transportistaId, selectedId = null) {
    $('#economicoDropdown').html('<option value="">Seleccione economico</option><option value="" disabled>Cargando...</option>');

    $.get('/Entrada/GetEconomicosByTransportista', { idTransportista: transportistaId }, function (data) {
        let options = '<option value="">Seleccione economico</option>';
        $.each(data, function () {
            const selected = (selectedId && this.Id == selectedId) ? 'selected' : '';
            options += `<option value="${this.Id}" ${selected}>${this.Economico}</option>`;
        });
        $('#economicoDropdown').html(options);
    }).fail(function () {
        $('#economicoDropdown').html('<option value="">Error al cargar económicos</option>');
    });
}

// Función para obtener datos de confirmación
function obtenerDatosConfirmacion(esEntradaGeneral) {
    return {
        transportista: esEntradaGeneral ? $('#TransportistaNombre').val() : $('#transportistaDropdown option:selected').text(),
        operador: esEntradaGeneral ? $('#OperadorNombre').val() : $('#operadorDropdown option:selected').text(),
        economico: esEntradaGeneral ? $('#EconomicoNombre').val() : $('#economicoDropdown option:selected').text(),
        tipo: esEntradaGeneral ? 'Entrada General' : 'Entrada'
    };
}

// Función para mostrar diálogo de confirmación
function mostrarConfirmacion(datos, formId) {
    const transportista = encodeURIComponent(datos.transportista || 'No especificado');
    const operador = encodeURIComponent(datos.operador || 'No especificado');
    const economico = encodeURIComponent(datos.economico || 'No especificado');

    let estadoTexto = 'No seleccionado';
    if ($('#vacioCheckbox').is(':checked')) estadoTexto = 'Vacio';
    else if ($('#cargadoCheckbox').is(':checked')) estadoTexto = 'Cargado';
    else if ($('#botandoCheckbox').is(':checked')) estadoTexto = 'Botando';

    // Determinar qué imagen mostrar según el tipo
    const imagenUrl = datos.tipo === 'Entrada General'
        ? '../../imagen/camionblanco.png'
        : '../../imagen/camionblanco.png';

    Swal.fire({
        title: `Confirmar Registro de ${datos.tipo}`,
        html: `
            <div class="text-center">
                <img src="${imagenUrl}" alt="Imagen de transporte" 
                     style="width: 100%; height: auto; margin-bottom: 15px;
                            animation: ${datos.tipo === 'Entrada General' ? 'moveTruck' : 'moveTruck'} 1.5s infinite;">
                <div class="confirmation-details">
                    <p><strong>Transportista:</strong> ${decodeURIComponent(transportista)}</p>
                    <p><strong>Operador:</strong> ${decodeURIComponent(operador)}</p>
                    <p><strong>Economico:</strong> ${decodeURIComponent(economico)}</p>
                    <p><strong>Estado:</strong> ${estadoTexto}</p>
                </div>
            </div>
        `,
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#6c757d',
        confirmButtonText: '<i class="fas fa-check-circle mr-2"></i> Confirmar',
        cancelButtonText: '<i class="fas fa-times-circle mr-2"></i> Cancelar',
        width: '600px',
        customClass: {
            popup: 'animated fadeIn',
            confirmButton: 'btn-confirm-transporte',
            cancelButton: 'btn-cancel-transporte'
        },
        backdrop: 'rgba(0,123,255,0.2)'
    }).then((result) => {
        if (result.isConfirmed) {
            mostrarLoaderRegistro(datos.tipo, formId);
        }
    });
}

// Función para mostrar loader durante el registro
function mostrarLoaderRegistro(tipo, formId) {
    Swal.fire({
        title: `Registrando ${tipo}`,
        html: `
            <div class="text-center">
                <img src="../../imagen/camionblanco.png" alt="Cargando..."
                     style="width: 100%; height: auto; margin-bottom: 15px;">
                <p>Procesando el registro...</p>
            </div>
        `,
        allowOutsideClick: false,
        showConfirmButton: false,
        willOpen: () => {
            setTimeout(() => {
                $(`#${formId}`).off('submit').submit();
            }, 500);
        }
    });
}

function mostrarError(titulo, mensaje, detalles = null) {
    const errorContent = detalles
        ? `<div class="text-left">
              <p class="mb-2">${mensaje}</p>
              <div class="error-details">
                  <strong>Detalles:</strong> ${detalles}
              </div>
           </div>`
        : mensaje;

    Swal.fire({
        icon: 'error',
        title: titulo,
        html: errorContent,
        showConfirmButton: true, // Asegurar que el botón se muestre
        confirmButtonText: 'Entendido',
        confirmButtonColor: '#dc3545',
        allowOutsideClick: false, // Evitar cerrar haciendo clic fuera
        width: '600px',
        customClass: {
            container: 'error-container', // Clase contenedora para mejor control
            popup: '', // Clase personalizada sin animación obligatoria
            title: 'error-title',
            confirmButton: 'error-confirm-btn'
        },
        willOpen: () => {
            setTimeout(() => {
                const icon = document.querySelector('.swal2-icon.swal2-error');
                const popup = document.querySelector('.swal2-popup');

                if (icon) {
                    icon.classList.add('animate-error-icon');
                }

                if (popup) {
                    popup.classList.add('shake-error-popup');

                    // Escuchar cuando termine la animación para quitar la clase
                    popup.addEventListener('animationend', () => {
                        popup.classList.remove('shake-error-popup');
                    }, { once: true }); // `once: true` asegura que solo se ejecute una vez
                }
            }, 10);
        }

    });
}

// Función para agregar estilos dinámicos
function agregarEstilosAnimaciones() {
    const style = document.createElement('style');
    style.innerHTML = `

    @keyframes shakePopup {
    0% { transform: translateX(0); }
    20% { transform: translateX(-10px); }
    40% { transform: translateX(10px); }
    60% { transform: translateX(-10px); }
    80% { transform: translateX(10px); }
    100% { transform: translateX(0); }
}

.shake-error-popup {
    animation: shakePopup 0.6s ease;
}

/* Animación solo para el icono de error (opcional) */
        @keyframes iconShake {
            0%, 100% { transform: translateX(0); }
            25% { transform: translateX(-5px); }
            75% { transform: translateX(5px); }
        }

        .animate-error-icon {
            animation: iconShake 0.5s ease 2;
        }

        /* Estilos para el diálogo de error */
        .custom-error-popup {
            border: 2px solid #dc3545;
            border-radius: 0.5rem;
        }

        .error-title {
            color: #dc3545;
            font-size: 1.5rem;
            margin-bottom: 1rem;
        }

        .error-confirm-btn {
            padding: 0.5rem 2rem;
            font-weight: bold;
            border-radius: 0.3rem;
            transition: all 0.2s;
        }

        .error-confirm-btn:hover {
            transform: scale(1.02);
            box-shadow: 0 0 10px rgba(220, 53, 69, 0.5);
        }

        .error-details {
            background-color: #f8d7da;
            border-left: 4px solid #dc3545;
            padding: 0.75rem;
            margin-top: 1rem;
            border-radius: 0 0.25rem 0.25rem 0;
        }

        /* Media queries para responsividad */
        @media (max-width: 768px) {
            .custom-error-popup {
                width: 90% !important;
                margin: 0 auto;
            }
        }



        @keyframes moveTruck {
            0% { transform: translateX(-10px); }
            50% { transform: translateX(10px); }
            100% { transform: translateX(-10px); }
        }
        
        @keyframes pulseScale {
            0% { transform: scale(1); opacity: 1; }
            50% { transform: scale(1.1); opacity: 0.8; }
            100% { transform: scale(1); opacity: 1; }
        }
        
        @keyframes pulse {
            0% { transform: scale(1); opacity: 1; }
            50% { transform: scale(1.1); opacity: 0.8; }
            100% { transform: scale(1); opacity: 1; }
        }
        
        .btn-confirm-transporte, .btn-cancel-transporte {
            padding: 0.5rem 1.5rem;
            font-size: 1rem;
            border-radius: 0.3rem;
        }
        
        .confirmation-details {
            background: #f8f9fa;
            border-radius: 0.5rem;
            padding: 1rem;
            margin: 1rem 0;
            text-align: left;
        }
        
        .confirmation-details p {
            margin-bottom: 0.5rem;
            font-size: 1rem;
        }
        
        @media (max-width: 576px) {
            .swal2-popup {
                width: 90% !important;
            }
            

        }
    `;
    document.head.appendChild(style);
}