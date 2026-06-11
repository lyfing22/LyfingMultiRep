using System.Security.Cryptography;
using System.Text;

namespace DataMigrate.Infrastructure;

/// <summary>
/// 确定性 GUID 生成器：对相同输入始终输出相同 GUID。
/// 算法 = MD5(ASCII(input)) → Guid。
/// 用于确保同一 AccessionNumber 在 MongoDB 中始终对应同一 _id，
/// 实现幂等写入（同名检查号不会重复创建文档）。
/// </summary>
public static class IdGenerator
{
    /// <param name="input">输入字符串（通常是 AccessionNumber）</param>
    /// <returns>MD5 哈希作为 Guid（16 字节，同输入同输出）</returns>
    public static Guid FromString(string input)
    {
        var hash = MD5.HashData(Encoding.ASCII.GetBytes(input));
        return new Guid(hash);
    }
}
