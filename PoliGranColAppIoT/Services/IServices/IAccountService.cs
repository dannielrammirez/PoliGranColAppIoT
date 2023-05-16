using PoliGranColAppIoT.Models;

namespace PoliGranColAppIoT.Services.IServices
{
    public interface IAccountService
    {
        Task<GenericResponse<AccountAuth>> LoginAsync(string url, Account objAccount);
    }
}