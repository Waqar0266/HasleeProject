using System;

namespace Hasslefree.Services.Emails.Crud
{
	public interface IPreviewEmailService
	{
		IPreviewEmailService this[String type] { get; }
		IPreviewEmailService this[Int32 id] { get; }

		String Get(String template);
	}
}
