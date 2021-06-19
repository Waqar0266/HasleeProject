using System;

namespace Hasslefree.Services.Emails.Crud
{
	public interface IResetEmailService
	{
		Boolean this[String type] { get; }
		Boolean this[Int32 id] { get; }
	}
}
