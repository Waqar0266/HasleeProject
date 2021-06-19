using Hasslefree.Core;
using Hasslefree.Core.Domain.Catalog;
using Hasslefree.Core.Domain.Common;
using Hasslefree.Core.Domain.Media;
using Hasslefree.Data;
using Hasslefree.Web.Models.Catalog.Categories.Get;
using Hasslefree.Web.Models.Media.Pictures;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Z.EntityFramework.Plus;

namespace Hasslefree.Services.Catalog.Categories.Crud
{
	public class GetAgentService : IGetAgentService
	{
		#region Private Properties

		// Repos
		private IDataRepository<Agent> AgentRepo { get; }
		private IDataRepository<Picture> PictureRepo { get; }

		#endregion

		#region Constructor

		public GetAgentService
		(
			IDataRepository<Agent> agentRepo,
			IDataRepository<Picture> pictureRepo
		)
		{
			// Repos
			AgentRepo = agentRepo;
			PictureRepo = pictureRepo;
		}

		#endregion

		#region IGetAgentService

		public AgentWarning Warning { get; private set; }

		public AgentGet this[int agentId, bool includeDates = true, bool includeProducts = false]
		{
			get
			{
				if (agentId <= 0) return AgentNotFound();

				var agent = AgentQuery(agentId);

				if (agent == null) return AgentNotFound();

				return new AgentGet
				{
					AgentId = agent.AgentId,
					CreatedOn = includeDates ? agent.CreatedOn : (DateTime?)null,
					ModifiedOn = includeDates ? agent.ModifiedOn : (DateTime?)null,
					Path = agent.Path,
					Name = agent.Name,
					Description = agent.Description,
					NestedLevel = agent.NestedLevel,
					DisplayOrder = agent.DisplayOrder,
					ParentAgentId = agent.ParentAgentId,
					Hidden = agent.Hidden,
					Tag = agent.Tag,
					Picture = agent.PictureId.HasValue && agent.Picture != null ? new PictureModel
					{
						PictureId = agent.PictureId.Value,
						CreatedOn = includeDates ? agent.Picture.CreatedOn : (DateTime?)null,
						ModifiedOn = includeDates ? agent.Picture.ModifiedOn : (DateTime?)null,
						Name = agent.Picture.Name,
						Path = agent.Picture.Path,
						AltText = agent.Picture.AltText,
						DisplayOrder = 0,
						FormatEnum = agent.Picture.FormatEnum,
						Transforms = agent.Picture.Transforms
					} : null
				};
			}
		}

		public QueryFutureValue<Agent> FutureValue(int agentId) => agentId <= 0 ? null : AgentRepo.Table.DeferredFirstOrDefault(c => c.AgentId == agentId).FutureValue();

		#endregion

		#region Private Methods

		private dynamic AgentNotFound()
		{
			Warning = new AgentWarning(AgentWarningCode.AgentNotFound);
			return null;
		}

		private Agent AgentQuery(int agentId)
		{
			var agentType = EntityType.Agent.ToString();

			var cFuture = (from c in AgentRepo.Table
						   where c.AgentId == agentId
						   select c).DeferredFirstOrDefault().FutureValue();

			var picFuture = (from c in AgentRepo.Table
							 where c.AgentId == agentId && c.PictureId.HasValue
							 join p in PictureRepo.Table on c.PictureId.Value equals p.PictureId
							 select p).DeferredFirstOrDefault().FutureValue();

			var agent = cFuture.Value;

			if (agent == null) return null;

			agent.Picture = agent.Picture ?? picFuture.Value;

			return agent;
		}

		#endregion
	}
}