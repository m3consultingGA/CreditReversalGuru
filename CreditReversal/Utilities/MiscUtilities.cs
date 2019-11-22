using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace CreditReversal.Utilities
{
    public class MiscUtilities
    {
        public static byte[] PasswordHash(string password)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            UTF8Encoding encoder = new UTF8Encoding();

            byte[] data = md5Hasher.ComputeHash(encoder.GetBytes(password));

            return data;
        }
    }
}