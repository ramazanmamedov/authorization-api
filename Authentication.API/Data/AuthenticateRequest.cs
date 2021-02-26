// unset

namespace Authentication.API.Data
{
    public class AuthenticateRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}