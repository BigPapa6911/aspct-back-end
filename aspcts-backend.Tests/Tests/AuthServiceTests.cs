// Tests/AuthServiceTests.cs
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Options;
using aspcts_backend.Services;
using aspcts_backend.Repositories.Interface;
using aspcts_backend.Models.Entities;
using aspcts_backend.Models.DTOs.Auth;
using aspcts_backend.Models.Configuration;
using aspcts_backend.Data;

namespace aspcts_backend.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly IOptions<JwtSettings> _jwtSettings;
    private readonly Mock<ApplicationDbContext> _contextMock;
    private readonly AuthService _authService;
    
    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _contextMock = new Mock<ApplicationDbContext>();
        
        _jwtSettings = Options.Create(new JwtSettings
        {
            SecretKey = "TestSecretKeyThatIsAtLeast32CharactersLong!",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpiryMinutes = 60
        });
        
        _authService = new AuthService(_userRepositoryMock.Object, _contextMock.Object, _jwtSettings);
    }
    
    private User CreateTestUser(string email = "test@test.com", string role = "Psychologist")
    {
        return new User
        {
            UserId = Guid.NewGuid(),
            Username = "testuser",
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
            Role = role,
            FirstName = "Test",
            LastName = "User",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
    
    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsLoginResponse()
    {
        // Arrange
        var testUser = CreateTestUser();
        var loginRequest = new LoginRequest
        {
            Email = "test@test.com",
            Password = "Test@123"
        };
        
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(testUser);
        
        // Act
        var result = await _authService.LoginAsync(loginRequest);
        
        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.Email.Should().Be(testUser.Email);
        result.Role.Should().Be(testUser.Role);
    }
    
    [Fact]
    public async Task LoginAsync_WithInvalidEmail_ThrowsUnauthorizedException()
    {
        // Arrange
        var loginRequest = new LoginRequest
        {
            Email = "invalid@test.com",
            Password = "Test@123"
        };
        
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);
        
        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _authService.LoginAsync(loginRequest)
        );
    }
    
    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ThrowsUnauthorizedException()
    {
        // Arrange
        var testUser = CreateTestUser();
        var loginRequest = new LoginRequest
        {
            Email = "test@test.com",
            Password = "WrongPassword"
        };
        
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(testUser);
        
        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _authService.LoginAsync(loginRequest)
        );
    }
    
    [Fact]
    public async Task LoginAsync_WithInactiveUser_ThrowsUnauthorizedException()
    {
        // Arrange
        var testUser = CreateTestUser();
        testUser.IsActive = false;
        
        var loginRequest = new LoginRequest
        {
            Email = "test@test.com",
            Password = "Test@123"
        };
        
        _userRepositoryMock
            .Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(testUser);
        
        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _authService.LoginAsync(loginRequest)
        );
    }
    
    [Fact]
    public async Task RegisterAsync_WithValidData_CreatesUser()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Username = "newuser",
            Email = "newuser@test.com",
            Password = "Test@123",
            Role = "Psychologist",
            FirstName = "New",
            LastName = "User",
            LicenseNumber = "CRP-12345",
            Specialization = "ABA Therapy"
        };
        
        _userRepositoryMock
            .Setup(x => x.EmailExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        
        _userRepositoryMock
            .Setup(x => x.UsernameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        
        // Act
        var result = await _authService.RegisterAsync(registerRequest);
        
        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.Email.Should().Be(registerRequest.Email);
        result.Role.Should().Be("Psychologist");
    }
    
    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ThrowsException()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Username = "newuser",
            Email = "existing@test.com",
            Password = "Test@123",
            Role = "Parent",
            FirstName = "New",
            LastName = "User"
        };
        
        _userRepositoryMock
            .Setup(x => x.EmailExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
        
        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _authService.RegisterAsync(registerRequest)
        );
        
        ex.Message.Should().Contain("email já está em uso");
    }
    
    [Fact]
    public async Task RegisterAsync_WithInvalidRole_ThrowsException()
    {
        // Arrange
        var registerRequest = new RegisterRequest
        {
            Username = "newuser",
            Email = "newuser@test.com",
            Password = "Test@123",
            Role = "InvalidRole",
            FirstName = "New",
            LastName = "User"
        };
        
        _userRepositoryMock
            .Setup(x => x.EmailExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        
        _userRepositoryMock
            .Setup(x => x.UsernameExistsAsync(It.IsAny<string>()))
            .ReturnsAsync(false);
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            async () => await _authService.RegisterAsync(registerRequest)
        );
    }
    
    [Fact]
    public void GenerateJwtToken_WithValidData_ReturnsToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@test.com";
        var role = "Psychologist";
        
        // Act
        var token = _authService.GenerateJwtToken(userId, email, role);
        
        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Split('.').Should().HaveCount(3); // JWT tem 3 partes
    }
    
    [Fact]
    public async Task ValidateTokenAsync_WithValidToken_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@test.com";
        var role = "Psychologist";
        var token = _authService.GenerateJwtToken(userId, email, role);
        
        // Act
        var isValid = await _authService.ValidateTokenAsync(token);
        
        // Assert
        isValid.Should().BeTrue();
    }
    
    [Fact]
    public async Task ValidateTokenAsync_WithInvalidToken_ReturnsFalse()
    {
        // Arrange
        var invalidToken = "invalid.token.here";
        
        // Act
        var isValid = await _authService.ValidateTokenAsync(invalidToken);
        
        // Assert
        isValid.Should().BeFalse();
    }
}