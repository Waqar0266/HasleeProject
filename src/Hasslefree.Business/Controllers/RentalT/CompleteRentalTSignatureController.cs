using Hasslefree.Core;
using Hasslefree.Core.Domain.Rentals;
using Hasslefree.Core.Helpers.Extensions;
using Hasslefree.Core.Logging;
using Hasslefree.Core.Sessions;
using Hasslefree.Services.Accounts.Actions;
using Hasslefree.Services.Common;
using Hasslefree.Services.Emails;
using Hasslefree.Services.Media.Downloads;
using Hasslefree.Services.Media.Pictures;
using Hasslefree.Services.RentalTs.Crud;
using Hasslefree.Web.Framework;
using Hasslefree.Web.Framework.Filters;
using Hasslefree.Web.Models.RentalTs;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Hasslefree.Business.Controllers.RentalT
{
    public class CompleteRentalTSignatureController : BaseController
    {
        #region Private Properties 

        // Services
        private IUpdateRentalTService UpdateRentalService { get; }
        private IUpdateTenantService UpdateTenantService { get; }
        private IUploadPictureService UploadPicture { get; }
        private IUploadDownloadService UploadDownload { get; }
        private IGetFirmService GetFirmService { get; }
        private ILogoutService LogoutService { get; }
        private ISendMail SendMail { get; }
        private IGetRentalTService GetRental { get; }

        // Other
        private IWebHelper WebHelper { get; }
        private ISessionManager SessionManager { get; }

        #endregion

        #region Constructor

        public CompleteRentalTSignatureController
        (
            //Services
            IUpdateRentalTService updateRentalService,
            IUpdateTenantService updateTenantService,
            IUploadPictureService uploadPicture,
            IUploadDownloadService uploadDownload,
            IGetFirmService getFirmService,
            ILogoutService logoutService,
            ISendMail sendMail,
            IGetRentalTService getRental,

            //Other
            IWebHelper webHelper,
            ISessionManager sessionManager
        )
        {
            // Services
            UpdateRentalService = updateRentalService;
            UpdateTenantService = updateTenantService;
            UploadPicture = uploadPicture;
            UploadDownload = uploadDownload;
            GetFirmService = getFirmService;
            LogoutService = logoutService;
            SendMail = sendMail;
            GetRental = getRental;

            // Other
            WebHelper = webHelper;
            SessionManager = sessionManager;
        }

        #endregion

        #region Actions

        [HttpGet, Route("account/rentalt/complete-signature")]
        [AccessControlFilter]
        public ActionResult CompleteTenantSignature(string hash)
        {
            string decodedHash = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(hash));

            int rentalTId = Int32.Parse(decodedHash.Split(';')[0]);
            int tenantId = Int32.Parse(decodedHash.Split(';')[0]);

            var rental = GetRental[rentalTId].Get();

            if (rental.Status != RentalTStatus.PendingTenantSignature) return Redirect($"/account/tenants");

            var model = new CompleteRentalTSignature
            {
                RentalTId = rentalTId,
                TenantId = tenantId
            };

            ViewBag.Title = "Complete Tenant Signature";

            // Ajax
            if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/RentalTs/CompleteTenantSignature", model);

            // Default
            return View("../Rentals/RentalTs/CompleteTenantSignature", model);
        }
        
        [HttpPost, Route("account/rentalt/complete-signature")]
        public ActionResult CompleteLandlordSignature(CompleteRentalTSignature model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var rental = GetRental[model.RentalTId].Get();

                    var tenant = rental.Tenants.FirstOrDefault(a => a.TenantId == model.TenantId);

                    //add the signatures
                    UploadPicture.WithPath("signatures");

                    var signatureData = RemoveWhitespace(model.Signature);

                    var key = $"{tenant.Person.FirstName.ToLower().Replace(" ", "-")}_{tenant.Person.Surname.ToLower().Replace(" ", "-")}_{tenant.UniqueId}";
                    
                    UploadPicture.Add(new Web.Models.Media.Pictures.Crud.PictureModel()
                    {
                        Action = Web.Models.Common.CrudAction.Create,
                        File = signatureData,
                        Format = Core.Domain.Media.PictureFormat.Png,
                        Key = $"{key}_signature.png",
                        Name = $"{key}_signature.png",
                        MimeType = "image/png",
                        AlternateText = $"{key}_signature.png"
                    });

                    UploadPicture.Add(new Web.Models.Media.Pictures.Crud.PictureModel()
                    {
                        Action = Web.Models.Common.CrudAction.Create,
                        File = RemoveWhitespace(model.Initials),
                        Format = Core.Domain.Media.PictureFormat.Png,
                        Key = $"{key}_initial.png",
                        Name = $"{key}_initial.png",
                        MimeType = "image/png",
                        AlternateText = $"{key}_initial.png"
                    });

                    var pictures = UploadPicture.Save();

                    var success = UpdateTenantService[tenant.TenantId]
                    .Set(a => a.SignedAt, model.SignedAtSignature)
                    .Set(a => a.SignedOn, DateTime.Now)
                    .Set(a => a.SignatureId, pictures.FirstOrDefault(p => p.Name == $"{key}_signature.png").PictureId)
                    .Set(a => a.InitialsId, pictures.FirstOrDefault(p => p.Name == $"{key}_initial.png").PictureId)
                    .Update();

                    if (rental.RentalTType.ToString().ToLower().StartsWith("natural"))
                    {
                        success = UpdateRentalService[rental.RentalTId]
                        .Set(a => a.RentalTStatus, RentalTStatus.PendingNew)
                        .Update();

                        SendAgentEmail(rental.Rental.Agent.Person.Email, rental.RentalTId);
                    }
                    else
                    {
                        //foreach (var member in rental.RentalResolution.Members)
                        //    SendMemberSignatureEmail(member.Email, rental.RentalId, member.RentalResolutionMemberId);

                        //success = UpdateRentalService[rental.RentalTId]
                        //.Set(a => a.RentalTStatus, RentalTStatus.PendingNew)
                        //.Update();
                    }

                    // Success
                    if (success)
                    {
                        // Ajax (+ Json)
                        if (WebHelper.IsAjaxRequest() || WebHelper.IsJsonRequest()) return Json(new
                        {
                            Success = true,
                            AgentId = 1,
                        }, JsonRequestBehavior.AllowGet);

                        // Default
                        return Redirect($"/account/tenants");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                while (ex.InnerException != null) ex = ex.InnerException;
                ModelState.AddModelError("", ex.Message);
            }

            var errors = "";

            //if (UpdateAgentService.HasWarnings) UpdateAgentService.Warnings.ForEach(w => errors += w.Message + "\n");
            //if (CreateAgentForm.HasWarnings) CreateAgentForm.Warnings.ForEach(w => errors += w.Message + "\n");

            ModelState.AddModelError("", errors);

            ViewBag.Title = "Complete Tenant Signature";

            // Ajax (Json)
            if (WebHelper.IsJsonRequest()) return Json(new
            {
                Success = false,
                Message = errors ?? "Unexpected error has occurred."
            }, JsonRequestBehavior.AllowGet);

            // Ajax
            if (WebHelper.IsAjaxRequest()) return PartialView("../Rentals/RentalTs/CompleteTenantSignature", model);

            // Default
            return View("../Rentals/RentalTs/CompleteTenantSignature", model);
        }
        
        #endregion

        #region Private Methods

        private static byte[] RemoveWhitespace(string base64)
        {
            Bitmap bmp;
            byte[] imageAsBytes = Convert.FromBase64String(base64.Replace("data:image/png;base64,", ""));
            using (var ms = new MemoryStream(imageAsBytes))
            {
                var image = Bitmap.FromStream(ms);
                bmp = new Bitmap(image);
            }

            int w = bmp.Width;
            int h = bmp.Height;

            Func<int, bool> allWhiteRow = row =>
            {
                for (int i = 0; i < w; ++i)
                {
                    if (bmp.GetPixel(i, row).R != 255)
                        return false;
                }
                return true;
            };

            Func<int, bool> allWhiteColumn = col =>
            {
                for (int i = 0; i < h; ++i)
                {
                    if (bmp.GetPixel(col, i).R != 255)
                        return false;
                }
                return true;
            };

            int topmost = 0;
            for (int row = 0; row < h; ++row)
            {
                if (allWhiteRow(row))
                    topmost = row;
                else break;
            }

            int bottommost = 0;
            for (int row = h - 1; row >= 0; --row)
            {
                if (allWhiteRow(row))
                    bottommost = row;
                else break;
            }

            int leftmost = 0, rightmost = 0;
            for (int col = 0; col < w; ++col)
            {
                if (allWhiteColumn(col))
                    leftmost = col;
                else
                    break;
            }

            for (int col = w - 1; col >= 0; --col)
            {
                if (allWhiteColumn(col))
                    rightmost = col;
                else
                    break;
            }

            if (rightmost == 0) rightmost = w; // As reached left
            if (bottommost == 0) bottommost = h; // As reached top.

            int croppedWidth = rightmost - leftmost;
            int croppedHeight = bottommost - topmost;

            if (croppedWidth == 0) // No border on left or right
            {
                leftmost = 0;
                croppedWidth = w;
            }

            if (croppedHeight == 0) // No border on top or bottom
            {
                topmost = 0;
                croppedHeight = h;
            }

            try
            {
                var target = new Bitmap(croppedWidth, croppedHeight);
                using (Graphics g = Graphics.FromImage(target))
                {
                    g.DrawImage(bmp,
                      new RectangleF(0, 0, croppedWidth, croppedHeight),
                      new RectangleF(leftmost, topmost, croppedWidth, croppedHeight),
                      GraphicsUnit.Pixel);
                }

                target.MakeTransparent();

                return target.ToByteArray(System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception ex)
            {
                throw new Exception(
                  string.Format("Values are topmost={0} btm={1} left={2} right={3} croppedWidth={4} croppedHeight={5}", topmost, bottommost, leftmost, rightmost, croppedWidth, croppedHeight),
                  ex);
            }
        }

        private bool SendAgentEmail(string email, int rentalTId)
        {
            var url = $"account/rentals/emails/rental-tenant-agent-documentation-email?rentalId={rentalTId}";

            SendMail.WithUrlBody(url).WithRecipient(email);

            return SendMail.Send("Pre-Approval Rental - Agent Documentation");
        }

        #endregion
    }
}