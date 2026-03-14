using CarslineApp.Models;
using CarslineApp.Services;
using CarslineApp.Views.Avaluo;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CarslineApp.ViewModels
{
    public class EquipamientoAvaluoViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private int _avaluoId;
        private bool _isLoading;
        private string _errorMessage = string.Empty;

        public static readonly List<string> MarcasLlantas =
            new() { "Bridgestone", "Michelin", "Continental", "Goodyear", "Pirelli",
                    "Hankook", "Firestone", "BFGoodrich", "Yokohama", "Dunlop",
                    "Nexen", "Toyo", "Kumho", "General", "Cooper", "Falken", "Otra" };

        public static readonly List<string> VidaUtilOpciones =
            new() { "100%", "90%", "80%", "70%", "60%", "50%", "40%", "30%", "20%", "10%" };

        public static readonly List<int> CantidadPuertasOpciones =
            new() { 2, 3, 4, 5 };

        public static readonly List<int> CantidadCilindrosOpciones =
            new() { 2, 3, 4, 5, 6, 8 };

        public static readonly List<string> VestidurasOpciones =
            new() { "Tela", "Vinil", "Piel", "Piel Sintética", "Alcántara", "Mixta" };

        public static readonly List<string> MotorOpciones =
            new() { "1.0L", "1.2L", "1.4L", "1.5L", "1.6L", "1.8L", "2.0L",
                    "2.4L", "2.5L", "3.0L", "3.5L", "4.0L", "5.0L", "Híbrido", "Eléctrico" };

        public static readonly List<string> TenenciaOpciones =
            Enumerable.Range(DateTime.Now.Year - 5, 6).Reverse()
                      .Select(y => y.ToString()).Prepend("No pagada").ToList();

        public static readonly List<string> VerificacionOpciones =
            Enumerable.Range(DateTime.Now.Year - 3, 4).Reverse()
                      .Select(y => y.ToString()).Prepend("No tiene").ToList();

        public static readonly List<int> NumeroDuenosOpciones =
            Enumerable.Range(1,5).ToList();

        public static readonly List<int> RefacturacionesOpciones =
            Enumerable.Range(0, 4).ToList();

        public EquipamientoAvaluoViewModel(ApiService apiService, int avaluoId)
        {
            _apiService = apiService;
            AvaluoId = avaluoId;

            GuardarCommand = new Command(async () => await GuardarEquipamientoAsync());

            // Cargar info del vehículo en background
            Task.Run(async () => await CargarAvaluoAsync());
        }

        public ICommand GuardarCommand { get; }

        private bool _acc;
        private bool _quemacocos;
        private bool _espejosElectricos;
        private bool _segurosElectricos;
        private bool _cristalesElectricos;
        private bool _asientosElectricos;
        private bool _farosNiebla;
        private bool _rinesAluminio;
        private bool _controlesVolante;
        private bool _estereoCD;
        private bool _abs;
        private bool _direccionAsistida;
        private bool _bolsasAire;
        private bool _transmisionAutomatica;
        private bool _transmisionManual;
        private bool _turbo;
        private bool _traccion4x4;
        private bool _bluetooth;
        private bool _usb;
        private bool _pantalla;
        private bool _gps;
        private int _cantidadPuertas = 4;
        private string _vestiduras = string.Empty;
        private string _motor = string.Empty;
        private int _cantidadCilindros = 4;
        private bool _facturaOriginal;
        private int _numeroDuenos = 1;
        private int _refacturaciones;
        private string _tenenciaSeleccionada = "No pagada";
        private string _verificacionSeleccionada = "No tiene";
        private bool _duplicadoLlave;
        private bool _carnetServicios;
        private string _equipoAdicional = string.Empty;
        private string _marcaLlantasDelanteras = string.Empty;
        private string _vidaUtilDelanteras = "100%";
        private string _marcaLlantasTraseras = string.Empty;
        private string _vidaUtilTraseras = "100%";
        private bool _mismaMarcaLlantas;
        private AvaluoDto? _avaluo;


        public int CantidadPuertas { get => _cantidadPuertas; set { _cantidadPuertas = value; OnPropertyChanged(); } }
        public string Vestiduras { get => _vestiduras; set { _vestiduras = value; OnPropertyChanged(); ErrorMessage = string.Empty; } }
        public string Motor { get => _motor; set { _motor = value; OnPropertyChanged(); ErrorMessage = string.Empty; } }
        public int CantidadCilindros { get => _cantidadCilindros; set { _cantidadCilindros = value; OnPropertyChanged(); } }
        public bool FacturaOriginal { get => _facturaOriginal; set { _facturaOriginal = value; OnPropertyChanged(); } }
        public int NumeroDuenos { get => _numeroDuenos; set { _numeroDuenos = value; OnPropertyChanged(); } }
        public int Refacturaciones { get => _refacturaciones; set { _refacturaciones = value; OnPropertyChanged(); } }
        public bool Turbo { get => _turbo; set { _turbo = value; OnPropertyChanged(); } }
        public bool Traccion4x4 { get => _traccion4x4; set { _traccion4x4 = value; OnPropertyChanged(); } }
        public bool Bluetooth { get => _bluetooth; set { _bluetooth = value; OnPropertyChanged(); } }
        public bool USB { get => _usb; set { _usb = value; OnPropertyChanged(); } }
        public bool Pantalla { get => _pantalla; set { _pantalla = value; OnPropertyChanged(); } }
        public bool GPS { get => _gps; set { _gps = value; OnPropertyChanged(); } }
        public bool ACC { get => _acc; set { _acc = value; OnPropertyChanged(); } }
        public bool Quemacocos { get => _quemacocos; set { _quemacocos = value; OnPropertyChanged(); } }
        public bool EspejosElectricos { get => _espejosElectricos; set { _espejosElectricos = value; OnPropertyChanged(); } }
        public bool SegurosElectricos { get => _segurosElectricos; set { _segurosElectricos = value; OnPropertyChanged(); } }
        public bool CristalesElectricos { get => _cristalesElectricos; set { _cristalesElectricos = value; OnPropertyChanged(); } }
        public bool AsientosElectricos { get => _asientosElectricos; set { _asientosElectricos = value; OnPropertyChanged(); } }
        public bool FarosNiebla { get => _farosNiebla; set { _farosNiebla = value; OnPropertyChanged(); } }
        public bool RinesAluminio { get => _rinesAluminio; set { _rinesAluminio = value; OnPropertyChanged(); } }
        public bool ControlesVolante { get => _controlesVolante; set { _controlesVolante = value; OnPropertyChanged(); } }
        public bool EstereoCD { get => _estereoCD; set { _estereoCD = value; OnPropertyChanged(); } }
        public bool ABS { get => _abs; set { _abs = value; OnPropertyChanged(); } }
        public bool DireccionAsistida { get => _direccionAsistida; set { _direccionAsistida = value; OnPropertyChanged(); } }
        public bool BolsasAire { get => _bolsasAire; set { _bolsasAire = value; OnPropertyChanged(); } }
        public bool DuplicadoLlave { get => _duplicadoLlave; set { _duplicadoLlave = value; OnPropertyChanged(); } }
        public bool CarnetServicios { get => _carnetServicios; set { _carnetServicios = value; OnPropertyChanged(); } }
        public string EquipoAdicional { get => _equipoAdicional; set { _equipoAdicional = value; OnPropertyChanged(); } }
        public int AvaluoId {get => _avaluoId; set { _avaluoId = value; OnPropertyChanged(); }}
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

        public AvaluoDto? Avaluo
        {
            get => _avaluo;
            set
            {
                _avaluo = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(VehiculoCompleto));
                OnPropertyChanged(nameof(InfoCliente));
                OnPropertyChanged(nameof(InfoVin));
                OnPropertyChanged(nameof(InfoKilometraje));
            }
        }
        private string MimmaMarcaTrasera() =>
    MismaMarcaLlantas ? MarcaLlantasDelanteras : MarcaLlantasTraseras;


        public string VehiculoCompleto => _avaluo != null
            ? $"{_avaluo.Marca} {_avaluo.Modelo} {_avaluo.Version} {_avaluo.Anio}"
            : "Cargando...";

        public string InfoCliente => _avaluo != null
            ? $"👤 {_avaluo.NombreCompleto}  ·  📞 {_avaluo.Telefono1}"
            : string.Empty;

        public string InfoVin => _avaluo != null
            ? $"VIN: {_avaluo.VIN}  ·  Placas: {_avaluo.Placas}"
            : string.Empty;

        public string InfoKilometraje => _avaluo != null
            ? $"🛣 {_avaluo.KilometrajeFormateado}  ·  {_avaluo.Color}"
            : string.Empty;


        public bool TransmisionAutomatica
        {
            get => _transmisionAutomatica;
            set
            {
                _transmisionAutomatica = value;
                OnPropertyChanged();
                if (value) { _transmisionManual = false; OnPropertyChanged(nameof(TransmisionManual)); }
            }
        }
        public bool TransmisionManual
        {
            get => _transmisionManual;
            set
            {
                _transmisionManual = value;
                OnPropertyChanged();
                if (value) { _transmisionAutomatica = false; OnPropertyChanged(nameof(TransmisionAutomatica)); }
            }
        }

        public string TenenciaSeleccionada
        {
            get => _tenenciaSeleccionada;
            set
            {
                _tenenciaSeleccionada = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(UltimaTenenciaPagada));
            }
        }

        public string VerificacionSeleccionada
        {
            get => _verificacionSeleccionada;
            set
            {
                _verificacionSeleccionada = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Verificacion));
            }
        }

        /// <summary>Convierte el string seleccionado al short? que espera la API.</summary>
        public short? UltimaTenenciaPagada =>
            short.TryParse(_tenenciaSeleccionada, out short t) ? t : (short?)null;

        /// <summary>Convierte el string seleccionado al short? que espera la API.</summary>
        public short? Verificacion =>
            short.TryParse(_verificacionSeleccionada, out short v) ? v : (short?)null;

        public string MarcaLlantasDelanteras
        {
            get => _marcaLlantasDelanteras;
            set
            {
                _marcaLlantasDelanteras = value;
                OnPropertyChanged();
                ErrorMessage = string.Empty;
                // Si están sincronizadas, copiar al trasero también
                if (_mismaMarcaLlantas)
                {
                    _marcaLlantasTraseras = value;
                    OnPropertyChanged(nameof(MarcaLlantasTraseras));
                }
            }
        }
        public string VidaUtilDelanteras
        {
            get => _vidaUtilDelanteras;
            set
            {
                _vidaUtilDelanteras = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(VidaUtilLlantasDelanteras));
            }
        }
        public string MarcaLlantasTraseras
        {
            get => _marcaLlantasTraseras;
            set { _marcaLlantasTraseras = value; OnPropertyChanged(); ErrorMessage = string.Empty; }
        }
        public string VidaUtilTraseras
        {
            get => _vidaUtilTraseras;
            set
            {
                _vidaUtilTraseras = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(VidaUtilLlantasTraseras));
            }
        }

        /// <summary>Copiar marca y vida útil delantera a trasera con un checkbox.</summary>
        public bool MismaMarcaLlantas
        {
            get => _mismaMarcaLlantas;
            set
            {
                _mismaMarcaLlantas = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MostrarLlantasTraseras));
                if (value)
                {
                    MarcaLlantasTraseras = MarcaLlantasDelanteras;
                    VidaUtilTraseras = VidaUtilDelanteras;
                }
            }
        }

        public bool MostrarLlantasTraseras => !MismaMarcaLlantas;

        /// <summary>Convierte "80%" → 80 para la API.</summary>
        public byte? VidaUtilLlantasDelanteras =>
            byte.TryParse(_vidaUtilDelanteras.TrimEnd('%'), out byte d) ? d : (byte?)null;

        /// <summary>Convierte "80%" → 80 para la API.</summary>
        public byte? VidaUtilLlantasTraseras =>
            byte.TryParse(_vidaUtilTraseras.TrimEnd('%'), out byte t) ? t : (byte?)null;

        private bool Validar()
        {
            if (!TransmisionAutomatica && !TransmisionManual)
            { ErrorMessage = "Selecciona el tipo de transmisión"; return false; }
            if (string.IsNullOrWhiteSpace(Vestiduras))
            { ErrorMessage = "Selecciona el tipo de vestiduras"; return false; }
            if (string.IsNullOrWhiteSpace(Motor))
            { ErrorMessage = "Selecciona el motor"; return false; }
            if (string.IsNullOrWhiteSpace(MarcaLlantasDelanteras))
            { ErrorMessage = "Selecciona la marca de llantas delanteras"; return false; }
            if (!MismaMarcaLlantas && string.IsNullOrWhiteSpace(MarcaLlantasTraseras))
            { ErrorMessage = "Selecciona la marca de llantas traseras"; return false; }
            return true;
        }

        private async Task GuardarEquipamientoAsync()
        {
            if (!Validar()) return;

            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                int asesorId = Preferences.Get("UserId", 2);

                var request = new CrearEquipamientoRequest
                {
                    AvaluoId = AvaluoId,
                    AsesorId = asesorId,
                    ACC = ACC,
                    Quemacocos = Quemacocos,
                    EspejosElectricos = EspejosElectricos,
                    SegurosElectricos = SegurosElectricos,
                    CristalesElectricos = CristalesElectricos,
                    AsientosElectricos = AsientosElectricos,
                    FarosNiebla = FarosNiebla,
                    RinesAluminio = RinesAluminio,
                    ControlesVolante = ControlesVolante,
                    EstereoCD = EstereoCD,
                    ABS = ABS,
                    DireccionAsistida = DireccionAsistida,
                    BolsasAire = BolsasAire,
                    TransmisionAutomatica = TransmisionAutomatica,
                    TransmisionManual = TransmisionManual,
                    Turbo = Turbo,
                    Traccion4x4 = Traccion4x4,
                    Bluetooth = Bluetooth,
                    USB = USB,
                    Pantalla = Pantalla,
                    GPS = GPS,
                    CantidadPuertas = (byte)CantidadPuertas,
                    Vestiduras = Vestiduras,
                    Motor = Motor,
                    CantidadCilindros = (byte)CantidadCilindros,
                    FacturaOriginal = FacturaOriginal,
                    NumeroDuenos = (byte)NumeroDuenos,
                    Refacturaciones = (byte)Refacturaciones,
                    UltimaTenenciaPagada = UltimaTenenciaPagada,
                    Verificacion = Verificacion,
                    DuplicadoLlave = DuplicadoLlave,
                    CarnetServicios = CarnetServicios,
                    EquipoAdicional = string.IsNullOrWhiteSpace(EquipoAdicional)
                                           ? null : EquipoAdicional.Trim(),
                    MarcaLlantasDelanteras = MarcaLlantasDelanteras,
                    VidaUtilLlantasDelanteras = VidaUtilLlantasDelanteras,
                    MarcaLlantasTraseras = MimmaMarcaTrasera(),
                    VidaUtilLlantasTraseras = VidaUtilLlantasTraseras
                };

                var response = await _apiService.CrearEquipamientoAvaluoAsync(request);

                if (response.Success)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "✅ Equipamiento Guardado",
                        $"El equipamiento del avalúo #{AvaluoId} se registró correctamente.",
                        "OK");
                    //await Shell.Current.GoToAsync("..");

                    // Preguntar si desea tomar evidencias
                    bool Fotos = await Application.Current.MainPage.DisplayAlert(
                        " Fotos del avaluo ",
                        "¿Deseas registrar las fotos de la unidad ahora?",
                        "Sí",
                        "No");

                    if (Fotos)
                    {
                        await Application.Current.MainPage.Navigation.PopToRootAsync();

                        var evidenciasPage = new FotosAvaluoPage(AvaluoId);
                        await Application.Current.MainPage.Navigation.PushAsync(evidenciasPage);
                    }
                    else
                    {
                        // 🔹 Solo regresar al inicio
                        await Application.Current.MainPage.Navigation.PopToRootAsync();
                    }

                }
                else
                {
                    ErrorMessage = response.Message;
                    await Application.Current.MainPage.DisplayAlert("❌ Error", response.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"❌ GuardarEquipamiento: {ex.Message}");
            }
            finally { IsLoading = false; }
        }


        private async Task CargarAvaluoAsync()
        {
            IsLoading = true;
            try
            {
                var response = await _apiService.ObtenerAvaluoCompletoAsync(AvaluoId);
                if (response.Success && response.Avaluo != null)
                    Avaluo = response.Avaluo;
                else
                    ErrorMessage = response.Message ?? "No se pudo cargar el avalúo";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
            finally { IsLoading = false; }
        }


        #region ── INotifyPropertyChanged ────────────────────────────────────────────────
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion




    }
}