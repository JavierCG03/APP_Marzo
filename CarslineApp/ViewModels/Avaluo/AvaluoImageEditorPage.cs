using Microsoft.Maui.Controls.Shapes;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;


namespace CarslineApp.ViewModels.Avaluo;


public enum HerramientaDibujo { Lapiz, Circulo, Flecha, Texto }

public abstract class Trazo
{
    public SKColor Color { get; set; }
    public float Grosor { get; set; }
    public bool EsPreview { get; set; }
    public abstract void Dibujar(SKCanvas canvas);
}

public class TrazoCurva : Trazo
{
    public List<SKPoint> Puntos { get; } = new();
    public override void Dibujar(SKCanvas canvas)
    {
        if (Puntos.Count < 2) return;
        using var paint = new SKPaint
        {
            Color = Color,
            StrokeWidth = Grosor,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round
        };
        using var path = new SKPath();
        path.MoveTo(Puntos[0]);
        for (int i = 1; i < Puntos.Count; i++)
            path.LineTo(Puntos[i]);
        canvas.DrawPath(path, paint);
    }
}

public class TrazoCirculo : Trazo
{
    public SKPoint Centro { get; set; }
    public float Radio { get; set; }
    public override void Dibujar(SKCanvas canvas)
    {
        using var paint = new SKPaint
        {
            Color = Color,
            StrokeWidth = Grosor,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };
        canvas.DrawCircle(Centro, Radio, paint);
    }
}

public class TrazoFlecha : Trazo
{
    public SKPoint Inicio { get; set; }
    public SKPoint Fin { get; set; }
    public override void Dibujar(SKCanvas canvas)
    {
        using var paint = new SKPaint
        {
            Color = Color,
            StrokeWidth = Grosor,
            IsAntialias = true,
            Style = SKPaintStyle.StrokeAndFill,
            StrokeCap = SKStrokeCap.Round
        };
        canvas.DrawLine(Inicio, Fin, paint);

        float dx = Fin.X - Inicio.X, dy = Fin.Y - Inicio.Y;
        float L = MathF.Sqrt(dx * dx + dy * dy);
        if (L < 1) return;
        float ux = dx / L, uy = dy / L;
        float tam = MathF.Min(Grosor * 5, L * 0.35f);
        float a = 0.45f;
        float c = MathF.Cos(a), s = MathF.Sin(a);
        var a1 = new SKPoint(Fin.X - tam * (ux * c - uy * s), Fin.Y - tam * (uy * c + ux * s));
        var a2 = new SKPoint(Fin.X - tam * (ux * c + uy * s), Fin.Y - tam * (uy * c - ux * s));
        using var path = new SKPath();
        path.MoveTo(Fin); path.LineTo(a1); path.LineTo(a2); path.Close();
        canvas.DrawPath(path, paint);
    }
}

public class TrazoTexto : Trazo
{
    public SKPoint Posicion { get; set; }
    public string Texto { get; set; } = string.Empty;
    public override void Dibujar(SKCanvas canvas)
    {
        using var sombra = new SKPaint { Color = SKColors.Black.WithAlpha(180), TextSize = Grosor * 4, IsAntialias = true };
        using var paint = new SKPaint { Color = Color, TextSize = Grosor * 4, IsAntialias = true };
        canvas.DrawText(Texto, Posicion.X + 1, Posicion.Y + 1, sombra);
        canvas.DrawText(Texto, Posicion.X, Posicion.Y, paint);
    }
}

// ════════════════════════════════════════════════════════════════════
// AvaluoImageEditorPage — UI 100% code-behind
// ════════════════════════════════════════════════════════════════════
public class AvaluoImageEditorPage : ContentPage
{
    // ── Evento de retorno ──────────────────────────────────────────
    public event Action<byte[]>? ImagenEditada;

    // ── Bitmap ────────────────────────────────────────────────────
    private SKBitmap? _bitmap;

    // ── Estado ────────────────────────────────────────────────────
    private HerramientaDibujo _herramienta = HerramientaDibujo.Lapiz;
    private SKColor _color = SKColor.Parse("#F44336");
    private float _grosor = 6f;

    private readonly List<Trazo> _trazos = new();
    private TrazoCurva? _curvaActual;
    private SKPoint _ptDown;

    private SKMatrix _mxImg = SKMatrix.Identity;
    private SKMatrix _mxInv = SKMatrix.Identity;

    // ── Referencias a controles creados en código ─────────────────
    private SKCanvasView _canvas = null!;
    private Label _lblInfo = null!;
    private Label _lblTitulo = null!;
    private Slider _sliderGrosor = null!;

