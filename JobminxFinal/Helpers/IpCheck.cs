using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace JobminxFinal.Helpers
{
    //{"ip":"108.224.68.67","country_code":"US","country_name":"United States","region_code":"WI","region_name":"Wisconsin","city":"Milwaukee","zip_code":"53218","time_zone":"America/Chicago","latitude":43.116,"longitude":-87.994,"metro_code":617}
    //"statusCode": "OK",
    //"statusMessage": "",
    //"ipAddress": "108.224.68.67",
    //"countryCode": "US",
    //"countryName": "UNITED STATES",
    //"regionName": "WISCONSIN",
    //"cityName": "BUTLER",
    //"zipCode": "53007",
    //"latitude": "43.1058",
    //"longitude": "-88.0695",
    //"timeZone": "-06:00" 
    public class ipobj
    {
        public string statusCode { get; set; }
        public string statusMessage { get; set; }
        public string ipAddress { get; set; }
        public string countryCode { get; set; }
        public string countryName { get; set; }
        public string regionName { get; set; }
        public string cityName { get; set; }
        public string zipCode { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string timeZone { get; set; }
    }
    public class IpCheck
    {
        public string getIpData(string ip)
        {

            string ipzip = "";
            string url = "https://chrislim2888-ip-address-geolocation.p.mashape.com//?key=018dff050e84f8468e10e9cffaea3ade1e1f3ce8bd664387d240be42bdda1b7c&format=json&ip=" + ip;
            ipobj obj = new ipobj();
            HttpWebRequest web_request = (HttpWebRequest)WebRequest.Create(url);
            web_request.Headers.Add("X-Mashape-Key: v3wVxdpR5DmshhM91RxN7W9ksJT7p1DZBjrjsnjOZVWOrtoibf");
            var response_string = "";
            using (HttpWebResponse web_response = (HttpWebResponse)web_request.GetResponse())
            {
                using (StreamReader response_stream = new StreamReader(web_response.GetResponseStream()))
                {
                    response_string = response_stream.ReadToEnd();
                    obj = new JavaScriptSerializer().Deserialize<ipobj>(response_string);

                }

            }

            return obj.zipCode;
        }
    }
}