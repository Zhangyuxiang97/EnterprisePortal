using AutoMapper;
using HailongConsulting.API.Common;
using HailongConsulting.API.Common.Helpers;
using HailongConsulting.API.Data;
using HailongConsulting.API.Models.DTOs;
using HailongConsulting.API.Models.Entities;
using HailongConsulting.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HailongConsulting.API.Services;

/// <summary>
/// 用户服务实现
/// </summary>
public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _dbContext;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUnitOfWork unitOfWork,
        ApplicationDbContext dbContext,
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<UserService> logger)
    {
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// 获取用户列表（分页）
    /// </summary>
    public async Task<PagedResult<UserDto>> GetPagedListAsync(UserQueryDto query)
    {
        var (items, total) = await _userRepository.GetPagedListAsync(
            query.Username,
            query.RealName,
            query.Role,
            query.Status,
            query.Page,
            query.PageSize);

        var dtos = _mapper.Map<List<UserDto>>(items);

        return new PagedResult<UserDto>
        {
            Items = dtos,
            TotalCount = total,
            PageIndex = query.Page,
            PageSize = query.PageSize
        };
    }

    /// <summary>
    /// 根据ID获取用户
    /// </summary>
    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null || user.IsDeleted == 1)
        {
            return null;
        }

        return _mapper.Map<UserDto>(user);
    }

    /// <summary>
    /// 创建用户
    /// </summary>
    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        // 检查用户名是否已存在
        if (await _userRepository.UsernameExistsAsync(dto.Username))
        {
            throw new InvalidOperationException("用户名已存在");
        }

        // 检查邮箱是否已存在
        if (await _userRepository.EmailExistsAsync(dto.Email))
        {
            throw new InvalidOperationException("邮箱已存在");
        }

        var user = new User
        {
            Username = dto.Username,
            Password = PasswordHelper.HashPassword(dto.Password),
            Email = dto.Email,
            Phone = dto.Phone,
            RealName = dto.RealName,
            Role = dto.Role,
            Status = dto.Status,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User created: {Username}", user.Username);

        return _mapper.Map<UserDto>(user);
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    public async Task<UserDto> UpdateAsync(int id, UpdateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null || user.IsDeleted == 1)
        {
            throw new InvalidOperationException("用户不存在");
        }

        // 检查邮箱是否已被其他用户使用
        if (await _userRepository.EmailExistsAsync(dto.Email, id))
        {
            throw new InvalidOperationException("邮箱已被其他用户使用");
        }

        user.Email = dto.Email;
        user.Phone = dto.Phone;
        user.RealName = dto.RealName;
        user.Role = dto.Role;
        user.Status = dto.Status;
        user.TokenVersion++;
        user.UpdatedAt = DateTime.Now;

        if (user.Status == 0)
        {
            await RevokeActiveSessionsAsync(user.Id);
        }

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User updated: {Username}", user.Username);

        return _mapper.Map<UserDto>(user);
    }

    /// <summary>
    /// 删除用户（软删除）
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null || user.IsDeleted == 1)
        {
            return false;
        }

        user.IsDeleted = 1;
        user.TokenVersion++;
        user.UpdatedAt = DateTime.Now;
        await RevokeActiveSessionsAsync(user.Id);

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User deleted: {Username}", user.Username);

        return true;
    }

    /// <summary>
    /// 重置用户密码
    /// </summary>
    public async Task<bool> ResetPasswordAsync(int id, ResetPasswordDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null || user.IsDeleted == 1)
        {
            return false;
        }

        user.Password = PasswordHelper.HashPassword(dto.NewPassword);
        user.TokenVersion++;
        user.UpdatedAt = DateTime.Now;
        await RevokeActiveSessionsAsync(user.Id);

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User password reset: {Username}", user.Username);

        return true;
    }

    /// <summary>
    /// 启用/禁用用户
    /// </summary>
    public async Task<bool> ToggleStatusAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null || user.IsDeleted == 1)
        {
            return false;
        }

        user.Status = (sbyte)(user.Status == 1 ? 0 : 1);
        user.TokenVersion++;
        user.UpdatedAt = DateTime.Now;

        if (user.Status == 0)
        {
            await RevokeActiveSessionsAsync(user.Id);
        }

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("User status toggled: {Username}, Status: {Status}", user.Username, user.Status);

        return true;
    }

    private async Task RevokeActiveSessionsAsync(int userId)
    {
        var activeTokens = await _dbContext.UserRefreshTokens
            .Where(token => token.UserId == userId && token.RevokedAt == null)
            .ToListAsync();
        foreach (var token in activeTokens)
        {
            token.RevokedAt = DateTime.UtcNow;
        }
    }
}
