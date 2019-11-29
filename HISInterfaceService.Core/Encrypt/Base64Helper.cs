using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HISInterfaceService.Core.Encrypt
{
    public class Base64Helper
    {
        /// <summary>
        /// Base64加密，采用utf8编码方式加密
        /// </summary>
        /// <param name="source">待加密的明文</param>
        /// <returns>加密后的字符串</returns>
        public static string Base64Encode(string source)
        {
            return Base64Encode(Encoding.UTF8, source);

        }
        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="encodeType">加密采用的编码方式</param>
        /// <param name="source">待加密的明文</param>
        /// <returns></returns>
        private static string Base64Encode(Encoding encodeType, string source)
        {
            string encode = string.Empty;
            byte[] bytes = encodeType.GetBytes(source);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {

                encode = source;
            }
            return encode;
        }
        /// <summary>
        ///  Base64解密，采用utf8编码方式解密
        /// </summary>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        public static string Base64Decoede(string result)
        {
            return Base64Decoede(Encoding.UTF8, result);
        }
        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="encodeType">解密采用的编码方式，和加密时采用的方式一致</param>
        /// <param name="result">待解密的密文</param>
        /// <returns>解密后的字符串</returns>
        private static string Base64Decoede(Encoding encodeType, string result)
        {
            string decode = string.Empty;
            try
            {
                Byte[] bytes = Convert.FromBase64String(result);
                decode = encodeType.GetString(bytes);
            }
            catch
            {
                decode = result;
            }
            return decode;
        }
    }
}
