using System;
using HtmlAgilityPack;
using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;
using PdfSharp.Pdf.IO;
using Property24ScrapeTester.Pages;

namespace Property24ScrapeTester
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //TestProperty24();
            TestPdfForms();
        }

        private static void TestProperty24()
        {
            var web = new HtmlWeb();
            var propertyInfoPage = new PropertyInfoPage(web, "109676148");
            var images = propertyInfoPage.GetImages();
            var province = propertyInfoPage.GetProvince();
            var city = propertyInfoPage.GetCity();
            var suburb = propertyInfoPage.GetSuburb();
            var price = propertyInfoPage.GetPrice();
            var name = propertyInfoPage.GetName();
            var description = propertyInfoPage.GetDescription();
        }

        private static void TestPdfForms()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            // Open the file
            PdfDocument document = PdfReader.Open(Environment.CurrentDirectory + "\\test.pdf", PdfDocumentOpenMode.Modify);
            PdfTextField field = (PdfTextField)(document.AcroForm.Fields["Text1"]);

            PdfString pdfString = new PdfString("Johannes Daniel Pretorius");

            //This section makes the text visible after passing a long text and will wrap it!
            if (document.AcroForm.Elements.ContainsKey("/NeedAppearances"))
                document.AcroForm.Elements["/NeedAppearances"] = new PdfBoolean(true);
            else
                document.AcroForm.Elements.Add("/NeedAppearances", new PdfBoolean(true));

            //set the value of this field
            field.Value = pdfString;

            document.Save(Environment.CurrentDirectory + "\\test2.pdf");
        }
    }
}
