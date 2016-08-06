using AlchemyAPIClient;
using AlchemyAPIClient.Responses;
using GemBox.Document;
using HtmlAgilityPack;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using JobminxFinal.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace jobminxFinal.Controllers
{
    public class ParseController : ApiController
    {

        public class jobdata
        {
            public string jobtext { get; set; }
        }

        public class resumeData
        {
            public string resumeText { get; set; }
        }

        public class resumeUrl
        {
            public string url { get; set; }
        }



        [HttpPost]
        public async Task<IHttpActionResult> PostResumeParse()
        {
            string xJson = "";
            string zip = "";
            //resume res = new resume();
            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);
            try
            {

                foreach (var file in provider.Contents)
                {
                    var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                    var buffer = await file.ReadAsByteArrayAsync();
                    //Do whatever you want with filename and its binaray data.
                    Stream stream = new MemoryStream(buffer);
                    string contentType = file.Headers.ContentType.ToString();
                    DocumentParser doc = new DocumentParser();
                    string contents = doc.parse(stream, contentType);
                    HtmlUtility cleaner = new HtmlUtility();
                    contents = cleaner.SanitizeHtml(contents);


                    var client = new AlchemyClient("028a44801d2c3bbb341a5b727046a9866f760d40");
                    alchemy keys = new alchemy();
                    AlchemyKeywordsResponse key = await keys.GetKeywords(contents, client);
                    AlchemyEntitiesResponse ent = await keys.GetEntities(contents, client);
                    AlchemyConceptsResponse con = await keys.GetConcepts(contents, client);
                    var fieldT = (from r in ent.Entities.OrderByDescending(r => r.Relevance)
                                  where r.Type == "FieldTerminology"
                                  where r.Relevance > 0.2
                                  select r.Text).ToList();

                    var nonFieldT = (from r in ent.Entities
                                     where r.Type != "FieldTerminology"
                                     select r.Text).ToList();

                    var keywords = (from r in key.Keywords.OrderByDescending(r => r.Relevance)
                                    where r.Text != ""
                                    where r.Relevance > 0.2
                                    select r.Text).ToList();

                    var concepts = (from r in con.Concepts.OrderByDescending(r => r.Relevance)
                                    where r.Text != ""
                                    where r.Relevance > 0.2
                                    select r.Text).ToList();

                    var jobtitle = (from r in ent.Entities.OrderByDescending(r => r.Relevance)
                                    where r.Type == "JobTitle"
                                    where r.Relevance > 0.2
                                    select r.Text).FirstOrDefault();

                    List<string> black = new List<string>();
                    black.Add("br");
                    black.Add("/b");

                    var cleankeywords = keywords.Except(nonFieldT).ToList();
                    var cleanconcepts = concepts.Except(nonFieldT).ToList();

                    cleankeywords = keywords.Except(black).ToList();
                    cleanconcepts = concepts.Except(black).ToList();

                    //ViewBag.Keywords = cleankeywords;
                    //ViewBag.Entities = fieldT;
                    //ViewBag.Concepts = cleanconcepts;
                    alchemyCompare res = new alchemyCompare();
                    var allProducts = cleanconcepts.Concat(cleankeywords)
                                    .Concat(fieldT)
                                    .ToList();
                    res.Competencies = allProducts.Except(black).ToList();
                    res.Concepts = cleanconcepts.Except(black).ToList();
                    res.Keywords = cleankeywords.Except(black).ToList();
                    res.Skills = fieldT.Except(black).ToList();
                    res.JobTitle = jobtitle;
                    res.Text = contents;

                    var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    xJson = oSerializer.Serialize(res);

                    string ip = HttpContext.Current.Request.UserHostAddress.ToString();
                    IpCheck ipx = new IpCheck();
                    zip = ipx.getIpData(ip);

                    //zip = "95350";

                }


            }
            catch (Exception e)
            {
                return Json(new { error = e.Message, results = xJson, zipcode = "95350" });
            }


            return Json(new { results = xJson, zipcode = zip });
        }

        [HttpPost]
        public async Task<IHttpActionResult> TextResumeParse([FromBody] resumeData resume)
        {
            string xJson = "";
            string zip = "";

            try
            {


                string contents = resume.resumeText;
                HtmlUtility cleaner = new HtmlUtility();
                contents = cleaner.SanitizeHtml(contents);


                var client = new AlchemyClient("028a44801d2c3bbb341a5b727046a9866f760d40");
                alchemy keys = new alchemy();
                AlchemyKeywordsResponse key = await keys.GetKeywords(contents, client);
                AlchemyEntitiesResponse ent = await keys.GetEntities(contents, client);
                AlchemyConceptsResponse con = await keys.GetConcepts(contents, client);
                var fieldT = (from r in ent.Entities.OrderByDescending(r => r.Relevance)
                              where r.Type == "FieldTerminology"
                              where r.Relevance > 0.2
                              select r.Text).ToList();

                var nonFieldT = (from r in ent.Entities
                                 where r.Type != "FieldTerminology"
                                 select r.Text).ToList();

                var keywords = (from r in key.Keywords.OrderByDescending(r => r.Relevance)
                                where r.Text != ""
                                where r.Relevance > 0.2
                                select r.Text).ToList();

                var concepts = (from r in con.Concepts.OrderByDescending(r => r.Relevance)
                                where r.Text != ""
                                where r.Relevance > 0.2
                                select r.Text).ToList();

                var jobtitle = (from r in ent.Entities.OrderByDescending(r => r.Relevance)
                                where r.Type == "JobTitle"
                                where r.Relevance > 0.2
                                select r.Text).FirstOrDefault();
                List<string> black = new List<string>();
                black.Add("br");
                black.Add("/b");
                var cleankeywords = keywords.Except(nonFieldT).ToList();
                var cleanconcepts = concepts.Except(nonFieldT).ToList();

                cleankeywords = keywords.Except(black).ToList();
                cleanconcepts = concepts.Except(black).ToList();
                //ViewBag.Keywords = cleankeywords;
                //ViewBag.Entities = fieldT;
                //ViewBag.Concepts = cleanconcepts;
                alchemyCompare res = new alchemyCompare();
                var allProducts = cleanconcepts.Concat(cleankeywords)
                                .Concat(fieldT)
                                .ToList();
                res.Competencies = allProducts.Except(black).ToList();
                res.Concepts = cleanconcepts.Except(black).ToList();
                res.Keywords = cleankeywords.Except(black).ToList();
                res.Skills = fieldT.Except(black).ToList();
                res.JobTitle = jobtitle;
                res.Text = contents;

                var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                xJson = oSerializer.Serialize(res);

                string ip = HttpContext.Current.Request.UserHostAddress.ToString();
                IpCheck ipx = new IpCheck();
                zip = ipx.getIpData(ip);

                //zip = "95350";




            }
            catch (Exception e)
            {
                return Json(new { error = e.Message, results = xJson, zipcode = "95350" });
            }


            return Json(new { results = xJson, zipcode = zip });
        }



        [HttpPost]
        public async Task<IHttpActionResult> PostJobParse([FromBody] jobdata job)
        {


            var client = new AlchemyClient("028a44801d2c3bbb341a5b727046a9866f760d40");
            alchemy keys = new alchemy();
            AlchemyKeywordsResponse key = await keys.GetKeywords(job.jobtext, client);
            AlchemyEntitiesResponse ent = await keys.GetEntities(job.jobtext, client);
            AlchemyConceptsResponse con = await keys.GetConcepts(job.jobtext, client);
            var fieldT = (from r in ent.Entities.OrderByDescending(r => r.Relevance)
                          where r.Type == "FieldTerminology"
                          where r.Relevance > 0.2
                          select r.Text).ToList();

            var nonFieldT = (from r in ent.Entities
                             where r.Type != "FieldTerminology"
                             select r.Text).ToList();

            var keywords = (from r in key.Keywords.OrderByDescending(r => r.Relevance)
                            where r.Text != ""
                            where r.Relevance > 0.2
                            select r.Text).ToList();

            var concepts = (from r in con.Concepts.OrderByDescending(r => r.Relevance)
                            where r.Text != ""
                            where r.Relevance > 0.2
                            select r.Text).ToList();
            var jobtitle = (from r in ent.Entities.OrderByDescending(r => r.Relevance)
                            where r.Type == "JobTitle"
                            where r.Relevance > 0.2
                            select r.Text).FirstOrDefault();
            List<string> black = new List<string>();
            black.Add("br");
            black.Add("/b");
            var cleankeywords = keywords.Except(nonFieldT).ToList();
            var cleanconcepts = concepts.Except(nonFieldT).ToList();

            cleankeywords = keywords.Except(black).ToList();
            cleanconcepts = concepts.Except(black).ToList();
            alchemyCompare res = new alchemyCompare();
            var allProducts = cleanconcepts.Concat(cleankeywords)
                                    .Concat(fieldT)
                                    .ToList();
            res.Competencies = allProducts.Except(black).ToList();
            res.Concepts = cleanconcepts.Except(black).ToList();
            res.Keywords = cleankeywords.Except(black).ToList();
            res.Skills = fieldT.Except(black).ToList();
            res.JobTitle = jobtitle;
            res.Text = job.jobtext;


            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string xJson = oSerializer.Serialize(res);
            return Json(new { results = xJson });
        }

        public HttpResponseMessage PostResume([FromBody] resumeData resume)
        {

            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);

            var doc = new HtmlDocument();
            doc.LoadHtml(resume.resumeText);
            ComponentInfo.SetLicense("DBTX-8EIK-50LW-QTKQ");

            // Save output file to stream.
            //DocumentModel document = new DocumentModel();
            string memString = resume.resumeText;
            // convert string to stream
            byte[] buffer = Encoding.ASCII.GetBytes(memString);
            MemoryStream ms = new MemoryStream(buffer);


            //DocumentModel document = DocumentModel.Load(ms, LoadOptions.HtmlDefault);
            //responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
            DocumentModel document = new DocumentModel();
            document.Save(HttpContext.Current.Response, "MyResume.docx", SaveOptions.DocxDefault);
            //
            HttpContext.Current.Response.Flush();
            HttpContext.Current.Response.End();

            return responseMessage;
        }

        [HttpPost]
        public IHttpActionResult UploadFromDropBox([FromBody] resumeUrl url)
        {

            string finaltext = "";
            //Create a stream for the file
            Stream stream = null;

            //This controls how many bytes to read at a time and send to the client
            int bytesToRead = 10000;

            // Buffer to read bytes in chunk size specified above
            byte[] buffer = new Byte[bytesToRead];

            // The number of bytes read
            try
            {
                //Create a WebRequest to get the file
                HttpWebRequest fileReq = (HttpWebRequest)WebRequest.Create(url.url);

                //Create a response for this request
                HttpWebResponse fileResp = (HttpWebResponse)fileReq.GetResponse();

                if (fileReq.ContentLength > 0)
                    fileResp.ContentLength = fileReq.ContentLength;

                //Get the Stream returned from the response
                stream = fileResp.GetResponseStream();
                MemoryStream ms = new MemoryStream();
                stream.CopyTo(ms);
                // prepare the response to the client. resp is the client Response
                var resp = System.Web.HttpContext.Current.Response;

                //Indicate the type of data being sent
                //resp.ContentType = "application/octet-stream";
                //string fileName = "Resume";
                //Name the file 
                //resp.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
                //resp.AddHeader("Content-Length", fileResp.ContentLength.ToString());

                int length;
                do
                {
                    // Verify that the client is connected.
                    if (resp.IsClientConnected)
                    {
                        // Read data into the buffer.
                        length = stream.Read(buffer, 0, bytesToRead);

                        // and write it out to the response's output stream
                        resp.OutputStream.Write(buffer, 0, length);


                        if (resp != null)
                        {
                            ComponentInfo.SetLicense("DBTX-8EIK-50LW-QTKQ");

                            if (fileResp.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                            {
                                DocumentModel document = DocumentModel.Load(ms, LoadOptions.DocxDefault);
                                StringBuilder sb = new StringBuilder();

                                foreach (GemBox.Document.Paragraph paragraph in document.GetChildElements(true, ElementType.Paragraph))
                                {
                                    foreach (Run run in paragraph.GetChildElements(true, ElementType.Run))
                                    {
                                        bool isBold = run.CharacterFormat.Bold;
                                        string text = run.Text;

                                        sb.AppendFormat("{0}{1}{2}", isBold ? "<b>" : "", text, isBold ? "</b>" : "");
                                    }
                                    sb.AppendLine();
                                }

                                finaltext = sb.ToString();
                                finaltext = finaltext.Replace("\n", "<br/>");



                                return Json(new { resume = finaltext });
                            }

                            if (fileResp.ContentType == "application/msword")
                            {

                                DocumentModel document = DocumentModel.Load(ms, LoadOptions.DocDefault);
                                StringBuilder sb = new StringBuilder();

                                foreach (GemBox.Document.Paragraph paragraph in document.GetChildElements(true, ElementType.Paragraph))
                                {
                                    foreach (Run run in paragraph.GetChildElements(true, ElementType.Run))
                                    {
                                        bool isBold = run.CharacterFormat.Bold;
                                        string text = run.Text;

                                        sb.AppendFormat("{0}{1}{2}", isBold ? "<b>" : "", text, isBold ? "</b>" : "");
                                    }
                                    sb.AppendLine();
                                }

                                finaltext = sb.ToString();
                                finaltext = finaltext.Replace("\n", "<br/>");



                                return Json(new { resume = finaltext });
                            }

                            if (fileResp.ContentType == "application/pdf")
                            {
                                string currentText = "";
                                PdfReader reader = new PdfReader(ms.ToArray());
                                for (int page = 1; page <= reader.NumberOfPages; page++)
                                {
                                    ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                                    currentText += PdfTextExtractor.GetTextFromPage(reader, page, strategy);



                                }

                                currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
                                finaltext = currentText.Replace("\n", "<br/>");


                                return Json(new { resume = finaltext });

                            }

                            if (fileResp.ContentType == "text/plain")
                            {
                                DocumentModel document = DocumentModel.Load(ms, LoadOptions.TxtDefault);

                                StringBuilder sb = new StringBuilder();

                                foreach (GemBox.Document.Paragraph paragraph in document.GetChildElements(true, ElementType.Paragraph))
                                {
                                    foreach (Run run in paragraph.GetChildElements(true, ElementType.Run))
                                    {
                                        bool isBold = run.CharacterFormat.Bold;
                                        string text = run.Text;

                                        sb.AppendFormat("{0}{1}{2}", isBold ? "<b>" : "", text, isBold ? "</b>" : "");
                                    }
                                    sb.AppendLine();
                                }
                                finaltext = sb.ToString();
                                return Json(new { resume = finaltext });
                            }

                            // Flush the data
                            resp.Flush();

                            //Clear the buffer
                            buffer = new Byte[bytesToRead];

                        }
                    }
                    else
                    {
                        // cancel the download if client has disconnected
                        length = -1;
                    }
                } while (length > 0); //Repeat until no data is read
            }

            finally
            {
                if (stream != null)
                {

                    //Close the input stream
                    stream.Close();
                }


            }

            return Json(new { resume = "Something went wrong please try again later." });
        }


    }
}

