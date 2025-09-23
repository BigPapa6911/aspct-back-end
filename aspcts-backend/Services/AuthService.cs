// Services/AuthService.cs
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using BCrypt.Net;
using aspcts_backend.Services.Interfaces;
using aspcts_backend.Repositories.Interfaces;
using aspcts_backend.Models.DTOs.Auth;
using aspcts_backend.Models.Entities;
using aspcts_backend.Models.Configuration;
using aspcts_backend.Data;

namespace aspcts_backend.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly ApplicationDbContext _context;
    private readonly JwtSettings _jwtSettings;
    
    public AuthService(IUserRepository userRepository, ApplicationDbContext context, IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _context = context;
        _jwtSettings = jwtSettings.Value;
    }
    
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        
        if (user == null || !user.IsActive)
            throw new UnauthorizedAccessException("Email ou senha inválidos");
        
        if (!BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Email ou senha inválidos");
        
        // Update last login
        user.LastLogin = DateTime.UtcNow;
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();
        
        var token = GenerateJwtToken(user.UserId, user.Email, user.Role);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);
        
        return new LoginResponse
        {
            Token = token,
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ExpiresAt = expiresAt
        };
    }
    
    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        // Validate unique email and username
        if (await _userRepository.EmailExistsAsync(request.Email))
            throw new InvalidOperationException("Este email já está em uso");
        
        if (await _userRepository.UsernameExistsAsync(request.Username))
            throw new InvalidOperationException("Este nome de usuário já está em uso");
        
        // Validate role
        if (request.Role != "Psychologist" && request.Role != "Parent")
            throw new ArgumentException("Role deve ser 'Psychologist' ou 'Parent'");
        
        // Create user
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.HashPassword(request.Password),
            Role = request.Role,
            FirstName = request.FirstName,
            LastName = request.LastName,
            ContactNumber = request.ContactNumber
        };
        
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        
        // Create role-specific record
        if (request.Role == "Psychologist")
        {
            var psychologist = new Psychologist
            {
                UserId = user.UserId,
                LicenseNumber = request.LicenseNumber,
                Specialization = request.Specialization,
                ClinicName = request.ClinicName
            };
            
            _context.Psychologists.Add(psychologist);
        }
        else if (request.Role == "Parent")
        {
            var parent = new Parent
            {
                UserId = user.UserId,
                ChildRelationship = request.ChildRelationship ?? "Parent"
            };
            
            _context.Parents.Add(parent);
        }
        
        // Save all changes
        await _context.SaveChangesAsync();
        
        var token = GenerateJwtToken(user.UserId, user.Email, user.Role);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes);
        
        return new LoginResponse
        {
            Token = token,
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ExpiresAt = expiresAt
        };
    }
    
    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
            
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
            
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    public string GenerateJwtToken(Guid userId, string email, string role)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("userId", userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim("jti", Guid.NewGuid().ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}