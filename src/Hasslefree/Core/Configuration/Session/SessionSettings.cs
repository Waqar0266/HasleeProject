namespace Hasslefree.Core.Configuration.Session
{
	public class SessionSettings : ISettings
	{
		public bool EnableGeoOrigin { get; set; }
		public bool SessionExpires { get; set; }
		public int SessionExpireMonths { get; set; }
	}
}
