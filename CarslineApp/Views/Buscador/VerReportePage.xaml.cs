using System.Net;
using CarslineApp.Services;

namespace CarslineApp.Views.Buscador;

public partial class VerReportePage : ContentPage
{
    private readonly ApiService _apiService;
    private bool _isLoading;
    private int _ordenId;

    // Ruta del PDF en caché (disponible tras carga)
    private string _pdfCachedPath;

    public VerReportePage(int ordenId)
    {
        InitializeComponent();
        _ordenId = ordenId;
        _apiService = new ApiService();

#if ANDROID
        ConfigurarWebViewAndroid();
#endif

        _ = CargarPdfAsync();
    }

    // ??? BOTONES DE NAVEGACIÓN ???

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    // ??? BOTONES DEL FOOTER ???

    private async void OnShareClicked(object sender, EventArgs e)
    {
        if (_pdfCachedPath == null || !File.Exists(_pdfCachedPath))
        {
            await DisplayAlert("Aviso", "El PDF aún no está disponible.", "OK");
            return;
        }

        try
        {
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Compartir PDF",
                File = new ShareFile(_pdfCachedPath)
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo compartir el PDF: {ex.Message}", "OK");
        }
    }

    private async void OnDownloadClicked(object sender, EventArgs e)
    {
        if (_pdfCachedPath == null || !File.Exists(_pdfCachedPath))
        {
            await DisplayAlert("Aviso", "El PDF aún no está disponible.", "OK");
            return;
        }

        // ? Confirmación antes de descargar
        bool confirmar = await DisplayAlert(
            "Descargar PDF",
            "¿Deseas guardar el PDF en tu dispositivo?",
            "Descargar",
            "Cancelar");

        if (!confirmar) return;

        try
        {
            LoadingIndicator.IsVisible = true;

#if ANDROID
            await GuardarPdfAndroidAsync(_pdfCachedPath);
#elif IOS
            await GuardarPdfIOSAsync(_pdfCachedPath);
#elif WINDOWS
            await GuardarPdfWindowsAsync(_pdfCachedPath);
#else
            await DisplayAlert("Info", "Descarga no soportada en esta plataforma.", "OK");
#endif
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo guardar el PDF: {ex.Message}", "OK");
        }
        finally
        {
            LoadingIndicator.IsVisible = false;
        }
    }

    private async void OnOpenWithClicked(object sender, EventArgs e)
    {
        if (_pdfCachedPath == null || !File.Exists(_pdfCachedPath))
        {
            await DisplayAlert("Aviso", "El PDF aún no está disponible.", "OK");
            return;
        }

        try
        {
            await Launcher.Default.OpenAsync(
                new OpenFileRequest
                {
                    Title = "Abrir PDF con...",
                    File = new ReadOnlyFile(_pdfCachedPath)
                });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo abrir el PDF: {ex.Message}", "OK");
        }
    }

    private async Task OnSharePdfAsync(string title)
    {
        if (_pdfCachedPath == null || !File.Exists(_pdfCachedPath))
        {
            await DisplayAlert("Aviso", "El PDF aún no está disponible.", "OK");
            return;
        }

        try
        {
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = title,
                File = new ShareFile(_pdfCachedPath)
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo procesar la acción: {ex.Message}", "OK");
        }
    }

#if ANDROID
    private void ConfigurarWebViewAndroid()
    {
        Microsoft.Maui.Handlers.WebViewHandler.Mapper.AppendToMapping("pdfviewer", (handler, View) =>
        {
            handler.PlatformView.Settings.AllowFileAccess = true;
            handler.PlatformView.Settings.AllowFileAccessFromFileURLs = true;
            handler.PlatformView.Settings.AllowUniversalAccessFromFileURLs = true;
            handler.PlatformView.Settings.JavaScriptEnabled = true;
        });
    }
#endif

    private async Task CargarPdfAsync()
    {
        try
        {
            _isLoading = true;
            LoadingIndicator.IsVisible = true;

            var response = await _apiService.ObtenerVistaPreviaPdfAsync(_ordenId);

            if (!response.Success || string.IsNullOrEmpty(response.PdfBase64))
            {
                await DisplayAlert("Error", response.Message ?? "No se pudo obtener el PDF", "OK");
                return;
            }

            byte[] pdfBytes = Convert.FromBase64String(response.PdfBase64);

            string fileName = string.IsNullOrEmpty(response.NumeroOrden)
                ? $"Orden_{_ordenId}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
                : $"Orden_{response.NumeroOrden}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            await GuardarYMostrarPdfAsync(pdfBytes, fileName);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Error al cargar el PDF: {ex.Message}", "OK");
            System.Diagnostics.Debug.WriteLine($"Error en CargarPdfAsync: {ex}");
        }
        finally
        {
            _isLoading = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private async Task GuardarYMostrarPdfAsync(byte[] pdfBytes, string fileName)
    {
#if ANDROID
        await GuardarYMostrarPdfAndroidAsync(pdfBytes, fileName);
#elif IOS || MACCATALYST
        await GuardarYMostrarPdfIOSAsync(pdfBytes, fileName);
#elif WINDOWS
        await GuardarYMostrarPdfWindowsAsync(pdfBytes, fileName);
#else
        await GuardarYMostrarPdfGenericoAsync(pdfBytes, fileName);
#endif
    }

#if ANDROID
    private async Task GuardarYMostrarPdfAndroidAsync(byte[] pdfBytes, string fileName)
    {
        try
        {
            string cachePath = FileSystem.Current.CacheDirectory;
            string filePath = Path.Combine(cachePath, fileName);

            await File.WriteAllBytesAsync(filePath, pdfBytes);

            // Guardar ruta para botones de acción
            _pdfCachedPath = filePath;

            string pdfUrl = $"file://{filePath}";
            string viewerUrl = $"file:///android_asset/pdfjs/web/viewer.html?file={WebUtility.UrlEncode(pdfUrl)}";

            pdfview.Source = viewerUrl;

            System.Diagnostics.Debug.WriteLine($"PDF cargado desde: {viewerUrl}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en GuardarYMostrarPdfAndroidAsync: {ex}");
            await DisplayAlert("Error", $"No se pudo mostrar el PDF en Android: {ex.Message}", "OK");
        }
    }

    private async Task GuardarPdfAndroidAsync(string sourcePath)
    {
        try
        {
            var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Permiso denegado", "Se necesita acceso al almacenamiento.", "OK");
                return;
            }

            string docsPath = Android.OS.Environment.GetExternalStoragePublicDirectory(
                Android.OS.Environment.DirectoryDownloads).AbsolutePath;

            string destFolder = Path.Combine(docsPath, "Carsline");
            Directory.CreateDirectory(destFolder);

            string destPath = Path.Combine(destFolder, Path.GetFileName(sourcePath));
            File.Copy(sourcePath, destPath, overwrite: true);

            // Notificar al sistema de archivos
            var mediaScanIntent = new Android.Content.Intent(
                Android.Content.Intent.ActionMediaScannerScanFile);
            mediaScanIntent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(destPath)));
            Android.App.Application.Context.SendBroadcast(mediaScanIntent);

