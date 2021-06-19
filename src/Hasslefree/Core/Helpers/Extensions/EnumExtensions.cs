using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hasslefree.Core.Helpers.Extensions
{
	public static class EnumExtensions
	{
		public static T ToEnum<T>(this String value) => (T)Enum.Parse(typeof(T), value);
	}
}
