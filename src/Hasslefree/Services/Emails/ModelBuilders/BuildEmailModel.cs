using System;
using System.Collections.Generic;
using Hasslefree.Core;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Web.Models.Emails;

namespace Hasslefree.Services.Emails.ModelBuilders
{
	public class BuildEmailModel : IBuildEmailModel, IInstancePerRequest
	{
		#region Fields

		private Dictionary<String, Object> _dict;

		private readonly String _storeEmail;

		#endregion

		#region IBuildEmailModel

		public IBuildEmailModel this[String title, String subject, String fromName]
		{
			get
			{
				_dict = new Dictionary<String, Object>
				{
					{
						"Email",
						new SharedEmailModel()
						{
							Title = title, 
							Subject = subject, 
							FromName = fromName, 
							ReplyTo = _storeEmail
						}
					},
					{"Store", "StoreName"}
				};



				return this;
			}
		}

		public IBuildEmailModel WithModel(String name, Object model)
		{
			_dict.Add(name, model);

			return this;
		}

		public Dictionary<String, Object> Get()
		{
			return _dict;
		}

		#endregion
	}
}
