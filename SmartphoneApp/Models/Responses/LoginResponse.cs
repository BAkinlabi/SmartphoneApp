namespace SmartphoneApp.Models.Responses
{
    public class LoginResponse
    {
        public string? accessToken { get; set; }
        public string? refreshToken { get; set; }
        public int id { get; set; }
        public string? username { get; set; }
        public string? email { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? gender { get; set; }
        public string? image { get; set; }
    }
}
