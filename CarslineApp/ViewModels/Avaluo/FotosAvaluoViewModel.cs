using CarslineApp.Models;
using CarslineApp.Services;
using CarslineApp.Views.Avaluo;
using CarslineApp.ViewModels.Avaluo;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CarslineApp.ViewModels
{
    public class FotosAvaluoViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;

        // ══════════════════════════════════════════════════════════════
        #region ── Estado ────────────────────────────────────────────
        // ══════════════════════════════════════════════════════════════

        private int _avaluoId;
        private bool _isLoading;
        private string _mensajeEstado = string.Empty;

        // Info del vehículo para el header
        private string _vehiculoCompleto = string.Empty;
        private string _vin = string.Empty;
        private string _cliente = string.Empty;

        private ObservableCollection<FotoAvaluoItem> _fotos = new();

        public int AvaluoId
        {
            get => _avaluoId;
            set
            {
                _avaluoId = value;
                OnPropertyChanged();
                Task.Run(async () =>
                {
                    await CargarInfoAvaluoAsync(value);
                    await CargarFotosExistentesAsync();
                });
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public string MensajeEstado
        {
            get => _mensajeEstado;
            set { _mensajeEstado = value; OnPropertyChanged(); }
        }

        public string VehiculoCompleto
        {
            get => _vehiculoCompleto;
            set { _vehiculoCompleto = value; OnPropertyChanged(); }
        }

        public string VIN
        {
            get => _vin;
            set { _vin = value; OnPropertyChanged(); }
        }

        public string Cliente
        {
            get => _cliente;
            set { _cliente = value; OnPropertyChanged(); }
        }

        public ObservableCollection<FotoAvaluoItem> Fotos
        {
            get => _fotos;
            set
            {
                _fotos = value;
                OnPropertyChanged();
                ((Command)GuardarFotosCommand).ChangeCanExecute();
            }
        }

        #endregion

        // ══════════════════════════════════════════════════════════════
        #region ── Comandos ──────────────────────────────────────────
        // ══════════════════════════════════════════════════════════════

        public ICommand TomarFotoCommand { get; }
        public ICommand SeleccionarFotoCommand { get; }
        public ICommand GuardarFotosCommand { get; }
        public ICommand EliminarFotoCommand { get; }
        public ICommand CargarFotosCommand { get; }
        public ICommand VerFotoCommand { get; }
        public ICommand EditarFotoCommand { get; }
        public ICommand BackCommand { get; }

        #endregion

        // ══════════════════════════════════════════════════════════════
        #region ── Constructor ───────────────────────────────────────
        // ══════════════════════════════════════════════════════════════

        public FotosAvaluoViewModel()
        {
            _apiService = new ApiService();

            TomarFotoCommand = new Command<string>(async (tipo) => await TomarFotoAsync(tipo));
            SeleccionarFotoCommand = new Command<string>(async (tipo) => await SeleccionarFotoAsync(tipo));
            GuardarFotosCommand = new Command(async () => await GuardarFotosAsync(),() => Fotos.Any(f => f.TieneFoto && f.FotoId == null));
            EliminarFotoCommand = new Command<FotoAvaluoItem>(async (foto) => await EliminarFotoAsync(foto));
            CargarFotosCommand = new Command(async () => await CargarFotosExistentesAsync());
            BackCommand = new Command(async () => await RegresarAsync());

            VerFotoCommand = new Command<FotoAvaluoItem>(async (foto) =>
            {
                if (foto?.ImagenBytes == null) return;
                await Application.Current.MainPage.Navigation
                    .PushModalAsync(new AvaluoImagePreviewPage(foto.ImagenBytes, foto.Descripcion));
            });

            EditarFotoCommand = new Command<FotoAvaluoItem>(async (foto) =>
            {
                if (foto?.ImagenBytes == null) return;

                // Abre el editor de anotaciones
                var editorPage = new AvaluoImageEditorPage(foto.ImagenBytes, foto.Descripcion);
                editorPage.ImagenEditada += async (bytesEditados) =>
                {
                    // Actualizar preview con la imagen editada
                    foto.ImagenBytes = bytesEditados;
                    foto.TieneFoto = true;
                    foto.FotoId = null; // Marcar como pendiente de guardar
                    foto.ImagenPreview = ImageSource.FromStream(() => new MemoryStream(bytesEditados));
                    MensajeEstado = $"✏️ Anotaciones guardadas en {foto.Descripcion}";
                    ((Command)GuardarFotosCommand).ChangeCanExecute();
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                };
                await Application.Current.MainPage.Navigation.PushModalAsync(editorPage);
            });

            InicializarTiposEvidencia();
        }

        #endregion

        // ══════════════════════════════════════════════════════════════
        #region ── Tipos de foto ─────────────────────────────────────
        // ══════════════════════════════════════════════════════════════

        private void InicializarTiposEvidencia()
        {
            var tipos = new[]
            {
                "Frontal",
                "Lateral Derecha",
                "Trasera",
                "Lateral Izquierda",
                "Interior Delantero",
                "Interior Trasero",
                "Tablero",
                "Motor",
                "Cajuela",
                "Carrocería Detalle",
                "Golpe / Daño"
            };

            foreach (var tipo in tipos)
                Fotos.Add(new FotoAvaluoItem { Descripcion = tipo });
        }

        #endregion

        // ══════════════════════════════════════════════════════════════
        #region ── Carga de info del avalúo ──────────────────────────
        // ══════════════════════════════════════════════════════════════

        private async Task CargarInfoAvaluoAsync(int avaluoId)
        {
            try
            {
                IsLoading = true;
                MensajeEstado = "Cargando información del avalúo...";

                var response = await _apiService.ObtenerDatosSimplesAvaluosAsync(avaluoId);

                if (response.Success)
                {
                    VehiculoCompleto = response.VehiculoCompleto;
                    VIN = response.VIN;
                    Cliente = response.Vendedor;
                    MensajeEstado = string.Empty;
                }
                else
                {
                    MensajeEstado = "No se pudo cargar el detalle del avalúo";
                }
            }
            catch (Exception ex)
            {
                MensajeEstado = $"Error al cargar avalúo: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion

        // ══════════════════════════════════════════════════════════════
        #region ── Captura / Selección ───────────────────────────────
        // ══════════════════════════════════════════════════════════════

        private async Task TomarFotoAsync(string tipo)
        {
            try
            {
                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    await ShowAlert("Cámara no disponible", "La cámara no está disponible en este dispositivo.");
                    return;
                }

                var photo = await MediaPicker.Default.CapturePhotoAsync(
                    new MediaPickerOptions { Title = $"Capturar {tipo}" });

                if (photo != null)
                    await ProcesarFotoAsync(photo, tipo);
            }
            catch (PermissionException)
            {
                await ShowAlert("Permisos", "Se necesitan permisos de cámara para continuar.");
            }
            catch (Exception ex)
            {
                MensajeEstado = $"Error al tomar foto: {ex.Message}";
            }
        }

        private async Task SeleccionarFotoAsync(string tipo)
        {
            try
            {
                var photo = await MediaPicker.Default.PickPhotoAsync(
                    new MediaPickerOptions { Title = $"Seleccionar {tipo}" });

                if (photo != null)
                    await ProcesarFotoAsync(photo, tipo);
            }
            catch (PermissionException)
            {
                await ShowAlert("Permisos", "Se necesitan permisos para acceder a la galería.");
            }
            catch (Exception ex)
            {
                MensajeEstado = $"Error al seleccionar foto: {ex.Message}";
            }
        }

        private async Task ProcesarFotoAsync(FileResult photo, string tipo)
        {
            var item = Fotos.FirstOrDefault(f => f.Descripcion == tipo);
            if (item == null) return;

            using var stream = await photo.OpenReadAsync();
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);

            var bytesOriginales = ms.ToArray();
            item.ImagenBytes = await ComprimirImagenAsync(bytesOriginales, calidadJpeg: 90, anchoMaximo: 1280);
            item.NombreArchivo = photo.FileName;
            item.TieneFoto = true;
            item.FotoId = null; // pendiente de guardar
            item.ImagenPreview = ImageSource.FromStream(() => new MemoryStream(item.ImagenBytes));

            MensajeEstado = $"📷 {tipo} capturada";
            ((Command)GuardarFotosCommand).ChangeCanExecute();
        }

        #endregion

        // ══════════════════════════════════════════════════════════════
        #region ── Guardar ───────────────────────────────────────────
        // ══════════════════════════════════════════════════════════════

        private async Task GuardarFotosAsync()
        {
            try
            {
                IsLoading = true;
                MensajeEstado = "Guardando fotos...";

                var pendientes = Fotos
                    .Where(f => f.TieneFoto && f.ImagenBytes != null && f.FotoId == null)
                    .ToList();

                if (!pendientes.Any())
                {
                    MensajeEstado = "No hay fotos nuevas para guardar";
                    await ShowAlert("Información", "No hay fotos nuevas para guardar.");
                    return;
                }

                var uploads = pendientes.Select(f => new FotoAvaluoUpload
                {
                    TipoFoto = f.Descripcion,
                    ImagenBytes = f.ImagenBytes!,
                    NombreArchivo = f.NombreArchivo ?? $"{f.Descripcion.Replace(" ", "_")}.jpg"
                }).ToList();

                var resultado = await _apiService.SubirFotosAvaluoAsync(AvaluoId, uploads);

                if (resultado.Success)
                {
                    MensajeEstado = $"✅ {pendientes.Count} foto(s) guardada(s)";

                    // Limpiar estado pendiente
                    foreach (var f in pendientes)
                    {
                        f.ImagenBytes = null;
                        f.NombreArchivo = null;
                        f.TieneFoto = false;
                        f.ImagenPreview = null;
                    }

                    await ShowAlert("✅ Éxito", resultado.Message);
                    await CargarFotosExistentesAsync();
                }
                else
                {
                    MensajeEstado = resultado.Message;
                    await ShowAlert("❌ Error", resultado.Message);
                }
            }
            catch (Exception ex)
            {
                MensajeEstado = $"Error: {ex.Message}";
                await ShowAlert("Error", $"Error al guardar fotos: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                ((Command)GuardarFotosCommand).ChangeCanExecute();
            }
        }

        #endregion

        // ══════════════════════════════════════════════════════════════
        #region ── Eliminar ──────────────────────────────────────────
        // ══════════════════════════════════════════════════════════════

        private async Task EliminarFotoAsync(FotoAvaluoItem foto)
        {
            if (foto == null || !foto.TieneFoto) return;

            bool confirmar = await Application.Current.MainPage.DisplayAlert(
                "Eliminar foto",
                $"¿Eliminar la foto \"{foto.Descripcion}\"?",
                "Sí, eliminar", "Cancelar");

            if (!confirmar) return;

            IsLoading = true;
            MensajeEstado = "Eliminando foto...";
            try
            {
                // Si ya existe en servidor la eliminamos (reutilizamos el endpoint de evidencias
                // si la API de avalúos no tiene endpoint de borrado individual de foto,
                // simplemente reseteamos localmente)
                if (foto.FotoId.HasValue && foto.FotoId.Value > 0)
                {
                    // TODO: llamar _apiService.EliminarFotoAvaluoAsync(foto.FotoId.Value)
                    // cuando la API lo soporte. Por ahora solo limpiamos local.
                }

                foto.ImagenBytes = null;
                foto.NombreArchivo = null;
                foto.TieneFoto = false;
                foto.ImagenPreview = null;
                foto.FotoId = null;

                MensajeEstado = $"Foto {foto.Descripcion} eliminada";
                ((Command)GuardarFotosCommand).ChangeCanExecute();
            }
            catch (Exception ex)
            {
                MensajeEstado = $"Error: {ex.Message}";
            }
            finally { IsLoading = false; }
        }

        #endregion

        // ══════════════════════════════════════════════════════════════
        #region ── Cargar fotos existentes ───────────────────────────
        // ══════════════════════════════════════════════════════════════

        private async Task CargarFotosExistentesAsync()
        {
            if (AvaluoId <= 0) return;
            try
            {
                IsLoading = true;
                MensajeEstado = "Cargando fotos existentes...";

                var response = await _apiService.ObtenerAvaluoCompletoAsync(AvaluoId);

                if (response?.Fotos == null || !response.Fotos.Any())
                {
                    MensajeEstado = string.Empty;
                    return;
                }

                foreach (var fotoDto in response.Fotos)
                {
                    var local = Fotos.FirstOrDefault(f =>
                        string.Equals(f.Descripcion, fotoDto.TipoFoto, StringComparison.OrdinalIgnoreCase));

                    if (local == null) continue;

                    local.FotoId = fotoDto.Id;
                    local.TieneFoto = true;

                    // Construir preview a partir de la URL del servidor
                    var url = _apiService.ObtenerUrlFotoAvaluo(fotoDto.Id);
                    local.ImagenPreview = ImageSource.FromUri(new Uri(url));
                }

                MensajeEstado = $"Se cargaron {response.Fotos.Count} foto(s) existentes";
            }
            catch (Exception ex)
            {
                MensajeEstado = $"Error al cargar fotos: {ex.Message}";
            }
            finally { IsLoading = false; }
        }

        #endregion

        // ══════════════════════════════════════════════════════════════
        #region ── Navegación ────────────────────────────────────────
        // ══════════════════════════════════════════════════════════════

        private async Task RegresarAsync()
        {
            var pendientes = Fotos.Any(f => f.TieneFoto && f.FotoId == null);
            if (pendientes)
            {
                bool salir = await Application.Current.MainPage.DisplayAlert(
                    "Fotos sin guardar",
                    "Tienes fotos sin guardar. ¿Deseas salir de todas formas?",
                    "Sí, salir", "Cancelar");
                if (!salir) return;
            }
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        #endregion

        // ══════════════════════════════════════════════════════════════
        #region ── Compresión + EXIF ─────────────────────────────────
        // ══════════════════════════════════════════════════════════════

        private async Task<byte[]> ComprimirImagenAsync(byte[] original, int calidadJpeg = 70, int anchoMaximo = 1280)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var bmp = SKBitmap.Decode(original);
                    if (bmp == null) return original;

                    int rot = ObtenerRotacionExif(original);
                    bool intercambia = rot == 90 || rot == 270;
                    int anchoReal = intercambia ? bmp.Height : bmp.Width;
                    int altoReal = intercambia ? bmp.Width : bmp.Height;

                    int nW = anchoReal, nH = altoReal;
                    if (anchoReal > anchoMaximo)
                    {
                        float esc = (float)anchoMaximo / anchoReal;
                        nW = anchoMaximo;
                        nH = (int)(altoReal * esc);
                    }

                    SKBitmap resized = bmp;
                    if (nW != bmp.Width || nH != bmp.Height)
                    {
                        var info = intercambia ? new SKImageInfo(nH, nW) : new SKImageInfo(nW, nH);
                        resized = bmp.Resize(info, SKFilterQuality.Medium);
                    }

                    var final = AplicarRotacion(resized, rot);

                    using var img = SKImage.FromBitmap(final);
                    using var data = img.Encode(SKEncodedImageFormat.Jpeg, calidadJpeg);
                    var resultado = data.ToArray();

                    if (!ReferenceEquals(resized, bmp)) resized.Dispose();
                    if (!ReferenceEquals(final, resized)) final.Dispose();
                    bmp.Dispose();

                    return resultado;
                });
            }
            catch { return original; }
        }

        private static int ObtenerRotacionExif(byte[] bytes)
        {
            try
            {
                using var ms = new MemoryStream(bytes);
                using var stream = new SKManagedStream(ms);
                using var codec = SKCodec.Create(stream);
                if (codec == null) return 0;
                return codec.EncodedOrigin switch
                {
                    SKEncodedOrigin.RightTop => 90,
                    SKEncodedOrigin.BottomRight => 180,
                    SKEncodedOrigin.LeftBottom => 270,
                    _ => 0
                };
            }
            catch { return 0; }
        }

        private static SKBitmap AplicarRotacion(SKBitmap bmp, int grados)
        {
            if (grados == 0) return bmp;
            bool intercambia = grados == 90 || grados == 270;
            int nW = intercambia ? bmp.Height : bmp.Width;
            int nH = intercambia ? bmp.Width : bmp.Height;
            var rotado = new SKBitmap(nW, nH);
            using var canvas = new SKCanvas(rotado);
            canvas.Translate(nW / 2f, nH / 2f);
            canvas.RotateDegrees(grados);
            canvas.Translate(-bmp.Width / 2f, -bmp.Height / 2f);
            canvas.DrawBitmap(bmp, 0, 0);
            return rotado;
        }

        #endregion

        // ══════════════════════════════════════════════════════════════
        #region ── Helpers ───────────────────────────────────────────
        // ══════════════════════════════════════════════════════════════

        private static Task ShowAlert(string title, string msg) =>
            Application.Current.MainPage.DisplayAlert(title, msg, "OK");

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion
    }

    // ══════════════════════════════════════════════════════════════════
    // Modelo de item para cada foto del avalúo
    // ══════════════════════════════════════════════════════════════════
    public class FotoAvaluoItem : INotifyPropertyChanged
    {
        private int? _fotoId;
        private string _descripcion = string.Empty;
        private byte[]? _imagenBytes;
        private string? _nombreArchivo;
        private bool _tieneFoto;
        private ImageSource? _imagenPreview;

        public int? FotoId { get => _fotoId; set { _fotoId = value; OnPropertyChanged(); } }
        public string Descripcion { get => _descripcion; set { _descripcion = value; OnPropertyChanged(); } }
        public byte[]? ImagenBytes { get => _imagenBytes; set { _imagenBytes = value; OnPropertyChanged(); } }
        public string? NombreArchivo { get => _nombreArchivo; set { _nombreArchivo = value; OnPropertyChanged(); } }

        public bool TieneFoto
        {
            get => _tieneFoto;
            set { _tieneFoto = value; OnPropertyChanged(); }
        }

        public ImageSource? ImagenPreview
        {
            get => _imagenPreview;
            set { _imagenPreview = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}