    // Bordes de herramientas
    private Border _btnLapiz = null!;
    private Border _btnCirculo = null!;
    private Border _btnFlecha = null!;
    private Border _btnTexto = null!;

    // Chips de color
    private readonly Dictionary<string, Border> _chipColor = new();

    // ────────────────────────────────────────────────────────────────
    public AvaluoImageEditorPage(byte[] imageBytes, string titulo)
    {
        BackgroundColor = Color.FromArgb("#1A1A1A");
        _bitmap = SKBitmap.Decode(imageBytes);

        Content = ConstruirUI();
        _lblTitulo.Text = $"✏️ {titulo}";
        ActualizarUIHerramienta();
        SeleccionarChipColor("#F44336");
    }

    // ════════════════════════════════════════════════════════════════
    // Construcción de UI
    // ════════════════════════════════════════════════════════════════
    private View ConstruirUI()
    {
        // ── Header ────────────────────────────────────────────────
        _lblTitulo = new Label
        {
            Text = "Editor",
            TextColor = Colors.White,
            FontSize = 17,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Fill,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalOptions = LayoutOptions.Center
        };

        var btnAtras = new Button
        {
            Text = "←",
            FontSize = 34,
            TextColor = Colors.White,
            BackgroundColor = Colors.Transparent,
            Padding = Thickness.Zero,
            WidthRequest = 50
        };
        btnAtras.Clicked += OnBackClicked;

        var btnGuardar = new Button
        {
            Text = "✓",
            FontSize = 22,
            TextColor = Color.FromArgb("#4CAF50"),
            FontAttributes = FontAttributes.Bold,
            BackgroundColor = Colors.Transparent,
            Padding = Thickness.Zero,
            WidthRequest = 50
        };
        btnGuardar.Clicked += OnGuardarClicked;

        var header = new Grid
        {
            BackgroundColor = Color.FromArgb("#1A1A1A"),
            HeightRequest = 54,
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new(new GridLength(50)),
                new(new GridLength(1, GridUnitType.Star)),
                new(new GridLength(50))
            },
            Padding = new Thickness(4, 0)
        };
        header.Add(btnAtras, 0, 0);
        header.Add(_lblTitulo, 1, 0);
        header.Add(btnGuardar, 2, 0);

        // ── Barra herramientas ────────────────────────────────────
        _btnLapiz = CrearBtnHerramienta("✏️", OnLapizClicked);
        _btnCirculo = CrearBtnHerramienta("⭕", OnCirculoClicked);
        _btnFlecha = CrearBtnHerramienta("➡️", OnFlechaClicked);
        _btnTexto = CrearBtnHerramienta("🔤", OnTextoClicked);

        _sliderGrosor = new Slider
        {
            Minimum = 2,
            Maximum = 30,
            Value = 6,
            WidthRequest = 90,
            VerticalOptions = LayoutOptions.Center,
            ThumbColor = Color.FromArgb("#B00000"),
            MinimumTrackColor = Color.FromArgb("#B00000"),
            MaximumTrackColor = Color.FromArgb("#555555")
        };
        _sliderGrosor.ValueChanged += (s, e) => _grosor = (float)e.NewValue;

        var btnDeshacer = new Button
        {
            Text = "↩",
            FontSize = 20,
            TextColor = Colors.White,
            BackgroundColor = Colors.Transparent,
            Padding = new Thickness(6, 0)
        };
        btnDeshacer.Clicked += OnDeshacerClicked;

        var btnLimpiar = new Button
        {
            Text = "🗑️",
            FontSize = 18,
            TextColor = Color.FromArgb("#FF5252"),
            BackgroundColor = Colors.Transparent,
            Padding = new Thickness(6, 0)
        };
        btnLimpiar.Clicked += OnLimpiarClicked;

