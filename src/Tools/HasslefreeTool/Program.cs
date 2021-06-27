using Hasslefree.Core.Domain.Security;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Data;
using Hasslefree.Services.People.Interfaces;
using Hasslefree.Services.Security.Groups;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;
using PdfSharp.Pdf.IO;
using System;
using System.Linq;

namespace HasslefreeTool
{
	class Program
	{
		static void Main(string[] args)
		{
			Init();

			TestPdfForms();

			//InstallSecurityGroups();
		}

		private static void Init()
		{
			//Start the Hasslefree application engine
			EngineContext.Initialize(false);
		}

		private static void InstallSecurityGroups()
		{
			var createSecurityGroupService = EngineContext.Current.Resolve<ICreateSecurityGroupService>();
			var createPersonService = EngineContext.Current.Resolve<ICreatePersonService>();
			var loginRepo = EngineContext.Current.Resolve<IDataRepository<Login>>();

			if (!loginRepo.Table.Any(l => l.Email == "admin@hasslefree.sa.com"))
			{

				createPersonService.New("Admin", "Admin", "Admin", "admin@hasslefree.sa.com").WithPassword("password", "").Create();
				createSecurityGroupService.New("Admin", "Admin").WithUser(createPersonService.LoginId).Create();
			}

			if (!loginRepo.Table.Any(l => l.Email == "director@hasslefree.sa.com"))
			{
				createPersonService.New("Director", "Director", "Director", "director@hasslefree.sa.com").WithPassword("password", "").Create();
				createSecurityGroupService.New("Director", "Director").WithUser(createPersonService.LoginId).Create();
			}

			//create the agent role
			createSecurityGroupService.New("Agent", "Agent").Create();
		}

		private static void TestPdfForms()
		{
			//System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

			// Open the file
			PdfDocument document = PdfReader.Open(Environment.CurrentDirectory + "\\test.pdf", PdfDocumentOpenMode.Modify);
			PdfTextField field = (PdfTextField)(document.AcroForm.Fields["First Names"]);

			PdfString pdfString = new PdfString("Johannes Daniel Pretorius");

			//This section makes the text visible after passing a long text and will wrap it!
			if (document.AcroForm.Elements.ContainsKey("/NeedAppearances"))
				document.AcroForm.Elements["/NeedAppearances"] = new PdfBoolean(true);
			else
				document.AcroForm.Elements.Add("/NeedAppearances", new PdfBoolean(true));

			//set the value of this field
			field.Value = pdfString;

			// Get an XGraphics object for drawing
			XGraphics gfx = XGraphics.FromPdfPage(document.Pages[0]);
			DrawImage(gfx, Environment.CurrentDirectory + "\\signature.png", (document.Pages[0].Width.Value - 190), (document.Pages[0].Height.Value - 160), 45, 45);

			document.Save(Environment.CurrentDirectory + "\\test2.pdf");
		}

		private static void DrawImage(XGraphics gfx, string jpegSamplePath, double x, double y, int width, int height)
		{
			XImage image = XImage.FromFile(jpegSamplePath);
			gfx.DrawImage(image, x, y, width, height);
		}
	}
}
