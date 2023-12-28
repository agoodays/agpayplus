namespace AGooday.AgPay.Common.Utils
{
    public class SmsVerificationCodeGenerator
    {
        private static readonly Random random = new Random();

        public static string GenerateCode(int length)
        {
            const string chars = "0123456789";
            char[] code = new char[length];

            for (int i = 0; i < length; i++)
            {
                code[i] = chars[random.Next(chars.Length)];
            }

            return new string(code);
        }
    }
}
