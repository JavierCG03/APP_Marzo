
namespace CarslineApp.Views
{
    public partial class ImagePreviewPage : ContentPage
    {
        // Estados de zoom y pan
        private double currentZoom = 1.0;
        private double startZoom = 1.0;
        private double xOffset = 0;
        private double yOffset = 0;

        // Límites
        private const double MIN_ZOOM = 1.0;
        private const double MAX_ZOOM = 4.0;
        private const double ZOOM_STEP = 0.50;

        private bool isAnimating = false;

        private readonly ImageSource _imageSource;
        public ImagePreviewPage(ImageSource imagen)
        {
            InitializeComponent();
            _imageSource = imagen;
            PreviewImage.Source = imagen;
        }
        private async void OnShareClicked(object sender, EventArgs e)
        {
            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                string tempPath = await GuardarImagenTemporalAsync();
                if (tempPath == null) return;

                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "Compartir imagen",
                    File = new ShareFile(tempPath)
                });
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo compartir la imagen: {ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }


        private async void OnDownloadClicked(object sender, EventArgs e)
        {
            try
            {

                // ✅ Confirmación antes de descargar
                bool confirmar = await DisplayAlert(
                    "Descargar Fotografia",
                    "¿Deseas guardar la imagen en el dispositivo?",
                    "Descargar",
                    "Cancelar");

                if (!confirmar) return;

                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                string tempPath = await GuardarImagenTemporalAsync();
                if (tempPath == null) return;

#if ANDROID
                await GuardarEnAndroidAsync(tempPath);
#elif IOS
                await GuardarEniOSAsync(tempPath);
#elif WINDOWS
                await GuardarEnWindowsAsync(tempPath);
#else
                await DisplayAlert("Info", "Descarga no soportada en esta plataforma.", "OK");
#endif
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo guardar la imagen: {ex.Message}", "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }

        // ═══ HELPERS DE PLATAFORMA ═══

        /// <summary>
        /// Convierte el ImageSource a un archivo temporal .jpg
        /// </summary>
        private async Task<string> GuardarImagenTemporalAsync()
        {
            try
            {
                string tempDir = FileSystem.CacheDirectory;
                string fileName = $"carsline_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                string tempPath = Path.Combine(tempDir, fileName);

                // Obtener stream según el tipo de ImageSource
                Stream stream = null;

                if (_imageSource is StreamImageSource streamSource)
                {
                    stream = await streamSource.Stream(CancellationToken.None);
                }
                else if (_imageSource is FileImageSource fileSource)
                {
                    stream = File.OpenRead(fileSource.File);
                }
                else if (_imageSource is UriImageSource uriSource)
                {
                    using var httpClient = new HttpClient();
                    stream = await httpClient.GetStreamAsync(uriSource.Uri);
                }

                if (stream == null)
                {
                    await DisplayAlert("Error", "No se pudo leer la imagen.", "OK");
                    return null;
                }

                using (stream)
                using (var fileStream = File.Create(tempPath))
                {
                    await stream.CopyToAsync(fileStream);
                }

                return tempPath;
            }
            catch
            {
                await DisplayAlert("Error", "No se pudo procesar la imagen.", "OK");
                return null;
            }
        }

#if ANDROID
        private async Task GuardarEnAndroidAsync(string tempPath)
        {
            // Android 10+ usa MediaStore; anteriores usan carpeta Pictures directamente
            try
            {
                var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Permiso denegado", "Se necesita acceso al almacenamiento.", "OK");
                    return;
                }

                string picturesPath = Android.OS.Environment.GetExternalStoragePublicDirectory(
                    Android.OS.Environment.DirectoryPictures).AbsolutePath;

                string destFolder = Path.Combine(picturesPath, "Carsline");
                Directory.CreateDirectory(destFolder);

                string destPath = Path.Combine(destFolder, Path.GetFileName(tempPath));
                File.Copy(tempPath, destPath, overwrite: true);

                // Notificar a la galería
                var mediaScanIntent = new Android.Content.Intent(
                    Android.Content.Intent.ActionMediaScannerScanFile);
                mediaScanIntent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(destPath)));
                Android.App.Application.Context.SendBroadcast(mediaScanIntent);

