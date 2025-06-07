namespace SmartphoneApp.Services
{
    public interface IApiService
    {
        Task<(bool Success, string Token, string ErrorMessage)> LoginAsync(string username, string password);
    }
}
