using System;
using System.Globalization;
using Hasslefree.Core.Infrastructure;

namespace Hasslefree.Core.Helpers
{
	public static class DecimalHelper
	{

		public static string ToPointString(this decimal value)
		{
			var nfi = new NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," };

			return value.ToString("n", nfi);
		}
	}
}
