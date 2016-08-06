using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GemBox.Document;
using System.Text;
using System.IO;

namespace JobminxFinal.Controllers
{
    public class AppController : Controller
    {
        // GET: App
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Start()
        {
            return View();
        }
        [Authorize]
        public ActionResult One()
        {
            return View();
        }
        [Authorize]
        public ActionResult Two()
        {
            return View();
        }

        public ActionResult Welcome()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ResumeDocx(string resume)
        {

            ComponentInfo.SetLicense("DBTX-8EIK-50LW-QTKQ");

            string memString = resume;

            byte[] buffer = Encoding.ASCII.GetBytes(memString);
            MemoryStream ms = new MemoryStream(buffer);

            byte[] fileContents;

            DocumentModel document = DocumentModel.Load(ms, LoadOptions.HtmlDefault);
            var options = SaveOptions.DocxDefault;
            var ms2 = new MemoryStream();

            document.Save(ms2, options);

            fileContents = ms2.ToArray();

            return File(fileContents, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "MyResume.docx");




        }
    }
}