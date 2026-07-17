using System.Security.Cryptography;
using System.Text;
using HailongConsulting.API.Common.Helpers;
using HailongConsulting.API.Data;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Models.Entities;
using HailongConsulting.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HailongConsulting.API.Services;

/// <summary>
/// 认证服务实现。
/// </summary>
public class AuthService : IAuthService
{
    private const int MaxFailedLoginAttempts = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _dbContext;
    private readonly JwtHelper _jwtHelper;
    private readonly IConfiguration _configuration;

    public AuthService(
        IUnitOfWork unitOfWork,
        ApplicationDbContext dbContext,
        JwtHelper jwtHelper,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
        _jwtHelper = jwtHelper;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var now = DateTime.UtcNow;
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsDeleted == 0);

        if (user == null || user.Status == 0)
        {
            return null;
        }

        if (user.LockoutUntil.HasValue && user.LockoutUntil.Value > now)
        {
            return null;
        }

        if (!PasswordHelper.VerifyPassword(request.Password, user.Password, out var needsRehash))
        {
            user.FailedLoginCount++;
            if (user.FailedLoginCount >= MaxFailedLoginAttempts)
            {
                user.FailedLoginCount = 0;
                user.LockoutUntil = now.Add(LockoutDuration);
            }

            await _dbContext.SaveChangesAsync();
            return null;
        }

        if (needsRehash)
        {
            user.Password = PasswordHelper.HashPassword(request.Password);
        }

        user.FailedLoginCount = 0;
        user.LockoutUntil = null;
        user.LastLoginTime = DateTime.Now;
        user.UpdatedAt = DateTime.Now;

        var token = _jwtHelper.GenerateToken(user.Id, user.Username, user.Role, user.TokenVersion);
        var refreshToken = _jwtHelper.GenerateRefreshToken();
        _dbContext.UserRefreshTokens.Add(CreateRefreshToken(user.Id, refreshToken, now));
        await _dbContext.SaveChangesAsync();

        return CreateLoginResponse(user, token, refreshToken);
    }

    public async Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return null;
        }

        var now = DateTime.UtcNow;
        var refreshTokenHash = HashRefreshToken(refreshToken);
        var storedToken = await _dbContext.UserRefreshTokens
            .SingleOrDefaultAsync(token => token.TokenHash == refreshTokenHash);

        if (storedToken == null)
        {
            return null;
        }

        if (storedToken.RevokedAt != null)
        {
            // 已轮换令牌再次出现说明令牌可能被窃取，撤销该用户全部会话。
            await RevokeAllRefreshTokensAsync(storedToken.UserId, now);
            return null;
        }

        if (storedToken.ExpiresAt <= now)
        {
            storedToken.RevokedAt = now;
            await _dbContext.SaveChangesAsync();
            return null;
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(user =>
            user.Id == storedToken.UserId && user.Status == 1 && user.IsDeleted == 0);
        if (user == null)
        {
            storedToken.RevokedAt = now;
            await _dbContext.SaveChangesAsync();
            return null;
        }

        var newRefreshToken = _jwtHelper.GenerateRefreshToken();
        storedToken.LastUsedAt = now;
        storedToken.RevokedAt = now;
        _dbContext.UserRefreshTokens.Add(CreateRefreshToken(user.Id, newRefreshToken, now));

        var token = _jwtHelper.GenerateToken(user.Id, user.Username, user.Role, user.TokenVersion);
        await _dbContext.SaveChangesAsync();

        return CreateLoginResponse(user, token, newRefreshToken);
    }

    public async Task<UserInfoDto?> GetCurrentUserAsync(int userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);

        if (user == null || user.Status == 0 || user.IsDeleted != 0)
        {
            return null;
        }

        return new UserInfoDto
        {
            UserId = user.Id,
            Username = user.Username,
            FullName = user.RealName,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
            LastLogin = user.LastLoginTime
        };
    }

    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequestDto request)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);

        if (user == null || user.Status == 0 || user.IsDeleted != 0)
        {
            return false;
        }

        if (!PasswordHelper.VerifyPassword(request.OldPassword, user.Password, out _))
        {
            return false;
        }

        user.Password = PasswordHelper.HashPassword(request.NewPassword);
        user.TokenVersion++;
        user.UpdatedAt = DateTime.Now;
        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        await RevokeAllRefreshTokensAsync(user.Id, DateTime.UtcNow, saveChanges: false);
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RevokeRefreshTokenAsync(int userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
        {
            return false;
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        user.TokenVersion++;
        user.UpdatedAt = DateTime.Now;
        await RevokeAllRefreshTokensAsync(userId, DateTime.UtcNow, saveChanges: false);
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        var principal = _jwtHelper.ValidateToken(token);
        return await Task.FromResult(principal != null);
    }

    public Task<LoginResponseDto?> LoginAsync(LoginDto loginDto) => LoginAsync(new LoginRequestDto
    {
        Username = loginDto.Username,
        Password = loginDto.Password
    });

    public Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword) =>
        ChangePasswordAsync(userId, new ChangePasswordRequestDto
        {
            OldPassword = oldPassword,
            NewPassword = newPassword
        });

    private LoginResponseDto CreateLoginResponse(User user, string token, string refreshToken)
    {
        var expireHours = Convert.ToDouble(_configuration["Jwt:ExpireHours"]);
        return new LoginResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            FullName = user.RealName,
            Email = user.Email,
            Role = user.Role,
            Token = token,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(expireHours)
        };
    }

    private static UserRefreshToken CreateRefreshToken(int userId, string rawToken, DateTime now) => new()
    {
        UserId = userId,
        TokenHash = HashRefreshToken(rawToken),
        CreatedAt = now,
        ExpiresAt = now.Add(RefreshTokenLifetime)
    };

    private async Task RevokeAllRefreshTokensAsync(int userId, DateTime now, bool saveChanges = true)
    {
        var activeTokens = await _dbContext.UserRefreshTokens
            .Where(token => token.UserId == userId && token.RevokedAt == null)
            .ToListAsync();
        foreach (var token in activeTokens)
        {
            token.RevokedAt = now;
        }

        if (saveChanges && activeTokens.Count > 0)
        {
            await _dbContext.SaveChangesAsync();
        }
    }

    private static string HashRefreshToken(string refreshToken) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
}
