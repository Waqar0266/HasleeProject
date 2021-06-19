using System;

namespace Hasslefree.Core.Logging
{
	internal class SqsLoggerMessage
	{
		public DateTime UtcTime { get; set; }
		public SqsLogType Type { get; set; }
		public String Message { get; set; }
		public String StackTrace { get; set; }
		public String Url { get; set; }
	}
}
