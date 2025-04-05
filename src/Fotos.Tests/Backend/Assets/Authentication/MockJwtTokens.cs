using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Fotos.Tests.Backend.Assets.Authentication;

internal static class MockJwtTokens
{
    public static string Issuer { get; } = Guid.NewGuid().ToString(); // random issuer
    public static SecurityKey SecurityKey { get; }
    public static SigningCredentials SigningCredentials { get; }

    private static readonly JwtSecurityTokenHandler _tokenHandler = new();
    private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();
    private static readonly byte[] _key = new byte[32];

    static MockJwtTokens()
    {
        _rng.GetBytes(_key);
        SecurityKey = new SymmetricSecurityKey(_key) { KeyId = Guid.NewGuid().ToString() };
        SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
    }

    public static string GenerateJwtToken(IEnumerable<Claim> claims)
    {
        return _tokenHandler.WriteToken(new JwtSecurityToken(Issuer, null, claims, null, DateTime.UtcNow.AddMinutes(20), SigningCredentials));
    }
}
