using System;

namespace Hasslefree.Core.Helpers
{
	public static class DateTimeHelper
	{

		public static DateTime RoundUp(this DateTime dt, TimeSpan d)
		{
			if (dt.Second > 45)
				dt = dt.AddSeconds(60-dt.Second);

			return new DateTime((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks);
		}

	}
}
