using Hasslefree.Core.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace Hasslefree.Data
{
	public class ModelExtender : IModelExtender
	{
		#region Fields

		private List<Action<DbModelBuilder>> _modelCustomizations;

		#endregion

		#region IModelExtender

		/// <summary>
		/// Extend the Database with a Model Builder
		/// </summary>
		/// <param name="modelCustomization"></param>
		public void Extend(Action<DbModelBuilder> modelCustomization)
		{
			if(_modelCustomizations == null)
				_modelCustomizations = new List<Action<DbModelBuilder>>();

			_modelCustomizations.Add(modelCustomization);
		}

		/// <summary>
		/// Apply the extensions to the database model
		/// </summary>
		/// <param name="modelBuilder"></param>
		public void ApplyExtensions(DbModelBuilder modelBuilder)
		{
			if(_modelCustomizations == null)
				return;
			if(_modelCustomizations.Count == 0)
				return;

			// Invoke the model customization methods on the model builder
			foreach(var modelCustomization in _modelCustomizations)
				modelCustomization?.Invoke(modelBuilder);
		}

		#endregion
	}
}

