namespace Authentication.API.Infrastructure
{
    using Data;
    using Extensions;
    using IdentityServer4;
    using IdentityServer4.Configuration;
    using IdentityServer4.Events;
    using IdentityServer4.Models;
    using IdentityServer4.Services;
    using IdentityServer4.Stores;
    using IdentityServer4.Validation;
    using Microsoft.AspNetCore.Http;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class CustomClientSecretValidator : IClientSecretValidator
    {
        private readonly IClientStore _clients;
        private readonly IEventService _events;
        private readonly ISecretsListValidator _validator;
        private readonly IdentityServerOptions _options;


        public CustomClientSecretValidator(IClientStore clients, IEventService events, ISecretsListValidator validator,
            IdentityServerOptions options)
        {
            _clients = clients;
            _events = events;
            _validator = validator;
            _options = options;
        }

        public async Task<ClientSecretValidationResult> ValidateAsync(HttpContext context)
        {
            var fail = new ClientSecretValidationResult {IsError = true};

            var parsedSecret = await ParseAsync(context);
            if (parsedSecret == null)
            {
                await RaiseFailureEventAsync("unknown", "No client id found");
                return fail;
            }

            var client = await _clients.FindEnabledClientByIdAsync(parsedSecret.Id);
            if (client == null)
            {
                await RaiseFailureEventAsync(parsedSecret.Id, "Unknown client");
                return fail;
            }

            SecretValidationResult secretValidationResult = null;
            if (client.RequireClientSecret || !client.IsImplicitOnly())
            {
                secretValidationResult = await _validator.ValidateAsync(client.ClientSecrets, parsedSecret);
                if (secretValidationResult.Success == false)
                {
                    await RaiseFailureEventAsync(client.ClientId, "Invalid client secret");

                    return fail;
                }
            }
            
            var success = new ClientSecretValidationResult
            {
                IsError = false,
                Client = client,
                Secret = parsedSecret,
                Confirmation = secretValidationResult?.Confirmation
            };

            await RaiseSuccessEventAsync(client.ClientId, parsedSecret.Type);
            return success;
        }


        private Task RaiseSuccessEventAsync(string clientId, string authMethod)
        {
            return _events.RaiseAsync(new ApiAuthenticationSuccessEvent(clientId, authMethod));
        }

        private Task RaiseFailureEventAsync(string clientId, string message)
        {
            return _events.RaiseAsync(new ApiAuthenticationFailureEvent(clientId, message));
        }

        //By default IdentityServer4 using x-www-form-urlencoded form body request parsing.
        //In this sample we using application/json type body request parsing.
        private async Task<ParsedSecret> ParseAsync(HttpContext context)
        {
            //Re-read http context body
            context.Request.Body.Position = 0;
            string bodyStream;
            using (var reader = new StreamReader(context.Request.Body))
            {
                bodyStream = await reader.ReadToEndAsync();
            }
            var options = new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase,};
            var request = JsonSerializer.Deserialize<AuthenticateRequest>(bodyStream, options);
            if (request != null)
            {
                var id = request.ClientId;
                var secret = request.ClientSecret;

                if (id.IsPresent())
                {
                    if (id.Length > _options.InputLengthRestrictions.ClientId)
                    {
                        return null;
                    }

                    if (secret.IsPresent())
                    {
                        if (secret.Length > _options.InputLengthRestrictions.ClientSecret)
                        {
                            return null;
                        }

                        return new ParsedSecret
                        {
                            Id = id,
                            Credential = secret,
                            Type = IdentityServerConstants.ParsedSecretTypes.SharedSecret
                        };
                    }

                    return new ParsedSecret {Id = id, Type = IdentityServerConstants.ParsedSecretTypes.NoSecret};
                }
            }

            return null;
        }
    }
}