                await DisplayAlert("✅ Guardado", $"Imagen guardada en:\nPictures/Carsline", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
#endif

#if IOS
        private async Task GuardarEniOSAsync(string tempPath)
        {
            // Guarda directo en el carrete de fotos (Photo Library)
            var status = await Permissions.RequestAsync<Permissions.Photos>();
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Permiso denegado", "Se necesita acceso a Fotos.", "OK");
                return;
            }

            UIKit.UIImage image = UIKit.UIImage.FromFile(tempPath);
            if (image == null)
            {
                await DisplayAlert("Error", "No se pudo cargar la imagen.", "OK");
                return;
            }

            var tcs = new TaskCompletionSource<bool>();
            image.SaveToPhotosAlbum((img, error) =>
            {
                tcs.SetResult(error == null);
            });

            bool success = await tcs.Task;
            await DisplayAlert(
                success ? "✅ Guardado" : "Error",
                success ? "Imagen guardada en tu carrete de fotos." : "No se pudo guardar.",
                "OK");
        }
#endif

#if WINDOWS
        private async Task GuardarEnWindowsAsync(string tempPath)
        {
            // Abre el diálogo nativo de "Guardar como" en Windows
            var picker = new Windows.Storage.Pickers.FileSavePicker();

            // Asociar al hwnd de la ventana actual (requisito WinUI3)
            var hwnd = ((MauiWinUIWindow)Application.Current.Windows[0].Handler.PlatformView).WindowHandle;
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.SuggestedFileName = Path.GetFileNameWithoutExtension(tempPath);
            picker.FileTypeChoices.Add("Imagen JPEG", new List<string> { ".jpg" });
            picker.FileTypeChoices.Add("Imagen PNG", new List<string> { ".png" });

            var file = await picker.PickSaveFileAsync();
            if (file == null) return; // Usuario canceló

            using var sourceStream = File.OpenRead(tempPath);
            using var destStream = await file.OpenStreamForWriteAsync();
            await sourceStream.CopyToAsync(destStream);

            await DisplayAlert("✅ Guardado", $"Imagen guardada en:\n{file.Path}", "OK");
        }
#endif
        private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            if (isAnimating) return;

