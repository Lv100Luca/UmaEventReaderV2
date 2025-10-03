using System.Security.Cryptography;
using System.Text;

namespace UmaEventReaderV2.Common;

public static class GuidGenerator
{
    public static Guid Generate(string umaName)
    {
        var bytes = Encoding.UTF8.GetBytes(umaName);

        var hash = MD5.HashData(bytes);

        return new Guid(hash);
    }
}