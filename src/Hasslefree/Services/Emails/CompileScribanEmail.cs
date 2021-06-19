using Hasslefree.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using Scriban;
using Scriban.Runtime;
using Hasslefree.Core.Helpers.Extensions;

namespace Hasslefree.Services.Emails
{
	public class CompileScribanEmail : ICompileScribanEmail, IInstancePerRequest
	{
		#region ICompileScribanEmail

		public String this[String template, Object model]
		{
			get
			{
				var temp = Template.Parse(template);

				var mode = model.ToSnakeDictionary();

				var so = new ScriptObject();
				so.Import(mode, renamer: member => member.Name);

				var context = new TemplateContext()
				{
					MemberRenamer = r => r.Name, 
					MemberFilter = null
				};
				context.PushGlobal(so);

				return temp.Render(context);
			}
		}

		#endregion
	}
}
