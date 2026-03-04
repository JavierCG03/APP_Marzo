using CarslineApp.Models;
using CarslineApp.Views;
using CarslineApp.Services;
using SkiaSharp;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace CarslineApp.ViewModels
{
    public class CheckListServicioViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private bool _isLoading;
        private bool _trabajoFinalizado = false;

        // Comentarios
        private string _comentariosDireccion = string.Empty;
        private string _comentariosSuspension = string.Empty;
        private string _comentariosFrenado = string.Empty;
        private string _comentariosNeumaticos = string.Empty;
        private string _comentariosRefacciones = string.Empty;
        private string _comentariosNiveles = string.Empty;
        private string _comentariosTrabajos = string.Empty;
        private string _comentariosGenerales = string.Empty;

        // ✅ NUEVO: Propiedades para mostrar/ocultar secciones de evidencias
        private bool _mostrarEvidenciaBalatasDelanteras;
        private bool _mostrarEvidenciaBalatasTraseras;
        private bool _mostrarEvidenciaAceiteMotor;
        private bool _mostrarEvidenciaFiltroAceite;
        private bool _mostrarEvidenciaFiltroAireMotor;
        private bool _mostrarEvidenciaFiltroAirePolen;

        // ✅ NUEVO: Colecciones de evidencias

        public ObservableCollection<EvidenciaTrabajoItem> EvidenciasBalatasDelanteras { get; set; }
        public ObservableCollection<EvidenciaTrabajoItem> EvidenciasBalatasTraseras { get; set; }
        public ObservableCollection<EvidenciaTrabajoItem> EvidenciasAceiteMotor { get; set; }
        public ObservableCollection<EvidenciaTrabajoItem> EvidenciasFiltroAceite { get; set; }
        public ObservableCollection<EvidenciaTrabajoItem> EvidenciasFiltroAireMotor { get; set; }
        public ObservableCollection<EvidenciaTrabajoItem> EvidenciasFiltroAirePolen { get; set; }

        public string Orden { get; }
        public int TrabajoId { get; }
        public string Vehiculo { get; }
        public string Indicaciones { get; }
        public string Trabajo { get; }
        public string VehiculoVIN { get; }

        public CheckListServicioModel CheckList { get; }

        public CheckListServicioViewModel(int trabajoId, int ordenId, string orden, string trabajo, string vehiculo, string indicaciones, string VIN)
        {
            _apiService = new ApiService();
            CheckList = new CheckListServicioModel
            {
                TrabajoId = trabajoId,
                OrdenId = ordenId,
                Trabajo = trabajo
            };

            TrabajoId = trabajoId;
            Orden = orden;
            Vehiculo = vehiculo;
            Trabajo = trabajo;
            VehiculoVIN = VIN;

            // ✅ Inicializar colecciones de evidencias
            InicializarEvidencias();

            GuardarCommand = new Command(async () => await GuardarCheckList(), () => !IsLoading);
            PausarCommand = new Command(async () => await PausarTrabajo(), () => !IsLoading);

            // ✅ NUEVO: Comandos para evidencias
            TomarFotoCommand = new Command<string>(async (tipo) => await TomarFoto(tipo));
            SeleccionarImagenCommand = new Command<string>(async (tipo) => await SeleccionarImagen(tipo));
            EliminarEvidenciaCommand = new Command<EvidenciaTrabajoItem>(async (evidencia) => await EliminarEvidencia(evidencia));
            VerImagenCommand = new Command<ImageSource>(async (imagen) =>
            {
                if (imagen == null) return;

                await Application.Current.MainPage.Navigation
                    .PushModalAsync(new ImagePreviewPage(imagen));
            });
        }

        #region Propiedades

        public bool MostrarEvidenciaBalatasDelanteras
        {
            get => _mostrarEvidenciaBalatasDelanteras;
            set { _mostrarEvidenciaBalatasDelanteras = value; OnPropertyChanged(); }
        }
        public bool MostrarEvidenciaBalatasTraseras
        {
            get => _mostrarEvidenciaBalatasTraseras;
            set { _mostrarEvidenciaBalatasTraseras = value; OnPropertyChanged(); }
        }

        public bool MostrarEvidenciaAceiteMotor
        {
            get => _mostrarEvidenciaAceiteMotor;
            set { _mostrarEvidenciaAceiteMotor = value; OnPropertyChanged(); }
        }

        public bool MostrarEvidenciaFiltroAceite
        {
            get => _mostrarEvidenciaFiltroAceite;
            set { _mostrarEvidenciaFiltroAceite = value; OnPropertyChanged(); }
        }

        public bool MostrarEvidenciaFiltroAireMotor
        {
            get => _mostrarEvidenciaFiltroAireMotor;
            set { _mostrarEvidenciaFiltroAireMotor = value; OnPropertyChanged(); }
        }

        public bool MostrarEvidenciaFiltroAirePolen
        {
            get => _mostrarEvidenciaFiltroAirePolen;
            set { _mostrarEvidenciaFiltroAirePolen = value; OnPropertyChanged(); }
        }

        public string ComentariosDireccion
        {
            get => _comentariosDireccion;
            set { _comentariosDireccion = value; OnPropertyChanged(); }
        }

        public string ComentariosSuspension
        {
            get => _comentariosSuspension;
            set { _comentariosSuspension = value; OnPropertyChanged(); }
        }

        public string ComentariosFrenado
        {
            get => _comentariosFrenado;
            set { _comentariosFrenado = value; OnPropertyChanged(); }
        }

        public string ComentariosNeumaticos
        {
            get => _comentariosNeumaticos;
            set { _comentariosNeumaticos = value; OnPropertyChanged(); }
        }

        public string ComentariosRefacciones
        {
            get => _comentariosRefacciones;
            set { _comentariosRefacciones = value; OnPropertyChanged(); }
        }

        public string ComentariosNiveles
        {
            get => _comentariosNiveles;
            set { _comentariosNiveles = value; OnPropertyChanged(); }
        }

        public string ComentariosTrabajos
        {
            get => _comentariosTrabajos;
            set { _comentariosTrabajos = value; OnPropertyChanged(); }
        }

        public string ComentariosGenerales
        {
            get => _comentariosGenerales;
            set { _comentariosGenerales = value; OnPropertyChanged(); }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                ((Command)GuardarCommand).ChangeCanExecute();
                ((Command)PausarCommand).ChangeCanExecute();
            }
        }

        #endregion

        #region Comandos

        public ICommand GuardarCommand { get; }
        public ICommand PausarCommand { get; }
        public ICommand TomarFotoCommand { get; }
        public ICommand SeleccionarImagenCommand { get; }
        public ICommand EliminarEvidenciaCommand { get; }
        public ICommand VerImagenCommand { get; }

        #endregion

        #region Métodos de Evidencias

        private void InicializarEvidencias()
        {

            EvidenciasBalatasDelanteras = new ObservableCollection<EvidenciaTrabajoItem>
            {
                new EvidenciaTrabajoItem
                {
                    TipoEvidencia = "BalatasDelanteras",
                    Descripcion = "Estado de balatas delanteras "
                }
            };

            EvidenciasBalatasTraseras = new ObservableCollection<EvidenciaTrabajoItem>
            {
                new EvidenciaTrabajoItem
                {
                    TipoEvidencia = "BalatasTraseras",
                    Descripcion = "Estado de balatas traseras"
                }
            };

            // Aceite de Motor
            EvidenciasAceiteMotor = new ObservableCollection<EvidenciaTrabajoItem>
            {
                new EvidenciaTrabajoItem
                {
                    TipoEvidencia = "AceiteMotor",
                    Descripcion = "Aceite drenado"
                }
            };

            // Filtro de Aceite
            EvidenciasFiltroAceite = new ObservableCollection<EvidenciaTrabajoItem>
            {
                new EvidenciaTrabajoItem
                {
                    TipoEvidencia = "FiltroAceite",
                    Descripcion = "Filtro de aceite usado"
                }
            };

            // Filtro de Aire Motor
            EvidenciasFiltroAireMotor = new ObservableCollection<EvidenciaTrabajoItem>
            {
                new EvidenciaTrabajoItem
                {
                    TipoEvidencia = "FiltroAireMotor",
                    Descripcion = "Filtro de aire motor usado"
                }
            };

            // Filtro de Aire Polen
            EvidenciasFiltroAirePolen = new ObservableCollection<EvidenciaTrabajoItem>
            {
                new EvidenciaTrabajoItem
                {
                    TipoEvidencia = "FiltroAirePolen",
                    Descripcion = "Filtro de aire polen usado"
                }
            };
        }

        private async Task TomarFoto(string tipo)
        {
            try
            {
                if (!MediaPicker.Default.IsCaptureSupported)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "Advertencia",
                        "La cámara no está disponible",
                        "OK");
                    return;
                }

                var photo = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = $"Capturar evidencia"
                });

                if (photo != null)
                {
                    await ProcesarImagen(photo, tipo);
                }
            }
            catch (PermissionException)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Permisos",
                    "Se necesitan permisos de cámara para continuar",
                    "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"Error al tomar foto: {ex.Message}",
                    "OK");
            }
        }

        private async Task SeleccionarImagen(string tipo)
        {
            try
            {
                var photo = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = $"Seleccionar evidencia"
                });

                if (photo != null)
                {
                    await ProcesarImagen(photo, tipo);
                }
            }
            catch (PermissionException)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Permisos",
                    "Se necesitan permisos para acceder a la galería",
                    "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"Error al seleccionar imagen: {ex.Message}",
                    "OK");
            }
        }

        private async Task ProcesarImagen(FileResult photo, string tipo)
        {
            try
            {
                // Leer la imagen como bytes
                using var stream = await photo.OpenReadAsync();
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);

                var imagenOriginal = memoryStream.ToArray();
                var imageBytes = await Task.Run(() =>
                ComprimirImagen(imagenOriginal, calidadJpeg: 70, anchoMaximo: 1280).Result);
                //var imageBytes = memoryStream.ToArray();
                var imageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));

                // Encontrar la evidencia correspondiente según el tipo
                EvidenciaTrabajoItem? evidencia = tipo switch
                {
                    "BalatasDelanteras" => EvidenciasBalatasDelanteras.FirstOrDefault(),
                    "BalatasTraseras" => EvidenciasBalatasTraseras.FirstOrDefault(),
                    "AceiteMotor" => EvidenciasAceiteMotor.FirstOrDefault(),
                    "FiltroAceite" => EvidenciasFiltroAceite.FirstOrDefault(),
                    "FiltroAireMotor" => EvidenciasFiltroAireMotor.FirstOrDefault(),
                    "FiltroAirePolen" => EvidenciasFiltroAirePolen.FirstOrDefault(),
                    _ => null
                };

                if (evidencia != null)
                {
                    evidencia.ImagenBytes = imageBytes;
                    evidencia.NombreArchivo = photo.FileName;
                    evidencia.ImagenPreview = imageSource;
                    evidencia.TieneImagen = true;

                    // ✅ Forzar actualización de la UI
                    evidencia.OnPropertyChanged(nameof(evidencia.TieneImagen));
                    evidencia.OnPropertyChanged(nameof(evidencia.ImagenPreview));
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"Error al procesar imagen: {ex.Message}",
                    "OK");
            }
        }

        private async Task EliminarEvidencia(EvidenciaTrabajoItem evidencia)
        {
            if (evidencia == null || !evidencia.TieneImagen)
                return;

            bool confirmar = await Application.Current.MainPage.DisplayAlert(
                "Eliminar evidencia",
                $"¿Deseas eliminar la foto de {evidencia.Descripcion}?",
                "Sí, eliminar",
                "Cancelar");

            if (!confirmar)
                return;

            evidencia.ImagenBytes = null;
            evidencia.NombreArchivo = null;
            evidencia.TieneImagen = false;
            evidencia.ImagenPreview = null;

            // Notificar cambio
            OnPropertyChanged(evidencia.TipoEvidencia switch
            {
                "BalatasDelanteras" => nameof(EvidenciasBalatasDelanteras),
                "BalatasTraseras" => nameof(EvidenciasBalatasTraseras),
                "AceiteMotor" => nameof(EvidenciasAceiteMotor),
                "FiltroAceite" => nameof(EvidenciasFiltroAceite),
                "FiltroAireMotor" => nameof(EvidenciasFiltroAireMotor),
                "FiltroAirePolen" => nameof(EvidenciasFiltroAirePolen),
                _ => string.Empty
            });
        }

        #endregion

        #region Métodos Principales

        private async Task PausarTrabajo()
        {
            bool confirmar = await Application.Current.MainPage.DisplayAlert(
                "Confirmar",
                "¿Deseas pausar el servicio?",
                "Sí",
                "Cancelar");
            if (!confirmar) return;

            string motivo = await Application.Current.MainPage.DisplayPromptAsync(
                "Motivo de la pausa",
                "Describe el motivo de la pausa",
                "Aceptar",
                "Cancelar",
                placeholder: "Ej. Esperando refacciones");

            if (string.IsNullOrWhiteSpace(motivo))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Atención",
                    "El motivo de la pausa es obligatorio",
                    "OK");
                return;
            }

            int tecnicoId = Preferences.Get("user_id", 0);

            var response = await _apiService.PausarTrabajoAsync(
                TrabajoId,
                tecnicoId,
                motivo.Trim()
            );

            if (response.Success)
            {
                _trabajoFinalizado = true;
                await Application.Current.MainPage.DisplayAlert("Éxito", response.Message, "OK");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "OK");
            }
        }

        public void SetValor(string campo, object valor)
        {
            var prop = typeof(CheckListServicioModel).GetProperty(campo);
            if (prop == null) return;

            try
            {
                if (prop.PropertyType == typeof(string))
                {
                    prop.SetValue(CheckList, valor?.ToString() ?? string.Empty);
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    bool boolValue = valor is string strValor
                        ? strValor.ToLower() == "true"
                        : Convert.ToBoolean(valor);

                    prop.SetValue(CheckList, boolValue);

                    // ✅ NUEVO: Mostrar/ocultar secciones de evidencias
                    if (campo == "ReemplazoAceiteMotor")
                        MostrarEvidenciaAceiteMotor = boolValue;
                    else if (campo == "ReemplazoFiltroAceite")
                        MostrarEvidenciaFiltroAceite = boolValue;
                    else if (campo == "ReemplazoFiltroAireMotor")
                        MostrarEvidenciaFiltroAireMotor = boolValue;
                    else if (campo == "ReemplazoFiltroAirePolen")
                        MostrarEvidenciaFiltroAirePolen = boolValue;
                }

                System.Diagnostics.Debug.WriteLine($"✅ Campo '{campo}' = '{valor}'");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error al asignar {campo}: {ex.Message}");
            }
        }

        // ✅ NUEVO: Método para detectar cuando se selecciona vida útil de balatas
        public void ValidarMostrarEvidenciaBalatasDelanteras(string valorBalatasDelanteras)
        {
            MostrarEvidenciaBalatasDelanteras =
                valorBalatasDelanteras == "1mm" ||  valorBalatasDelanteras == "2mm-3mm";

        }

        public void ValidarMostrarEvidenciaBalatasTraseras(string valorBalatasTraseras)
        { 

            MostrarEvidenciaBalatasTraseras =
                valorBalatasTraseras == "1mm" ||  valorBalatasTraseras == "2mm-3mm";
        }

        private string ConstruirComentarios()
        {
            var sb = new System.Text.StringBuilder();

            void Agregar(string titulo, string contenido)
            {
                if (!string.IsNullOrWhiteSpace(contenido))
                {
                    sb.AppendLine($"- {titulo}: {contenido.Trim()}");
                }
            }

            Agregar( CheckList.Trabajo , "Realizado con Exito");

            if (MostrarEvidenciaBalatasDelanteras && MostrarEvidenciaBalatasTraseras)
            {
                Agregar("Balatas", $"El vehiculo necesita Remplazo de balatas Generales, vida util de balatas (Delanteras: {CheckList.BalatasDelanteras},Traseras:{CheckList.BalatasTraseras})");

            }
            else if (MostrarEvidenciaBalatasDelanteras)
            {
                Agregar("Balatas", $"El vehiculo necesita Remplazo de balatas Delanteras, vida util de balatas Delanteras: {CheckList.BalatasDelanteras}");

            }
            else if (MostrarEvidenciaBalatasTraseras)
            {
                Agregar("Balatas", $"El vehiculo necesita Remplazo de balatas Traseras, vida util de balatas Traseras: {CheckList.BalatasTraseras}");
            }

            Agregar("Sistema de Direccion", ComentariosDireccion);
            Agregar("Sistema de Suspension", ComentariosSuspension);
            Agregar("Sistema de Frenado", ComentariosFrenado);
            Agregar("Neumáticos", ComentariosNeumaticos);
            Agregar("Refacciones", ComentariosRefacciones);
            Agregar("Niveles", ComentariosNiveles);
            Agregar("Trabajos realizados", ComentariosTrabajos);
            Agregar("Comentarios generales", ComentariosGenerales);

            return sb.ToString().Trim();
        }

        public async Task RestablecerEstadoTrabajo()
        {
            if (_trabajoFinalizado) return;

            try
            {
                System.Diagnostics.Debug.WriteLine($"🔄 Restableciendo trabajo {CheckList.TrabajoId} a PENDIENTE");
                int tecnicoId = Preferences.Get("user_id", 0);
                var response = await _apiService.RestablecerTrabajoAsync(CheckList.TrabajoId, tecnicoId);

                if (!response.Success)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", response.Message, "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error al restablecer estado: {ex.Message}");
            }
        }

        private async Task GuardarCheckList()
        {
            if (IsLoading) return;

            try
            {
                if (!ValidarCheckList())
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "⚠️ Campos incompletos",
                        "Por favor completa todos los campos del checklist antes de Finalizar",
                        "OK");
                    return;
                }

                if (!ValidarTrabajos())
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "⚠️ Campos incompletos",
                        "Por favor revisa los niveles del vehiculo y realiza los trabajos Faltantes",
                        "OK");
                    return;
                }

                bool confirmar = await Application.Current.MainPage.DisplayAlert(
                    "Confirmar",
                    "¿Estás seguro de Finalizar el checklist?\n\n✅ Esto marcará el trabajo como COMPLETADO",
                    "Sí, guardar",
                    "Cancelar");

                if (!confirmar) return;

                CheckList.ComentariosTecnico = ConstruirComentarios();

                if (string.IsNullOrWhiteSpace(CheckList.ComentariosTecnico))
                {
                    if(MostrarEvidenciaBalatasDelanteras && MostrarEvidenciaBalatasTraseras)
                    {
                        CheckList.ComentariosTecnico = $"{CheckList.Trabajo} Realizado con Exito, el vehiculo necesita Remplazo de balatas Generales, vida util de balatas (Delanteras: {CheckList.BalatasDelanteras},Traseras:{CheckList.BalatasTraseras})";

                    }
                    else if (MostrarEvidenciaBalatasDelanteras)
                    {
                        CheckList.ComentariosTecnico = $"{CheckList.Trabajo} Realizado con Exito, el vehiculo necesita Remplazo de balatas Delanteras, vida util de balatas Delanteras: {CheckList.BalatasDelanteras}";
                    }
                    else if (MostrarEvidenciaBalatasTraseras)
                    {
                        CheckList.ComentariosTecnico = $"{CheckList.Trabajo} Realizado con Exito, el vehiculo necesita Remplazo de balatas Traseras, vida util de balatas Traseras: {CheckList.BalatasTraseras}";
                    }
                    else
                        CheckList.ComentariosTecnico = $"{CheckList.Trabajo} Realizado con Exito, el vehiculo no presenta fallas";
                }

                IsLoading = true;

                // ✅ 1. Guardar checklist
                var response = await _apiService.GuardarCheckListAsync(CheckList);

                if (!response.Success)
                {
                    await Application.Current.MainPage.DisplayAlert(
                        "❌ Error",
                        $"Error al Finalizar:\n{response.Message}",
                        "OK");
                    return;
                }

                // ✅ 2. Subir evidencias si existen
                await SubirEvidenciasTrabajo();

                _trabajoFinalizado = true;

                await Application.Current.MainPage.DisplayAlert(
                    "✅ Éxito",
                    "Checklist guardado exitosamente\n\nEl trabajo ha sido marcado como completado",
                    "OK");

                await Application.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ EXCEPCIÓN: {ex.Message}");

                await Application.Current.MainPage.DisplayAlert(
                    "❌ Error",
                    $"Error al Finalizar:\n{ex.Message}",
                    "OK");
            }
            finally
            {
                IsLoading = false;
            }
        }
        private async Task<byte[]> ComprimirImagen(byte[] imagenOriginal, int calidadJpeg = 80, int anchoMaximo = 1280)
        {
            try
            {
                var skBitmap = SKBitmap.Decode(imagenOriginal);
                if (skBitmap == null) return imagenOriginal;

                // ✅ NUEVO: Leer orientación EXIF
                int rotacionGrados = ObtenerRotacionExif(imagenOriginal);

                // Calcular tamaño según orientación
                // Si la rotación es 90° o 270°, ancho y alto se invierten
                bool estaRotada = rotacionGrados == 90 || rotacionGrados == 270;
                int anchoReal = estaRotada ? skBitmap.Height : skBitmap.Width;
                int altoReal = estaRotada ? skBitmap.Width : skBitmap.Height;

                int nuevoAncho = anchoReal;
                int nuevoAlto = altoReal;

                if (anchoReal > anchoMaximo)
                {
                    float escala = (float)anchoMaximo / anchoReal;
                    nuevoAncho = anchoMaximo;
                    nuevoAlto = (int)(altoReal * escala);
                }

                // Redimensionar
                SKBitmap bitmapRedim = skBitmap;
                if (nuevoAncho != skBitmap.Width || nuevoAlto != skBitmap.Height)
                {
                    // Si está rotada, redimensionar con dimensiones corregidas
                    var infoDestino = estaRotada
                        ? new SKImageInfo(nuevoAlto, nuevoAncho)   // invertidas para rotar después
                        : new SKImageInfo(nuevoAncho, nuevoAlto);

                    bitmapRedim = skBitmap.Resize(infoDestino, SKFilterQuality.Medium);
                }

                // ✅ NUEVO: Aplicar rotación si es necesaria
                SKBitmap bitmapFinal = AplicarRotacion(bitmapRedim, rotacionGrados);

                // Comprimir a JPEG
                using var image = SKImage.FromBitmap(bitmapFinal);
                using var data = image.Encode(SKEncodedImageFormat.Jpeg, calidadJpeg);
                var resultado = data.ToArray();

                System.Diagnostics.Debug.WriteLine(
                    $"🗜️ Compresión: {imagenOriginal.Length / 1024}KB → {resultado.Length / 1024}KB " +
                    $"({100 - (resultado.Length * 100 / imagenOriginal.Length)}% reducción) " +
                    $"| Rotación aplicada: {rotacionGrados}°");

                // Liberar memoria
                if (!ReferenceEquals(bitmapRedim, skBitmap)) bitmapRedim.Dispose();
                if (!ReferenceEquals(bitmapFinal, bitmapRedim)) bitmapFinal.Dispose();
                skBitmap.Dispose();

                return resultado;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Error al comprimir imagen: {ex.Message}");
                return imagenOriginal;
            }
        }

        // ─── Lee la orientación EXIF desde los bytes originales ───────────────────
        private int ObtenerRotacionExif(byte[] imageBytes)
        {
            try
            {
                using var ms = new MemoryStream(imageBytes);
                using var managedStream = new SKManagedStream(ms);
                using var codec = SKCodec.Create(managedStream);

                if (codec == null) return 0;

                return codec.EncodedOrigin switch
                {
                    SKEncodedOrigin.RightTop => 90,   // Portrait normal
                    SKEncodedOrigin.BottomRight => 180,  // Boca abajo
                    SKEncodedOrigin.LeftBottom => 270,  // Portrait invertido
                    _ => 0     // Landscape o sin rotación
                };
            }
            catch
            {
                return 0;
            }
        }

        // ─── Rota el bitmap los grados indicados ──────────────────────────────────
        private SKBitmap AplicarRotacion(SKBitmap bitmap, int grados)
        {
            if (grados == 0) return bitmap;

            // Para 90 y 270, ancho y alto se intercambian
            bool intercambiar = grados == 90 || grados == 270;
            int nuevoAncho = intercambiar ? bitmap.Height : bitmap.Width;
            int nuevoAlto = intercambiar ? bitmap.Width : bitmap.Height;

            var rotado = new SKBitmap(nuevoAncho, nuevoAlto);

            using var canvas = new SKCanvas(rotado);
            canvas.Translate(nuevoAncho / 2f, nuevoAlto / 2f);
            canvas.RotateDegrees(grados);
            canvas.Translate(-bitmap.Width / 2f, -bitmap.Height / 2f);
            canvas.DrawBitmap(bitmap, 0, 0);

            return rotado;
        }
        /*
        private async Task<byte[]> ComprimirImagen(byte[] imagenOriginal, int calidadJpeg = 70, int anchoMaximo = 1280)
        {
            try
            {
                // Decodificar la imagen original
                var skBitmap = SKBitmap.Decode(imagenOriginal);
                if (skBitmap == null) return imagenOriginal;

                // Calcular nuevo tamaño manteniendo proporción
                int nuevoAncho = skBitmap.Width;
                int nuevoAlto = skBitmap.Height;

                if (skBitmap.Width > anchoMaximo)
                {
                    float escala = (float)anchoMaximo / skBitmap.Width;
                    nuevoAncho = anchoMaximo;
                    nuevoAlto = (int)(skBitmap.Height * escala);
                }

                // Redimensionar si es necesario
                SKBitmap bitmapFinal = skBitmap;
                if (nuevoAncho != skBitmap.Width)
                {
                    bitmapFinal = skBitmap.Resize(
                        new SKImageInfo(nuevoAncho, nuevoAlto),
                        SKFilterQuality.Medium
                    );
                }

                // Comprimir a JPEG
                using var image = SKImage.FromBitmap(bitmapFinal);
                using var data = image.Encode(SKEncodedImageFormat.Jpeg, calidadJpeg);
                var resultado = data.ToArray();

                System.Diagnostics.Debug.WriteLine(
                    $"🗜️ Compresión: {imagenOriginal.Length / 1024}KB → {resultado.Length / 1024}KB " +
                    $"({100 - (resultado.Length * 100 / imagenOriginal.Length)}% reducción)");

                // Liberar memoria
                if (!ReferenceEquals(bitmapFinal, skBitmap)) bitmapFinal.Dispose();
                skBitmap.Dispose();

                return resultado;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Error al comprimir imagen: {ex.Message}");
                return imagenOriginal; // Retorna original si falla
            }
        }
        */
        private async Task SubirEvidenciasTrabajo()
        {
            try
            {
                // Recolectar todas las evidencias con imágenes
                var evidenciasConImagen = new List<EvidenciaUpload>();

                // Balatas
                if (MostrarEvidenciaBalatasDelanteras)
                {
                    foreach (var ev in EvidenciasBalatasDelanteras.Where(e => e.TieneImagen && e.ImagenBytes != null))
                    {
                        evidenciasConImagen.Add(new EvidenciaUpload
                        {
                            Descripcion = ev.Descripcion,
                            ImagenBytes = ev.ImagenBytes,
                            NombreArchivo = ev.NombreArchivo ?? "BalatasDelanteras.jpg"
                        });
                    }
                }

                if (MostrarEvidenciaBalatasTraseras)
                {
                    foreach (var ev in EvidenciasBalatasTraseras.Where(e => e.TieneImagen && e.ImagenBytes != null))
                    {
                        evidenciasConImagen.Add(new EvidenciaUpload
                        {
                            Descripcion = ev.Descripcion,
                            ImagenBytes = ev.ImagenBytes,
                            NombreArchivo = ev.NombreArchivo ?? "BalatasTraseras.jpg"
                        });
                    }
                }

                // Aceite Motor
                if (MostrarEvidenciaAceiteMotor)
                {
                    foreach (var ev in EvidenciasAceiteMotor.Where(e => e.TieneImagen && e.ImagenBytes != null))
                    {
                        evidenciasConImagen.Add(new EvidenciaUpload
                        {
                            Descripcion = ev.Descripcion,
                            ImagenBytes = ev.ImagenBytes,
                            NombreArchivo = ev.NombreArchivo ?? "Aceite_motor.jpg"
                        });
                    }
                }

                // Filtro Aceite
                if (MostrarEvidenciaFiltroAceite)
                {
                    foreach (var ev in EvidenciasFiltroAceite.Where(e => e.TieneImagen && e.ImagenBytes != null))
                    {
                        evidenciasConImagen.Add(new EvidenciaUpload
                        {
                            Descripcion = ev.Descripcion,
                            ImagenBytes = ev.ImagenBytes,
                            NombreArchivo = ev.NombreArchivo ?? "Filtro_aceite.jpg"
                        });
                    }
                }

                // Filtro Aire Motor
                if (MostrarEvidenciaFiltroAireMotor)
                {
                    foreach (var ev in EvidenciasFiltroAireMotor.Where(e => e.TieneImagen && e.ImagenBytes != null))
                    {
                        evidenciasConImagen.Add(new EvidenciaUpload
                        {
                            Descripcion = ev.Descripcion,
                            ImagenBytes = ev.ImagenBytes,
                            NombreArchivo = ev.NombreArchivo ?? "Filtro_aire_motor.jpg"
                        });
                    }
                }

                // Filtro Aire Polen
                if (MostrarEvidenciaFiltroAirePolen)
                {
                    foreach (var ev in EvidenciasFiltroAirePolen.Where(e => e.TieneImagen && e.ImagenBytes != null))
                    {
                        evidenciasConImagen.Add(new EvidenciaUpload
                        {
                            Descripcion = ev.Descripcion,
                            ImagenBytes = ev.ImagenBytes,
                            NombreArchivo = ev.NombreArchivo ?? "Filtro_aire_polen.jpg"
                        });
                    }
                }

                // Solo subir si hay evidencias
                if (evidenciasConImagen.Any())
                {
                    var resultado = await _apiService.SubirEvidenciasTrabajo(
                        CheckList.OrdenId,
                        evidenciasConImagen
                    );

                    if (!resultado.Success)
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ Advertencia: {resultado.Message}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"✅ Evidencias subidas correctamente");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error al subir evidencias: {ex.Message}");
                // No bloqueamos el guardado del checklist si falla la subida de evidencias
            }
        }

        private bool ValidarCheckList()
        {
            if (string.IsNullOrWhiteSpace(CheckList.Bieletas) ||
                string.IsNullOrWhiteSpace(CheckList.Terminales) ||
                string.IsNullOrWhiteSpace(CheckList.CajaDireccion) ||
                string.IsNullOrWhiteSpace(CheckList.Volante) ||
                string.IsNullOrWhiteSpace(CheckList.AmortiguadoresDelanteros) ||
                string.IsNullOrWhiteSpace(CheckList.AmortiguadoresTraseros) ||
                string.IsNullOrWhiteSpace(CheckList.BarraEstabilizadora) ||
                string.IsNullOrWhiteSpace(CheckList.Horquillas) ||
                string.IsNullOrWhiteSpace(CheckList.NeumaticosDelanteros) ||
                string.IsNullOrWhiteSpace(CheckList.NeumaticosTraseros) ||
                string.IsNullOrWhiteSpace(CheckList.Balanceo) ||
                string.IsNullOrWhiteSpace(CheckList.Alineacion) ||
                string.IsNullOrWhiteSpace(CheckList.LucesAltas) ||
                string.IsNullOrWhiteSpace(CheckList.LucesBajas) ||
                string.IsNullOrWhiteSpace(CheckList.LucesAntiniebla) ||
                string.IsNullOrWhiteSpace(CheckList.LucesReversa) ||
                string.IsNullOrWhiteSpace(CheckList.LucesDireccionales) ||
                string.IsNullOrWhiteSpace(CheckList.LucesIntermitentes) ||
                string.IsNullOrWhiteSpace(CheckList.DiscosTamboresDelanteros) ||
                string.IsNullOrWhiteSpace(CheckList.DiscosTamboresTraseros) ||
                string.IsNullOrWhiteSpace(CheckList.BalatasDelanteras) ||
                string.IsNullOrWhiteSpace(CheckList.BalatasTraseras))
            {
                return false;
            }

            return true;
        }

        private bool ValidarTrabajos()
        {
            if (CheckList.NivelLiquidoFrenos == false ||
                CheckList.NivelAnticongelante == false ||
                CheckList.NivelDepositoLimpiaparabrisas == false ||
                CheckList.NivelAceiteMotor == false ||
                CheckList.DescristalizacionTamboresDiscos == false ||
                CheckList.AjusteFrenos == false ||
                CheckList.CalibracionPresionNeumaticos == false ||
                CheckList.TorqueNeumaticos == false)
            {
                return false;
            }

            return true;
        }

        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}