        var barraHerr = new Grid
        {
            BackgroundColor = Color.FromArgb("#2A2A2A"),
            Padding = new Thickness(10, 6),
            ColumnSpacing = 4,
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new(GridLength.Auto), // lapiz
                new(GridLength.Auto), // circulo
                new(GridLength.Auto), // flecha
                new(GridLength.Auto), // texto
                new(GridLength.Auto), // sep
                new(GridLength.Auto), // slider
                new(new GridLength(1, GridUnitType.Star)), // spacer
                new(GridLength.Auto), // deshacer
                new(GridLength.Auto)  // limpiar
            }
        };
        barraHerr.Add(_btnLapiz, 0, 0);
        barraHerr.Add(_btnCirculo, 1, 0);
        barraHerr.Add(_btnFlecha, 2, 0);
        barraHerr.Add(_btnTexto, 3, 0);
        barraHerr.Add(new BoxView { WidthRequest = 1, BackgroundColor = Color.FromArgb("#555"), VerticalOptions = LayoutOptions.Fill, Margin = new Thickness(4, 4) }, 4, 0);
        barraHerr.Add(_sliderGrosor, 5, 0);
        barraHerr.Add(new BoxView { BackgroundColor = Colors.Transparent }, 6, 0);
        barraHerr.Add(btnDeshacer, 7, 0);
        barraHerr.Add(btnLimpiar, 8, 0);

        // ── Paleta de colores ─────────────────────────────────────
        var colores = new (string hex, bool bordeBlanco)[]
        {
            ("#F44336", true), ("#FF9800", false), ("#FFEB3B", false),
            ("#4CAF50", false), ("#2196F3", false), ("#FFFFFF", false), ("#212121", false)
        };

        var paleta = new HorizontalStackLayout
        {
            BackgroundColor = Color.FromArgb("#2A2A2A"),
            Spacing = 10,
            Padding = new Thickness(14, 6),
            HorizontalOptions = LayoutOptions.Center
        };
        paleta.Add(new Label { Text = "Color:", TextColor = Color.FromArgb("#AAA"), FontSize = 13, VerticalOptions = LayoutOptions.Center });

        foreach (var (hex, _) in colores)
        {
            var chip = new Border
            {
                BackgroundColor = Color.FromArgb(hex),
                WidthRequest = 28,
                HeightRequest = 28,
                StrokeShape = new RoundRectangle { CornerRadius = 14 },
                StrokeThickness = 1,
                Stroke = Color.FromArgb("#555555")
            };
            var tapChip = new TapGestureRecognizer();
            var hexCopy = hex;
            tapChip.Tapped += (s, e) =>
            {
                _color = SKColor.Parse(hexCopy);
                SeleccionarChipColor(hexCopy);
            };
            chip.GestureRecognizers.Add(tapChip);
            paleta.Add(chip);
            _chipColor[hex] = chip;
        }

        // ── Canvas ────────────────────────────────────────────────
        _canvas = new SKCanvasView
        {
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill,
            EnableTouchEvents = true
        };
        _canvas.PaintSurface += OnPaintSurface;
        _canvas.Touch += OnTouch;

        var canvasWrapper = new Grid { BackgroundColor = Color.FromArgb("#111111") };
        canvasWrapper.Add(_canvas);

        // ── Footer info ───────────────────────────────────────────
        _lblInfo = new Label
        {
            Text = "✏️ Lápiz  ·  Dibuja libremente sobre la imagen",
            TextColor = Color.FromArgb("#AAAAAA"),
            FontSize = 13,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        var footer = new Grid
        {
            BackgroundColor = Color.FromArgb("#1A1A1A"),
            HeightRequest = 44,
            Padding = new Thickness(16, 0)
        };
        footer.Add(_lblInfo);

        // ── Layout principal ──────────────────────────────────────
        var root = new Grid
        {
            RowDefinitions = new RowDefinitionCollection
            {
                new(GridLength.Auto), // header
                new(GridLength.Auto), // barra herr
                new(new GridLength(1, GridUnitType.Star)), // canvas
                new(GridLength.Auto), // paleta
                new(GridLength.Auto)  // footer
            }
        };
        root.Add(header, 0, 0);
        root.Add(barraHerr, 0, 1);
        root.Add(canvasWrapper, 0, 2);
        root.Add(paleta, 0, 3);
        root.Add(footer, 0, 4);

        return root;
    }

    // ════════════════════════════════════════════════════════════════
    // Helper — crea un botón de herramienta con Border
    // ════════════════════════════════════════════════════════════════
    private static Border CrearBtnHerramienta(string emoji, EventHandler handler)
    {
        var btn = new Button
        {
            Text = emoji,
            FontSize = 18,
            BackgroundColor = Colors.Transparent,
            TextColor = Colors.White,
            Padding = Thickness.Zero
        };
        btn.Clicked += handler;

        return new Border
        {
            BackgroundColor = Color.FromArgb("#2A2A2A"),
            StrokeThickness = 1,
            Stroke = Color.FromArgb("#555555"),
            StrokeShape = new RoundRectangle { CornerRadius = 8 },
            Padding = new Thickness(8, 6),
            Content = btn
        };
    }

    // ════════════════════════════════════════════════════════════════
    // PaintSurface
    // ════════════════════════════════════════════════════════════════
    private void OnPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Black);
        if (_bitmap == null) return;

        var info = e.Info;
        float sx = (float)info.Width / _bitmap.Width;
        float sy = (float)info.Height / _bitmap.Height;
        float sc = MathF.Min(sx, sy);
        float ox = (info.Width - _bitmap.Width * sc) / 2f;
        float oy = (info.Height - _bitmap.Height * sc) / 2f;

        _mxImg = SKMatrix.CreateScaleTranslation(sc, sc, ox, oy);
        _mxImg.TryInvert(out _mxInv);

        canvas.SetMatrix(_mxImg);
        canvas.DrawBitmap(_bitmap, 0, 0);
        foreach (var t in _trazos) t.Dibujar(canvas);
        canvas.ResetMatrix();
    }

    // ════════════════════════════════════════════════════════════════
    // Touch
    // ════════════════════════════════════════════════════════════════
    private void OnTouch(object? sender, SKTouchEventArgs e)
    {
        var pt = _mxInv.MapPoint(e.Location);

        switch (e.ActionType)
        {
            case SKTouchAction.Pressed:
                _ptDown = pt;
                if (_herramienta == HerramientaDibujo.Lapiz)
                {
                    _curvaActual = new TrazoCurva { Color = _color, Grosor = _grosor };
                    _curvaActual.Puntos.Add(pt);
                    _trazos.Add(_curvaActual);
                }
                break;

            case SKTouchAction.Moved:
                if (_herramienta == HerramientaDibujo.Lapiz && _curvaActual != null)
                {
                    _curvaActual.Puntos.Add(pt);
                    _canvas.InvalidateSurface();
                }
                else if (_herramienta is HerramientaDibujo.Circulo or HerramientaDibujo.Flecha)
                {
                    if (_trazos.Count > 0 && _trazos[^1].EsPreview)
                        _trazos.RemoveAt(_trazos.Count - 1);
                    _trazos.Add(CrearPreview(_ptDown, pt));
                    _canvas.InvalidateSurface();
                }
                break;

            case SKTouchAction.Released:
                if (_herramienta == HerramientaDibujo.Lapiz)
                {
                    _curvaActual = null;
                }
                else if (_herramienta is HerramientaDibujo.Circulo or HerramientaDibujo.Flecha)
                {
                    if (_trazos.Count > 0 && _trazos[^1].EsPreview)
                        _trazos.RemoveAt(_trazos.Count - 1);
                    _trazos.Add(CrearDefinitivo(_ptDown, pt));
                    _canvas.InvalidateSurface();
                }
                break;
        }
        e.Handled = true;
    }

    private Trazo CrearPreview(SKPoint ini, SKPoint fin) =>
        _herramienta == HerramientaDibujo.Circulo
            ? new TrazoCirculo { Color = _color.WithAlpha(160), Grosor = _grosor, EsPreview = true, Centro = Centro(ini, fin), Radio = Radio(ini, fin) }
            : new TrazoFlecha { Color = _color.WithAlpha(160), Grosor = _grosor, EsPreview = true, Inicio = ini, Fin = fin };

    private Trazo CrearDefinitivo(SKPoint ini, SKPoint fin) =>
        _herramienta == HerramientaDibujo.Circulo
            ? new TrazoCirculo { Color = _color, Grosor = _grosor, Centro = Centro(ini, fin), Radio = Radio(ini, fin) }
            : new TrazoFlecha { Color = _color, Grosor = _grosor, Inicio = ini, Fin = fin };

    private static SKPoint Centro(SKPoint a, SKPoint b) => new((a.X + b.X) / 2, (a.Y + b.Y) / 2);
    private static float Radio(SKPoint a, SKPoint b) { float dx = b.X - a.X, dy = b.Y - a.Y; return MathF.Sqrt(dx * dx + dy * dy) / 2; }

    // ════════════════════════════════════════════════════════════════
    // Herramientas
    // ════════════════════════════════════════════════════════════════
    private void OnLapizClicked(object? s, EventArgs e) { _herramienta = HerramientaDibujo.Lapiz; ActualizarUIHerramienta(); }
    private void OnCirculoClicked(object? s, EventArgs e) { _herramienta = HerramientaDibujo.Circulo; ActualizarUIHerramienta(); }
    private void OnFlechaClicked(object? s, EventArgs e) { _herramienta = HerramientaDibujo.Flecha; ActualizarUIHerramienta(); }

    private async void OnTextoClicked(object? s, EventArgs e)
    {
        _herramienta = HerramientaDibujo.Texto;
        ActualizarUIHerramienta();

        string? texto = await DisplayPromptAsync(
            "Agregar texto", "Escribe la anotación:",
            accept: "Agregar", cancel: "Cancelar",
            placeholder: "Ej: Golpe, Rayón, Abolladura…",
            maxLength: 60, keyboard: Keyboard.Text);

        if (string.IsNullOrWhiteSpace(texto)) return;

        float cx = _bitmap != null ? _bitmap.Width / 2f : 200;
        float cy = _bitmap != null ? _bitmap.Height / 2f : 200;

        _trazos.Add(new TrazoTexto { Color = _color, Grosor = _grosor, Posicion = new SKPoint(cx, cy), Texto = texto });
        _canvas.InvalidateSurface();
    }

    private void ActualizarUIHerramienta()
    {
        // Reset todos
        foreach (var b in new[] { _btnLapiz, _btnCirculo, _btnFlecha, _btnTexto })
        {
            b.BackgroundColor = Color.FromArgb("#2A2A2A");
            b.Stroke = Color.FromArgb("#555555");
            b.StrokeThickness = 1;
        }
        // Resaltar activo
        var activo = _herramienta switch
        {
            HerramientaDibujo.Circulo => _btnCirculo,
            HerramientaDibujo.Flecha => _btnFlecha,
            HerramientaDibujo.Texto => _btnTexto,
            _ => _btnLapiz
        };
        activo.BackgroundColor = Color.FromArgb("#3A1A1A");
        activo.Stroke = Color.FromArgb("#B00000");
        activo.StrokeThickness = 2;

        _lblInfo.Text = _herramienta switch
        {
            HerramientaDibujo.Lapiz => "✏️ Lápiz  ·  Dibuja libremente sobre la imagen",
            HerramientaDibujo.Circulo => "⭕ Círculo  ·  Arrastra para crear un círculo de resaltado",
            HerramientaDibujo.Flecha => "➡️ Flecha  ·  Arrastra para indicar un punto específico",
            HerramientaDibujo.Texto => "🔤 Texto  ·  Toca el botón para escribir una anotación",
            _ => string.Empty
        };
    }

    // ════════════════════════════════════════════════════════════════
    // Colores
    // ════════════════════════════════════════════════════════════════
    private void SeleccionarChipColor(string hex)
    {
        foreach (var (k, chip) in _chipColor)
        {
            chip.StrokeThickness = 1;
            chip.Stroke = Color.FromArgb("#555555");
        }
        if (_chipColor.TryGetValue(hex, out var sel))
        {
            sel.StrokeThickness = 3;
            sel.Stroke = Colors.White;
        }
    }

    // ════════════════════════════════════════════════════════════════
    // Deshacer / Limpiar
    // ════════════════════════════════════════════════════════════════
    private void OnDeshacerClicked(object? s, EventArgs e)
    {
        // Eliminar el último trazo no-preview
        for (int i = _trazos.Count - 1; i >= 0; i--)
        {
            if (!_trazos[i].EsPreview) { _trazos.RemoveAt(i); break; }
        }
        _canvas.InvalidateSurface();
    }

    private async void OnLimpiarClicked(object? s, EventArgs e)
    {
        if (_trazos.Count == 0) return;
        bool ok = await DisplayAlert("Limpiar", "¿Eliminar todas las anotaciones?", "Sí", "Cancelar");
        if (!ok) return;
        _trazos.Clear();
        _canvas.InvalidateSurface();
    }

    // ════════════════════════════════════════════════════════════════
    // Guardar — aplana bitmap + trazos → JPEG
    // ════════════════════════════════════════════════════════════════
    private async void OnGuardarClicked(object? s, EventArgs e)
    {
        if (_bitmap == null) return;
        try
        {
            var bytes = await Task.Run(() =>
            {
                using var bmpOut = new SKBitmap(_bitmap.Width, _bitmap.Height);
                using var cv = new SKCanvas(bmpOut);
                cv.DrawBitmap(_bitmap, 0, 0);
                foreach (var t in _trazos) t.Dibujar(cv);
                cv.Flush();
                using var img = SKImage.FromBitmap(bmpOut);
                using var data = img.Encode(SKEncodedImageFormat.Jpeg, 92);
                return data.ToArray();
            });
            ImagenEditada?.Invoke(bytes);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo guardar: {ex.Message}", "OK");
        }
    }

    // ════════════════════════════════════════════════════════════════
    // Back
    // ════════════════════════════════════════════════════════════════
    private async void OnBackClicked(object? s, EventArgs e)
    {
        if (_trazos.Any(t => !t.EsPreview))
        {
            bool descartar = await DisplayAlert("Descartar", "¿Cerrar sin guardar los trazos?", "Sí", "Cancelar");
            if (!descartar) return;
        }
        await Navigation.PopModalAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _bitmap?.Dispose();
        _bitmap = null;
    }
}