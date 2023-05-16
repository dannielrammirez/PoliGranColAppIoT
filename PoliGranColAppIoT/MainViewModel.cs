using PoliGranColAppIoT.Models;
using PoliGranColAppIoT.Utilities;
using System.Text;
using System.Text.Json;
using System.Windows.Input;

namespace PoliGranColAppIoT
{
    public class MainViewModel
    {
        HttpClient client;
        JsonSerializerOptions _serializerOptions;
        string token = string.Empty;
        string idDevice = "6cba5e50-de70-11ed-a681-85bbb1891ac6";
        string parameter = "Status";
        Account objAccount;
        public MainViewModel()
        {
            client = new HttpClient();
            _serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            objAccount = new Account()
            {
                username = "tenant@thingsboard.org",
                password = "tenant",
            };

            Login();
        }

        public async void Login()
        {
            var url = $"{CT.UrlBaseApi}/auth/login";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            string objJson = string.Empty;

            if (objAccount.IsValid())
            {
                objJson = JsonSerializer.Serialize(objAccount);
                request.Content = new StringContent(objJson, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    using (var responseStream = await response.Content.ReadAsStreamAsync())
                    {
                        var data = await JsonSerializer.DeserializeAsync<ResponseToken>(responseStream, _serializerOptions);
                        token = data.token;
                    }
                }
            }
        }

        public async Task GetAction()
        {
            var url = $"{CT.UrlBaseApi}/plugins/telemetry/DEVICE/{idDevice}/values/attributes/SHARED_SCOPE?keys={parameter}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            if (token != null && token.Length != 0)
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                //var content = await response.Content.ReadAsStringAsync();
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<Data[]>(jsonString);
                }
            }
        }
         
        public async Task InsertAction()
        {
            var url = $"{CT.UrlBaseApi}/plugins/telemetry/{idDevice}/SHARED_SCOPE?keys={parameter}";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var objTest = new SendData() { NewAction = "dddAtack!" };

            var testJson = JsonSerializer.Serialize(objTest);
            if (token != null && token.Length != 0)
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);


            request.Content = new StringContent(testJson, Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("BUENA!");
            }
            else
            {
                Console.WriteLine("Error");
            }
        }

        public ICommand GetCurrentStatus =>
            new Command(async () =>
            {
                await GetAction();
            });

        public ICommand InsertActionOne =>
            new Command(async () =>
            {
                await InsertAction();
            });
    }
}