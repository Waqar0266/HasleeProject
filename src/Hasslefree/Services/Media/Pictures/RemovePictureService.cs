using Hasslefree.Core;
using Hasslefree.Core.Domain.Media;
using Hasslefree.Data;
using Hasslefree.Services.Infrastructure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using Hasslefree.Web.Models.Media.Pictures.Crud;

namespace Hasslefree.Services.Media.Pictures
{
	public class RemovePictureService : IRemovePictureService
	{
		#region Private Members

		// Data Repositories
		private IDataRepository<Picture> PictureRepo { get; }

		// Services
		private ICloudStorageService StorageService { get; }

		// Other
		private IDataContext Database { get; }

		private List<Picture> _pictures;

		#endregion

		#region Constructor

		public RemovePictureService(
				// Data Repositories
				IDataRepository<Picture> pictureRepo,
				// Services
				ICloudStorageService storageService,
				// Managers
				// Other
				IDataContext database
			)
		{
			// Data Repositories
			PictureRepo = pictureRepo;

			// Services
			StorageService = storageService;

			// Managers

			// Other
			Database = database;
		}

		#endregion

		#region IRemovePictureService

		/// <summary>
		/// Adds a single picture to the removal list
		/// </summary>
		/// <param name="picture"></param>
		/// <returns></returns>
		public IRemovePictureService Add(Int32 pictureId)
		{
			if (_pictures == null)
				_pictures = new List<Picture>();

			var picture = PictureRepo.Table.Where(p => p.PictureId == pictureId).FirstOrDefault();
			
			_pictures.Add(picture);

			return this;
		}

		/// <summary>
		/// Adds a single picture to the removal list
		/// </summary>
		/// <param name="picture"></param>
		/// <returns></returns>
		public IRemovePictureService Add(PictureModel model)
		{
			if (_pictures == null)
				_pictures = new List<Picture>();

			var picture = PictureRepo.Table.Where(p => p.PictureId == model.PictureId).FirstOrDefault();

			_pictures.Add(picture);

			return this;
		}

		/// <summary>
		/// Adds a range of pictures to the removal list
		/// </summary>
		/// <param name="pictures"></param>
		/// <returns></returns>
		public IRemovePictureService Add(IEnumerable<Int32> pictureIds)
		{
			if (_pictures == null)
				_pictures = new List<Picture>();

			pictureIds.ToList().ForEach(i =>
			{
				var picture = PictureRepo.Table.Where(p => p.PictureId == i).FirstOrDefault();
				if (picture != null) _pictures.Add(picture);
			});

			return this;
		}

		/// <summary>
		/// Adds a range of pictures to the removal list
		/// </summary>
		/// <param name="pictures"></param>
		/// <returns></returns>
		public IRemovePictureService Add(IEnumerable<PictureModel> pictures)
		{
			if (_pictures == null)
				_pictures = new List<Picture>();

			pictures.ToList().ForEach(i =>
			{
				var picture = PictureRepo.Table.Where(p => p.PictureId == i.PictureId).FirstOrDefault();
				if (picture != null) _pictures.Add(picture);
			});

			return this;
		}

		/// <summary>
		/// Banishes the picture from existence by removing entry in the repo and deleting file on s3
		/// </summary>
		public void Remove()
		{
			ProcessRemovePictures();
		}

		#endregion

		#region Private Methods

		private void ProcessRemovePictures()
		{
			//var bucketName = WebConfigurationManager.AppSettings["BucketName"];

			foreach (var picture in _pictures)
			{
				//Acquire model key
				//var key = pic.Key;

				//Resolve picture
				//var image = PictureRepo.Table.Where(p => p.PictureId == pic.PictureId).FirstOrDefault();

				//Delete picture from db
				PictureRepo.Delete(picture);

				//Delete picture from s3
				//StorageService.RemoveObject(key).WithBucket(bucketName).Process(); //S3 deletion unecessary
			}
		}

		#endregion
	}
}