            switch (e.Status)
            {
                case GestureStatus.Started:
                    startZoom = currentZoom;

                    // Zoom centrado en los dedos
                    PreviewImage.AnchorX = e.ScaleOrigin.X;
                    PreviewImage.AnchorY = e.ScaleOrigin.Y;
                    break;

                case GestureStatus.Running:
                    double delta = Math.Abs(e.Scale - 1.0);

                    // Factores según intensidad del gesto
                    double zoomFactor =
                        delta < 0.25 ? 1.25 : 1.5;

                    double targetZoom;

                    if (e.Scale > 1.0)
                    {
                        // Zoom IN
                        targetZoom = startZoom * (1 + (e.Scale - 1) * zoomFactor);
                    }
                    else
                    {
                        // Zoom OUT
                        targetZoom = startZoom * (1 - (1 - e.Scale) * zoomFactor);
                    }

                    targetZoom = Math.Clamp(targetZoom, MIN_ZOOM, MAX_ZOOM);

                    if (Math.Abs(targetZoom - currentZoom) < 0.01)
                        return;

                    currentZoom = targetZoom;
                    PreviewImage.Scale = currentZoom;

                    UpdateZoomLabel();
                    break;

                case GestureStatus.Completed:
                    xOffset = PreviewImage.TranslationX;
                    yOffset = PreviewImage.TranslationY;
                    break;
            }
        }

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (currentZoom <= 1.0 || isAnimating) return;


            switch (e.StatusType)
            {
                case GestureStatus.Started:

                    break;

                case GestureStatus.Running:


                    double newX = xOffset + e.TotalX;
                    double newY = yOffset + e.TotalY;

                    ClampPanBounds(ref newX, ref newY);

                    PreviewImage.TranslationX = newX;
                    PreviewImage.TranslationY = newY;
                    break;

                case GestureStatus.Completed:
                    xOffset = PreviewImage.TranslationX;
                    yOffset = PreviewImage.TranslationY;

                    // Snap a bordes
                    _ = SnapToEdgesAsync();
                    break;
            }
        }

        private async void OnDoubleTap(object sender, TappedEventArgs e)
        {
            if (isAnimating) return;

            // Alternar entre zoom base y 2x
            double targetZoom = currentZoom > 1.5 ? 1.0 : 2.0;
            await AnimateZoomAsync(targetZoom);
        }

        // ═══ BOTONES DEL HEADER ═══

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void OnZoomInClicked(object sender, EventArgs e)
        {
            if (isAnimating || currentZoom >= MAX_ZOOM) return;

            double targetZoom = Math.Min(currentZoom + ZOOM_STEP, MAX_ZOOM);
            await AnimateZoomAsync(targetZoom);
        }

        private async void OnZoomOutClicked(object sender, EventArgs e)
        {
            if (isAnimating || currentZoom <= MIN_ZOOM) return;

            double targetZoom = Math.Max(currentZoom - ZOOM_STEP, MIN_ZOOM);
            await AnimateZoomAsync(targetZoom);
        }

        // ═══ BOTONES DEL FOOTER ═══

        private async void OnResetClicked(object sender, EventArgs e)
        {
            await ResetZoomAsync();
        }

        private async void OnFitClicked(object sender, EventArgs e)
        {
            await ResetZoomAsync();
        }

        private async void OnInfoClicked(object sender, EventArgs e)
        {
            await DisplayAlert(
                "Información",
                $"Zoom: {(int)(currentZoom * 100)}%\n" +
                $"Posición X: {(int)xOffset}px\n" +
                $"Posición Y: {(int)yOffset}px",
                "OK"
            );
        }



        // ═══ ANIMACIONES ═══

        private async Task AnimateZoomAsync(double targetZoom)
        {
            if (isAnimating) return;
            isAnimating = true;

            try
            {
                // Asegurar que la imagen esté centrada durante zoom
                PreviewImage.AnchorX = 0.5;
                PreviewImage.AnchorY = 0.5;

                await PreviewImage.ScaleTo(targetZoom, 200, Easing.CubicInOut);
                currentZoom = targetZoom;
                UpdateZoomLabel();

                // Si reseteamos, también limpiar traducción
                if (Math.Abs(targetZoom - 1.0) < 0.01)
                {
                    await PreviewImage.TranslateTo(0, 0, 200, Easing.CubicOut);
                    xOffset = 0;
                    yOffset = 0;
                }
            }
            finally
            {
                isAnimating = false;
            }
        }

        private async Task ResetZoomAsync()
        {
            if (isAnimating) return;
            isAnimating = true;

            try
            {
                currentZoom = 1.0;
                xOffset = 0;
                yOffset = 0;

                await Task.WhenAll(
                    PreviewImage.ScaleTo(1.0, 250, Easing.CubicOut),
                    PreviewImage.TranslateTo(0, 0, 250, Easing.CubicOut)
                );

                UpdateZoomLabel();
            }
            finally
            {
                isAnimating = false;
            }
        }

        private async Task SnapToEdgesAsync()
        {
            const double snapDistance = 20;

            double targetX = Math.Abs(xOffset) < snapDistance ? 0 : xOffset;
            double targetY = Math.Abs(yOffset) < snapDistance ? 0 : yOffset;

            if (Math.Abs(targetX - xOffset) > 0.1 || Math.Abs(targetY - yOffset) > 0.1)
            {
                await PreviewImage.TranslateTo(targetX, targetY, 150, Easing.CubicOut);
                xOffset = targetX;
                yOffset = targetY;
            }
        }
 
        private void ClampPanBounds(ref double x, ref double y)
        {
            var container = this.Content as Layout;
            double containerWidth = ImageContainer.Width;   // Referencia directa al Grid/Layout
            double containerHeight = ImageContainer.Height;

            double excess_x = (PreviewImage.Width * currentZoom - containerWidth) / 2;
            double excess_y = (PreviewImage.Height * currentZoom - containerHeight) / 2;

            x = Math.Clamp(x, -Math.Max(0, excess_x), Math.Max(0, excess_x));
            y = Math.Clamp(y, -Math.Max(0, excess_y), Math.Max(0, excess_y));
        }

        // ═══ UTILIDADES ═══

        private void UpdateZoomLabel()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                int percentage = (int)(currentZoom * 100);
                ZoomLabel.Text = $"{percentage}%";

                // Cambiar color según nivel de zoom
                ZoomLabel.TextColor = currentZoom > 1.0
                    ? Color.FromArgb("#00FF00")
                    : Color.FromArgb("#00D4FF");
            });
        }
    }
}