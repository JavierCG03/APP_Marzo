using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CarslineApp.Models
{
    // ============================================
    // REQUEST DTOs
    // ============================================

    public class CrearAvaluoRequest
    {
        public int AsesorId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string TipoCliente { get; set; } = "Externo";
        public string Telefono1 { get; set; } = string.Empty;
        public string? Telefono2 { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public short Anio { get; set; }
        public string? Color { get; set; }
        public string VIN { get; set; } = string.Empty;
        public string Placas { get; set; } = "S/P";
        public int Kilometraje { get; set; }
        public string CuentaDeVehiculo { get; set; } = "No Aplica";
        public decimal PrecioSolicitado { get; set; } = 0;
    }

    public class CrearEquipamientoRequest
    {
        public int AvaluoId { get; set; }
        public int AsesorId { get; set; }

        // Electrónico
        public bool ACC { get; set; } = false;
        public bool Quemacocos { get; set; } = false;
        public bool EspejosElectricos { get; set; } = false;
        public bool SegurosElectricos { get; set; } = false;
        public bool CristalesElectricos { get; set; } = false;
        public bool AsientosElectricos { get; set; } = false;
        public bool FarosNiebla { get; set; } = false;
        public bool RinesAluminio { get; set; } = false;
        public bool ControlesVolante { get; set; } = false;
        public bool EstereoCD { get; set; } = false;
        public bool ABS { get; set; } = false;
        public bool DireccionAsistida { get; set; } = false;
        public bool BolsasAire { get; set; } = false;
        public bool TransmisionAutomatica { get; set; } = false;
        public bool TransmisionManual { get; set; } = false;
        public bool Turbo { get; set; } = false;
        public bool Traccion4x4 { get; set; } = false;
        public bool Bluetooth { get; set; } = false;
        public bool USB { get; set; } = false;
        public bool Pantalla { get; set; } = false;
        public bool GPS { get; set; } = false;

        // Mecánico
        public byte CantidadPuertas { get; set; } = 4;
        public string Vestiduras { get; set; } = string.Empty;
        public string Motor { get; set; } = string.Empty;
        public byte CantidadCilindros { get; set; } = 4;
        public bool FacturaOriginal { get; set; } = false;
        public byte NumeroDuenos { get; set; } = 1;
        public byte Refacturaciones { get; set; } = 0;
        public short? UltimaTenenciaPagada { get; set; }
        public short? Verificacion { get; set; }
        public bool DuplicadoLlave { get; set; } = false;
        public bool CarnetServicios { get; set; } = false;
        public string? EquipoAdicional { get; set; }
        public string MarcaLlantasDelanteras { get; set; } = string.Empty;
        public byte? VidaUtilLlantasDelanteras { get; set; }
        public string MarcaLlantasTraseras { get; set; } = string.Empty;
        public byte? VidaUtilLlantasTraseras { get; set; }
    }

    public class CrearReparacionesRequest
    {
        public int AvaluoId { get; set; }
        public List<ReparacionItemRequest> Reparaciones { get; set; } = new();
    }

    public class ReparacionItemRequest
    {
        public string Descripcion { get; set; } = string.Empty;
        public decimal CostoAproximado { get; set; }
    }

    public class SubirFotosAvaluoRequest
    {
        public int AvaluoId { get; set; }
        public List<FotoAvaluoUpload> Fotos { get; set; } = new();
    }

    public class FotoAvaluoUpload
    {
        public string TipoFoto { get; set; } = string.Empty;
        public byte[] ImagenBytes { get; set; } = Array.Empty<byte>();
        public string NombreArchivo { get; set; } = string.Empty;
    }

    public class AutorizarAvaluoRequest
    {
        public decimal PrecioAutorizado { get; set; }
        public bool VehiculoApto { get; set; } = true;
    }

    // ============================================
    // RESPONSE DTOs
    // ============================================

    public class AvaluoResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int AvaluoId { get; set; }
        public AvaluoDto? Avaluo { get; set; }
    }

    public class EquipamientoAvaluoResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public EquipamientoAvaluoDto? Equipamiento { get; set; }
    }

    public class ReparacionesAvaluoResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<ReparacionAvaluoDto> Reparaciones { get; set; } = new();
        public decimal TotalCostoReparaciones { get; set; }

        // Propiedades calculadas
        public string TotalFormateado => $"${TotalCostoReparaciones:N2}";
        public bool TieneReparaciones => Reparaciones.Any();
        public int CantidadReparaciones => Reparaciones.Count;
    }

    public class FotosAvaluoResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<FotoAvaluoDto> Fotos { get; set; } = new();
        public int CantidadFotos { get; set; }
    }

    public class AvaluoCompletoResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int AvaluoId { get; set; }
        public AvaluoDto? Avaluo { get; set; }
        public EquipamientoAvaluoDto? Equipamiento { get; set; }
        public List<ReparacionAvaluoDto> Reparaciones { get; set; } = new();
        public List<FotoAvaluoDto> Fotos { get; set; } = new();
        public decimal TotalCostoReparaciones { get; set; }

        // Propiedades calculadas
        public bool TieneEquipamiento => Equipamiento != null;
        public bool TieneReparaciones => Reparaciones.Any();
        public bool TieneFotos => Fotos.Any();
        public string TotalFormateado => $"${TotalCostoReparaciones:N2}";
    }

    public class AvaluosListaResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int Total { get; set; }
        public List<AvaluoDto> Avaluos { get; set; } = new();
    }

    // ============================================
    // DATA TRANSFER OBJECTS
    // ============================================

    public class AvaluoDto
    {
        public int Id { get; set; }
        public int AsesorId { get; set; }
        public string AsesorNombre { get; set; } = string.Empty;

        // Cliente
        public string NombreCompleto { get; set; } = string.Empty;
        public string TipoCliente { get; set; } = string.Empty;
        public string Telefono1 { get; set; } = string.Empty;
        public string? Telefono2 { get; set; }

        // Vehículo
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public short Anio { get; set; }
        public string? Color { get; set; }
        public string VIN { get; set; } = string.Empty;
        public string Placas { get; set; } = string.Empty;
        public int Kilometraje { get; set; }
        public string CuentaDeVehiculo { get; set; } = string.Empty;

        // Precios
        public decimal PrecioSolicitado { get; set; }
        public decimal CostoAproximadoReacondicionamiento { get; set; }
        public decimal PrecioAutorizado { get; set; }

        // Estado
        public DateTime FechaAvaluo { get; set; }
        public bool BajaPlacas { get; set; }
        public bool VehiculoApto { get; set; }
        public bool VehiculoTomadoRevision { get; set; }
        public bool VehiculoComprado { get; set; }
        public bool Activo { get; set; }

        // Propiedades calculadas
        public string VehiculoCompleto => $"{Marca} {Modelo} {Version} {Anio}";
        public string Ultimos4VIN => VIN.Length >= 4 ? VIN[^4..] : VIN;
        public string FechaFormateada => FechaAvaluo.ToString("dd/MMM/yyyy");
        public string PrecioSolicitadoFormateado => $"${PrecioSolicitado:N2}";
        public string PrecioAutorizadoFormateado => $"${PrecioAutorizado:N2}";
        public string CostoReacondicionamientoFormateado => $"${CostoAproximadoReacondicionamiento:N2}";
        public string KilometrajeFormateado => $"{Kilometraje:N0} km";

        // Estado visual
        public string ColorEstado => VehiculoApto ? "#43A047" : "#E53935";
        public string TextoEstado => VehiculoApto ? "Apto" : "No Apto";
        public string ColorFondoEstado => VehiculoApto ? "#E8F5E9" : "#FFEBEE";
    }

    public class EquipamientoAvaluoDto
    {
        public int Id { get; set; }
        public int AvaluoId { get; set; }

        // Electrónico
        public bool ACC { get; set; }
        public bool Quemacocos { get; set; }
        public bool EspejosElectricos { get; set; }
        public bool SegurosElectricos { get; set; }
        public bool CristalesElectricos { get; set; }
        public bool AsientosElectricos { get; set; }
        public bool FarosNiebla { get; set; }
        public bool RinesAluminio { get; set; }
        public bool ControlesVolante { get; set; }
        public bool EstereoCD { get; set; }
        public bool ABS { get; set; }
        public bool DireccionAsistida { get; set; }
        public bool BolsasAire { get; set; }
        public bool TransmisionAutomatica { get; set; }
        public bool TransmisionManual { get; set; }
        public bool Turbo { get; set; }
        public bool Traccion4x4 { get; set; }
        public bool Bluetooth { get; set; }
        public bool USB { get; set; }
        public bool Pantalla { get; set; }
        public bool GPS { get; set; }

        // Mecánico
        public byte CantidadPuertas { get; set; }
        public string Vestiduras { get; set; } = string.Empty;
        public string Motor { get; set; } = string.Empty;
        public byte CantidadCilindros { get; set; }
        public bool FacturaOriginal { get; set; }
        public byte NumeroDuenos { get; set; }
        public byte Refacturaciones { get; set; }
        public short? UltimaTenenciaPagada { get; set; }
        public short? Verificacion { get; set; }
        public bool DuplicadoLlave { get; set; }
        public bool CarnetServicios { get; set; }
        public string? EquipoAdicional { get; set; }
        public string MarcaLlantasDelanteras { get; set; } = string.Empty;
        public byte? VidaUtilLlantasDelanteras { get; set; }
        public string MarcaLlantasTraseras { get; set; } = string.Empty;
        public byte? VidaUtilLlantasTraseras { get; set; }

        // Propiedades calculadas para resumen visual
        public int TotalEquipamientoActivo =>
            (ACC ? 1 : 0) + (Quemacocos ? 1 : 0) + (EspejosElectricos ? 1 : 0) +
            (SegurosElectricos ? 1 : 0) + (CristalesElectricos ? 1 : 0) +
            (AsientosElectricos ? 1 : 0) + (FarosNiebla ? 1 : 0) +
            (RinesAluminio ? 1 : 0) + (ControlesVolante ? 1 : 0) +
            (EstereoCD ? 1 : 0) + (ABS ? 1 : 0) + (DireccionAsistida ? 1 : 0) +
            (BolsasAire ? 1 : 0) + (Bluetooth ? 1 : 0) + (USB ? 1 : 0) +
            (Pantalla ? 1 : 0) + (GPS ? 1 : 0) + (Turbo ? 1 : 0) +
            (Traccion4x4 ? 1 : 0);

        public string TransmisionTexto =>
            TransmisionAutomatica ? "Automática" :
            TransmisionManual ? "Manual" : "No especificada";

        public string LlantasDelanteras =>
            $"{MarcaLlantasDelanteras} {(VidaUtilLlantasDelanteras.HasValue ? VidaUtilLlantasDelanteras + "%" : "")}".Trim();

        public string LlantasTraseras =>
            $"{MarcaLlantasTraseras} {(VidaUtilLlantasTraseras.HasValue ? VidaUtilLlantasTraseras + "%" : "")}".Trim();
    }

    public class ReparacionAvaluoDto
    {
        public int Id { get; set; }
        public int AvaluoId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal CostoAproximado { get; set; }

        // Propiedades calculadas
        public string CostoFormateado => $"${CostoAproximado:N2}";
    }

    public class FotoAvaluoDto
    {
        public int Id { get; set; }
        public int AvaluoId { get; set; }
        public string? TipoFoto { get; set; }
        public string? RutaFoto { get; set; }
        public DateTime Fecha { get; set; }

        // Propiedades calculadas
        public string FechaFormateada => Fecha.ToString("dd/MMM/yyyy HH:mm");
    }
    public class AvaluoDatosSimplesResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Vendedor { get; set; } = string.Empty;
        public string VehiculoCompleto { get; set; } = string.Empty;
        public string VIN { get; set; } = string.Empty;

    }

    // ============================================
    // VIEWMODEL para item de equipamiento en UI
    // ============================================

    public class EquipamientoItemViewModel : INotifyPropertyChanged
    {
        private bool _activo;

        public string Nombre { get; set; } = string.Empty;
        public string Icono { get; set; } = string.Empty;

        public bool Activo
        {
            get => _activo;
            set
            {
                _activo = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ColorFondo));
                OnPropertyChanged(nameof(ColorBorde));
                OnPropertyChanged(nameof(ColorTexto));
            }
        }

        public string ColorFondo => Activo ? "#E8F5E9" : "#F5F5F5";
        public string ColorBorde => Activo ? "#43A047" : "#BDBDBD";
        public string ColorTexto => Activo ? "#2E7D32" : "#757575";

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}