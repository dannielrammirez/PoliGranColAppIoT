using PoliGranColAppIoT.Models;
using PoliGranColAppIoT.Services.IServices;
using PoliGranColAppIoT.ViewModel;
using System.Collections.ObjectModel;

namespace PoliGranColAppIoT.Views;

public partial class HomePage : ContentPage
{
    public HomePage(IRepository<Data[]> repository)
    {
        BindingContext = new HomePageViewModel(repository);
        InitializeComponent();

        MessagingCenter.Subscribe<HomePageViewModel, string>(this, "ShowMessage", async (sender, arg) =>
        {
            await DisplayAlert("Mensaje recibido", arg, "OK");
        });
    }

    private void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Aqu� no se hace nada, ya que la acci�n se realiza en el ViewModel
        // al suscribirse al evento PropertyChanged de OpcionSeleccionada
    }
}