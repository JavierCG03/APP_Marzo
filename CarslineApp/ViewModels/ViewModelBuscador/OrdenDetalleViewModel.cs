using CarslineApp.Models;
using CarslineApp.Services;
using CarslineApp.Views.Ordenes;
using CarslineApp.Views.Buscador;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace CarslineApp.ViewModels.ViewModelBuscador
{
    public class OrdenDetalleViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private readonly int _ordenId;
        private OrdenConTrabajosDto _orden;
        private ClienteDto _cliente;
        private VehiculoDto _vehiculo;
        private bool _isLoading;
        private string _errorMessage;
        private bool _mostrarModalTrabajos;
        private ObservableCollection<CatalogoServicioSeleccionable> _serviciosDisponibles;

        public OrdenDetalleViewModel(int ordenId)
        {
            _apiService = new ApiService();
            _ordenId = ordenId;

            // Inicializar colecciones
            ServiciosDisponibles = new ObservableCollection<CatalogoServicioSeleccionable>();

            // Comandos
            VerClienteCommand = new Command(async () => await VerCliente());
            VerVehiculoCommand = new Command(async () => await VerVehiculo());
            VerRefaccionesCommand = new Command<int>(async (id) => await VerRefaccionesTrabajo(id));
            VerEvidenciasCommand = new Command(async () => await VerEvidencias());
            CancelarOrdenCommand = new Command(async () => await CancelarOrden());
            EntregarOrdenCommand = new Command(async () => await EntregarOrden());
            RefreshCommand = new Command(async () => await CargarDatosOrden());
            VerEvidenciasTrabajoCommand = new Command(async () => await VerEvidenciasTrabajo());
            GenerarPdfCommand = new Command(async () => await OnVerReporte());
            AbrirModalTrabajosCommand = new Command(async () => await AbrirModalTrabajos());
            CerrarModalTrabajosCommand = new Command(() => CerrarModalTrabajos());
            ConfirmarAgregarTrabajosCommand = new Command(async () => await ConfirmarAgregarTrabajos());
            CompartirPdfCommand = new Command(async () => await CompartirPdf(), () => TieneOrden);
            EnviarWhatsAppCommand = new Command(async () => await EnviarWhatsApp(), () => TieneOrden);
            EliminarTrabajoCommand = new Command<int>(async (id) => await EliminarTrabajo(id)); // NUEVO

            _ = CargarDatosOrden();
        }

        #region Propiedades

        public OrdenConTrabajosDto Orden
        {
            get => _orden;
            set
            {
                System.Diagnostics.Debug.WriteLine($"📝 SET Orden - Antes: {(_orden == null ? "NULL" : _orden.NumeroOrden)}");
                System.Diagnostics.Debug.WriteLine($"📝 SET Orden - Nuevo: {(value == null ? "NULL" : value.NumeroOrden)}");

                _orden = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TieneOrden));
                OnPropertyChanged(nameof(EsPendiente));
                OnPropertyChanged(nameof(EsEnProceso));
                OnPropertyChanged(nameof(EsFinalizada));
                OnPropertyChanged(nameof(PuedeEntregar));
                OnPropertyChanged(nameof(PuedeCancelar));
                OnPropertyChanged(nameof(ColorEstado));
                OnPropertyChanged(nameof(IconoEstado));
                OnPropertyChanged(nameof(MostrarBotonAgregarTrabajos));
                ((Command)CompartirPdfCommand).ChangeCanExecute();
                ((Command)EnviarWhatsAppCommand).ChangeCanExecute();
                // Actualizar comando
                try
                {
                    System.Diagnostics.Debug.WriteLine("🔄 Actualizando CanExecute de GenerarPdfCommand...");
                    ((Command)GenerarPdfCommand).ChangeCanExecute();
                    System.Diagnostics.Debug.WriteLine($"✅ CanExecute actualizado - TieneOrden: {TieneOrden}");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"❌ Error actualizando CanExecute: {ex.Message}");
                }
            }
        }

        public ClienteDto Cliente
        {
            get => _cliente;
            set { _cliente = value; OnPropertyChanged(); }
        }

        public VehiculoDto Vehiculo
        {
            get => _vehiculo;
            set { _vehiculo = value; OnPropertyChanged(); }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        public bool MostrarModalTrabajos
        {
            get => _mostrarModalTrabajos;
            set
            {
                _mostrarModalTrabajos = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<CatalogoServicioSeleccionable> ServiciosDisponibles
        {
            get => _serviciosDisponibles;
            set
            {
                _serviciosDisponibles = value;
                OnPropertyChanged();
            }
        }

        // Propiedades calculadas
        public bool TieneOrden => Orden != null;
        public bool EsPendiente => Orden?.EstadoOrdenId == 1;
        public bool EsEnProceso => Orden?.EstadoOrdenId == 2;
        public bool EsFinalizada => Orden?.EstadoOrdenId == 3;
        public bool PuedeEntregar => EsFinalizada && Orden?.ProgresoGeneral >= 100;
        public bool PuedeCancelar => EsPendiente;

        // Nueva propiedad: Solo muestra el botón si es TipoOrdenId == 1 (Servicio) y la orden aun no ha sido entregada
        public bool MostrarBotonAgregarTrabajos => Orden?.TipoOrdenId == 1 && Orden?.EstadoOrdenId < 4;

        public string ColorEstado => Orden?.EstadoOrdenId switch
        {
            1 => "#FFA500", // Pendiente - Naranja
            2 => "#2196F3", // En Proceso - Azul
            3 => "#00BCD4", // Finalizada - Turquesa
            4 => "#4CAF50", // Entregada - Verde 
            5 => "#757575", // Cancelada - Gris oscuro
            _ => "#757575"  // Desconocido - Gris
        };

        public string IconoEstado => Orden?.EstadoOrdenId switch
        {
            1 => "📋",  // Pendiente
            2 => "⚙️",  // En Proceso
            3 => "✔️",  // Finalizada
            4 => "✅",  // Entregada
            5 => "❌",  // Cancelada          
            _ => "❓"   // Desconocido
        };
        #endregion

        #region Comandos

        public ICommand VerClienteCommand { get; }
        public ICommand VerVehiculoCommand { get; }
        public ICommand VerRefaccionesCommand { get; }
        public ICommand VerEvidenciasCommand { get; }
        public ICommand CancelarOrdenCommand { get; }
        public ICommand EntregarOrdenCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand VerEvidenciasTrabajoCommand { get; }
        public ICommand GenerarPdfCommand { get; }
        public ICommand AbrirModalTrabajosCommand { get; }
        public ICommand CerrarModalTrabajosCommand { get; }
        public ICommand ConfirmarAgregarTrabajosCommand { get; }
        public ICommand EliminarTrabajoCommand { get; }
        public ICommand CompartirPdfCommand { get; }
        public ICommand EnviarWhatsAppCommand { get; }
        #endregion

        #region Métodos - Eliminar Trabajo

        private async Task EliminarTrabajo(int trabajoId)
        {
            try
            {
                // Buscar el trabajo en la lista actual
                var trabajo = Orden?.Trabajos?.FirstOrDefault(t => t.Id == trabajoId);

                if (trabajo == null)
                {
                    await MostrarAlerta("Error", "Trabajo no encontrado");
                    return;
                }

                // Validación: Estado del trabajo debe ser < 3 (Pendiente o Asignado)
                if (trabajo.EstadoTrabajo >= 3)
                {
                    await MostrarAlerta(
                        "No se puede eliminar",
                        "Solo se pueden eliminar trabajos que estén Pendientes o Asignados.\n\n" +
                        "Los trabajos En Proceso, Completados o Pausados no pueden eliminarse.");
                    return;
                }

                // Validación: No debe ser el único trabajo
                int totalTrabajos = Orden?.Trabajos?.Count ?? 0;
                if (totalTrabajos <= 1)
                {
                    await MostrarAlerta(
                        "No se puede eliminar",
                        "No se puede eliminar el único trabajo de la orden.\n\n" +
                        "Una orden debe tener al menos un trabajo.");
                    return;
                }

                // Confirmación
                bool confirmar = await Application.Current.MainPage.DisplayAlert(
                    "⚠️ Eliminar Trabajo",
                    $"¿Estás seguro de eliminar el siguiente trabajo?\n\n" +
                    $"📋 {trabajo.Trabajo}\n" +
                    $"Estado: {trabajo.EstadoTrabajoNombre}\n\n" +
                    "Esta acción no se puede deshacer.",
                    "Sí, eliminar",
                    "Cancelar");

                if (!confirmar) return;

                IsLoading = true;

                // Llamar al API
                var response = await _apiService.EliminarTrabajoAsync(trabajoId);

                if (response.Success)
                {
                    await MostrarAlerta("✅ Éxito", "Trabajo eliminado correctamente");

                    // Recargar la orden para actualizar la vista
                    await CargarDatosOrden();
                }
                else
                {
                    await MostrarAlerta("❌ Error", response.Message);
                }
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"Error al eliminar trabajo: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Métodos - Agregar Trabajos

        private async Task AbrirModalTrabajos()
        {
            bool confirmar = await Application.Current.MainPage.DisplayAlert(
                "➕ Agregar Trabajos",
                "¿Deseas agregar trabajos adicionales a esta orden?",
                "OK",
                "Cancelar");

            if (!confirmar) return;

            IsLoading = true;
            try
            {
                // Obtener catálogo completo de servicios
                var catalogoCompleto = await _apiService.ObtenerServiciosFrecuentesAsync();

                if (catalogoCompleto == null || catalogoCompleto.Count == 0)
                {
                    await MostrarAlerta("Error", "No se pudo cargar el catálogo de servicios");
                    return;
                }

                // Filtrar servicios que ya están en la orden
                var trabajosExistentes = Orden?.Trabajos?.Select(t => t.Trabajo).ToList() ?? new List<string>();

                var serviciosFiltrados = catalogoCompleto
                    .Where(s => !trabajosExistentes.Contains(s.Nombre, StringComparer.OrdinalIgnoreCase))
                    .Select(s => new CatalogoServicioSeleccionable
                    {
                        Id = s.Id,
                        Trabajo = s.Nombre,
                        Indicaciones = "",
                        EstaSeleccionado = false
                    })
                    .ToList();

                if (serviciosFiltrados.Count == 0)
                {
                    await MostrarAlerta(
                        "Información",
                        "Todos los servicios disponibles ya están agregados a esta orden");
                    return;
                }

                ServiciosDisponibles.Clear();
                foreach (var servicio in serviciosFiltrados)
                {
                    ServiciosDisponibles.Add(servicio);
                }

                MostrarModalTrabajos = true;
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"Error al cargar servicios: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void CerrarModalTrabajos()
        {
            MostrarModalTrabajos = false;

            // Limpiar selecciones
            foreach (var servicio in ServiciosDisponibles)
            {
                servicio.EstaSeleccionado = false;
                servicio.Indicaciones = "";
            }
        }

        private async Task ConfirmarAgregarTrabajos()
        {
            var trabajosSeleccionados = ServiciosDisponibles
                .Where(s => s.EstaSeleccionado)
                .ToList();

            if (trabajosSeleccionados.Count == 0)
            {
                await MostrarAlerta("Atención", "Debes seleccionar al menos un trabajo");
                return;
            }

            // Crear mensaje con los trabajos seleccionados
            var mensaje = "Se agregarán los siguientes trabajos:\n\n";
            foreach (var trabajo in trabajosSeleccionados)
            {
                mensaje += $"• {trabajo.Trabajo}\n";
                if (!string.IsNullOrWhiteSpace(trabajo.Indicaciones))
                {
                    mensaje += $"  Indicaciones: {trabajo.Indicaciones}\n";
                }
            }
            mensaje += "\n¿Deseas continuar?";

            bool confirmar = await Application.Current.MainPage.DisplayAlert(
                "Confirmar Trabajos",
                mensaje,
                "Sí, agregar",
                "Cancelar");

            if (!confirmar) return;

            IsLoading = true;
            MostrarModalTrabajos = false;

            try
            {
                int exitosos = 0;
                int fallidos = 0;
                string errores = "";

                foreach (var trabajo in trabajosSeleccionados)
                {
                    var dto = new TrabajoCrearDto
                    {
                        Trabajo = trabajo.Trabajo,
                        Indicaciones = trabajo.Indicaciones ?? null

                       /* Indicaciones = string.IsNullOrWhiteSpace(trabajo.Indicaciones)
                            ? "Sin indicaciones"
                            : trabajo.Indicaciones*/
                    };

                    var resultado = await _apiService.AgregarTrabajoAsync(_ordenId, dto);

                    if (resultado.Success)
                    {
                        exitosos++;
                    }
                    else
                    {
                        fallidos++;
                        errores += $"• {trabajo.Trabajo}: {resultado.Message}\n";
                    }
                }

                // Recargar la orden
                await CargarDatosOrden();

                // Mostrar resultado
                if (fallidos == 0)
                {
                    await MostrarAlerta(
                        "✅ Éxito",
                        $"Se agregaron {exitosos} trabajo(s) exitosamente");
                }
                else
                {
                    await MostrarAlerta(
                        "⚠️ Resultado Parcial",
                        $"Exitosos: {exitosos}\nFallidos: {fallidos}\n\nErrores:\n{errores}");
                }

                // Limpiar selecciones
                ServiciosDisponibles.Clear();
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"Error al agregar trabajos: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        #region Metodos - PDF

        private async Task<string> ObtenerRutaPdfLocal()
        {
            try
            {
                var result = await _apiService.DescargarPdfOrdenAsync(_ordenId);

                if (!result.Success || result.PdfBytes == null || result.PdfBytes.Length == 0)
                {
                    await MostrarAlerta("❌ Error", result.Message ?? "El servidor no devolvió datos para el PDF.");
                    return null;
                }

                string carpeta = FileSystem.CacheDirectory;
                string nombreArchivo = result.NombreArchivo ?? $"Orden_{_ordenId}_{DateTime.Now:yyyyMMdd}.pdf";
                string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                await File.WriteAllBytesAsync(rutaCompleta, result.PdfBytes);
                return rutaCompleta;
            }
            catch (Exception ex)
            {
                await MostrarAlerta("❌ Error", $"No se pudo generar el PDF:\n{ex.Message}");
                return null;
            }
        }


        // ═══════════════════════════════════════════════════════════════
        //  COMPARTIR PDF → hoja nativa del SO
        // ═══════════════════════════════════════════════════════════════
        private async Task CompartirPdf()
        {
            IsLoading = true;
            try
            {
                var rutaCache = await ObtenerRutaPdfLocal();
                if (rutaCache == null) return;

                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = $"Reporte Orden {Orden?.NumeroOrden}",
                    File = new ShareFile(rutaCache, "application/pdf")
                });
            }
            catch (Exception ex)
            {
                await MostrarAlerta("❌ Error", $"No se pudo compartir el archivo:\n{ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task EnviarWhatsApp()
        {
            try
            {
                // ── 1. Resolver número de teléfono ──────────────────────────────
                string telefono = new string(
                    (Orden?.ClienteTelefono ?? string.Empty)
                    .Where(char.IsDigit).ToArray());

                bool usarNumeroCliente = false;

                if (!string.IsNullOrWhiteSpace(telefono))
                {
                    usarNumeroCliente = await Application.Current!.MainPage!.DisplayAlert(
                        "📱 Enviar por WhatsApp",
                        $"El cliente tiene registrado:\n📞 {Orden?.ClienteTelefono}\n\n¿Deseas usar ese número?",
                        "Sí, usar ese número",
                        "No, ingresar otro");
                }

                if (!usarNumeroCliente)
                {
                    string? ingresado = await Application.Current!.MainPage!.DisplayPromptAsync(
                        "📱 Número de WhatsApp",
                        "Ingresa el número (10 dígitos, sin espacios ni guiones).\nEjemplo: 8112345678",
                        placeholder: "8112345678",
                        keyboard: Keyboard.Telephone,
                        maxLength: 10);

                    if (string.IsNullOrWhiteSpace(ingresado)) return; // usuario canceló

                    telefono = new string(ingresado.Where(char.IsDigit).ToArray());
                }

                // ── 2. Validar número ───────────────────────────────────────────
                if (telefono.Length < 10)
                {
                    await MostrarAlerta("⚠️ Número inválido",
                        "El número debe tener al menos 10 dígitos.\nEjemplo: 8112345678");
                    return;
                }

                // Agregar lada de México (+52) si no la tiene
                if (telefono.Length == 10)
                    telefono = "52" + telefono;

                // ── 3. Construir mensaje ────────────────────────────────────────
                string mensaje =
                    $"🔧 *REPORTE DE SERVICIO*\n\n" +
                    $"Estimado/a *{Orden?.ClienteNombre}*, le compartimos el reporte de su vehículo.\n\n" +
                    $"🚗 *{Orden?.VehiculoCompleto}*\n" +
                    $"📋 Orden: *{Orden?.NumeroOrden}*\n" +
                    $"📅 Fecha: {DateTime.Now:dd/MM/yyyy}\n\n" +
                    $"Gracias por su preferencia. ✅";

                string mensajeCodificado = Uri.EscapeDataString(mensaje);

                // ── 4. Abrir WhatsApp ───────────────────────────────────────────
                bool abierto = false;
                try
                {
                    var uriNativo = new Uri($"whatsapp://send?phone={telefono}&text={mensajeCodificado}");
                    if (await Launcher.Default.CanOpenAsync(uriNativo))
                    {
                        await Launcher.Default.OpenAsync(uriNativo);
                        abierto = true;
                    }
                }
                catch { /* fallback a web */ }

                if (!abierto)
                {
                    var uriWeb = new Uri($"https://wa.me/{telefono}?text={mensajeCodificado}");
                    await Browser.Default.OpenAsync(uriWeb, BrowserLaunchMode.SystemPreferred);
                }

                // ── 5. Ofrecer compartir el PDF también ─────────────────────────
                bool adjuntar = await Application.Current!.MainPage!.DisplayAlert(
                    "📎 ¿Adjuntar PDF?",
                    "WhatsApp fue abierto con el mensaje.\n\n" +
                    "¿Deseas compartir también el archivo PDF ahora?",
                    "Sí, compartir PDF",
                    "No, solo el mensaje");

                if (adjuntar)
                    await CompartirPdf();
            }
            catch (Exception ex)
            {
                await MostrarAlerta("❌ Error", $"No se pudo abrir WhatsApp:\n{ex.Message}");
            }
        }
        #endregion 
        #region Métodos Originales

        private async Task OnVerReporte()
        {
            try
            {
                IsLoading = true;

                if (_ordenId <= 0)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        "No hay una orden seleccionada para generar el reporte",
                        "OK");
                    return;
                }

                await Application.Current.MainPage.Navigation.PushAsync(
                    new VerReportePage(_ordenId));
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"No se pudo abrir el reporte: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task VerEvidenciasTrabajo()
        {
            if (Orden == null) return;

            try
            {
                var evidenciasPage = new EvidenciasOrdenTrabajo(Orden.Id, 1);
                await Application.Current.MainPage.Navigation.PushAsync(evidenciasPage);
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"No se pudo abrir las evidencias de trabajo: {ex.Message}");
            }
        }

        private async Task VerRefaccionesTrabajo(int TrabajoID)
        {
            int trabajoId = TrabajoID;

            try
            {
                var refaccionesTrabajoPage = new RefaccionesTrabajo(trabajoId);
                await Application.Current.MainPage.Navigation.PushAsync(refaccionesTrabajoPage);
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"No se pudo abrir las refacciones de trabajo: {ex.Message}");
            }
        }

        private async Task CargarDatosOrden()
        {
            System.Diagnostics.Debug.WriteLine($"🔍 Iniciando carga de orden {_ordenId}");

            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                System.Diagnostics.Debug.WriteLine("📡 Llamando API...");
                var ordenCompleta = await _apiService.ObtenerOrdenCompletaAsync(_ordenId);

                if (ordenCompleta != null)
                {
                    System.Diagnostics.Debug.WriteLine($"✅ Orden cargada: {ordenCompleta.NumeroOrden}");

                    // Calcular si cada trabajo puede ser eliminado
                    if (ordenCompleta.Trabajos != null && ordenCompleta.Trabajos.Count > 0)
                    {
                        bool esUnicoTrabajo = ordenCompleta.Trabajos.Count == 1;

                        foreach (var trabajo in ordenCompleta.Trabajos)
                        {
                            // Puede eliminar si: Estado < 3 Y no es el único trabajo
                            trabajo.PuedeEliminar = trabajo.EstadoTrabajo < 3 && !esUnicoTrabajo;
                        }
                    }

                    Orden = ordenCompleta;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("❌ ordenCompleta es NULL");
                    ErrorMessage = "No se pudo cargar la orden";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"💥 EXCEPCIÓN: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"📚 StackTrace: {ex.StackTrace}");
                ErrorMessage = $"Error al cargar datos: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
                System.Diagnostics.Debug.WriteLine("🏁 Finalizó carga");
            }
        }

        private async Task VerCliente()
        {
            if (Orden == null) return;

            try
            {
                var clientePage = new ClientesPage(Orden.ClienteId);
                await Application.Current.MainPage.Navigation.PushAsync(clientePage);
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"No se pudo abrir los datos del cliente: {ex.Message}");
            }
        }

        private async Task VerVehiculo()
        {
            if (Orden == null) return;

            try
            {
                var vehiculoPage = new VehiculosPage(Orden.VehiculoId);
                await Application.Current.MainPage.Navigation.PushAsync(vehiculoPage);
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"No se pudo abrir los datos del vehículo: {ex.Message}");
            }
        }

        private async Task VerEvidencias()
        {
            if (Orden == null) return;

            try
            {
                var evidenciasPage = new EvidenciasOrdenTrabajo(Orden.Id, 2);
                await Application.Current.MainPage.Navigation.PushAsync(evidenciasPage);
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"No se pudo abrir las evidencias: {ex.Message}");
            }
        }

        private async Task CancelarOrden()
        {
            if (Orden == null || !PuedeCancelar) return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "⚠️ Cancelar Orden",
                $"¿Estás seguro de cancelar la orden {Orden.NumeroOrden}?\n\n" +
                "Esta acción cancelará todos los trabajos asociados.",
                "Sí, cancelar",
                "No");

            if (!confirm) return;

            IsLoading = true;
            try
            {
                var response = await _apiService.CancelarOrdenAsync(Orden.Id);

                if (response.Success)
                {
                    await MostrarAlerta("✅ Éxito", "Orden cancelada correctamente");
                    await CargarDatosOrden();
                }
                else
                {
                    await MostrarAlerta("❌ Error", response.Message);
                }
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"Error al cancelar: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task EntregarOrden()
        {
            if (Orden == null || !PuedeEntregar) return;

            if (Orden.ProgresoGeneral < 100)
            {
                await MostrarAlerta(
                    "⚠️ No se puede entregar",
                    $"La orden aún no está completada.\n\n" +
                    $"Progreso actual: {Orden.ProgresoFormateado}\n" +
                    $"Trabajos: {Orden.ProgresoTexto}");
                return;
            }

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "🚗 Entregar Vehículo",
                $"¿Confirmas la entrega del vehículo?\n\n" +
                $"Orden: {Orden.NumeroOrden}\n" +
                $"Cliente: {Orden.ClienteNombre}\n" +
                $"Vehículo: {Orden.VehiculoCompleto}",
                "Sí, entregar",
                "Cancelar");

            if (!confirm) return;

            IsLoading = true;
            try
            {
                var response = await _apiService.EntregarOrdenAsync(Orden.Id);

                if (response.Success)
                {
                    await MostrarAlerta(
                        "✅ Vehículo Entregado",
                        "El vehículo ha sido entregado correctamente.\n" +
                        "Se ha registrado en el historial.");

                    await Application.Current.MainPage.Navigation.PopAsync();
                }
                else
                {
                    await MostrarAlerta("❌ Error", response.Message);
                }
            }
            catch (Exception ex)
            {
                await MostrarAlerta("Error", $"Error al entregar: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task MostrarAlerta(string titulo, string mensaje)
        {
            try
            {
                await Application.Current.MainPage.DisplayAlert(titulo, mensaje, "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error mostrando alerta: {ex.Message}");
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    #region Clases Auxiliares

    public class CatalogoServicioSeleccionable : INotifyPropertyChanged
    {
        private bool _estaSeleccionado;
        private string _indicaciones;

        public int Id { get; set; }
        public string Trabajo { get; set; }

        public bool EstaSeleccionado
        {
            get => _estaSeleccionado;
            set
            {
                _estaSeleccionado = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MostrarIndicaciones));
            }
        }

        public string Indicaciones
        {
            get => _indicaciones;
            set
            {
                _indicaciones = value;
                OnPropertyChanged();
            }
        }

        public bool MostrarIndicaciones => EstaSeleccionado;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    #endregion
}