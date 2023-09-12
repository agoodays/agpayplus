using BCryptNet = BCrypt.Net.BCrypt;

namespace AGooday.AgPay.Common.Utils
{
    /// <summary>
    /// $2a$(2 chars work cost)$(22 chars salt 随机的盐含符号)(31 chars hash 加密的hash含符号)
    /// https://github.com/lustan3216/BlogArticle/wiki/BCrypt-%E5%8A%A0%E5%AF%86%E6%BC%94%E7%AE%97%E6%B3%95%E7%B2%BE%E9%97%A2%E8%A7%A3%E9%87%8B
    /// https://blog.csdn.net/dreamboyxk/article/details/120045166
    /// https://img-blog.csdnimg.cn/1d2fc5001afd4fdb8a18d707b812b19e.png
    /// </summary>
    public class BCryptUtil
    {
        public static string Hash(string text, out string salt)
        {
            salt = BCryptNet.GenerateSalt();
            return BCryptNet.HashPassword(text, salt);
        }

        public static bool VerifyHash(string text, string hashedText)
        {
            return BCryptNet.Verify(text, hashedText);
        }
    }
}
