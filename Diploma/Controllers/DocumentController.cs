using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aspose.Words;
using Aspose.Words.Saving;
using Microsoft.Office.Interop.Word;

namespace Diploma.Controllers
{
    internal class DocumentController
    {
        public void openDocument(String filePath, System.Windows.Forms.WebBrowser myWeb)
        {
            //Aspose.Words.Document myDoc = new Aspose.Words.Document(filePath);
            myWeb.Navigate(filePath);

        }
        public void saveHtml(string fileInput, string fileOutput)
        {
            var wordApp = new Microsoft.Office.Interop.Word.Application();
            var doc = wordApp.Documents.Open(fileInput);
            doc.SaveAs2(fileOutput, WdSaveFormat.wdFormatFilteredHTML);
            doc.Close();
            wordApp.Quit();
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




    }
}
