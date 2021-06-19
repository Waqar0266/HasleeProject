using System;
using System.Data.Entity;

namespace Hasslefree.Core.Data
{
	public interface IModelExtender
	{
		/// <summary>
		/// Add customization to the model
		/// </summary>
		/// <param name="modelCustomization"></param>
		void Extend(Action<DbModelBuilder> modelCustomization);

		/// <summary>
		/// Apply extension customization to the model
		/// </summary>
		/// <param name="modelBuilder"></param>
		void ApplyExtensions(DbModelBuilder modelBuilder);
	}
}
