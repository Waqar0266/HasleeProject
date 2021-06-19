using System;
using System.Collections.Generic;

namespace Hasslefree.Services.Emails.ModelBuilders
{
	public interface IBuildEmailModel
	{
		IBuildEmailModel this[String title, String subject, String fromName] { get; }

		IBuildEmailModel WithModel(String name, Object model);

		Dictionary<String, Object> Get();
	}
}
