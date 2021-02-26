namespace Authentication.API
{
    using IdentityServer4;
    using IdentityServer4.Models;
    using Microsoft.Extensions.Configuration;
    using Options;
    using System.Collections.Generic;

    public static class Config
    {
        private static readonly PosApiClientOptions Options = new PosApiClientOptions();

        static Config()
        {
            Startup.StaticConfig.GetSection(nameof(PosApiClientOptions)).Bind(Options);
        }
        
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId()
            };
        }
        
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource> {new ApiResource("all", "all")};
        }
        
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope
                {
                    Name = Options.Name,
                    DisplayName = Options.Name,
                }
            };
        }
        
        
        public static IEnumerable<Client> GetClients()
        {
            var tokenLifetimeMinutes = Options.TokenLifeTimeMinutes * 60;
            return new List<Client>
            {
                new Client
                {
                    ClientName = Options.Name,
                    ClientId = Options.Id,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret(Options.Secret.Sha256())
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    AccessTokenLifetime = tokenLifetimeMinutes,
                    RefreshTokenExpiration = TokenExpiration.Absolute,
                    AbsoluteRefreshTokenLifetime = tokenLifetimeMinutes
                }
            };
        }
    }
}