using CarslineApp.Models;
using CarslineApp.Services;
using CarslineApp.Views.Avaluo;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CarslineApp.ViewModels
{
    public class CrearAvaluoViewModel : INotifyPropertyChanged
    {
        #region ── Servicios ──────────────────────────────────────────────────────────────
        private readonly ApiService _apiService;
        private readonly VinDecoderService _vinDecoder = new();
        #endregion

        #region ── Catálogos estáticos ────────────────────────────────────────────────────
        private static readonly List<string> _todasLasMarcas =
            VehiculosCatalogo.ObtenerMarcas();

        private static readonly List<string> _todosLosAnios =
            Enumerable.Range(2000, DateTime.Now.Year - 2000 + 1)
                      .Reverse().Select(y => y.ToString()).ToList();

        public static readonly List<string> TiposCliente =
            new() { "Primera", "Socio Comercial", "Repetitivo" };
        #endregion

        #region ── INotifyPropertyChanged ────────────────────────────────────────────────
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        #endregion

        // ══════════════════════════════════════════════════════════════════════════════════
        #region ── Estado general / Pasos ────────────────────────────────────────────────
        // ══════════════════════════════════════════════════════════════════════════════════

        private bool _isLoading;
        private string _errorMessage = string.Empty;
        private int _pasoActual = 1;

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

        public int PasoActual
        {
            get => _pasoActual;
            set
            {
                _pasoActual = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TituloPaso));
                OnPropertyChanged(nameof(MostrarPaso1));
                OnPropertyChanged(nameof(MostrarPaso2));
                OnPropertyChanged(nameof(MostrarBotonSiguiente));
                OnPropertyChanged(nameof(MostrarBotonCrear));
                OnPropertyChanged(nameof(MostrarBotonAnterior));
            }
        }

        public string TituloPaso => PasoActual switch
        {
            1 => "📋 Datos del Cliente",
            2 => "🚗 Datos del Vehículo",
            _ => "Crear Avalúo"
        };

        public bool MostrarPaso1 => PasoActual == 1;
        public bool MostrarPaso2 => PasoActual == 2;
        public bool MostrarBotonAnterior => PasoActual > 1;
        public bool MostrarBotonSiguiente => PasoActual == 1;
        public bool MostrarBotonCrear => PasoActual == 2;
        public bool MostrarDecodificador = false;

        #endregion

        // ══════════════════════════════════════════════════════════════════════════════════
        #region ── PASO 1: Campos del Cliente ────────────────────────────────────────────
        // ══════════════════════════════════════════════════════════════════════════════════

        private string _nombreCompleto = string.Empty;
        private string _tipoCliente = "Primera";
        private string _telefono1 = string.Empty;
        private string _telefono2 = string.Empty;
        private decimal _precioSolicitado;

        public string NombreCompleto
        {
            get => _nombreCompleto;
            set { _nombreCompleto = value; OnPropertyChanged(); ErrorMessage = string.Empty; }
        }
        public string TipoCliente
        {
            get => _tipoCliente;
            set { _tipoCliente = value; OnPropertyChanged(); }
        }
        public string Telefono1
        {
            get => _telefono1;
            set { _telefono1 = value; OnPropertyChanged(); ErrorMessage = string.Empty; }
        }
        public string Telefono2
        {
            get => _telefono2;
            set { _telefono2 = value; OnPropertyChanged(); }
        }
        public decimal PrecioSolicitado
        {
            get => _precioSolicitado;
            set { _precioSolicitado = value; OnPropertyChanged(); }
        }

        // ── Vehículo a cuenta ─────────────────────────────────────────────────────────────
        // Si es false → CuentaDeVehiculo = "No Aplica" y se oculta el panel de selección.
        // Si es true  → el usuario elige Marca/Modelo/Versión/Año del vehículo que desea
        //               y CuentaDeVehiculo se construye como "Marca Modelo Versión Año".

        private bool _vehiculoACuenta;

        public bool VehiculoACuenta
        {
            get => _vehiculoACuenta;
            set
            {
                _vehiculoACuenta = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MostrarPanelVehiculoCuenta));
                OnPropertyChanged(nameof(CuentaDeVehiculo));

                if (!value)
                {
                    // Limpiar selección al desactivar
                    CuentaMarca = string.Empty; BusquedaCuentaMarca = string.Empty;
                    CuentaModelo = string.Empty; BusquedaCuentaModelo = string.Empty;
                    CuentaVersion = string.Empty; BusquedaCuentaVersion = string.Empty;
                    CuentaAnio = 0; BusquedaCuentaAnio = string.Empty;
                    MostrarSugerenciasCuentaMarca = false;
                    MostrarSugerenciasCuentaModelo = false;
                    MostrarSugerenciasCuentaVersion = false;
                    MostrarSugerenciasCuentaAnio = false;
                }
            }
        }

        public bool MostrarPanelVehiculoCuenta => VehiculoACuenta;

        /// <summary>
        /// Valor final que se envía a la API.
        /// "No Aplica" si VehiculoACuenta es false, o la cadena construida si es true.
        /// </summary>
        public string CuentaDeVehiculo
        {
            get
            {
                if (!VehiculoACuenta) return "No Aplica";
                var partes = new List<string>();
                if (!string.IsNullOrWhiteSpace(CuentaMarca)) partes.Add(CuentaMarca);
                if (!string.IsNullOrWhiteSpace(CuentaModelo)) partes.Add(CuentaModelo);
                if (!string.IsNullOrWhiteSpace(CuentaVersion)) partes.Add(CuentaVersion);
                if (CuentaAnio > 0) partes.Add(CuentaAnio.ToString());
                return partes.Any() ? string.Join(" ", partes) : string.Empty;
            }
        }

        // ── Campos internos del vehículo a cuenta ─────────────────────────────────────────

        private string _cuentaMarca = string.Empty;
        private string _cuentaModelo = string.Empty;
        private string _cuentaVersion = string.Empty;
        private int _cuentaAnio;

        public string CuentaMarca
        {
            get => _cuentaMarca;
            set { _cuentaMarca = value; OnPropertyChanged(); OnPropertyChanged(nameof(CuentaDeVehiculo)); }
        }
        public string CuentaModelo
        {
            get => _cuentaModelo;
            set { _cuentaModelo = value; OnPropertyChanged(); OnPropertyChanged(nameof(CuentaDeVehiculo)); }
        }
        public string CuentaVersion
        {
            get => _cuentaVersion;
            set { _cuentaVersion = value; OnPropertyChanged(); OnPropertyChanged(nameof(CuentaDeVehiculo)); }
        }
        public int CuentaAnio
        {
            get => _cuentaAnio;
            set { _cuentaAnio = value; OnPropertyChanged(); OnPropertyChanged(nameof(CuentaDeVehiculo)); }
        }

        // ── Autocompletado Marca (cuenta) ─────────────────────────────────────────────────

        private string _busquedaCuentaMarca = string.Empty;
        private ObservableCollection<string> _sugerenciasCuentaMarca = new();
        private bool _mostrarSugerenciasCuentaMarca;

        public string BusquedaCuentaMarca
        {
            get => _busquedaCuentaMarca;
            set
            {
                _busquedaCuentaMarca = value;
                OnPropertyChanged();
                CuentaMarca = value?.Trim() ?? string.Empty;
                FiltrarCuentaMarcas();
            }
        }
        public ObservableCollection<string> SugerenciasCuentaMarca
        {
            get => _sugerenciasCuentaMarca;
            set { _sugerenciasCuentaMarca = value; OnPropertyChanged(); }
        }
        public bool MostrarSugerenciasCuentaMarca
        {
            get => _mostrarSugerenciasCuentaMarca;
            set { _mostrarSugerenciasCuentaMarca = value; OnPropertyChanged(); }
        }

        private void FiltrarCuentaMarcas()
        {
            var txt = BusquedaCuentaMarca?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(txt))
            {
                SugerenciasCuentaMarca = new ObservableCollection<string>();
                MostrarSugerenciasCuentaMarca = false;
                return;
            }
            var lista = _todasLasMarcas
                .Where(m => m.StartsWith(txt, StringComparison.OrdinalIgnoreCase))
                .Concat(_todasLasMarcas.Where(m =>
                    !m.StartsWith(txt, StringComparison.OrdinalIgnoreCase) &&
                     m.Contains(txt, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            SugerenciasCuentaMarca = new ObservableCollection<string>(lista);
            MostrarSugerenciasCuentaMarca = SugerenciasCuentaMarca.Count > 0;
        }

        public void SeleccionarCuentaMarca(string marca)
        {
            CuentaMarca = marca; BusquedaCuentaMarca = marca;
            MostrarSugerenciasCuentaMarca = false;

            CuentaModelo = string.Empty; BusquedaCuentaModelo = string.Empty;
            CuentaVersion = string.Empty; BusquedaCuentaVersion = string.Empty;
            CuentaAnio = 0; BusquedaCuentaAnio = string.Empty;
            FiltrarCuentaModelos();
            MostrarSugerenciasCuentaModelo = SugerenciasCuentaModelo.Count > 0;
            MostrarSugerenciasCuentaVersion = false;
            MostrarSugerenciasCuentaAnio = false;
        }

        // ── Autocompletado Modelo (cuenta) ────────────────────────────────────────────────

        private string _busquedaCuentaModelo = string.Empty;
        private ObservableCollection<string> _sugerenciasCuentaModelo = new();
        private bool _mostrarSugerenciasCuentaModelo;

        public string BusquedaCuentaModelo
        {
            get => _busquedaCuentaModelo;
            set
            {
                _busquedaCuentaModelo = value;
                OnPropertyChanged();
                CuentaModelo = value?.Trim() ?? string.Empty;
                FiltrarCuentaModelos();
            }
        }
        public ObservableCollection<string> SugerenciasCuentaModelo
        {
            get => _sugerenciasCuentaModelo;
            set { _sugerenciasCuentaModelo = value; OnPropertyChanged(); }
        }
        public bool MostrarSugerenciasCuentaModelo
        {
            get => _mostrarSugerenciasCuentaModelo;
            set { _mostrarSugerenciasCuentaModelo = value; OnPropertyChanged(); }
        }

        private void FiltrarCuentaModelos()
        {
            var modelos = VehiculosCatalogo.ObtenerModelos(CuentaMarca);
            if (!modelos.Any())
            {
                SugerenciasCuentaModelo = new ObservableCollection<string>();
                MostrarSugerenciasCuentaModelo = false;
                return;
            }
            var txt = BusquedaCuentaModelo?.Trim() ?? string.Empty;
            var lista = string.IsNullOrWhiteSpace(txt)
                ? modelos
                : modelos.Where(m => m.StartsWith(txt, StringComparison.OrdinalIgnoreCase))
                    .Concat(modelos.Where(m =>
                        !m.StartsWith(txt, StringComparison.OrdinalIgnoreCase) &&
                         m.Contains(txt, StringComparison.OrdinalIgnoreCase))).ToList();
            SugerenciasCuentaModelo = new ObservableCollection<string>(lista);
            MostrarSugerenciasCuentaModelo = SugerenciasCuentaModelo.Count > 0;
        }

        public void SeleccionarCuentaModelo(string modelo)
        {
            CuentaModelo = modelo; BusquedaCuentaModelo = modelo;
            MostrarSugerenciasCuentaModelo = false;

            CuentaVersion = string.Empty; BusquedaCuentaVersion = string.Empty;
            CuentaAnio = 0; BusquedaCuentaAnio = string.Empty;
            FiltrarCuentaVersiones();
            MostrarSugerenciasCuentaVersion = SugerenciasCuentaVersion.Count > 0;
            MostrarSugerenciasCuentaAnio = false;
        }

        // ── Autocompletado Versión (cuenta) ───────────────────────────────────────────────

        private string _busquedaCuentaVersion = string.Empty;
        private ObservableCollection<string> _sugerenciasCuentaVersion = new();
        private bool _mostrarSugerenciasCuentaVersion;

        public string BusquedaCuentaVersion
        {
            get => _busquedaCuentaVersion;
            set
            {
                _busquedaCuentaVersion = value;
                OnPropertyChanged();
                CuentaVersion = value?.Trim() ?? string.Empty;
                FiltrarCuentaVersiones();
            }
        }
        public ObservableCollection<string> SugerenciasCuentaVersion
        {
            get => _sugerenciasCuentaVersion;
            set { _sugerenciasCuentaVersion = value; OnPropertyChanged(); }
        }
        public bool MostrarSugerenciasCuentaVersion
        {
            get => _mostrarSugerenciasCuentaVersion;
            set { _mostrarSugerenciasCuentaVersion = value; OnPropertyChanged(); }
        }

        private void FiltrarCuentaVersiones()
        {
            var versiones = VehiculosCatalogo.ObtenerVersiones(CuentaMarca, CuentaModelo);
            if (!versiones.Any())
            {
                SugerenciasCuentaVersion = new ObservableCollection<string>();
                MostrarSugerenciasCuentaVersion = false;
                return;
            }
            var txt = BusquedaCuentaVersion?.Trim() ?? string.Empty;
            var lista = string.IsNullOrWhiteSpace(txt)
                ? versiones
                : versiones.Where(v => v.StartsWith(txt, StringComparison.OrdinalIgnoreCase))
                    .Concat(versiones.Where(v =>
                        !v.StartsWith(txt, StringComparison.OrdinalIgnoreCase) &&
                         v.Contains(txt, StringComparison.OrdinalIgnoreCase))).ToList();
            SugerenciasCuentaVersion = new ObservableCollection<string>(lista);
            MostrarSugerenciasCuentaVersion = SugerenciasCuentaVersion.Count > 0;
        }

        public void SeleccionarCuentaVersion(string version)
        {
            CuentaVersion = version; BusquedaCuentaVersion = version;
            MostrarSugerenciasCuentaVersion = false;

            BusquedaCuentaAnio = string.Empty;
            FiltrarCuentaAnios();
            MostrarSugerenciasCuentaAnio = SugerenciasCuentaAnio.Count > 0;
        }

        // ── Autocompletado Año (cuenta) ───────────────────────────────────────────────────

        private string _busquedaCuentaAnio = string.Empty;
        private ObservableCollection<string> _sugerenciasCuentaAnio = new();
        private bool _mostrarSugerenciasCuentaAnio;

        public string BusquedaCuentaAnio
        {
            get => _busquedaCuentaAnio;
            set { _busquedaCuentaAnio = value; OnPropertyChanged(); FiltrarCuentaAnios(); }
        }
        public ObservableCollection<string> SugerenciasCuentaAnio
        {
            get => _sugerenciasCuentaAnio;
            set { _sugerenciasCuentaAnio = value; OnPropertyChanged(); }
        }
        public bool MostrarSugerenciasCuentaAnio
        {
            get => _mostrarSugerenciasCuentaAnio;
            set { _mostrarSugerenciasCuentaAnio = value; OnPropertyChanged(); }
        }

        private void FiltrarCuentaAnios()
        {
            var txt = BusquedaCuentaAnio?.Trim() ?? string.Empty;
            SugerenciasCuentaAnio = string.IsNullOrWhiteSpace(txt)
                ? new ObservableCollection<string>(_todosLosAnios)
                : new ObservableCollection<string>(_todosLosAnios.Where(a => a.StartsWith(txt)));
            MostrarSugerenciasCuentaAnio = SugerenciasCuentaAnio.Count > 0;
        }

        public void SeleccionarCuentaAnio(string anio)
        {
            if (int.TryParse(anio, out int v)) CuentaAnio = v;
            BusquedaCuentaAnio = anio;
            MostrarSugerenciasCuentaAnio = false;
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════════════════
        #region ── PASO 2: Campos del Vehículo ───────────────────────────────────────────
        // ══════════════════════════════════════════════════════════════════════════════════

        private string _vin = string.Empty;
        private string _marca = string.Empty;
        private string _modelo = string.Empty;
        private string _version = string.Empty;
        private int _anio = DateTime.Now.Year;
        private string _color = string.Empty;
        private string _placas = "S/P";
        private int _kilometraje;

        public string VIN
        {
            get => _vin;
            set { _vin = value?.ToUpper() ?? string.Empty; OnPropertyChanged(); ErrorMessage = string.Empty; }
        }
        public string Marca
        {
            get => _marca;
            set { _marca = value; OnPropertyChanged(); ErrorMessage = string.Empty; }
        }
        public string Modelo
        {
            get => _modelo;
            set { _modelo = value; OnPropertyChanged(); ErrorMessage = string.Empty; }
        }
        public string Version
        {
            get => _version;
            set { _version = value; OnPropertyChanged(); ErrorMessage = string.Empty; }
        }
        public int Anio
        {
            get => _anio;
            set { _anio = value; OnPropertyChanged(); }
        }
        public string Color
        {
            get => _color;
            set { _color = value; OnPropertyChanged(); }
        }
        public string Placas
        {
            get => _placas;
            set { _placas = value?.ToUpper() ?? string.Empty; OnPropertyChanged(); }
        }
        public int Kilometraje
        {
            get => _kilometraje;
            set { _kilometraje = value; OnPropertyChanged(); }
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════════════════
        #region ── Panel de estado VIN ───────────────────────────────────────────────────
        // ══════════════════════════════════════════════════════════════════════════════════

        private bool _vinDecodificando;
        private bool _vinDecodificadoExito;
        private string _vinMensajeDecodificacion = string.Empty;

        public bool VinDecodificando
        {
            get => _vinDecodificando;
            set { _vinDecodificando = value; OnPropertyChanged(); OnPropertyChanged(nameof(MostrarEstadoVin)); }
        }
        public bool VinDecodificadoExito
        {
            get => _vinDecodificadoExito;
            set
            {
                _vinDecodificadoExito = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(MostrarEstadoVin));
                OnPropertyChanged(nameof(ColorEstadoVin));
                OnPropertyChanged(nameof(ColorBordeEstadoVin));
                OnPropertyChanged(nameof(ColorTextoEstadoVin));
            }
        }
        public string VinMensajeDecodificacion
        {
            get => _vinMensajeDecodificacion;
            set { _vinMensajeDecodificacion = value; OnPropertyChanged(); OnPropertyChanged(nameof(MostrarEstadoVin)); }
        }

        public bool MostrarEstadoVin => VinDecodificando || !string.IsNullOrEmpty(VinMensajeDecodificacion);
        public string ColorEstadoVin => VinDecodificadoExito ? "#E8F5E9" : "#FFF8E1";
        public string ColorBordeEstadoVin => VinDecodificadoExito ? "#4CAF50" : "#FF9800";
        public string ColorTextoEstadoVin => VinDecodificadoExito ? "#2E7D32" : "#E65100";

        #endregion

        // ══════════════════════════════════════════════════════════════════════════════════
        #region ── Autocompletado — Marca ────────────────────────────────────────────────
        // ══════════════════════════════════════════════════════════════════════════════════

        private string _busquedaMarca = string.Empty;
        private ObservableCollection<string> _marcasFiltradas = new();
        private bool _mostrarSugerenciasMarca;

        public string BusquedaMarca
        {
            get => _busquedaMarca;
            set
            {
                _busquedaMarca = value;
                OnPropertyChanged();
                Marca = value?.Trim() ?? string.Empty;
                FiltrarMarcas();
            }
        }
        public ObservableCollection<string> MarcasFiltradas
        {
            get => _marcasFiltradas;
            set { _marcasFiltradas = value; OnPropertyChanged(); }
        }
        public bool MostrarSugerenciasMarca
        {
            get => _mostrarSugerenciasMarca;
            set { _mostrarSugerenciasMarca = value; OnPropertyChanged(); }
        }

        private void FiltrarMarcas()
        {
            var txt = BusquedaMarca?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(txt))
            {
                MarcasFiltradas = new ObservableCollection<string>();
                MostrarSugerenciasMarca = false;
                return;
            }
            var lista = _todasLasMarcas
                .Where(m => m.StartsWith(txt, StringComparison.OrdinalIgnoreCase))
                .Concat(_todasLasMarcas.Where(m =>
                    !m.StartsWith(txt, StringComparison.OrdinalIgnoreCase) &&
                     m.Contains(txt, StringComparison.OrdinalIgnoreCase)))
                .ToList();
            MarcasFiltradas = new ObservableCollection<string>(lista);
            MostrarSugerenciasMarca = MarcasFiltradas.Count > 0;
        }

        /// <summary>Marca seleccionada → limpia modelo y versión → abre lista de modelos.</summary>
        public void SeleccionarMarca(string marca)
        {
            Marca = marca; BusquedaMarca = marca;
            MostrarSugerenciasMarca = false;

            Modelo = string.Empty; BusquedaModelo = string.Empty;
            FiltrarModelos();
            MostrarSugerenciasModelo = ModelosFiltrados.Count > 0;

            Version = string.Empty; BusquedaVersion = string.Empty;
            MostrarSugerenciasVersion = false;
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════════════════
        #region ── Autocompletado — Modelo ───────────────────────────────────────────────
        // ══════════════════════════════════════════════════════════════════════════════════

        private string _busquedaModelo = string.Empty;
        private ObservableCollection<string> _modelosFiltrados = new();
        private bool _mostrarSugerenciasModelo;

        public string BusquedaModelo
        {
            get => _busquedaModelo;
            set
            {
                _busquedaModelo = value;
                OnPropertyChanged();
                Modelo = value?.Trim() ?? string.Empty;
                FiltrarModelos();
            }
        }
        public ObservableCollection<string> ModelosFiltrados
        {
            get => _modelosFiltrados;
            set { _modelosFiltrados = value; OnPropertyChanged(); }
        }
        public bool MostrarSugerenciasModelo
        {
            get => _mostrarSugerenciasModelo;
            set { _mostrarSugerenciasModelo = value; OnPropertyChanged(); }
        }

        private void FiltrarModelos()
        {
            var modelos = VehiculosCatalogo.ObtenerModelos(Marca);
            if (!modelos.Any())
            {
                ModelosFiltrados = new ObservableCollection<string>();
                MostrarSugerenciasModelo = false;
                return;
            }
            var txt = BusquedaModelo?.Trim() ?? string.Empty;
            var lista = string.IsNullOrWhiteSpace(txt)
                ? modelos
                : modelos.Where(m => m.StartsWith(txt, StringComparison.OrdinalIgnoreCase))
                    .Concat(modelos.Where(m =>
                        !m.StartsWith(txt, StringComparison.OrdinalIgnoreCase) &&
                         m.Contains(txt, StringComparison.OrdinalIgnoreCase))).ToList();
            ModelosFiltrados = new ObservableCollection<string>(lista);
            MostrarSugerenciasModelo = ModelosFiltrados.Count > 0;
        }

        /// <summary>Modelo seleccionado → limpia versión → abre lista de versiones.</summary>
        public void SeleccionarModelo(string modelo)
        {
            Modelo = modelo; BusquedaModelo = modelo;
            MostrarSugerenciasModelo = false;

            Version = string.Empty; BusquedaVersion = string.Empty;
            FiltrarVersiones();
            MostrarSugerenciasVersion = VersionesFiltradas.Count > 0;
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════════════════
        #region ── Autocompletado — Versión ──────────────────────────────────────────────
        // ══════════════════════════════════════════════════════════════════════════════════

        private string _busquedaVersion = string.Empty;
        private ObservableCollection<string> _versionesFiltradas = new();
        private bool _mostrarSugerenciasVersion;

        public string BusquedaVersion
        {
            get => _busquedaVersion;
            set
            {
                _busquedaVersion = value;
                OnPropertyChanged();
                Version = value?.Trim() ?? string.Empty;
                FiltrarVersiones();
            }
        }
        public ObservableCollection<string> VersionesFiltradas
        {
            get => _versionesFiltradas;
            set { _versionesFiltradas = value; OnPropertyChanged(); }
        }
        public bool MostrarSugerenciasVersion
        {
            get => _mostrarSugerenciasVersion;
            set { _mostrarSugerenciasVersion = value; OnPropertyChanged(); }
        }

        private void FiltrarVersiones()
        {
            var versiones = VehiculosCatalogo.ObtenerVersiones(Marca, Modelo);
            if (!versiones.Any())
            {
                VersionesFiltradas = new ObservableCollection<string>();
                MostrarSugerenciasVersion = false;
                return;
            }
            var txt = BusquedaVersion?.Trim() ?? string.Empty;
            var lista = string.IsNullOrWhiteSpace(txt)
                ? versiones
                : versiones.Where(v => v.StartsWith(txt, StringComparison.OrdinalIgnoreCase))
                    .Concat(versiones.Where(v =>
                        !v.StartsWith(txt, StringComparison.OrdinalIgnoreCase) &&
                         v.Contains(txt, StringComparison.OrdinalIgnoreCase))).ToList();
            VersionesFiltradas = new ObservableCollection<string>(lista);
            MostrarSugerenciasVersion = VersionesFiltradas.Count > 0;
        }

        public void SeleccionarVersion(string version)
        {
            Version = version; BusquedaVersion = version;
            MostrarSugerenciasVersion = false;

            BusquedaAnio = string.Empty;
            FiltrarAnios();
            MostrarSugerenciasAnio = AniosFiltrados.Count > 0;
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════════════════
        #region ── Autocompletado — Año ──────────────────────────────────────────────────
        // ══════════════════════════════════════════════════════════════════════════════════

        private string _busquedaAnio = string.Empty;
        private ObservableCollection<string> _aniosFiltrados = new();
        private bool _mostrarSugerenciasAnio;

        public string BusquedaAnio
        {
            get => _busquedaAnio;
            set { _busquedaAnio = value; OnPropertyChanged(); FiltrarAnios(); }
        }
        public ObservableCollection<string> AniosFiltrados
        {
            get => _aniosFiltrados;
            set { _aniosFiltrados = value; OnPropertyChanged(); }
        }
        public bool MostrarSugerenciasAnio
        {
            get => _mostrarSugerenciasAnio;
            set { _mostrarSugerenciasAnio = value; OnPropertyChanged(); }
        }

        private void FiltrarAnios()
        {
            var txt = BusquedaAnio?.Trim() ?? string.Empty;
            AniosFiltrados = string.IsNullOrWhiteSpace(txt)
                ? new ObservableCollection<string>(_todosLosAnios)
                : new ObservableCollection<string>(_todosLosAnios.Where(a => a.StartsWith(txt)));
            MostrarSugerenciasAnio = AniosFiltrados.Count > 0;
        }

        public void SeleccionarAnio(string anio)
        {
            if (int.TryParse(anio, out int v)) Anio = v;
            BusquedaAnio = anio;
            MostrarSugerenciasAnio = false;
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════════════════
        #region ── Decodificación VIN ────────────────────────────────────────────────────
        // ══════════════════════════════════════════════════════════════════════════════════

        private async Task DecodificarVinAsync(string vin)
        {
            VinDecodificando = true;
            VinDecodificadoExito = false;
            VinMensajeDecodificacion = "🔍 Consultando datos del vehículo...";

            if (vin.Length == 17)
            {
                try
                {
                    var resultado = await _vinDecoder.DecodificarVinAsync(vin);

                    if (resultado == null || !resultado.MarcaReconocida)
                    {
                        VinMensajeDecodificacion = "⚠️ VIN no reconocido. Ingresa los datos manualmente.";
                        return;
                    }

                    // ── Marca ──────────────────────────────────────────────────────────
                    var marcaNorm = NormalizarConCatalogo(_todasLasMarcas, resultado.Marca);
                    Marca = marcaNorm; BusquedaMarca = marcaNorm;
                    MostrarSugerenciasMarca = false;

                    // ── Año ────────────────────────────────────────────────────────────
                    if (resultado.AnioReconocido)
                    {
                        Anio = resultado.Anio;
                        BusquedaAnio = resultado.Anio.ToString();
                    }
                    MostrarSugerenciasAnio = false;

                    // ── Modelo ─────────────────────────────────────────────────────────
                    var modelosDisponibles = VehiculosCatalogo.ObtenerModelos(Marca);
                    var modeloNorm = NormalizarConCatalogo(modelosDisponibles, resultado.Modelo);

                    if (!string.IsNullOrEmpty(modeloNorm))
                    {
                        Modelo = modeloNorm; BusquedaModelo = modeloNorm;
                        MostrarSugerenciasModelo = false;
                        Version = string.Empty; BusquedaVersion = string.Empty;
                        FiltrarVersiones();
                        MostrarSugerenciasVersion = VersionesFiltradas.Count > 0;
                    }
                    else
                    {
                        Modelo = string.Empty; BusquedaModelo = string.Empty;
                        FiltrarModelos();
                        MostrarSugerenciasModelo = ModelosFiltrados.Count > 0;
                        Version = string.Empty; BusquedaVersion = string.Empty;
                        MostrarSugerenciasVersion = false;
                    }

                    // ── Mensaje resumen ────────────────────────────────────────────────
                    var extras = new List<string>();
                    if (!string.IsNullOrEmpty(resultado.NumCilindros))
                        extras.Add($"{resultado.NumCilindros} cil.");
                    if (!string.IsNullOrEmpty(resultado.Displacement) &&
                        float.TryParse(resultado.Displacement, out float lt) && lt > 0)
                        extras.Add($"{lt:F1}L");
                    if (!string.IsNullOrEmpty(resultado.TipoCombust))
                        extras.Add(resultado.TipoCombust);

                    var extraTexto = extras.Any() ? $"  ·  {string.Join(", ", extras)}" : string.Empty;
                    var pendiente = string.IsNullOrEmpty(modeloNorm)
                        ? "  —  Selecciona el modelo 👇"
                        : "  —  Selecciona la versión 👇";

                    VinDecodificadoExito = true;
                    VinMensajeDecodificacion = $"✅ {resultado.Anio} {Marca}{extraTexto}{pendiente}";
                }
                catch (Exception ex)
                {
                    VinMensajeDecodificacion = "⚠️ Error al decodificar. Ingresa los datos manualmente.";
                    System.Diagnostics.Debug.WriteLine($"[VIN] {ex.Message}");
                }
                finally { VinDecodificando = false; }
            }
            else
            {
                var faltantes = 17 - vin.Length;
                VinMensajeDecodificacion = $"⚠️ Faltan {faltantes} caracteres para decodificar";
                VinDecodificadoExito = false;
                VinDecodificando = false;
            }
        }

        /// <summary>Busca en catálogo (exacto → parcial → Title Case).</summary>
        private static string NormalizarConCatalogo(List<string> lista, string? texto)
        {
            if (string.IsNullOrEmpty(texto)) return string.Empty;
            var exacto = lista.FirstOrDefault(x => x.Equals(texto, StringComparison.OrdinalIgnoreCase));
            if (exacto != null) return exacto;
            var parcial = lista.FirstOrDefault(x =>
                texto.Contains(x, StringComparison.OrdinalIgnoreCase) ||
                x.Contains(texto, StringComparison.OrdinalIgnoreCase));
            if (parcial != null) return parcial;
            return System.Globalization.CultureInfo.InvariantCulture
                         .TextInfo.ToTitleCase(texto.ToLower());
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════════════════
        #region ── Validaciones ──────────────────────────────────────────────────────────
        // ══════════════════════════════════════════════════════════════════════════════════

        private bool ValidarPaso1()
        {
            if (string.IsNullOrWhiteSpace(NombreCompleto))
            { ErrorMessage = "El nombre del cliente es requerido"; return false; }
            if (NombreCompleto.Trim().Length < 3)
            { ErrorMessage = "El nombre debe tener al menos 3 caracteres"; return false; }
            if (string.IsNullOrWhiteSpace(Telefono1))
            { ErrorMessage = "El teléfono principal es requerido"; return false; }
            if (Telefono1.Trim().Length < 10)
            { ErrorMessage = "El teléfono debe tener 10 dígitos"; return false; }
            return true;
        }

        private bool ValidarPaso2()
        {
            // Sincronizar desde búsqueda libre si el usuario no seleccionó del dropdown
            if (string.IsNullOrWhiteSpace(Marca) && !string.IsNullOrWhiteSpace(BusquedaMarca))
                Marca = BusquedaMarca.Trim();
            if (string.IsNullOrWhiteSpace(Modelo) && !string.IsNullOrWhiteSpace(BusquedaModelo))
                Modelo = BusquedaModelo.Trim();
            if (string.IsNullOrWhiteSpace(Version) && !string.IsNullOrWhiteSpace(BusquedaVersion))
                Version = BusquedaVersion.Trim();
            if (int.TryParse(BusquedaAnio, out int anioTexto) && anioTexto >= 2000)
                Anio = anioTexto;

            if (string.IsNullOrWhiteSpace(VIN) || VIN.Length != 17)
            { ErrorMessage = "El VIN debe tener exactamente 17 caracteres"; return false; }
            if (string.IsNullOrWhiteSpace(Marca))
            { ErrorMessage = "La marca es requerida"; return false; }
            if (string.IsNullOrWhiteSpace(Modelo))
            { ErrorMessage = "El modelo es requerido"; return false; }
            if (string.IsNullOrWhiteSpace(Version))
            { ErrorMessage = "La versión es requerida"; return false; }
            if (Anio < 2000 || Anio > DateTime.Now.Year + 1)
            { ErrorMessage = "El año ingresado no es válido"; return false; }
            if (Kilometraje < 0)
            { ErrorMessage = "El kilometraje no puede ser negativo"; return false; }
            if (PrecioSolicitado <= 0)
            { ErrorMessage = "Ingresa el precio Solicitado"; return false; }
            return true;
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════════════════
        #region ── Guardar Avalúo ────────────────────────────────────────────────────────
        // ══════════════════════════════════════════════════════════════════════════════════

        private async Task CrearAvaluoAsync()
        {
            if (!ValidarPaso2()) return;

            IsLoading = true;
            ErrorMessage = string.Empty;
            try
            {
                int asesorId = Preferences.Get("UserId", 9);
                if (asesorId == 0)
                {
                    ErrorMessage = "Sesión expirada. Vuelve a iniciar sesión.";
                    return;
                }

                var request = new CrearAvaluoRequest
                {
                    AsesorId = asesorId,
                    NombreCompleto = NombreCompleto.Trim(),
                    TipoCliente = TipoCliente,
                    Telefono1 = Telefono1.Trim(),
                    Telefono2 = string.IsNullOrWhiteSpace(Telefono2) ? null : Telefono2.Trim(),
                    Marca = Marca.Trim(),
                    Modelo = Modelo.Trim(),
                    Version = Version.Trim(),
                    Anio = (short)Anio,
                    Color = string.IsNullOrWhiteSpace(Color) ? null : Color.Trim(),
                    VIN = VIN.Trim().ToUpper(),
                    Placas = string.IsNullOrWhiteSpace(Placas) ? "S/P" : Placas.Trim().ToUpper(),
                    Kilometraje = Kilometraje,
                    CuentaDeVehiculo = CuentaDeVehiculo,
                    PrecioSolicitado = PrecioSolicitado
                };

                var response = await _apiService.CrearAvaluoAsync(request);

                if (response.Success)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "✅ Avalúo Creado",
                        $"Se registró el avalúo correctamente.\n\n" +
                        $"Vehículo: {Marca} {Modelo} {Version} {Anio}\n" +
                        $"Cliente: {NombreCompleto}",
                        "OK");

                    // Preguntar si desea tomar evidencias
                    bool Equipamiento = await Application.Current.MainPage.DisplayAlert(
                        " Equipamiento del avaluo ",
                        "¿Deseas registrar el equipamiento de la unidad ahora?",
                        "Sí",
                        "No");

                    if (Equipamiento)
                    {
                        await Application.Current.MainPage.Navigation.PopToRootAsync();

                        var evidenciasPage = new EquipamientoAvaluoPage(response.AvaluoId);
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
                System.Diagnostics.Debug.WriteLine($"❌ CrearAvaluo: {ex.Message}");
            }
            finally { IsLoading = false; }
        }

        #endregion

        // ══════════════════════════════════════════════════════════════════════════════════
        #region ── Comandos ──────────────────────────────────────────────────────────────
        // ══════════════════════════════════════════════════════════════════════════════════

        public ICommand SiguienteCommand { get; }
        public ICommand AnteriorCommand { get; }
        public ICommand CrearAvaluoCommand { get; }
        public ICommand VinCambiadoCommand { get; }
        public ICommand AbrirSugerenciasMarcaCommand { get; }
        public ICommand SeleccionarMarcaCommand { get; }
        public ICommand AbrirSugerenciasModeloCommand { get; }
        public ICommand SeleccionarModeloCommand { get; }
        public ICommand AbrirSugerenciasVersionCommand { get; }
        public ICommand SeleccionarVersionCommand { get; }
        public ICommand AbrirSugerenciasAnioCommand { get; }
        public ICommand SeleccionarAnioCommand { get; }


        // Vehículo a cuenta
        public ICommand AbrirSugerenciasCuentaMarcaCommand { get; }
        public ICommand SeleccionarCuentaMarcaCommand { get; }
        public ICommand AbrirSugerenciasCuentaModeloCommand { get; }
        public ICommand SeleccionarCuentaModeloCommand { get; }
        public ICommand AbrirSugerenciasCuentaVersionCommand { get; }
        public ICommand SeleccionarCuentaVersionCommand { get; }
        public ICommand AbrirSugerenciasCuentaAnioCommand { get; }
        public ICommand SeleccionarCuentaAnioCommand { get; }

        #endregion

        // ══════════════════════════════════════════════════════════════════════════════════
        #region ── Constructor ───────────────────────────────────────────────────────────
        // ══════════════════════════════════════════════════════════════════════════════════

        public CrearAvaluoViewModel(ApiService apiService)
        {
            _apiService = apiService;

            // ── Navegación por pasos ───────────────────────────────────────────────────
            SiguienteCommand = new Command(() =>
            {
                ErrorMessage = string.Empty;
                if (PasoActual == 1 && !ValidarPaso1()) return;
                if (PasoActual < 2) PasoActual++;
            });

            AnteriorCommand = new Command(() =>
            {
                ErrorMessage = string.Empty;
                if (PasoActual > 1) PasoActual--;
            });

            CrearAvaluoCommand = new Command(async () => await CrearAvaluoAsync());

            // ── VIN en tiempo real ─────────────────────────────────────────────────────
            VinCambiadoCommand = new Command<string>(async (vin) =>
            {
                VIN = vin ?? string.Empty;
                if (VIN.Length == 17)
                    await DecodificarVinAsync(VIN);
                else
                {
                    MostrarDecodificador = false;
                    VinDecodificadoExito = false;
                    VinMensajeDecodificacion = VIN.Length > 0
                        ? $"⚠️ Faltan {17 - VIN.Length} caracteres para decodificar"
                        : string.Empty;
                }
            });

            // ── Autocompletado Marca ───────────────────────────────────────────────────
            AbrirSugerenciasMarcaCommand = new Command(() =>
            {
                MarcasFiltradas = new ObservableCollection<string>(_todasLasMarcas);
                MostrarSugerenciasMarca = true;
            });
            SeleccionarMarcaCommand = new Command<string>(SeleccionarMarca);

            // ── Autocompletado Modelo ──────────────────────────────────────────────────
            AbrirSugerenciasModeloCommand = new Command(() =>
            {
                FiltrarModelos();
                if (ModelosFiltrados.Count > 0) MostrarSugerenciasModelo = true;
            });
            SeleccionarModeloCommand = new Command<string>(SeleccionarModelo);

            // ── Autocompletado Versión ─────────────────────────────────────────────────
            AbrirSugerenciasVersionCommand = new Command(() =>
            {
                FiltrarVersiones();
                if (VersionesFiltradas.Count > 0) MostrarSugerenciasVersion = true;
            });
            SeleccionarVersionCommand = new Command<string>(SeleccionarVersion);

            // ── Autocompletado Año ─────────────────────────────────────────────────────
            AbrirSugerenciasAnioCommand = new Command(() =>
            {
                AniosFiltrados = new ObservableCollection<string>(_todosLosAnios);
                MostrarSugerenciasAnio = true;
            });
            SeleccionarAnioCommand = new Command<string>(SeleccionarAnio);

            // ── Vehículo a cuenta — Marca ──────────────────────────────────────────────
            AbrirSugerenciasCuentaMarcaCommand = new Command(() =>
            {
                SugerenciasCuentaMarca = new ObservableCollection<string>(_todasLasMarcas);
                MostrarSugerenciasCuentaMarca = true;
            });
            SeleccionarCuentaMarcaCommand = new Command<string>(SeleccionarCuentaMarca);

            // ── Vehículo a cuenta — Modelo ─────────────────────────────────────────────
            AbrirSugerenciasCuentaModeloCommand = new Command(() =>
            {
                FiltrarCuentaModelos();
                if (SugerenciasCuentaModelo.Count > 0) MostrarSugerenciasCuentaModelo = true;
            });
            SeleccionarCuentaModeloCommand = new Command<string>(SeleccionarCuentaModelo);

            // ── Vehículo a cuenta — Versión ────────────────────────────────────────────
            AbrirSugerenciasCuentaVersionCommand = new Command(() =>
            {
                FiltrarCuentaVersiones();
                if (SugerenciasCuentaVersion.Count > 0) MostrarSugerenciasCuentaVersion = true;
            });
            SeleccionarCuentaVersionCommand = new Command<string>(SeleccionarCuentaVersion);

            // ── Vehículo a cuenta — Año ────────────────────────────────────────────────
            AbrirSugerenciasCuentaAnioCommand = new Command(() =>
            {
                SugerenciasCuentaAnio = new ObservableCollection<string>(_todosLosAnios);
                MostrarSugerenciasCuentaAnio = true;
            });
            SeleccionarCuentaAnioCommand = new Command<string>(SeleccionarCuentaAnio);
        }

        #endregion
    }
}