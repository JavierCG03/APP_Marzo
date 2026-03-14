using CarslineApp.Services;
using CarslineApp.ViewModels;

namespace CarslineApp.Views.Avaluo;

public partial class EquipamientoAvaluoPage : ContentPage
{
    private readonly EquipamientoAvaluoViewModel _viewModel;

    public EquipamientoAvaluoPage(int avaluoId)
    {
        InitializeComponent();
        _viewModel = new EquipamientoAvaluoViewModel(new ApiService(), avaluoId);
        BindingContext = _viewModel;
    }
}