using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml.Serialization;

namespace SPP_APISERVER.Models
{
    public class CryptoRSA
    {
        private static RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
        private RSAParameters publickey;
        private RSAParameters privatekey;
        public CryptoRSA()
        {
            publickey = csp.ExportParameters(false);
            privatekey = csp.ExportParameters(true);
        }
        public string GetPublicKey()
        {
            var sw = new StringWriter();
            var xs = new XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, publickey);
            return sw.ToString();
        }
        public string EncryptRSA(string message)
        {
            csp = new RSACryptoServiceProvider();
            csp.ImportParameters(publickey);
            var data = Encoding.UTF8.GetBytes(message);
            var cypher = csp.Encrypt(data, false);
            return Convert.ToBase64String(cypher);
        }
        public string DecryptRSA(string cypher)
        {
            var data = Convert.FromBase64String(cypher);
            csp.ImportParameters(privatekey);
            var message = csp.Decrypt(data, false);
            return Encoding.ASCII.GetString(message);
        }
        public string GetPrivateKey()
        {
            var sw = new StringWriter();
            var xs = new XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, privatekey);
            return sw.ToString();
        }
    }
}