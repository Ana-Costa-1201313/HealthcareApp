using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using Microsoft.IdentityModel.Tokens;
using Backoffice.Domain.Shared;

public class TokenServiceTests
{
    private readonly TokenService _tokenService;
    private readonly Mock<IConfiguration> _mockConfig;

    public TokenServiceTests()
    {
        _mockConfig = new Mock<IConfiguration>();

        _mockConfig.Setup(config => config["Jwt:SecretKey"]).Returns("supersecurekey12345supersecurekey12345supersecurekey12345supersecurekey12345");
        _mockConfig.Setup(config => config["Jwt:Issuer"]).Returns("BackOffice");
        _mockConfig.Setup(config => config["Jwt:Audience"]).Returns("BackOffice");

        _tokenService = new TokenService(_mockConfig.Object);
    }

    [Fact]
    public void GenerateJwtToken_ShouldReturnValidToken()
    {
        var email = "test@example.com";

        var token = _tokenService.GenerateJwtToken(email);

        Assert.NotNull(token);
        Assert.NotEmpty(token);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        Assert.Contains(jwtToken.Claims, claim => claim.Type == ClaimTypes.Email && claim.Value == email);
    }

    [Fact]
    public void ValidateJwtToken_ShouldReturnEmail_WhenTokenIsValid()
    {
        var email = "test@example.com";
        var token = _tokenService.GenerateJwtToken(email);

        var validatedEmail = _tokenService.ValidateJwtToken(token);

        Assert.Equal(email, validatedEmail);
    }

    [Fact]
    public void ValidateJwtToken_ShouldReturnNull_WhenTokenIsInvalid()
    {
        var invalidToken = "invalid.token.value";

        var result = _tokenService.ValidateJwtToken(invalidToken);

        Assert.Null(result);
    }

    [Fact]
    public void ValidateJwtToken_ShouldReturnNull_WhenTokenIsExpired()
    {
        var expiredToken = GenerateExpiredJwtToken("expired@example.com");

        var result = _tokenService.ValidateJwtToken(expiredToken);

        Assert.Null(result);
    }

    private string GenerateExpiredJwtToken(string email)
    {
        var claims = new[] { new Claim(ClaimTypes.Email, email) };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_mockConfig.Object["Jwt:SecretKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _mockConfig.Object["Jwt:Issuer"],
            audience: _mockConfig.Object["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddSeconds(-1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
