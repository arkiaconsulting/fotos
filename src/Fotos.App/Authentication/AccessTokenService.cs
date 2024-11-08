using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
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

    public AuthenticationProperties StoreNewToken(AuthenticationProperties authenticationProperties, string nameIdentifier, string givenName)
    {
        var token = GenerateAccessToken(nameIdentifier, givenName);

        authenticationProperties.StoreTokens(
        [
            new AuthenticationToken
            {
                Name = "access_token",
                Value = new JwtSecurityTokenHandler().WriteToken(token)
            }
        ]);

        return authenticationProperties;
    }

    private JwtSecurityToken GenerateAccessToken(string nameIdentifier, string givenName)
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
            expires: DateTime.UtcNow.AddMinutes(1),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signinKey)),
                SecurityAlgorithms.HmacSha256)
        );
    }
}
