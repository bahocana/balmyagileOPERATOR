using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BalmyAgilev1
{
    public static class ApiHelper
    {
        public static string SendRequest(string url, string jsonRequest)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(jsonRequest);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (jsonRequest.Contains("login"))
                    {
                        dynamic jsonResponse = JsonConvert.DeserializeObject(result);
                        return jsonResponse?.msg; // Login için session ID döner
                    }
                    return result; // Diğer istekler için tam yanıt
                }
            }
            catch (WebException ex)
            {
                using (var streamReader = new StreamReader(ex.Response.GetResponseStream()))
                {
                    var error = streamReader.ReadToEnd();
                    //MessageBox.Show("Error: " + error);
                    return null;
                }
            }
        }
    }
}
