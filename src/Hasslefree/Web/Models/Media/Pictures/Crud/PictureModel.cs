using Newtonsoft.Json;
using Hasslefree.Core.Domain.Media;
using Hasslefree.Web.Models.Common;
using System;

namespace Hasslefree.Web.Models.Media.Pictures.Crud
{
	public class PictureModel
	{
		public PictureModel()
		{
			Format = PictureFormat.Jpeg;
			Action = CrudAction.None;
		}

		public Int32 PictureId { get; set; }
		public String Name { get; set; }

		[JsonIgnore]
		public PictureFormat Format { get; set; }

		public String FormatEnum
		{
			get => Format.ToString();
			set => Format = (PictureFormat)Enum.Parse(typeof(PictureFormat), value);
		}
		public String MimeType { get; set; }
		public String AlternateText { get; set; }
		public String Key { get; set; }

		[JsonIgnore]
		public CrudAction Action { get; set; }

		public String ActionEnum
		{
			get => Action.ToString();
			set => Action = (CrudAction)Enum.Parse(typeof(CrudAction), value);
		}

		public Byte[] File { get; set; }

		public String Transforms { get; set; }
	}
}
