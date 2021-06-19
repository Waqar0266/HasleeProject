using System;
using Hasslefree.Core.Infrastructure;
using Hasslefree.Web.Models.Emails.Customers;

namespace Hasslefree.Services.Emails.ModelBuilders.Customers
{
	public class BuildOtpEmailModel : IBuildOtpEmailModel, IInstancePerRequest
	{
		#region Private Fields

		private Int32 _pin;

		#endregion

		#region IBuildOtpEmailModel

		public IBuildOtpEmailModel this[Int32 pin]
		{
			get
			{
				_pin = pin;

				return this;
			}
		}

		public OtpEmailModel Get()
		{
			if (_pin == 0)
				return null;

			var model = new OtpEmailModel()
			{
				Pin = _pin
			};

			_pin = 0;

			return model;
		}

		#endregion
	}
}
