using Hasslefree.Core.Infrastructure;
using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			return this;
		}

		public IFillFormService WithField(string fieldName, string fieldValue)
		{
			PdfTextField field = (PdfTextField)(_document.AcroForm.Fields[fieldName]);
			PdfString pdfString = new PdfString(fieldValue);

			//set the value of this field
			field.Value = pdfString;

			return this;
		}

		public IFillFormService WithImage(byte[] image, int pageNumber, int x, int y, int height, int width)
		{
			return this;
		}

		public Byte[] Process()
		{
			throw new NotImplementedException();
		}

		private byte[] ExtractResource(string filename)
		{
			System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
			using (Stream resFilestream = a.GetManifestResourceStream($"Hasslefree.Services.Forms.EmbeddedForms{filename}"))
			{
				if (resFilestream == null) return null;
				byte[] ba = new byte[resFilestream.Length];
				resFilestream.Read(ba, 0, ba.Length);
				return ba;
			}
		}
	}
}
