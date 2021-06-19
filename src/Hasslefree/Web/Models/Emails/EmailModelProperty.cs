using System;
using System.Collections.Generic;

namespace Hasslefree.Web.Models.Emails
{
	/// <summary>
	/// A model containing a single property for the email model
	/// </summary>
	public class EmailModelProperty
	{
		public EmailModelProperty()
		{

		}

		public EmailModelProperty(String name, String type, Object value = null)
		{
			Name = name;
			Type = type;
			Value = value;
		}

		/// <summary>
		/// The property name
		/// </summary>
		public String Name { get; set; }

		/// <summary>
		/// The property type
		/// String, Int32, DateTime, Object, List
		/// </summary>
		public String Type { get; set; }

		/// <summary>
		/// The value of the property
		/// </summary>
		public Object Value { get; set; }

		/// <summary>
		/// When an object, the object's properties
		/// </summary>
		public List<EmailModelProperty> Children { get; set; }
	}
}
