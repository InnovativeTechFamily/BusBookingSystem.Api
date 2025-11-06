using BusBookingSystem.Api.Models.DTOs.Auth;
using BusBookingSystem.Api.Models.Entities;
//using BusBookingSystem.API.Interfaces;
//using BusBookingSystem.API.Models.DTOs.Auth;
//using BusBookingSystem.API.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BusBookingSystem.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthService(UserManager<ApplicationUser> userManager,
                         RoleManager<IdentityRole> roleManager,
                         IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "User already exists with this email"
                    };
                }

                // Create new user
                var user = new ApplicationUser
                {
                    FullName = request.FullName,
                    UserName = request.Email,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = string.Join(", ", result.Errors.Select(e => e.Description))
                    };
                }

                // Add user to Customer role
                await _userManager.AddToRoleAsync(user, "Customer");

                // Generate JWT token
                var token = await GenerateJwtToken(user);

                return new AuthResponse
                {
                    Success = true,
                    Message = "Registration successful",
                    Data = new AuthData
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        FullName = user.FullName,
                        Token = token,
                        RefreshToken = GenerateRefreshToken(),
                        ExpiresIn = 3600,
                        Role = "Customer"
                    }
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = $"Registration failed: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    };
                }

                var result = await _userManager.CheckPasswordAsync(user, request.Password);
                if (!result)
                {
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Invalid email or password"
                    };
                }

                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "Customer";

                var token = await GenerateJwtToken(user);

                return new AuthResponse
                {
                    Success = true,
                    Message = "Login successful",
                    Data = new AuthData
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        FullName = user.FullName,
                        Token = token,
                        RefreshToken = GenerateRefreshToken(),
                        ExpiresIn = 3600,
                        Role = role
                    }
                };
            }
            catch (Exception ex)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = $"Login failed: {ex.Message}"
                };
            }
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add roles
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            return $"rt_{Guid.NewGuid():N}";
        }
    }
}