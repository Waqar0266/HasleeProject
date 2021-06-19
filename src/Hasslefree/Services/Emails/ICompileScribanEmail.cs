using System;

namespace Hasslefree.Services.Emails
{
	public interface ICompileScribanEmail
	{
		String this[String template, Object model] { get; }
	}
}
