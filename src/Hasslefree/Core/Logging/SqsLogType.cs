// ReSharper disable InconsistentNaming
namespace Hasslefree.Core.Logging
{
	public enum SqsLogType
	{
		PreApplicationStart,
		Application_Start,
		Application_Error,
		Application_BeginRequest,
		Application_Init,
		Application_Disposed,
		Application_End,
		Application_EndRequest,
		Application_PreRequestHandlerExecute,
		Application_PostRequestHandlerExecute,
		Applcation_PreSendRequestHeaders,
		Application_PreSendContent,
		Application_AcquireRequestState,
		Application_ReleaseRequestState,
		Application_ResolveRequestCache,
		Application_UpdateRequestCache,
		Application_AuthenticateRequest,
		Application_AuthorizeRequest,
		Session_Start,
		Session_End,
		CategoryMenu
	}
}
