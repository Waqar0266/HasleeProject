namespace Hasslefree.Core.Configuration
{
	public class EmailSettings : ISettings
	{
		public EmailSettings()
		{
			this.Host = "smtp.mailtrap.io";
			this.Port = 587;
			this.Username = "f381a3ce5d4a40";
			this.Password = "dca9031a13d84f";
		}

		public string Host { get; set; }
		public int Port { get; set; }
		public bool Ssl { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
	}
}
