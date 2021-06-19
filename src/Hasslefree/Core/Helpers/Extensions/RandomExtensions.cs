using System;

namespace Hasslefree.Core.Helpers.Extensions
{
	/// <summary>
	/// Copied from StackOverflow : https://stackoverflow.com/questions/6651554/random-number-in-long-range-is-this-the-way
	/// </summary>
	public static class RandomExtensions
	{
		/// <summary>
		/// Returns a random long from min (inclusive) to max (exclusive)
		/// </summary>
		/// <param name="random">The given random instance</param>
		/// <param name="min">The inclusive minimum bound</param>
		/// <param name="max">The exclusive maximum bound.  Must be greater than min</param>
		public static Int64 NextLong(this Random random, Int64 min, Int64 max)
		{
			if (max <= min)
				throw new ArgumentOutOfRangeException("max", "max must be > min!");

			//Working with ulong so that modulo works correctly with values > long.MaxValue
			var uRange = (UInt64)(max - min);

			//Prevent a modolo bias; see https://stackoverflow.com/a/10984975/238419
			//for more information.
			//In the worst case, the expected number of calls is 2 (though usually it's
			//much closer to 1) so this loop doesn't really hurt performance at all.
			UInt64 ulongRand;
			do
			{
				var buf = new Byte[8];
				random.NextBytes(buf);
				ulongRand = (UInt64)BitConverter.ToInt64(buf, 0);
			} while (ulongRand > UInt64.MaxValue - ((UInt64.MaxValue % uRange) + 1) % uRange);

			return (Int64)(ulongRand % uRange) + min;
		}

		/// <summary>
		/// Returns a random long from 0 (inclusive) to max (exclusive)
		/// </summary>
		/// <param name="random">The given random instance</param>
		/// <param name="max">The exclusive maximum bound.  Must be greater than 0</param>
		public static Int64 NextLong(this Random random, Int64 max)
		{
			return random.NextLong(0, max);
		}

		/// <summary>
		/// Returns a random long over all possible values of long (except long.MaxValue, similar to
		/// random.Next())
		/// </summary>
		/// <param name="random">The given random instance</param>
		public static Int64 NextLong(this Random random)
		{
			return random.NextLong(Int64.MinValue, Int64.MaxValue);
		}
	}
}
