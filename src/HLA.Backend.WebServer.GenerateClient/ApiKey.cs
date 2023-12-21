using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HLA.Backend.WebServer.GenerateClient
{
    public static class ApiKey
    {
        public static string Generate()
        {
            using var provider = new RNGCryptoServiceProvider();
            var bytes = new byte[64];
            provider.GetBytes(bytes);

            return Convert.ToBase64String(bytes);
        }
    }
}
