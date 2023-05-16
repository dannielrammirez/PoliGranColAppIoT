using PoliGranColAppIoT.Models;

namespace PoliGranColAppIoT.Services.IServices
{
    public interface IRepository<T> where T : class
    {
        Task<GenericResponse<T>> Get(string url, string parameters, string token);
        Task<bool> Update(string url, string jsonData, string token);
    }
}