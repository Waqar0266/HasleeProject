using System;

namespace Hasslefree.Core.Domain.Common
{
	public enum Gender
	{
		Male,
		Female
	}

	public enum Titles
	{
		Mr,
		Mrs,
		Miss,
		Dr,
		Prof,
		Other
	}

	public static class EnumExtensions
	{
		public static Titles ResolveTitle(this string t)
		{
			if (Enum.TryParse(t, out Titles title)) return title;
			else return Titles.Mr;
		}
	}
}
