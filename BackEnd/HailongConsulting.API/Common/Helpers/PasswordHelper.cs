using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace HailongConsulting.API.Common.Helpers;

/// <summary>
/// 密码帮助类
/// </summary>
public static class PasswordHelper
{
    private static readonly PasswordHasher<object> PasswordHasher = new();

    /// <summary>
    /// 生成使用 PBKDF2 的密码哈希。
    /// </summary>
    public static string HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        return PasswordHasher.HashPassword(new object(), password);
    }

    /// <summary>
    /// 验证密码
    /// </summary>
    public static bool VerifyPassword(string password, string hashedPassword, out bool needsRehash)
    {
        needsRehash = false;
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
        {
            return false;
        }

        // Existing databases used MD5. Allow one successful legacy login, then upgrade it.
        if (IsLegacyMd5Hash(hashedPassword))
        {
            var hashOfInput = ComputeLegacyMd5(password);
            var verified = CryptographicOperations.FixedTimeEquals(
                Encoding.ASCII.GetBytes(hashOfInput),
                Encoding.ASCII.GetBytes(hashedPassword.ToLowerInvariant()));
            needsRehash = verified;
            return verified;
        }

        var result = PasswordHasher.VerifyHashedPassword(new object(), hashedPassword, password);
        needsRehash = result == PasswordVerificationResult.SuccessRehashNeeded;
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }

    /// <summary>
    /// 生成随机密码
    /// </summary>
    public static string GenerateRandomPassword(int length = 12)
    {
        if (length < 3) throw new ArgumentOutOfRangeException(nameof(length));

        const string lower = "abcdefghijklmnopqrstuvwxyz";
        const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string digits = "0123456789";
        const string validChars = lower + upper + digits;
        var chars = new char[length];
        chars[0] = lower[RandomNumberGenerator.GetInt32(lower.Length)];
        chars[1] = upper[RandomNumberGenerator.GetInt32(upper.Length)];
        chars[2] = digits[RandomNumberGenerator.GetInt32(digits.Length)];
        for (var i = 3; i < length; i++)
        {
            chars[i] = validChars[RandomNumberGenerator.GetInt32(validChars.Length)];
        }

        return new string(chars.OrderBy(_ => RandomNumberGenerator.GetInt32(int.MaxValue)).ToArray());
    }

    private static bool IsLegacyMd5Hash(string value) =>
        value.Length == 32 && value.All(Uri.IsHexDigit);

    private static string ComputeLegacyMd5(string password)
    {
        var hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
