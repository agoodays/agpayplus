using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Common.Utils
{
    public class AgPayUtil
    {
        public static string AES_KEY = "4ChT08phkz59hquD795X7w==";

        public static string AesEncode(string data)
        {
           return EnDecryptUtil.AESEncryptToHex(data, AES_KEY);
        }
        public static string AesDecode(string data)
        {
            return EnDecryptUtil.AESDecryptUnHex(data, AES_KEY);
        }
    }
}
