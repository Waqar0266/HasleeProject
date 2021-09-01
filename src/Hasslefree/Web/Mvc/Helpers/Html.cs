using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;
using Hasslefree.Core;
using Hasslefree.Services.Cache;
using static System.String;
using Hasslefree.Core.Sessions;

namespace Hasslefree.Web.Mvc.Helpers
{
	public static class Html
	{
		private const string CssViewDataName = "RenderStyle";

		public static MvcHtmlString RenderStyles(this HtmlHelper htmlHelper)
		{
			var result = new StringBuilder();

			var styleList = htmlHelper.ViewContext.HttpContext.Items[CssViewDataName] as List<string>;

			if (styleList != null)
				foreach (string script in styleList)
					result.AppendLine($"<link href=\"{script}\" rel=\"stylesheet\" type=\"text/css\" />");

			return MvcHtmlString.Create(result.ToString());
		}

		public static RouteValueDictionary AddIf(this RouteValueDictionary dict, bool condition, string name, object value)
		{
			if (condition) dict.Add(name, value);
			return dict;
		}

		public static IDictionary<string, object> AddProperty(this object obj, bool condition, string name, object value)
		{
			var dictionary = obj.ToDictionary();
			if (condition) dictionary.Add(name, value);
			return dictionary;
		}

		// helper
		public static IDictionary<string, object> ToDictionary(this object obj)
		{
			var result = new Dictionary<string, object>();
			var properties = TypeDescriptor.GetProperties(obj);
			foreach (PropertyDescriptor property in properties) result.Add(property.Name, property.GetValue(obj));
			return result;
		}

		public static void AddStyle(this HtmlHelper htmlHelper, string styleUrl)
		{
			var styleList = htmlHelper.ViewContext.HttpContext.Items[CssViewDataName] as List<string>;

			if (styleList != null)
			{
				if (!styleList.Contains(styleUrl))
				{
					styleList.Add(styleUrl);
				}
			}
			else
			{
				styleList = new List<string> { styleUrl };
				htmlHelper.ViewContext.HttpContext.Items.Add(CssViewDataName, styleList);
			}
		}

		public static void AddJavaScript(this HtmlHelper htmlHelper, string scriptUrl)
		{
			var scriptList = htmlHelper.ViewContext.HttpContext.Items["JavaScripts"] as List<string>;
			if (scriptList != null)
			{
				if (!scriptList.Contains(scriptUrl))
				{
					scriptList.Add(scriptUrl);
				}
			}
			else
			{
				scriptList = new List<string> { scriptUrl };
				htmlHelper.ViewContext.HttpContext.Items.Add("JavaScripts", scriptList);
			}
		}

		public static string SplitCamelCase(this string input) => Regex.Replace(input, "([A-Z])", " $1", RegexOptions.Compiled).Trim();

		public static void RegisterJavaScripts(this HtmlHelper hh, string url)
		{
			var scripts = hh.ViewContext.HttpContext.Items["JavaScripts"] as List<string>;
			if (scripts != null)
			{
				if (!scripts.Contains(url))
				{
					scripts.Add(url);
				}
			}
			else
			{
				scripts = new List<string> { url };
				hh.ViewContext.HttpContext.Items.Add("JavaScripts", scripts);
			}
		}

		public static void RegisterJavaScriptsAsync(this HtmlHelper hh, string url)
		{
			var scripts = hh.ViewContext.HttpContext.Items["JavaScriptsAsync"] as List<String>;
			if (scripts != null)
			{
				if (!scripts.Contains(url))
				{
					scripts.Add(url);
				}
			}
			else
			{
				scripts = new List<string> { url };
				hh.ViewContext.HttpContext.Items.Add("JavaScriptsAsync", scripts);
			}
		}

		public static void RegisterJavaScriptsDefer(this HtmlHelper hh, string url)
		{
			var scripts = hh.ViewContext.HttpContext.Items["JavaScriptsDefer"] as List<string>;
			if (scripts != null)
			{
				if (!scripts.Contains(url))
				{
					scripts.Add(url);
				}
			}
			else
			{
				scripts = new List<string> { url };
				hh.ViewContext.HttpContext.Items.Add("JavaScriptsDefer", scripts);
			}
		}

