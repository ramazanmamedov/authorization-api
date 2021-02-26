namespace Authentication.API.Providers
{
    using Data;
    using IdentityModel;
    using IdentityServer4.ResponseHandling;
    using IdentityServer4.Validation;
    using Microsoft.AspNetCore.Http;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Threading.Tasks;
    using IdpTokenResponse = IdentityServer4.ResponseHandling.TokenResponse;


    public class TokenProvider : ITokenProvider
    {
        private readonly ITokenRequestValidator _requestValidator;
        private readonly IClientSecretValidator _clientValidator;
        private readonly ITokenResponseGenerator _responseGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;


        private const string GrandType = "password";
        private const string Scope = "openid";
        
        public TokenProvider(ITokenRequestValidator requestValidator,
            IClientSecretValidator clientValidator,
            ITokenResponseGenerator responseGenerator,
            IHttpContextAccessor httpContextAccessor)
        {
            _requestValidator = requestValidator;
            _clientValidator = clientValidator;
            _responseGenerator = responseGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthenticateResponse> GetTokenAsync(AuthenticateRequest request)
        {
            var parameters = new NameValueCollection
            {
                {"username", request.Username},
                {"password", request.Password},
                {"grant_type", GrandType},
                {"scope", Scope},
                {"response_type", OidcConstants.ResponseTypes.Token},
            };

            var response = await GetIdpToken(parameters);
            var authResponse = GetTokenResponse(response);
            return authResponse;
        }

        private async Task<IdpTokenResponse> GetIdpToken(NameValueCollection parameters)
        {
            var clientResult = await _clientValidator.ValidateAsync(_httpContextAccessor.HttpContext);

            if (clientResult.IsError)
            {
                return new IdpTokenResponse {Custom = new Dictionary<string, object> {{"Error", "invalid_client"}, {"ErrorDescription", "Invalid client/secret combination"}}};
            }

            var validationResult = await _requestValidator.ValidateRequestAsync(parameters, clientResult);

            if (validationResult.IsError)
            {
                return new IdpTokenResponse {Custom = new Dictionary<string, object> {{"Error", validationResult.Error}, {"ErrorDescription", validationResult.ErrorDescription}}};
            }

            return await _responseGenerator.ProcessAsync(validationResult);
        }

        private static AuthenticateResponse GetTokenResponse(IdpTokenResponse response)
        {
            if (response.Custom != null && response.Custom.ContainsKey("Error"))
            {
                return new AuthenticateResponse {Data = new Data {Errors = new[] {new Error {Code = 500, Text = response.Custom["Error"].ToString()}}}};
            }
            return new AuthenticateResponse {Data = new Data {Token = response.AccessToken}};
        }
    }
}