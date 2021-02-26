// unset

using System.Linq;

namespace Authentication.API.Infrastructure
{
    using Context;
    using IdentityModel;
    using IdentityServer4.Validation;
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class CustomResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {

        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            //Use your own users validation
            var user = UserDbContext.Users.FirstOrDefault(u => u.UserName == context.UserName);
            if (user == null) return Task.CompletedTask;
            var passwordCompareResult = user.Password == context.Password;
            if (passwordCompareResult != true) return Task.CompletedTask;
            context.Result.IsError = false;
            context.Result.Subject = GetClaimsPrincipal(user.UserName, user.Id);
            return Task.CompletedTask;

        }

        private static ClaimsPrincipal GetClaimsPrincipal(string username, string userId)
        {
            var issued = DateTimeOffset.Now.ToUnixTimeSeconds();

            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, userId),
                new Claim(JwtClaimTypes.AuthenticationTime, issued.ToString()),
                new Claim(JwtClaimTypes.Name, username),
                new Claim(JwtClaimTypes.IdentityProvider, "localhost")
            };

            return new ClaimsPrincipal(new ClaimsIdentity(claims));
        }
    }
}