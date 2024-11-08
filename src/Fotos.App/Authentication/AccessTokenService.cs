using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Fotos.App.Authentication;

internal sealed class AccessTokenService
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly string _signinKey;

    public AccessTokenService(IConfiguration configuration)
    {
        _issuer = configuration["BaseUrl"] ?? throw new ArgumentNullException("BaseUrl setting is missing or empty", default(Exception));
        _audience = configuration["BaseUrl"] ?? throw new ArgumentNullException("BaseUrl setting is missing or empty", default(Exception));
        _signinKey = configuration["AccessTokenSigningKey"] ?? throw new ArgumentNullException("AccessTokenSigningKey setting is missing or empty", default(Exception));
    }

    public JwtSecurityToken GenerateAccessToken(string nameIdentifier, string givenName)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, nameIdentifier),
            new(ClaimTypes.GivenName, givenName)
        };

        return new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(6),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signinKey)),
                SecurityAlgorithms.HmacSha256)
        );
    }
}

internal static class AccessTokenExtensions
{
    public static void StoreFotosApiToken(this AuthenticationProperties authenticationProperties, JwtSecurityToken securityToken)
    {
        authenticationProperties.StoreTokens(
        [
            new()
            {
                Name = "access_token",
                Value = new JwtSecurityTokenHandler().WriteToken(securityToken)
            },
            new()
            {
                Name = "expires_at",
                Value = securityToken.ValidTo.ToString("o", CultureInfo.InvariantCulture)
            }
        ]);
    }
}