		public static void RegisterJavaScriptsBlock(this HtmlHelper hh, String script)
		{
			var scripts = hh.ViewContext.HttpContext.Items["JavaScriptsBlock"] as List<String>;
			if (scripts != null)
			{
				if (!scripts.Contains(script))
				{
					scripts.Add(script);
				}
			}
			else
			{
				scripts = new List<string> { script };
				hh.ViewContext.HttpContext.Items.Add("JavaScriptsBlock", scripts);
			}
		}

		public static MvcHtmlString RenderJavaScripts(this HtmlHelper hh)
		{
			var scripts = hh.ViewContext.HttpContext.Items["JavaScripts"] as List<string>;
			if (scripts == null) return MvcHtmlString.Create("");

			var result = new StringBuilder();
			scripts.Reverse();
			scripts.Aggregate(result, (sb, i) => { sb.AppendLine($"<script type=\"text/javascript\" src=\"{i}\"></script>"); return sb; });

			return MvcHtmlString.Create(result.ToString());
		}

		public static MvcHtmlString RenderJavaScriptsAsync(this HtmlHelper hh)
		{
			var scripts = hh.ViewContext.HttpContext.Items["JavaScriptsAsync"] as List<String>;
			if (scripts == null) return MvcHtmlString.Create(String.Empty);

			var result = new StringBuilder();
			scripts.Reverse();
			scripts.Aggregate(result, (sb, i) => { sb.AppendLine($"<script type=\"text/javascript\" src=\"{i}\" async></script>"); return sb; });

			return MvcHtmlString.Create(result.ToString());
		}

		public static MvcHtmlString RenderJavaScriptsDefer(this HtmlHelper hh)
		{
			var scripts = hh.ViewContext.HttpContext.Items["JavaScriptsDefer"] as List<String>;
			if (scripts == null) return MvcHtmlString.Create("");

			var result = new StringBuilder();
			scripts.Reverse();
			scripts.Aggregate(result, (sb, i) => { sb.AppendLine($"<script type=\"text/javascript\" src=\"{i}\" defer></script>"); return sb; });

			return MvcHtmlString.Create(result.ToString());
		}

		public static MvcHtmlString RenderJavaScriptsBlock(this HtmlHelper hh)
		{
			var scripts = hh.ViewContext.HttpContext.Items["JavaScriptsBlock"] as List<string>;
			if (scripts == null) return MvcHtmlString.Create("");

			var result = new StringBuilder();
			scripts.Reverse();
			scripts.Aggregate(result, (sb, i) => { sb.AppendLine($"{i}"); return sb; });

			return MvcHtmlString.Create(result.ToString());
		}

		[SuppressMessage("ReSharper", "Mvc.ControllerNotResolved")]
		[SuppressMessage("ReSharper", "Mvc.ActionNotResolved")]
		public static string AreaContent(this UrlHelper urlHelper, string resourceName)
		{
			var areaName = (string)urlHelper.RequestContext.RouteData.DataTokens["area"];
			return urlHelper.Action("Index", "Resource", new { resourceName, area = areaName });
		}

		public static string SlugifyUrl(this string url)
		{
			//First to lower case 
			url = url.ToLowerInvariant();

			//Remove all accents
			var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(url);

			url = Encoding.ASCII.GetString(bytes);

			//Replace spaces 
			url = Regex.Replace(url, @"\s", "-", RegexOptions.Compiled);

			//Remove invalid chars 
			url = Regex.Replace(url, @"[^\w\s\p{Pd}/]", "", RegexOptions.Compiled);

			//Trim dashes from end 
			url = url.Trim('-', '_');

			//Replace double occurences of - or \_ 
			url = Regex.Replace(url, @"([-_]){2,}", "-", RegexOptions.Compiled);

			return url.Replace("/", "-").Replace("--", "-").Replace("--", "-");
		}

		/// <summary>
		/// Replace all special characters including spaces in the specified text with hyphens.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="preserveLength"></param>
		/// <param name="ignoreCharacters"></param>
		/// <returns></returns>
		public static string ToKebabCase(this string text, bool preserveLength = false, char[] ignoreCharacters = null)
		{
			var pattern = preserveLength ? "[^a-zA-Z0-9{0}]" : "[^a-zA-Z0-9{0}]+";

			pattern = string.Format(pattern, string.Join("", ignoreCharacters ?? "".ToArray()));

			return Regex.Replace(text, pattern, "-", RegexOptions.Compiled);
		}

