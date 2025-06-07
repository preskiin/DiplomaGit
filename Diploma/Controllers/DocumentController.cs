using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aspose.Words;
using Aspose.Words.Saving;
using Microsoft.Office.Interop.Word;
using System.Runtime.InteropServices;
using HtmlAgilityPack;
using System.Diagnostics.Eventing.Reader;

namespace Diploma.Controllers
{
    internal class DocumentController
    {
        
        

        private String htmlCode;
        public String htmlPath;

        public DocumentController()
        {
            //string licensePath = "GroupDocs.Conversion.lic";
            //GroupDocs.Conversion.License lic = new GroupDocs.Conversion.License();
            //lic.SetLicense(licensePath);
        }

        public void openDocument(System.Windows.Forms.WebBrowser myWeb)
        {
            //Aspose.Words.Document myDoc = new Aspose.Words.Document(filePath);
            myWeb.Navigate(htmlPath);

        }

        public bool saveFromDocxToHtml(string docxPath, string fileOutput)
        {
            if (System.IO.File.Exists(docxPath))
            {
                var wordApp = new Microsoft.Office.Interop.Word.Application();
                var doc = wordApp.Documents.Open(docxPath);
                doc.SaveAs2(fileOutput, WdSaveFormat.wdFormatFilteredHTML);
                htmlPath = fileOutput; //если не присвоить, метод openDocument не отработает
                doc.Close();
                wordApp.Quit();
                return true;
            }
            else
            {
                return false;
            }
            
            //// Use Aspose.Words license to remove trial version limitations after converting Word DOCX to HTML
            //License licenseForConvertingDOCXtoHTML = new License();
            //licenseForConvertingDOCXtoHTML.SetLicense("Aspose.Words.lic");

            //// Load input Word DOCX file with Document class
            //Document doc = new Document("Input.docx");

            //// Set different properties of HtmlSaveOptions class
            //HtmlSaveOptions saveOptions = new HtmlSaveOptions();
            //saveOptions.CssStyleSheetType = CssStyleSheetType.Inline;
            //saveOptions.ExportPageMargins = true;
            //saveOptions.ImageResolution = 90;

            //// Save output HTML
            //doc.Save("HtmlSaveOptions.html", saveOptions);
        }

        public bool docxToHtml(string docxPath)
        {
            if (System.IO.File.Exists(docxPath))
            {

                //var doc = new Aspose.Words.Document(docxPath);
                //MemoryStream stream = new MemoryStream();
                //doc.Save(stream, SaveFormat.Html);
                //stream.Position = 0;
                //using (StreamReader reader = new StreamReader(stream))
                //{
                //    this.htmlCode = reader.ReadToEnd();
                //}
                //stream.Close();

                var doc = new Aspose.Words.Document(docxPath);
                var options = new HtmlSaveOptions()
                {
                    ExportImagesAsBase64 = true
                };
                MemoryStream stream = new MemoryStream();
                doc.Save(stream, options);
                stream.Position = 0;
                using (StreamReader reader = new StreamReader(stream))
                {
                    this.htmlCode += reader.ReadToEnd();
                }
                if (putStringAfter("<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">", "<head>") != -1)
                    return true;
                else return false;

                //this.htmlCode = File.ReadAllText(docxPath, Encoding.UTF8);
                //return true;
            }
            else
                return false;
        }

        public String getHtml()
        {
            if (htmlCode != null)
            {
                return this.htmlCode;
            }
            else
            {
                return null;
            }
        }

        private int putStringAfter(String putString, String afterString)
        {
            if (this.htmlCode.IndexOf(afterString) != -1)
            {
                Int32 stringPosition = this.htmlCode.IndexOf(afterString) + afterString.Length;
                String tmpString = this.htmlCode.Insert(stringPosition, putString);
                this.htmlCode = tmpString;
                return 1;
            }
            else return -1;
            
        }
        //private class HandleImageSaving : IImageSavingCallback
        //{
        //    void IImageSavingCallback.ImageSaving(Aspose.Words.Saving.ImageSavingArgs e)
        //    {
        //        Stream imageStream = new MemoryStream();
        //        e.ImageStream = imageStream;
        //    }
        //}

    }

}
