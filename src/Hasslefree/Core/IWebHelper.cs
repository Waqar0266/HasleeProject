using System;
using System.Collections.Generic;
using System.Web;

namespace Hasslefree.Core
{
	/// <summary>
	/// Represents a common helper
	/// </summary>
	public interface IWebHelper
	{
		/// <summary>
		/// Checks if the request was initiated using XMLHttpRequest
		/// </summary>
		/// <returns></returns>
		bool IsAjaxRequest();

		/// <summary>
		/// Check if the request content type is application/json
		/// </summary>
		/// <returns></returns>
		bool IsJsonRequest();

		/// <summary>
		/// Check if the request user agent is Screaming Frog
		/// </summary>
		/// <returns></returns>
		bool IsScreamingFrog();

		/// <summary>
		/// Get URL referrer
		/// </summary>
		/// <returns>URL referrer</returns>
		string GetUrlReferrer();


		/// <summary>
		/// Gets server variable by name
		/// </summary>
		/// <param name="name">Name</param>
		/// <returns>Server variable</returns>
		string ServerVariables(string name);

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
		bool IsStaticResource(HttpRequest request);

		/// <summary>
		/// Maps a virtual path to a physical disk path.
		/// </summary>
		/// <param name="path">The path to map. E.g. "~/bin"</param>
		/// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
		string MapPath(string path);

		/// <summary>
		/// Returns true if the website is being run locally
		/// </summary>
		/// <returns></returns>
		bool IsLocal();

		/// <summary>
		/// Get the IP address from the request
		/// </summary>
		/// <returns></returns>
		string GetRequestIp();

		/// <summary>
		/// Get the protocol of the request
		/// </summary>
		/// <returns></returns>
		string GetRequestProtocol();

		/// <summary>
		/// Get the port from the request
		/// </summary>
		/// <returns></returns>
		string GetRequestPort();

		/// <summary>
		/// Get the hostname used in the request
		/// </summary>
		/// <returns></returns>
		string GetRequestHost();

		/// <summary>
		/// Create a new cookie with the provided details
		/// </summary>
		/// <param name="name">Name of the Cookie</param>
		/// <param name="value">String value of the cookie</param>
		/// <param name="expiresOn"></param>
		/// <param name="mode"></param>
		/// <param name="secure"></param>
		void SetCookieValue(string name, string value, DateTime? expiresOn = null, SameSiteMode mode = SameSiteMode.Strict, bool secure = true);

		/// <summary>
		/// Set the expire date for a cookie
		/// </summary>
		/// <param name="name"></param>
		/// <param name="expiresOn"></param>
		void SetCookieExpireDate(string name, DateTime expiresOn);

		/// <summary>
		/// Get the entire cookie
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		HttpCookie GetCookie(string name);

		/// <summary>
		/// Get the value of a cookie
		/// </summary>
		/// <param name="name">Name of the cookie</param>
		/// <returns></returns>
		string GetCookieValue(string name);

		/// <summary>
		/// Expire an existing cookie
		/// </summary>
		/// <param name="name"></param>
		void DeleteCookie(string name);

		Dictionary<string, string> QueryString();

		/// <summary>
		/// Get the form as a specified class
		/// </summary>
		/// <returns></returns>
		T GetFormObject<T>() where T : class, new();

		/// <summary>
		/// Get the form as a list of specified class
		/// </summary>
		/// <returns></returns>
		List<T> GetListFormObject<T>() where T : class, new();

		/// <summary>
		/// Return whether this is a bot making the request
		/// </summary>
		/// <returns></returns>
		bool IsBotRequest();

		/// <summary>
		/// Gets a specific header from the request
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		string Header(string name);

		void SetCacheability(HttpCacheability cacheability, int maxAgeSeconds = 900);
	}
}
