//using System;
//using System.Threading.Tasks;
//using Xunit;
//using Moq;
//using Microsoft.Extensions.Configuration;
//using Backoffice.Domain.Shared;
//using Backoffice.Domain.Users;

//public class UserServiceTests
//{
//    private readonly UserService _userService;
//    private readonly Mock<IUserRepository> _mockUserRepo;
//    private readonly Mock<IEmailService> _mockEmailService;
//    private readonly Mock<TokenService> _mockTokenService;
//    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
//    private readonly Mock<IConfiguration> _mockConfig;

//    public UserServiceTests()
//    {
//        _mockUserRepo = new Mock<IUserRepository>();
//        _mockEmailService = new Mock<IEmailService>();
//        _mockTokenService = new Mock<TokenService>(Mock.Of<IConfiguration>());
//        _mockUnitOfWork = new Mock<IUnitOfWork>();
//        _mockConfig = new Mock<IConfiguration>();

//        _mockConfig.Setup(c => c["AppSettings:PasswordResetUrl"]).Returns("https://localhost:5001/api/Users/new-password/");

//        _userService = new UserService(
//            _mockUnitOfWork.Object,
//            _mockUserRepo.Object,
//            _mockEmailService.Object,
//            null,
//            null,
//            _mockTokenService.Object,
//            _mockConfig.Object);
//    }

//    [Fact]
//    public async Task SendPasswordResetLink_ShouldSendEmail_WhenUserIsValid()
//    {
//        var email = "teste@isep.ipp.pt";
//        var token = "valid_jwt_token";

//        var user = new User(Role.Doctor, email);
//        user.ActivateUser("AAAAAAAAAA!_1");

//        _mockUserRepo.Setup(repo => repo.getUserByEmail(email)).ReturnsAsync(user);
//        _mockTokenService.Setup(service => service.GenerateJwtToken(email)).Returns(token);

//        var result = await _userService.SendPasswordResetLink(email);

//        _mockEmailService.Verify(emailService => emailService.SendEmail(
//            email, $"{_mockConfig.Object["AppSettings:PasswordResetUrl"]}?token={token}", "Reset Password"), Times.Once);

//        Assert.Equal(user.Id.AsGuid(), result.Id);
//        Assert.Equal(user.Email.ToString(), result.Email);
//        Assert.True(result.Active);
//    }

//    [Fact]
//    public async Task SendPasswordResetLink_ShouldThrowException_WhenUserDoesNotExist()
//    {
//        var email = "nonexistent@example.com";
//        _mockUserRepo.Setup(repo => repo.getUserByEmail(email)).ReturnsAsync((User)null);

//        var exception = await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _userService.SendPasswordResetLink(email));
//        Assert.Equal("Error: Email doesn't exist.", exception.Message);
//    }

//    [Fact]
//    public async Task SendPasswordResetLink_ShouldThrowException_WhenUserIsInactive()
//    {
//        var email = "inactive@example.com";
//        var user = new User(Role.Doctor, email);

//        _mockUserRepo.Setup(repo => repo.getUserByEmail(email)).ReturnsAsync(user);

//        var exception = await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _userService.SendPasswordResetLink(email));
//        Assert.Equal("Error: User inactive.", exception.Message);
//    }

//    [Fact]
//    public async Task SendPasswordResetLink_ShouldThrowException_WhenUserRoleIsNotPermitted()
//    {
//        var email = "user@example.com";
//        var user = new User(Role.Patient, email);
//        user.ActivateUser("AAAAAAAAAA!_1");

//        _mockUserRepo.Setup(repo => repo.getUserByEmail(email)).ReturnsAsync(user);

//        var exception = await Assert.ThrowsAsync<BusinessRuleValidationException>(() => _userService.SendPasswordResetLink(email));
//        Assert.Equal("Error: User with this role can't ask for a password reset", exception.Message);
//    }

//    [Fact]
//    public async Task NewPassword_ShouldUpdatePassword_WhenTokenIsValid()
//    {
//        var email = "valid@example.com";
//        var newPassword = "NewSecurePassword!1";
//        var token = "valid_jwt_token";

//        var user = new User(Role.Doctor, email);
//        user.ActivateUser("AAAAAAAAAA!_1");

//        _mockTokenService.Setup(service => service.ValidateJwtToken(token)).Returns(email);
//        _mockUserRepo.Setup(repo => repo.getUserByEmail(email)).ReturnsAsync(user);

//        var result = await _userService.NewPassword(token, newPassword);

//        _mockUnitOfWork.Verify(unitOfWork => unitOfWork.CommitAsync(), Times.Once);
//        Assert.Equal(user.Id.AsGuid(), result.Id);
//        Assert.Equal(email, result.Email);
//        Assert.True(result.Active);
//    }

//}
