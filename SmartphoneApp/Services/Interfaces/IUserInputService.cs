namespace SmartphoneApp.Services
{
    public interface IUserInputService
    {
        (string Username, string Password) GetCredentials();
        int GetPercentage();
    }
}
