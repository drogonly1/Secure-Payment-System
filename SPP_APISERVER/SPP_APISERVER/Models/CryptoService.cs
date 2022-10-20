using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SPP_APISERVER.Models
{
    public class CryptoService
    {
        public string privateKey = "<RSAKeyValue><Modulus>MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQC+N1zMl/vLYsl18o+3ZA2y4VkOlgLilGZ0bmom1gbRbLmIznVOfYrJfXSPPMyvb7phueAKVqQlmyoq8gphXlovQZ8V6se3nCCTYv4mnuQ0aRUQABA25hu+i2Cb5RNvyDMVr756qCstmCw593OYTkli2cPD/ujGanoc+yi+dRCVdRqwVx/Xdd2qbdAEwusgumrjQKqTal/Ab83q9nLvihX8/FD2ZRzHiROPcIwgIOZMJObIex0j4QyRERcu1UorFUE6CnCF8StumyqH4DU/uH0ok6IsIo8os8kp5BAoLdSBb+9+knKXS3JUWo3a/muxnux6FPjZXPiR4yP11DbnKs83AgMBAAECggEAB0+rcnW5COVAJxdurLbcy+5bdPMRI1Je6cTAscNyOaR7MSX2XlD2/62hyEum9mtN96mkUeFop8ViBoqNGZb90mksSzRd/tGyctfQOv3KebU8SQQnm2SW1mkXCobwi1BfnkrGAILsPfFmacf4GneP3IvQOX1XxyCalviUZVWskIIBusG1fzTyk/BFv6SevTbzUcEK+rjMpmkQ7RdRmDA56X8sZrxLe3jVRFiNqzjAvWecM+dI6iF/LPIzcqk52LfipVGkXyRuAXSMKrpArKdKnwlVwkh6Y4cexmjwBj3L3rQdunJ5Gwegg7gnc4l2491c6hJMvoW8u4UiM8jJWkdhKQKBgQD0GZ0bwDIqyFbOrEKz1o858ag+5EWPdoZwFDmgylWdpJ80THvb5BFZKx1bJIm32Th18cW9jnAOZ+ao6QNdTARfR46mXdelVGFQFEhcdVUXNxh1wdeLq7lRFIHKTCfN8ANQaDJW2ysjn0uHaNpE2KGw06fHz3XHf7iM5/jqKZ8mbQKBgQDHfUZwiT7XrxaHZrD1WaXNLOi7ymcJe9bwEKP0PZ0i54xhEYAl+x1oXDaH5bKf83JtBrqTKxjZu6ytZcOlVMcqYw7GURNpQHu/FfOTY3FlGNDDxw4SBTWV6H5hLdoceATQ0+P2/wY8RweWydq7stplrgbfCxLdm8imNHntNCwVswKBgBcR/KlMxvEIMjwiR8ObebnuPwqCpJhkiVw6bR/nP6RAlNNqjyI/MeyNiJ6+m02G+DEdgqThu3GBHGnGbAz7TEZ9CtxqbiEwEmpxIE2swTgKewNWKLsIpeEl4QZvoCt0jbuhvXA8Ep2xRoArxsVO/CksN+VIlEcgqR6YJgxrlqEZAoGBAJpdxVklytwpn1xNSZfBtDwtiTZvrAW5r5v6tHQ2DVdKJ+WhHjHFvH7aBFRqmSuOvbfCIjPpgIRT5o4TFi7kiG2zU2aArG2guWEnQmGF7ORXLIyihq/JGisTSmA2k/W9pbRWkqu5sgCI0kWLk1f1UWynGqgTsJiNiNmb99FmsspjAoGBAN3nl1PUiaBDPtSxKKpI+fNscQ3jeVsNZxYvAizuJGA84A3DkCysNaqYrd2GYW/x1Eoxodzeeja+SDu6Nzz1HkcCxcHwfpzJe9JUHRMlYjZMeKlxfy3AvpLgq4fZlG++6lhbuyfy7Y+QwUjXB6C9b9LdLe/AyGyrCIs3GNodfxyj</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        public string publicKey = "<RSAKeyValue><Modulus>MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAkGwo3Z7YQNge8fdP29RUhWaB+Fuz+rUa5GikN6AqZAP4g1RmreFK6Vgzd3armTJVd87mRf5h8VRtK7O4Ruhu0510lGODcU04LlDhauA0hAhCMsDYfc+6gSxTAfETLBu3H2aRBOufRgGkySRqpjdcLOM8mqCIaeWcpx7UlTu4Ry7A2ZRvaoVXH9joIXo9UqRJd5kM9pXX6/6qaKBvvkD1jrtavdFluNxPXT4M+tA4S3QFCPP6Qqgd0glJJwztHSw5aybsRWOJHHINFr0qDFqAiCixYjLROS/OAmbokI9Yx5Xqz46uAhK/m//tWlWHSdziRmqBCIEff1Ms2nmEaFla6QIDAQAB</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        public string signSHA256(string message, string key)
        {
            byte[] keyByte = Encoding.UTF8.GetBytes(key);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                string hex = BitConverter.ToString(hashmessage);
                hex = hex.Replace("-", "").ToLower();
                return hex;

            }
        }
        public string EncryptUri(string message)
        {
            //string json = "{\"transId\":\"" +
            //    transId + "\",\"shopId\":\"" +
            //    shopId + "\",\"amount\":\"" +
            //    amount + "\",\"oderInfo\":\"" +
            //    oderInfo + "\",\"responseTime\":\"" +
            //    responseTime + "\"}";
            string json = "transid=a&shopId=b";
            byte[] data = Encoding.UTF8.GetBytes(json);
            string result = null;
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                try
                {
                    // client encrypting data with public key issued by server
                    rsa.FromXmlString(publicKey);
                    var encryptedData = rsa.Encrypt(data, false);
                    var base64Encrypted = Convert.ToBase64String(encryptedData);
                    result = base64Encrypted;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
            return result;
        }
        public string EncryptUri(string source, string key)
        {
            TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();

            byte[] byteHash;
            byte[] byteBuff;

            byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
            desCryptoProvider.Key = byteHash;
            desCryptoProvider.Mode = CipherMode.ECB; //CBC, CFB
            byteBuff = Encoding.UTF8.GetBytes(source);

            string encoded =
                Convert.ToBase64String(desCryptoProvider.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            return encoded;
        }
        public string DecryptUri(string encodedText, string key)
        {
            TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();

            byte[] byteHash;
            byte[] byteBuff;

            byteHash = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
            desCryptoProvider.Key = byteHash;
            desCryptoProvider.Mode = CipherMode.ECB; //CBC, CFB
            byteBuff = Convert.FromBase64String(encodedText);

            string plaintext = Encoding.UTF8.GetString(desCryptoProvider.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
            return plaintext;
        }
        public string DatetimeToTimestamp(DateTime dateTime)
        {
            var UnixTimeStamp = dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            string timeStamp = Convert.ToInt64(UnixTimeStamp).ToString();
            return timeStamp;
        }
        public string TimestampToDatetime(string value)
        {
            var timeStamptoDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(long.Parse(value)).ToUniversalTime();
            string time = timeStamptoDateTime.ToString();
            return time;
        }
    }
}