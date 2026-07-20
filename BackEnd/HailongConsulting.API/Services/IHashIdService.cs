namespace HailongConsulting.API.Services;

/// <summary>
/// HashId服务接口 - 用于编码/解码整数ID到哈希字符串
/// </summary>
public interface IHashIdService
{
    /// <summary>
    /// 将整数ID编码为Hash字符串
    /// </summary>
    string Encode(int id);

    /// <summary>
    /// 将Hash字符串解码为整数ID
    /// </summary>
    int? Decode(string hash);
}
