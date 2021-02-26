// unset

namespace Authentication.API.Controllers
{
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using Providers;
    using System.Threading.Tasks;

    [ApiController]
    [Route("[controller]/[action]")]
    public class TokenController : ControllerBase
    {
        private readonly ITokenProvider _tokenProvider;
        public TokenController(ITokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        [HttpPost]
        public async Task<ActionResult<AuthenticateResponse>> Authorize([FromBody]AuthenticateRequest request)
        {
            var response = await _tokenProvider.GetTokenAsync(request);
 
            if (!string.IsNullOrEmpty(response.Data.Errors?[0].Text))
            {
                return new BadRequestObjectResult(response);
            }
 
            return response;
        }
    }
}