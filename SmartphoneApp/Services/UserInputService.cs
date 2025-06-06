
namespace SmartphoneApp.Services
{
    public class UserInputService : IUserInputService
    {
        public (string Username, string Password) GetCredentials()
        {
            Console.Write("Please enter a Username: ");
            var username = Console.ReadLine() ?? "";
            Console.Write("Please enter a Password: ");
            var password = Console.ReadLine() ?? "";
            return (username, password);
        }

        public int GetPercentage()
        {
            while (true)
            {
                Console.Write("Enter percentage to increase price: ");
                var input = Console.ReadLine();
                if (int.TryParse(input, out int percent) && percent > 0)
                    return percent;
                Console.WriteLine("Invalid input. Please enter a positive integer.");
            }
        }
    }
}
