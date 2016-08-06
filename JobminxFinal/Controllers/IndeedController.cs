using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace JobminxFinal.Controllers
{
    public class IndeedController : Controller
    {
        // GET: Indeed
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchIndeed(string search, string loc)
        {
            HttpWebRequest web_request = (HttpWebRequest)WebRequest.Create("http://api.indeed.com/ads/apisearch?publisher=5422421003768593&q=" + search + "&l=" + loc + "&sort=&radius=&st=&jt=&format=json&start=&limit=&fromage=&filter=&latlong=1&co=us&chnl=&userip=&useragent=&v=2");
            web_request.Method = "GET";
            web_request.ContentType = "application/json; charset=utf-8";
            var response_string = "";
            using (HttpWebResponse web_response = (HttpWebResponse)web_request.GetResponse())
            {
                using (StreamReader response_stream = new StreamReader(web_response.GetResponseStream()))
                {
                    response_string = response_stream.ReadToEnd();
                }
            }

            return Content(response_string, "application/json");
        }

        [HttpPost]
        public ActionResult GetJobFromIndeed(string url)
        {


            HttpWebRequest web_request = (HttpWebRequest)WebRequest.Create(url);
            var response_string = "";
            using (HttpWebResponse web_response = (HttpWebResponse)web_request.GetResponse())
            {
                using (StreamReader response_stream = new StreamReader(web_response.GetResponseStream()))
                {
                    response_string = response_stream.ReadToEnd();
                }
            }

            HtmlDocument doc = new HtmlDocument();

            doc.LoadHtml(response_string);

            var jobdesc = doc.GetElementbyId("job_summary").InnerHtml;
            //split by Compensation:???//
            var jobtitle = doc.GetElementbyId("job_header").InnerHtml;

            return Json(new { jobdesc = jobdesc, jobtitle = jobtitle });
        }


    }
}