namespace Hasslefree.Web.Models.Security.Sessions.Get
{
	/// <summary>
	/// Location where the session is accessed from
	/// </summary>
	public class SessionLocation
	{
		/// <summary>
		/// IP address
		/// </summary>
		public string IpAddress { get; set; }

		/// <summary>
		/// Latitude
		/// </summary>
		public double Latitude { get; set; }

		/// <summary>
		/// Longitude
		/// </summary>
		public double Longitude { get; set; }
	}
}