		/// <summary>
		/// Replace all special characters including spaces in the specified text with hyphens and all uppercase characters with lowercase characters.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="preserveLength"></param>
		/// <param name="ignoreCharacters"></param>
		/// <returns></returns>
		public static string ToLowerKebabCase(this string text, bool preserveLength = false, char[] ignoreCharacters = null) => text.ToKebabCase(preserveLength, ignoreCharacters).ToLower();

		public static MvcHtmlString MvcCodeRouteAction(this HtmlHelper hh, string route, object routeValues = null)
		{
			var helperReq = hh.ViewContext.HttpContext.Request;
			var prefix = $"{helperReq.Url?.Scheme}://{helperReq.Url?.Authority}/";
			var request = new HttpRequest(null, prefix + route, null);
			var response = new HttpResponse(new StringWriter());
			var httpContext = new HttpContext(request, response);
			var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
			var values = routeData?.Values ?? new RouteValueDictionary();
			var controllerName = values["controller"].ToString();
			var actionName = values["action"].ToString();
			var actionIndex = route.ToLower().IndexOf(actionName.ToLower(), StringComparison.CurrentCultureIgnoreCase);
			var routeContext = actionIndex > 0 ? (route.StartsWith("/") ?
									route.Substring(1, actionIndex - 2) :
									route.Substring(0, actionIndex - 1)) :
								route.StartsWith("/") ? route.Substring(1) : route;

			var routeValueDictionary = new RouteValueDictionary(routeValues)
			{
				{ "area", "" },
				{ "__routecontext", routeContext }
			};

			return hh.Action(actionName, controllerName, routeValueDictionary);
		}

		/// <summary>
		/// Render a partial view for a specific complex type
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="helper"></param>
		/// <param name="expression"></param>
		/// <param name="partialViewName"></param>
		/// <returns></returns>
		public static MvcHtmlString PartialFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, string partialViewName)
		{
			string name = ExpressionHelper.GetExpressionText(expression);
			object model = ModelMetadata.FromLambdaExpression(expression, helper.ViewData).Model;
			var viewData = new ViewDataDictionary(helper.ViewData)
			{
				TemplateInfo = new TemplateInfo { HtmlFieldPrefix = name }
			};
			return helper.Partial(partialViewName, model, viewData);
		}

