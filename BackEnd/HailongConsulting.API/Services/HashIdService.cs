using HashidsNet;

namespace HailongConsulting.API.Services;

/// <summary>
/// HashId服务实现 - 使用Hashids算法编码/解码整数ID
/// </summary>
public class HashIdService : IHashIdService
{
    private readonly Hashids _hashids;

    public HashIdService()
    {
        _hashids = new Hashids("HailongConsulting2024Portal", 8);
    }

    public string Encode(int id)
    {
        return _hashids.Encode(id);
    }

    public int? Decode(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            return null;

        var numbers = _hashids.Decode(hash);
        return numbers.Length > 0 ? numbers[0] : null;
    }
}
