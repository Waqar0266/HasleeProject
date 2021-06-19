using Hasslefree.Core;
using Hasslefree.Web.Framework.Helpers;
using Hasslefree.Web.Mvc.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Hasslefree.Web.Framework
{
	/// <summary>
	/// Represents a common helper
	/// </summary>
	public class WebHelper : IWebHelper
	{
		#region Fields 

		private readonly HttpContextBase _httpContext;

		#endregion

		/// <summary>
		/// Check if the Request object is available
		/// </summary>
		private bool IsRequestAvailable => _httpContext?.Request != null;

		#region Methods

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="httpContext">HTTP context</param>
		public WebHelper(HttpContextBase httpContext)
		{
			_httpContext = httpContext;
		}

		/// <summary>
		/// Get URL referrer
		/// </summary>
		/// <returns>URL referrer</returns>
		public virtual string GetUrlReferrer()
		{
			string referrerUrl = String.Empty;

			//URL referrer is null in some case (for example, in IE 8)
			if (IsRequestAvailable && _httpContext.Request.UrlReferrer != null)
				referrerUrl = _httpContext.Request.UrlReferrer.PathAndQuery;

			return referrerUrl;
		}

		/// <summary>
		/// Gets server variable by name
		/// </summary>
		/// <param name="name">Name</param>
		/// <returns>Server variable</returns>
		public virtual string ServerVariables(string name)
		{
			var result = String.Empty;

			try
			{
				if (!IsRequestAvailable) return result;

				// put this method in try-catch  as described here http://www.nopcommerce.com/boards/t/21356/multi-store-roadmap-lets-discuss-update-done.aspx?p=6#90196
				if (_httpContext.Request.ServerVariables[name] != null) result = _httpContext.Request.ServerVariables[name];
			}
			catch
			{
				result = String.Empty;
			}
			return result;
		}

		/// <summary>
		/// Returns true if the requested resource is one of the typical resources that needn't be processed by the cms engine.
		/// </summary>
		/// <param name="request">HTTP Request</param>
		/// <returns>True if the request targets a static resource file.</returns>
		/// <remarks>
		/// These are the file extensions considered to be static resources:
		/// .css
		///	.gif
		/// .png 
		/// .jpg
		/// .jpeg
		/// .js
		/// .axd
		/// .ashx
		/// </remarks>
		public virtual bool IsStaticResource(HttpRequest request)
		{
			if (request == null)
				throw new ArgumentNullException("request");

			string path = request.Path;
			string extension = VirtualPathUtility.GetExtension(path);

			if (extension == null) return false;

			switch (extension.ToLower())
			{
				case ".axd":
				case ".ashx":
				case ".bmp":
				case ".css":
				case ".gif":
				case ".htm":
				case ".html":
				case ".ico":
				case ".jpeg":
				case ".jpg":
				case ".js":
				case ".png":
				case ".rar":
				case ".zip":
					return true;
			}

			return false;
		}

		/// <summary>
		/// Maps a virtual path to a physical disk path.
		/// </summary>
		/// <param name="path">The path to map. E.g. "~/bin"</param>
		/// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
		public virtual string MapPath(string path)
		{
			if (HostingEnvironment.IsHosted)
			{
				//hosted
				return HostingEnvironment.MapPath(path);
			}

			//not hosted. For example, run in unit tests
			string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
			return Path.Combine(baseDirectory, path);
		}

		/// <summary>
		/// Returns true if the website is being run locally
		/// </summary>
		/// <returns></returns>
		public virtual bool IsLocal()
		{
			try
			{
				return _httpContext?.Request?.IsLocal ?? true;
			}
			catch (Exception)
			{
				return true;
			}
		}

		/// <summary>
		/// Returns true if the website is being run locally
		/// </summary>
		/// <returns></returns>
		private bool IsSecure()
		{
			return GetRequestProtocol().ToLower().Contains("s");
		}

		/// <summary>
		/// Get the IP address from the request
		/// </summary>
		/// <returns></returns>
		public string GetRequestIp()
		{
			if (!IsRequestAvailable) return "";

			// The request object
			var request = _httpContext.Request;

			// Check the X-Forwarded-For header used by load balancers
			var remoteAddress = request.Headers["X-Forwarded-For"];
			if (!String.IsNullOrEmpty(remoteAddress)) return remoteAddress.Split(',')[0];

			// Get the remote address from the request
			remoteAddress = ServerVariables("REMOTE_ADDR");

			// Return the address for remote requests
			if (remoteAddress != "localhost" && remoteAddress != "::1" && remoteAddress != "127.0.0.1") return remoteAddress;

			// If the request comes from localhost, attempt to get the IP address of the internet gateway
			try
			{
				remoteAddress = new WebClient().DownloadString("https://api.ipify.org");
			}
			catch (Exception)
			{

				remoteAddress = "127.0.0.1";
			}

			return remoteAddress;
		}

		/// <summary>
		/// Get the protocol of the request
		/// </summary>
		/// <returns></returns>
		public string GetRequestProtocol()
		{
			if (!IsRequestAvailable) return "";

			//If a request was done by a load balancer the request ip
			//should be available in the X-Forwarded-Proto header
			var request = _httpContext.Request;
			var protocol = request.Headers["X-Forwarded-Proto"];
			if (!String.IsNullOrEmpty(protocol)) return protocol;

			// Do the standard check to see if the connection was secure or not. A secure connection will return "ON"
			protocol = ServerVariables("HTTPS")?.ToLower() ?? "off";
			return protocol.Equals("on", StringComparison.InvariantCultureIgnoreCase) ? "https" : "http";
		}

		/// <summary>
		/// Get the port from the request
		/// </summary>
		/// <returns></returns>
		public string GetRequestPort()
		{
			if (!IsRequestAvailable) return "";

			//If a request was done by a load balancer the request IP
			//should be available in the X-Forwarded-Proto header
			var request = _httpContext.Request;
			string port = request.Headers["X-Forwarded-Port"];
			if (!String.IsNullOrEmpty(port)) return port;

			// Else return the result from the standard port check
			return ServerVariables("SERVER_PORT");
		}

		/// <summary>
		/// Get the host name used in the request
		/// </summary>
		/// <returns></returns>
		public string GetRequestHost()
		{
			return ServerVariables("HTTP_HOST");
		}

		/// <summary>
		/// Create a new cookie with the provided details
		/// </summary>
		/// <param name="name">Name of the Cookie</param>
		/// <param name="value">String value of the cookie</param>
		/// <param name="expiresOn"></param>
		/// <param name="mode"></param>
		/// <param name="secure"></param>
		public void SetCookieValue(string name, string value, DateTime? expiresOn = null, SameSiteMode mode = SameSiteMode.Strict, bool secure = true)
		{
			// Can't set cookies without a Request object
			if (!IsRequestAvailable) return;

			// Try to find the cookie
			var cName = GetCookieName(name);
			var cookie = _httpContext.Request.Cookies[cName];

			// Set the cookie if it exists
			if (cookie != null)
			{
				_httpContext.Response.Cookies.Set(CreateCookie(cName, value, expiresOn, mode, secure));
				_httpContext.Request.Cookies.Set(CreateCookie(cName, value, expiresOn, mode, secure));
			}

			// Create a new cookie
			else
			{
				cookie = CreateCookie(cName, value, expiresOn, mode, secure);
				_httpContext.Response.Cookies.Add(cookie);
				_httpContext.Request.Cookies.Set(cookie);
			}
		}

		/// <summary>
		/// Set the expire date for a cookie
		/// </summary>
		/// <param name="name"></param>
		/// <param name="expiresOn"></param>
		public void SetCookieExpireDate(string name, DateTime expiresOn)
		{
			CookieExpiryDate(GetCookieName(name), expiresOn);
		}

		/// <summary>
		/// Get the entire cookie
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public HttpCookie GetCookie(string name)
		{
			return IsRequestAvailable ? _httpContext.Request.Cookies[GetCookieName(name)] : null;
		}

		/// <summary>
		/// Get the value of a cookie
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string GetCookieValue(string name)
		{
			return _httpContext?.Request?.Cookies[GetCookieName(name)]?.Value ?? "";
		}

		/// <summary>
		/// Expire an existing cookie
		/// </summary>
		/// <param name="name"></param>
		public void DeleteCookie(string name)
		{
			RemoveCookie(GetCookieName(name));
		}

		/// <summary>
		/// Check if the request was made using XMLHttpRequest
		/// </summary>
		/// <returns></returns>
		public bool IsAjaxRequest()
		{
			return _httpContext?.Request?.IsAjaxRequest() ?? false;
		}

		/// <summary>
		/// Check if the request content type is application/json
		/// </summary>
		/// <returns></returns>
		public bool IsJsonRequest()
		{
			return _httpContext?.Request?.ContentType.EndsWith("json", StringComparison.InvariantCultureIgnoreCase) ?? false;
		}

		public bool IsScreamingFrog()
		{
			return _httpContext?.Request?.Headers["User-Agent"]?.Contains("Screaming Frog") ?? false;
		}

		public Dictionary<string, string> QueryString()
		{
			return _httpContext?.Request.QueryString.Keys.Cast<string>().OrderBy(k => k).ToDictionary(k => k.ToLower(), v => HttpContext.Current.Request.QueryString[v]);
		}


		public T GetFormObject<T>() where T : class, new()
		{
			var dictionary = FormToDictionary();
			return dictionary.ToObject<T>();
		}

		public List<T> GetListFormObject<T>() where T : class, new()
		{
			var dictionary = FormToDictionary();
			var keys = dictionary.Keys.ToList();

			var indexes = new Dictionary<Int32, T>();
			foreach (var key in keys)
			{
				if (!key.StartsWith("["))
					continue;

				var index = Int32.Parse(key.Substring(1, key.IndexOf(']') - 1));

				if (!indexes.ContainsKey(index))
					indexes.Add(index, null);
			}

			for (var i = 0; i < indexes.Count; i++)
			{
				var index = indexes.Keys.Skip(i).FirstOrDefault();
				indexes[index] = dictionary.Where(s => s.Key.StartsWith($"[{index}]")).ToDictionary(a => a.Key.Substring($"[{index}].".Length), b => b.Value).ToObject<T>();
			}


			return indexes.Values.ToList();
		}

		//TODO - Needs to be debugged with Multi Store with regard to Email
		public bool IsBotRequest() => _httpContext?.Request?.UserAgent?.ToLower().Contains("bot") ?? false;

		public string Header(string name)
		{
			var result = String.Empty;

			try
			{
				if (!IsRequestAvailable) return result;
				if (_httpContext.Request.Headers[name] != null) result = _httpContext.Request.Headers[name];
			}
			catch
			{
				result = String.Empty;
			}
			return result;
		}

		public void SetCacheability(HttpCacheability cacheability, int maxAgeSeconds = 900)
		{
			_httpContext.Response.Cache.SetCacheability(cacheability);
			_httpContext.Response.Cache.SetMaxAge(TimeSpan.FromSeconds(maxAgeSeconds));
		}

		#endregion

		#region Private Methods

		private String GetCookieName(String name)
		{
			var setName = ConfigurationManager.AppSettings[$"sf:CookieName_{name}"];
			if (String.IsNullOrWhiteSpace(setName)) return name;

			RemoveCookie(name);
			return setName;
		}

		private void RemoveCookie(string name)
		{
			CookieExpiryDate(name, DateTime.UtcNow.Date.AddDays(-1));
		}

		private void CookieExpiryDate(string name, DateTime expire)
		{
			if (_httpContext?.Request?.Cookies != null)
			{
				var cookie = _httpContext?.Request.Cookies[name];
				if (cookie != null)
				{
					var updateCookie = CreateCookie(name, cookie.Value, expire);
					_httpContext.Response.Cookies.Set(updateCookie);
					_httpContext.Request.Cookies.Set(updateCookie);
				}
			}
		}

		private Dictionary<String, String> FormToDictionary()
		{
			var form = _httpContext?.Request.Form;

			var dict = new Dictionary<String, String>();
			if (form == null)
				return dict;
			foreach (var key in form.AllKeys)
			{
				var val = form[key];
				dict.Add(key, val);
			}

			return dict;
		}

		private HttpCookie CreateCookie(string name, string value, DateTime? expiresOn = null, SameSiteMode mode = SameSiteMode.Strict, bool secure = true)
		{
			var cookie = new HttpCookie(name, value);
			cookie.Expires = expiresOn ?? cookie.Expires;
			cookie.SameSite = mode;
			cookie.Secure = secure || (mode == SameSiteMode.None);
			return cookie;
		}
		#endregion
	}
}
