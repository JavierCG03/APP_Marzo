using Microsoft.Maui.Controls;
using CarslineApp.ViewModels.Avaluo;

namespace CarslineApp.Views.Avaluo;

public partial class AvaluoImagePreviewPage : ContentPage
{
    // ?? Zoom / Pan (idéntico a ImagePreviewPage) ???????????????????
    private double _zoom = 1.0;
    private double _zoomStart = 1.0;
    private double _xOff = 0;
    private double _yOff = 0;
    private bool _animando = false;

    private const double ZOOM_MIN = 1.0;
    private const double ZOOM_MAX = 4.0;
    private const double ZOOM_STEP = 0.50;

    // ?? Datos de la foto ???????????????????????????????????????????
    private readonly byte[] _bytesOriginales;
    private byte[] _bytesActuales;   // puede ser editada
    private readonly string _descripcion;

    public AvaluoImagePreviewPage(byte[] imageBytes, string descripcion)
    {
        InitializeComponent();
        _bytesOriginales = imageBytes;
        _bytesActuales = imageBytes;
        _descripcion = descripcion;

        TituloLabel.Text = descripcion;
        PreviewImage.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
    }

    // ??????????????????????????????????????????????????????????????
    // EDITAR — abre el editor y actualiza el preview si se guarda
    // ??????????????????????????????????????????????????????????????
    private async void OnEditarClicked(object sender, EventArgs e)
    {
        var editorPage = new AvaluoImageEditorPage(_bytesActuales, _descripcion);
        editorPage.ImagenEditada += (bytesEditados) =>
        {
            _bytesActuales = bytesEditados;
            PreviewImage.Source = ImageSource.FromStream(() => new MemoryStream(bytesEditados));
        };
        await Navigation.PushModalAsync(editorPage);
    }

    // ??????????????????????????????????????????????????????????????
    // COMPARTIR / DESCARGAR (mismo patrón que ImagePreviewPage)
    // ??????????????????????????????????????????????????????????????

    private async void OnShareClicked(object sender, EventArgs e)
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            string path = await GuardarTemporalAsync();
            if (path == null) return;
            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = "Compartir foto avalúo",
                File = new ShareFile(path)
            });
        }
        catch (Exception ex) { await DisplayAlert("Error", ex.Message, "OK"); }
        finally { LoadingIndicator.IsVisible = false; LoadingIndicator.IsRunning = false; }
    }

    private async void OnDownloadClicked(object sender, EventArgs e)
    {
        bool ok = await DisplayAlert("Guardar foto", "¿Guardar la imagen en el dispositivo?",
            "Guardar", "Cancelar");
        if (!ok) return;

        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            string path = await GuardarTemporalAsync();
            if (path == null) return;

#if ANDROID
            await GuardarEnAndroidAsync(path);
#elif IOS
            await GuardarEniOSAsync(path);
#elif WINDOWS
            await GuardarEnWindowsAsync(path);
#else
            await DisplayAlert("Info", "Descarga no disponible en esta plataforma.", "OK");
#endif
        }
        catch (Exception ex) { await DisplayAlert("Error", ex.Message, "OK"); }
        finally { LoadingIndicator.IsVisible = false; LoadingIndicator.IsRunning = false; }
    }

    private async Task<string?> GuardarTemporalAsync()
    {
        try
        {
            string nombre = $"avaluo_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
            string ruta = Path.Combine(FileSystem.CacheDirectory, nombre);
            await File.WriteAllBytesAsync(ruta, _bytesActuales);
            return ruta;
        }
        catch { await DisplayAlert("Error", "No se pudo procesar la imagen.", "OK"); return null; }
    }

#if ANDROID
    private async Task GuardarEnAndroidAsync(string tempPath)
    {
        var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
        if (status != PermissionStatus.Granted)
        { await DisplayAlert("Permiso denegado", "Se necesita acceso al almacenamiento.", "OK"); return; }

        string picDir  = Android.OS.Environment.GetExternalStoragePublicDirectory(
            Android.OS.Environment.DirectoryPictures).AbsolutePath;
        string destDir = Path.Combine(picDir, "Carsline");
        Directory.CreateDirectory(destDir);
        string dest = Path.Combine(destDir, Path.GetFileName(tempPath));
        File.Copy(tempPath, dest, true);

        var intent = new Android.Content.Intent(Android.Content.Intent.ActionMediaScannerScanFile);
        intent.SetData(Android.Net.Uri.FromFile(new Java.IO.File(dest)));
        Android.App.Application.Context.SendBroadcast(intent);

        await DisplayAlert("? Guardado", "Imagen guardada en Pictures/Carsline", "OK");
    }
#endif

#if IOS
    private async Task GuardarEniOSAsync(string tempPath)
    {
        var status = await Permissions.RequestAsync<Permissions.Photos>();
        if (status != PermissionStatus.Granted)
        { await DisplayAlert("Permiso denegado", "Se necesita acceso a Fotos.", "OK"); return; }

        var image = UIKit.UIImage.FromFile(tempPath);
        if (image == null) { await DisplayAlert("Error", "No se pudo cargar la imagen.", "OK"); return; }

        var tcs = new TaskCompletionSource<bool>();
        image.SaveToPhotosAlbum((img, err) => tcs.SetResult(err == null));
        bool ok = await tcs.Task;
        await DisplayAlert(ok ? "? Guardado" : "Error",
            ok ? "Imagen guardada en tu carrete." : "No se pudo guardar.", "OK");
    }
#endif

