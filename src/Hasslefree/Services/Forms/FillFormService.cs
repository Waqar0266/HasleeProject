using Hasslefree.Core.Infrastructure;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;
using PdfSharp.Pdf.IO;
using System.Collections.Generic;
using System.IO;

namespace Hasslefree.Services.Forms
{
	public class FillFormService : IFillFormService, IInstancePerRequest
	{
		private PdfDocument _document;
		private List<string> _fields = new List<string>();

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

			//disable further editing on the form
			cb.ReadOnly = true;

			//add to the list
			_fields.Add(checkboxName);

			return this;
		}

		public IFillFormService WithField(string fieldName, string fieldValue)
		{
			if (string.IsNullOrEmpty(fieldValue)) return this;
			if (_document.AcroForm.Fields[fieldName] == null) return this;

			PdfTextField field = (PdfTextField)(_document.AcroForm.Fields[fieldName]);
			PdfString pdfString = new PdfString(fieldValue);

			//set the value of this field
			field.Value = pdfString;

			//disabled further editing on the form
			field.ReadOnly = true;

			//add to the list
			_fields.Add(fieldName);

			return this;
		}

		public IFillFormService WithImage(byte[] image, int pageNumber, int x, int y, int height, int width, bool xFromPageWidth = false, bool yFromPageHeight = false)
		{
			using (XGraphics gfx = XGraphics.FromPdfPage(_document.Pages[pageNumber]))
			using (var ms = new MemoryStream(image))
			{
				var img = XImage.FromStream(ms);
				gfx.DrawImage(img, (xFromPageWidth ? _document.Pages[pageNumber].Width - x : x), (yFromPageHeight ? _document.Pages[pageNumber].Height - y : y), width, height);
			}

			return this;
		}

		public byte[] Process()
		{
			//remove the other form fields that has not been filled in yet
			foreach (var field in _document.AcroForm.Fields.Names)
				if (!_fields.Contains(field)) _document.AcroForm.Fields[field].ReadOnly = true;

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
