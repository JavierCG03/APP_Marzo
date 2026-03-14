using CarslineApp.ViewModels;

namespace CarslineApp.Views.Avaluo;

public partial class FotosAvaluoPage : ContentPage
{
    public FotosAvaluoPage(int avaluoId)
    {
        InitializeComponent();

        if (BindingContext is FotosAvaluoViewModel vm)
            vm.AvaluoId = avaluoId;
    }

    public FotosAvaluoPage()
    {
        InitializeComponent();
    }
}