#if WINDOWS
    private async Task GuardarEnWindowsAsync(string tempPath)
    {
        var picker = new Windows.Storage.Pickers.FileSavePicker();
        var hwnd = ((MauiWinUIWindow)Application.Current.Windows[0].Handler.PlatformView).WindowHandle;
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
        picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
        picker.SuggestedFileName = Path.GetFileNameWithoutExtension(tempPath);
        picker.FileTypeChoices.Add("JPEG", new List<string> { ".jpg" });
        var file = await picker.PickSaveFileAsync();
        if (file == null) return;
        using var src = File.OpenRead(tempPath);
        using var dst = await file.OpenStreamForWriteAsync();
        await src.CopyToAsync(dst);
        await DisplayAlert("? Guardado", "Imagen guardada correctamente.", "OK");
    }
#endif

    // ??????????????????????????????????????????????????????????????
    // ZOOM / PAN (igual que ImagePreviewPage)
    // ??????????????????????????????????????????????????????????????

    private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
    {
        switch (e.Status)
        {
            case GestureStatus.Started:
                _zoomStart = _zoom;
                PreviewImage.AnchorX = e.ScaleOrigin.X;
                PreviewImage.AnchorY = e.ScaleOrigin.Y;
                break;

            case GestureStatus.Running:
                double d = Math.Abs(e.Scale - 1.0);
                double fac = d < 0.25 ? 1.25 : 1.5;
                double t = e.Scale > 1.0
                    ? _zoomStart * (1 + (e.Scale - 1) * fac)
                    : _zoomStart * (1 - (1 - e.Scale) * fac);

                _zoom = Math.Clamp(t, ZOOM_MIN, ZOOM_MAX);
                PreviewImage.Scale = _zoom;
                ActualizarEtiqueta();
                break;

            case GestureStatus.Completed:
                _xOff = PreviewImage.TranslationX;
                _yOff = PreviewImage.TranslationY;
                break;
        }
    }

    private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
    {
        if (_zoom <= 1.0 || _animando) return;

        switch (e.StatusType)
        {
            case GestureStatus.Running:
                double nx = _xOff + e.TotalX;
                double ny = _yOff + e.TotalY;
                ClampPan(ref nx, ref ny);
                PreviewImage.TranslationX = nx;
                PreviewImage.TranslationY = ny;
                break;

            case GestureStatus.Completed:
                _xOff = PreviewImage.TranslationX;
                _yOff = PreviewImage.TranslationY;
                _ = SnapBordesAsync();
                break;
        }
    }

    private async void OnDoubleTap(object sender, TappedEventArgs e)
    {
        if (_animando) return;
        await AnimarZoomAsync(_zoom > 1.5 ? 1.0 : 2.0);
    }

    private async void OnBackClicked(object sender, EventArgs e) => await Navigation.PopModalAsync();
    private async void OnZoomInClicked(object sender, EventArgs e) { if (!_animando && _zoom < ZOOM_MAX) await AnimarZoomAsync(Math.Min(_zoom + ZOOM_STEP, ZOOM_MAX)); }
    private async void OnZoomOutClicked(object sender, EventArgs e) { if (!_animando && _zoom > ZOOM_MIN) await AnimarZoomAsync(Math.Max(_zoom - ZOOM_STEP, ZOOM_MIN)); }
    private async void OnResetClicked(object sender, EventArgs e) => await ResetZoomAsync();

    private async Task AnimarZoomAsync(double target)
    {
        if (_animando) return; _animando = true;
        try
        {
            PreviewImage.AnchorX = 0.5; PreviewImage.AnchorY = 0.5;
            await PreviewImage.ScaleTo(target, 200, Easing.CubicInOut);
            _zoom = target; ActualizarEtiqueta();
            if (Math.Abs(target - 1.0) < 0.01)
            {
                await PreviewImage.TranslateTo(0, 0, 200, Easing.CubicOut);
                _xOff = _yOff = 0;
            }
        }
        finally { _animando = false; }
    }

    private async Task ResetZoomAsync()
    {
        if (_animando) return; _animando = true;
        try
        {
            _zoom = 1; _xOff = _yOff = 0;
            await Task.WhenAll(
                PreviewImage.ScaleTo(1, 250, Easing.CubicOut),
                PreviewImage.TranslateTo(0, 0, 250, Easing.CubicOut));
            ActualizarEtiqueta();
        }
        finally { _animando = false; }
    }

    private async Task SnapBordesAsync()
    {
        const double snap = 20;
        double tx = Math.Abs(_xOff) < snap ? 0 : _xOff;
        double ty = Math.Abs(_yOff) < snap ? 0 : _yOff;
        if (Math.Abs(tx - _xOff) > 0.1 || Math.Abs(ty - _yOff) > 0.1)
        {
            await PreviewImage.TranslateTo(tx, ty, 150, Easing.CubicOut);
            _xOff = tx; _yOff = ty;
        }
    }

    private void ClampPan(ref double x, ref double y)
    {
        double ex = (PreviewImage.Width * _zoom - ImageContainer.Width) / 2;
        double ey = (PreviewImage.Height * _zoom - ImageContainer.Height) / 2;
        x = Math.Clamp(x, -Math.Max(0, ex), Math.Max(0, ex));
        y = Math.Clamp(y, -Math.Max(0, ey), Math.Max(0, ey));
    }

    private void ActualizarEtiqueta()
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            ZoomLabel.Text = $"{(int)(_zoom * 100)}%";
            ZoomLabel.TextColor = _zoom > 1.0 ? Color.FromArgb("#00FF00") : Color.FromArgb("#00D4FF");
        });
    }
}