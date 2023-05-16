namespace PoliGranColAppIoT.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
	}

	private async void LogoutButton_Clicked(object sender, EventArgs e)
	{
		if (await DisplayAlert("Estas seguro?", "Seras desconectado.", "Si", "No"))
		{
			SecureStorage.RemoveAll();
			await Shell.Current.GoToAsync("///login");
		}
	}
}