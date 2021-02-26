// unset

namespace Authentication.API.Data
{
    public class AuthenticateResponse
    {
        public Data Data { get; set; }

        public AuthenticateResponse(string token)
        {
            Data = new Data {Token = token};
        }

        public AuthenticateResponse()
        {
            
        }
    }
}