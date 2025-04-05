using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Fotos.Tests.Backend.Assets.Authentication;

[SuppressMessage("Design", "CA1812", Justification = "Instantiated by DI")]
internal sealed class JwtAuthHandler : JwtBearerHandler
{
    public JwtAuthHandler(
     IOptionsMonitor<JwtBearerOptions> options,
     ILoggerFactory logger,
     UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Context.Request.Headers.TryGetValue("Authorization", out var authHeaderValues))
        {
            var jwtToken = authHeaderValues.FirstOrDefault()?.Split(" ").LastOrDefault();

            if (!string.IsNullOrEmpty(jwtToken))
            {
                if (new JwtSecurityTokenHandler().ReadToken(jwtToken) is JwtSecurityToken token)
                {
                    var claims = token.Claims;
                    var identity = new ClaimsIdentity(claims, "Bearer", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

                    return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), "Bearer")));
                }
            }
        }

        return Task.FromResult(AuthenticateResult.Fail("No token provided"));
    }
}