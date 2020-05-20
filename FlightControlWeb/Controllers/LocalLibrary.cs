using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FlightControlWeb.Controllers
{
    public static class LocalLibrary
    {
        public static DateTime ConvertToDateTime(string str)
        {
            string pattern = @"(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2}):(\d{2})Z";
            if (Regex.IsMatch(str, pattern))
            {
                Match match = Regex.Match(str, pattern);
                int second = Convert.ToInt32(match.Groups[6].Value);
                int minute = Convert.ToInt32(match.Groups[5].Value);
                int hour = Convert.ToInt32(match.Groups[4].Value);
                int day = Convert.ToInt32(match.Groups[3].Value);
                int month = Convert.ToInt32(match.Groups[2].Value);
                int year = Convert.ToInt32(match.Groups[1].Value);
                return new DateTime(year, month, day, hour, minute, second);
            }
            else
            {
                throw new Exception("Unable to parse");
            }
        }

        public static T GetFlightFromServer<T>(string serverUrl)
        {
            string get = String.Format(serverUrl);
            // Create request.
            WebRequest request = WebRequest.Create(get);
            request.Method = "GET";
            HttpWebResponse response = null;
            // Get response.
            response = (HttpWebResponse)request.GetResponse();
            string result = null;
            // Get data - Json file.
            using (Stream stream = response.GetResponseStream())
            {
                StreamReader sr = new StreamReader(stream);
                result = sr.ReadToEnd();
                sr.Close();
            }
            if (result == "" || result == null)
            {
                return default;
            }
            // Convert to object.
            T externalFlights = JsonConvert.DeserializeObject<T>(result);
            return externalFlights;
        }

        public static char getLetter()
        {
            // create rendom letter.
            var rand = new Random();
            int num = rand.Next(0, 26);
            char letter = (char)('A' + num);
            return letter;
        }

        public static int getNumber()
        {
            // create random number.
            var rand = new Random();
            int num = rand.Next(0, 10);
            return num;
        }
    }

}