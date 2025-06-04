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
using GroupDocs.Conversion.Options.Convert;
using System.Runtime.InteropServices;

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

                //var doc = new Aspose.Words.Document(docxPath);
                //var options = new HtmlSaveOptions()
                //{
                //    ExportImagesAsBase64 = true
                //};
                //MemoryStream stream = new MemoryStream();
                //doc.Save(stream, options);
                //stream.Position = 0;
                //using (StreamReader reader = new StreamReader(stream))
                //{
                //    this.htmlCode += reader.ReadToEnd();
                //}

                this.htmlPath = System.IO.Directory.GetCurrentDirectory() + "\\tmp_data\\GroupDocsToHtml.html";
                var converter = new GroupDocs.Conversion.Converter(docxPath);
                WebConvertOptions convOpt = new WebConvertOptions();
                converter.Convert(this.htmlPath, convOpt);
                using (StreamReader reader = new StreamReader(this.htmlPath))
                {
                    this.htmlCode = reader.ReadToEnd();
                }
                return true;
            }
            else
                return false;
        }

        //public bool saveFromHtmlToDocx(string fileInput, string fileOutput)
        //{
        //    if (System.IO.File.Exists(fileInput))
        //    {
        //        Aspose.Words.Document doc = new Aspose.Words.Document(fileInput);
        //        doc.Save(fileOutput);
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        public bool setHtml(System.Windows.Forms.WebBrowser myWeb)
        {
            if (myWeb != null && htmlCode != null)
            {
                myWeb.DocumentText = this.htmlCode;
                return true;
            }
            else
            {
                return false;
            }
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
