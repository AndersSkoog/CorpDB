using System.IO;
using Nancy.IO;
using Nancy;
using Nancy.Responses;
using System;
using Nancy.Routing;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using MongoDB.Bson;

namespace corpdb
{



    public static class utils {


        public static string validjson(BsonDocument doc)
        {
            var json = doc.ToJson();
            var result = Regex.Match(json, @"ObjectId\(([^\)]*)\)").Value;
            var id = result.Replace("ObjectId(", string.Empty).Replace(")", String.Empty);
            return json.Replace(result, id);
        }
        public static List<string> writeaccess(string userrole)
        {
            switch (userrole)
            {
                case "shareholder":
                    return new List<string> { };
                case "emploee":
                    return new List<string> { "customer" };
                case "manager":
                    return new List<string> { "customer", "emploee" };
                case "boardmember":
                    return new List<string> { "customer", "emploee", "manager"};
                case "ceo":
                    return new List<string> { "customer", "emploee", "manager", "boardmember"};
                case "admin":
                    return new List<string> { "customer", "emploee", "manager", "boardmember", "ceo", "shareholder"};
                default:
                    return new List<string> { };
            }
        }
        public static bool haswriteaccess(string userrole, string subjectrole)
        {
            return writeaccess(userrole).Contains(subjectrole);
        }
        public static string hasher(string input) {
            var sha1 = new SHA1CryptoServiceProvider();
            var hashedbytes = sha1.ComputeHash(Encoding.ASCII.GetBytes(input));
            return Convert.ToBase64String(hashedbytes);
        }
        public static bool stringvalidation(string input, string hashstore){
            return hasher(input) == hashstore;
        }
        public static string randstr(int minchars, int maxchars)
        {
            var rnd = new Random();
            Func<char, int, char> rndchar = (start, end) => { return (char)(start + rnd.Next(0, end)); };// Zero to 25
            string output = "";
            for (int i = 0; i < rnd.Next(minchars,maxchars); i++)
            {
                char[] arr = new char[] { rndchar('A', 26), rndchar('0', 9), rndchar('a', 26) };
                output += arr[rnd.Next(0, 3)];
            }
            return output;
        }
        public static void print(string txt) {
            System.Diagnostics.Debug.WriteLine(txt);
        }
    }
    public static class RequestBodyExtensions
    {
        public static string ReadAsString(this RequestStream requestStream)
        {
            using (var reader = new StreamReader(requestStream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}