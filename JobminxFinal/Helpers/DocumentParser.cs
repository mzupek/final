using GemBox.Document;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace JobminxFinal.Helpers
{
    class DocumentParser
    {
        public string parse(Stream stream, string contentType)
        {

            string finaltext = "";

            if (stream != null)
            {
                ComponentInfo.SetLicense("DBTX-8EIK-50LW-QTKQ");

                if (contentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                {
                    DocumentModel document = DocumentModel.Load(stream, LoadOptions.DocxDefault);
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
                }

                if (contentType == "application/msword")
                {

                    try
                    {
                        DocumentModel document = DocumentModel.Load(stream, LoadOptions.DocDefault);
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
                            finaltext = sb.ToString();
                            finaltext = finaltext.Replace("\n", "<br/>");
                        }
                    }
                    catch (Exception e)
                    {

                    }


                }

                if (contentType == "application/pdf")
                {
                    string currentText = "";
                    PdfReader reader = new PdfReader(stream);

                    for (int page = 1; page <= reader.NumberOfPages; page++)
                    {
                        ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                        currentText += PdfTextExtractor.GetTextFromPage(reader, page, strategy);




                    }
                    currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
                    finaltext = currentText.Replace("\n", "<br/>");

                }

                if (contentType == "text/plain")
                {
                    DocumentModel document = DocumentModel.Load(stream, LoadOptions.TxtDefault);

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
                }
            }

            return finaltext;
        }
    }
}