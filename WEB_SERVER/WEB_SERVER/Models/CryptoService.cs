using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WEB_SERVER.Models
{
    public class CryptoService
    {
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