namespace Hasslefree.Core.Data
{
	public interface IExtensibleModelRegistrar
	{
		/// <summary>
		/// Register the model changes you want
		/// </summary>
		/// <param name="modelExtender"></param>
		void Register(IModelExtender modelExtender);
	}
}
