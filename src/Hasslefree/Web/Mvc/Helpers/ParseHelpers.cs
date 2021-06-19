using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Hasslefree.Web.Mvc.Helpers
{
	public static class ParseHelpers
	{
		private const string ELAPSED_TIME_KEY = "{0} {1} ago";
		private static JavaScriptSerializer json;

		private static JavaScriptSerializer JSON { get { return json ?? ( json = new JavaScriptSerializer() ); } }

		public static Stream ToStream( this string @this )
		{
			var stream = new MemoryStream();
			var writer = new StreamWriter( stream );
			writer.Write( @this );
			writer.Flush();
			stream.Position = 0;
			return stream;
		}

		[Obsolete]
		public static object ToAnonymousObject(this IDictionary<string, object> @this)
		{
			var expandoObject = new ExpandoObject();
			var expandoDictionary = (IDictionary<string, object>)expandoObject;

			foreach (var keyValuePair in @this)
			{
				expandoDictionary.Add(keyValuePair);
			}
			return expandoObject;
		}


		[Obsolete]
		public static T ParseXML<T>( this string @this ) where T : class
		{
			var reader = XmlReader.Create( @this.Trim().ToStream(), new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Document } );
			return new XmlSerializer( typeof( T ) ).Deserialize( reader ) as T;
		}

		[Obsolete]
		public static T ParseJSON<T>( this string @this ) where T : class
		{
			return JSON.Deserialize<T>( @this.Trim() );
		}
		
		public static string FormatPrice( this decimal price, string currency, string format = null )
		{
			if ( !String.IsNullOrEmpty( format ) ) return price.ToString( format );

			string t = price.ToString( "###,###,###,###,##0.00" );
			if ( !String.IsNullOrEmpty( currency ) ) return String.Concat( currency, t );

			return t;
		}

		[Obsolete]
		public static IEnumerable<T> DataTableToList<T>( this DataTable table ) where T : class, new()
		{
			IDictionary<Type, IEnumerable<PropertyInfo>> _Properties =
			new Dictionary<Type, IEnumerable<PropertyInfo>>();

			try
			{
				var objType = typeof( T );
				IEnumerable<PropertyInfo> properties;

				lock ( _Properties )
				{
					if ( !_Properties.TryGetValue( objType, out properties ) )
					{
						properties = objType.GetProperties().Where( property => property.CanWrite );
						_Properties.Add( objType, properties );
					}
				}

				var list = new List<T>( table.Rows.Count );

				foreach ( var row in table.AsEnumerable() )
				{
					var obj = new T();

					foreach ( var prop in properties )
					{
						try
						{
							prop.SetValue( obj, Convert.ChangeType( row[ prop.Name ], prop.PropertyType ), null );
						}
						catch
						{
						}
					}

					list.Add( obj );
				}

				return list;
			}
			catch
			{
				return Enumerable.Empty<T>();
			}
		}

		[Obsolete]
		/// <summary>
		/// Format a price
		/// </summary>
		/// <param name="price"></param>
		/// <param name="currency"></param>
		/// <param name="qty"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string FormatPrice(decimal price, string currency = null, string format = null, decimal qty = 1)
		{
			decimal sum = price * qty;
			if (!String.IsNullOrEmpty(format)) return sum.ToString(format);

			string t = sum.ToString("###,###,###,###,##0.00");
			if (!String.IsNullOrEmpty(currency)) return String.Concat(currency, t);
			return t;
		}

		[Obsolete]
		public static string ElapsedTime(this DateTime date)
		{
			var now = DateTime.UtcNow;
			string elapsedTimeString = String.Format(ELAPSED_TIME_KEY, date.ToShortDateString(), date.ToShortTimeString());
		
			double d = (now - date).TotalDays;
			double s = (now - date).TotalSeconds;

			int years = (int) d / 365;
			int months = (int) d / 31;
			int days = (int) s / 86400;
			int hours = (int) s / 3600;
			int minutes = (int) s / 60;
			int seconds = (int) s;

			if (years >= 1) elapsedTimeString = String.Format(ELAPSED_TIME_KEY, years, years == 1 ? "year" : "years");
			else if(months >= 1) elapsedTimeString = String.Format(ELAPSED_TIME_KEY, months, months == 1 ? "month" : "months");
			else if(days >= 1) elapsedTimeString = String.Format(ELAPSED_TIME_KEY, days, days == 1 ? "day" : "days");
			else if(hours >= 1) elapsedTimeString = String.Format(ELAPSED_TIME_KEY, hours, hours == 1 ? "hour" : "hours");
			else if (minutes >= 1) elapsedTimeString = String.Format(ELAPSED_TIME_KEY, minutes, minutes == 1 ? "minute" : "minutes");
			else if(seconds >= 1) elapsedTimeString = String.Format(ELAPSED_TIME_KEY, seconds, seconds == 1 ? "second" : "seconds");
			return elapsedTimeString;
		}

	}

}
