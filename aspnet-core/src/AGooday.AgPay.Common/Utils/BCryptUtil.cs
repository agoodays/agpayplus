using BCryptNet = BCrypt.Net.BCrypt;

namespace AGooday.AgPay.Common.Utils
{
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
