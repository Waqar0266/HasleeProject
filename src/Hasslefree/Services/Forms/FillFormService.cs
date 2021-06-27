using Hasslefree.Core.Infrastructure;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;
using PdfSharp.Pdf.IO;
using System.IO;

namespace Hasslefree.Services.Forms
{
	public class FillFormService : IFillFormService, IInstancePerRequest
	{
		private PdfDocument _document;

		public IFillFormService Prepare(string formName)
		{
			// Open the file
			var fileData = ExtractResource(formName);
			using (var ms = new MemoryStream(fileData))
				_document = PdfReader.Open(ms, PdfDocumentOpenMode.Modify);

			return this;
		}

		public IFillFormService WithCheckbox(string checkboxName, bool check)
		{
			PdfCheckBoxField cb = (PdfCheckBoxField)(_document.AcroForm.Fields[checkboxName]);

			//set the checkbox value
			cb.Checked = check;

			return this;
		}

		public IFillFormService WithField(string fieldName, string fieldValue)
		{
			if (string.IsNullOrEmpty(fieldValue)) return this;

			PdfTextField field = (PdfTextField)(_document.AcroForm.Fields[fieldName]);
			PdfString pdfString = new PdfString(fieldValue);

			//set the value of this field
			field.Value = pdfString;

			return this;
		}

		public IFillFormService WithImage(byte[] image, int pageNumber, int x, int y, int height, int width, bool xFromPage = false, bool yFromPage = false)
		{
			XGraphics gfx = XGraphics.FromPdfPage(_document.Pages[pageNumber]);
			using (var ms = new MemoryStream(image))
			{
				var img = XImage.FromStream(ms);
				gfx.DrawImage(img, x, y, (xFromPage ? _document.Pages[pageNumber].Width - width : width), (yFromPage ? _document.Pages[pageNumber].Height - height : height));
			}
			return this;
		}

		public byte[] Process()
		{
			//This section makes the text visible after passing a long text and will wrap it!
			if (_document.AcroForm.Elements.ContainsKey("/NeedAppearances"))
				_document.AcroForm.Elements["/NeedAppearances"] = new PdfBoolean(true);
			else
				_document.AcroForm.Elements.Add("/NeedAppearances", new PdfBoolean(true));

			//save to stream
			byte[] data;
			using (var ms = new MemoryStream())
			{
				_document.Save(ms);
				data = ms.ToArray();
			}

			return data;
		}

		private byte[] ExtractResource(string filename)
		{
			System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
			using (Stream resFilestream = a.GetManifestResourceStream($"Hasslefree.Services.Forms.EmbeddedForms.{filename}"))
			{
				if (resFilestream == null) return null;
				byte[] ba = new byte[resFilestream.Length];
				resFilestream.Read(ba, 0, ba.Length);
				return ba;
			}
		}
	}
}