            await DisplayAlert("? Guardado", "PDF guardado en:\nDescargas/Carsline", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
#endif

#if IOS || MACCATALYST
    private async Task GuardarYMostrarPdfIOSAsync(byte[] pdfBytes, string fileName)
    {
        try
        {
            string cachePath = FileSystem.Current.CacheDirectory;
            string filePath = Path.Combine(cachePath, fileName);

            await File.WriteAllBytesAsync(filePath, pdfBytes);

            _pdfCachedPath = filePath;

            pdfview.Source = filePath;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en GuardarYMostrarPdfIOSAsync: {ex}");
            await DisplayAlert("Error", $"No se pudo mostrar el PDF en iOS: {ex.Message}", "OK");
        }
    }

    private async Task GuardarPdfIOSAsync(string sourcePath)
    {
        // En iOS usamos el share sheet nativo que incluye "Guardar en Archivos"
        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Guardar PDF",
            File = new ShareFile(sourcePath)
        });
    }
#endif

#if WINDOWS
    private async Task GuardarYMostrarPdfWindowsAsync(byte[] pdfBytes, string fileName)
    {
        try
        {
            string cachePath = FileSystem.Current.CacheDirectory;
            string filePath = Path.Combine(cachePath, fileName);

            await File.WriteAllBytesAsync(filePath, pdfBytes);

            _pdfCachedPath = filePath;

            pdfview.Source = new Uri(filePath).AbsoluteUri;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en GuardarYMostrarPdfWindowsAsync: {ex}");
            await DisplayAlert("Error", $"No se pudo mostrar el PDF en Windows: {ex.Message}", "OK");
        }
    }

    private async Task GuardarPdfWindowsAsync(string sourcePath)
    {
        try
        {
            var picker = new Windows.Storage.Pickers.FileSavePicker();

            var hwnd = ((MauiWinUIWindow)Application.Current.Windows[0].Handler.PlatformView).WindowHandle;
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.SuggestedFileName = Path.GetFileNameWithoutExtension(sourcePath);
            picker.FileTypeChoices.Add("Documento PDF", new List<string> { ".pdf" });

            var file = await picker.PickSaveFileAsync();
            if (file == null) return;

            using var sourceStream = File.OpenRead(sourcePath);
            using var destStream = await file.OpenStreamForWriteAsync();
            await sourceStream.CopyToAsync(destStream);

            await DisplayAlert("? Guardado", $"PDF guardado en:\n{file.Path}", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
#endif

    private async Task GuardarYMostrarPdfGenericoAsync(byte[] pdfBytes, string fileName)
    {
        try
        {
            string cachePath = FileSystem.Current.CacheDirectory;
            string filePath = Path.Combine(cachePath, fileName);

            await File.WriteAllBytesAsync(filePath, pdfBytes);

            _pdfCachedPath = filePath;

            pdfview.Source = filePath;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error en GuardarYMostrarPdfGenericoAsync: {ex}");
            await DisplayAlert("Error", $"No se pudo mostrar el PDF: {ex.Message}", "OK");
        }
    }

    // ??? LIMPIEZA ???

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _ = LimpiarArchivosTemporalesAsync();
    }

    private async Task LimpiarArchivosTemporalesAsync()
    {
        await Task.Run(() =>
        {
            try
            {
                string folderPath = FileSystem.Current.CacheDirectory;
                var files = Directory.GetFiles(folderPath, "Orden_*.pdf");

                foreach (var file in files)
                {
                    try
                    {
                        var fileInfo = new FileInfo(file);
                        if (DateTime.Now - fileInfo.CreationTime > TimeSpan.FromHours(24))
                        {
                            File.Delete(file);
                            System.Diagnostics.Debug.WriteLine($"Archivo temporal eliminado: {file}");
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error al eliminar archivo {file}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al limpiar archivos temporales: {ex.Message}");
            }
        });
    }
}