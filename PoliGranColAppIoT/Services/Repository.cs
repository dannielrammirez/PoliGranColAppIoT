using PoliGranColAppIoT.Models;
using PoliGranColAppIoT.Services.IServices;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace PoliGranColAppIoT.Services
{
    public class Repository<T> : IRepository<T> where T : class
    {
        public readonly HttpClient client;
        public Repository()
        {
            
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = ValidateServerCertificate;

            client = new HttpClient(handler);
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            // Siempre devuelve verdadero para aceptar cualquier certificado
            return true;
        }

        public async Task<GenericResponse<T>> Get(string url, string parameters, string token)
        {
            var objT = new GenericResponse<T>();

            try
            {
                var tempParams = string.IsNullOrWhiteSpace(parameters) ? "" : $"?keys={parameters}";
                var request = new HttpRequestMessage(HttpMethod.Get, $"{url}{tempParams}");

                if (token != null && token.Length != 0)
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    using (var responseStream = await response.Content.ReadAsStreamAsync())
                    {
                        objT.IsSuccess = true;
                        var jsonString = await response.Content.ReadAsStringAsync();
                        objT.Data = JsonSerializer.Deserialize<T>(jsonString);
                    }
                }
                else
                {
                    objT.IsSuccess = false;
                    objT.Message = $"{response.StatusCode} - {response.Content}";
                }
            }
            catch (Exception ex)
            {
                objT.IsSuccess = false;
                objT.Message = $"{ex.Message} - {ex.InnerException}";
            }

            return objT;
        }

        public async Task<bool> Update(string url, string jsonData, string token)
        {
            bool resp;
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url);

                if (token != null && token.Length != 0)
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                request.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);

                var jsonString = await response.Content.ReadAsStringAsync();

                resp = true;
            }
            catch (Exception e)
            {
                resp = false;
            }

            return resp;
        }
    }
}