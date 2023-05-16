using PoliGranColAppIoT.Models;
using PoliGranColAppIoT.Services.IServices;
using PoliGranColAppIoT.Utilities;

namespace PoliGranColAppIoT.Views;

public partial class LoginPage : ContentPage
{
    private readonly IAccountService _accountService;
    public LoginPage(IAccountService accountService)
    {
        InitializeComponent();

        _accountService = accountService;
    }

    protected override bool OnBackButtonPressed()
    {
        Application.Current.Quit();
        return true;
    }

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        var objAccount = new Account()
        {
            username = Username.Text,
            password = Password.Text,
        };

        if (string.IsNullOrEmpty(EndPoint.Text) || objAccount.IsValid())
        {
            CT.UrlBaseApi = $"{EndPoint.Text}/api";

            var loginResponse = await IsCredentialCorrect(objAccount);

            if (loginResponse.IsSuccess)
            {
                await SecureStorage.SetAsync("hasAuth", "true");
                await Shell.Current.GoToAsync("///home");
            }
            else
            {
                await DisplayAlert("Login failed", loginResponse.Message, "Try again");
            }
        }
        else
            await DisplayAlert("Login failed", "Ingrese los datos completos", "Try again");

    }


    private async Task<GenericResponse<AccountAuth>> IsCredentialCorrect(Account objAccount)
    {
        var response = new GenericResponse<AccountAuth>();

        var respAccountAuth = await _accountService.LoginAsync($"{CT.UrlBaseApi}/auth/login", objAccount);

        if (respAccountAuth.Data != null)
        {
            response.IsSuccess = true;
            await SecureStorage.SetAsync("myToken", respAccountAuth.Data.token);
        }
        else
        {
            response.IsSuccess = false;
            response.Message = respAccountAuth.Message;
        }

        return response;
    }
}