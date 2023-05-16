using PoliGranColAppIoT.Models;
using PoliGranColAppIoT.Services.IServices;
using PoliGranColAppIoT.ViewModel;
using System.Collections.ObjectModel;

namespace PoliGranColAppIoT.Views;

public partial class NewUpdatePage : ContentPage
{
    public NewUpdatePage(IRepository<Data[]> repository)
    {
        BindingContext = new NewUpdatePageViewModel(repository);
        InitializeComponent();

        MessagingCenter.Subscribe<NewUpdatePageViewModel, string>(this, "ShowMessage", async (sender, arg) =>
        {
            await DisplayAlert("Mensaje recibido", arg, "OK");
        });
    }

    private void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Aquí no se hace nada, ya que la acción se realiza en el ViewModel
        // al suscribirse al evento PropertyChanged de OpcionSeleccionada
    }
}