		/// <summary>
		/// Render a partial view for a specific complex type
		/// </summary>
		/// <typeparam name="TModel"></typeparam>
		/// <typeparam name="TProperty"></typeparam>
		/// <param name="helper"></param>
		/// <param name="expression"></param>
		/// <param name="partialViewName"></param>
		/// <returns></returns>
		public static MvcHtmlString PartialForEach<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, string partialViewName) where TProperty : IEnumerable
		{
			var name = ExpressionHelper.GetExpressionText(expression);
			var model = (IEnumerable)ModelMetadata.FromLambdaExpression(expression, helper.ViewData).Model;
			var i = 0;
			var partials = (from object item in model
							let viewData = new ViewDataDictionary(helper.ViewData)
							{
								TemplateInfo = new TemplateInfo { HtmlFieldPrefix = name + "[" + i++ + "]" }
							}
							select helper.Partial(partialViewName, item, viewData)).ToList();

			return new MvcHtmlString(Concat(partials));
		}

		#region Editable For

		public static MvcHtmlString Editable<TModel>(this HtmlHelper<TModel> helper, String name)
		{
			return Editable(helper, name, null, new RouteValueDictionary());
		}

		public static MvcHtmlString Editable<TModel>(this HtmlHelper<TModel> helper, String name, object value)
		{
			return Editable(helper, name, value, new RouteValueDictionary());
		}

		public static MvcHtmlString Editable<TModel>(this HtmlHelper<TModel> helper, String name, object value, object htmlAttributes)
		{
			var dict = new RouteValueDictionary(htmlAttributes);
			while (dict.Any(d => d.Key.Contains("_")))
			{
				var item = dict.FirstOrDefault(d => d.Key.Contains("_"));
				dict.Remove(item.Key);
				dict.Add(item.Key.Replace("_", "-"), item.Value);
			}
			return Editable(helper, name, value, dict);
		}

		public static MvcHtmlString Editable<TModel>(this HtmlHelper<TModel> helper, String name, object value, IDictionary<string, object> htmlAttributes)
		{
			var prefix = helper.ViewData.TemplateInfo.HtmlFieldPrefix;
			if (!IsNullOrWhiteSpace(prefix)) name = $"{prefix}.{name}";

			var builder = new TagBuilder("span");

			var aTag = EditableATag(name, value.ToString(), typeof(String), htmlAttributes);
			var hiddenInput = EditableHiddenInput(name, value.ToString());

			builder.AddCssClass("settings-inline");
			builder.InnerHtml = aTag + hiddenInput;

			return new MvcHtmlString(builder.ToString(TagRenderMode.Normal));
		}

		public static MvcHtmlString EditableFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression)
		{
			return EditableFor(helper, expression, new RouteValueDictionary());
		}

		public static MvcHtmlString EditableFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
		{
			var dict = new RouteValueDictionary(htmlAttributes);
			while (dict.Any(d => d.Key.Contains("_")))
			{
				var item = dict.FirstOrDefault(d => d.Key.Contains("_"));
				dict.Remove(item.Key);
				dict.Add(item.Key.Replace("_", "-"), item.Value);
			}
			return EditableFor(helper, expression, dict);
		}

		public static MvcHtmlString EditableFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
		{
			var prefix = helper.ViewData.TemplateInfo.HtmlFieldPrefix;
			var name = ExpressionHelper.GetExpressionText(expression);
			if (!String.IsNullOrWhiteSpace(prefix)) name = $"{prefix}.{name}";
			var value = ModelMetadata.FromLambdaExpression(expression, helper.ViewData).Model?.ToString() ?? "";
			var type = typeof(TProperty);

			var builder = new TagBuilder("span");

			var aTag = EditableATag(name, value, type, htmlAttributes);
			var hiddenInput = EditableHiddenInput(name, value);

			builder.AddCssClass("settings-inline");
			builder.InnerHtml = aTag + hiddenInput;

			return new MvcHtmlString(builder.ToString(TagRenderMode.Normal));
		}

		private static string EditableATag(string name, string value, Type type, IDictionary<string, object> htmlAttributes)
		{
			var builder = new TagBuilder("a");

			builder.MergeAttribute("href", "#");
			builder.AddCssClass("inline-link");

			if (htmlAttributes != null)
			{
				if (htmlAttributes.ContainsKey("class"))
					foreach (var cl in htmlAttributes["class"].ToString().Split(' '))
						builder.AddCssClass(cl);

				if (htmlAttributes.Any(a => a.Key.StartsWith("data-", StringComparison.InvariantCultureIgnoreCase)))
					foreach (var data in htmlAttributes.Where(a => a.Key.StartsWith("data-", StringComparison.InvariantCultureIgnoreCase) && a.Key != "data-name" && a.Key != "data-value"))
						builder.MergeAttribute(data.Key, data.Value.ToString());
			}
			builder.MergeAttribute("data-name", name);

			if (!builder.Attributes.ContainsKey("data-type"))
			{
				if (type == typeof(decimal))
				{
					builder.MergeAttribute("data-type", "number");
					if (!builder.Attributes.ContainsKey("data-step"))
						builder.MergeAttribute("data-step", "any");
					if (!builder.Attributes.ContainsKey("data-emptytext"))
						builder.MergeAttribute("data-emptytext", "0");
				}
				else if (type == typeof(string))
				{
					builder.MergeAttribute("data-type", "text");
					if (!builder.Attributes.ContainsKey("data-emptytext"))
						builder.MergeAttribute("data-emptytext", "empty");
				}
			}
			builder.MergeAttribute("data-value", value);
			builder.SetInnerText(value);

			return builder.ToString(TagRenderMode.Normal);
		}

		private static string EditableHiddenInput(string name, string value)
		{
			var builder = new TagBuilder("input");
			builder.MergeAttribute("type", "hidden");
			builder.MergeAttribute("name", name);
			builder.MergeAttribute("value", value);

			return builder.ToString(TagRenderMode.SelfClosing);
		}

		#endregion

		#region Content

		public static MvcHtmlString CacheHtml(this HtmlHelper hh, String key, HelperResult result)
		{
			var loggedIn = Core.Infrastructure.EngineContext.Current.Resolve<ISessionManager>().IsLoggedIn();
			var cache = CacheManager.Current;

			return MvcHtmlString.Create(result.ToHtmlString());
		}

		#endregion
	}
}