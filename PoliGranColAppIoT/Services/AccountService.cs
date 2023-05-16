using PoliGranColAppIoT.Models;
using PoliGranColAppIoT.Services.IServices;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

namespace PoliGranColAppIoT.Services
{
    public class AccountService : IAccountService
    {
        public readonly HttpClient client;
        public AccountService()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = ValidateServerCertificate;

            client = new HttpClient(handler);
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        public async Task<GenericResponse<AccountAuth>> LoginAsync(string url, Account objAccount)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var response = new GenericResponse<AccountAuth>();

            try
            {
                var objAccountAccess = new AccountAuth()
                {
                    username = objAccount.username,
                    password = objAccount.password
                };

                string objJson = JsonSerializer.Serialize(objAccount);
                request.Content = new StringContent(objJson, Encoding.UTF8, "application/json");

                var responseService = await client.SendAsync(request);

                if (responseService.IsSuccessStatusCode)
                {
                    using (var responseStream = await responseService.Content.ReadAsStreamAsync())
                    {
                        var data = await JsonSerializer.DeserializeAsync<AccountAuth>(responseStream);
                        objAccountAccess.token = data.token;
                        response.IsSuccess = true;
                    }

                    response.Data = objAccountAccess;
                    return response;
                }
                else if(responseService.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    response.IsSuccess = false;
                    response.Message = "Credenciales invalidas!";
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Se presento un error realizando la petición al servidor";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = $"{ex.Message} - {ex.InnerException}";
            }

            return response;
        }
    }
}